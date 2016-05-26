using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor (typeof(ConsoleAnimation))]

public class ConsoleAnimationEditor : Editor {
	
#region Instance Variables
	public	ConsoleAnimation	animation;
	private	GUIStyle			headerStyle;
	private	GUILayoutOption[]	addRemoveButtonSize;
#endregion
	
#region Initialization
	void OnEnable () {
		
		animation		= (ConsoleAnimation)target;
		
		headerStyle				= new GUIStyle ();
		headerStyle.fontStyle	= FontStyle.Bold;
		headerStyle.fontSize	= 13;
		
		addRemoveButtonSize		= new GUILayoutOption[1] { GUILayout.Width ( 30 ) };
		
	}
#endregion
	
#region  OnInspectorGUI
	public override void OnInspectorGUI () {
		
		int		i;
		IntRect rect;
		
		EditorGUI.BeginChangeCheck ();
		
		animation.spriteIndex		= EditorGUILayout.IntField ( "Sprite", animation.spriteIndex );
		
		animation.loop				= EditorGUILayout.Toggle ( "Loop", animation.loop );
		
		EditorGUILayout.Space ();
		
		if ( animation.frames.Count == 0 ) {
			if ( GUILayout.Button ( "Add First Frame" ) ) { animation.frames.Add ( new IntRect () ); }//IntRect.NewIntRect () ); }
		}
		else {
			for ( i = 0; i < animation.frames.Count; i++ ) {
			
				EditorGUILayout.BeginVertical ();
				
				EditorGUILayout.BeginHorizontal ();
				
				EditorGUILayout.LabelField ( "Frame " + i  + ":");
				
				if ( GUILayout.Button ( "+", addRemoveButtonSize ) ) {	animation.frames.Insert ( i + 1, new IntRect () ); }//IntRect.NewIntRect () ); }
				if ( GUILayout.Button ( "-", addRemoveButtonSize ) ) {	animation.frames.RemoveAt ( i ); }
				
				EditorGUILayout.EndHorizontal ();
				
				rect = animation.frames[i];
				
				rect.x		= EditorGUILayout.IntField ( "x", rect.x );
				rect.y		= EditorGUILayout.IntField ( "y", rect.y );
				rect.width	= EditorGUILayout.IntField ( "width", rect.width );
				rect.height	= EditorGUILayout.IntField ( "height", rect.height );
				
				//animation.frames[i] = (IntRect)rect;
				
				EditorGUILayout.EndVertical ();
				
				EditorGUILayout.Space ();
				
			}
		}
		
		if ( EditorGUI.EndChangeCheck () == true ) {
			
			animation.frames.TrimExcess ();
			
			EditorUtility.SetDirty ( animation );
			
			//foreach ( IntRect intRect in animation.frames )
			//	EditorUtility.SetDirty ( intRect );
			/*for ( i = 0; i < animation.frames.Count; i++ )
				EditorUtility.SetDirty ( animation.frames[i] );*/
			
		}
		
	}
#endregion
	
}
