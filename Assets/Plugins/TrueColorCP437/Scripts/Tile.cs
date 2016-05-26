using UnityEngine;
using System;

[System.Serializable]
public class Tile : ScriptableObject {

#region Constants
	public	const	int		DEFAULT_TYPE			= 0;
	public	const	int 	DEFAULT_GLYPH			= 1;
	public	const	int 	DEFAULT_PARAM_SIZE		= 1;
	public	const	int 	DEFAULT_PARAM			= 0;
	public	const	bool 	DEFAULT_DESTRUCTIBILITY	= false;
#endregion

#region Interface
	public	int			type;
	public	int			glyph;
	public	Color		front;
	public	Color		back;
	public	float[]		param;
	public	bool		destructible;
#endregion

#region Constructor
	public static Tile NewTile () {
			
			Tile newTile;
			
			newTile					= (Tile)ScriptableObject.CreateInstance ( "Tile" );
			
			newTile.type			= DEFAULT_TYPE;
			newTile.glyph			= Console.DEFAULT_GLYPH;
			newTile.front			= new Color ( 1.0f, 1.0f, 1.0f, 1.0f );
			newTile.back			= new Color ( 0.0f, 0.0f, 0.0f, 1.0f );
			
			newTile.param			= new float[DEFAULT_PARAM_SIZE];
			for ( int i = 0; i < DEFAULT_PARAM_SIZE; i++ )
				newTile.param[i] = DEFAULT_PARAM;
			
			newTile.destructible	= DEFAULT_DESTRUCTIBILITY;
			
			return newTile;
			
		}
#endregion

#region Initialization
	public void SetWithTile ( Tile tile ) {

		int i;

		this.type			= tile.type;
		this.glyph			= tile.glyph;

		this.front			= tile.front;
		this.back			= tile.back;

		for ( i = 0; i < DEFAULT_PARAM_SIZE; i++ )
			this.param [i] = tile.param [i];

		this.destructible	= tile.destructible;

	}
#endregion

#region Copy
	public void Copy ( Tile dest ) {

		int i;

		dest.type			= this.type;
		dest.glyph			= this.glyph;
		
		dest.front			= this.front;
		dest.back			= this.back;
		
		for ( i = 0; i < DEFAULT_PARAM_SIZE; i++ )
			dest.param [i] = this.param [i];
		
		dest.destructible	= this.destructible;

	}
#endregion

#region Duplicate
	public Tile Duplicate () {

		int i;

		Tile dup = new Tile ();

		dup.type			= this.type;
		dup.glyph			= this.glyph;
		
		dup.front			= this.front;
		dup.back			= this.back;
		
		for ( i = 0; i < DEFAULT_PARAM_SIZE; i++ )
			dup.param [i] = this.param [i];
		
		dup.destructible	= this.destructible;

		return dup;

	}
#endregion

}
