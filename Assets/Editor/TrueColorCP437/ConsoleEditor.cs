using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor (typeof(Console))]

public class ConsoleEditor : Editor {

#region Constants
	private	const	int	LEFT_BUTTON				= 0;
	
	private	const	int	PEN_TOOL_INDEX			= 0;
	private	const	int	LINE_TOOL_INDEX			= 1;
	private	const	int	STROKE_RECT_TOOL_INDEX	= 2;
	private	const	int	FILL_RECT_TOOL_INDEX	= 3;
	private	const	int	STAMP_TOOL_INDEX		= 4;
#endregion
	
#region Instance Variables
	private Console			console;
	private Transform		consoleTransform;
	
	private	int				width;
	private	int 			height;
	
	private	int				selectedFontIndex;
	
	private	Color			gridColor			= new Color ( 0.3f, 0.3f, 0.3f, 1.0f );
	private	Color			glyphFrameColor		= new Color ( 1.0f, 0.0f, 0.0f, 1.0f );
	private	Color			selectionFrameColor	= new Color ( 0.0f, 1.0f, 0.0f, 1.0f );
	private Color			stampFrameColor		= new Color ( 1.0f, 1.0f, 0.0f, 1.0f );
	
	private	int				pointedGlyphX;
	private	int				pointedGlyphY;
	private	int				previousGlyphX;
	private	int				previousGlyphY;
	
	private	bool			firstClick;
	private	int				firstClickX;
	private	int				firstClickY;
	
	private	int				frameX;
	private	int				frameY;
	private	int				frameWidth;
	private	int				frameHeight;
	
	private	GUIStyle		headerStyle;
	
	private	string[]		toolNames	= new string[5] { "Pen", "Stroke Line", "Stroke Rectangle", "Fill Rectangle", "Stamp" };
	
	private	Texture2D		pickerGlyphFrame;
	
	private	Texture2D		sceneGUITexture;
	private	GUIStyle		sceneGUIStyle;
#endregion
	
#region Initialization
	void OnEnable () {
		
		console				= (Console)target;
		consoleTransform 	= console.transform;
		
		width				= console.width;
		height				= console.height;
		
		selectedFontIndex	= console.GetCurrentFontIndex ();
		
		headerStyle				= new GUIStyle ();
		headerStyle.fontStyle	= FontStyle.Bold;
		headerStyle.fontSize	= 13;
		
		pickerGlyphFrame	= new Texture2D ( 1, 1 );
		pickerGlyphFrame.SetPixels ( new Color[1] { new Color ( 1.0f, 1.0f, 0.0f, 0.5f ) } );

		firstClick			= true;
		
		sceneGUITexture					= new Texture2D ( 1, 1 );
		sceneGUITexture.SetPixel ( 0, 0, new Color ( 0.1f, 0.1f, 0.45f, 0.6f ) );
		sceneGUITexture.Apply ();
		sceneGUIStyle					= new GUIStyle ();
		sceneGUIStyle.normal.background	= sceneGUITexture;

	}
#endregion
	
#region OnInspectorGUI
	public override void OnInspectorGUI () {

		int	i, glyphX, glyphY;
		
		Rect 				pickerRect;
		GUILayoutOption[]	pickerSize;
		Color				swapColor;
		
		string	path;
		
		Event	e;
		
		GUILayoutOption[]	addRemoveButtonSize;
		
		if ( ( console != null ) && ( AssetDatabase.Contains ( console.gameObject ) == false ) ) {
		
			EditorGUILayout.BeginVertical ();
			
			EditorGUI.BeginChangeCheck ();
			
			if ( serializedObject != null ) {
																				
				EditorGUILayout.LabelField ( "Fonts", headerStyle );
				
				serializedObject.Update ();
				
				EditorGUI.BeginChangeCheck ();
				
				EditorGUILayout.PropertyField ( serializedObject.FindProperty ( "fonts" ), true );
				EditorGUILayout.PropertyField ( serializedObject.FindProperty ( "defaultFont" ) );
				
				serializedObject.ApplyModifiedProperties ();
				
				if ( selectedFontIndex != Console.NO_FONT_LIST ) {
				
					if ( selectedFontIndex > console.fontNames.Length )
						selectedFontIndex = 0;
					
					selectedFontIndex = EditorGUILayout.Popup ( "Fonts", selectedFontIndex, console.fontNames );
					
				}
				
				if ( ( EditorGUI.EndChangeCheck () == true ) && ( console.setupComplete == true ) )
					console.SetupFontList ();
				
				EditorGUILayout.Space ();
				
			}
			
			if ( console.setupComplete == true ) {
				if ( GUILayout.Button ( "Update Font" ) ) {
					console.Rebuild ( width, height, selectedFontIndex );
					EditorUtility.SetDirty ( console );
				}
			}
			
			EditorGUILayout.Space ();
			
			EditorGUILayout.LabelField ( "Size", headerStyle );
			
			width	= EditorGUILayout.IntField ( "Width", width );
			height	= EditorGUILayout.IntField ( "Height", height );
			
			EditorGUILayout.Space ();
			
			if ( GUILayout.Button ( "Update Console Geometry" ) ) {
				console.Rebuild ( width, height, selectedFontIndex );
				EditorUtility.SetDirty ( console );
			}

			EditorGUILayout.Space ();
			
			if ( console.setupComplete == true ) {
			
				EditorGUILayout.Space ();
				
				EditorGUILayout.LabelField ( "Drawing Tools", headerStyle );
				
				console.currentTool	= EditorGUILayout.Popup ( "Tools", console.currentTool, toolNames );
				
				if ( console.currentTool == STAMP_TOOL_INDEX ) {
					
					EditorGUILayout.BeginHorizontal ();
					
					if ( GUILayout.Button ( "Rotate 90d Left" ) )
						if ( console.hasStamp == true )
							console.stamp = Console.RotateSprite90DegreesLeft ( console.stamp );
					
					if ( GUILayout.Button ( "Rotate 90d Right" ) )
						if ( console.hasStamp == true )
							console.stamp = Console.RotateSprite90DegreesRight ( console.stamp );
					
					if ( GUILayout.Button ( "Flip Horizontal" ) )
						if ( console.hasStamp == true )
							console.stamp = Console.FlipSpriteHorizontal ( console.stamp );
					
					if ( GUILayout.Button ( "Flip Vertical" ) )
						if ( console.hasStamp == true )
							console.stamp = Console.FlipSpriteVertical ( console.stamp );
					
					EditorGUILayout.EndHorizontal ();
					
					EditorGUILayout.BeginHorizontal ();
					
					if ( GUILayout.Button ( "Load Stamp" ) ) {
						
						path = EditorUtility.OpenFilePanel ( "Open Stamp", Application.dataPath, "asset" );
						
						if ( path != "" ) {
							console.stamp 		= (ConsoleSprite)AssetDatabase.LoadMainAssetAtPath ( AbsoluteToAssetsRelativePath ( path ) );
							console.hasStamp	= true;
						}
	
					}
					
					if ( GUILayout.Button ( "Save Stamp" ) ) {
						
						path = EditorUtility.SaveFilePanelInProject ( "Save Stamp", "stamp", "asset", "Please enter a file name to save the stamp to" );

						if ( ( path != "" ) && ( console.hasStamp == true ) ) {
							AssetDatabase.CreateAsset ( console.stamp, path );
							AssetDatabase.SaveAssets ();
						}
	
					}
						
					EditorGUILayout.EndHorizontal ();
					
					console.useSpriteMask = EditorGUILayout.Toggle ( "Use Sprite Mask", console.useSpriteMask );
					
				}
				else
					console.hasStamp = false;
				
				console.DrawGlyph			= EditorGUILayout.Toggle ( "Draw Glyph", console.DrawGlyph );
				console.DrawGlyphColor		= EditorGUILayout.Toggle ( "Draw Glyph Color", console.DrawGlyphColor );
				console.DrawBackgroundColor	= EditorGUILayout.Toggle ( "Draw Background Color", console.DrawBackgroundColor );
			
				console.SetType				= EditorGUILayout.Toggle ( "Set Type", console.SetType );
				console.SetParam			= EditorGUILayout.Toggle ( "Set Param", console.SetParam );
				console.SetDestructability	= EditorGUILayout.Toggle ( "Set Destructability", console.SetDestructability );
				
				EditorGUILayout.Space ();
				
				if ( console.currentTool != STAMP_TOOL_INDEX ) {
				
					EditorGUILayout.BeginHorizontal ();
						
					EditorGUILayout.BeginVertical ();
					
					console.GlyphColor		= EditorGUILayout.ColorField ( "Glyph Color", console.GlyphColor );
					console.BackgroundColor	= EditorGUILayout.ColorField ( "Background Color", console.BackgroundColor );
					
					console.Type			= EditorGUILayout.IntField ( "Type", console.Type );
					
					for ( i = 0; i < Tile.DEFAULT_PARAM_SIZE; i++ )
						console.currentParam[i] = EditorGUILayout.FloatField ( "Parameter " + i, console.currentParam[i] );
					
					console.Destructible	= EditorGUILayout.Toggle ( "Destructible", console.Destructible );
					
					EditorGUILayout.EndVertical ();
					
					EditorGUILayout.BeginVertical ();
					
					if ( GUILayout.Button ( "Reset Colors") ) {
						console.GlyphColor		= new Color ( 	Console.DEFAULT_GLYPH_COLOR_RED,
																Console.DEFAULT_GLYPH_COLOR_GREEN,
																Console.DEFAULT_GLYPH_COLOR_BLUE );
						console.BackgroundColor	= new Color ( 	Console.DEFAULT_BACKGROUND_COLOR_RED,
																Console.DEFAULT_BACKGROUND_COLOR_GREEN,
																Console.DEFAULT_BACKGROUND_COLOR_BLUE );
					}
					
					if ( GUILayout.Button ( "Reverse Colors" ) ) {
						swapColor				= console.GlyphColor;
						console.GlyphColor		= console.BackgroundColor;
						console.BackgroundColor	= swapColor;
					}
					
					EditorGUILayout.EndVertical ();
					
					EditorGUILayout.EndHorizontal ();
								
				}
				
				EditorGUILayout.Space ();
				
				pickerSize	= new GUILayoutOption[2] {	GUILayout.Width ( 2 * console.currentFont.fontTexture.width * console.currentFont.glyphPickerTexWidth ),
														GUILayout.Height ( 2 * console.currentFont.fontTexture.height * console.currentFont.glyphPickerTexHeight ) };
				EditorGUILayout.LabelField ( "", pickerSize );
				
				pickerRect	= GUILayoutUtility.GetLastRect();
				GUI.DrawTextureWithTexCoords ( 	pickerRect,
												console.currentFont.fontTexture,
												new Rect ( 0.0f, 0.0f, console.currentFont.glyphPickerTexWidth, console.currentFont.glyphPickerTexHeight ),
												true );
				
				GUI.DrawTexture ( 	new Rect (	pickerRect.x + 2 * console.currentGlyphX * console.currentFont.glyphPixelWidth,
												pickerRect.y + pickerRect.height - 2 * ( console.currentGlyphY + 1 ) * console.currentFont.glyphPixelHeight,
												2 * console.currentFont.glyphPixelWidth,
												2 * console.currentFont.glyphPixelHeight ),
									pickerGlyphFrame,
									ScaleMode.StretchToFill,
									true,
									0 );
				
				EditorGUILayout.Space ();
				
				if ( GUILayout.Button ( "Clear Console" ) ) {
					Undo.RegisterCompleteObjectUndo ( console, "Clear" );
					ClearConsoleInSceneView ();
				}
				
				e	= Event.current;
				
				if (	( e.type == EventType.mouseDown ) &&
						( pickerRect.Contains ( e.mousePosition ) == true ) &&
						( e.button == LEFT_BUTTON ) &&
						( e.clickCount == 1 ) ) {
					
					glyphX			= Mathf.FloorToInt ( ( e.mousePosition.x - pickerRect.x ) / console.currentFont.glyphPixelWidth / 2 );
					glyphY			= Mathf.FloorToInt ( ( pickerRect.height - e.mousePosition.y + pickerRect.y ) / console.currentFont.glyphPixelHeight / 2 );
					
					console.Glyph	= glyphX + glyphY * console.currentFont.fontGlyphWidth;
	
					e.Use ();
					
					EditorUtility.SetDirty ( console );
					
				}
				
				EditorGUILayout.Space ();
				
				EditorGUILayout.LabelField ( "Sprites", headerStyle );
				
				console.topMaxSpriteGlyphs		= EditorGUILayout.IntField ( "Top Sprite Tiles", console.topMaxSpriteGlyphs );
				console.bottomMaxSpriteGlyphs	= EditorGUILayout.IntField ( "Bottom Sprite Tiles", console.bottomMaxSpriteGlyphs );
				
				console.hiddingPosition	= EditorGUILayout.Vector3Field ( "Sprites Hidding Position", console.hiddingPosition );
				
				EditorGUILayout.Space ();
				
				if ( console.sprites.Capacity == 0 ) {
					if ( GUILayout.Button ( "Add First Sprite Asset" ) )
						console.sprites.Insert ( 0, null );
				}
				else {
							
					for ( i = 0; i < console.sprites.Count; i++ ) {
		
						EditorGUILayout.BeginHorizontal ();
						
						console.sprites[i]	= (ConsoleSprite)EditorGUILayout.ObjectField ( "sprite " + i, console.sprites[i], typeof ( ConsoleSprite ), true );
						
						addRemoveButtonSize	= new GUILayoutOption[1] { GUILayout.Width ( 30 ) };
						
						if ( GUILayout.Button ( "+", addRemoveButtonSize ) ) {	console.sprites.Insert		( i + 1, null );	}
						if ( GUILayout.Button ( "-", addRemoveButtonSize ) ) {	console.sprites.RemoveAt	( i );				}
						
						EditorGUILayout.EndHorizontal ();
						
					}
					
					console.sprites.TrimExcess ();
					
				}
				
				if ( GUILayout.Button ( "Update Sprites Asset" ) ) {
					console.Rebuild ( width, height, selectedFontIndex );
					EditorUtility.SetDirty ( console );
				}
			
			}
			
			
			EditorGUILayout.EndVertical ();
			
			if ( EditorGUI.EndChangeCheck () == true ) {
				
				firstClick = true;
				
				EditorUtility.SetDirty ( console );
				
			}
			
		}
		
	}
#endregion	
	
#region OnSceneGUI
	public void OnSceneGUI () {
		
		int		i, j, k, stampYIndex, stampIndex;
		float	z0;
		
		Color	glyphColor, backColor;
		
		Event	e;
		
		
		if ( ( Application.isEditor == true ) && ( console.setupComplete == true ) ) {
			
			z0 = consoleTransform.position.z;
			
			e = Event.current;

			
			if ( e.type == EventType.MouseDrag ) {
				DrawGrid ();
			}
			else if ( Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed" ) {
				console.shouldRefresh = true;
				DrawGrid ();
			}
			else {
				
				if ( console.shouldRefresh == true ) {
					RefreshConsole ();
					EditorUtility.SetDirty ( console );
					console.shouldRefresh = false;
				}
			
				FindPointedGlyph ();
				
				if ( ( pointedGlyphX != previousGlyphX ) || ( pointedGlyphY != previousGlyphY ) ) {
					
					previousGlyphX = pointedGlyphX;
					previousGlyphY = pointedGlyphY;
					
					Repaint ();
			
					HandleUtility.Repaint ();
					
				}
				
				if ( ( pointedGlyphX >= 0 ) && ( pointedGlyphX < console.width ) && ( pointedGlyphY >= 0 ) && ( pointedGlyphY < console.height ) ) {
				
					Handles.BeginGUI ();
					GUILayout.BeginArea( new Rect ( 10.0f, 10.0f, 215.0f, ( 6 + Tile.DEFAULT_PARAM_SIZE ) * 18.0f ), sceneGUIStyle );
					GUILayout.BeginVertical ();
					
					GUILayout.Label ( "Position : " + pointedGlyphX + "," + pointedGlyphY );
					GUILayout.Label ( "Glyph Index : " + console.tiles[pointedGlyphX + console.width * pointedGlyphY].glyph );
					
					//glyphColor = console.foreground.GetPixel ( pointedGlyphX, pointedGlyphY );
					glyphColor = console.tiles[pointedGlyphX+console.width*pointedGlyphY].front;
					GUILayout.Label ( "Glyph Color : r="	+ string.Format ( "{0:0.00}", glyphColor.r ) +
									               " g="	+ string.Format ( "{0:0.00}", glyphColor.g ) +
									               " b="	+ string.Format ( "{0:0.00}", glyphColor.b ) );
					
					//backColor = console.background.GetPixel ( pointedGlyphX, pointedGlyphY );
					backColor = console.tiles[pointedGlyphX+console.width*pointedGlyphY].back;
					GUILayout.Label ( "Back Color  : r="	+ string.Format ( "{0:0.00}", backColor.r ) +
									               " g="	+ string.Format ( "{0:0.00}", backColor.g ) +
									               " b="	+ string.Format ( "{0:0.00}", backColor.b ) );
					
					GUILayout.Label ( "Type : " + console.tiles[pointedGlyphX + pointedGlyphY * console.width].type );
					
					for ( i = 0; i < Tile.DEFAULT_PARAM_SIZE; i++ ) 
						GUILayout.Label ( "Param " + i + " : " + console.tiles[pointedGlyphX + pointedGlyphY * console.width].param[i] );
						
					GUILayout.Label ( "Destructible : " + ( console.tiles[pointedGlyphX + pointedGlyphY * console.width].destructible ? "yes" : "no" ) );
					
					GUILayout.EndVertical ();
					GUILayout.EndArea();
					Handles.EndGUI ();
					
					switch ( console.currentTool ) {
					
					case PEN_TOOL_INDEX:
						DrawPointedGlyphFrame ( (float)pointedGlyphX, (float)pointedGlyphY, z0 );
						break;
						
					case LINE_TOOL_INDEX:
						
						if ( firstClick == true )
							DrawPointedGlyphFrame ( (float)pointedGlyphX, (float)pointedGlyphY, z0 );
						else {
							
							console.ScanLine ( firstClickX, firstClickY, pointedGlyphX, pointedGlyphY );
							
							for ( i = 0; i < console.lineLength; i++ )
								DrawFrame (	console.lineScan[4 * i],
											console.lineScan[4 * i + 1],
											z0,
											1.0f, 1.0f, 
											selectionFrameColor );
	
						}
						
						break;
						
					case STAMP_TOOL_INDEX:
						
						if ( console.hasStamp == true ) {
							DrawFrame (	pointedGlyphX,
										pointedGlyphY,
										z0,
										pointedGlyphX + console.stamp.width <= console.width ? console.stamp.width : console.width - pointedGlyphX,
										pointedGlyphY + console.stamp.height <= console.height ? console.stamp.height : console.height - pointedGlyphY,
										stampFrameColor );
							
						}
						else if ( firstClick == true )
							DrawPointedGlyphFrame ( (float)pointedGlyphX, (float)pointedGlyphY, z0 );
						else
							DrawSelectionFrame ();
						
						break;
						
					case STROKE_RECT_TOOL_INDEX:
					case FILL_RECT_TOOL_INDEX:
	
						if ( firstClick == true )
							DrawPointedGlyphFrame ( (float)pointedGlyphX, (float)pointedGlyphY, z0 );
						else
							DrawSelectionFrame ();
	
						break;
						
					default:
						break;
						
					}
					
					if ( ( e.type == EventType.keyDown ) && ( e.keyCode == KeyCode.Escape ) ) {
						firstClick = true;
						SceneView.RepaintAll ();
					}
					
					if ( ( e.type == EventType.mouseDown ) && ( e.button == LEFT_BUTTON) && ( e.clickCount == 1 ) ) {
	
						switch ( console.currentTool ) {
							
						case PEN_TOOL_INDEX:
							
							if ( e.alt == true ) {
								
								if ( console.DrawGlyph == true )
									console.Glyph			= console.tiles[pointedGlyphX + pointedGlyphY * console.width].glyph;
								
								if ( console.DrawGlyphColor == true )
									console.GlyphColor		= console.foreground.GetPixel ( pointedGlyphX, pointedGlyphY );
								
								if ( console.DrawBackgroundColor == true )
									console.BackgroundColor	= console.background.GetPixel ( pointedGlyphX, pointedGlyphY );
									
								if ( console.SetType == true )
									console.Type			= console.tiles[pointedGlyphX + pointedGlyphY * console.width].type;
								
								if ( console.SetParam == true )
									Console.CopyParams (	console.tiles[pointedGlyphX + pointedGlyphY * console.width].param,
															console.currentParam	);
								
								if ( console.SetDestructability == true )
									console.Destructible	= console.tiles[pointedGlyphX + pointedGlyphY * console.width].destructible;
								
							}
							else {
								
								RegisterConsoleUndo ( "Pen" );
								DrawGlyphInSceneView ( pointedGlyphX, pointedGlyphY );
								EditorUtility.SetDirty ( console );
								
								firstClick = false;
								
							}
							
							break;
							
						case LINE_TOOL_INDEX:
							
							if ( firstClick == false ) {
								RegisterConsoleUndo ( "Stroke Line" );
								StrokeLineInSceneView ();
								EditorUtility.SetDirty ( console );
							}
							
							break;
							
						case STROKE_RECT_TOOL_INDEX:
							
							if ( firstClick == false ) {
								RegisterConsoleUndo ( "Stroke Rectangle" );
								StrokeRectangleInSceneView ();
								EditorUtility.SetDirty ( console );
							}
							
							break;
							
						case FILL_RECT_TOOL_INDEX:
							
							if ( firstClick == false ) {
								RegisterConsoleUndo ( "Fill Rectangle" );
								FillRectangleInSceneView ();
								EditorUtility.SetDirty ( console );
							}
							
							break;
							
						case STAMP_TOOL_INDEX:
							
							if ( ( firstClick == true ) && ( e.alt == true) )
								console.hasStamp = false;
							
							if ( firstClick == false ) {
							
								console.hasStamp = true;
								
								console.stampX		= firstClickX < pointedGlyphX ? firstClickX : pointedGlyphX;
								console.stampY		= firstClickY < pointedGlyphY ? firstClickY : pointedGlyphY;
	
								console.stamp					= ConsoleSprite.NewSprite ( 	Mathf.Abs ( pointedGlyphX - firstClickX ) + 1,
																								Mathf.Abs ( pointedGlyphY - firstClickY ) + 1 );
								
								for ( i = 0; i < console.stamp.height; i++ ) {
									
									stampYIndex = i * console.stamp.width;
									
									for ( j = 0; j < console.stamp.width; j++ ) {
					
										stampIndex = j + stampYIndex;

										console.stamp.glyph[stampIndex]			= console.tiles[ ( console.stampX + j ) + ( console.stampY + i ) * console.width ].glyph;
										console.stamp.front[stampIndex]			= console.foreground.GetPixel ( console.stampX + j, console.stampY + i );
										console.stamp.back[stampIndex]			= console.background.GetPixel ( console.stampX + j, console.stampY + i );
										
										if (	( console.stamp.front[stampIndex].Equals ( Color.black ) == true ) &&
										    ( console.stamp.back[stampIndex].Equals ( Color.black ) == true )		)
											console.stamp.mask[stampIndex] = false;
										else
											console.stamp.mask[stampIndex] = true;
										
										console.stamp.type[stampIndex]			= console.tiles[ ( console.stampX + j ) + ( console.stampY + i ) * console.width ].type;
										
										for ( k = 0; k < Tile.DEFAULT_PARAM_SIZE; k++ )
											console.stamp.param[stampIndex * Tile.DEFAULT_PARAM_SIZE + k] = console.tiles [ ( console.stampX + j ) + ( console.stampY + i ) * console.width ].param[k];
										
										console.stamp.destructible[stampIndex]	= console.tiles[ ( console.stampX + j ) + ( console.stampY + i ) * console.width ].destructible;
										
									}
								}
								
								EditorUtility.SetDirty ( console );
								
							}
							
							if ( ( firstClick == true ) && ( e.alt == false ) ) {
							
								if ( console.hasStamp == true ) {
									RegisterConsoleUndo ( "Stamp" );
									DrawConsoleSpriteInScene ();
									EditorUtility.SetDirty ( console );	
								}
								
								firstClick = false;
								
							}
							
							break;
							
						default:
							break;
							
						}
						
						if ( firstClick == true ) {
							firstClick	= false;
							firstClickX	= pointedGlyphX;
							firstClickY	= pointedGlyphY;
						}
						else
							firstClick	= true;
						
					}
					
				}
				
			}	

		}
	
	}
#endregion
	
#region Drawing Tools
	void ClearConsoleInSceneView () {
		
		console.PrepareUpdate ();
		
		console._Clear_ALL ();
		
		console.EndUpdate ();
		
	}
	
	void DrawGlyphInSceneView ( int x, int y ) {
		
		console.PrepareUpdate ();
		
		console._SetTile_ALL ( x, y );
		
		console.EndUpdate ();
		
	}
	
	void StrokeLineInSceneView () {
		
		console.PrepareUpdate ();
		
		console._StrokeScannedLine_ALL ( false );
		
		console.EndUpdate ();
		
	}
	
	void StrokeRectangleInSceneView () {
		
		console.PrepareUpdate ();
		
		console._StrokeHorizontalLine_ALL ( frameX, frameX + frameWidth - 1,                   frameY );
		console._StrokeHorizontalLine_ALL ( frameX, frameX + frameWidth - 1, frameY + frameHeight - 1 );
		
		console._StrokeVerticalLine_ALL (                  frameX, frameY, frameY + frameHeight - 1 );
		console._StrokeVerticalLine_ALL ( frameX + frameWidth - 1, frameY, frameY + frameHeight - 1 );

		console.EndUpdate ();
		
	}
	
	void FillRectangleInSceneView () {
		
		console.PrepareUpdate ();
		
		console._FillRectangle_ALL ( frameX, frameY, frameWidth, frameHeight );
		
		console.EndUpdate ();
		
	}
	
	void DrawConsoleSpriteInScene () {

		if ( console.hasStamp == true ) {
			console.PrepareUpdate ();
			
			console._DrawConsoleSprite_ALL ( console.stamp, pointedGlyphX, pointedGlyphY );
			
			console.EndUpdate ();
		}
		
	}
#endregion
	
#region Grid
	void DrawGrid () {
		
		int 	i;
		float	z0;
		
		z0 = consoleTransform.position.z;
		
		Handles.color = gridColor;

		for ( i=0; i <= console.width; i++ )
			Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 (  i * console.currentFont.glyphWidth,           0.0f, z0 ) ),
								consoleTransform.TransformPoint( new Vector3 ( 	i * console.currentFont.glyphWidth, console.height, z0 ) ) );
		
		for ( i=0; i <= console.height; i++ )
			Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 (           0.0f, i * console.currentFont.glyphHeight, z0 ) ),
								consoleTransform.TransformPoint( new Vector3 ( 	console.width, i * console.currentFont.glyphHeight, z0 ) ) );
		
	}
	
	void FindPointedGlyph () {
		
		Plane 	plane;
		Ray		ray;
		Vector3	hit, position;
		float	distance;

		plane	= new Plane ( consoleTransform.TransformDirection(Vector3.forward), consoleTransform.position);
		ray		= HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			
		hit		= new Vector3();
			
		if ( plane.Raycast( ray, out distance ) )
			hit = ray.origin + ray.direction.normalized * distance;
			
		position		= consoleTransform.InverseTransformPoint(hit);
		
		pointedGlyphX	= Mathf.FloorToInt( position.x / console.currentFont.glyphWidth );
		pointedGlyphY	= Mathf.FloorToInt( position.y / console.currentFont.glyphHeight );
		
	}
	
	void DrawFrame ( float x, float y, float z, float width, float height, Color color ) {
		
		Handles.color = color;
	
		Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 (             x * console.currentFont.glyphWidth,              y * console.currentFont.glyphHeight, z ) ),
							consoleTransform.TransformPoint( new Vector3 ( ( x + width ) * console.currentFont.glyphWidth,              y * console.currentFont.glyphHeight, z ) ) );
		
		Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 ( ( x + width ) * console.currentFont.glyphWidth,              y * console.currentFont.glyphHeight, z ) ),
							consoleTransform.TransformPoint( new Vector3 ( ( x + width ) * console.currentFont.glyphWidth, ( y + height ) * console.currentFont.glyphHeight, z ) ) );
		
		Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 ( ( x + width ) * console.currentFont.glyphWidth, ( y + height ) * console.currentFont.glyphHeight, z ) ),
							consoleTransform.TransformPoint( new Vector3 (             x * console.currentFont.glyphWidth, ( y + height ) * console.currentFont.glyphHeight, z ) ) );
		
		Handles.DrawLine(	consoleTransform.TransformPoint( new Vector3 (             x * console.currentFont.glyphWidth, ( y + height ) * console.currentFont.glyphHeight, z ) ),
							consoleTransform.TransformPoint( new Vector3 (             x * console.currentFont.glyphWidth,              y * console.currentFont.glyphHeight, z ) ) );
		
	}
	
	void DrawPointedGlyphFrame ( float x, float y, float z ) {
		DrawFrame (x, y, z, 1.0f, 1.0f, glyphFrameColor );
	}
	
	void DrawSelectionFrame () {
				
		frameX		= firstClickX < pointedGlyphX ? firstClickX : pointedGlyphX;
		frameY		= firstClickY < pointedGlyphY ? firstClickY : pointedGlyphY;

		frameWidth	= Mathf.Abs ( pointedGlyphX - firstClickX ) + 1;
		frameHeight	= Mathf.Abs ( pointedGlyphY - firstClickY ) + 1;
						
		DrawFrame (	frameX,
					frameY,
					consoleTransform.position.z,
					frameWidth,
					frameHeight,
					selectionFrameColor );

	}
#endregion

#region Undo Helpers
	void RegisterConsoleUndo ( string undoTitle ) {
		foreach ( Tile tile in console.tiles )
			Undo.RecordObject ( tile, undoTitle );
	}
	
	void RefreshConsole () {
	
		int		previousGlyph				= console.currentGlyph;
		Color	previousGlyphColor			= console.GlyphColor;
		Color	previousBackgroundColour	= console.BackgroundColor;

		console.PrepareUpdate ();
		
		for ( int y = 0; y < console.height; y++ ) {
			for ( int x = 0; x < console.width; x++ ) {
				console.Glyph 			= console.tiles[x + y * console.width].glyph;
				console.GlyphColor		= console.tiles[x + y * console.width].front;
				console.BackgroundColor	= console.tiles[x + y * console.width].back;
				console._SetTile_GFXONLY ( x, y );
			}
		}
	
		console.EndUpdate ();
										
		console.Glyph			= previousGlyph;
		console.GlyphColor		= previousGlyphColor;
		console.BackgroundColor	= previousBackgroundColour;
	
	}
#endregion
			
#region Utilities
	string AbsoluteToAssetsRelativePath ( string path ) {
		
		int 		i;
		string		relativePath;
		string[]	projectPathArray, absolutePathArray;

		if ( Application.platform == RuntimePlatform.WindowsEditor ) {
			path 				= path.Replace ( "/", "\\" );
			projectPathArray	= Application.dataPath.Replace ( "/", "\\" ).Split ( Path.DirectorySeparatorChar );
		}
		else
			projectPathArray 	= Application.dataPath.Split ( Path.DirectorySeparatorChar );

		absolutePathArray	= path.Split ( Path.DirectorySeparatorChar );
		
		relativePath = absolutePathArray[projectPathArray.Length - 1];
		
		for ( i = projectPathArray.Length; i < absolutePathArray.Length; i++ )
			relativePath += Path.DirectorySeparatorChar + absolutePathArray[i];

		return relativePath;
		
	}
#endregion

}
