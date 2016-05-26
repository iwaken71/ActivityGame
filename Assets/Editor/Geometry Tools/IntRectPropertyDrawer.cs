using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

[CustomPropertyDrawer ( typeof ( IntRect ) )]
public class IntRectPropertyDrawer : PropertyDrawer {

	public override void OnGUI ( Rect pos, SerializedProperty property, GUIContent label ) {
		
		SerializedProperty	x, y, width, height;
		
		int 	indent;
		float	fieldWidth;
		Rect	xRect, yRect, widthRect, heightRect;
		
		x		= property.FindPropertyRelative ( "x" );
		y		= property.FindPropertyRelative ( "y" );
		width	= property.FindPropertyRelative ( "width" );
		height	= property.FindPropertyRelative ( "height" );

		EditorGUI.BeginChangeCheck ();
		
		EditorGUI.BeginProperty (pos, label, property);
        
		// Draw label
		pos						= EditorGUI.PrefixLabel ( pos, GUIUtility.GetControlID ( FocusType.Passive ), new GUIContent ( "Int Rect " + IndexFromPath ( property.propertyPath ) ) );
		// Don't make child fields be indented
		indent					= EditorGUI.indentLevel;
		EditorGUI.indentLevel	= 0;
        
		// Calculate rects
		fieldWidth				= pos.width / 4.0f;
		
		xRect					= new Rect ( pos.x,                  pos.y, fieldWidth, pos.height );
		yRect					= new Rect ( pos.x +     fieldWidth, pos.y, fieldWidth, pos.height );
		widthRect				= new Rect ( pos.x + 2 * fieldWidth, pos.y, fieldWidth, pos.height );
		heightRect				= new Rect ( pos.x + 3 * fieldWidth, pos.y, fieldWidth, pos.height );
        
		EditorGUI.PropertyField (      xRect,      x, GUIContent.none );
		EditorGUI.PropertyField (      yRect,      y, GUIContent.none );
		EditorGUI.PropertyField (  widthRect,  width, GUIContent.none );
		EditorGUI.PropertyField ( heightRect, height, GUIContent.none );
        
		// Set indent back to what it was
		EditorGUI.indentLevel	= indent;
        
		EditorGUI.EndProperty ();
		
	}
	
	private string IndexFromPath ( string path ) {

		string	index;
		
		index = Regex.Match ( path, @"\d+\]$" ).Value;
		
		if ( index.Length >= 1 ) index = index.Remove ( index.Length - 1 );
		
		return index;
	}	
	
}
