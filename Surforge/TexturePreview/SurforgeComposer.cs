#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class SurforgeComposer : MonoBehaviour {

	[HideInInspector]
	public SurforgeSettings surforgeSettings;

	public Texture2D modelBakedAO;

	public MeshFilter meshFilter;

	public Transform[] directions;

	int cameraDir;
	int oldCameraDir;

	bool oldSeamlessMode;

	public Mesh[] seamlessPreviewMeshes;

	void Start () {
		EditorApplication.update += RelinkMaps;
	}


	void Update() {
		if (surforgeSettings) {
			cameraDir = FindClosestCameraDirection();

			if ((oldCameraDir != cameraDir) || (oldSeamlessMode != surforgeSettings.seamless)) {
				ChangeSemlessPreviewMesh();
			}
			oldCameraDir = cameraDir;
			oldSeamlessMode = surforgeSettings.seamless;
		}
	}

	bool IsPreviewOnCube() {
		if (surforgeSettings.model == surforgeSettings.cubePreviewModel) {
			return true;
		}
		for (int i=0; i < seamlessPreviewMeshes.Length; i++) {
			if (surforgeSettings.model == seamlessPreviewMeshes[i]) return true;
		}
		return false;
	}

	public void ChangeSemlessPreviewMesh() {
		if (IsPreviewOnCube()) {

			if (surforgeSettings.seamless) {
				//Debug.Log ("Camera dir changed: " + cameraDir);
				surforgeSettings.model = seamlessPreviewMeshes[cameraDir];
				if (cameraDir == 0) surforgeSettings.model = seamlessPreviewMeshes[1];
				if (cameraDir == 1) surforgeSettings.model = seamlessPreviewMeshes[0];

				surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh = surforgeSettings.model;
				MeshCollider meshCollider = (MeshCollider)surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
				meshCollider.sharedMesh = surforgeSettings.model;
			}
			else {
				if (surforgeSettings.model != surforgeSettings.cubePreviewModel) {
					surforgeSettings.model = surforgeSettings.cubePreviewModel;
					surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh = surforgeSettings.model;
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
					meshCollider.sharedMesh = surforgeSettings.model;
				}
			}

		}
	}


	int FindClosestCameraDirection() {
		float minDistance = Mathf.Infinity;
		int minDistanceIndex = 0;
		for (int i=0; i < directions.Length; i++) {
			float currentDistance = Vector3.Distance(directions[i].position, surforgeSettings.extentTexturePreview.previewCamera.transform.position);
			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				minDistanceIndex = i;
			}
		}
		return minDistanceIndex;
	}


	public void RelinkMaps() { 
		if (surforgeSettings != null) {
			Renderer renderer = this.GetComponent<Renderer>();

			renderer.sharedMaterial.SetTexture("_NormalMap", surforgeSettings.normalMap);
			renderer.sharedMaterial.SetTexture("_ObjectMasks", surforgeSettings.objectMasks);
			renderer.sharedMaterial.SetTexture("_ObjectMasks2", surforgeSettings.objectMasks2);
			renderer.sharedMaterial.SetTexture("_AoEdgesDirtDepth", surforgeSettings.aoEdgesDirtDepth);
			renderer.sharedMaterial.SetTexture("_EmissionMask", surforgeSettings.emissionMap);
			renderer.sharedMaterial.SetTexture("_LabelTexture", surforgeSettings.labelsMap);

			if (PlayerSettings.colorSpace == ColorSpace.Linear) {
				renderer.sharedMaterial.SetInt("_LinearColorSpace", 1);
			}
			else {
				renderer.sharedMaterial.SetInt("_LinearColorSpace", 0); 
			}

			//Debug.Log ("relink maps!");

			UpdateComposerMaterialAsset(renderer.sharedMaterial);
		}
	}

	void UpdateComposerMaterialAsset(Material mat) {
		if (surforgeSettings.composerMaterialAsset == null) {
			surforgeSettings.composerMaterialAsset = (Material)AssetDatabase.LoadAssetAtPath("Assets/Surforge/TexturePreview/composerMaterial.mat", typeof(Material));
			//Debug.Log (surforgeSettings.composerMaterialAsset);
		}

		CopySurforgeMaterialComplete(mat, surforgeSettings.composerMaterialAsset);
	}

	void CopySurforgeMaterialComplete(Material sourceMaterial,  Material destMaterial) {
		destMaterial.renderQueue = -1;

		destMaterial.SetTexture("_NormalMap", sourceMaterial.GetTexture("_NormalMap"));
		destMaterial.SetTexture("_ObjectMasks", sourceMaterial.GetTexture("_ObjectMasks"));
		destMaterial.SetTexture("_ObjectMasks2", sourceMaterial.GetTexture("_ObjectMasks2"));
		destMaterial.SetTexture("_AoEdgesDirtDepth", sourceMaterial.GetTexture("_AoEdgesDirtDepth"));
		destMaterial.SetTexture("_EmissionMask", sourceMaterial.GetTexture("_EmissionMask"));

		destMaterial.SetTexture("_RGBAnoise", sourceMaterial.GetTexture("_RGBAnoise"));

		destMaterial.SetFloat("_ShowID", sourceMaterial.GetFloat("_ShowID"));

		destMaterial.SetColor("_LabelTint", sourceMaterial.GetColor ("_LabelTint"));
		destMaterial.SetTexture("_LabelTexture", sourceMaterial.GetTexture("_LabelTexture"));
		destMaterial.SetFloat("_LabelSpecularIntensity", sourceMaterial.GetFloat ("_LabelSpecularIntensity"));
		destMaterial.SetFloat("_LabelGlossiness", sourceMaterial.GetFloat ("_LabelGlossiness"));


		destMaterial.SetInt("_LinearColorSpace", sourceMaterial.GetInt ("_LinearColorSpace"));

		for (int i=0; i<8; i++) {

			destMaterial.SetColor("_Tint" + i, sourceMaterial.GetColor ("_Tint" + i));
			destMaterial.SetColor("_SpecularTint" + i, sourceMaterial.GetColor("_SpecularTint" + i));
		
			destMaterial.SetFloat("_SpecularIntensity" + i, sourceMaterial.GetFloat("_SpecularIntensity" + i));
			destMaterial.SetFloat("_" + i + "SpecularContrast", sourceMaterial.GetFloat("_" + i + "SpecularContrast"));
			destMaterial.SetFloat("_" + i + "SpecularBrightness", sourceMaterial.GetFloat ("_" + i + "SpecularBrightness"));
		
			destMaterial.SetFloat("_Glossiness" + i, sourceMaterial.GetFloat ("_Glossiness" + i));
			destMaterial.SetFloat("_GlossinessIntensity" + i, sourceMaterial.GetFloat ("_GlossinessIntensity" + i));
			destMaterial.SetFloat("_" + i + "GlossinessContrast", sourceMaterial.GetFloat ("_" + i + "GlossinessContrast"));
			destMaterial.SetFloat("_" + i + "GlossinessBrightness", sourceMaterial.GetFloat ("_" + i + "GlossinessBrightness"));
		
			destMaterial.SetFloat("_" + i + "Paint1Intensity", sourceMaterial.GetFloat ("_" + i + "Paint1Intensity"));
			destMaterial.SetFloat("_" + i + "Paint2Intensity", sourceMaterial.GetFloat ("_" + i + "Paint2Intensity"));
			destMaterial.SetVector("_" + i + "WornEdgesNoiseMix", sourceMaterial.GetVector("_" + i + "WornEdgesNoiseMix"));
			destMaterial.SetFloat("_" + i + "WornEdgesAmount", sourceMaterial.GetFloat ("_" + i + "WornEdgesAmount"));
			destMaterial.SetFloat("_" + i + "WornEdgesOpacity", sourceMaterial.GetFloat ("_" + i + "WornEdgesOpacity"));
			destMaterial.SetFloat("_" + i + "WornEdgesContrast", sourceMaterial.GetFloat ("_" + i + "WornEdgesContrast"));
			destMaterial.SetFloat("_" + i + "WornEdgesBorder", sourceMaterial.GetFloat ("_" + i + "WornEdgesBorder"));
			destMaterial.SetColor("_" + i + "WornEdgesBorderTint", sourceMaterial.GetColor ("_" + i + "WornEdgesBorderTint"));
			destMaterial.SetColor("_" + i + "UnderlyingDiffuseTint", sourceMaterial.GetColor ("_" + i + "UnderlyingDiffuseTint"));
			destMaterial.SetColor("_" + i + "UnderlyingSpecularTint", sourceMaterial.GetColor ("_" + i + "UnderlyingSpecularTint"));
			destMaterial.SetFloat("_" + i + "UnderlyingDiffuse", sourceMaterial.GetFloat ("_" + i + "UnderlyingDiffuse"));
			destMaterial.SetFloat("_" + i + "UnderlyingSpecular", sourceMaterial.GetFloat ("_" + i + "UnderlyingSpecular"));
			destMaterial.SetFloat("_" + i + "UnderlyingGlossiness", sourceMaterial.GetFloat ("_" + i + "UnderlyingGlossiness"));
		
			destMaterial.SetFloat("_NormalsStrength" + i, sourceMaterial.GetFloat ("_NormalsStrength" + i));
			destMaterial.SetFloat("_BumpMapStrength" + i, sourceMaterial.GetFloat ("_BumpMapStrength" + i));

			destMaterial.SetFloat("_OcclusionMapStrength" + i, sourceMaterial.GetFloat ("_OcclusionMapStrength" + i));
		
			destMaterial.SetFloat("_" + i + "Paint1Specular", sourceMaterial.GetFloat ("_" + i + "Paint1Specular"));
			destMaterial.SetFloat("_" + i + "Paint1Glossiness", sourceMaterial.GetFloat ("_" + i + "Paint1Glossiness"));
			destMaterial.SetColor("_" + i + "Paint1Color", sourceMaterial.GetColor ("_" + i + "Paint1Color"));
			destMaterial.SetTexture("_" + i + "Paint1MaskTex", sourceMaterial.GetTexture("_" + i + "Paint1MaskTex"));
			destMaterial.SetTextureScale("_" + i + "Paint1MaskTex", sourceMaterial.GetTextureScale("_" + i + "Paint1MaskTex"));
			destMaterial.SetTextureOffset("_" + i + "Paint1MaskTex", sourceMaterial.GetTextureOffset("_" + i + "Paint1MaskTex"));
			destMaterial.SetVector("_" + i + "Paint1NoiseMix", sourceMaterial.GetVector ("_" + i + "Paint1NoiseMix"));
		
			destMaterial.SetFloat("_" + i + "Paint2Specular", sourceMaterial.GetFloat ("_" + i + "Paint2Specular"));
			destMaterial.SetFloat("_" + i + "Paint2Glossiness", sourceMaterial.GetFloat ("_" + i + "Paint2Glossiness"));
			destMaterial.SetColor("_" + i + "Paint2Color", sourceMaterial.GetColor ("_" + i + "Paint2Color"));
			destMaterial.SetTexture("_" + i + "Paint2MaskTex", sourceMaterial.GetTexture ("_" + i + "Paint2MaskTex"));
			destMaterial.SetTextureScale("_" + i + "Paint2MaskTex", sourceMaterial.GetTextureScale ("_" + i + "Paint2MaskTex"));
			destMaterial.SetTextureOffset("_" + i + "Paint2MaskTex", sourceMaterial.GetTextureOffset ("_" + i + "Paint2MaskTex"));
			destMaterial.SetVector("_" + i + "Paint2NoiseMix", sourceMaterial.GetVector ("_" + i + "Paint2NoiseMix"));
		
		
			destMaterial.SetTexture("_Texture" + i, sourceMaterial.GetTexture ("_Texture" + i));
			destMaterial.SetTextureScale("_Texture" + i, sourceMaterial.GetTextureScale ("_Texture" + i));
			destMaterial.SetTextureOffset("_Texture" + i, sourceMaterial.GetTextureOffset ("_Texture" + i));

			destMaterial.SetTexture("_BumpMap" + i, sourceMaterial.GetTexture ("_BumpMap" + i));
			destMaterial.SetTextureScale("_BumpMap" + i, sourceMaterial.GetTextureScale ("_BumpMap" + i));
			destMaterial.SetTextureOffset("_BumpMap" + i, sourceMaterial.GetTextureOffset ("_BumpMap" + i));

			destMaterial.SetTexture("_OcclusionMap" + i, sourceMaterial.GetTexture ("_OcclusionMap" + i));

			destMaterial.SetTexture("_SpecularMap" + i, sourceMaterial.GetTexture ("_SpecularMap" + i));
			destMaterial.SetTextureScale("_SpecularMap" + i, sourceMaterial.GetTextureScale ("_SpecularMap" + i));
			destMaterial.SetTextureOffset("_SpecularMap" + i, sourceMaterial.GetTextureOffset ("_SpecularMap" + i));

			destMaterial.SetFloat("_UseSpecularMap" + i, sourceMaterial.GetFloat ("_UseSpecularMap" + i));
			destMaterial.SetFloat("_GlossinessFromAlpha" + i, sourceMaterial.GetFloat ("_GlossinessFromAlpha" + i));

			destMaterial.SetTexture("_EmissionMap" + i, sourceMaterial.GetTexture ("_EmissionMap" + i));
			destMaterial.SetTextureScale("_EmissionMap" + i, sourceMaterial.GetTextureScale ("_EmissionMap" + i));
			destMaterial.SetTextureOffset("_EmissionMap" + i, sourceMaterial.GetTextureOffset ("_EmissionMap" + i));
			destMaterial.SetColor("_EmissionMapTint" + i, sourceMaterial.GetColor ("_EmissionMapTint" + i));
			destMaterial.SetFloat("_EmissionMapIntensity" + i, sourceMaterial.GetFloat ("_EmissionMapIntensity" + i));

			destMaterial.SetFloat("_" + i + "GlobalTransparency", sourceMaterial.GetFloat ("_" + i + "GlobalTransparency"));
			destMaterial.SetFloat("_" + i + "AlbedoTransparency", sourceMaterial.GetFloat ("_" + i + "AlbedoTransparency"));
			destMaterial.SetFloat("_" + i + "Paint1Transparency", sourceMaterial.GetFloat ("_" + i + "Paint1Transparency"));
			destMaterial.SetFloat("_" + i + "Paint2Transparency", sourceMaterial.GetFloat ("_" + i + "Paint2Transparency"));

			destMaterial.SetFloat("_" + i + "MaterialRotation", sourceMaterial.GetFloat ("_" + i + "MaterialRotation"));
		}

		destMaterial.SetColor("_Dirt1Tint", sourceMaterial.GetColor("_Dirt1Tint"));
		destMaterial.SetVector("_DirtNoise1Mix", sourceMaterial.GetVector("_DirtNoise1Mix"));
		destMaterial.SetFloat("_Dirt1Amount", sourceMaterial.GetFloat("_Dirt1Amount"));
		destMaterial.SetFloat("_Dirt1Contrast", sourceMaterial.GetFloat("_Dirt1Contrast"));
		destMaterial.SetFloat("_Dirt1Opacity", sourceMaterial.GetFloat("_Dirt1Opacity"));
		destMaterial.SetTexture("_DirtTexture1", sourceMaterial.GetTexture("_DirtTexture1"));
		
		destMaterial.SetColor("_Dirt2Tint", sourceMaterial.GetColor("_Dirt2Tint"));
		destMaterial.SetVector("_DirtNoise2Mix", sourceMaterial.GetVector("_DirtNoise2Mix"));
		destMaterial.SetFloat("_Dirt2Amount", sourceMaterial.GetFloat("_Dirt2Amount"));
		destMaterial.SetFloat("_Dirt2Contrast", sourceMaterial.GetFloat("_Dirt2Contrast"));
		destMaterial.SetFloat("_Dirt2Opacity", sourceMaterial.GetFloat("_Dirt2Opacity"));
		destMaterial.SetTexture("_DirtTexture2", sourceMaterial.GetTexture("_DirtTexture2"));


		destMaterial.SetColor("_0EmissionTint", sourceMaterial.GetColor("_0EmissionTint"));
		destMaterial.SetFloat("_0EmissionIntensity", sourceMaterial.GetFloat("_0EmissionIntensity"));
		destMaterial.SetColor("_1EmissionTint", sourceMaterial.GetColor("_1EmissionTint"));
		destMaterial.SetFloat("_1EmissionIntensity", sourceMaterial.GetFloat("_1EmissionIntensity"));

		destMaterial.SetFloat("_specMin", sourceMaterial.GetFloat("_specMin"));
		destMaterial.SetFloat("_specMax", sourceMaterial.GetFloat("_specMax"));
		destMaterial.SetFloat("_glossMin", sourceMaterial.GetFloat("_glossMin"));
		destMaterial.SetFloat("_glossMax", sourceMaterial.GetFloat("_glossMax"));

		destMaterial.SetFloat("_gamma", sourceMaterial.GetFloat("_gamma"));
		destMaterial.SetFloat("_minInput", sourceMaterial.GetFloat("_minInput"));
		destMaterial.SetFloat("_maxInput", sourceMaterial.GetFloat("_maxInput"));
		destMaterial.SetFloat("_minOutput", sourceMaterial.GetFloat("_minOutput"));
		destMaterial.SetFloat("_maxOutput", sourceMaterial.GetFloat("_maxOutput"));

		destMaterial.SetFloat("_Hue", sourceMaterial.GetFloat("_Hue"));
		destMaterial.SetFloat("_Saturation", sourceMaterial.GetFloat("_Saturation"));
		destMaterial.SetFloat("_Brightness", sourceMaterial.GetFloat("_Brightness"));
		destMaterial.SetFloat("_Contrast", sourceMaterial.GetFloat("_Contrast"));

		destMaterial.SetFloat("_GlobalScale", sourceMaterial.GetFloat("_GlobalScale"));


	}


	void OnDisable() {
		EditorApplication.update -= RelinkMaps;
	}


}
#endif