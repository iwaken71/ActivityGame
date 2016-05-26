using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

[ExecuteInEditMode()]

public class Console : MonoBehaviour {

#region CslPoint Structure
	public struct CslPoint {

		public int x, y;

		public CslPoint ( int x, int y ) {
			this.x	= x;
			this.y	= y;
		}

	}
#endregion

#region Delegate Types
	public	delegate void Clear_DELEGATE ();
	public	delegate void SetTile_DELEGATE ( int x, int y );
	public	delegate void StrokeHorizontalLine_DELEGATE ( int x1, int x2, int y );
	public	delegate void StrokeVerticalLine_DELEGATE ( int x1, int x2, int y );
	public	delegate void StrokeScannedLine_DELEGATE ( bool smooth );
	public	delegate void FillRectangle_DELEGATE ( int x, int y, int w, int h );
	public	delegate void FillPolygon_DELEGATE ( Vector2[] vertices );
	public	delegate void DrawConsoleSprite_DELEGATE ( ConsoleSprite sprite, int x, int y );
#endregion

#region Constants
	private	const	bool	NO_FONT								= false;
	public	const	int		NO_FONT_LIST						= -1;
	
	public	const	int		TEXTURE_MAX_SIZE					= 1024;
	
	public	const	int		CONSOLE_COORDS						= 0;
	public	const	int		FIRST_SPRITE_COORDS					= 1;
	
	public	const	bool	TOP									= true;
	public	const	bool	BOTTOM								= false;
	
	public	const	int 	DEFAULT_GLYPH						= 1;

	public	const	float	DEFAULT_GLYPH_COLOR_RED				= 1.0f;
	public	const	float	DEFAULT_GLYPH_COLOR_GREEN			= 1.0f;
	public	const	float	DEFAULT_GLYPH_COLOR_BLUE			= 1.0f;
	public	const	float	DEFAULT_BACKGROUND_COLOR_RED		= 0.0f;
	public	const	float	DEFAULT_BACKGROUND_COLOR_GREEN		= 0.0f;
	public	const	float	DEFAULT_BACKGROUND_COLOR_BLUE		= 0.0f;

	public	const	int		LINE_HORIZONTAL_GLYPH				= 95;
	public	const	int		LINE_VERTICAL_GLYPH					= 179;
	public	const	int		LINE_BOTTOM_LEFT_TO_TOP_RIGHT_GLYPH	= 47;
	public	const	int		LINE_TOP_LEFT_TO_BOTTOM_RIGHT_GLYPH	= 92;
	
	private	const	int		DIGIT_0								= 48;
	private	const	int		DIGIT_9								= 57;
	private	const	int		QUESTION_MARK						= 63;
#endregion

#region Instance Variables
	public	bool				setupComplete					= false;
	
	public	int 				width 							= 0;
	public	int 				height							= 0;
	private	int					consoleSize						= 0;
	
	private Vector3[] 			vertices;
	private Vector2[]			fontUV;
	private Vector2[]			colorUV;

	public	float				glyphTexWidth					= 0.0f;
	public	float				glyphTexHeight					= 0.0f;
	public	float				glyphVTexOffsetForWindows		= 0.0f;

	public	float				colorTexWidth					= 0.0f;
	public	float				colorTexHeight					= 0.0f;
	public	Rect[]				colorsTexCoords;
	public	Texture2D			foreground;
	public	Texture2D			background;
	public	float				colorVTexOffsetForWindows		= 0.0f;
	
	public	MeshRenderer		meshRenderer;
	public	MeshFilter 			meshFilter;
	public 	Mesh				mesh;

	public	Tile[]				tiles;

	public	ConsoleFont[]		fonts;
	public	string[]			fontNames;
	public	ConsoleFont			currentFont;
	public	ConsoleFont			defaultFont;

	public	int					currentGlyph;
	public	int					currentGlyphX;
	public	int					currentGlyphY;

	public	float				currentGlyphTexX;
	public	float				currentGlyphTexY;
	public	float				nextGlyphTexX;
	public	float				nextGlyphTexY;

	public	Color				currentGlyphColor;
	public	Color				currentBackgroundColor;
	
	public	int					currentType;
	public	float[]				currentParam;
	public	bool				currentDestructability;

	public	int					clearGlyph;
	public	int					clearGlyphX;
	public	int					clearGlyphY;

	public	float				clearGlyphTexX;
	public	float				clearGlyphTexY;
	public	float				nextClearGlyphTexX;
	public	float				nextClearGlyphTexY;

	public	Color				clearGlyphColor;
	public	Color				clearBackgroundColor;

	public	bool				drawGlyph						= true;
	public	bool				drawGlyphColor					= true;
	public	bool				drawBackgroundColor				= true;
	public	bool				setType							= true;
	public	bool				setParam						= true;
	public	bool				setDestructability				= true;

	public	bool				graphicsOnly					= false;

	public	bool				useSpriteMask					= true;
	
	public	int[]				lineScan;
	public	int					lineLength;

	private	int[]				leftScan;
	private	int[]				rightScan;
	
	public	int					topMaxSpriteGlyphs;
	private	int					topSpriteGlyphsCount;
	private	int					lastTopSpriteGlyphsCount;
	public	int					bottomMaxSpriteGlyphs;
	private	int					bottomSpriteGlyphsCount;
	private	int					lastBottomSpriteGlyphsCount;
	public	List<ConsoleSprite>	sprites;
	public	Vector3				hiddingPosition;

#if UNITY_EDITOR
	public	int					currentTool;

	public	bool				hasStamp;
	public	int					stampX;
	public	int					stampY;
	public	ConsoleSprite		stamp;
	
	public	bool				shouldRefresh;
#endif
	
#endregion

#region Delegate Variables
	public	Clear_DELEGATE					Clear;
	public	SetTile_DELEGATE				SetTile;
	public	StrokeHorizontalLine_DELEGATE	StrokeHorizontalLine;
	public	StrokeVerticalLine_DELEGATE		StrokeVerticalLine;
	public	StrokeScannedLine_DELEGATE		StrokeScannedLine;
	public	FillRectangle_DELEGATE			FillRectangle;
	public	FillPolygon_DELEGATE			FillPolygon;
	public	DrawConsoleSprite_DELEGATE		DrawConsoleSprite;
#endregion

#region Initialization
	void OnEnable () {
		
		if ( mesh == null ) {
			
			setupComplete = false;
			
			SetupMesh ();

		}

		SetupScanBuffers ( width, height );

	}

	void OnWillRenderObject () {

		if ( setupComplete == true ) {
			meshRenderer.sharedMaterial.SetTexture ( "_FontTex", currentFont.fontTexture );
			meshRenderer.sharedMaterial.SetTexture ( "_ForegroundTex", foreground );
			meshRenderer.sharedMaterial.SetTexture ( "_BackgroundTex", background );
		}
		
	}

	public void Rebuild ( int newWidth, int newHeight, int newFontIndex ) {
		
		if ( setupComplete == true ) {

			SetFont ( newFontIndex );
	
			ResizeTilemap ( newWidth, newHeight );
	
			ResizeColorTextures ( newWidth, newHeight );
	
			ResizeGlyphs ( newWidth, newHeight );
	
			ResizeCurrentGlyph ();
			ResizeClearGlyph ();
			
		}
		else {

			if ( SetupFonts () != NO_FONT ) {
				
				width			= newWidth;
				height			= newHeight;
				
				SetupTilemap ();
				
				SetupColors ();

				SetupGlyphs ();

				SetupContext ();
				
				setupComplete	= true;
				
			}
			
		}
		
		SetupScanBuffers ( newWidth, newHeight );
		
	}

	private void SetupMesh () {

		meshRenderer	= (MeshRenderer)GetComponent ( "MeshRenderer" );		
		meshRenderer.sharedMaterial	= new Material ( meshRenderer.sharedMaterial.shader );

		meshFilter		= (MeshFilter)GetComponent ( "MeshFilter" );
		
		mesh			= meshFilter.sharedMesh;
		if ( mesh == null ) {
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}
		mesh.Clear();

	}

	private bool SetupFonts () {
		
		if ( fonts == null )
			return NO_FONT;
		
		if ( fonts.Length == 0 )
			return NO_FONT;
		
		if ( defaultFont == null)
			currentFont	=	fonts[0];
		else
			currentFont	=	defaultFont;

		meshRenderer.sharedMaterial.SetTexture ( "_FontTex", currentFont.fontTexture );

		glyphTexWidth	=  ((float)currentFont.glyphPixelWidth) / currentFont.fontTexture.width;
		glyphTexHeight	= ((float)currentFont.glyphPixelHeight) / currentFont.fontTexture.height;
		
		glyphVTexOffsetForWindows	= 1.0f / ( 256.0f * currentFont.fontTexture.height );

		SetupFontList ();
		
		return !NO_FONT;

	}
	
	public bool SetupFontList () {

		int i;
		
		if ( fonts == null )
			return NO_FONT;
		
		if ( fonts.Length == 0 )
			return NO_FONT;
		
		fontNames	= new string[fonts.Length];
		
		for ( i = 0; i < fonts.Length; i++ )
			fontNames[i] = fonts[i].fontName;
		
		return !NO_FONT;
		
	}
	
	private void SetupTilemap () {
		
		int i;
		
		consoleSize	= width * height;
		tiles		= new Tile[consoleSize];
		
		for ( i = 0; i < width * height; i++ )
			tiles[i] = Tile.NewTile ();
		
		currentParam = new float[Tile.DEFAULT_PARAM_SIZE];
		for ( i = 0; i < Tile.DEFAULT_PARAM_SIZE; i++ )
			currentParam[i] = 0.0f;
		
	}
	
	private void ResizeTilemap ( int newWidth, int newHeight ) {
		
		int		i, j, minWidth, minHeight;
		Tile[]	newTiles;
		
		minWidth	=  width <  newWidth ?  width :  newWidth;
		minHeight	= height < newHeight ? height : newHeight;
		
		consoleSize	= newWidth * newHeight;
		
		newTiles	= new Tile[newWidth * newHeight];
		
		for ( i = 0; i < newHeight; i++ )
			for ( j = 0; j < newWidth; j++ )
				newTiles[j + i * newWidth] = Tile.NewTile ();
		
		for ( i = 0; i < minHeight; i++ )
			for ( j = 0; j < minWidth; j++ )
				newTiles[j + i * newWidth].SetWithTile ( tiles[j + i * width] );
		
		tiles		= newTiles;
		
	}
	
	private void SetupColors () {

		int			i, j;
		Texture2D	consoleForegroundTex, consoleBackgroundTex;
		Texture2D[]	foregroundTexs, backgroundTexs;
		Color		front, back;

		// ---=== Build the Console Glyph and Background Textures : ===---
		consoleForegroundTex			= new Texture2D ( Mathf.NextPowerOfTwo ( width ), Mathf.NextPowerOfTwo ( height ) );
		consoleForegroundTex.filterMode	= FilterMode.Point;
		
		consoleBackgroundTex			= new Texture2D ( Mathf.NextPowerOfTwo ( width ), Mathf.NextPowerOfTwo ( height ) );
		consoleBackgroundTex.filterMode	= FilterMode.Point;

		for ( i = 0; i < height; i++ ) {
			for ( j = 0; j < width; j++ ) {
			
				if ( ( ( i % 2 ) + j ) % 2 == 0 ) {
					front 	= new Color( 1.0f, 0.0f, 0.0f, 1.0f );
					back	= new Color( 0.0f, 0.0f, 1.0f, 1.0f );
				}
				else {
					front 	= new Color( 1.0f, 1.0f, 0.0f, 1.0f );
					back	= new Color( 1.0f, 0.0f, 1.0f, 1.0f );
				}
				
				consoleForegroundTex.SetPixel ( j, i, front );
				consoleBackgroundTex.SetPixel ( j, i, back );
				
				tiles[j+i*width].front	= front;
				tiles[j+i*width].back	= back;

			}
		}
		
		consoleForegroundTex.Apply ();
		consoleBackgroundTex.Apply ();
		
		
		// ---=== Pack the Console and Sprites Textures in Glyph and Background Arrays : ===---
		foregroundTexs		= new Texture2D[sprites.Count + 1];
		backgroundTexs		= new Texture2D[sprites.Count + 1];
		
		foregroundTexs[0]	= consoleForegroundTex;
		backgroundTexs[0]	= consoleBackgroundTex;
		
		for ( i = 0; i < sprites.Count; i++ ) {
			foregroundTexs[i + 1] = sprites[i].GlyphColorsTexture ();
			backgroundTexs[i + 1] = sprites[i].BackgroundColorsTexture ();
		}
		
		
		// ---=== Pack the Actual Textures and Store their Coordinates in the new Global Textures : ===---
		foreground				= new Texture2D ( TEXTURE_MAX_SIZE, TEXTURE_MAX_SIZE, TextureFormat.RGBA32, false );
		foreground.filterMode	= FilterMode.Point;
		
		background				= new Texture2D ( TEXTURE_MAX_SIZE, TEXTURE_MAX_SIZE, TextureFormat.RGBA32, false );
		background.filterMode	= FilterMode.Point;
		
		colorsTexCoords	= foreground.PackTextures ( foregroundTexs, 0, TEXTURE_MAX_SIZE );
		background.PackTextures ( backgroundTexs, 0, TEXTURE_MAX_SIZE );
		
		
		// ---=== Compute the Size of a single Pixel ( i.e. Glyph Color ) in Global Texture Space : ===---
		colorTexWidth	= 1.0f / (float)foreground.width;
		colorTexHeight	= 1.0f / (float)foreground.height;
		
		
		// ---=== Assign the Global Textures to the Console Mesh : ===---
		meshRenderer.sharedMaterial.SetTexture ( "_ForegroundTex", foreground );
		meshRenderer.sharedMaterial.SetTexture ( "_BackgroundTex", background );


		// ---=== Calculating proper Texture Offset for FUCKING Windows : ===---
		colorVTexOffsetForWindows	= 1.0f / (256.0f * (float)foreground.height);

	}

	private void ResizeColorTextures ( int newWidth, int newHeight ) {

		int			i, j;
		Texture2D	newConsoleForegroundTex, newConsoleBackgroundTex;
		Texture2D[]	foregroundTexs, backgroundTexs;

		newConsoleForegroundTex				= new Texture2D ( Mathf.NextPowerOfTwo ( newWidth ), Mathf.NextPowerOfTwo ( newHeight ) );
		newConsoleForegroundTex.filterMode	= FilterMode.Point;
		
		newConsoleBackgroundTex				= new Texture2D ( Mathf.NextPowerOfTwo ( newWidth ), Mathf.NextPowerOfTwo ( newHeight ) );
		newConsoleBackgroundTex.filterMode	= FilterMode.Point;
		
		for ( i = 0; i < newHeight; i++ ) {
			for ( j = 0; j < newWidth; j++ ) {
				newConsoleForegroundTex.SetPixel ( j, i, tiles[j + i * newWidth].front );
				newConsoleBackgroundTex.SetPixel ( j, i, tiles[j + i * newWidth].back );
			}
		}
		
		newConsoleForegroundTex.Apply ();
		newConsoleBackgroundTex.Apply ();
		
		// ---=== Pack the Console and Sprites Textures in Glyph and Background Arrays : ===---
		foregroundTexs		= new Texture2D[sprites.Count + 1];
		backgroundTexs		= new Texture2D[sprites.Count + 1];
		
		foregroundTexs[0]	= newConsoleForegroundTex;
		backgroundTexs[0]	= newConsoleBackgroundTex;
		
		for ( i = 0; i < sprites.Count; i++ ) {
			foregroundTexs[i+1] = sprites[i].GlyphColorsTexture ();
			backgroundTexs[i+1] = sprites[i].BackgroundColorsTexture ();
		}
		
		
		// ---=== Pack the Actual Textures and Store their Coordinates in the new Global Textures : ===---
		colorsTexCoords	= foreground.PackTextures ( foregroundTexs, 0, TEXTURE_MAX_SIZE );
		background.PackTextures ( backgroundTexs, 0, TEXTURE_MAX_SIZE );
		
		
		// ---=== Compute the Size of a single Pixel ( i.e. Glyph Color ) in Global Texture Space : ===---
		colorTexWidth	= 1.0f / (float)foreground.width;
		colorTexHeight	= 1.0f / (float)foreground.height;


		// ---=== Assign the Global Textures to the Console Mesh : ===---
		meshRenderer.sharedMaterial.SetTexture ( "_ForegroundTex", foreground );
		meshRenderer.sharedMaterial.SetTexture ( "_BackgroundTex", background );
		
		
		// ---=== Calculating proper Texture Offset for FUCKING Windows : ===---
		colorVTexOffsetForWindows	= 1.0f / (256.0f * (float)foreground.height);

	}
	
	private void SetupGlyphs () {

		int i, j, index;

		int			size;
		Vector3[]	newVertices;
		Vector2[]	newFontUVs;
		Vector2[]	newColorUVs;
		Vector3[]	newNormals;
		int[]		newTriangles;

		CslPoint	fontCoords;
		int			fontX, fontY;
		
		Vector2		colorsU0V0;

		// ---=== Initialize the Vertices Array : ===---
		size = bottomMaxSpriteGlyphs + width * height + topMaxSpriteGlyphs;

		newVertices		= new Vector3[4 * size];
		newFontUVs		= new Vector2[4 * size];
		newColorUVs		= new Vector2[4 * size];
		newNormals		= new Vector3[4 * size];
		newTriangles	= new int[6 * size];
		
		colorsU0V0		= new Vector2 ( colorsTexCoords[CONSOLE_COORDS].x, colorsTexCoords[CONSOLE_COORDS].y );
		
		// ---=== Polygons for the Bottom Sprites : ===---
		for ( i = 0; i < bottomMaxSpriteGlyphs; i++ ) {
			
			// Vertices :
			newVertices[4 * i]		= hiddingPosition;
			newVertices[4 * i + 1]	= hiddingPosition + new Vector3 ( currentFont.glyphWidth,                    0.0f, 0.0f );
			newVertices[4 * i + 2]	= hiddingPosition + new Vector3 ( currentFont.glyphWidth, currentFont.glyphHeight, 0.0f );
			newVertices[4 * i + 3]	= hiddingPosition + new Vector3 (                   0.0f, currentFont.glyphHeight, 0.0f );

			// Font UV Coordinates :
			fontCoords		= FontCoordsFromIndex ( DEFAULT_GLYPH );
			fontX			= fontCoords.x;
			fontY			= fontCoords.y;

			newFontUVs[4 * i]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );

			// Color UV Coordinates :
			newColorUVs[4 * i]		= colorsU0V0 + new Vector2 (          0.0f,                                       0.0f );
			newColorUVs[4 * i + 1]	= colorsU0V0 + new Vector2 ( colorTexWidth,                                       0.0f );
			newColorUVs[4 * i + 2]	= colorsU0V0 + new Vector2 ( colorTexWidth, colorTexHeight - colorVTexOffsetForWindows );
			newColorUVs[4 * i + 3]	= colorsU0V0 + new Vector2 (          0.0f, colorTexHeight - colorVTexOffsetForWindows );

			// Normals :
			newNormals[4 * i]		= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 1]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 2]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 3]	= new Vector3 ( 0.0f, 0.0f, -1.0f );

			// Triangles :
			newTriangles[6 * i]		= 4 * i;
			newTriangles[6 * i + 1]	= 4 * i + 2;
			newTriangles[6 * i + 2]	= 4 * i + 1;
			newTriangles[6 * i + 3]	= 4 * i;
			newTriangles[6 * i + 4]	= 4 * i + 3;
			newTriangles[6 * i + 5]	= 4 * i + 2;
			
		}
		
		// ---=== Polygons of the Console itself : ===---
		for ( i = 0; i < height; i++ ) {

			for ( j = 0; j < width; j++ ) {

				index = bottomMaxSpriteGlyphs + j + i * width;

				// Vertices :
				newVertices[4 * index]		= new Vector3 (         j * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				newVertices[4 * index + 1]	= new Vector3 ( ( j + 1 ) * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				newVertices[4 * index + 2]	= new Vector3 ( ( j + 1 ) * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );
				newVertices[4 * index + 3]	= new Vector3 (         j * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );

				// Font UV Coordinates :
				fontCoords		= FontCoordsFromIndex ( tiles[index - bottomMaxSpriteGlyphs].glyph );
				fontX			= fontCoords.x;
				fontY			= fontCoords.y;

				newFontUVs[4 * index]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				newFontUVs[4 * index + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				newFontUVs[4 * index + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
				newFontUVs[4 * index + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );


				// Color UV Coordinates :
				newColorUVs[4 * index]		= colorsU0V0 + new Vector2 (         j * colorTexWidth,         i * colorTexHeight - colorVTexOffsetForWindows );
				newColorUVs[4 * index + 1]	= colorsU0V0 + new Vector2 ( ( j + 1 ) * colorTexWidth,         i * colorTexHeight - colorVTexOffsetForWindows );
				newColorUVs[4 * index + 2]	= colorsU0V0 + new Vector2 ( ( j + 1 ) * colorTexWidth, ( i + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				newColorUVs[4 * index + 3]	= colorsU0V0 + new Vector2 (         j * colorTexWidth, ( i + 1 ) * colorTexHeight - colorVTexOffsetForWindows );

				// Normals :
				newNormals[4 * index]		= new Vector3 ( 0.0f, 0.0f, -1.0f );
				newNormals[4 * index + 1]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
				newNormals[4 * index + 2]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
				newNormals[4 * index + 3]	= new Vector3 ( 0.0f, 0.0f, -1.0f );

				// Triangles :
				newTriangles[6 * index]		= 4 * index;
				newTriangles[6 * index + 1]	= 4 * index + 2;
				newTriangles[6 * index + 2]	= 4 * index + 1;
				newTriangles[6 * index + 3]	= 4 * index;
				newTriangles[6 * index + 4]	= 4 * index + 3;
				newTriangles[6 * index + 5]	= 4 * index + 2;

			}
			
		}
		
		// ---=== Polygons for the Top Sprites : ===---
		for ( i = bottomMaxSpriteGlyphs + width * height; i < bottomMaxSpriteGlyphs + width * height + topMaxSpriteGlyphs; i++ ) {
			
			// Vertices :
			newVertices[4 * i]		= hiddingPosition;
			newVertices[4 * i + 1]	= hiddingPosition + new Vector3 ( currentFont.glyphWidth,                    0.0f, 0.0f );
			newVertices[4 * i + 2]	= hiddingPosition + new Vector3 ( currentFont.glyphWidth, currentFont.glyphHeight, 0.0f );
			newVertices[4 * i + 3]	= hiddingPosition + new Vector3 (                   0.0f, currentFont.glyphHeight, 0.0f );

			// Font UV Coordinates :
			fontCoords		= FontCoordsFromIndex ( DEFAULT_GLYPH );
			fontX			= fontCoords.x;
			fontY			= fontCoords.y;

			newFontUVs[4 * i]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
			newFontUVs[4 * i + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );

			// Color UV Coordinates :
			newColorUVs[4 * i]		= colorsU0V0 + new Vector2 (          0.0f,                                       0.0f );
			newColorUVs[4 * i + 1]	= colorsU0V0 + new Vector2 ( colorTexWidth,                                       0.0f );
			newColorUVs[4 * i + 2]	= colorsU0V0 + new Vector2 ( colorTexWidth, colorTexHeight - colorVTexOffsetForWindows );
			newColorUVs[4 * i + 3]	= colorsU0V0 + new Vector2 (          0.0f, colorTexHeight - colorVTexOffsetForWindows );

			// Normals :
			newNormals[4 * i]		= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 1]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 2]	= new Vector3 ( 0.0f, 0.0f, -1.0f );
			newNormals[4 * i + 3]	= new Vector3 ( 0.0f, 0.0f, -1.0f );

			// Triangles :
			newTriangles[6 * i]		= 4 * i;
			newTriangles[6 * i + 1]	= 4 * i + 2;
			newTriangles[6 * i + 2]	= 4 * i + 1;
			newTriangles[6 * i + 3]	= 4 * i;
			newTriangles[6 * i + 4]	= 4 * i + 3;
			newTriangles[6 * i + 5]	= 4 * i + 2;
			
		}

		// ---=== Pushing the Vertices Array in the Mesh Structure : ===---
		mesh.vertices	= newVertices;
		mesh.uv 		= newFontUVs;
		mesh.uv2		= newColorUVs;
		mesh.normals	= newNormals;

		mesh.triangles	= newTriangles;


		// ---=== Clean-up the mesh : ===---
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

	}

	private void ResizeGlyphs ( int newWidth, int newHeight ) {

		mesh.Clear();

		width	= newWidth;
		height	= newHeight;

		SetupGlyphs ();

	}

	private void SetupContext () {

		Glyph = DEFAULT_GLYPH;

		GlyphColor	= new Color ( 	DEFAULT_GLYPH_COLOR_RED,
									DEFAULT_GLYPH_COLOR_GREEN,
									DEFAULT_GLYPH_COLOR_BLUE );
		BackgroundColor	= new Color (	DEFAULT_BACKGROUND_COLOR_RED,
										DEFAULT_BACKGROUND_COLOR_GREEN,
										DEFAULT_BACKGROUND_COLOR_BLUE );

		DrawGlyph			= true;
		DrawGlyphColor		= true;
		DrawBackgroundColor	= true;
		
		SetType				= true;
		SetParam			= true;
		SetDestructability	= true;
		
		Clear					= new Clear_DELEGATE ( _Clear_ALL );
		SetTile					= new SetTile_DELEGATE ( _SetTile_ALL );
		StrokeHorizontalLine	= new StrokeHorizontalLine_DELEGATE ( _StrokeHorizontalLine_ALL );
		StrokeVerticalLine		= new StrokeVerticalLine_DELEGATE ( _StrokeVerticalLine_ALL );
		StrokeScannedLine		= new StrokeScannedLine_DELEGATE ( _StrokeScannedLine_ALL );
		FillRectangle			= new FillRectangle_DELEGATE ( _FillRectangle_ALL );
		FillPolygon				= new FillPolygon_DELEGATE ( _FillPolygon_ALL );
		DrawConsoleSprite		= new DrawConsoleSprite_DELEGATE ( _DrawConsoleSprite_ALL );

	}

	private void SetupScanBuffers ( int width, int height ) {

		lineScan	= new int[8 * ( width < height ? height : width )];

		leftScan	= new int[height];
		rightScan	= new int[height];

	}
#endregion

#region Update
	public void PrepareUpdate () {
		
		vertices	= mesh.vertices;
		fontUV 		= mesh.uv;
		colorUV		= mesh.uv2;
		
		lastBottomSpriteGlyphsCount	= bottomSpriteGlyphsCount;
		bottomSpriteGlyphsCount		= 0;
		
		lastTopSpriteGlyphsCount	= topSpriteGlyphsCount;
		topSpriteGlyphsCount		= 0;
		
	}

	public void EndUpdate () {

		foreground.Apply ();
		background.Apply ();
		
		mesh.uv2		= colorUV;
		mesh.uv			= fontUV;
		mesh.vertices	= vertices;

	}
#endregion

#region Graphic Context
	public void SetFont ( int fontIndex ) {
		
		if ( fontIndex <= 0 )
			currentFont =	fonts[0];
		else if ( fontIndex < fonts.Length )
			currentFont	=	fonts[fontIndex];
		else
			return;

		meshRenderer.sharedMaterial.SetTexture ( "_FontTex", currentFont.fontTexture );

		glyphTexWidth	=  ((float)currentFont.glyphPixelWidth) / currentFont.fontTexture.width;
		glyphTexHeight	= ((float)currentFont.glyphPixelHeight) / currentFont.fontTexture.height;

	}

	public int GetCurrentFontIndex () {

		int i;
		
		if ( fonts == null )
			return NO_FONT_LIST;
		
		if ( fonts.Length == 0 )
			return NO_FONT_LIST;
		
		for ( i = 0; i < fonts.Length; i++ )
			if ( fonts[i].fontName == currentFont.fontName )
				break;

		return i;

	}

	public int Glyph {
		get {	return currentGlyph; }
		set {	currentGlyph		= value;
				currentGlyphX		= value % currentFont.fontGlyphWidth;
				currentGlyphY		= ( value - currentGlyphX ) / currentFont.fontGlyphWidth;
				currentGlyphTexX	= glyphTexWidth * (float)currentGlyphX;
				currentGlyphTexY	= glyphTexHeight * (float)currentGlyphY;
				nextGlyphTexX		= ((float)glyphTexWidth) * ( (float)currentGlyphX + 1.0f );
				nextGlyphTexY		= ((float)glyphTexHeight) * ( (float)currentGlyphY + 1.0f ); }
	}

	void ResizeCurrentGlyph () {
		currentGlyphTexX	= glyphTexWidth * currentGlyphX;
		currentGlyphTexY	= glyphTexHeight * currentGlyphY - ( 1.0f / 512.0f );
		nextGlyphTexX		= glyphTexWidth * ( currentGlyphX + 1.0f );
		nextGlyphTexY		= glyphTexHeight * ( currentGlyphY + 1.0f );
	}

	public Color GlyphColor {
		get {	return currentGlyphColor; }
		set {	currentGlyphColor = value; }
	}

	public Color BackgroundColor {
		get {	return currentBackgroundColor; }
		set {	currentBackgroundColor = value; }
	}

	public int ClearGlyph {
		get {	return clearGlyph; }
		set {	clearGlyph			= value;
				clearGlyphX			= value % currentFont.fontGlyphWidth;
				clearGlyphY			= ( value - currentGlyphX ) / currentFont.fontGlyphWidth;
				clearGlyphTexX		= glyphTexWidth * clearGlyphX;
				clearGlyphTexY		= glyphTexHeight * clearGlyphY;
				nextClearGlyphTexX	= ((float)glyphTexWidth) * ( (float)clearGlyphX + 1.0f );
				nextClearGlyphTexY	= ((float)glyphTexHeight) * ( (float)clearGlyphY + 1.0f ); }
	}

	void ResizeClearGlyph () {
		clearGlyphTexX		= glyphTexWidth * clearGlyphX;
		clearGlyphTexY		= glyphTexHeight * clearGlyphY;
		nextClearGlyphTexX	= glyphTexWidth * ( clearGlyphX + 1 );
		nextClearGlyphTexY	= glyphTexHeight * ( clearGlyphY + 1 );
	}

	public Color ClearGlyphColor {
		get {	return clearGlyphColor; }
		set {	clearGlyphColor = value; }
	}

	public Color ClearBackgroundColor {
		get {	return clearBackgroundColor; }
		set {	clearBackgroundColor = value; }
	}

	public bool DrawGlyph {
		get {	return drawGlyph; }
		set {	drawGlyph = value; }
	}

	public bool DrawGlyphColor {
		get {	return drawGlyphColor; }
		set {	drawGlyphColor = value; }
	}

	public bool DrawBackgroundColor {
		get {	return drawBackgroundColor; }
		set {	drawBackgroundColor = value; }
	}
	
	public int Type {
		get {	return currentType; }
		set {	currentType = value; }
	}
	
	public bool SetType {
		get {	return setType; }
		set {	setType = value; }
	}
	
	public float[] Param {
		get {	return currentParam; }
		set {	currentParam = value; }
	}
	
	public bool SetParam {
		get {	return setParam; }
		set {	setParam = value; }
	}
	
	public bool Destructible {
		get {	return currentDestructability; }
		set {	currentDestructability = value; }
	}
	
	public bool SetDestructability {
		get {	return setDestructability; }
		set {	setDestructability = value; }
	}
	
	public bool GraphicsOnly {
		get {	return graphicsOnly; }
		set {	graphicsOnly = value;
				if ( graphicsOnly == true ) {
					Clear					= new Clear_DELEGATE ( _Clear_GFXONLY );
					SetTile					= new SetTile_DELEGATE ( _SetTile_GFXONLY );
					StrokeHorizontalLine	= new StrokeHorizontalLine_DELEGATE ( _StrokeHorizontalLine_GFXONLY );
					StrokeVerticalLine		= new StrokeVerticalLine_DELEGATE ( _StrokeVerticalLine_GFXONLY );
					StrokeScannedLine		= new StrokeScannedLine_DELEGATE ( _StrokeScannedLine_GFXONLY );
					FillRectangle			= new FillRectangle_DELEGATE ( _FillRectangle_GFXONLY );
					FillPolygon				= new FillPolygon_DELEGATE ( _FillPolygon_GFXONLY );
					DrawConsoleSprite		= new DrawConsoleSprite_DELEGATE ( _DrawConsoleSprite_GFXONLY );
				}
			else {
					Clear					= new Clear_DELEGATE ( _Clear_ALL );
					SetTile					= new SetTile_DELEGATE ( _SetTile_ALL );
					StrokeHorizontalLine	= new StrokeHorizontalLine_DELEGATE ( _StrokeHorizontalLine_ALL );
					StrokeVerticalLine		= new StrokeVerticalLine_DELEGATE ( _StrokeVerticalLine_ALL );
					StrokeScannedLine		= new StrokeScannedLine_DELEGATE ( _StrokeScannedLine_ALL );
					FillRectangle			= new FillRectangle_DELEGATE ( _FillRectangle_ALL );
					FillPolygon				= new FillPolygon_DELEGATE ( _FillPolygon_ALL );
					DrawConsoleSprite		= new DrawConsoleSprite_DELEGATE ( _DrawConsoleSprite_ALL );
				}
			}
	}
#endregion

#region Drawing - Clear
	public void _Clear_GFXONLY () {

		int i, j, tileIndex;
		
		if ( drawGlyph == true ) {
			
			tileIndex = 0;
			
			for ( i = 0; i < height; i++ ) {
				
				for ( j = 0; j < width; j++ ) {
				
					fontUV[4 * tileIndex]		= new Vector2 (     clearGlyphTexX,     clearGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 1]	= new Vector2 ( nextClearGlyphTexX,     clearGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 2]	= new Vector2 ( nextClearGlyphTexX, nextClearGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 3]	= new Vector2 (     clearGlyphTexX, nextClearGlyphTexY - glyphVTexOffsetForWindows );
					
					tiles[tileIndex].glyph = clearGlyph;
					
					tileIndex += 1;
					
				}
			}
		}
		
		if ( drawGlyphColor == true ) {
			tileIndex = 0;
			for ( i = 0; i < height; i++ ) {
				for ( j = 0; j < width; j++ ) {
					foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     clearGlyphColor );
					
					tiles[tileIndex].front = clearGlyphColor;
					tileIndex += 1;
				}
			}
		}
		
		if ( drawBackgroundColor == true ) {
			tileIndex = 0;
			for ( i = 0; i < height; i++ ) {
				for ( j = 0; j < width; j++ ) {
					background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     clearBackgroundColor );
					
					tiles[tileIndex].back = clearBackgroundColor;
					tileIndex += 1;
				}
			}
		}
	
	}
	
	public void _Clear_ALL () {
		
		_Clear_GFXONLY ();
		
		int i, j, tileIndex;
		
		if ( setType == true ) {
			tileIndex = 0;
			for ( i = 0; i < height; i++ ) {
				for ( j = 0; j < width; j++ ) {
					tiles[tileIndex].type = currentType;
					tileIndex += 1;
				}
			}
		}
		
		if ( setParam == true ) {
			tileIndex = 0;
			for ( i = 0; i < height; i++ ) {
				for ( j = 0; j < width; j++ ) {
					CopyParams ( currentParam, tiles[tileIndex].param );
					tileIndex += 1;
				}
			}
		}
		
		if ( setDestructability == true ) {
			tileIndex = 0;
			for (i = 0; i < height; i++) {
				for (j = 0; j < width; j++) {
					tiles[tileIndex].destructible = currentDestructability;
					tileIndex += 1;
				}
			}
		}
		
	}
#endregion

#region Drawing - Single Character
	public void _SetTile_GFXONLY ( int x, int y ) {
		
		int	glyphIndex;
		
		glyphIndex = bottomMaxSpriteGlyphs + x + y * width;
		
		if ( drawGlyph == true ) {

			fontUV[4 * glyphIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
			fontUV[4 * glyphIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
			fontUV[4 * glyphIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
			fontUV[4 * glyphIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
			
			tiles[glyphIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
			
		}
		
		if ( drawGlyphColor == true ) {
			foreground.SetPixel ( 	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
			                     (int)colorsTexCoords[CONSOLE_COORDS].y + y,
			                     currentGlyphColor);
			tiles[glyphIndex - bottomMaxSpriteGlyphs].front = currentGlyphColor;
		}
		
		if ( drawBackgroundColor == true ) {
			background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
			                     (int)colorsTexCoords[CONSOLE_COORDS].y + y,
			                     currentBackgroundColor);
			tiles[glyphIndex - bottomMaxSpriteGlyphs].back = currentBackgroundColor;
		}
		
	}
	
	public void _SetTile_ALL ( int x, int y ) {
		
		_SetTile_GFXONLY ( x, y );
		
		int glyphIndex = x + y * width;
		
		if ( setType == true )
			tiles[glyphIndex - bottomMaxSpriteGlyphs].type = currentType;
		
		if (setParam == true)
			CopyParams (currentParam, tiles [glyphIndex - bottomMaxSpriteGlyphs].param);
		
		if ( setDestructability == true )
			tiles[glyphIndex - bottomMaxSpriteGlyphs].destructible = currentDestructability;
		
		
	}
#endregion

#region Drawing - Line
	private void ScanHorizontalLine ( int x1, int x2, int y ) {

		int i, minX, maxX;

		if ( x1 < x2 )	{ minX = x1; maxX = x2; }
		else    		{ minX = x2; maxX = x1; }

		lineLength	= 0;

		for ( i = minX; i <= maxX; i++ ) {

			lineScan[4 * lineLength]		= i;
			lineScan[4 * lineLength + 1]	= y;
			lineScan[4 * lineLength + 2]	= LINE_HORIZONTAL_GLYPH;

			lineLength += 1;

		}

	}

	public void ScanVerticalLine ( int x, int y1, int y2 ) {

		int i, minY, maxY;

		if ( y1 < y2 )	{ minY = y1; maxY = y2; }
		else    		{ minY = y2; maxY = y1; }

		lineLength	= 0;

		for ( i = minY; i <= maxY; i++ ) {

			lineScan[4 * lineLength]		= x;
			lineScan[4 * lineLength + 1]	= i;
			lineScan[4 * lineLength + 2]	= LINE_VERTICAL_GLYPH;

			lineLength += 1;

		}

	}

	public void ScanLine ( int x1, int y1, int x2, int y2 ) {

		int		i, glyphIndex;
		int		x, y, dx, dy, xIncrement, yIncrement, d, stepGlyph;
		float 	tan;


		// Set-up of the bresenham algorythm :
		x = x1;
		y = y1;

		dx = x2 - x1;
		dy = y2 - y1;

		xIncrement = ( dx > 0 ) ? 1 : -1 ;
		yIncrement = ( dy > 0 ) ? 1 : -1 ;


		// Edge cases :
		if ( dy == 0 ) {      // horizontal line
			ScanHorizontalLine ( x1, x2, y1 );
			return;
    	}

		if ( dx == 0 ) {      // vertical line
			ScanVerticalLine ( x1, y1, y2 );
			return;
		}


		// Chosing the "stepping" glyph :
		if ( dx > 0 )
			stepGlyph = dy > 0 ? LINE_BOTTOM_LEFT_TO_TOP_RIGHT_GLYPH : LINE_TOP_LEFT_TO_BOTTOM_RIGHT_GLYPH;
		else
			stepGlyph = dy > 0 ? LINE_TOP_LEFT_TO_BOTTOM_RIGHT_GLYPH : LINE_BOTTOM_LEFT_TO_TOP_RIGHT_GLYPH;


		dx = Mathf.Abs ( dx );
		dy = Mathf.Abs ( dy );


		// Scanning the line :

		// First Point :
		lineScan[0] = x1;
		lineScan[1] = y1;

		tan = Mathf.Abs ( ((float)dy)/dx );

		if ( ( tan > 0.08 ) && ( tan < 5.67 ) )
			lineScan[2] = stepGlyph;
		else {
			if (dx > dy)
				lineScan[2] = LINE_HORIZONTAL_GLYPH;
			else
				lineScan[2] = LINE_VERTICAL_GLYPH;
		}

		lineLength = 1;

		// Rest of the Line :
		if ( dx > dy ) {

			d = dx / 2;

			for ( i = 1; i <= dx; i++ ) {

				x += xIncrement;
				d += dy;

				glyphIndex = lineLength<<2;

				if ( d >= dx ) {

					d -= dx;
					y += yIncrement;

					lineScan[glyphIndex]		= x;
					lineScan[glyphIndex + 1]	= y;
					lineScan[glyphIndex + 2] 	= stepGlyph;

				}
				else {
					lineScan[glyphIndex]		= x;
					lineScan[glyphIndex + 1]	= yIncrement < 0 ? y : y + 1;
					lineScan[glyphIndex + 2]	= LINE_HORIZONTAL_GLYPH;
				}

				lineLength += 1;

			}

		}
		else {

			d = dy / 2;

			for ( i = 1; i <= dy; i++ ) {

				y += yIncrement;
				d += dx;

				glyphIndex = lineLength<<2;

				if ( d >= dy ) {

					d -= dy;
					x += xIncrement;

					lineScan[glyphIndex + 2] = stepGlyph;

			}
			else
				lineScan[glyphIndex + 2]	= LINE_VERTICAL_GLYPH;

				lineScan[glyphIndex]		= x;
				lineScan[glyphIndex + 1]	= y;

				lineLength += 1;

			}

		}

	}

	public void _StrokeHorizontalLine_ALL ( int x1, int x2, int y ) {

		int i, minX, maxX, tileIndexY, tileIndex; //glyphIndexY, glyphIndex;

		if ( x1 < x2 )	{ minX = x1; maxX = x2; }
		else    		{ minX = x2; maxX = x1; }

		tileIndexY = bottomMaxSpriteGlyphs + y * width;

		if ( drawGlyph == true ) {

			tileIndex = tileIndexY;

			for ( i = minX; i <= maxX; i++ ) {

				tileIndex = i + tileIndexY;
			
				if ( drawGlyph == true ) {
			
					fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
	
					tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
				
				}
			
				tileIndex += 1;

			}

		}
		
		tileIndexY -= bottomMaxSpriteGlyphs;

		if ( drawGlyphColor == true ) {
			for ( i = minX; i <= maxX; i++ ) {
				foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + i,
										(int)colorsTexCoords[CONSOLE_COORDS].y + y,
										currentGlyphColor );
				tiles[tileIndexY+i].front = currentGlyphColor;
			}
		}

		if ( drawBackgroundColor == true ) {
			for ( i = minX; i <= maxX; i++ ) {
				background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + i,
										(int)colorsTexCoords[CONSOLE_COORDS].y + y,
				               			currentBackgroundColor	);
				tiles[tileIndexY+i].back = currentBackgroundColor;
			}
		}

		if ( setType == true )
			for ( i = minX; i <= maxX; i++ )
				tiles[tileIndexY+i].type = currentType;
			
		if ( setParam == true )
			for ( i = minX; i <= maxX; i++ )
				CopyParams ( currentParam, tiles[tileIndexY+i].param );
			
		if ( setDestructability == true )
			for ( i = minX; i <= maxX; i++ )
				tiles[tileIndexY+i].destructible = currentDestructability;

	}
	
	public void _StrokeHorizontalLine_GFXONLY ( int x1, int x2, int y ) {
		
		int i, minX, maxX, tileIndexY, tileIndex;
		
		if ( x1 < x2 )	{ minX = x1; maxX = x2; }
		else    		{ minX = x2; maxX = x1; }
		
		tileIndexY = bottomMaxSpriteGlyphs + y * width;
		
		if ( drawGlyph == true ) {
			
			tileIndex = tileIndexY;
			
			for ( i = minX; i <= maxX; i++ ) {
				
				tileIndex = i + tileIndexY;
				
				if ( drawGlyph == true ) {
					
					fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					
					tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
					
				}
				
				tileIndex += 1;
				
			}
			
		}
		
		tileIndexY -= bottomMaxSpriteGlyphs;
		
		if ( drawGlyphColor == true ) {
			for ( i = minX; i <= maxX; i++ ) {
				foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + i,
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + y,
				                     currentGlyphColor );
				tiles[tileIndexY+i].front = currentGlyphColor;
			}
		}
		
		if ( drawBackgroundColor == true ) {
			for ( i = minX; i <= maxX; i++ ) {
				background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + i,
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + y,
				                     currentBackgroundColor	);
				tiles[tileIndexY+i].back = currentBackgroundColor;
			}
		}
		
	}

	public void _StrokeVerticalLine_ALL ( int x, int y1, int y2 ) {

		int i, minY, maxY, tileIndex0, tileIndex1, tileIndex;

		if ( y1 < y2 )	{ minY = y1; maxY = y2; }
		else    		{ minY = y2; maxY = y1; }

		if ( drawGlyph == true ) {

			for ( i = minY; i <= maxY; i++ ) {

				tileIndex = bottomMaxSpriteGlyphs + x + i * width;
	
				fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );

				tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;

			}

		}
		
		tileIndex0 = x + minY * width;
		tileIndex1 = x + maxY * width;

		if ( drawGlyphColor == true ) {
			tileIndex = tileIndex0;
			for ( i = minY; i <= maxY; i++ ) {
				foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
										(int)colorsTexCoords[CONSOLE_COORDS].y + i,
										currentGlyphColor	);
				tiles[tileIndex].front = currentGlyphColor;
				tileIndex += width;
			}
		}

		if ( drawBackgroundColor == true ) {
			tileIndex = tileIndex0;
			for ( i = minY; i <= maxY; i++ ) {
				background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
										(int)colorsTexCoords[CONSOLE_COORDS].y + i,
										currentBackgroundColor	);
				tiles[tileIndex].back = currentBackgroundColor;
				tileIndex += width;
			}
		}
										
		if ( setType == true )
			for ( tileIndex = tileIndex0; tileIndex <= tileIndex1; tileIndex+=width )
				tiles[tileIndex].type = currentType;
		
		if ( setParam == true )
			for ( tileIndex = tileIndex0; tileIndex <= tileIndex1; tileIndex+=width )
				CopyParams ( currentParam, tiles[tileIndex].param );
				
		
		if ( setDestructability == true )
			for ( tileIndex = tileIndex0; tileIndex <= tileIndex1; tileIndex+=width )
				tiles[tileIndex].destructible = currentDestructability;

	}
	
	public void _StrokeVerticalLine_GFXONLY ( int x, int y1, int y2 ) {
		
		int i, minY, maxY, tileIndex;
		
		if ( y1 < y2 )	{ minY = y1; maxY = y2; }
		else    		{ minY = y2; maxY = y1; }
		
		if ( drawGlyph == true ) {
			
			for ( i = minY; i <= maxY; i++ ) {
				
				tileIndex = bottomMaxSpriteGlyphs + x + i * width;
				
				fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
				
				tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
				
			}
			
		}
		
		if ( drawGlyphColor == true ) {
			tileIndex = x + minY * width;
			for ( i = minY; i <= maxY; i++ ) {
				foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
				                     currentGlyphColor	);
				tiles[tileIndex].front = currentGlyphColor;
				tileIndex += width;
			}
		}
		
		if ( drawBackgroundColor == true ) {
			tileIndex = x + minY * width;
			for ( i = minY; i <= maxY; i++ ) {
				background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + x,
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
				                     currentBackgroundColor	);
				tiles[tileIndex].back = currentBackgroundColor;
				tileIndex += width;
			}
		}
		
	}
	
	public void _StrokeScannedLine_GFXONLY ( bool smooth ) {
		
		int i, tileIndex;
		int	previousGlyph;
		
		if ( drawGlyph == true ) {
			
			previousGlyph = this.Glyph;
			
			for ( i = 0; i < lineLength; i++ ) {
				
				tileIndex = bottomMaxSpriteGlyphs + lineScan[4 * i] + lineScan[4 * i + 1] * width;
				
				if ( smooth == true )
					this.Glyph	= lineScan[4 * i + 2];
				
				fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
				fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
				
				tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
				
			}
			
			this.Glyph = previousGlyph;
			
		}
		
		if ( drawGlyphColor == true ) {
			for ( i = 0; i < lineLength; i++ ) {
				foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + lineScan[4 * i],
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + lineScan[4 * i + 1],
				                     currentGlyphColor	);
				tiles[lineScan[4 * i] + width * lineScan[4 * i + 1]].front = currentGlyphColor;
			}
		}
		
		if ( drawBackgroundColor == true ) {
			for ( i = 0; i < lineLength; i++ ) {
				background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + lineScan[4 * i],
				                     (int)colorsTexCoords[CONSOLE_COORDS].y + lineScan[4 * i + 1],
				                     currentBackgroundColor	);
				tiles[lineScan[4 * i] + width * lineScan[4 * i + 1]].back = currentBackgroundColor;
			}
		}
		
	}
	
	public void _StrokeScannedLine_ALL ( bool smooth ) {
		
		_StrokeScannedLine_GFXONLY ( smooth );
		
		int i;
		
		if ( setType == true )
			for ( i = 0; i < lineLength; i++ )
				tiles[lineScan[4 * i] + width * lineScan[4 * i + 1]].type = currentType;
		
		if ( setParam == true )
			for ( i = 0; i < lineLength; i++ )
				CopyParams ( currentParam, tiles[lineScan[4 * i] + width * lineScan[4 * i + 1]].param );
		
		if ( setDestructability == true )
			for ( i = 0; i < lineLength; i++ )
				tiles[lineScan[4 * i] + width * lineScan[4 * i + 1]].destructible = currentDestructability;
		
	}

	public void StrokeLine ( int x1, int y1, int x2, int y2, bool smooth ) {
		ScanLine ( x1, y1, x2, y2 );
		StrokeScannedLine ( smooth );
	}
#endregion

#region Drawing - Rectangle
	public void StrokeRectangle ( int x, int y, int w, int h ) {

		StrokeHorizontalLine ( x, x + w - 1,         y );
		StrokeHorizontalLine ( x, x + w - 1, y + h - 1 );

		StrokeVerticalLine (         x, y, y + h - 1 );
		StrokeVerticalLine ( x + w - 1, y, y + h - 1 );

	}

	public void _FillRectangle_GFXONLY ( int x, int y, int w, int h ) {

		int i, j, tileIndexY, tileIndex;

		if ( drawGlyph == true ) {

			for ( i = y; i < y + h; i++ ) {

				tileIndexY = i * width;

				for ( j = x; j < x + w; j++) {
				
					tileIndex = bottomMaxSpriteGlyphs + j + tileIndexY;

					fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );

					tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;

				}

			}

		}

		if ( drawGlyphColor == true ) {
			for ( i = y; i < y + h; i++ ) {
				tileIndexY = i * width;
				for ( j = x; j < x + w; j++) {
					foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     	(int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     	currentGlyphColor	);
					tiles[tileIndexY + j].front = currentGlyphColor;
				}
			}
		}

		if ( drawBackgroundColor == true ) {
			for ( i = y; i < y + h; i++ ) {
				tileIndexY = i * width;
				for ( j = x; j < x + w; j++) {
					background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     	(int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     	currentBackgroundColor	);
					tiles[tileIndexY + j].back = currentBackgroundColor;
				}
			}
		}

	}
	
	public void _FillRectangle_ALL ( int x, int y, int w, int h ) {
		
		_FillRectangle_GFXONLY (x, y, w, h );
		
		int i, j, tileIndexY;
		
		if ( setType == true ) {
			for ( i = y; i < y + h; i++ ) {
				tileIndexY = i * width;
				for ( j = x; j < x + w; j++)
					tiles[tileIndexY + j].type = currentType;
			}
		}
		
		if ( setParam == true ) {
			for ( i = y; i < y + h; i++ ) {
				tileIndexY = i * width;
				for ( j = x; j < x + w; j++)
					CopyParams ( currentParam, tiles[tileIndexY + j].param );
			}
		}
		
		if ( setDestructability == true ) {
			for ( i = y; i < y + h; i++ ) {
				tileIndexY = i * width;
				for ( j = x; j < x + w; j++)
					tiles[tileIndexY + j].destructible = currentDestructability;
			}
		}
		
	}
#endregion

#region Drawing - Polygon
	private int quadrant( float x, float y ) {

		int quadrantCode;

		quadrantCode = 0;

		if ( Mathf.Abs ( y ) > 0.414213562373095 * Mathf.Abs ( x ) )
			quadrantCode = 1;

		if ( Mathf.Abs ( y ) > 2.41421356237309 * Mathf.Abs ( x ) )
			quadrantCode = 2;

		if ( x >= 0 ) {

			if ( y <= 0 ) {
				if ( quadrantCode == 1 )
					quadrantCode = 7;
			else if ( quadrantCode == 2 )
				quadrantCode = 6;
			}

		}
		else if ( x <= 0 ) {

			if ( y >= 0 ) {
				if ( quadrantCode == 0 )
					quadrantCode = 4;
				else if ( quadrantCode == 1 )
					quadrantCode = 3;
			}
			else
				quadrantCode += 4;

		}

		return quadrantCode;

	}

	private int CornerGlyphCode( float x1, float y1, float xCorner, float yCorner, float x2, float y2 ) {

		int quadrant1, quadrant2, cornerCode;

		quadrant1 = quadrant( x1 - xCorner, y1 - yCorner );
		quadrant2 = quadrant( x2 - xCorner, y2 - yCorner );

		cornerCode = quadrant1 + (quadrant2<<3);

		switch ( cornerCode ) {

		case 2:
			return 192;

		case 4:
			return 196;

		case 6:
			return 218;

		case 11:
			return 86;

		case 13:
			return 47;

		case 15:
			return 60;

		case 16:
			return 192;

		case 20:
			return 217;

		case 22:
			return 179;

		case 25:
			return 86;

		case 29:
			return 62;

		case 31:
			return 92;

		case 32:
			return 196;

		case 34:
			return 217;

		case 38:
			return 191;

		case 41:
			return 47;

		case 43:
			return 62;

		case 47:
			return 94;

		case 48:
			return 218;

		case 50:
			return 179;

		case 52:
			return 191;

		case 57:
			return 60;

		case 59:
			return 92;

		case 61:
			return 94;

		default:
			return 249;

		}

	}

	public void StrokePolygon ( Vector2[] vertices, bool smooth ) {

		int i, previousGlyph;

		// Edges :
		for ( i = 0; i < vertices.Length; i++ )
			StrokeLine ( 	(int)vertices[i].x,
							(int)vertices[i].y,
							(int)vertices[( i + 1 ) % vertices.Length].x,
							(int)vertices[( i + 1 ) % vertices.Length].y,
							smooth );

		// Vertices :
		if ( smooth == true ) {

			previousGlyph = this.Glyph;

			for ( i = 0; i < vertices.Length; i++ ) {

				this.Glyph = CornerGlyphCode(	vertices[( i + vertices.Length - 1 ) % vertices.Length].x, vertices[( i + vertices.Length - 1 ) % vertices.Length].y,
		        								vertices[i].x, vertices[i].y,
		        								vertices[( i + 1 ) % vertices.Length].x, vertices[( i + 1 ) % vertices.Length].y);

				SetTile( (int)vertices[i].x, (int)vertices[i].y );

			}

			this.Glyph = previousGlyph;

		}

	}

	public void ScanPolygonLeftEdge( float x1, float y1, float x2, float y2 ) {

		int		y, yTop, yBottom;
		float	islope, x;

		// Escape horizontal lines :
		//if ( y1 == y2 )
		if ( Mathf.Abs ( y1 - y2 ) < 0.75f )
			return;


		// Calculating useful values for the scanning loop :
		islope	= ( x2 - x1 ) / ( y2 - y1 );

		yTop	= y1 > 0.5f ? Mathf.FloorToInt ( y1 - 0.5f ) : 0;
		yBottom	= Mathf.FloorToInt ( y2 + 0.5f );

		x		= ( yTop + 0.5f - y1 ) * islope + x1;      // + 0.5 : the scan line goes through the middle of the pixel


		// Scan converting :
		for ( y = yTop; y >= yBottom; y-- ) {
			leftScan[y] = Mathf.FloorToInt ( x + 0.5f );
			x -= islope;
		}

	}

	public void ScanPolygonRightEdge(float x1, float y1, float x2, float y2) {

		int		y, yTop, yBottom;
		float	islope, x;

		// Escape horizontal lines :
		//if ( y1 == y2 )
		if ( Mathf.Abs ( y1 - y2 ) < 0.75f )
			return;


		// Calculating useful values for the scanning loop :
		islope	= ( x2 - x1 ) / ( y2 - y1 );

		yTop	= y1 > 0.5f ? Mathf.FloorToInt ( y1 - 0.5f ) : 0;
		yBottom	= Mathf.FloorToInt ( y2 + 0.5f );

		x		= ( yTop + 0.5f - y1 ) * islope + x1;      // + 0.5 : the scan line goes through the middle of the pixel


		// Scan converting :
		for ( y = yTop; y >= yBottom; y-- ) {
			rightScan[y] = x > 0.5f ? Mathf.FloorToInt ( x - 0.5f ) : 0;
			x -= islope;
		}

	}
	
	public void _FillPolygon_GFXONLY ( Vector2[] vertices ) {
		
		int	i, j, windingOrder, tileIndex, tileIndexY;
		int	maxY, minY, maxYIndex, minYIndex;
		int	previousPointX, nextPointX;
		
		// Clearing the rasterizer's buffers :
		for ( i = 0; i < height; i++ ) {
			leftScan[i]		= int.MaxValue;
			rightScan[i]	= 0;
		}
		
		
		// Finding the top and bottom of the polygon :
		
		// Initializing the loop with the first point y value :
		maxYIndex   = 0;
		minYIndex   = 0;
		
		maxY = (int)vertices[maxYIndex].y;
		minY = (int)vertices[minYIndex].y;
		
		for ( i = 1; i < vertices.Length; i++ ) {
			
			// Check the bottom :
			if ( (int)vertices[i].y < minY ) {
				minY		= (int)vertices[i].y;
				minYIndex	= i;
			}
			
			// Check the top :
			if ( (int)vertices[i].y > maxY ) {
				maxY		= (int)vertices[i].y;
				maxYIndex	= i;
			}
			
		}
		
		
		// Discard null height polygons :
		if ( minY == maxY )
			return;
		
		
		// Devise the winding order :
		previousPointX	= (int)vertices[( maxYIndex - 1 + vertices.Length) % vertices.Length].x;
		nextPointX      = (int)vertices[( maxYIndex + 1 ) % vertices.Length].x;
		windingOrder    = previousPointX < nextPointX ? -1 : 1;
		
		
		// Scan the left and right edges of the polygon :
		
		// Left :
		i = 0;
		while ( (int)vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].y != minY ) {
			
			ScanPolygonLeftEdge(	vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].x,
			                    vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].y,
			                    vertices[( ( maxYIndex + ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].x,
			                    vertices[( ( maxYIndex + ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].y );
			
			i += 1;
			
		}
		
		// Right :
		i = 0;
		while ( (int)vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].y != minY ) {
			
			ScanPolygonRightEdge(	vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].x,
			                     vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].y,
			                     vertices[( ( maxYIndex - ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].x,
			                     vertices[( ( maxYIndex - ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].y );
			
			i += 1;
			
		}
		
		
		// Draw the polygon :
		if ( drawGlyph == true ) {
			for (i = minY; i <= maxY; i++ ) {
				
				tileIndexY = i * width;
				
				for ( j = leftScan[i]; j <= rightScan[i]; j++ ) {
					
					tileIndex = bottomMaxSpriteGlyphs + j + tileIndexY;
					
					fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
					
					tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;
					
				}
			}
		}
		
		
		if ( drawGlyphColor == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ ) {
					foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     currentGlyphColor	);
					tiles[tileIndexY + j].front = currentGlyphColor;
				}
			}
		}
		
		
		if ( drawBackgroundColor == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ ) {
					background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     (int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     currentBackgroundColor	);
					tiles[tileIndexY + j].back = currentBackgroundColor;
				}
			}
		}
		
	}

	public void _FillPolygon_ALL ( Vector2[] vertices ) {

		int	i, j, windingOrder, tileIndex, tileIndexY;
		int	maxY, minY, maxYIndex, minYIndex;
		int	previousPointX, nextPointX;

		// Clearing the rasterizer's buffers :
		for ( i = 0; i < height; i++ ) {
			leftScan[i]		= int.MaxValue;
			rightScan[i]	= 0;
		}


		// Finding the top and bottom of the polygon :

		// Initializing the loop with the first point y value :
		maxYIndex   = 0;
		minYIndex   = 0;

		maxY = (int)vertices[maxYIndex].y;
		minY = (int)vertices[minYIndex].y;

		for ( i = 1; i < vertices.Length; i++ ) {

			// Check the bottom :
			if ( (int)vertices[i].y < minY ) {
				minY		= (int)vertices[i].y;
				minYIndex	= i;
			}

			// Check the top :
			if ( (int)vertices[i].y > maxY ) {
				maxY		= (int)vertices[i].y;
				maxYIndex	= i;
			}

		}


		// Discard null height polygons :
		if ( minY == maxY )
			return;

		
		// Devise the winding order :
		previousPointX	= (int)vertices[( maxYIndex - 1 + vertices.Length) % vertices.Length].x;
		nextPointX      = (int)vertices[( maxYIndex + 1 ) % vertices.Length].x;
		windingOrder    = previousPointX < nextPointX ? -1 : 1;

		
		// Scan the left and right edges of the polygon :

		// Left :
		i = 0;
		while ( (int)vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].y != minY ) {
			
			ScanPolygonLeftEdge(	vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].x,
									vertices[( ( maxYIndex + i * windingOrder ) + vertices.Length ) % vertices.Length].y,
									vertices[( ( maxYIndex + ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].x,
									vertices[( ( maxYIndex + ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].y );

			i += 1;

		}

		// Right :
		i = 0;
		while ( (int)vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].y != minY ) {

			ScanPolygonRightEdge(	vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].x,
									vertices[( ( maxYIndex - i * windingOrder ) + vertices.Length ) % vertices.Length].y,
									vertices[( ( maxYIndex - ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].x,
									vertices[( ( maxYIndex - ( i + 1 ) * windingOrder ) + vertices.Length ) % vertices.Length].y );

			i += 1;

		}
		
		
		// Draw the polygon :
		if ( drawGlyph == true ) {
			for (i = minY; i <= maxY; i++ ) {

				tileIndexY = i * width;

				for ( j = leftScan[i]; j <= rightScan[i]; j++ ) {

						tileIndex = bottomMaxSpriteGlyphs + j + tileIndexY;

						fontUV[4 * tileIndex]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
						fontUV[4 * tileIndex + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
						fontUV[4 * tileIndex + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
						fontUV[4 * tileIndex + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );

						tiles[tileIndex - bottomMaxSpriteGlyphs].glyph = currentGlyph;

				}
			}
		}


		if ( drawGlyphColor == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ ) {
					foreground.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     	(int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     	currentGlyphColor	);
					tiles[tileIndexY + j].front = currentGlyphColor;
				}
			}
		}


		if ( drawBackgroundColor == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ ) {
					background.SetPixel (	(int)colorsTexCoords[CONSOLE_COORDS].x + j,
					                     	(int)colorsTexCoords[CONSOLE_COORDS].y + i,
					                     	currentBackgroundColor	);
					tiles[tileIndexY + j].back = currentBackgroundColor;
				}
			}
		}
		
		if ( setType == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ )
					tiles[tileIndexY + j].type = currentType;
			}
		}
		
		if ( setParam == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ )
					CopyParams ( currentParam, tiles[tileIndexY + j].param );
			}
		}
		
		if ( setDestructability == true ) {
			for ( i = minY; i <= maxY; i++ ) {
				tileIndexY = i * width;
				for (j = leftScan[i]; j <= rightScan[i]; j++ )
					tiles[tileIndexY + j].destructible = currentDestructability;
			}
		}

	}
#endregion

#region Drawing - String
	public void DrawString ( int x, int y, string s ) {
		
		int		i, glyphX, glyphY;
		byte[]	stringBytes;

		stringBytes = System.Text.Encoding.Default.GetBytes ( s );

		glyphX = x;
		glyphY = y;
		
		for ( i = 0; i < s.Length; i++ ) {

			switch ( stringBytes[i] ) {
				
				case 10:
					glyphX  = x - 1;
					glyphY -= 1;
					break;
				
				case 47:	// slash
					this.Glyph = 	100 * GlyphCodeToDigit ( stringBytes[i+1] ) +
									 10 * GlyphCodeToDigit ( stringBytes[i+2] ) +
										  GlyphCodeToDigit ( stringBytes[i+3] );
					i += 3;
					SetTile ( glyphX, glyphY );
					break;
				
				default:
					this.Glyph = stringBytes[i];
					SetTile ( glyphX, glyphY );
					break;
				
			}

			glyphX += 1;
				
		}
		
	}
	
	private int GlyphCodeToDigit ( int glyphCode ) {
		if ( ( glyphCode >= DIGIT_0 ) && ( glyphCode <= DIGIT_9 ) )
			return glyphCode - DIGIT_0;
		else
			return QUESTION_MARK;
	}
#endregion
	
#region Drawing - Console Sprite
	public void _DrawConsoleSprite_GFXONLY ( ConsoleSprite sprite, int x, int y ) {
		
		int 	i, j;
		int		tileIndexY, tileIndex;
		int		spriteIndexY, spriteIndex;
		
		int		previousGlyph;
		Color	previousGlyphColor, previousBackgroundColor;
		
		previousGlyph			= this.Glyph;
		previousGlyphColor		= this.GlyphColor;
		previousBackgroundColor	= this.BackgroundColor;
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			tileIndexY		= ( i + y ) * width;
			spriteIndexY	= i * sprite.width;
			
			for ( j = 0; j < sprite.width; j++ ) {
				
				spriteIndex	=  j + spriteIndexY;
				
				if ( ( ( sprite.mask[spriteIndex] == true ) && ( useSpriteMask == true ) ) || ( useSpriteMask == false ) ){
					
					if ( ( x + j < width ) && ( y + i < height ) ) {
						
						tileIndex =  j + x + tileIndexY;
						
						if ( drawGlyph == true ) {
							
							this.Glyph	= sprite.glyph[spriteIndex];
							
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) ]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
							
							tiles[tileIndex].glyph = currentGlyph;
							
						}
						
						if ( drawGlyphColor == true ) {
							foreground.SetPixel ( x + j, y + i, sprite.front[spriteIndex] );
							tiles[tileIndex].front = sprite.front[spriteIndex];
						}
						
						if ( drawBackgroundColor == true ) {
							background.SetPixel ( x + j, y + i, sprite.back[spriteIndex] );
							tiles[tileIndex].back = sprite.back[spriteIndex];
						}
						
					}
					
				}
				
			}
		}
		
		this.Glyph				= previousGlyph;
		this.GlyphColor			= previousGlyphColor;
		this.BackgroundColor	= previousBackgroundColor;
		
	}
	
	public void _DrawConsoleSprite_ALL ( ConsoleSprite sprite, int x, int y ) {
		
		int 	i, j, k;
		int		tileIndexY, tileIndex;
		int		spriteIndexY, spriteIndex;
		
		int		previousGlyph, previousType;
		float[]	previousParam;
		Color	previousGlyphColor, previousBackgroundColor;
		bool	previousDestructability;

		previousGlyph			= this.Glyph;
		previousGlyphColor		= this.GlyphColor;
		previousBackgroundColor	= this.BackgroundColor;
		previousType			= this.Type;
		
		previousParam 			= new float[Tile.DEFAULT_PARAM_SIZE] ;
		CopyParams ( currentParam, previousParam );				
		
		previousDestructability	= this.Destructible;
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			tileIndexY		= ( i + y ) * width;
			spriteIndexY	= i * sprite.width;
			
			for ( j = 0; j < sprite.width; j++ ) {
						
				spriteIndex	=  j + spriteIndexY;
				
				if ( ( ( sprite.mask[spriteIndex] == true ) && ( useSpriteMask == true ) ) || ( useSpriteMask == false ) ){
				
					if ( ( x + j < width ) && ( y + i < height ) ) {
						
						tileIndex =  j + x + tileIndexY;
						
						if ( drawGlyph == true ) {
							
							this.Glyph	= sprite.glyph[spriteIndex];
	
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) ]		= new Vector2 ( currentGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 1]	= new Vector2 (    nextGlyphTexX, currentGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 2]	= new Vector2 (    nextGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
							fontUV[4 * ( tileIndex + bottomMaxSpriteGlyphs ) + 3]	= new Vector2 ( currentGlyphTexX,    nextGlyphTexY - glyphVTexOffsetForWindows );
	
							tiles[tileIndex].glyph = currentGlyph;
	
						}
						
						if ( drawGlyphColor == true ) {
							foreground.SetPixel ( x + j, y + i, sprite.front[spriteIndex] );
							tiles[tileIndex].front = sprite.front[spriteIndex];
						}
						
						if ( drawBackgroundColor == true ) {
							background.SetPixel ( x + j, y + i, sprite.back[spriteIndex] );
							tiles[tileIndex].back = sprite.back[spriteIndex];
						}
						
						if ( setType == true )
							tiles[tileIndex].type = sprite.type[spriteIndex];

						if ( setParam == true )
							for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
								tiles[tileIndex].param[k] = sprite.param[spriteIndex * Tile.DEFAULT_PARAM_SIZE + k];

						if ( setDestructability == true )
							tiles[tileIndex].destructible = sprite.destructible[spriteIndex];
					
					}
					
				}
				
			}
		}
		
		this.Glyph				= previousGlyph;
		this.GlyphColor			= previousGlyphColor;
		this.BackgroundColor	= previousBackgroundColor;
		this.Type				= previousType;
		
		CopyParams ( previousParam, currentParam );
		
		this.Destructible		= previousDestructability;

	}
	
	public static ConsoleSprite RotateSprite90DegreesLeft ( ConsoleSprite sprite ) {
		
		int				i, j, k;
		int				spriteIndexY, spriteIndex;
		int				rotSpriteIndex;
		ConsoleSprite	rotatedSprite;
		
		rotatedSprite = ConsoleSprite.NewSprite ( sprite.height, sprite.width );
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			spriteIndexY	= i * sprite.width;
			
			for ( j = 0; j < sprite.width; j++ ) {
				
				spriteIndex		= j + spriteIndexY;
				rotSpriteIndex	= sprite.height - i - 1 + j * sprite.height;

				rotatedSprite.type[rotSpriteIndex]			= sprite.type[spriteIndex];
				rotatedSprite.glyph[rotSpriteIndex]			= sprite.glyph[spriteIndex];
				
				rotatedSprite.front[rotSpriteIndex]			= sprite.front[spriteIndex];
				rotatedSprite.back[rotSpriteIndex]			= sprite.back[spriteIndex];
				
				for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
					rotatedSprite.param[rotSpriteIndex * Tile.DEFAULT_PARAM_SIZE + k] = sprite.param[spriteIndex * Tile.DEFAULT_PARAM_SIZE + k];
				
				rotatedSprite.destructible[rotSpriteIndex]	= sprite.destructible[spriteIndex];

				rotatedSprite.mask[rotSpriteIndex]			= sprite.mask[spriteIndex];

			}
		}
		
		return rotatedSprite;
		
	}
	
	public static ConsoleSprite RotateSprite90DegreesRight ( ConsoleSprite sprite ) {
		
		int				i, j, k;
		int				spriteIndexY, spriteIndex;
		int				rotSpriteIndex;
		ConsoleSprite	rotatedSprite;
		
		rotatedSprite = ConsoleSprite.NewSprite ( sprite.height, sprite.width );
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			spriteIndexY	= i * sprite.width;

			for ( j = 0; j < sprite.width; j++ ) {
				
				spriteIndex		= j + spriteIndexY;
				rotSpriteIndex	= i + ( sprite.width - j - 1 )  * sprite.height;
				
				rotatedSprite.type[rotSpriteIndex]			= sprite.type[spriteIndex];
				rotatedSprite.glyph[rotSpriteIndex]			= sprite.glyph[spriteIndex];
				
				rotatedSprite.front[rotSpriteIndex]			= sprite.front[spriteIndex];
				rotatedSprite.back[rotSpriteIndex]			= sprite.back[spriteIndex];
				
				for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
					rotatedSprite.param[rotSpriteIndex * Tile.DEFAULT_PARAM_SIZE + k] = sprite.param[spriteIndex * Tile.DEFAULT_PARAM_SIZE + k];
				
				rotatedSprite.destructible[rotSpriteIndex]	= sprite.destructible[spriteIndex];
				
				rotatedSprite.mask[rotSpriteIndex]			= sprite.mask[spriteIndex];
	
			}
		}
		
		return rotatedSprite;
		
	}
	
	public static ConsoleSprite FlipSpriteHorizontal ( ConsoleSprite sprite ) {
		
		int				i, j, k;
		int				spriteIndexY, spriteIndex;
		int				flippedSpriteIndex;
		ConsoleSprite	flippedSprite;
		
		flippedSprite = ConsoleSprite.NewSprite ( sprite.width, sprite.height );
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			spriteIndexY	= i * sprite.width;

			for ( j = 0; j < sprite.width; j++ ) {
				
				spriteIndex			= j + spriteIndexY;
				flippedSpriteIndex	= sprite.width - j - 1 + spriteIndexY;
				
				flippedSprite.type[flippedSpriteIndex]			= sprite.type[spriteIndex];
				flippedSprite.glyph[flippedSpriteIndex]			= sprite.glyph[spriteIndex];
				
				flippedSprite.front[flippedSpriteIndex]			= sprite.front[spriteIndex];
				flippedSprite.back[flippedSpriteIndex]			= sprite.back[spriteIndex];
				
				for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
					flippedSprite.param[flippedSpriteIndex * Tile.DEFAULT_PARAM_SIZE + k] = sprite.param[spriteIndex * Tile.DEFAULT_PARAM_SIZE + k];
				
				flippedSprite.destructible[flippedSpriteIndex]	= sprite.destructible[spriteIndex];
				
				flippedSprite.mask[flippedSpriteIndex]			= sprite.mask[spriteIndex];
				
			}
		}
		
		return flippedSprite;
	
	}
	
	public static ConsoleSprite FlipSpriteVertical ( ConsoleSprite sprite ) {
		
		int				i, j, k;
		int				spriteIndexY, spriteIndex;
		int				flippedSpriteIndexY, flippedSpriteIndex;
		ConsoleSprite	flippedSprite;
		
		flippedSprite = ConsoleSprite.NewSprite ( sprite.width, sprite.height );
		
		for ( i = 0; i < sprite.height; i++ ) {
			
			spriteIndexY		= i * sprite.width;
			flippedSpriteIndexY	= ( sprite.height - i - 1 ) * sprite.width;

			for ( j = 0; j < sprite.width; j++ ) {
				
				spriteIndex			= j + spriteIndexY;
				flippedSpriteIndex	= j + flippedSpriteIndexY;
				
				flippedSprite.type[flippedSpriteIndex]			= sprite.type[spriteIndex];
				flippedSprite.glyph[flippedSpriteIndex]			= sprite.glyph[spriteIndex];
				
				flippedSprite.front[flippedSpriteIndex]			= sprite.front[spriteIndex];
				flippedSprite.back[flippedSpriteIndex]			= sprite.back[spriteIndex];
				
				for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
					flippedSprite.param[flippedSpriteIndex * Tile.DEFAULT_PARAM_SIZE + k] = sprite.param[spriteIndex * Tile.DEFAULT_PARAM_SIZE + k];
				
				flippedSprite.destructible[flippedSpriteIndex]	= sprite.destructible[spriteIndex];
				
				flippedSprite.mask[flippedSpriteIndex]			= sprite.mask[spriteIndex];

			}
		}
		
		return flippedSprite;
	
	}
#endregion
	
#region Drawing - Free Sprites
	public void DrawSprite ( int sprite, float x, float y, bool topOrBottom ) {
		
		int			i, j;
		int			spriteIndex, spriteYIndex, spriteGlyphIndex;
		Vector3		position;
		Vector2		colorsU0V0;
		CslPoint	fontCoords;
		int			fontX, fontY;
		
		position			= new Vector3 ( x, y, 0.0f );
		
		colorsU0V0			= new Vector2 ( colorsTexCoords[FIRST_SPRITE_COORDS + sprite].x, colorsTexCoords[FIRST_SPRITE_COORDS + sprite].y );
		
		spriteGlyphIndex	= topOrBottom ? bottomMaxSpriteGlyphs + width * height + topSpriteGlyphsCount : bottomSpriteGlyphsCount;
		
		for ( i = 0; i < sprites[sprite].height; i++ ) {
			
			spriteYIndex = i * sprites[sprite].width;
			
			for ( j = 0; j < sprites[sprite].width; j++ ) {
				
				spriteIndex	= spriteYIndex + j;
				
				// Vertices :
				vertices[4 * spriteGlyphIndex]		= position + new Vector3 (         j * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 1]	= position + new Vector3 ( ( j + 1 ) * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 2]	= position + new Vector3 ( ( j + 1 ) * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 3]	= position + new Vector3 (         j * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );
	
				// Font UV Coordinates :
				fontCoords		= FontCoordsFromIndex ( sprites[sprite].glyph[spriteIndex] );
				fontX			= fontCoords.x;
				fontY			= fontCoords.y;
	
				fontUV[4 * spriteGlyphIndex]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
	
				// Color UV Coordinates :
				colorUV[4 * spriteGlyphIndex]		= colorsU0V0 + new Vector2 (         j * colorTexWidth,         i * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 1]	= colorsU0V0 + new Vector2 ( ( j + 1 ) * colorTexWidth,         i * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 2]	= colorsU0V0 + new Vector2 ( ( j + 1 ) * colorTexWidth, ( i + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 3]	= colorsU0V0 + new Vector2 (         j * colorTexWidth, ( i + 1 ) * colorTexHeight - colorVTexOffsetForWindows );				
				
				spriteGlyphIndex += 1;
				
				if ( topOrBottom == TOP )
					topSpriteGlyphsCount	+= 1;
				else
					bottomSpriteGlyphsCount	+= 1;

			}

		}
		
	}

	public void DrawAnimation ( ConsoleAnimation animation, float x, float y, bool topOrBottom ) {
		
		int				i, j;
		int				spriteIndex, spriteYIndex, spriteGlyphIndex;
		Vector3			position;
		Vector2			colorsU0V0;
		ConsoleSprite	sprite;
		IntRect			frame;
		CslPoint		fontCoords;
		int				fontX, fontY;
		
		position			= new Vector3 ( x, y, 0.0f );
		
		sprite				= sprites[animation.spriteIndex];
		frame				= animation.Frame;
		
		colorsU0V0			= new Vector2 (	colorsTexCoords[FIRST_SPRITE_COORDS + animation.spriteIndex].x,
											colorsTexCoords[FIRST_SPRITE_COORDS + animation.spriteIndex].y );
		
		spriteGlyphIndex	= topOrBottom ? bottomMaxSpriteGlyphs + width * height + topSpriteGlyphsCount : bottomSpriteGlyphsCount;
		
		
		for ( i = 0; i < frame.height; i++ ) {
		
			spriteYIndex = ( i + frame.y ) * sprite.width;
		
			for ( j = 0; j < frame.width; j++ ) {
				
				spriteIndex	= spriteYIndex + frame.x + j;

				// Vertices :
				vertices[4 * spriteGlyphIndex]		= position + new Vector3 (         j * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 1]	= position + new Vector3 ( ( j + 1 ) * currentFont.glyphWidth,         i * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 2]	= position + new Vector3 ( ( j + 1 ) * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );
				vertices[4 * spriteGlyphIndex + 3]	= position + new Vector3 (         j * currentFont.glyphWidth, ( i + 1 ) * currentFont.glyphHeight, 0.0f );
	
				// Font UV Coordinates :
				fontCoords		= FontCoordsFromIndex ( sprite.glyph[spriteIndex] );
				fontX			= fontCoords.x;
				fontY			= fontCoords.y;
	
				fontUV[4 * spriteGlyphIndex]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
	
				// Color UV Coordinates :
				colorUV[4 * spriteGlyphIndex]		= colorsU0V0 + new Vector2 (     ( j + frame.x ) * colorTexWidth,     ( i + frame.y ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 1]	= colorsU0V0 + new Vector2 ( ( j + frame.x + 1 ) * colorTexWidth,     ( i + frame.y ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 2]	= colorsU0V0 + new Vector2 ( ( j + frame.x + 1 ) * colorTexWidth, ( i + frame.y + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 3]	= colorsU0V0 + new Vector2 (     ( j + frame.x ) * colorTexWidth, ( i + frame.y + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				
				spriteGlyphIndex += 1;
				
				if ( topOrBottom == TOP )
					topSpriteGlyphsCount	+= 1;
				else
					bottomSpriteGlyphsCount	+= 1;

			}

		}
		
	}

	public void DrawRotatedAnimation ( ConsoleAnimation animation, float x, float y, float angle, bool topOrBottom ) {
		
		int				i, j;
		int				spriteIndex, spriteYIndex, spriteGlyphIndex;
		int				frameHalfWidth, frameHalfHeight;
		Vector3			position, offset, uOffset, vOffset;
		Vector2			colorsU0V0;
		ConsoleSprite	sprite;
		IntRect			frame;
		CslPoint		fontCoords;
		int				fontX, fontY;
		
		position			= new Vector3 ( x, y, 0.0f );
		
		uOffset				= currentFont.glyphWidth * ( Quaternion.Euler ( 0.0f, 0.0f, angle ) * Vector3.down );
		vOffset				= currentFont.glyphHeight * ( Quaternion.Euler ( 0.0f, 0.0f, angle ) * Vector3.right );
		
		sprite				= sprites[animation.spriteIndex];
		frame				= animation.Frame;
		
		frameHalfWidth		= frame.width  / 2;
		frameHalfHeight		= frame.height / 2;
		
		offset				= new Vector3 ( frameHalfWidth * currentFont.glyphWidth, frameHalfHeight * currentFont.glyphHeight, 0.0f );
		
		colorsU0V0			= new Vector2 (	colorsTexCoords[FIRST_SPRITE_COORDS + animation.spriteIndex].x,
											colorsTexCoords[FIRST_SPRITE_COORDS + animation.spriteIndex].y );
		
		spriteGlyphIndex	= topOrBottom ? bottomMaxSpriteGlyphs + width * height + topSpriteGlyphsCount : bottomSpriteGlyphsCount;
		
		
		for ( i = 0; i < frame.height; i++ ) {
		
			spriteYIndex = ( i + frame.y ) * sprite.width;
		
			for ( j = 0; j < frame.width; j++ ) {
				
				spriteIndex	= spriteYIndex + frame.x + j;

				// Vertices :
				vertices[4 * spriteGlyphIndex]		= position +     ( j - frameHalfWidth ) * uOffset +     ( i - frameHalfHeight ) * vOffset + offset;
				vertices[4 * spriteGlyphIndex + 1]	= position + ( j + 1 - frameHalfWidth ) * uOffset +     ( i - frameHalfHeight ) * vOffset + offset;
				vertices[4 * spriteGlyphIndex + 2]	= position + ( j + 1 - frameHalfWidth ) * uOffset + ( i + 1 - frameHalfHeight ) * vOffset + offset;
				vertices[4 * spriteGlyphIndex + 3]	= position +     ( j - frameHalfWidth ) * uOffset + ( i + 1 - frameHalfHeight ) * vOffset + offset;
	
				// Font UV Coordinates :
				fontCoords		= FontCoordsFromIndex ( sprite.glyph[spriteIndex] );
				fontX			= fontCoords.x;
				fontY			= fontCoords.y;
	
				fontUV[4 * spriteGlyphIndex]		= new Vector2 (         fontX * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 1]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth,         fontY * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 2]	= new Vector2 ( ( fontX + 1 ) * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
				fontUV[4 * spriteGlyphIndex + 3]	= new Vector2 (         fontX * glyphTexWidth, ( fontY + 1 ) * glyphTexHeight - glyphVTexOffsetForWindows );
	
				// Color UV Coordinates :
				colorUV[4 * spriteGlyphIndex]		= colorsU0V0 + new Vector2 (     ( j + frame.x ) * colorTexWidth,     ( i + frame.y ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 1]	= colorsU0V0 + new Vector2 ( ( j + frame.x + 1 ) * colorTexWidth,     ( i + frame.y ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 2]	= colorsU0V0 + new Vector2 ( ( j + frame.x + 1 ) * colorTexWidth, ( i + frame.y + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				colorUV[4 * spriteGlyphIndex + 3]	= colorsU0V0 + new Vector2 (     ( j + frame.x ) * colorTexWidth, ( i + frame.y + 1 ) * colorTexHeight - colorVTexOffsetForWindows );
				
				spriteGlyphIndex += 1;
				
				if ( topOrBottom == TOP )
					topSpriteGlyphsCount	+= 1;
				else
					bottomSpriteGlyphsCount	+= 1;

			}

		}
		
	}
	
	public void ClearSprite ( int sprite, bool topOrBottom ) {
		
		int spriteGlyphIndex = topOrBottom ? bottomMaxSpriteGlyphs + width * height + sprite : sprite;
		
		vertices[4 * spriteGlyphIndex]		= hiddingPosition;
		vertices[4 * spriteGlyphIndex + 1]	= hiddingPosition + new Vector3 ( 0.0f, 0.0f, 0.0f );
		vertices[4 * spriteGlyphIndex + 2]	= hiddingPosition + new Vector3 ( 0.0f, 0.0f, 0.0f );
		vertices[4 * spriteGlyphIndex + 3]	= hiddingPosition + new Vector3 ( 0.0f, 0.0f, 0.0f );
		
	}
	
	public void ClearUnusedTopSprites () {
		for ( int i = topSpriteGlyphsCount; i < lastTopSpriteGlyphsCount; i++ )
			ClearSprite ( i, TOP );
	}
	
	public void ClearUnusedBottomSprites () {
		for ( int i = bottomSpriteGlyphsCount; i < lastBottomSpriteGlyphsCount; i++ )
			ClearSprite ( i, BOTTOM );
	}
	
	public void ClearUnusedSprites () {
		ClearUnusedBottomSprites ();
		ClearUnusedTopSprites ();
	}
#endregion

#region Utilities
	CslPoint FontCoordsFromIndex ( int index ) {

		int x, y;

		x = index % currentFont.fontGlyphWidth;
		y = ( index - x ) / currentFont.fontGlyphWidth;

		return new CslPoint ( x, y );

	}

	public static void CopyParams ( float[] src, float[] dest ) {
		for ( int i = 0; i < Tile.DEFAULT_PARAM_SIZE; i++ )
			dest [i] = src [i];
	}
#endregion
	
}
