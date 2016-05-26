using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RotatingCube : MonoBehaviour {
	
	// Delegates :
	private delegate void ConsoleUpdate ();
	
	// Camera :
	public	float		depthOfField	= 37.5f;
	private Vector3[]	projectedVertices;
	
	
	// Colors :
	private	Color	white	= new Color ( 1.0f, 1.0f, 1.0f, 1.0f );
	private	Color	gray	= new Color ( 0.4f, 0.4f, 0.4f, 1.0f );
	private	Color	black	= new Color ( 0.0f, 0.0f, 0.0f, 1.0f );


	// Cube :
	public	float		edgeLength	= 15.0f;
	
	private	Vector3[]	vertices	= new Vector3[8] 	{	new Vector3 ( -0.5f, -0.5f, -0.5f ),
															new Vector3 ( -0.5f,  0.5f, -0.5f ),
															new Vector3 (  0.5f,  0.5f, -0.5f ),
															new Vector3 (  0.5f, -0.5f, -0.5f ),
															new Vector3 ( -0.5f, -0.5f,  0.5f ),
															new Vector3 ( -0.5f,  0.5f,  0.5f ),
															new Vector3 (  0.5f,  0.5f,  0.5f ),
															new Vector3 (  0.5f, -0.5f,  0.5f ) };
	
	private int[,]		edges		= new int[12,2]		{	{ 0, 1 },
															{ 1, 2 },
															{ 2, 3 },
															{ 3, 0 },
															{ 4, 5 },
															{ 5, 6 },
															{ 6, 7 },
															{ 7, 4 },
															{ 1, 5 },
															{ 6, 2 },
															{ 0, 4 },
															{ 7, 3 } };
	
	private	int[,]		faces		= new int[6,4]		{ 	{ 0, 1, 2, 3 },
															{ 3, 2, 6, 7 },
															{ 7, 6, 5, 4 },
															{ 4, 5, 1, 0 },
															{ 0, 4, 7, 3 },
															{ 1, 5, 6, 2 } };
	
	private Vector3[]	normals		= new Vector3[6]	{	new Vector3 (  0.0f,  0.0f, -1.0f ),
															new Vector3 (  1.0f,  0.0f,  0.0f ),
															new Vector3 (  0.0f,  0.0f,  1.0f ),
															new Vector3 ( -1.0f,  0.0f,  0.0f ),
															new Vector3 (  0.0f, -1.0f,  0.0f ),
															new Vector3 (  0.0f,  1.0f,  0.0f ) };
	
	private	Vector3[]	centers		= new Vector3[6]	{	new Vector3 (  0.0f,  0.0f, -0.5f ),
															new Vector3 (  0.5f,  0.0f,  0.0f ),
															new Vector3 (  0.0f,  0.0f,  0.5f ),
															new Vector3 ( -0.5f,  0.0f,  0.0f ),
															new Vector3 (  0.0f, -0.5f,  0.0f ),
															new Vector3 (  0.0f,  0.5f,  0.0f ) };	
	
	private	int[]		glyphs				= new int[6]	{ 49, 50, 51, 52, 53, 54 };
	private Color[]		glyphColors			= new Color[] 	{ 	new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),		// red
																new Color ( 1.0f, 0.0f, 1.0f, 1.0f ),		// purple
																new Color ( 0.0f, 0.0f, 1.0f, 1.0f ),		// blue
																new Color ( 0.0f, 1.0f, 0.0f, 1.0f ),		// green
																new Color ( 1.0f, 1.0f, 0.0f, 1.0f ),		// yellow
																new Color ( 1.0f, 0.5f, 0.0f, 1.0f ) };		// orange										
	private Color		backgroundColor		= new Color ( 0.0f, 0.0f, 0.0f, 1.0f );
	
	
	// Background :
	private	const	float	BACKGROUND_GLYPH_SWITCH_DURATION	= 0.05f;
	
	private	float	backgroundGlyphSwitchTime	= 0.0f;
	private int		backgroundGlyphIndex		= 0;
	private	int[]	backgroundGlyphs			= new int[] { 0, 250, 249, 7, 254, 4, 254, 7, 249, 250 };
	
	
	// Rotation :
	private	float		xAngle, yAngle, zAngle;
	public	float		xRotSpeed, yRotSpeed, zRotSpeed;
	private	Vector3[]	rotatedVertices;
	private	Vector3[]	rotatedNormals;
	private Vector3[]	rotatedCenters;
	
	// Display :
	private	Console 		console;
	private ConsoleUpdate[]	consoleUpdates;
	private	int				updateIndex;
	
	public	ConsoleSprite[]	spheres;
	
	void Start () {
		
		console					= GetComponent<Console>();
		console.GraphicsOnly	= true;
		
		consoleUpdates	= new ConsoleUpdate[3] {	new ConsoleUpdate ( wireframeUpdate ),
													new ConsoleUpdate ( flatUpdate ),
													new ConsoleUpdate ( spriteUpdate )		};

		xAngle	= 0.0f;
		yAngle	= 0.0f;
		zAngle	= 0.0f;
		
		rotatedVertices		= new Vector3[8];
		rotatedNormals		= new Vector3[6];
		rotatedCenters		= new Vector3[6];
		projectedVertices	= new Vector3[8];
		
		updateIndex	= 0;

	}
	
	void Update () {
	
		int i;

		Quaternion	rotation;
		
		if ( Input.GetKeyDown ( KeyCode.Space ) )
			updateIndex = ( updateIndex + 1 ) % consoleUpdates.Length;
		
		xAngle	+= xRotSpeed * Time.deltaTime;
		yAngle	+= yRotSpeed * Time.deltaTime;
		zAngle	+= zRotSpeed * Time.deltaTime;

		rotation	= Quaternion.Euler ( xAngle, yAngle, zAngle );
		
		for ( i = 0; i < 8; i++ ) {
			
			rotatedVertices[i] 		= edgeLength * ( rotation * vertices[i] );
			rotatedVertices[i].z   += 30.0f;
			
			projectedVertices[i].x	= depthOfField * rotatedVertices[i].x / rotatedVertices[i].z + 50.0f;
			projectedVertices[i].y	= depthOfField * rotatedVertices[i].y / rotatedVertices[i].z + 37.5f;
			projectedVertices[i].z	= rotatedVertices[i].z;
			
		}
		
		for ( i = 0; i < 6; i++ ) {
			rotatedNormals[i]		= rotation * normals[i];
			rotatedCenters[i]		= edgeLength * ( rotation * centers[i] );
			rotatedCenters[i].z    += 30;
		}
		
		console.PrepareUpdate ();
		
		consoleUpdates[updateIndex] ();
		
		console.EndUpdate ();
		
	}
	
	private void wireframeUpdate () {
		
		int i;
		
		console.Glyph			= 0;
		console.GlyphColor		= white;
		console.BackgroundColor	= black;
		console.FillRectangle ( 9, 9, 82, 57 );
			
		for ( i = 0; i < 12; i++ )
			console.StrokeLine (	(int)projectedVertices[ edges[i,0] ].x, (int)projectedVertices[ edges[i,0] ].y,
									(int)projectedVertices[ edges[i,1] ].x, (int)projectedVertices[ edges[i,1] ].y,
									true );
		
		console.Glyph			= 0;
		console.GlyphColor		= Color.black;
		console.BackgroundColor	= Color.black;
		console.FillRectangle ( 12, 12, 41, 4 );
		console.GlyphColor		= Color.white;
		console.BackgroundColor	= Color.black;
		console.DrawString ( 13, 14, "Graphics in the inner frame are\nprogrammatically updated ( wireframe )." );
		
	}
	
	private void flatUpdate () {
	
		int 		i, faceIndex, highestZIndex;
		float		faceZ;
		List<float>	facesZ;
		List<int>	faceIndices, sortedFaces;
			
		if ( Time.time - backgroundGlyphSwitchTime > BACKGROUND_GLYPH_SWITCH_DURATION ) {
			backgroundGlyphIndex = ( backgroundGlyphIndex + 1 ) % backgroundGlyphs.Length;
			backgroundGlyphSwitchTime = Time.time;
		}

		console.Glyph			= backgroundGlyphs [ backgroundGlyphIndex ];
		console.GlyphColor		= gray;
		console.BackgroundColor	= black;
		console.FillRectangle ( 9, 9, 82, 57 );
		
		faceIndex	= 0;
		faceIndices	= new List<int> ( 0 );
		sortedFaces	= new List<int> ( 0 );
		facesZ		= new List<float> ( 0 );
		
		for ( i = 0; i < 6; i++ ) {
			faceIndices.Add ( i );
			facesZ.Add ( rotatedCenters[i].z );
		}
		
		while ( faceIndices.Count > 0 ) {
				
			highestZIndex	= -1;
			faceIndex		= -1;
			faceZ			= float.MinValue;
			
			for ( i = 0; i < faceIndices.Count; i++ ) {
				if ( facesZ[i] > faceZ ) {
					highestZIndex	= i;
					faceIndex		= faceIndices[i];
					faceZ			= facesZ[i];
				}
			}

			sortedFaces.Add ( faceIndex );

			faceIndices.RemoveAt ( highestZIndex );
			facesZ.RemoveAt ( highestZIndex );
			
		}
		
		for ( i = 0; i < sortedFaces.Count; i++ ) {
			
			if ( rotatedNormals[ sortedFaces[i] ].z < 0.0f ) {
			
				console.Glyph			= glyphs[ sortedFaces[i] ];
				console.GlyphColor		= glyphColors[ sortedFaces[i] ];
				console.BackgroundColor	= backgroundColor;
				
				Vector2[] face	= new Vector2[] {	projectedVertices[ faces[sortedFaces[i],0] ],
													projectedVertices[ faces[sortedFaces[i],1] ],
													projectedVertices[ faces[sortedFaces[i],2] ],
													projectedVertices[ faces[sortedFaces[i],3] ] };
			
				console.FillPolygon ( face );
				
			}
			
		}
		
		console.Glyph			= 0;
		console.GlyphColor		= Color.black;
		console.BackgroundColor	= Color.black;
		console.FillRectangle ( 12, 12, 43, 4 );
		console.GlyphColor		= Color.white;
		console.BackgroundColor	= Color.black;
		console.DrawString ( 13, 14, "Graphics in the inner frame are\nprogrammatically updated ( flat shades )." );
		
	}
	
	private void spriteUpdate () {
		
		int 		i, edgeIndex, highestZIndex;
		float		edgeZ;
		List<float>	edgesZ;
		List<int>	edgeIndices, sortedEdges;
		
		edgeIndex	= 0;
		edgeIndices	= new List<int> ( 0 );
		sortedEdges	= new List<int> ( 0 );
		edgesZ		= new List<float> ( 0 );
		
		for ( i = 0; i < 12; i++ ) {
			edgeIndices.Add ( i );
			edgesZ.Add ( ( rotatedVertices[edges[i,0]].z + rotatedVertices[edges[i,1]].z ) / 2.0f );
		}
		
		while ( edgeIndices.Count > 0 ) {
				
			highestZIndex	= -1;
			edgeIndex		= -1;
			edgeZ			= float.MinValue;
			
			for ( i = 0; i < edgeIndices.Count; i++ ) {
				if ( edgesZ[i] > edgeZ ) {
					highestZIndex	= i;
					edgeIndex		= edgeIndices[i];
					edgeZ			= edgesZ[i];
				}
			}

			sortedEdges.Add ( edgeIndex );

			edgeIndices.RemoveAt ( highestZIndex );
			edgesZ.RemoveAt ( highestZIndex );
			
		}
		
		console.Glyph			= 0;
		console.GlyphColor		= black;
		console.BackgroundColor	= black;
		console.FillRectangle ( 9, 9, 82, 57 );
		
		for ( i = 0; i < 12; i++ ) {
			if ( projectedVertices[edges[sortedEdges[i],0]].z < projectedVertices[edges[sortedEdges[i],1]].z )
				DrawSpriteEdge ( projectedVertices[edges[sortedEdges[i],1]], projectedVertices[edges[sortedEdges[i],0]], 3 );
			else
				DrawSpriteEdge ( projectedVertices[edges[sortedEdges[i],0]], projectedVertices[edges[sortedEdges[i],1]], 3 );
		}
		
		console.GlyphColor		= Color.white;
		console.BackgroundColor	= Color.black;
		console.DrawString ( 13, 14, "Graphics in the inner frame are\nprogrammatically updated ( sprites )." );
		
	}
	
	private void DrawSpriteEdge ( Vector3 p1, Vector3 p2, int div ) {
		
		int i;
		float x, y, z, divX, divY, divZ;
		
		divX = ( p2.x - p1.x ) / div;
		divY = ( p2.y - p1.y ) / div;
		divZ = ( p2.z - p1.z ) / div;
		
		for ( i = 0; i <= div; i++ ) {
			
			x = p1.x + i * divX;
			y = p1.y + i * divY;
			z = p1.z + i * divZ;
			
			if ( z < 25.6f )
				console.DrawConsoleSprite ( spheres[0], (int)x - 4, (int)y - 4 );
			else if ( z < 34.3f )
				console.DrawConsoleSprite ( spheres[1], (int)x - 3, (int)y - 3 );
			else
				console.DrawConsoleSprite ( spheres[2], (int)x - 2, (int)y - 2 );
			
		}

	}
	
	void PrintFloatArray ( float[] a ) {
		int i;
		string s = "" + a[0];
		
		for (i = 1; i < a.Length; i++ )
			s+=" - " + a[i] ;
		
		Debug.Log ( s );

	}
	
}
