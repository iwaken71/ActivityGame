using UnityEngine;
using System.Collections;

[System.Serializable]
public class ConsoleSprite : ScriptableObject {
	
	public	int 	width;
	public	int 	height;

	public	int[]		type;
	public	int[]		glyph;
	public	Color[]		front;
	public	Color[]		back;
	public	float[]		param;
	public	bool[]		destructible;
	
	public	bool[]	mask;
	
	public static ConsoleSprite NewSprite ( int width, int height ) {

		ConsoleSprite newSprite = (ConsoleSprite)ScriptableObject.CreateInstance ( "ConsoleSprite" );
		
		newSprite.width		= width;
		newSprite.height	= height;
			
		newSprite.type			= new int[width * height];
		newSprite.glyph			= new int[width * height];
		newSprite.front			= new Color[width * height];
		newSprite.back			= new Color[width * height];
		
		newSprite.param			= new float[width * height * Tile.DEFAULT_PARAM_SIZE];
		for ( int i = 0; i < newSprite.param.Length; i++ )
			newSprite.param[i]	= Tile.DEFAULT_PARAM;
		
		newSprite.destructible	= new bool[width * height];

		newSprite.mask			= new bool[width * height];
		
		for ( int y = 0; y < height; y++ ) {
			for ( int x = 0; x < width; x++ ) {
			
				newSprite.type[x+width*y]			= Tile.DEFAULT_TYPE;
				newSprite.glyph[x+width*y]			= Tile.DEFAULT_GLYPH;
				newSprite.front[x+width*y]			= new Color ( 1.0f, 1.0f, 1.0f, 1.0f );
				newSprite.back[x+width*y]			= new Color ( 0.0f, 0.0f, 0.0f, 1.0f );
				
				for ( int p = 0; p < Tile.DEFAULT_PARAM_SIZE; p++ )
					newSprite.param[(x + width * y) * Tile.DEFAULT_PARAM_SIZE + p]	= Tile.DEFAULT_PARAM;
				
				newSprite.destructible[x+width*y]	= Tile.DEFAULT_DESTRUCTIBILITY;
				newSprite.mask[x+width*y]			= true;
			
			}
		}
		
		return newSprite;
		
	}
	
	public Texture2D GlyphColorsTexture () {

		int			i, j;
		Texture2D	colorsTexture;
		
		colorsTexture				= new Texture2D ( width, height, TextureFormat.ARGB32, false );
		colorsTexture.filterMode	= FilterMode.Point;
		
		for ( i = 0; i < height; i++ )
			for ( j = 0; j < width; j++ )
				colorsTexture.SetPixel ( j, i, front[j + i * width] );
		
		colorsTexture.Apply ();
		
		return colorsTexture;

	}
	
	public Texture2D BackgroundColorsTexture () {

		int			i, j;
		Texture2D	colorsTexture;
		
		colorsTexture				= new Texture2D ( width, height, TextureFormat.ARGB32, false );
		colorsTexture.filterMode	= FilterMode.Point;
		
		for ( i = 0; i < height; i++ )
			for ( j = 0; j < width; j++ )
				colorsTexture.SetPixel ( j, i, back[j + i * width] );
		
		colorsTexture.Apply ();
		
		return colorsTexture;

	}
	
}
