#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class SurforgeComposerShaderGUI : ShaderGUI {

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

		public static GUIContent specularAdjust = new GUIContent("", "Global Specular adjust\n\nHandy for final tweaking of the whole material set.\nAlso, lowering the Max value can be useful to reduce specular flickering");
		public static GUIContent glossinessAdjust = new GUIContent("", "Global Glossiness adjust\n\nHandy for final tweaking of the whole material set.\nAlso, lowering the Max value can be useful to reduce specular flickering");


	} 


	bool specularMap0set = false;
	bool specularMap1set = false;
	bool specularMap2set = false;
	bool specularMap3set = false;
	bool specularMap4set = false;
	bool specularMap5set = false;
	bool specularMap6set = false;
	bool specularMap7set = false;

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



	MaterialProperty _Texture1 = null;
	MaterialProperty _BumpMap1 = null;
	MaterialProperty _OcclusionMap1 = null;
	MaterialProperty _SpecularMap1 = null;
	MaterialProperty _UseSpecularMap1 = null;
	MaterialProperty _GlossinessFromAlpha1 = null;

	MaterialProperty _EmissionMap1 = null;
	MaterialProperty _EmissionMapTint1 = null;
	MaterialProperty _EmissionMapIntensity1 = null;
	
	MaterialProperty _Tint1 = null;
	MaterialProperty _SpecularTint1 = null;
	
	MaterialProperty _SpecularIntensity1 = null;
	MaterialProperty _1SpecularContrast = null;
	MaterialProperty _1SpecularBrightness = null;
	
	MaterialProperty _Glossiness1 = null;
	MaterialProperty _GlossinessIntensity1 = null;
	MaterialProperty _1GlossinessContrast = null;
	MaterialProperty _1GlossinessBrightness = null;
	
	MaterialProperty _1Paint1Intensity = null;
	MaterialProperty _1Paint2Intensity = null;
	
	MaterialProperty _1WornEdgesNoiseMix = null;
	MaterialProperty _1WornEdgesAmount = null;
	MaterialProperty _1WornEdgesOpacity = null;
	MaterialProperty _1WornEdgesContrast = null;
	MaterialProperty _1WornEdgesBorder = null;
	MaterialProperty _1WornEdgesBorderTint = null;
	
	MaterialProperty _1UnderlyingDiffuseTint = null;
	MaterialProperty _1UnderlyingSpecularTint = null;
	MaterialProperty _1UnderlyingDiffuse = null;
	MaterialProperty _1UnderlyingSpecular = null;
	MaterialProperty _1UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength1 = null;
	MaterialProperty _BumpMapStrength1 = null;
	MaterialProperty _OcclusionMapStrength1 = null;

	MaterialProperty _1Paint1MaskTex = null;
	MaterialProperty _1Paint1Color = null;
	MaterialProperty _1Paint1NoiseMix = null;
	MaterialProperty _1Paint1Specular = null;
	MaterialProperty _1Paint1Glossiness = null;
	
	MaterialProperty _1Paint2MaskTex = null;
	MaterialProperty _1Paint2Color = null;
	MaterialProperty _1Paint2NoiseMix = null;
	MaterialProperty _1Paint2Specular = null;
	MaterialProperty _1Paint2Glossiness = null;

	MaterialProperty _1GlobalTransparency = null;
	MaterialProperty _1AlbedoTransparency = null;
	MaterialProperty _1Paint1Transparency = null;
	MaterialProperty _1Paint2Transparency = null;

	MaterialProperty _1MaterialRotation = null;



	MaterialProperty _Texture2 = null;
	MaterialProperty _BumpMap2 = null;
	MaterialProperty _OcclusionMap2 = null;
	MaterialProperty _SpecularMap2 = null;
	MaterialProperty _UseSpecularMap2 = null;
	MaterialProperty _GlossinessFromAlpha2 = null;

	MaterialProperty _EmissionMap2 = null;
	MaterialProperty _EmissionMapTint2 = null;
	MaterialProperty _EmissionMapIntensity2 = null;
	
	MaterialProperty _Tint2 = null;
	MaterialProperty _SpecularTint2 = null;
	
	MaterialProperty _SpecularIntensity2 = null;
	MaterialProperty _2SpecularContrast = null;
	MaterialProperty _2SpecularBrightness = null;
	
	MaterialProperty _Glossiness2 = null;
	MaterialProperty _GlossinessIntensity2 = null;
	MaterialProperty _2GlossinessContrast = null;
	MaterialProperty _2GlossinessBrightness = null;
	
	MaterialProperty _2Paint1Intensity = null;
	MaterialProperty _2Paint2Intensity = null;
	
	MaterialProperty _2WornEdgesNoiseMix = null;
	MaterialProperty _2WornEdgesAmount = null;
	MaterialProperty _2WornEdgesOpacity = null;
	MaterialProperty _2WornEdgesContrast = null;
	MaterialProperty _2WornEdgesBorder = null;
	MaterialProperty _2WornEdgesBorderTint = null;
	
	MaterialProperty _2UnderlyingDiffuseTint = null;
	MaterialProperty _2UnderlyingSpecularTint = null;
	MaterialProperty _2UnderlyingDiffuse = null;
	MaterialProperty _2UnderlyingSpecular = null;
	MaterialProperty _2UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength2 = null;
	MaterialProperty _BumpMapStrength2 = null;
	MaterialProperty _OcclusionMapStrength2 = null;

	MaterialProperty _2Paint1MaskTex = null;
	MaterialProperty _2Paint1Color = null;
	MaterialProperty _2Paint1NoiseMix = null;
	MaterialProperty _2Paint1Specular = null;
	MaterialProperty _2Paint1Glossiness = null;
	
	MaterialProperty _2Paint2MaskTex = null;
	MaterialProperty _2Paint2Color = null;
	MaterialProperty _2Paint2NoiseMix = null;
	MaterialProperty _2Paint2Specular = null;
	MaterialProperty _2Paint2Glossiness = null;

	MaterialProperty _2GlobalTransparency = null;
	MaterialProperty _2AlbedoTransparency = null;
	MaterialProperty _2Paint1Transparency = null;
	MaterialProperty _2Paint2Transparency = null;

	MaterialProperty _2MaterialRotation = null;



	MaterialProperty _Texture3 = null;
	MaterialProperty _BumpMap3 = null;
	MaterialProperty _OcclusionMap3 = null;
	MaterialProperty _SpecularMap3 = null;
	MaterialProperty _UseSpecularMap3 = null;
	MaterialProperty _GlossinessFromAlpha3 = null;

	MaterialProperty _EmissionMap3 = null;
	MaterialProperty _EmissionMapTint3 = null;
	MaterialProperty _EmissionMapIntensity3 = null;
	
	MaterialProperty _Tint3 = null;
	MaterialProperty _SpecularTint3 = null;
	
	MaterialProperty _SpecularIntensity3 = null;
	MaterialProperty _3SpecularContrast = null;
	MaterialProperty _3SpecularBrightness = null;
	
	MaterialProperty _Glossiness3 = null;
	MaterialProperty _GlossinessIntensity3 = null;
	MaterialProperty _3GlossinessContrast = null;
	MaterialProperty _3GlossinessBrightness = null;
	
	MaterialProperty _3Paint1Intensity = null;
	MaterialProperty _3Paint2Intensity = null;
	
	MaterialProperty _3WornEdgesNoiseMix = null;
	MaterialProperty _3WornEdgesAmount = null;
	MaterialProperty _3WornEdgesOpacity = null;
	MaterialProperty _3WornEdgesContrast = null;
	MaterialProperty _3WornEdgesBorder = null;
	MaterialProperty _3WornEdgesBorderTint = null;
	
	MaterialProperty _3UnderlyingDiffuseTint = null;
	MaterialProperty _3UnderlyingSpecularTint = null;
	MaterialProperty _3UnderlyingDiffuse = null;
	MaterialProperty _3UnderlyingSpecular = null;
	MaterialProperty _3UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength3 = null;
	MaterialProperty _BumpMapStrength3 = null;
	MaterialProperty _OcclusionMapStrength3 = null;

	MaterialProperty _3Paint1MaskTex = null;
	MaterialProperty _3Paint1Color = null;
	MaterialProperty _3Paint1NoiseMix = null;
	MaterialProperty _3Paint1Specular = null;
	MaterialProperty _3Paint1Glossiness = null;
	
	MaterialProperty _3Paint2MaskTex = null;
	MaterialProperty _3Paint2Color = null;
	MaterialProperty _3Paint2NoiseMix = null;
	MaterialProperty _3Paint2Specular = null;
	MaterialProperty _3Paint2Glossiness = null;

	MaterialProperty _3GlobalTransparency = null;
	MaterialProperty _3AlbedoTransparency = null;
	MaterialProperty _3Paint1Transparency = null;
	MaterialProperty _3Paint2Transparency = null;

	MaterialProperty _3MaterialRotation = null;



	MaterialProperty _Texture4 = null;
	MaterialProperty _BumpMap4 = null;
	MaterialProperty _OcclusionMap4 = null;
	MaterialProperty _SpecularMap4 = null;
	MaterialProperty _UseSpecularMap4 = null;
	MaterialProperty _GlossinessFromAlpha4 = null;

	MaterialProperty _EmissionMap4 = null;
	MaterialProperty _EmissionMapTint4 = null;
	MaterialProperty _EmissionMapIntensity4 = null;
	
	MaterialProperty _Tint4 = null;
	MaterialProperty _SpecularTint4 = null;
	
	MaterialProperty _SpecularIntensity4 = null;
	MaterialProperty _4SpecularContrast = null;
	MaterialProperty _4SpecularBrightness = null;
	
	MaterialProperty _Glossiness4 = null;
	MaterialProperty _GlossinessIntensity4 = null;
	MaterialProperty _4GlossinessContrast = null;
	MaterialProperty _4GlossinessBrightness = null;
	
	MaterialProperty _4Paint1Intensity = null;
	MaterialProperty _4Paint2Intensity = null;
	
	MaterialProperty _4WornEdgesNoiseMix = null;
	MaterialProperty _4WornEdgesAmount = null;
	MaterialProperty _4WornEdgesOpacity = null;
	MaterialProperty _4WornEdgesContrast = null;
	MaterialProperty _4WornEdgesBorder = null;
	MaterialProperty _4WornEdgesBorderTint = null;
	
	MaterialProperty _4UnderlyingDiffuseTint = null;
	MaterialProperty _4UnderlyingSpecularTint = null;
	MaterialProperty _4UnderlyingDiffuse = null;
	MaterialProperty _4UnderlyingSpecular = null;
	MaterialProperty _4UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength4 = null;
	MaterialProperty _BumpMapStrength4 = null;
	MaterialProperty _OcclusionMapStrength4 = null;

	MaterialProperty _4Paint1MaskTex = null;
	MaterialProperty _4Paint1Color = null;
	MaterialProperty _4Paint1NoiseMix = null;
	MaterialProperty _4Paint1Specular = null;
	MaterialProperty _4Paint1Glossiness = null;
	
	MaterialProperty _4Paint2MaskTex = null;
	MaterialProperty _4Paint2Color = null;
	MaterialProperty _4Paint2NoiseMix = null;
	MaterialProperty _4Paint2Specular = null;
	MaterialProperty _4Paint2Glossiness = null;

	MaterialProperty _4GlobalTransparency = null;
	MaterialProperty _4AlbedoTransparency = null;
	MaterialProperty _4Paint1Transparency = null;
	MaterialProperty _4Paint2Transparency = null;

	MaterialProperty _4MaterialRotation = null;



	MaterialProperty _Texture5 = null;
	MaterialProperty _BumpMap5 = null;
	MaterialProperty _OcclusionMap5 = null;
	MaterialProperty _SpecularMap5 = null;
	MaterialProperty _UseSpecularMap5 = null;
	MaterialProperty _GlossinessFromAlpha5 = null;

	MaterialProperty _EmissionMap5 = null;
	MaterialProperty _EmissionMapTint5 = null;
	MaterialProperty _EmissionMapIntensity5 = null;
	
	MaterialProperty _Tint5 = null;
	MaterialProperty _SpecularTint5 = null;
	
	MaterialProperty _SpecularIntensity5 = null;
	MaterialProperty _5SpecularContrast = null;
	MaterialProperty _5SpecularBrightness = null;
	
	MaterialProperty _Glossiness5 = null;
	MaterialProperty _GlossinessIntensity5 = null;
	MaterialProperty _5GlossinessContrast = null;
	MaterialProperty _5GlossinessBrightness = null;
	
	MaterialProperty _5Paint1Intensity = null;
	MaterialProperty _5Paint2Intensity = null;
	
	MaterialProperty _5WornEdgesNoiseMix = null;
	MaterialProperty _5WornEdgesAmount = null;
	MaterialProperty _5WornEdgesOpacity = null;
	MaterialProperty _5WornEdgesContrast = null;
	MaterialProperty _5WornEdgesBorder = null;
	MaterialProperty _5WornEdgesBorderTint = null;
	
	MaterialProperty _5UnderlyingDiffuseTint = null;
	MaterialProperty _5UnderlyingSpecularTint = null;
	MaterialProperty _5UnderlyingDiffuse = null;
	MaterialProperty _5UnderlyingSpecular = null;
	MaterialProperty _5UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength5 = null;
	MaterialProperty _BumpMapStrength5 = null;
	MaterialProperty _OcclusionMapStrength5 = null;

	MaterialProperty _5Paint1MaskTex = null;
	MaterialProperty _5Paint1Color = null;
	MaterialProperty _5Paint1NoiseMix = null;
	MaterialProperty _5Paint1Specular = null;
	MaterialProperty _5Paint1Glossiness = null;
	
	MaterialProperty _5Paint2MaskTex = null;
	MaterialProperty _5Paint2Color = null;
	MaterialProperty _5Paint2NoiseMix = null;
	MaterialProperty _5Paint2Specular = null;
	MaterialProperty _5Paint2Glossiness = null;

	MaterialProperty _5GlobalTransparency = null;
	MaterialProperty _5AlbedoTransparency = null;
	MaterialProperty _5Paint1Transparency = null;
	MaterialProperty _5Paint2Transparency = null;

	MaterialProperty _5MaterialRotation = null;



	MaterialProperty _Texture6 = null;
	MaterialProperty _BumpMap6 = null;
	MaterialProperty _OcclusionMap6 = null;
	MaterialProperty _SpecularMap6 = null;
	MaterialProperty _UseSpecularMap6 = null;
	MaterialProperty _GlossinessFromAlpha6 = null;

	MaterialProperty _EmissionMap6 = null;
	MaterialProperty _EmissionMapTint6 = null;
	MaterialProperty _EmissionMapIntensity6 = null;
	
	MaterialProperty _Tint6 = null;
	MaterialProperty _SpecularTint6 = null;
	
	MaterialProperty _SpecularIntensity6 = null;
	MaterialProperty _6SpecularContrast = null;
	MaterialProperty _6SpecularBrightness = null;
	
	MaterialProperty _Glossiness6 = null;
	MaterialProperty _GlossinessIntensity6 = null;
	MaterialProperty _6GlossinessContrast = null;
	MaterialProperty _6GlossinessBrightness = null;
	
	MaterialProperty _6Paint1Intensity = null;
	MaterialProperty _6Paint2Intensity = null;
	
	MaterialProperty _6WornEdgesNoiseMix = null;
	MaterialProperty _6WornEdgesAmount = null;
	MaterialProperty _6WornEdgesOpacity = null;
	MaterialProperty _6WornEdgesContrast = null;
	MaterialProperty _6WornEdgesBorder = null;
	MaterialProperty _6WornEdgesBorderTint = null;
	
	MaterialProperty _6UnderlyingDiffuseTint = null;
	MaterialProperty _6UnderlyingSpecularTint = null;
	MaterialProperty _6UnderlyingDiffuse = null;
	MaterialProperty _6UnderlyingSpecular = null;
	MaterialProperty _6UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength6 = null;
	MaterialProperty _BumpMapStrength6 = null;
	MaterialProperty _OcclusionMapStrength6 = null;

	MaterialProperty _6Paint1MaskTex = null;
	MaterialProperty _6Paint1Color = null;
	MaterialProperty _6Paint1NoiseMix = null;
	MaterialProperty _6Paint1Specular = null;
	MaterialProperty _6Paint1Glossiness = null;
	
	MaterialProperty _6Paint2MaskTex = null;
	MaterialProperty _6Paint2Color = null;
	MaterialProperty _6Paint2NoiseMix = null;
	MaterialProperty _6Paint2Specular = null;
	MaterialProperty _6Paint2Glossiness = null;

	MaterialProperty _6GlobalTransparency = null;
	MaterialProperty _6AlbedoTransparency = null;
	MaterialProperty _6Paint1Transparency = null;
	MaterialProperty _6Paint2Transparency = null;

	MaterialProperty _6MaterialRotation = null;



	MaterialProperty _Texture7 = null;
	MaterialProperty _BumpMap7 = null;
	MaterialProperty _OcclusionMap7 = null;
	MaterialProperty _SpecularMap7 = null;
	MaterialProperty _UseSpecularMap7 = null;
	MaterialProperty _GlossinessFromAlpha7 = null;

	MaterialProperty _EmissionMap7 = null;
	MaterialProperty _EmissionMapTint7 = null;
	MaterialProperty _EmissionMapIntensity7 = null;
	
	MaterialProperty _Tint7 = null;
	MaterialProperty _SpecularTint7 = null;
	
	MaterialProperty _SpecularIntensity7 = null;
	MaterialProperty _7SpecularContrast = null;
	MaterialProperty _7SpecularBrightness = null;
	
	MaterialProperty _Glossiness7 = null;
	MaterialProperty _GlossinessIntensity7 = null;
	MaterialProperty _7GlossinessContrast = null;
	MaterialProperty _7GlossinessBrightness = null;
	
	MaterialProperty _7Paint1Intensity = null;
	MaterialProperty _7Paint2Intensity = null;
	
	MaterialProperty _7WornEdgesNoiseMix = null;
	MaterialProperty _7WornEdgesAmount = null;
	MaterialProperty _7WornEdgesOpacity = null;
	MaterialProperty _7WornEdgesContrast = null;
	MaterialProperty _7WornEdgesBorder = null;
	MaterialProperty _7WornEdgesBorderTint = null;
	
	MaterialProperty _7UnderlyingDiffuseTint = null;
	MaterialProperty _7UnderlyingSpecularTint = null;
	MaterialProperty _7UnderlyingDiffuse = null;
	MaterialProperty _7UnderlyingSpecular = null;
	MaterialProperty _7UnderlyingGlossiness = null;
	
	//MaterialProperty _NormalsStrength7 = null;
	MaterialProperty _BumpMapStrength7 = null;
	MaterialProperty _OcclusionMapStrength7 = null;

	MaterialProperty _7Paint1MaskTex = null;
	MaterialProperty _7Paint1Color = null;
	MaterialProperty _7Paint1NoiseMix = null;
	MaterialProperty _7Paint1Specular = null;
	MaterialProperty _7Paint1Glossiness = null;
	
	MaterialProperty _7Paint2MaskTex = null;
	MaterialProperty _7Paint2Color = null;
	MaterialProperty _7Paint2NoiseMix = null;
	MaterialProperty _7Paint2Specular = null;
	MaterialProperty _7Paint2Glossiness = null;

	MaterialProperty _7GlobalTransparency = null;
	MaterialProperty _7AlbedoTransparency = null;
	MaterialProperty _7Paint1Transparency = null;
	MaterialProperty _7Paint2Transparency = null;

	MaterialProperty _7MaterialRotation = null;



	//MaterialProperty _DirtTexture1 = null;
	MaterialProperty _Dirt1Tint = null;
	MaterialProperty _DirtNoise1Mix = null;
	MaterialProperty _Dirt1Amount = null;
	MaterialProperty _Dirt1Contrast = null;
	MaterialProperty _Dirt1Opacity = null;

	//MaterialProperty _DirtTexture2 = null;
	MaterialProperty _Dirt2Tint = null;
	MaterialProperty _DirtNoise2Mix = null;
	MaterialProperty _Dirt2Amount = null;
	MaterialProperty _Dirt2Contrast = null;
	MaterialProperty _Dirt2Opacity = null;


	MaterialProperty _0EmissionTint = null;
	MaterialProperty _0EmissionIntensity = null;
	MaterialProperty _1EmissionTint = null;
	MaterialProperty _1EmissionIntensity = null;


	MaterialProperty _specMin = null;
	MaterialProperty _specMax = null;
	
	MaterialProperty _glossMin = null;
	MaterialProperty _glossMax = null;

	MaterialProperty _gamma = null;
	MaterialProperty _minInput = null;
	MaterialProperty _maxInput = null;
	MaterialProperty _minOutput = null;
	MaterialProperty _maxOutput = null;



	MaterialProperty _Hue = null;
	MaterialProperty _Saturation = null;
	MaterialProperty _Brightness = null;
	MaterialProperty _Contrast = null;

	MaterialProperty _GlobalScale = null;



	public void FindProperties (MaterialProperty[] props) {

		if ((showID == 0) || (showID == 11)) {

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

		if ((showID == 1) || (showID == 11)) {

			_Texture1 = FindProperty ("_Texture1", props);
			_BumpMap1 = FindProperty ("_BumpMap1", props);
			_OcclusionMap1 = FindProperty ("_OcclusionMap1", props);
			_SpecularMap1 = FindProperty ("_SpecularMap1", props);
			_UseSpecularMap1 = FindProperty ("_UseSpecularMap1", props);
			_GlossinessFromAlpha1 = FindProperty ("_GlossinessFromAlpha1", props);

			_EmissionMap1 = FindProperty ("_EmissionMap1", props);
			_EmissionMapTint1 = FindProperty ("_EmissionMapTint1", props);
			_EmissionMapIntensity1 = FindProperty ("_EmissionMapIntensity1", props);
		
			_Tint1 = FindProperty ("_Tint1", props);
			_SpecularTint1 = FindProperty ("_SpecularTint1", props);
		
			_SpecularIntensity1 = FindProperty ("_SpecularIntensity1", props);
			_1SpecularContrast = FindProperty ("_1SpecularContrast", props);
			_1SpecularBrightness = FindProperty ("_1SpecularBrightness", props);
		
			_Glossiness1 = FindProperty ("_Glossiness1", props);
			_GlossinessIntensity1 = FindProperty ("_GlossinessIntensity1", props);
			_1GlossinessContrast = FindProperty ("_1GlossinessContrast", props);
			_1GlossinessBrightness = FindProperty ("_1GlossinessBrightness", props);
		
			_1Paint1Intensity = FindProperty ("_1Paint1Intensity", props);
			_1Paint2Intensity = FindProperty ("_1Paint2Intensity", props);
		
			_1WornEdgesNoiseMix = FindProperty ("_1WornEdgesNoiseMix", props);
			_1WornEdgesAmount = FindProperty ("_1WornEdgesAmount", props);
			_1WornEdgesOpacity = FindProperty ("_1WornEdgesOpacity", props);
			_1WornEdgesContrast = FindProperty ("_1WornEdgesContrast", props);
			_1WornEdgesBorder = FindProperty ("_1WornEdgesBorder", props);
			_1WornEdgesBorderTint = FindProperty ("_1WornEdgesBorderTint", props);
		
			_1UnderlyingDiffuseTint = FindProperty ("_1UnderlyingDiffuseTint", props);
			_1UnderlyingSpecularTint = FindProperty ("_1UnderlyingSpecularTint", props);
			_1UnderlyingDiffuse = FindProperty ("_1UnderlyingDiffuse", props);
			_1UnderlyingSpecular = FindProperty ("_1UnderlyingSpecular", props);
			_1UnderlyingGlossiness = FindProperty ("_1UnderlyingGlossiness", props);
		
			//_NormalsStrength1 = FindProperty ("_NormalsStrength1", props);
			_BumpMapStrength1 = FindProperty ("_BumpMapStrength1", props);
			_OcclusionMapStrength1 = FindProperty ("_OcclusionMapStrength1", props);

			_1Paint1MaskTex = FindProperty ("_1Paint1MaskTex", props);
			_1Paint1Color = FindProperty ("_1Paint1Color", props);
			_1Paint1NoiseMix = FindProperty ("_1Paint1NoiseMix", props);
			_1Paint1Specular = FindProperty ("_1Paint1Specular", props);
			_1Paint1Glossiness = FindProperty ("_1Paint1Glossiness", props);
			
			_1Paint2MaskTex = FindProperty ("_1Paint2MaskTex", props);
			_1Paint2Color = FindProperty ("_1Paint2Color", props);
			_1Paint2NoiseMix = FindProperty ("_1Paint2NoiseMix", props);
			_1Paint2Specular = FindProperty ("_1Paint2Specular", props);
			_1Paint2Glossiness = FindProperty ("_1Paint2Glossiness", props);

			_1GlobalTransparency = FindProperty ("_1GlobalTransparency", props);
			_1AlbedoTransparency = FindProperty ("_1AlbedoTransparency", props);
			_1Paint1Transparency = FindProperty ("_1Paint1Transparency", props);
			_1Paint2Transparency = FindProperty ("_1Paint2Transparency", props);

			_1MaterialRotation = FindProperty ("_1MaterialRotation", props);
		}

		if ((showID == 2) || (showID == 11)) {
			_Texture2 = FindProperty ("_Texture2", props);
			_BumpMap2 = FindProperty ("_BumpMap2", props);
			_OcclusionMap2 = FindProperty ("_OcclusionMap2", props);
			_SpecularMap2 = FindProperty ("_SpecularMap2", props);
			_UseSpecularMap2 = FindProperty ("_UseSpecularMap2", props);
			_GlossinessFromAlpha2 = FindProperty ("_GlossinessFromAlpha2", props);

			_EmissionMap2 = FindProperty ("_EmissionMap2", props);
			_EmissionMapTint2 = FindProperty ("_EmissionMapTint2", props);
			_EmissionMapIntensity2 = FindProperty ("_EmissionMapIntensity2", props);
		
			_Tint2 = FindProperty ("_Tint2", props);
			_SpecularTint2 = FindProperty ("_SpecularTint2", props);
		
			_SpecularIntensity2 = FindProperty ("_SpecularIntensity2", props);
			_2SpecularContrast = FindProperty ("_2SpecularContrast", props);
			_2SpecularBrightness = FindProperty ("_2SpecularBrightness", props);
		
			_Glossiness2 = FindProperty ("_Glossiness2", props);
			_GlossinessIntensity2 = FindProperty ("_GlossinessIntensity2", props);
			_2GlossinessContrast = FindProperty ("_2GlossinessContrast", props);
			_2GlossinessBrightness = FindProperty ("_2GlossinessBrightness", props);
		
			_2Paint1Intensity = FindProperty ("_2Paint1Intensity", props);
			_2Paint2Intensity = FindProperty ("_2Paint2Intensity", props);
		
			_2WornEdgesNoiseMix = FindProperty ("_2WornEdgesNoiseMix", props);
			_2WornEdgesAmount = FindProperty ("_2WornEdgesAmount", props);
			_2WornEdgesOpacity = FindProperty ("_2WornEdgesOpacity", props);
			_2WornEdgesContrast = FindProperty ("_2WornEdgesContrast", props);
			_2WornEdgesBorder = FindProperty ("_2WornEdgesBorder", props);
			_2WornEdgesBorderTint = FindProperty ("_2WornEdgesBorderTint", props);
		
			_2UnderlyingDiffuseTint = FindProperty ("_2UnderlyingDiffuseTint", props);
			_2UnderlyingSpecularTint = FindProperty ("_2UnderlyingSpecularTint", props);
			_2UnderlyingDiffuse = FindProperty ("_2UnderlyingDiffuse", props);
			_2UnderlyingSpecular = FindProperty ("_2UnderlyingSpecular", props);
			_2UnderlyingGlossiness = FindProperty ("_2UnderlyingGlossiness", props);
		
			//_NormalsStrength2 = FindProperty ("_NormalsStrength2", props);
			_BumpMapStrength2 = FindProperty ("_BumpMapStrength2", props);
			_OcclusionMapStrength2 = FindProperty ("_OcclusionMapStrength2", props);

			_2Paint1MaskTex = FindProperty ("_2Paint1MaskTex", props);
			_2Paint1Color = FindProperty ("_2Paint1Color", props);
			_2Paint1NoiseMix = FindProperty ("_2Paint1NoiseMix", props);
			_2Paint1Specular = FindProperty ("_2Paint1Specular", props);
			_2Paint1Glossiness = FindProperty ("_2Paint1Glossiness", props);
			
			_2Paint2MaskTex = FindProperty ("_2Paint2MaskTex", props);
			_2Paint2Color = FindProperty ("_2Paint2Color", props);
			_2Paint2NoiseMix = FindProperty ("_2Paint2NoiseMix", props);
			_2Paint2Specular = FindProperty ("_2Paint2Specular", props);
			_2Paint2Glossiness = FindProperty ("_2Paint2Glossiness", props);

			_2GlobalTransparency = FindProperty ("_2GlobalTransparency", props);
			_2AlbedoTransparency = FindProperty ("_2AlbedoTransparency", props);
			_2Paint1Transparency = FindProperty ("_2Paint1Transparency", props);
			_2Paint2Transparency = FindProperty ("_2Paint2Transparency", props);

			_2MaterialRotation = FindProperty ("_2MaterialRotation", props);
		}

		if ((showID == 3) || (showID == 11)) {

			_Texture3 = FindProperty ("_Texture3", props);
			_BumpMap3 = FindProperty ("_BumpMap3", props);
			_OcclusionMap3 = FindProperty ("_OcclusionMap3", props);
			_SpecularMap3 = FindProperty ("_SpecularMap3", props);
			_UseSpecularMap3 = FindProperty ("_UseSpecularMap3", props);
			_GlossinessFromAlpha3 = FindProperty ("_GlossinessFromAlpha3", props);

			_EmissionMap3 = FindProperty ("_EmissionMap3", props);
			_EmissionMapTint3 = FindProperty ("_EmissionMapTint3", props);
			_EmissionMapIntensity3 = FindProperty ("_EmissionMapIntensity3", props);
		
			_Tint3 = FindProperty ("_Tint3", props);
			_SpecularTint3 = FindProperty ("_SpecularTint3", props);
		
			_SpecularIntensity3 = FindProperty ("_SpecularIntensity3", props);
			_3SpecularContrast = FindProperty ("_3SpecularContrast", props);
			_3SpecularBrightness = FindProperty ("_3SpecularBrightness", props);
		
			_Glossiness3 = FindProperty ("_Glossiness3", props);
			_GlossinessIntensity3 = FindProperty ("_GlossinessIntensity3", props);
			_3GlossinessContrast = FindProperty ("_3GlossinessContrast", props);
			_3GlossinessBrightness = FindProperty ("_3GlossinessBrightness", props);
		
			_3Paint1Intensity = FindProperty ("_3Paint1Intensity", props);
			_3Paint2Intensity = FindProperty ("_3Paint2Intensity", props);
		
			_3WornEdgesNoiseMix = FindProperty ("_3WornEdgesNoiseMix", props);
			_3WornEdgesAmount = FindProperty ("_3WornEdgesAmount", props);
			_3WornEdgesOpacity = FindProperty ("_3WornEdgesOpacity", props);
			_3WornEdgesContrast = FindProperty ("_3WornEdgesContrast", props);
			_3WornEdgesBorder = FindProperty ("_3WornEdgesBorder", props);
			_3WornEdgesBorderTint = FindProperty ("_3WornEdgesBorderTint", props);
		
			_3UnderlyingDiffuseTint = FindProperty ("_3UnderlyingDiffuseTint", props);
			_3UnderlyingSpecularTint = FindProperty ("_3UnderlyingSpecularTint", props);
			_3UnderlyingDiffuse = FindProperty ("_3UnderlyingDiffuse", props);
			_3UnderlyingSpecular = FindProperty ("_3UnderlyingSpecular", props);
			_3UnderlyingGlossiness = FindProperty ("_3UnderlyingGlossiness", props);
		
			//_NormalsStrength3 = FindProperty ("_NormalsStrength3", props);
			_BumpMapStrength3 = FindProperty ("_BumpMapStrength3", props);
			_OcclusionMapStrength3 = FindProperty ("_OcclusionMapStrength3", props);

			_3Paint1MaskTex = FindProperty ("_3Paint1MaskTex", props);
			_3Paint1Color = FindProperty ("_3Paint1Color", props);
			_3Paint1NoiseMix = FindProperty ("_3Paint1NoiseMix", props);
			_3Paint1Specular = FindProperty ("_3Paint1Specular", props);
			_3Paint1Glossiness = FindProperty ("_3Paint1Glossiness", props);
			
			_3Paint2MaskTex = FindProperty ("_3Paint2MaskTex", props);
			_3Paint2Color = FindProperty ("_3Paint2Color", props);
			_3Paint2NoiseMix = FindProperty ("_3Paint2NoiseMix", props);
			_3Paint2Specular = FindProperty ("_3Paint2Specular", props);
			_3Paint2Glossiness = FindProperty ("_3Paint2Glossiness", props);

			_3GlobalTransparency = FindProperty ("_3GlobalTransparency", props);
			_3AlbedoTransparency = FindProperty ("_3AlbedoTransparency", props);
			_3Paint1Transparency = FindProperty ("_3Paint1Transparency", props);
			_3Paint2Transparency = FindProperty ("_3Paint2Transparency", props);

			_3MaterialRotation = FindProperty ("_3MaterialRotation", props);
		}

		if ((showID == 4) || (showID == 11)) {

			_Texture4 = FindProperty ("_Texture4", props);
			_BumpMap4 = FindProperty ("_BumpMap4", props);
			_OcclusionMap4 = FindProperty ("_OcclusionMap4", props);
			_SpecularMap4 = FindProperty ("_SpecularMap4", props);
			_UseSpecularMap4 = FindProperty ("_UseSpecularMap4", props);
			_GlossinessFromAlpha4 = FindProperty ("_GlossinessFromAlpha4", props);

			_EmissionMap4 = FindProperty ("_EmissionMap4", props);
			_EmissionMapTint4 = FindProperty ("_EmissionMapTint4", props);
			_EmissionMapIntensity4 = FindProperty ("_EmissionMapIntensity4", props);
		
			_Tint4 = FindProperty ("_Tint4", props);
			_SpecularTint4 = FindProperty ("_SpecularTint4", props);
		
			_SpecularIntensity4 = FindProperty ("_SpecularIntensity4", props);
			_4SpecularContrast = FindProperty ("_4SpecularContrast", props);
			_4SpecularBrightness = FindProperty ("_4SpecularBrightness", props);
		
			_Glossiness4 = FindProperty ("_Glossiness4", props);
			_GlossinessIntensity4 = FindProperty ("_GlossinessIntensity4", props);
			_4GlossinessContrast = FindProperty ("_4GlossinessContrast", props);
			_4GlossinessBrightness = FindProperty ("_4GlossinessBrightness", props);
		
			_4Paint1Intensity = FindProperty ("_4Paint1Intensity", props);
			_4Paint2Intensity = FindProperty ("_4Paint2Intensity", props);
		
			_4WornEdgesNoiseMix = FindProperty ("_4WornEdgesNoiseMix", props);
			_4WornEdgesAmount = FindProperty ("_4WornEdgesAmount", props);
			_4WornEdgesOpacity = FindProperty ("_4WornEdgesOpacity", props);
			_4WornEdgesContrast = FindProperty ("_4WornEdgesContrast", props);
			_4WornEdgesBorder = FindProperty ("_4WornEdgesBorder", props);
			_4WornEdgesBorderTint = FindProperty ("_4WornEdgesBorderTint", props);
		
			_4UnderlyingDiffuseTint = FindProperty ("_4UnderlyingDiffuseTint", props);
			_4UnderlyingSpecularTint = FindProperty ("_4UnderlyingSpecularTint", props);
			_4UnderlyingDiffuse = FindProperty ("_4UnderlyingDiffuse", props);
			_4UnderlyingSpecular = FindProperty ("_4UnderlyingSpecular", props);
			_4UnderlyingGlossiness = FindProperty ("_4UnderlyingGlossiness", props);
		
			//_NormalsStrength4 = FindProperty ("_NormalsStrength4", props);
			_BumpMapStrength4 = FindProperty ("_BumpMapStrength4", props);
			_OcclusionMapStrength4 = FindProperty ("_OcclusionMapStrength4", props);

			_4Paint1MaskTex = FindProperty ("_4Paint1MaskTex", props);
			_4Paint1Color = FindProperty ("_4Paint1Color", props);
			_4Paint1NoiseMix = FindProperty ("_4Paint1NoiseMix", props);
			_4Paint1Specular = FindProperty ("_4Paint1Specular", props);
			_4Paint1Glossiness = FindProperty ("_4Paint1Glossiness", props);
			
			_4Paint2MaskTex = FindProperty ("_4Paint2MaskTex", props);
			_4Paint2Color = FindProperty ("_4Paint2Color", props);
			_4Paint2NoiseMix = FindProperty ("_4Paint2NoiseMix", props);
			_4Paint2Specular = FindProperty ("_4Paint2Specular", props);
			_4Paint2Glossiness = FindProperty ("_4Paint2Glossiness", props);

			_4GlobalTransparency = FindProperty ("_4GlobalTransparency", props);
			_4AlbedoTransparency = FindProperty ("_4AlbedoTransparency", props);
			_4Paint1Transparency = FindProperty ("_4Paint1Transparency", props);
			_4Paint2Transparency = FindProperty ("_4Paint2Transparency", props);

			_4MaterialRotation = FindProperty ("_4MaterialRotation", props);
		}

		if ((showID == 5) || (showID == 11)) {

			_Texture5 = FindProperty ("_Texture5", props);
			_BumpMap5 = FindProperty ("_BumpMap5", props);
			_OcclusionMap5 = FindProperty ("_OcclusionMap5", props);
			_SpecularMap5 = FindProperty ("_SpecularMap5", props);
			_UseSpecularMap5 = FindProperty ("_UseSpecularMap5", props);
			_GlossinessFromAlpha5 = FindProperty ("_GlossinessFromAlpha5", props);

			_EmissionMap5 = FindProperty ("_EmissionMap5", props);
			_EmissionMapTint5 = FindProperty ("_EmissionMapTint5", props);
			_EmissionMapIntensity5 = FindProperty ("_EmissionMapIntensity5", props);
		
			_Tint5 = FindProperty ("_Tint5", props);
			_SpecularTint5 = FindProperty ("_SpecularTint5", props);
		
			_SpecularIntensity5 = FindProperty ("_SpecularIntensity5", props);
			_5SpecularContrast = FindProperty ("_5SpecularContrast", props);
			_5SpecularBrightness = FindProperty ("_5SpecularBrightness", props);
		
			_Glossiness5 = FindProperty ("_Glossiness5", props);
			_GlossinessIntensity5 = FindProperty ("_GlossinessIntensity5", props);
			_5GlossinessContrast = FindProperty ("_5GlossinessContrast", props);
			_5GlossinessBrightness = FindProperty ("_5GlossinessBrightness", props);
		
			_5Paint1Intensity = FindProperty ("_5Paint1Intensity", props);
			_5Paint2Intensity = FindProperty ("_5Paint2Intensity", props);
		
			_5WornEdgesNoiseMix = FindProperty ("_5WornEdgesNoiseMix", props);
			_5WornEdgesAmount = FindProperty ("_5WornEdgesAmount", props);
			_5WornEdgesOpacity = FindProperty ("_5WornEdgesOpacity", props);
			_5WornEdgesContrast = FindProperty ("_5WornEdgesContrast", props);
			_5WornEdgesBorder = FindProperty ("_5WornEdgesBorder", props);
			_5WornEdgesBorderTint = FindProperty ("_5WornEdgesBorderTint", props);
		
			_5UnderlyingDiffuseTint = FindProperty ("_5UnderlyingDiffuseTint", props);
			_5UnderlyingSpecularTint = FindProperty ("_5UnderlyingSpecularTint", props);
			_5UnderlyingDiffuse = FindProperty ("_5UnderlyingDiffuse", props);
			_5UnderlyingSpecular = FindProperty ("_5UnderlyingSpecular", props);
			_5UnderlyingGlossiness = FindProperty ("_5UnderlyingGlossiness", props);
		
			//_NormalsStrength5 = FindProperty ("_NormalsStrength5", props);
			_BumpMapStrength5 = FindProperty ("_BumpMapStrength5", props);
			_OcclusionMapStrength5 = FindProperty ("_OcclusionMapStrength5", props);

			_5Paint1MaskTex = FindProperty ("_5Paint1MaskTex", props);
			_5Paint1Color = FindProperty ("_5Paint1Color", props);
			_5Paint1NoiseMix = FindProperty ("_5Paint1NoiseMix", props);
			_5Paint1Specular = FindProperty ("_5Paint1Specular", props);
			_5Paint1Glossiness = FindProperty ("_5Paint1Glossiness", props);
			
			_5Paint2MaskTex = FindProperty ("_5Paint2MaskTex", props);
			_5Paint2Color = FindProperty ("_5Paint2Color", props);
			_5Paint2NoiseMix = FindProperty ("_5Paint2NoiseMix", props);
			_5Paint2Specular = FindProperty ("_5Paint2Specular", props);
			_5Paint2Glossiness = FindProperty ("_5Paint2Glossiness", props);

			_5GlobalTransparency = FindProperty ("_5GlobalTransparency", props);
			_5AlbedoTransparency = FindProperty ("_5AlbedoTransparency", props);
			_5Paint1Transparency = FindProperty ("_5Paint1Transparency", props);
			_5Paint2Transparency = FindProperty ("_5Paint2Transparency", props);

			_5MaterialRotation = FindProperty ("_5MaterialRotation", props);
		}

		if ((showID == 6) || (showID == 11)) {

			_Texture6 = FindProperty ("_Texture6", props);
			_BumpMap6 = FindProperty ("_BumpMap6", props);
			_OcclusionMap6 = FindProperty ("_OcclusionMap6", props);
			_SpecularMap6 = FindProperty ("_SpecularMap6", props);
			_UseSpecularMap6 = FindProperty ("_UseSpecularMap6", props);
			_GlossinessFromAlpha6 = FindProperty ("_GlossinessFromAlpha6", props);

			_EmissionMap6 = FindProperty ("_EmissionMap6", props);
			_EmissionMapTint6 = FindProperty ("_EmissionMapTint6", props);
			_EmissionMapIntensity6 = FindProperty ("_EmissionMapIntensity6", props);
		
			_Tint6 = FindProperty ("_Tint6", props);
			_SpecularTint6 = FindProperty ("_SpecularTint6", props);
		
			_SpecularIntensity6 = FindProperty ("_SpecularIntensity6", props);
			_6SpecularContrast = FindProperty ("_6SpecularContrast", props);
			_6SpecularBrightness = FindProperty ("_6SpecularBrightness", props);
		
			_Glossiness6 = FindProperty ("_Glossiness6", props);
			_GlossinessIntensity6 = FindProperty ("_GlossinessIntensity6", props);
			_6GlossinessContrast = FindProperty ("_6GlossinessContrast", props);
			_6GlossinessBrightness = FindProperty ("_6GlossinessBrightness", props);
		
			_6Paint1Intensity = FindProperty ("_6Paint1Intensity", props);
			_6Paint2Intensity = FindProperty ("_6Paint2Intensity", props);
		
			_6WornEdgesNoiseMix = FindProperty ("_6WornEdgesNoiseMix", props);
			_6WornEdgesAmount = FindProperty ("_6WornEdgesAmount", props);
			_6WornEdgesOpacity = FindProperty ("_6WornEdgesOpacity", props);
			_6WornEdgesContrast = FindProperty ("_6WornEdgesContrast", props);
			_6WornEdgesBorder = FindProperty ("_6WornEdgesBorder", props);
			_6WornEdgesBorderTint = FindProperty ("_6WornEdgesBorderTint", props);
		
			_6UnderlyingDiffuseTint = FindProperty ("_6UnderlyingDiffuseTint", props);
			_6UnderlyingSpecularTint = FindProperty ("_6UnderlyingSpecularTint", props);
			_6UnderlyingDiffuse = FindProperty ("_6UnderlyingDiffuse", props);
			_6UnderlyingSpecular = FindProperty ("_6UnderlyingSpecular", props);
			_6UnderlyingGlossiness = FindProperty ("_6UnderlyingGlossiness", props);
		
			//_NormalsStrength6 = FindProperty ("_NormalsStrength6", props);
			_BumpMapStrength6 = FindProperty ("_BumpMapStrength6", props);
			_OcclusionMapStrength6 = FindProperty ("_OcclusionMapStrength6", props);

			_6Paint1MaskTex = FindProperty ("_6Paint1MaskTex", props);
			_6Paint1Color = FindProperty ("_6Paint1Color", props);
			_6Paint1NoiseMix = FindProperty ("_6Paint1NoiseMix", props);
			_6Paint1Specular = FindProperty ("_6Paint1Specular", props);
			_6Paint1Glossiness = FindProperty ("_6Paint1Glossiness", props);
			
			_6Paint2MaskTex = FindProperty ("_6Paint2MaskTex", props);
			_6Paint2Color = FindProperty ("_6Paint2Color", props);
			_6Paint2NoiseMix = FindProperty ("_6Paint2NoiseMix", props);
			_6Paint2Specular = FindProperty ("_6Paint2Specular", props);
			_6Paint2Glossiness = FindProperty ("_6Paint2Glossiness", props);

			_6GlobalTransparency = FindProperty ("_6GlobalTransparency", props);
			_6AlbedoTransparency = FindProperty ("_6AlbedoTransparency", props);
			_6Paint1Transparency = FindProperty ("_6Paint1Transparency", props);
			_6Paint2Transparency = FindProperty ("_6Paint2Transparency", props);

			_6MaterialRotation = FindProperty ("_6MaterialRotation", props);
		}

		if ((showID == 7) || (showID == 11)) {

			_Texture7 = FindProperty ("_Texture7", props);
			_BumpMap7 = FindProperty ("_BumpMap7", props);
			_OcclusionMap7 = FindProperty ("_OcclusionMap7", props);
			_SpecularMap7 = FindProperty ("_SpecularMap7", props);
			_UseSpecularMap7 = FindProperty ("_UseSpecularMap7", props);
			_GlossinessFromAlpha7 = FindProperty ("_GlossinessFromAlpha7", props);

			_EmissionMap7 = FindProperty ("_EmissionMap7", props);
			_EmissionMapTint7 = FindProperty ("_EmissionMapTint7", props);
			_EmissionMapIntensity7 = FindProperty ("_EmissionMapIntensity7", props);
		
			_Tint7 = FindProperty ("_Tint7", props);
			_SpecularTint7 = FindProperty ("_SpecularTint7", props);
		
			_SpecularIntensity7 = FindProperty ("_SpecularIntensity7", props);
			_7SpecularContrast = FindProperty ("_7SpecularContrast", props);
			_7SpecularBrightness = FindProperty ("_7SpecularBrightness", props);
		
			_Glossiness7 = FindProperty ("_Glossiness7", props);
			_GlossinessIntensity7 = FindProperty ("_GlossinessIntensity7", props);
			_7GlossinessContrast = FindProperty ("_7GlossinessContrast", props);
			_7GlossinessBrightness = FindProperty ("_7GlossinessBrightness", props);
		
			_7Paint1Intensity = FindProperty ("_7Paint1Intensity", props);
			_7Paint2Intensity = FindProperty ("_7Paint2Intensity", props);
		
			_7WornEdgesNoiseMix = FindProperty ("_7WornEdgesNoiseMix", props);
			_7WornEdgesAmount = FindProperty ("_7WornEdgesAmount", props);
			_7WornEdgesOpacity = FindProperty ("_7WornEdgesOpacity", props);
			_7WornEdgesContrast = FindProperty ("_7WornEdgesContrast", props);
			_7WornEdgesBorder = FindProperty ("_7WornEdgesBorder", props);
			_7WornEdgesBorderTint = FindProperty ("_7WornEdgesBorderTint", props);
		
			_7UnderlyingDiffuseTint = FindProperty ("_7UnderlyingDiffuseTint", props);
			_7UnderlyingSpecularTint = FindProperty ("_7UnderlyingSpecularTint", props);
			_7UnderlyingDiffuse = FindProperty ("_7UnderlyingDiffuse", props);
			_7UnderlyingSpecular = FindProperty ("_7UnderlyingSpecular", props);
			_7UnderlyingGlossiness = FindProperty ("_7UnderlyingGlossiness", props);
		
			//_NormalsStrength7 = FindProperty ("_NormalsStrength7", props);
			_BumpMapStrength7 = FindProperty ("_BumpMapStrength7", props);
			_OcclusionMapStrength7 = FindProperty ("_OcclusionMapStrength7", props);

			_7Paint1MaskTex = FindProperty ("_7Paint1MaskTex", props);
			_7Paint1Color = FindProperty ("_7Paint1Color", props);
			_7Paint1NoiseMix = FindProperty ("_7Paint1NoiseMix", props);
			_7Paint1Specular = FindProperty ("_7Paint1Specular", props);
			_7Paint1Glossiness = FindProperty ("_7Paint1Glossiness", props);
			
			_7Paint2MaskTex = FindProperty ("_7Paint2MaskTex", props);
			_7Paint2Color = FindProperty ("_7Paint2Color", props);
			_7Paint2NoiseMix = FindProperty ("_7Paint2NoiseMix", props);
			_7Paint2Specular = FindProperty ("_7Paint2Specular", props);
			_7Paint2Glossiness = FindProperty ("_7Paint2Glossiness", props);

			_7GlobalTransparency = FindProperty ("_7GlobalTransparency", props);
			_7AlbedoTransparency = FindProperty ("_7AlbedoTransparency", props);
			_7Paint1Transparency = FindProperty ("_7Paint1Transparency", props);
			_7Paint2Transparency = FindProperty ("_7Paint2Transparency", props);

			_7MaterialRotation = FindProperty ("_7MaterialRotation", props);
		}

		if ((showID == 8) || (showID == 11)) {

			//_DirtTexture1 = FindProperty ("_DirtTexture1", props);
			_Dirt1Tint = FindProperty ("_Dirt1Tint", props);
			_DirtNoise1Mix = FindProperty ("_DirtNoise1Mix", props);
			_Dirt1Amount = FindProperty ("_Dirt1Amount", props);
			_Dirt1Contrast = FindProperty ("_Dirt1Contrast", props);
			_Dirt1Opacity = FindProperty ("_Dirt1Opacity", props);

			//_DirtTexture2 = FindProperty ("_DirtTexture2", props);
			_Dirt2Tint = FindProperty ("_Dirt2Tint", props);
			_DirtNoise2Mix = FindProperty ("_DirtNoise2Mix", props);
			_Dirt2Amount = FindProperty ("_Dirt2Amount", props);
			_Dirt2Contrast = FindProperty ("_Dirt2Contrast", props);
			_Dirt2Opacity = FindProperty ("_Dirt2Opacity", props);
		}


		if ((showID == 9) || (showID == 11)) {

			_0EmissionTint = FindProperty ("_0EmissionTint", props);
			_0EmissionIntensity = FindProperty ("_0EmissionIntensity", props);
			_1EmissionTint = FindProperty ("_1EmissionTint", props);
			_1EmissionIntensity = FindProperty ("_1EmissionIntensity", props);
		}

		if ((showID == 10) || (showID == 11)) {

			_specMin = FindProperty ("_specMin", props);
			_specMax = FindProperty ("_specMax", props);
			
			_glossMin = FindProperty ("_glossMin", props);
			_glossMax = FindProperty ("_glossMax", props);

			_gamma = FindProperty ("_gamma", props);
			_minInput = FindProperty ("_minInput", props);
			_maxInput = FindProperty ("_maxInput", props);
			_minOutput = FindProperty ("_minOutput", props);
			_maxOutput = FindProperty ("_maxOutput", props);

			_Hue = FindProperty ("_Hue", props);
			_Saturation = FindProperty ("_Saturation", props);
			_Brightness = FindProperty ("_Brightness", props);
			_Contrast = FindProperty ("_Contrast", props);

			_GlobalScale = FindProperty ("_GlobalScale", props);
		}


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
			if (EditorGUIUtility.isProSkin) {
				boxBackground.SetPixel(0,0, new Color(1, 1, 1, 0.05f));
			}
			else {
				boxBackground.SetPixel(0,0, new Color(1, 1, 1, 0.175f));
			}
			boxBackground.Apply();
			boxStyle = new GUIStyle();
			boxStyle.normal.background = boxBackground;
		}

		if (labelStyle == null) {
			labelStyle = new GUIStyle(EditorStyles.boldLabel);
			if (EditorGUIUtility.isProSkin) {
				labelStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f, 1);
			}
			else {
				labelStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1);
			}
		}

	}


	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props) {

		colorRectWidth = 55.0f;
		colorRectHeigth = 16.0f;

		CreateStyles();

		showID = FindProperty ("_ShowID", props).floatValue;

		FindProperties (props); 


		if (Event.current.type == EventType.Layout) {   

			if ((showID == 0) || (showID == 11)) {
				if (_SpecularMap0.textureValue == null) {
					specularMap0set = false;
					_UseSpecularMap0.floatValue = 0;
				}
				else {
					specularMap0set = true;
					_UseSpecularMap0.floatValue = 1.0f;
				}
			}

			if ((showID == 1) || (showID == 11)) {
				if (_SpecularMap1.textureValue == null) { 
					specularMap1set = false;
					_UseSpecularMap1.floatValue = 0;
				}
				else {
					specularMap1set = true;
					_UseSpecularMap1.floatValue = 1.0f;
				}
			}

			if ((showID == 2) || (showID == 11)) {
				if (_SpecularMap2.textureValue == null) { 
					specularMap2set = false;
					_UseSpecularMap2.floatValue = 0;
				}
				else {
					specularMap2set = true;
					_UseSpecularMap2.floatValue = 1.0f;
				}
			}

			if ((showID == 3) || (showID == 11)) {
				if (_SpecularMap3.textureValue == null) {
					specularMap3set = false;
					_UseSpecularMap3.floatValue = 0;
				}
				else {
					specularMap3set = true;
					_UseSpecularMap3.floatValue = 1.0f;
				}
			}

			if ((showID == 4) || (showID == 11)) {
				if (_SpecularMap4.textureValue == null) {
					specularMap4set = false;
					_UseSpecularMap4.floatValue = 0;
				}
				else {
					specularMap4set = true;
					_UseSpecularMap4.floatValue = 1.0f;
				}
			}

			if ((showID == 5) || (showID == 11)) {
				if (_SpecularMap5.textureValue == null) {
					specularMap5set = false;
					_UseSpecularMap5.floatValue = 0;
				}
				else {
					specularMap5set = true;
					_UseSpecularMap5.floatValue = 1.0f;
				}
			}

			if ((showID == 6) || (showID == 11)) {
				if (_SpecularMap6.textureValue == null) {
					specularMap6set = false;
					_UseSpecularMap6.floatValue = 0;
				}
				else {
					specularMap6set = true;
					_UseSpecularMap6.floatValue = 1.0f;
				}
			}

			if ((showID == 7) || (showID == 11)) {
				if (_SpecularMap7.textureValue == null) {
					specularMap7set = false;
					_UseSpecularMap7.floatValue = 0;
				}
				else {
					specularMap7set = true;
					_UseSpecularMap7.floatValue = 1.0f;
				}
			}


		}
		
		
		if ((showID == 0) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture0); 

			EditorGUIUtility.labelWidth = 1f;

			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture0);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
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

		if ((showID == 1) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture1); 
			
			EditorGUIUtility.labelWidth = 1f;
			
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture1);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint1, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();



			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap1); 
			
			EditorGUIUtility.labelWidth = 1f;
			
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap1);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength1, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap1); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength1, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap1); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap1set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha1.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha1.floatValue = 1.0f;
				else _GlossinessFromAlpha1.floatValue = 0;
				
				_UseSpecularMap1.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint1, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity1, "Detail Intensity");
			materialEditor.RangeProperty (_1SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_1SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness1, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity1, "Detail Intensity");
			materialEditor.RangeProperty (_1GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_1GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_1Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_1Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_1WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_1WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_1WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_1WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_1WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_1UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_1UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_1UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength1, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _1Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _1Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_1Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_1Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_1Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _1Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _1Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_1Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_1Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_1Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();




			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap1); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap1);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint1, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity1, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();




			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_1GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_1AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_1Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_1Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_1MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		if ((showID == 2) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture2); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture2);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint2, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();



			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap2); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap2);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength2, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap2); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength2, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap2); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap2set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha2.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha2.floatValue = 1.0f;
				else _GlossinessFromAlpha2.floatValue = 0;
				
				_UseSpecularMap2.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint2, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity2, "Detail Intensity");
			materialEditor.RangeProperty (_2SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_2SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness2, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity2, "Detail Intensity");
			materialEditor.RangeProperty (_2GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_2GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_2Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_2Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_2WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_2WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_2WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_2WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_2WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _2WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _2UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _2UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_2UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_2UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_2UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength2, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _2Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _2Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_2Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _2Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_2Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_2Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _2Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _2Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_2Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _2Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_2Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_2Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap2); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap2);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint2, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity2, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();




			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_2GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_2AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_2Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_2Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_2MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

		}

		if ((showID == 3) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture3); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture3);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint3, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap3); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap3);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength3, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap3); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength3, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap3); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap3set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha3.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha3.floatValue = 1.0f;
				else _GlossinessFromAlpha3.floatValue = 0;
				
				_UseSpecularMap3.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint3, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity3, "Detail Intensity");
			materialEditor.RangeProperty (_3SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_3SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness3, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity3, "Detail Intensity");
			materialEditor.RangeProperty (_3GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_3GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_3Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_3Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_3WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_3WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_3WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_3WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_3WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _3WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _3UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _3UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_3UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_3UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_3UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength3, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _3Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _3Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_3Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _3Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_3Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_3Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _3Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _3Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_3Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _3Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_3Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_3Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap3); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap3);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint3, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity3, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_3GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_3AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_3Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_3Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_3MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		if ((showID == 4) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture4); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture4);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint4, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap4); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap4);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength4, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap4); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength4, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap4); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap4set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha4.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha4.floatValue = 1.0f;
				else _GlossinessFromAlpha4.floatValue = 0;
				
				_UseSpecularMap4.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint4, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity4, "Detail Intensity");
			materialEditor.RangeProperty (_4SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_4SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness4, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity4, "Detail Intensity");
			materialEditor.RangeProperty (_4GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_4GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_4Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_4Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_4WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_4WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_4WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_4WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_4WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _4WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _4UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _4UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_4UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_4UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_4UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength4, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _4Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _4Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_4Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _4Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_4Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_4Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _4Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _4Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_4Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _4Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_4Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_4Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap4); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap4);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint4, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity4, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_4GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_4AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_4Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_4Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_4MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		if ((showID == 5) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture5); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture5);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint5, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap5); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap5);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength5, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap5); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength5, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap5); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap5set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha5.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha5.floatValue = 1.0f;
				else _GlossinessFromAlpha5.floatValue = 0;
				
				_UseSpecularMap5.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint5, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity5, "Detail Intensity");
			materialEditor.RangeProperty (_5SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_5SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness5, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity5, "Detail Intensity");
			materialEditor.RangeProperty (_5GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_5GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_5Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_5Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_5WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_5WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_5WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_5WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_5WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _5WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _5UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _5UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_5UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_5UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_5UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength5, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _5Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _5Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_5Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _5Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_5Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_5Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _5Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _5Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_5Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _5Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_5Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_5Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap5); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap5);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint5, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity5, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_5GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_5AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_5Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_5Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_5MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

		}
		if ((showID == 6) || (showID == 11)) {	
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture6); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture6);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint6, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap6); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap6);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength6, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap6); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength6, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap6); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap6set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha6.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha6.floatValue = 1.0f;
				else _GlossinessFromAlpha6.floatValue = 0;
				
				_UseSpecularMap6.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint6, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity6, "Detail Intensity");
			materialEditor.RangeProperty (_6SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_6SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness6, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity6, "Detail Intensity");
			materialEditor.RangeProperty (_6GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_6GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_6Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_6Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_6WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_6WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_6WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_6WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_6WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _6WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _6UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _6UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_6UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_6UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_6UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength6, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _6Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _6Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_6Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _6Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_6Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_6Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _6Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _6Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_6Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _6Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_6Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_6Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap6); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap6);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint6, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity6, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_6GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_6AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_6Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_6Paint2Transparency, "Paint 2 Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_6MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		if ((showID == 7) || (showID == 11)) {	
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._TextureGuiContent, _Texture7); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _Texture7);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical(boxStyle);
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Albedo Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Tint7, "");
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._BumpMapGuiContent, _BumpMap7); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _BumpMap7);
			EditorGUILayout.Separator();
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			materialEditor.RangeProperty (_BumpMapStrength7, "Normals Strength");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();



			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._OcclusionMapGuiContent, _OcclusionMap7); 
			EditorGUIUtility.labelWidth = 1f;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			materialEditor.RangeProperty (_OcclusionMapStrength7, "Occlusion Strength");
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._SpecularMapGuiContent, _SpecularMap7); 
			EditorGUIUtility.labelWidth = 1f;

			if (!specularMap7set) {
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Toggle(false, "Alpha is Glossiness");
				EditorGUI.EndDisabledGroup ();
			}
			else {
				bool glossAlphaToggle = false;
				if (_GlossinessFromAlpha7.floatValue > 0.5f) glossAlphaToggle = true;
				else glossAlphaToggle = false;
				
				EditorGUIUtility.labelWidth = 0;
				glossAlphaToggle = GUILayout.Toggle(glossAlphaToggle, "Alpha is Glossiness");
				EditorGUIUtility.labelWidth = 1f;
				
				if (glossAlphaToggle) _GlossinessFromAlpha7.floatValue = 1.0f;
				else _GlossinessFromAlpha7.floatValue = 0;
				
				_UseSpecularMap7.floatValue = 1.0f;
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
			materialEditor.ColorProperty (colorRect, _SpecularTint7, "");
			GUILayout.EndHorizontal();
			materialEditor.RangeProperty (_SpecularIntensity7, "Detail Intensity");
			materialEditor.RangeProperty (_7SpecularContrast, "Detail Contrast");
			materialEditor.RangeProperty (_7SpecularBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Glossiness7, "Glossiness");
			materialEditor.RangeProperty (_GlossinessIntensity7, "Detail Intensity");
			materialEditor.RangeProperty (_7GlossinessContrast, "Detail Contrast");
			materialEditor.RangeProperty (_7GlossinessBrightness, "Detail Shift");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_7Paint1Intensity, "Paint 1 Intensity");
			materialEditor.RangeProperty (_7Paint2Intensity, "Paint 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUILayout.BeginVertical (Styles.noiseEdges, labelStyle);
			materialEditor.VectorProperty (_7WornEdgesNoiseMix, "Worn Edges Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			materialEditor.RangeProperty (_7WornEdgesAmount, "Worn Edges Amount");
			materialEditor.RangeProperty (_7WornEdgesOpacity, "Worn Edges Opacity");
			materialEditor.RangeProperty (_7WornEdgesContrast, "Worn Edges Contrast");
			materialEditor.RangeProperty (_7WornEdgesBorder, "Worn Edges Border");
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Worn Edges Border");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _7WornEdgesBorderTint, "");
			GUILayout.EndHorizontal();
			
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Diffuse");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _7UnderlyingDiffuseTint, "");
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Underlying Specular");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _7UnderlyingSpecularTint, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_7UnderlyingDiffuse, "Underlying Diffuse");
			materialEditor.RangeProperty (_7UnderlyingSpecular, "Underlying Specular");
			materialEditor.RangeProperty (_7UnderlyingGlossiness, "Underlying Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			/*
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_NormalsStrength7, "Normals Strength");
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
			materialEditor.TexturePropertySingleLine (Styles._Paint1MaskTexGuiContent, _7Paint1MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _7Paint1MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_7Paint1NoiseMix, "Paint 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _7Paint1Color, "");
			GUILayout.EndHorizontal();
			
			
			
			materialEditor.RangeProperty (_7Paint1Specular, "Paint 1 Specular");
			materialEditor.RangeProperty (_7Paint1Glossiness, "Paint 1 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._Paint2MaskTexGuiContent, _7Paint2MaskTex); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _7Paint2MaskTex);
			
			GUILayout.BeginVertical (Styles.paintNoise, labelStyle);
			materialEditor.VectorProperty (_7Paint2NoiseMix, "Paint 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Paint 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _7Paint2Color, "");
			GUILayout.EndHorizontal();
			
			
			materialEditor.RangeProperty (_7Paint2Specular, "Paint 2 Specular");
			materialEditor.RangeProperty (_7Paint2Glossiness, "Paint 2 Glossiness");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Emission", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._EmissionMapGuiContent, _EmissionMap7); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _EmissionMap7);
			
			EditorGUILayout.Separator();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label ("EmissionMap Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _EmissionMapTint7, "");
			GUILayout.EndHorizontal();
			
			materialEditor.RangeProperty (_EmissionMapIntensity7, "EmissionMap Intensity");
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();



			GUILayout.Label ("Transparency", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_7GlobalTransparency, "Global Transparency");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_7AlbedoTransparency, "Albedo Transparency");
			materialEditor.RangeProperty (_7Paint1Transparency, "Paint 1 Transparency");
			materialEditor.RangeProperty (_7Paint2Transparency, "Paint 2 Transparency"); 
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("Rotation", labelStyle);
			EditorGUILayout.Separator();
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_7MaterialRotation, "Material Rotation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		if ((showID == 8) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();

			/*
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._DirtTexture1GuiContent, _DirtTexture1); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _DirtTexture1);
			*/

			GUILayout.BeginVertical (Styles.dirtNoise, labelStyle);
			materialEditor.VectorProperty (_DirtNoise1Mix, "Dirt 1 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Dirt 1 Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Dirt1Tint, "");
			GUILayout.EndHorizontal();

			materialEditor.RangeProperty (_Dirt1Amount, "Dirt 1 Amount");
			materialEditor.RangeProperty (_Dirt1Contrast, "Dirt 1 Contrast");
			materialEditor.RangeProperty (_Dirt1Opacity, "Dirt 1 Opacity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			EditorGUILayout.Separator();

			/*
			EditorGUIUtility.labelWidth = 0;
			materialEditor.TexturePropertySingleLine (Styles._DirtTexture2GuiContent, _DirtTexture2); 
			EditorGUIUtility.labelWidth = 1f;
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-200.0f, controlRect.yMin, 200.0f, colorRectHeigth);
			materialEditor.TextureScaleOffsetProperty(colorRect, _DirtTexture2);
			*/

			GUILayout.BeginVertical (Styles.dirtNoise, labelStyle);
			materialEditor.VectorProperty (_DirtNoise2Mix, "Dirt 2 Noise");
			GUILayout.EndVertical ();
			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Dirt 2 Tint");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _Dirt2Tint, "");
			GUILayout.EndHorizontal();

			materialEditor.RangeProperty (_Dirt2Amount, "Dirt 2 Amount");
			materialEditor.RangeProperty (_Dirt2Contrast, "Dirt 2 Contrast");
			materialEditor.RangeProperty (_Dirt2Opacity, "Dirt 2 Opacity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

		}


		if ((showID == 9) || (showID == 11)) {
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Emission 1 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _0EmissionTint, "");
			GUILayout.EndHorizontal();

			materialEditor.RangeProperty (_0EmissionIntensity, "Emission 1 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.Label ("Emission 2 Color");
			controlRect = EditorGUILayout.GetControlRect();
			colorRect = new Rect(controlRect.xMax-colorRectWidth, controlRect.yMin, colorRectWidth, colorRectHeigth);
			materialEditor.ColorProperty (colorRect, _1EmissionTint, "");
			GUILayout.EndHorizontal();

			materialEditor.RangeProperty (_1EmissionIntensity, "Emission 2 Intensity");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

		}
		if ((showID == 10) || (showID == 11)) {
			GUILayout.Label ("Specular", labelStyle);
			EditorGUILayout.Separator();

			GUILayout.BeginVertical(Styles.specularAdjust, labelStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_specMin, "Specular Min");
			materialEditor.RangeProperty (_specMax, "Specular Max");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.Label ("Glossiness", labelStyle);
			EditorGUILayout.Separator();
			
			GUILayout.BeginVertical(Styles.glossinessAdjust, boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_glossMin, "Glossiness Min");
			materialEditor.RangeProperty (_glossMax, "Glossiness Max");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			GUILayout.Label ("Levels", labelStyle);
			EditorGUILayout.Separator();

			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_gamma, "Gamma");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_minInput, "Min Input");
			materialEditor.RangeProperty (_maxInput, "Max Input");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_minOutput, "Min Output");
			materialEditor.RangeProperty (_maxOutput, "Max Output");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


			GUILayout.Label ("HSBC", labelStyle);
			EditorGUILayout.Separator();

			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Hue, "Hue");
			materialEditor.RangeProperty (_Saturation, "Saturation");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_Brightness, "Brightness");
			materialEditor.RangeProperty (_Contrast, "Contrast");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.Separator();
			materialEditor.RangeProperty (_GlobalScale, "Global Texture scale");
			EditorGUILayout.Separator();
			GUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();


		}


		
	}
}
#endif