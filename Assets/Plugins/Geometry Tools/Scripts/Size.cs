using UnityEngine;
using System.Collections;

[System.Serializable]
public class Size {
	
#region Instance Variables
	public	float	width;
	public	float	halfWidth;
	public	float	height;
	public	float	halfHeight;
	
	public	Vector2	vec2Size;
	public	Vector2	vec2HalfSize;
	
	public	Vector3	vec3Size;
	public	Vector3	vec3HalfSize;
#endregion
	
#region Constructor
	public Size ( float width, float height ) {
		
		this.width	= width;
		this.height = height;
		
		this.halfWidth	=  width / 2.0f;
		this.halfHeight	= height / 2.0f;
		
		this.vec2Size		= new Vector2 ( width, height );
		this.vec2HalfSize	= new Vector2 ( this.halfWidth, this.halfHeight );
		
		this.vec3Size		= new Vector3 ( width, height, 0.0f );
		this.vec3HalfSize	= new Vector3 ( this.halfWidth, this.halfHeight, 0.0f );
		
	}
#endregion
	
}
