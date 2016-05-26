using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer ( typeof ( Box ) )]
public class BoxPropertyDrawer : PropertyDrawer {
	
	public	const float	LINE_HEIGHT			= 16.0f;
	public	const float	LABEL_WIDTH			= 18.0f;
	public	const float	TYPE_LABEL_WIDTH	= 40.0f;
	public	const float	LABEL_Y_OFFSET		= -1.0f;
	
	public override void OnGUI ( Rect pos, SerializedProperty property, GUIContent label ) {
		
		SerializedProperty	type;
		SerializedProperty	x, y, width, height;
		SerializedProperty	halfWidth, halfHeight;
		SerializedProperty	position;
		SerializedProperty	rect;
		
		int 	indent;
		float	fieldWidth;
		Rect	typeRect, typeLableRect;
		Rect	xRect, xLabelRect, yRect, yLabelRect, widthRect, widthLabelRect, heightRect, heightLabelRect;
		
		type		= property.FindPropertyRelative ( "type" );
		x			= property.FindPropertyRelative ( "x" );
		y			= property.FindPropertyRelative ( "y" );
		position	= property.FindPropertyRelative ( "position" );
		width		= property.FindPropertyRelative ( "width" );
		height		= property.FindPropertyRelative ( "height" );
		halfWidth	= property.FindPropertyRelative ( "halfWidth" );
		halfHeight	= property.FindPropertyRelative ( "halfHeight" );
		rect		= property.FindPropertyRelative ( "rect" );

		EditorGUI.BeginChangeCheck ();
		
		EditorGUI.BeginProperty (pos, label, property);
        
		// Draw label
		pos						= EditorGUI.PrefixLabel ( pos, GUIUtility.GetControlID ( FocusType.Passive ), label );
        
		// Don't make child fields be indented
		indent					= EditorGUI.indentLevel;
		EditorGUI.indentLevel	= 0;
        
		// Calculate rects
		fieldWidth	= ( pos.width - 4 * LABEL_WIDTH ) / 4.0f;
		
		xLabelRect				= new Rect ( pos.x              , pos.y + LABEL_Y_OFFSET, LABEL_WIDTH, pos.height / 2.0f + LABEL_Y_OFFSET);
		xRect					= new Rect ( pos.x + LABEL_WIDTH, pos.y                 ,  fieldWidth, pos.height / 2.0f );
		
		yLabelRect				= new Rect ( pos.x +     LABEL_WIDTH + fieldWidth, pos.y + LABEL_Y_OFFSET , LABEL_WIDTH, pos.height / 2.0f );
		yRect					= new Rect ( pos.x + 2 * LABEL_WIDTH + fieldWidth, pos.y                  ,  fieldWidth, pos.height / 2.0f );
		
		widthLabelRect			= new Rect ( pos.x + 2 * LABEL_WIDTH + 2 * fieldWidth, pos.y + LABEL_Y_OFFSET, LABEL_WIDTH, pos.height / 2.0f );
		widthRect				= new Rect ( pos.x + 3 * LABEL_WIDTH + 2 * fieldWidth, pos.y                 ,  fieldWidth, pos.height / 2.0f );
		
		heightLabelRect			= new Rect ( pos.x + 3 * LABEL_WIDTH + 3 * fieldWidth, pos.y + LABEL_Y_OFFSET, LABEL_WIDTH, pos.height / 2.0f );
		heightRect				= new Rect ( pos.x + 4 * LABEL_WIDTH + 3 * fieldWidth, pos.y                 ,  fieldWidth, pos.height / 2.0f );
		
		typeLableRect			= new Rect ( pos.x                   , pos.y + pos.height / 2.0f,             TYPE_LABEL_WIDTH, pos.height / 2.0f );
		typeRect				= new Rect ( pos.x + TYPE_LABEL_WIDTH, pos.y + pos.height / 2.0f, pos.width - TYPE_LABEL_WIDTH, pos.height / 2.0f );
		
		EditorGUI.LabelField	( xLabelRect,                   "x :" );
		EditorGUI.PropertyField (      xRect,      x, GUIContent.none );
		
		EditorGUI.LabelField	( yLabelRect,                   "y :" );
		EditorGUI.PropertyField (      yRect,      y, GUIContent.none );
		
		EditorGUI.LabelField	( widthLabelRect,                   "w :" );
		EditorGUI.PropertyField (      widthRect,  width, GUIContent.none );
		
		EditorGUI.LabelField	( heightLabelRect,                   "h :" );
		EditorGUI.PropertyField (      heightRect, height, GUIContent.none );
		
		EditorGUI.LabelField	( typeLableRect,                "type :" );
		EditorGUI.PropertyField (      typeRect,   type, GUIContent.none );
        
		// Set indent back to what it was
		EditorGUI.indentLevel	= indent;
        
		EditorGUI.EndProperty ();
		
		if ( EditorGUI.EndChangeCheck () == true ) {
			
			halfWidth.floatValue	=  width.floatValue / 2.0f;
			halfHeight.floatValue	= height.floatValue / 2.0f;
			
			position.vector3Value	= new Vector3 ( x.floatValue, y .floatValue, 0.0f );
			
			property.FindPropertyRelative ( "size.width"  ).floatValue			=  width.floatValue;
			property.FindPropertyRelative ( "size.height" ).floatValue			= height.floatValue;
			property.FindPropertyRelative ( "size.halfWidth"  ).floatValue		=  width.floatValue / 2.0f;
			property.FindPropertyRelative ( "size.halfHeight" ).floatValue		= height.floatValue / 2.0f;
			
			property.FindPropertyRelative ( "size.vec2Size"      ).vector2Value		=  new Vector2 ( width.floatValue,        height.floatValue );
			property.FindPropertyRelative ( "size.vec2HalfSize"  ).vector2Value		=  new Vector2 ( width.floatValue / 2.0f, height.floatValue / 2.0f );
			
			property.FindPropertyRelative ( "size.vec3Size"      ).vector3Value		=  new Vector3 ( width.floatValue,        height.floatValue,        0.0f );
			property.FindPropertyRelative ( "size.vec3HalfSize"  ).vector3Value		=  new Vector3 ( width.floatValue / 2.0f, height.floatValue / 2.0f, 0.0f );
			
			switch ( type.enumValueIndex ) {
			case (int)Box.BoxType.Centered:
				rect.rectValue	= new Rect (	x.floatValue +  halfWidth.floatValue,
												y.floatValue + halfHeight.floatValue,
											                        width.floatValue,
											                       height.floatValue );
				break;
			case (int)Box.BoxType.BottomLeft:
				rect.rectValue	= new Rect (	     x.floatValue,
												     y.floatValue,
												 width.floatValue,
												height.floatValue );
				break;
			case (int)Box.BoxType.BottomRight:
				rect.rectValue	= new Rect (	x.floatValue + width.floatValue,
											    y.floatValue,
												               width.floatValue,
												              height.floatValue );
				break;
			case (int)Box.BoxType.TopRight:
				rect.rectValue	= new Rect (	x.floatValue +  width.floatValue,
												y.floatValue + height.floatValue,
												                width.floatValue,
												               height.floatValue );
				break;
			case (int)Box.BoxType.TopLeft:
				rect.rectValue	= new Rect (	x.floatValue,
												y.floatValue + height.floatValue,
												                width.floatValue,
												               height.floatValue );
				break;
			default:
				rect.rectValue	= new Rect (	x.floatValue +  halfWidth.floatValue,
												y.floatValue + halfHeight.floatValue,
											                        width.floatValue,
											                       height.floatValue );
				break;
			}
		}
		
	}

	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) { return 2 * LINE_HEIGHT; }
	
	private static Vector2 	Vector2FromVector2	( Vector2 	v ) { return new Vector2 	( v.x, v.y );						}
	private static Vector3	Vector3FromVector3	( Vector3 	v ) { return new Vector3 	( v.x, v.y, v.z );					}
	private static Rect		RectFromRect		( Rect		r ) { return new Rect		( r.x, r.y, r.width, r.height );	}
	
}
