using UnityEngine;
using System.Collections;

[System.Serializable]
public class Box {
	
#region Constants
	public	enum		BoxType { Centered, BottomLeft, BottomRight, TopRight, TopLeft };
#endregion
	
#region Instance Variables
	public	BoxType	type;
	
	public	float	x;
	public	float	y;
	public	float	width;
	public	float	halfWidth;
	public	float	height;
	public	float	halfHeight;
	
	public	Vector3	position;
	public	Size	size;
	
	public	Rect	rect;
#endregion

#region Constructor
	public Box ( float x, float y, float width, float height, BoxType type ) {
		
		this.x		= x;
		this.y		= y;
		this.width	= width;
		this.height	= height;
		
		this.halfWidth	=  width / 2.0f;
		this.halfHeight	= height / 2.0f;
		
		this.position	= new Vector3 ( x, y, 0.0f );
		
		this.size = new Size ( width, height );
		
		this.rect = new Rect ( x, y, width, height );
		
		this.type = type;

		switch ( type ) {
			case BoxType.Centered:		this.rect = new Rect ( x + halfWidth, y + halfHeight, width, height ); break;
			case BoxType.BottomLeft:	this.rect = new Rect ( x,             y,              width, height ); break;
			case BoxType.BottomRight:	this.rect = new Rect ( x +     width, y,              width, height ); break;
			case BoxType.TopRight:		this.rect = new Rect ( x +     width, y + height,     width, height ); break;
			case BoxType.TopLeft:		this.rect = new Rect ( x,             y + height,     width, height ); break;
			default:					this.rect = new Rect ( x + halfWidth, y + halfHeight, width, height ); break;	
		}

	}
#endregion
	
#region Z
	public void SetZ ( float z ) { position.z = z; }	
#endregion
		
#region Update
	public void RectUpdate ( float dx, float dy ) { 
		position.x	+= dx;	position.y	+= dy;
		rect.x		+= dx;	rect.y		+= dy;
	}
#endregion
	
}
