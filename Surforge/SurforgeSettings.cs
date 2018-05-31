#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class SurforgeSettings : MonoBehaviour {

	public SurforgeOctree octree;

	public int currentLayer;
	public float scale = 1;
	public int maxBranches = 10;

	public int activeSet;

	public int activeContentPack;
	public SurforgeContentPack[] contentPacks;
	
	[HideInInspector]
	public List<SurforgeLayer> layers;

	[HideInInspector]
	public OctreeGizmosDraw octreeDrawPrefab;
	public GameObject root; 

	public int groupCounter;

	public int lastAction;

	[HideInInspector]
	public bool isBrushActive;

	[HideInInspector]
	public bool isPolygonLassoActive;

	[HideInInspector]
	public bool isPlaceToolActive;

	[HideInInspector]
	public bool isLimitsActive;

	public SurforgeLimits limits;
	public SurforgeLimits textureBorders;

	[HideInInspector]
	public bool canIvyOnGlueproof;

	public SurforgeTexturePreview extentTexturePreview;

	//rendering	
	public Texture2D normalMap;

	public Texture2D emissionMap;	

	public Texture2D objectMasks;

	public Texture2D objectMasks2;

	public Texture2D aoEdgesDirtDepth;

	public Texture2D labelsMap;

	public Texture2D labelsAlpha;


	//rendering temp

	[System.NonSerialized]
	public RenderTexture renderTexture;

	[System.NonSerialized]
	public RenderTexture renderTexture2;


	[System.NonSerialized]
	public RenderTexture edgesRenderTexture;

	[System.NonSerialized]
	public RenderTexture glowRenderTexture;
	

	[System.NonSerialized]
	public Texture2D edgesTexture;
	

	[System.NonSerialized]
	public Texture2D glowTexture;
	

	[System.NonSerialized]
	public Texture2D bucketTexture;




	public float backgroundQuadBorder;

	[HideInInspector]
	public GameObject backgroundQuad;

	[HideInInspector]
	public GameObject polygonLassoPlane;

	public PolyLassoProfiles polyLassoProfiles;
	public int activePolyLassoProfile;

	public DecalSets decalSets;
	public int activeDecal;

	public NoisePresets noisePresets;
	public int activeNoisePreset;


	//gpu render
	int gpuRenderResolution = 0; //1024, 2048, 4096

	int supersamplingMode = 0; //1, 2

	//map export
	[HideInInspector]
	public string exportPath = "";
	[HideInInspector]
	public string textureName = "texture";

	int maskExportMode = 0; //1, 2

	int aoMode = 0; //1

	//presets
	public SurforgeComposerPresets composerPresets;
	[HideInInspector]
	public int currentPreset = 0;

	[HideInInspector]
	public bool seamless = false;

	[HideInInspector]
	public List<GameObject> seamlessInstances = new List<GameObject>();

	public Shader[] materialGroups;
	public Shader[] floaterMaterialGroups;
		

	public Shader exportDiffuse;
	public Shader exportNormals;
	public Shader exportSpecular;
	public Shader exportGlossiness;
	public Shader exportAo;
	public Shader exportEmission;
	public Shader exportHeightmap;
	public Shader exportMask;
	public Shader exportMaskSeparate;
	public Shader exportAlpha;

	public Shader grayBackgroundPost;

	//model
	public Mesh model;

	public Mesh cubePreviewModel;

	//model bakedAO
	[HideInInspector]
	public Texture2D modelBakedAO;

	//model bakedNormalMap
	[HideInInspector]
	public Texture2D modelBakedNormal;

	//model bakedEdgeMap
	[HideInInspector]
	public Texture2D modelBakedEdgeMap;

	public Shader normalsBakedAddPost;

	//toggle grid
	[HideInInspector]
	public bool showGrid;

	[HideInInspector]
	public bool showUvs;

	[HideInInspector]
	public bool showUvGrid = true;

	[HideInInspector]
	[System.NonSerialized]
	public bool showLastAction;

	[HideInInspector]
	public bool showTextureEdges;

	//toggle snap
	[HideInInspector]
	public bool gloabalSnapActive = true;

	[HideInInspector]
	public bool gridSnapActive = true;

	[HideInInspector]
	public bool uvSnapActive = true;

	[HideInInspector]
	public bool objectSnapActive = true;

	[HideInInspector]
	public bool selfSnapActive = true;

	//texture preview
	[HideInInspector]
	public bool texturePreviewUpdated;

	public List<PolyLassoObject> polyLassoObjects;

	public List<Material> sceneMaterials;

	[HideInInspector]
	public int activeSceneMaterialNumber;

	[HideInInspector]
	public int newMaterialSetsCount;

	public int GetGpuRenderResolution() {
		return gpuRenderResolution;
	}
	public void SetGpuRenderResolution(int value) {
		gpuRenderResolution = value;
	}

	public int GetSupersamplingMode() {
		return supersamplingMode;
	}
	public void SetSupersamplingMode(int value) {
		supersamplingMode = value;
	}

	public int GetActiveSceneMaterialNumber() {
		return activeSceneMaterialNumber;
	}

	public void SetActiveSceneMaterialNumber(int numberToSet) { 
		activeSceneMaterialNumber = numberToSet;
	}

	public int GetMaskExportMode() {
		return maskExportMode;
	}
	public void SetMaskExportMode(int value) {
		maskExportMode = value;
	}

	public int GetAoMode() {
		return aoMode;
	}
	public void SetAoMode(int value) {
		aoMode = value;
	}


	public PlaceMeshes placeMeshes;
	public int activePlaceMesh;

	[HideInInspector]
	public GameObject placeToolPlane;

	public GameObject placeToolPreview;

	public GameObject placeToolPreviewSymmX;
	public GameObject placeToolPreviewSymmZ;
	public GameObject placeToolPreviewSymmXZ;

	public GameObject placeToolPreviewSymmDiagonal;
	public GameObject placeToolPreviewSymmDiagonalX;
	public GameObject placeToolPreviewSymmDiagonalZ;
	public GameObject placeToolPreviewSymmDiagonalXZ;

	public float placeToolVerticalOffset;

	//symmetry
	public Vector3 symmetryPoint = Vector3.zero;

	[HideInInspector]
	public GameObject symmetryParent;

	[HideInInspector]
	public bool symmetry; 

	[HideInInspector]
	public bool symmetryX = true;

	[HideInInspector]
	public bool symmetryZ;

	[HideInInspector]
	public bool symmetryDiagonal;

	public SurforgeSkybox[] skyboxes;
	public int activeSkybox;
	public int materialIconsRenderedWithSkybox;

	public Material[] materials;
	public Material[] loadedMaterials;
	public int activeMaterial;

	public float activeShowID; 

	public Camera renderMaterialIconCameraPrefab;
	public Camera renderMaterialIconCameraLitePrefab;
	public GameObject renderMaterialIconPrefab;
	public Shader renderMaterialIconShader;
	public Texture2D rgbaNoise;
	public Texture2D renderMaterialIconNormal;
	public Texture2D renderMaterialIconAoEdgesDirtDepth;
	public Texture2D renderMaterialIconNoise;

	public Texture2D editorPreviewAoEdgesDirtDepth;
	public Texture2D editorPreviewMasks;
	public Texture2D editorPreviewMasks2;
	
	public Texture2D[] materialButtons;

	public float globalScale = 1.0f;

	public GameObject wireframeObject;
	public bool showWireframe;

	public GameObject floorObject;
	public bool showFloor;

	[HideInInspector]
	public List<Vector3> mirrorLineSolid = new List<Vector3> {Vector3.zero, new Vector3(0, 0, 1.0f)};

	[HideInInspector]
	public List<Vector3> mirrorLineDotted = new List<Vector3> {Vector3.zero, new Vector3(1.0f, 0, 0)};

	[HideInInspector]
	public bool showSymAxis = true;

	public ShatterPresets shatterPresets;
	public int activeShatterPreset;

	[HideInInspector]
	public bool seamlessNeedUpdate;

	public List<List<Vector3>> storedShapes = new List<List<Vector3>>();
	public List<int> storedGroups = new List<int>();


	[HideInInspector]
	public string lastActionText = "";

	[HideInInspector]
	public string lastActionHotkey = "";

	[HideInInspector]
	public string lastActionSecondHotkey = "";

	[HideInInspector]
	public float lastActionTimer = 0;

	[HideInInspector]
	public List<Vector3> warpShape;

	[HideInInspector]
	public Vector3 warpShapeCenterLinePoint = Vector3.zero;

	[HideInInspector]
	public float doubleclickTimer = 0;

	[HideInInspector]
	public float skyboxWindowRepaintTimer = 0;

	[HideInInspector]
	public bool skyboxNeedWindowUpdate;

	public int uvGridStep;

	public Material seamlessHighlight;

	public GameObject mapsExportCameraPrefab;

	public Shader bucketDownscale;
	public Shader bucketsCollect;

	bool skipDestroyRoot;
	bool skipDestoryTexturePreview;

	[HideInInspector]
	public bool modelNeedUpdate;

	public void SkipDestroyRoot() {
		skipDestroyRoot = true;
	}

	public void SkipDestroyTexturePreview() {
		skipDestoryTexturePreview = true;
	}

	[HideInInspector]
	public Material composerMaterialAsset;

	public Object modelObject;


	//linear color space texture flags
	[HideInInspector]
	public bool isPreviewRenderTextureLinear = false;


	[HideInInspector]
	public bool isNormalMapLinear = false;

	[HideInInspector]
	public bool isEmissionMapLinear = false;

	[HideInInspector]
	public bool isObjectMasksLinear = false;

	[HideInInspector]
	public bool isObjectMasks2Linear = false;

	[HideInInspector]
	public bool isAoEdgesDirtDepthLinear = false;

	[HideInInspector]
	public bool isLabelsMapLinear = false;

	[HideInInspector]
	public bool isLabelsAlphaLinear = false;


	[HideInInspector]
	public bool isRenderTextureLinear = false;

	[HideInInspector]
	public bool isRenderTexture2Linear = false;

	[HideInInspector]
	public bool isEdgesRenderTextureLinear = false;

	[HideInInspector]
	public bool isGlowRenderTextureLinear = false;

	[HideInInspector]
	public bool isEdgesTextureLinear = false;

	[HideInInspector]
	public bool isGlowTextureLinear = false;

	[HideInInspector]
	public bool isBucketTextureLinear = false;

	[System.NonSerialized]
	public bool greeblesVoxelized = false;



	[HideInInspector]
	[System.NonSerialized]
	public string[] materialFoldersNames;

	[HideInInspector]
	[System.NonSerialized]
	public int[] materialFoldersMatCount;

	[HideInInspector]
	[System.NonSerialized]
	public bool[] materialFoldersFoldout;


	void OnDestroy(){
		if (root != null) {
			DestroySeamlessInstances();
			if (!skipDestroyRoot) {
				SurforgeRoot surforgeRoot = (SurforgeRoot)root.GetComponent<SurforgeRoot>();
				surforgeRoot.SkipDestroy();
				DestroyImmediate(root.gameObject);
			}
			if (!skipDestoryTexturePreview) {
				if (extentTexturePreview != null) {
					extentTexturePreview.SkipDestroy();
					DestroyImmediate(extentTexturePreview.gameObject);
				}
			}
		}
	}


	void DestroySeamlessInstances() {
		foreach (Transform child in root.transform) {

			PolyLassoObject pObj = (PolyLassoObject)child.gameObject.GetComponent<PolyLassoObject>();
			if (pObj != null) {
				if (pObj.seamlessInstances != null) {
					for (int i=0; i<pObj.seamlessInstances.Length; i++) {
						if (pObj.seamlessInstances[i] != null) {
							if (pObj.seamlessInstances[i].gameObject != null) DestroyImmediate(pObj.seamlessInstances[i].gameObject);
						}
					}
				}
			}

			else {
				PlaceMesh placeMeshObj = (PlaceMesh)child.gameObject.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) {
					if (placeMeshObj.seamlessInstances != null) {
						for (int i=0; i<placeMeshObj.seamlessInstances.Length; i++) {
							if (placeMeshObj.seamlessInstances[i] != null) {
								if (placeMeshObj.seamlessInstances[i].gameObject != null) DestroyImmediate(placeMeshObj.seamlessInstances[i].gameObject);
							}
						}
					}
				}
			}


		}

	}




}
#endif