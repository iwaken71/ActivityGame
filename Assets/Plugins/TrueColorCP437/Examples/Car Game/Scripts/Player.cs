using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

#region Constants
	public	const int	FREE	= 0;
	public	const int	STUN	= 1;
#endregion
	
#region Instance Variables
	// ---=== Status : ===---
	[HideInInspector]
	public	int		status			= -1;
	
	// ---=== Size : ===---
	public	Box		spriteBox;
	
	// ---=== Movement : ===---
	[HideInInspector]
	public	Vector3	position;
	[HideInInspector]
	public	float	halfWidth;
	[HideInInspector]
	public	float	halfHeight;
	[HideInInspector]
	public	float	speed;
	[HideInInspector]
	public	float	angle;
	[HideInInspector]
	public	Vector3	direction;
	[HideInInspector]
	public	Vector3	currentMovement			= Vector3.zero;
	public	float	steeringRate			= 0.25f;
	public	float	glideThreshold;
	private	float	a, b;
	public	float	steeringAttenuation;
	public	float	acceleration			= 0.5f;
	public	float	deceleration			= 0.25f;
	public	float	breakRate				= 1.0f;
	public	float	maxSpeed				= 10.0f;
	
	// ---=== Collisions : ===---
	public	Rect	hitbox;
	private	int		lastX, lastY;
	public	float	spinningRate			= 2.0f;
	private	bool	spinning;
	
	// ---=== Stun : ===---
	public	float	stunDuration			=  0.2f;		// in seconds
	[HideInInspector]
	public	float	lastStunTime			= -1.0f;
	
	// ---=== Animation : ===---
	public	ConsoleAnimation	run;
	[HideInInspector]
	public	ConsoleAnimation	currentAnimation;
	public	float				animationSpeed;
	[HideInInspector]
	public	IntRect				frame;
	[HideInInspector]
	public	float				animationTime;
	
	// ---=== Sound : ===---
	public	AudioSource	engine;
	public	AudioSource	glide;
	public	AudioSource	offRoad;
	
	// ---=== Environment : ===---
	public	Game		game;
	
	// ---=== Effects : ===---
	public	float		dustSpawnRate;
	private	float		lastDustSpawnTime;
#endregion
	
#region Initialize
	public void Setup ( Vector3 position ) {
		
		a				= ( glideThreshold * ( steeringAttenuation - 1 ) ) / ( maxSpeed * ( glideThreshold - 1 ) );
		b				= steeringAttenuation - a * maxSpeed;
		
		Reset ( position, 90.0f );
		
	}
#endregion
	
#region Status
	public void Reset ( Vector3 position, float angle ) {
		
		status				= FREE;
		
		this.position		= position;
		this.angle			= angle;
		
		speed				= 0.0f;
		
		spinning			= false;
		
		currentAnimation	= run;
		frame				= run.frames[0];
		animationTime		= 0.0f;

	}
#endregion
	
#region Logic
	public void UpdateLogic () {

		int		nextTileX, nextTileY, respawnX, respawnY;
		float	h, v;

		/*****************************************************************************************************************************
		 * DISPLACEMENT VECTOR :
		 *****************************************************************************************************************************/

		switch ( status ) {

			// Moving freely :
			case FREE:

				// INPUT :
				h = Input.GetAxisRaw ( "Horizontal" );
				v = Input.GetAxisRaw ( "Vertical" );

				if			( h < 0.0f )	angle += AttenuatedSteering () * Time.deltaTime;
				else if 	( h > 0.0f )	angle -= AttenuatedSteering () * Time.deltaTime;
				
				if			( v <  0.0f )	speed -= breakRate * Time.deltaTime;
				else if		( v == 0.0f )	speed -= deceleration * Time.deltaTime;
				else if 	( v >  0.0f )	speed += acceleration * Time.deltaTime;
			
				speed = Mathf.Clamp ( speed, 0.0f, maxSpeed );
			
				// CALCULATE MOTION :
				direction			= Quaternion.Euler ( 0.0f, 0.0f, angle ) * Vector3.right;
			
				currentMovement.x	= direction.x * speed * Time.deltaTime;
				currentMovement.y	= direction.y * speed * Time.deltaTime;
				currentMovement.z	= 0.0f;
			
				// SOUND :
				if ( ( speed > 0.05f * maxSpeed ) && ( engine.isPlaying == false ) )
					engine.Play ();
			
				if (	( speed >  maxSpeed / glideThreshold )	&&
						(     h != 0.0f						 )	) {
				
					if ( Time.time - lastDustSpawnTime > dustSpawnRate ) {
						
						game.InstantiateDust ( position );
						game.InstantiateDust ( new Vector3 ( position.x - spriteBox.halfWidth, position.y, position.z ) );

						lastDustSpawnTime = Time.time;

					}
				
					if	( glide.isPlaying == false ) glide.Play ();
				
				}
			
				if ( h == 0.0f ) glide.Stop ();
			
				break;

			// Stunned :
			case STUN:
			
				if ( Time.time - lastStunTime > stunDuration ) {
				
					status		= FREE;
					spinning	= false;
				
					respawnY	= lastY - 4;;
					respawnX	= FindEmptySpace ( lastX, respawnY );
				
					Reset ( new Vector3 ( respawnX, respawnY + game.progress * Game.ROAD_TILE_SIZE, 0.0f ), 90.0f );
				
				}
				else {
					currentMovement.x	= direction.x * ( speed / 5.0f ) * Time.deltaTime;
					currentMovement.y	= direction.y * ( speed / 5.0f ) * Time.deltaTime;
					currentMovement.z	= 0.0f;
				
					angle += spinningRate * Time.deltaTime;
				}

				break;
					
		}


		/*****************************************************************************************************************************
		 * MOVING ( finally ) :
		 *****************************************************************************************************************************/
		
		// ---=== Really moving : ===---
		position += currentMovement;
		position.z = 0.0f;

	
		// ---=== Collision witht the side of the road : ===---

		// The coordinates of the tile the player will be in in the NEXT frame IF there is no collision :
		nextTileX	= Mathf.FloorToInt ( position.x );
		nextTileY	= Mathf.FloorToInt ( position.y ) - game.progress * Game.ROAD_TILE_SIZE;

		// Acting if necessary :
		if ( spinning == false )
			ResolveCollisionsWithRoad ( currentMovement, nextTileX, nextTileY );
		
	}
	
	private float AttenuatedSteering () {
		
		if ( speed < maxSpeed / glideThreshold )
			return steeringRate;
		else
			return steeringRate / ( a * speed + b );
		
	}
#endregion
	
#region Collision
	public void ResolveCollisionsWithRoad ( Vector3 movement, int nextTileX, int nextTileY ) {

		if ( game.roadTiles[nextTileX,nextTileY].type == Game.BLOCK_TILE ) {
			
			status 			= STUN;
			
			spinning		= true;
			lastX			= nextTileX;
			lastY			= nextTileY;
				
			lastStunTime	= Time.time;
			
			offRoad.Play ();
			
		}
		
	}
	
	public int FindEmptySpace ( int x, int y ) {
		
		int i, minX, maxX;
		
		minX	= Game.ROAD_DISPLAY_WIDTH - 1;
		maxX	= 0;
		
		for ( i = 0; i < Game.ROAD_DISPLAY_WIDTH; i++ ) {
			if ( game.roadTiles[i,y].type == 0 ) {
				if ( i < minX )	minX = i;
				if ( i > maxX )	maxX = i;
			}
		}
		
		if ( lastX <= Game.ROAD_DISPLAY_WIDTH / 2 )
			return minX + 4;
		else
			return maxX - 4;
		
	}
#endregion
	
#region Display
	public IntRect UpdateFrame () {
		
		animationTime += Time.deltaTime;
		
		if ( ( animationTime >= animationSpeed ) && ( speed > 0.05f ) ) {
			currentAnimation.Play ();
			animationTime	= 0.0f;
		}
		
		return frame;
		
	}
#endregion
	
}
