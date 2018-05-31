using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeAbout : EditorWindow {
	
	static GUIStyle aboutGuiStyle;

	static string version;

	void OnGUI() {
		if (aboutGuiStyle == null) {
			aboutGuiStyle = new GUIStyle(GUI.skin.label);
			aboutGuiStyle.richText = true;

			aboutGuiStyle.fontSize = 0;
			aboutGuiStyle.alignment = TextAnchor.UpperLeft;
			aboutGuiStyle.fixedWidth = 0;
			aboutGuiStyle.fixedHeight = 0;
			aboutGuiStyle.margin = new RectOffset(4,4,2,2);
			aboutGuiStyle.padding.top = 1;
			aboutGuiStyle.padding.bottom = 2;
			aboutGuiStyle.padding.left = 2;
			aboutGuiStyle.padding.right = 2;
		}


		GUILayout.BeginArea(new Rect(10, 10, 290, 450));

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		GUILayout.Label("<b><size=14>Surforge </size></b>", aboutGuiStyle); 
		GUILayout.Label("Version " + version, aboutGuiStyle); 
		GUILayout.Label("Sergey Vladimirov", aboutGuiStyle);
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		GUILayout.Label("<b>Thanks to all Surforge Users!</b>", aboutGuiStyle); 
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		GUILayout.Label("<b>Special thanks to</b>", aboutGuiStyle); 
		EditorGUILayout.Separator();
		GUILayout.Label("My family and friends for their support", aboutGuiStyle); 
		EditorGUILayout.Separator();
		GUILayout.Label("Alexander Sokolov, Alexey Karpov,\nAlexey Makarov, Alexey Rubaev,\nEvan Daley, Linda MacGill, Vladimir \nLebed for their help in development \nand testing", aboutGuiStyle); 
		EditorGUILayout.Separator();
		GUILayout.Label("Unity Technologies for the great engine", aboutGuiStyle); 
		EditorGUILayout.Separator();
		GUILayout.Label("Authors of MIConvexHull library", aboutGuiStyle); 
		EditorGUILayout.Separator();
		GUILayout.Label("Runevision for Triangulator code", aboutGuiStyle);
		EditorGUILayout.Separator();
		GUILayout.Label("Thomas Hourdel for SMAA", aboutGuiStyle);
		EditorGUILayout.Separator();
		GUILayout.Label("Noemotion.net for great skyboxes", aboutGuiStyle);
		
		GUILayout.EndArea();
	}

	public void SetVersion(string text) {
		version = text;
	}

}
