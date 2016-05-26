using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer ( typeof ( Size ) )]
public class SizePropertyDrawer : PropertyDrawer {

	public override void OnGUI ( Rect pos, SerializedProperty property, GUIContent label ) {
		
		SerializedProperty	width, height;
		SerializedProperty	halfWidth, halfHeight;
		SerializedProperty	vec2Size, vec2HalfSize;
		SerializedProperty	vec3Size, vec3HalfSize;
		
		int 	indent;
		float	fieldWidth;
		Rect	widthRect, heightRect;

		width			= property.FindPropertyRelative ( "width" );
		height			= property.FindPropertyRelative ( "height" );
		halfWidth		= property.FindPropertyRelative ( "halfWidth" );
		halfHeight		= property.FindPropertyRelative ( "halfHeight" );
		vec2Size		= property.FindPropertyRelative ( "vec2Size" );
		vec2HalfSize	= property.FindPropertyRelative ( "vec2HalfSize" );
		vec3Size		= property.FindPropertyRelative ( "vec3Size" );
		vec3HalfSize	= property.FindPropertyRelative ( "vec3HalfSize" );

		EditorGUI.BeginChangeCheck ();
		
		EditorGUI.BeginProperty (pos, label, property);
        
		// Draw label
		pos						= EditorGUI.PrefixLabel ( pos, GUIUtility.GetControlID ( FocusType.Passive ), label );
        
		// Don't make child fields be indented
		indent					= EditorGUI.indentLevel;
		EditorGUI.indentLevel	= 0;
        
		// Calculate rects
		fieldWidth	= pos.width / 4.0f;
		
		widthRect				= new Rect ( pos.x,              pos.y, fieldWidth, pos.height );
		heightRect				= new Rect ( pos.x + fieldWidth, pos.y, fieldWidth, pos.height );
        
		EditorGUI.PropertyField (  widthRect,  width, GUIContent.none );
		EditorGUI.PropertyField ( heightRect, height, GUIContent.none );
        
		// Set indent back to what it was
		EditorGUI.indentLevel	= indent;
        
		EditorGUI.EndProperty ();
		
		if ( EditorGUI.EndChangeCheck () == true ) {
			
			halfWidth.floatValue		=  width.floatValue / 2.0f;
			halfHeight.floatValue		= height.floatValue / 2.0f;
			
			vec2Size.vector2Value		= new Vector2 ( width.floatValue, height.floatValue );
			vec2HalfSize.vector2Value	= new Vector2 ( width.floatValue / 2.0f, height.floatValue / 2.0f );
			
			vec3Size.vector3Value		= new Vector3 ( width.floatValue, height.floatValue, 0.0f );
			vec3HalfSize.vector3Value	= new Vector3 ( width.floatValue / 2.0f, height.floatValue / 2.0f, 0.0f );
			
		}
		
	}

}
