using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	
#region Constants
	public	const int	ROAD_TILE_SIZE			= 8;
	public	const int	ROAD_PIECES_WIDTH		= 5;
	public	const int	ROAD_PIECES_HEIGHT		= 5;
	public	const int	ROAD_SECTION_HEIGHT		= 10;
	public	const int	ROAD_DISPLAY_WIDTH		= ROAD_PIECES_WIDTH  * ROAD_TILE_SIZE;
	public	const int	ROAD_DISPLAY_HEIGHT		= ROAD_PIECES_HEIGHT * ROAD_TILE_SIZE;
	
	public	const int	TILES_IN_TILESET		= 22;
	
	public	const int	FREE_TILE				= 0;
	public	const int	BLOCK_TILE				= 1;
	
	public	const int	DUST_COUNT				= 100;
	public	const int	CLOUD_COUNT				= 10;
#endregion
	
#region Interface
	[HideInInspector]
	public	Tile[,]				roadTiles;
	private Color[,]			roadFrontColors;
	private Color[,]			roadBackColors;
	
	[HideInInspector]
	public	int					progress;
	
	private int[,,]				roadSections	= new int [10,10,5] {	{	{ 1, 0, 0, 0, 2 },		// 0
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0,16,13 },
																			{ 1, 0, 0,19,13 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{13,17, 0, 0, 2 },
																			{13,18, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{ 1, 0, 0, 0, 2 },		// 1
																			{ 1, 0, 0, 0, 2 },
																			{13,17, 0, 0, 2 },
																			{13,18, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0,16,13 },
																			{ 1, 0, 0,19,13 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{13, 3, 0, 4,13 },		// 2
																			{13, 1, 0, 2,13 },
																			{13, 5, 0, 6,13 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0, 7, 0, 2 },
																			{ 1, 0, 9, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{13, 3, 0, 4,13 },
																			{13, 1, 0, 2,13 },
																			{13, 5, 0, 6,13 }		},
																		{	{ 1, 0, 0, 4,13 },		// 3
																			{ 1, 0, 0, 2,13 },
																			{ 1, 0, 0, 6,13 },
																			{13, 3, 0, 0, 2 },
																			{13, 1, 0, 0, 2 },
																			{13, 5, 0, 0, 2 },
																			{ 1, 0, 0, 4,13 },
																			{ 1, 0, 0, 2,13 },
																			{ 1, 0, 0, 6,13 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{13, 3, 0, 0, 2 },		// 4
																			{13, 1, 0, 0, 2 },
																			{13, 5, 0, 0, 2 },
																			{ 1, 0, 0, 4,13 },
																			{ 1, 0, 0, 2,13 },
																			{ 1, 0, 0, 6,13 },
																			{13, 3, 0, 0, 2 },
																			{13, 1, 0, 0, 2 },
																			{13, 5, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{13, 3, 0, 0, 2 },		// 5
																			{13,13, 3, 0, 2 },
																			{13,13, 1, 0, 2 },
																			{13,13, 5, 0, 2 },
																			{13, 5, 0, 4,13 },
																			{ 1, 0, 4,13,13 },
																			{ 1, 0, 2,13,13 },
																			{ 1, 0, 6,13,13 },
																			{ 1, 0, 0, 6,13 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{ 1, 0, 0, 0, 2 },		// 6
																			{ 1, 0, 0, 4,13 },
																			{ 1, 0, 4,13,13 },
																			{ 1, 0, 2,13,13 },
																			{ 1, 0, 6,13,13 },
																			{13, 3, 0, 6,13 },
																			{13,13, 3, 0, 2 },
																			{13,13, 1, 0, 2 },
																			{13,13, 5, 0, 2 },
																			{13, 5, 0, 0, 2 }		},
																		{	{13, 3, 0, 4,13 },		// 7
																			{13, 1, 0, 2,13 },
																			{13, 5, 0, 6,13 },
																			{ 1, 0,21, 0, 2 },
																			{13, 3, 0, 4,13 },
																			{13, 5, 0, 6,13 },
																			{ 1, 0,21, 0, 2 },
																			{13, 3, 0, 4,13 },
																			{13, 1, 0, 2,13 },
																			{13, 5, 0, 6,13 }		},
																		{	{ 1, 0, 0, 4,13 },		// 8
																			{ 1, 0,12,14,13 },
																			{ 1, 0, 0, 0, 2 },
																			{13,11,10, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0,12,11,13 },
																			{ 1, 0, 0, 0, 2 },
																			{13,15,10, 0, 2 },
																			{13, 5, 0, 0, 2 },
																			{ 1, 0, 0, 0, 2 }		},
																		{	{ 1, 0, 0, 0, 2 },		// 9
																			{13,15, 3, 0, 2 },
																			{13,14,18, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0,12,11,13 },
																			{ 1, 0, 0, 0, 2 },
																			{13,11,10, 0, 2 },
																			{ 1, 0, 0, 0, 2 },
																			{ 1, 0,12,15,13 },
																			{ 1, 0, 0, 6,13 }		}	};
	
	private int[,]				roadMap	= new int[60,5]	{	{13, 1, 0, 2,13 },		// 0
															{13, 5, 0, 6,13 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },		// 10
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 9, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },		// 20
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },		// 30
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },		// 40
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },		// 50
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{ 1, 0, 0, 0, 2 },
															{13, 3, 0, 4,13 }	};
	
	public	ConsoleSprite[]	roadPieces;
	
	public	Console			map;
	private	Transform		mapTransform;
	
	public	Player			player;
	public	Vector3			defaultPlayerSpawnPosition;
	
	private	Vector3[]		dust;
	
	public	float			cloudSpeed;
	private	Vector3[]		cloud;
	public	float			cloudPopMinDuration;
	public	float			cloudPopMaxDuration;
	private	float			cloudPopDuration;
	private	float			lastCloudPopTime;
	#endregion
	
	#region Initialization
	void Start () {
		
		int i, j;
		
		// ---=== Initialize the Console : ===---
		map.GraphicsOnly = true;
		mapTransform	= map.transform;
		
		// ---=== Initialize the Road : ===---
		roadTiles		= new Tile[ROAD_DISPLAY_WIDTH,  ROAD_DISPLAY_HEIGHT];
		roadFrontColors	= new Color[ROAD_DISPLAY_WIDTH, ROAD_DISPLAY_HEIGHT];
		roadBackColors	= new Color[ROAD_DISPLAY_WIDTH, ROAD_DISPLAY_HEIGHT];
		
		for ( i = 0; i < ROAD_DISPLAY_HEIGHT; i++ ) {
			for ( j = 0; j < ROAD_DISPLAY_WIDTH; j++ ) {
				roadTiles[j,i] = Tile.NewTile ();
			}
		}
		
		progress 		= 0;
		
		CreateRoad ();
		MoveAlongRoad ( progress );
		
		map.PrepareUpdate ();
			UpdateRoadTileMapTiles ();
		map.EndUpdate ();
		
		// ---=== Initialize Dust : ===---
		dust = new Vector3[DUST_COUNT];
		for ( i = 0; i < DUST_COUNT; i++ )
			dust[i] = new Vector3 ( -1.0f, 0.0f, 0.0f );
		
		// ---=== Initialize Cloud : ===---
		cloud = new Vector3[CLOUD_COUNT];
		for ( i = 0; i < CLOUD_COUNT; i++ )
			cloud[i] = new Vector3 ( -100.0f, 0.0f, 0.0f );
		
		cloudPopDuration = Random.Range ( cloudPopMinDuration, cloudPopMaxDuration );
		
		// ---=== Initialize the Player : ===---
		player.Setup ( defaultPlayerSpawnPosition );
		
		// ---=== Quality and Frame Rate : ===---
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate	= 30;
Debug.Log ( QualitySettings.vSyncCount + " - " + Application.targetFrameRate );
	}
#endregion
	
#region Update
	void Update () {
		
		// ---=== Game Logic Update : ===---
		DoPlayerLogicUpdate ();
		
		DoDustLogicUpdate ();
		
		DoCloudLogicUpdate ();
		
		// ---=== Display Update : ===---
		map.PrepareUpdate ();
		
			DoMapDisplayUpdate ();					// updates the map
		
			DoPlayerDisplayUpdate ();				// updates player sprite
		
			DoDustDisplayUpdate ();
			DoCloudDisplayUpdate ();
		
			map.ClearUnusedTopSprites ();
		
		map.EndUpdate ();

	}
#endregion
	
#region Map
	private void CreateRoad () {
	
		int section, sectionSlot, i, j;
		
		for ( sectionSlot = 0; sectionSlot < 5; sectionSlot++ ) {
			
			section	= Random.Range(0, 10);
			
			for ( i = 0; i < ROAD_SECTION_HEIGHT; i++ )
				for ( j = 0; j < ROAD_PIECES_WIDTH; j++ )
					roadMap[5 + sectionSlot * ROAD_SECTION_HEIGHT + i,j] = roadSections[section,i,j];
			
		}
		
	}
	
	private void DoMapDisplayUpdate () {
		
		Vector3 mapPosition;
		
		mapPosition = mapTransform.position;
		
		mapTransform.position = new Vector3 ( mapPosition.x, mapPosition.y - player.currentMovement.y, mapPosition.z );
		
		if ( mapTransform.position.y < -ROAD_TILE_SIZE ) {
			
			mapTransform.position = new Vector3 ( mapPosition.x, 0.0f, mapPosition.z );
			
			progress += 1;
			if ( ( progress - 55 ) % 60 == 0 ) CreateRoad ();
			
			// Shift Player :
			player.position.Set ( player.position.x, progress * ROAD_TILE_SIZE + defaultPlayerSpawnPosition.y, 0.0f );
			
			// Shift Dust :
			for ( int i = 0; i < DUST_COUNT; i++ )
				if ( dust[i].x != -1.0f )
					dust[i].y -= ROAD_TILE_SIZE;
			
			// Shift Clouds :
			for ( int i = 0; i < CLOUD_COUNT; i++ )
				if ( cloud[i].x != -100.0f )
					cloud[i].y -= ROAD_TILE_SIZE;
			
			MoveAlongRoad ( progress );
			UpdateRoadTileMapTiles ();
			
		}
		
	}
	
	private void CopyRoadTileToRoad ( ConsoleSprite tile, int x, int y ) {
	
		int i, j, index;
		
		for ( i = 0; i < ROAD_TILE_SIZE; i++ ) {
			for ( j = 0; j < ROAD_TILE_SIZE; j++ ) {
				
				index = j + i * tile.width;
				
				roadTiles[x + j,y + i].type			= tile.type[index];
				roadTiles[x + j,y + i].glyph		= tile.glyph[index];
				
				roadFrontColors[x + j,y + i]		= tile.front[index];
				roadBackColors[x + j,y + i]			= tile.back[index];
				
				roadTiles[x + j,y + i].destructible	= tile.destructible[index];
				
			}
		}
		
	}
	
	private void MoveAlongRoad ( int progress ) {
	
		int i, j;

		for ( i = 0; i < ROAD_PIECES_HEIGHT; i++ ) {
			for ( j = 0; j < ROAD_PIECES_WIDTH; j++ ) {
				CopyRoadTileToRoad ( roadPieces[roadMap[(progress + i) % 60,j]], j * ROAD_TILE_SIZE, i * ROAD_TILE_SIZE );
			}
		}
		
	}
	
	private void UpdateRoadTileMapTiles () {
		
		int i, j;
		
		for ( i = 0; i < ROAD_DISPLAY_HEIGHT; i++ ) {
			for ( j = 0; j < ROAD_DISPLAY_WIDTH; j++ ) {
				
				map.Glyph 			= roadTiles[j,i].glyph;
				map.GlyphColor		= roadFrontColors[j,i];
				map.BackgroundColor	= roadBackColors[j,i];
				map.Type			= roadTiles[j,i].type;

				map.SetTile ( j, i );
				
			}	
		}
		
	}
#endregion
	
#region Dust
	public void InstantiateDust ( Vector3 position ) {
		
		for ( int i = 0; i < dust.Length; i++ ) {
			if ( dust[i].x == -1.0f ) {
				dust[i].Set ( position.x, defaultPlayerSpawnPosition.y - player.spriteBox.halfHeight - map.transform.position.y, Time.time );
				break;
			}
		}

	}
	
	public void DoDustLogicUpdate () {
		
		for ( int i = 0; i < dust.Length; i++ )
			if ( dust[i].x != -1.0f )
				if ( Time.time - dust[i].z > 0.25f )
					dust[i].Set ( -1.0f, 0.0f, 0.0f );
				
	}			
				
				
	public void DoDustDisplayUpdate () {
		
		for ( int i = 0; i < dust.Length; i++ )
			if ( dust[i].x != -1.0f )
				map.DrawSprite ( 1, dust[i].x, dust[i].y, Console.TOP );
				
	}
#endregion
	
#region Cloud
	public void InstantiateCloud () {
		
		for ( int i = 0; i < cloud.Length; i++ ) {
			if ( cloud[i].x == -100.0f ) {
				cloud[i].Set ( Random.Range ( -7.0f, 33.0f ), 30.0f, 0.0f );
				break;
			}
		}

	}
	
	public void DoCloudLogicUpdate () {
		
		if ( Time.time - lastCloudPopTime > cloudPopDuration ) {
			
			InstantiateCloud ();
			
			lastCloudPopTime	= Time.time;
			cloudPopDuration	= Random.Range ( cloudPopMinDuration, cloudPopMaxDuration );
			
		}
		
		for ( int i = 0; i < cloud.Length; i++ ) {
			if ( cloud[i].x != -100.0f ) {
				
				cloud[i].Set ( cloud[i].x, cloud[i].y - cloudSpeed * Time.deltaTime, 0.0f );
				
				if ( cloud[i].y < -11.0f )
					cloud[i].Set ( -100.0f, 0.0f, 0.0f );
				
			}
		}
				
	}			
				
				
	public void DoCloudDisplayUpdate () {
		
		for ( int i = 0; i < cloud.Length; i++ )
			if ( cloud[i].x != -100.0f )
				map.DrawSprite ( 2, cloud[i].x, cloud[i].y, Console.TOP );
				
	}
#endregion
	
#region Player
	private void DoPlayerLogicUpdate () {
		player.UpdateLogic ();
	}
	
	private void DoPlayerDisplayUpdate () {
		player.UpdateFrame ();
		map.DrawRotatedAnimation (	player.currentAnimation,
									player.position.x - player.spriteBox.halfWidth,
									defaultPlayerSpawnPosition.y - player.spriteBox.halfHeight - map.transform.position.y,
									player.angle,
									Console.TOP );
	}
#endregion
	
}
