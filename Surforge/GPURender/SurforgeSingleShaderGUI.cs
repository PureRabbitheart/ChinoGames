#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class SurforgeSingleShaderGUI : ShaderGUI {
	
	private static class Styles {
		public static GUIContent _TextureGuiContent = new GUIContent("Albedo", "Albedo (RGB). Could be source of Specular and Glossiness detail, if no Specular map selected");
		public static GUIContent _BumpMapGuiContent = new GUIContent("NormalMap", "NormalMap (RGB). Import Type should be \"Normal map\" in Unity Editor.");
		public static GUIContent _OcclusionMapGuiContent = new GUIContent("Occlusion", "OcclusionMap (R).");
		public static GUIContent _SpecularMapGuiContent = new GUIContent("Specular", "SpecularMap (RGB), GlossinessMap(A).\nIf no Specular map selected, Albedo could be used as source of Specular and Glossiness detail.");
		public static GUIContent _EmissionMapGuiContent = new GUIContent("EmissionMap", "EmissionMap (RGB)");

		public static GUIContent _DirtTexture1GuiContent = new GUIContent("Dirt 1 Texture", "Dirt 1 Texture (RGB). The amount of dirt is global for all materials in set");
		public static GUIContent _DirtTexture2GuiContent = new GUIContent("Dirt 2 Texture", "Dirt 2 Texture (RGB). The amount of dirt is global for all materials in set");
		
		public static GUIContent _Paint1MaskTexGuiContent = new GUIContent("Paint 1 Mask", "Paint 1 Mask (Grayscale). Each material has its own Paint Intensity value");
		public static GUIContent _Paint2MaskTexGuiContent = new GUIContent("Paint 2 Mask", "Paint 2 Mask (Grayscale). Each material has its own Paint Intensity value");
		
		public static GUIContent noiseEdges = new GUIContent("", "Worn Edges Noise \n\nMix XYZW to get a unique noise. \n(0, 0, 0, 1) Solid color with no noise \n(0, 0, 0, 0) Full transparent (the edges will not be seen)");
		public static GUIContent paintNoise = new GUIContent("", "Noise subtracted from the paint\n\nMix XYZW to get a unique noise. \n(0, 0, 0, 0) Solid paint without noise");
		
		public static GUIContent dirtNoise = new GUIContent("", "Dirt Noise \n\nMix XYZW to get a unique noise. \n(0, 0, 0, 0) No dirt \n(0, 0, 0, 1) Solid dirt texture");
		
	}

	bool specularMap0set = false;

	
	MaterialProperty _Texture0 = null;
	MaterialProperty _BumpMap0 = null;
	MaterialProperty _OcclusionMap0 = null;
	MaterialProperty _SpecularMap0 = null;
	MaterialProperty _UseSpecularMap0 = null;
	MaterialProperty _GlossinessFromAlpha0 = null;

	MaterialProperty _EmissionMap0 = null;
	MaterialProperty _EmissionMapTint0 = null;
	MaterialProperty _EmissionMapIntensity0 = null;
	
	MaterialProperty _Tint0 = null;
	MaterialProperty _SpecularTint0 = null;
	
	MaterialProperty _SpecularIntensity0 = null;
	MaterialProperty _0SpecularContrast = null;
	MaterialProperty _0SpecularBrightness = null;
	
	MaterialProperty _Glossiness0 = null;
	MaterialProperty _GlossinessIntensity0 = null;
	MaterialProperty _0GlossinessContrast = null;
	MaterialProperty _0GlossinessBrightness = null;
	
	MaterialProperty _0Paint1Intensity = null;
	MaterialProperty _0Paint2Intensity = null;
	
	MaterialProperty _0WornEdgesNoiseMix = null;
	MaterialProperty _0WornEdgesAmount = null;
	MaterialProperty _0WornEdgesOpacity = null;
	MaterialProperty _0WornEdgesContrast = null;
	MaterialProperty _0WornEdgesBorder = null;
	MaterialProperty _0WornEdgesBorderTint = null;
	
	MaterialProperty _0UnderlyingDiffuseTint = null;
	MaterialProperty _0UnderlyingSpecularTint = null;
	MaterialProperty _0UnderlyingDiffuse = null;
	MaterialProperty _0UnderlyingSpecular = null;
	MaterialProperty _0UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength0 = null;
	MaterialProperty _BumpMapStrength0 = null;
	MaterialProperty _OcclusionMapStrength0 = null;
	
	MaterialProperty _0Paint1MaskTex = null;
	MaterialProperty _0Paint1Color = null;
	MaterialProperty _0Paint1NoiseMix = null;
	MaterialProperty _0Paint1Specular = null;
	MaterialProperty _0Paint1Glossiness = null;
	
	MaterialProperty _0Paint2MaskTex = null;
	MaterialProperty _0Paint2Color = null;
	MaterialProperty _0Paint2NoiseMix = null;
	MaterialProperty _0Paint2Specular = null;
	MaterialProperty _0Paint2Glossiness = null;

	MaterialProperty _0GlobalTransparency = null;
	MaterialProperty _0AlbedoTransparency = null;
	MaterialProperty _0Paint1Transparency = null;
	MaterialProperty _0Paint2Transparency = null;

	MaterialProperty _0MaterialRotation = null;
	
	
	public void FindProperties (MaterialProperty[] props) {
			
		_Texture0 = FindProperty ("_Texture0", props);
		_BumpMap0 = FindProperty ("_BumpMap0", props);
		_OcclusionMap0 = FindProperty ("_OcclusionMap0", props);
		_SpecularMap0 = FindProperty ("_SpecularMap0", props);
		_UseSpecularMap0 = FindProperty ("_UseSpecularMap0", props);
		_GlossinessFromAlpha0 = FindProperty ("_GlossinessFromAlpha0", props);

		_EmissionMap0 = FindProperty ("_EmissionMap0", props);
		_EmissionMapTint0 = FindProperty ("_EmissionMapTint0", props);
		_EmissionMapIntensity0 = FindProperty ("_EmissionMapIntensity0", props);
			
		_Tint0 = FindProperty ("_Tint0", props);
		_SpecularTint0 = FindProperty ("_SpecularTint0", props);
			
		_SpecularIntensity0 = FindProperty ("_SpecularIntensity0", props);
		_0SpecularContrast = FindProperty ("_0SpecularContrast", props);
		_0SpecularBrightness = FindProperty ("_0SpecularBrightness", props);
			
		_Glossiness0 = FindProperty ("_Glossiness0", props);
		_GlossinessIntensity0 = FindProperty ("_GlossinessIntensity0", props);
		_0GlossinessContrast = FindProperty ("_0GlossinessContrast", props);
		_0GlossinessBrightness = FindProperty ("_0GlossinessBrightness", props);
			
		_0Paint1Intensity = FindProperty ("_0Paint1Intensity", props);
		_0Paint2Intensity = FindProperty ("_0Paint2Intensity", props);
			
		_0WornEdgesNoiseMix = FindProperty ("_0WornEdgesNoiseMix", props);
		_0WornEdgesAmount = FindProperty ("_0WornEdgesAmount", props);
		_0WornEdgesOpacity = FindProperty ("_0WornEdgesOpacity", props);
		_0WornEdgesContrast = FindProperty ("_0WornEdgesContrast", props);
		_0WornEdgesBorder = FindProperty ("_0WornEdgesBorder", props);
		_0WornEdgesBorderTint = FindProperty ("_0WornEdgesBorderTint", props);
			
		_0UnderlyingDiffuseTint = FindProperty ("_0UnderlyingDiffuseTint", props);
		_0UnderlyingSpecularTint = FindProperty ("_0UnderlyingSpecularTint", props);
		_0UnderlyingDiffuse = FindProperty ("_0UnderlyingDiffuse", props);
		_0UnderlyingSpecular = FindProperty ("_0UnderlyingSpecular", props);
		_0UnderlyingGlossiness = FindProperty ("_0UnderlyingGlossiness", props);
			
		//_NormalsStrength0 = FindProperty ("_NormalsStrength0", props);
		_BumpMapStrength0 = FindProperty ("_BumpMapStrength0", props);
		_OcclusionMapStrength0 = FindProperty ("_OcclusionMapStrength0", props);
			
		_0Paint1MaskTex = FindProperty ("_0Paint1MaskTex", props);
		_0Paint1Color = FindProperty ("_0Paint1Color", props);
		_0Paint1NoiseMix = FindProperty ("_0Paint1NoiseMix", props);
		_0Paint1Specular = FindProperty ("_0Paint1Specular", props);
		_0Paint1Glossiness = FindProperty ("_0Paint1Glossiness", props);
			
		_0Paint2MaskTex = FindProperty ("_0Paint2MaskTex", props);
		_0Paint2Color = FindProperty ("_0Paint2Color", props);
		_0Paint2NoiseMix = FindProperty ("_0Paint2NoiseMix", props);
		_0Paint2Specular = FindProperty ("_0Paint2Specular", props);
		_0Paint2Glossiness = FindProperty ("_0Paint2Glossiness", props);

			
		_0GlobalTransparency = FindProperty ("_0GlobalTransparency", props);
		_0AlbedoTransparency = FindProperty ("_0AlbedoTransparency", props);
		_0Paint1Transparency = FindProperty ("_0Paint1Transparency", props);
		_0Paint2Transparency = FindProperty ("_0Paint2Transparency", props);

		_0MaterialRotation = FindProperty ("_0MaterialRotation", props);
	}


	GUIStyle boxStyle;
	Texture2D boxBackground;
	
	GUIStyle labelStyle;
	
	Rect colorRect;
	float colorRectWidth;
	float colorRectHeigth;
	
	Rect controlRect;
	
	float showID;
	
	
	public void CreateStyles() {
		
		if (boxStyle == null) {
			boxBackground = new Texture2D(1,1);
			boxBackground.SetPixel(0,0, new Color(1, 1, 1, 0.05f));
			boxBackground.Apply();
			boxStyle = new GUIStyle();
			boxStyle.normal.background = boxBackground;
		}
		
		if (labelStyle == null) {
			labelStyle = new GUIStyle(EditorStyles.boldLabel);
			labelStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f, 1);
		}
		
	}
	
	
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props) {
		
		colorRectWidth = 55.0f;
		colorRectHeigth = 16.0f;
		
		CreateStyles();

		FindProperties (props); 
		
		if (Event.current.type == EventType.Layout) {   

			if (_SpecularMap0.textureValue == null) {
				specularMap0set = false;
				_UseSpecularMap0.floatValue = 0;
			}
			else {
				specularMap0set = true;
				_UseSpecularMap0.floatValue = 1.0f;
			}

		}

			
		GUILayout.BeginVertical();
		GUILayout.Label ("Material", labelStyle);
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture0); 
			
		EditorGUIUtility.labelWidth = 1f;
			
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
		materialEditor.TextureScaleOffsetProperty(colorRect, _Texture0);
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Albedo Tint");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _Tint0, "");
		GUILayout.EndHorizontal();
			
		EditorGUILayout.Separator();
		GUILayout.EndVertical();


		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap0); 
		
		EditorGUIUtility.labelWidth = 1f;
		
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
		materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap0);
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		
		GUILayout.BeginVertical();
		materialEditor.RangeProperty (_BumpMapStrength0, "Normals Strength");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		EditorGUILayout.Separator();


		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap0); 
		EditorGUIUtility.labelWidth = 1f;
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical(boxStyle);
		materialEditor.RangeProperty (_OcclusionMapStrength0, "Occlusion Strength");
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		GUILayout.EndVertical();


		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap0); 
		EditorGUIUtility.labelWidth = 1f;

		if (!specularMap0set) {
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.Toggle(false, "Alpha is Glossiness");
			EditorGUI.EndDisabledGroup ();
		}
		else {
			bool glossAlphaToggle = false;
			if (_GlossinessFromAlpha0.floatValue > 0.5f) glossAlphaToggle = true;
			else glossAlphaToggle = false;
			
			EditorGUIUtility.labelWidth = 0;
			glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
			EditorGUIUtility.labelWidth = 1f;
			
			if (glossAlphaToggle) _GlossinessFromAlpha0.floatValue = 1.0f;
			else _GlossinessFromAlpha0.floatValue = 0;
			
			_UseSpecularMap0.floatValue = 1.0f;
		}

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		GUILayout.EndVertical();

			
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Specular");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _SpecularTint0, "");
		GUILayout.EndHorizontal();
		materialEditor.RangeProperty (_SpecularIntensity0, "Detail Intensity");
		materialEditor.RangeProperty (_0SpecularContrast, "Detail Contrast");
		materialEditor.RangeProperty (_0SpecularBrightness, "Detail Shift");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_Glossiness0, "Glossiness");
		materialEditor.RangeProperty (_GlossinessIntensity0, "Detail Intensity");
		materialEditor.RangeProperty (_0GlossinessContrast, "Detail Contrast");
		materialEditor.RangeProperty (_0GlossinessBrightness, "Detail Shift");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_0Paint1Intensity, "Paint 1 Intensity");
		materialEditor.RangeProperty (_0Paint2Intensity, "Paint 2 Intensity");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
		materialEditor.VectorProperty (_0WornEdgesNoiseMix, "Worn Edges Noise");
		GUILayout.EndVertical();
		EditorGUILayout.Separator();
			
		materialEditor.RangeProperty (_0WornEdgesAmount, "Worn Edges Amount");
		materialEditor.RangeProperty (_0WornEdgesOpacity, "Worn Edges Opacity");
		materialEditor.RangeProperty (_0WornEdgesContrast, "Worn Edges Contrast");
		materialEditor.RangeProperty (_0WornEdgesBorder, "Worn Edges Border");
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Worn Edges Border");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _0WornEdgesBorderTint, "");
		GUILayout.EndHorizontal();
			
			
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Underlying Diffuse");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _0UnderlyingDiffuseTint, "");
		GUILayout.EndHorizontal();
			
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Underlying Specular");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _0UnderlyingSpecularTint, "");
		GUILayout.EndHorizontal();
			
		materialEditor.RangeProperty (_0UnderlyingDiffuse, "Underlying Diffuse");
		materialEditor.RangeProperty (_0UnderlyingSpecular, "Underlying Specular");
		materialEditor.RangeProperty (_0UnderlyingGlossiness, "Underlying Glossiness");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();

		/*
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_NormalsStrength0, "Normals Strength");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		*/

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
			
		GUILayout.Label ("Paint", labelStyle);
		EditorGUILayout.Separator();
			
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
			
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _0Paint1MaskTex); 
		EditorGUIUtility.labelWidth = 1f;
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
		materialEditor.TextureScaleOffsetProperty(colorRect, _0Paint1MaskTex);
			
		GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
		materialEditor.VectorProperty (_0Paint1NoiseMix, "Paint 1 Noise");
		GUILayout.EndVertical ();
		EditorGUILayout.Separator();
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Paint 1 Color");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _0Paint1Color, "");
		GUILayout.EndHorizontal();
			
			
			
		materialEditor.RangeProperty (_0Paint1Specular, "Paint 1 Specular");
		materialEditor.RangeProperty (_0Paint1Glossiness, "Paint 1 Glossiness");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
			
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _0Paint2MaskTex); 
		EditorGUIUtility.labelWidth = 1f;
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
		materialEditor.TextureScaleOffsetProperty(colorRect, _0Paint2MaskTex);
			
		GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
		materialEditor.VectorProperty (_0Paint2NoiseMix, "Paint 2 Noise");
		GUILayout.EndVertical ();
		EditorGUILayout.Separator();
			
		GUILayout.BeginHorizontal();
		GUILayout.Label ("Paint 2 Color");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _0Paint2Color, "");
		GUILayout.EndHorizontal();
			
			
		materialEditor.RangeProperty (_0Paint2Specular, "Paint 2 Specular");
		materialEditor.RangeProperty (_0Paint2Glossiness, "Paint 2 Glossiness");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
			
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
	


		GUILayout.Label ("Emission", labelStyle);
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		EditorGUIUtility.labelWidth = 0;
		materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap0); 
		EditorGUIUtility.labelWidth = 1f;
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
		materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap0);
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label ("EmissionMap Tint");
		controlRect = EditorGUILayout.GetControlRect();
		colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
		materialEditor.ColorProperty (colorRect, _EmissionMapTint0, "");
		GUILayout.EndHorizontal();
		
		materialEditor.RangeProperty (_EmissionMapIntensity0, "EmissionMap Intensity");
		EditorGUILayout.Separator();
		
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();



		GUILayout.Label ("Transparency", labelStyle);
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_0GlobalTransparency, "Global Transparency");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_0AlbedoTransparency, "Albedo Transparency");
		materialEditor.RangeProperty (_0Paint1Transparency, "Paint 1 Transparency");
		materialEditor.RangeProperty (_0Paint2Transparency, "Paint 2 Transparency");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();



		GUILayout.Label ("Rotation", labelStyle);
		EditorGUILayout.Separator();
		GUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		materialEditor.RangeProperty (_0MaterialRotation, "Material Rotation");
		EditorGUILayout.Separator();
		GUILayout.EndVertical();
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
	}
}
#endif