using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SurforgeInterface : EditorWindow 
{	

	class Line2d {
		public Vector2 start;
		public Vector2 end;
		public int lineBrightness; 
	}

	class MaterialGroup {
		public Color _Tint;
		public Color _SpecularTint;
		
		public float _SpecularIntensity;
		public float _SpecularContrast;
		public float _SpecularBrightness;
		
		public float _Glossiness;
		public float _GlossinessIntensity;
		public float _GlossinessContrast;
		public float _GlossinessBrightness;
		
		public float _Paint1Intensity;
		public float _Paint2Intensity;
		public Vector4 _WornEdgesNoiseMix;
		public float _WornEdgesAmount;
		public float _WornEdgesOpacity;
		public float _WornEdgesContrast;
		public float _WornEdgesBorder;
		public Color _WornEdgesBorderTint;
		public Color _UnderlyingDiffuseTint;
		public Color _UnderlyingSpecularTint;
		public float _UnderlyingDiffuse;
		public float _UnderlyingSpecular;
		public float _UnderlyingGlossiness;
		
		public float _NormalsStrength;
		public float _BumpMapStrength;

		public float _OcclusionMapStrength;

		public float _Paint1Specular;
		public float _Paint1Glossiness;
		public Color _Paint1Color;
		public Texture _Paint1MaskTex;
		public Vector2 _Paint1MaskTex_Scale;
		public Vector2 _Paint1MaskTex_Offset;
		public Vector4 _Paint1NoiseMix;
		
		public float _Paint2Specular;
		public float _Paint2Glossiness;
		public Color _Paint2Color;
		public Texture _Paint2MaskTex;
		public Vector2 _Paint2MaskTex_Scale;
		public Vector2 _Paint2MaskTex_Offset;
		public Vector4 _Paint2NoiseMix;

		
		public Texture _Texture;
		public Vector2 _Texture_Scale;
		public Vector2 _Texture_Offset;

		public Texture _BumpMap;
		public Vector2 _BumpMap_Scale;
		public Vector2 _BumpMap_Offset;

		public Texture _OcclusionMap;

		public Texture _SpecularMap;
		public Vector2 _SpecularMap_Scale;
		public Vector2 _SpecularMap_Offset;

		public float _UseSpecularMap; 
		public float _GlossinessFromAlpha;

		public Texture _EmissionMap;
		public Vector2 _EmissionMap_Scale;
		public Vector2 _EmissionMap_Offset;
		public Color _EmissionMapTint;
		public float _EmissionMapIntensity;

		public float _GlobalTransparency;
		public float _AlbedoTransparency;
		public float _Paint1Transparency;
		public float _Paint2Transparency;

		public float _MaterialRotation;
	}


	class DirtSettings {
		public Color _Dirt1Tint;
		public Vector4 _DirtNoise1Mix;
		public float _Dirt1Amount;
		public float _Dirt1Contrast;
		public float _Dirt1Opacity;
		public Texture _DirtTexture1;

		public Color _Dirt2Tint;
		public Vector4 _DirtNoise2Mix;
		public float _Dirt2Amount;
		public float _Dirt2Contrast;
		public float _Dirt2Opacity;
		public Texture _DirtTexture2;
	}

	static string version = "1.1.1";
	
	static SurforgeOctreeNode brushSelectedNode;
	
	static int selectedSetNum = 0;
	static int selectedContentPackNum = 0;
	static Texture[] setButtons;
	static Texture[] contentPackButtons;
	static string[] layerButtons;
	static GUIStyle buttonStyle;
	static GUIStyle buttonStyleActive;
	static GUIStyle layerButtonStyle;
	static GUIStyle contentPackButtonStyle;
	
	static GUIStyle buttonStyleMini;
	static GUIStyle buttonStyleMiniSelected;

	static GUIStyle tab;
	static GUIStyle tabSelected;

	static GUIStyle activeToolName;
	static GUIStyle resolutionPopup;
	static GUIStyle antialiasingPopup;
	static GUIStyle maskExportPopup;
	static GUIStyle aoModePopup;

	static GUIStyle buttonStyleMiniText;
	static GUIStyle buttonStyleMiniTextLite;

	static GUIStyle polyLassoButtonStyle;

	static GUIStyle snapButton;
	static GUIStyle snapButtonActive;
	static GUIStyle snapButtonTransparent;
	static GUIStyle snapToggleButton;
	static GUIStyle snapToggleButtonActive;

	static GUIStyle groupToolsButton;
	static GUIStyle lastActionLogStyle;

	static GUIStyle toggleButton;
	static GUIStyle toggleButtonActive;
	static GUIStyle smallToggleButton;
	static GUIStyle smallToggleButtonActive;

	static GUIStyle smallToggleButtonSymmX;
	static GUIStyle smallToggleButtonSymmXActive;

	static GUIStyle smallToggleButtonSymmZ;
	static GUIStyle smallToggleButtonSymmZActive;

	static GUIStyle smallToggleButtonSymmDiagonal;
	static GUIStyle smallToggleButtonSymmDiagonalActive;

	static GUIStyle composerMaterialDrag;

	static GUIStyle texturePreviewGuiLabel;

	static GUIStyle layerHeightButtonStyle;
	static GUIStyle layerHeightStyle;
	static GUIStyle logoBackgroundStyle;
	static GUIStyle logoBackgroundStyleLite;
	static GUIStyle versionGuiStyle;
	static GUIStyle versionGuiStyleLite;
	static GUIStyle startScreenButton;

	static GUIStyle logoGuiStyle;
	
	static GUIStyle guiButtonStyle;
	static GUIStyle guiTextStyle;

	static GUIStyle texturePreviewInfoStyle;
	static GUIStyle reloadMaterialsButtonStyle;

	static Texture2D logoBackgroundTexture;
	static Texture2D logoBackgroundTextureLite;

	static Texture surforgeLogo;
	static Texture surforgeLogoLite;

	static Texture startScreenNewTexture;
	static Texture startScreenUserGuide;
	static Texture startScreenVideos;
	static Texture startScreenAbout;

	static Texture startScreenNewTextureLite;
	static Texture startScreenUserGuideLite;
	static Texture startScreenVideosLite;
	static Texture startScreenAboutLite;

	static Texture menuHover;
	static Texture menuHoverLite;

	static Texture glossinessChart;
	static Texture specularChart;
	static Texture albedoChart;

	static Texture polygonLassoIcon;
	
	static Texture limitsIcon;
	
	static Texture decalsIcon;
	static Texture deformIcon;
	static Texture materialsIcon;
	static Texture renderIcon;
	static Texture placeIcon;
	static Texture shatterIcon;
	
	static Texture gridIcon;
	static Texture uvsIcon;
	static Texture uvGridIcon;
	static Texture lastActionIcon;
	static Texture symAxisIcon;

	static Texture newMaterialIcon;
	static Texture copyMaterialIcon;
	static Texture swapMaterialsIcon;
	static Texture deleteMaterialIcon;
	static Texture randomizeMaterialIcon;
	static Texture saveMaterialPresetIcon;
	
	static Texture snapIcon;
	static Texture snapActiveIcon;
	static Texture snapTransparentIcon;
	static Texture snapToggleActiveIcon;
	static Texture snapToggleIcon;

	static Texture randomGroupsIcon;
	static Texture similarGroupsIcon;
	static Texture groupsShiftIcon;
	static Texture randomHeightIcon;
	static Texture similarHeightIcon;

	static Texture upperLayerIcon;
	static Texture bottomLayerIcon;

	static Texture polyLassoButtonActive;
	static Texture materialButtonActive;
	static Texture2D polyLassoButtonBack;
	static Texture2D polyLassoButtonClick;

	public static Vector2 polyLassoScrollPosition;
	public static Vector2 placeToolScrollPosition;
	public static Vector2 greeblesToolScrollPosition;
	public static Vector2 skyboxScrollPosition;
	public static Vector2 materialScrollPosition;
	
	static int oldSelectedContentPackNum = 0;
	
	static Texture[] polyLassoProfileButtons;
	static int selectedPolyLassoProfileNum = 0;
	static int oldSelectedPolyLassoProfileNum = 0;

	static Texture[] placeToolProfileButtons;
	static int selectedPlaceToolProfileNum = 0;
	static int oldSelectedPlaceToolProfileNum = 0;

	static Texture[] skyboxButtons;
	static int selectedSkyboxNum = 0;
	
	static Texture[] decalButtons;
	static int selectedDecalNum = 0;
	
	static Texture[] noiseButtons;
	static int selectedNoiseNum = 0;

	static Texture[] shatterButtons;
	static int selectedShatterNum = 0;
	

	
	static int activeTool = 5;
	
	string[] gpuRenderResolutions = new string[3] {"1024", "2048", "4096" };

	string[] supersamplingModes = new string[3] {"1x", "2x", "4x" };

	string[] maskExportModes = new string[3] {"off", "ID map", "separate"};

	string[] aoModes = new string[2] {"hemisphere", "ssao"};
	string[] aoModesNoSsao = new string[1] {"hemisphere"};
	
	static SurforgeInterface window;
	
	
	//material editor
	static MaterialEditor materialEditor; 
	static Mesh lastPreviewGameObjectMesh;
	
	public static Vector2 scrollPosition;
	
	static int windowMinSizePlusOne; 
	
	static bool isMaterialRenamingNow;
	static Rect renamingMaterialRect;

	static bool isMaterialPanelSideLayout;

	static int snapState = 0;

	static bool isScrollNeedUpdate;

	static string textHeaderColorDark = "#d2d2d2ff";
	static string textHeaderColorLite = "#000000ff";

	static string textHotkeyColorDark = "#ffa500ff";
	static string textHotkeyColorLite = "#005cb2ff"; 

	static string textHotkeyColor2Dark = "#ffda82ff";
	static string textHotkeyColor2Lite = "#007f60ff";

	bool showAbout;

	static float polyLassoScale = 1.0f;


	
	[MenuItem("Window/" + "Surforge")]
	public static void Init() {
		SetupWindow();
	}

	static void SetupWindow() {
		// Init() not runs if window not closed before Unity exit. 
		// Texture type kept with window, Texture2D - not (check for reload in other place)

		window = GetWindow<SurforgeInterface>();
		
		window.titleContent.text = "Surforge";
		
		window.minSize = new Vector2(124, 450);

		logoBackgroundTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/logoBackground.psd", typeof(Texture2D));
		logoBackgroundTextureLite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/logoBackgroundLite.psd", typeof(Texture2D));

		surforgeLogo = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/surforgeLogo.png", typeof(Texture));
		surforgeLogoLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/surforgeLogoLite.png", typeof(Texture));

		startScreenNewTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/newTexture.psd", typeof(Texture));
		startScreenUserGuide = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/userGuide.psd", typeof(Texture));
		startScreenVideos = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/videos.psd", typeof(Texture));
		startScreenAbout = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/about.psd", typeof(Texture));

		startScreenNewTextureLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/newTextureLite.psd", typeof(Texture));
		startScreenUserGuideLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/userGuideLite.psd", typeof(Texture));
		startScreenVideosLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/videosLite.psd", typeof(Texture));
		startScreenAboutLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/aboutLite.psd", typeof(Texture));

		menuHover = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/menuHover.psd", typeof(Texture)); 
		menuHoverLite = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/menuHoverLite.psd", typeof(Texture)); 

		glossinessChart = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/glossinessChart.psd", typeof(Texture)); 
		specularChart = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/specularChart.psd", typeof(Texture));
		albedoChart = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/albedoChart.psd", typeof(Texture));
	
		
		windowMinSizePlusOne = (int)(window.minSize.x) + 1;
		windowSizeX = window.position.size.x - windowMinSizePlusOne;
	}


	void RepaintWindowOnUndoRedo() {
		window.Repaint();
	}


	static bool isSurforgeDelegateRunning = false;

	public void Update() {

		if (!isSurforgeDelegateRunning) {

			isSurforgeDelegateRunning = true;

			SceneView.onSceneGUIDelegate -= SurforgeOnScene;
			SceneView.onSceneGUIDelegate += SurforgeOnScene;

			Undo.undoRedoPerformed -= RepaintWindowOnUndoRedo;
			Undo.undoRedoPerformed += RepaintWindowOnUndoRedo;

			SetupWindow();

			if (Surforge.surforgeSettings) {
				StartSurforge();
			}
		}
	}

	static void SurforgeNewScene() {
		 
		if (Surforge.surforgeSettings == null) {
			Surforge.VoxelEngineActivate();
			shaderMode = 0; 
		}
		StartSurforge();
	}


	static void StartSurforge() {
		Surforge.DeactivateLimitsTool();
		Surforge.DeactivatePolygonLassoTool();
		Surforge.DeactivatePlaceTool();

		LoadGuiIcons();
		
		Surforge.ToggleEditorGrid(false);
		//CenterEditorViewOnGrid();
		
		Surforge.ActivateTexturePreview();
		
		showOctreeGrid = Surforge.surforgeSettings.showGrid;
		
		showUVs = Surforge.surforgeSettings.showUvs;
		
		showUVsHelperGrid = Surforge.surforgeSettings.showUvGrid;

		Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber];
		Surforge.surforgeSettings.extentTexturePreview.composer.RelinkMaps();
		if (materialEditor != null) DestroyImmediate (materialEditor);
		materialEditor = (MaterialEditor)Editor.CreateEditor (Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial);
		
		previewRect = new Rect(windowMinSizePlusOne, -1, windowSizeX+2, windowSizeX+2);

		SetSkybox();

		modelSize = 1.0f;
		modelSizeViewSensitivity = 1.0f;
		modelRenderBoundsCenter = new Vector3(0, 45.0f, 84.0f);
		oldMesh = null;

		ResetPreviewCameraPosition();
		UpdatePreview();
		
		if (window) window.Repaint();
	}


	static void SetSkybox() {
		Material skyboxCopy = Material.Instantiate(Surforge.surforgeSettings.skyboxes[Surforge.surforgeSettings.activeSkybox].skybox);
		RenderSettings.skybox = skyboxCopy;
		Skybox cameraBackSkybox = (Skybox)Surforge.surforgeSettings.extentTexturePreview.previewCamera.GetComponent<Skybox>();
		cameraBackSkybox.material = Material.Instantiate(Surforge.surforgeSettings.skyboxes[Surforge.surforgeSettings.activeSkybox].skybox_blur);
		DynamicGI.UpdateEnvironment();
		Surforge.surforgeSettings.skyboxNeedWindowUpdate = true;
		Surforge.surforgeSettings.skyboxWindowRepaintTimer = Time.realtimeSinceStartup + 2.0f; 
	}

	
	static void ResetPreviewCameraPosition() {
		Surforge.surforgeSettings.extentTexturePreview.transform.eulerAngles = new Vector3(0,0,0);

		UpdateCameraFocusSettings();

		Vector3 angles = Surforge.surforgeSettings.extentTexturePreview.composer.transform.eulerAngles;
		x = angles.y - 180.0f;
		y = angles.x;
		
		mouseScrollWheel = new Vector2();
		mouseDelta = new Vector2();
		
		distance = Mathf.Abs( modelSize * 0.5f / Mathf.Tan(30.0f * 0.5f * Mathf.Deg2Rad)) + 0.58f * modelSizeViewSensitivity;

		Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.localPosition = Surforge.surforgeSettings.extentTexturePreview.transform.InverseTransformPoint(modelRenderBoundsCenter);


	}
	
	static void CenterEditorViewOnGrid() {
		var sceneViews = UnityEditor.SceneView.sceneViews;
		if(sceneViews.Count == 0) return;
		
		for (int i=0; i < sceneViews.Count; i++) {
			UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[i];
			if(sceneView != null) {
				Quaternion rotation = Quaternion.identity;
				rotation.eulerAngles = new Vector3(70.0f, 0, 0);
				sceneView.LookAtDirect(new Vector3(0,0,0), rotation, 60.0f);	
			}
		}
	}

	static void RepaintSceneView() {
		var sceneViews = UnityEditor.SceneView.sceneViews;
		if(sceneViews.Count == 0) return;
		
		for (int i=0; i < sceneViews.Count; i++) {
			UnityEditor.SceneView sceneView = (UnityEditor.SceneView) sceneViews[i];
			if(sceneView != null) {
				sceneView.Repaint();
			}
		}
	}
	
	
	static void LoadGuiIcons() {
		string iconsPath = "Assets/Surforge/Editor/Interface/Gui/IconsLite/";
		if (EditorGUIUtility.isProSkin) {
			iconsPath = "Assets/Surforge/Editor/Interface/Gui/IconsDark/";
		}

		polygonLassoIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "polygonLasso.psd", typeof(Texture));
		limitsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "limits.psd", typeof(Texture));
		
		decalsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "decals.psd", typeof(Texture));
		deformIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "deform.psd", typeof(Texture));
		materialsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "materials.psd", typeof(Texture));
		renderIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "render.psd", typeof(Texture));
		placeIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "place.psd", typeof(Texture));
		shatterIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "shatter.psd", typeof(Texture));
		
		gridIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "grid.psd", typeof(Texture));
		uvsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "uvs.psd", typeof(Texture));
		uvGridIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "uvGrid.psd", typeof(Texture));
		lastActionIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "lastAction.psd", typeof(Texture));
		symAxisIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "symAxis.psd", typeof(Texture));

		newMaterialIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "newMaterial.psd", typeof(Texture));
		copyMaterialIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "copyMaterial.psd", typeof(Texture));
		swapMaterialsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "swapMaterials.psd", typeof(Texture));
		deleteMaterialIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "deleteMaterial.psd", typeof(Texture));
		randomizeMaterialIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "randomizeMaterial.psd", typeof(Texture));
		saveMaterialPresetIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "saveMaterialPreset.psd", typeof(Texture));

		snapIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "snap.psd", typeof(Texture));
		snapActiveIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "snapActive.psd", typeof(Texture));
		snapTransparentIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "snapTransparent.psd", typeof(Texture));
		snapToggleActiveIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "snapToggleActive.psd", typeof(Texture));
		snapToggleIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "snapToggle.psd", typeof(Texture));

		upperLayerIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "upperLayer.psd", typeof(Texture));
		bottomLayerIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "bottomLayer.psd", typeof(Texture));

		polyLassoButtonActive = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "polyLassoButtonActive.psd", typeof(Texture));
		polyLassoButtonBack = (Texture2D)AssetDatabase.LoadAssetAtPath(iconsPath + "polyLassoButtonBack.psd", typeof(Texture2D));
		polyLassoButtonClick = (Texture2D)AssetDatabase.LoadAssetAtPath(iconsPath + "polyLassoButtonClick.psd", typeof(Texture2D));
		materialButtonActive = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "materialButtonActive.psd", typeof(Texture));

		randomGroupsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "randomGroups.psd", typeof(Texture));
		similarGroupsIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "similarGroups.psd", typeof(Texture));
		groupsShiftIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "groupsShift.psd", typeof(Texture));
		randomHeightIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "randomHeight.psd", typeof(Texture));
		similarHeightIcon = (Texture)AssetDatabase.LoadAssetAtPath(iconsPath + "similarHeight.psd", typeof(Texture));
	}
	
	static void PrepareContentPackButtons() {
		if (contentPackButtons == null) {
			contentPackButtons = new Texture[Surforge.surforgeSettings.contentPacks.Length];
			for (int i = 0; i < Surforge.surforgeSettings.contentPacks.Length; i++) {
				contentPackButtons[i] = Surforge.surforgeSettings.contentPacks[i].texture;
			}
		}
	}
	
	
	static void PrepareSetButtons() {
		setButtons = new Texture[Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets.Length];
		for (int i = 0; i < Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets.Length; i++) {
			if (EditorGUIUtility.isProSkin) {
				setButtons[i] = Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[i].texture;
			}
			else {
				if (Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[i].textureLite != null) {
					setButtons[i] = Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[i].textureLite;
				}
				else {
					setButtons[i] = Surforge.surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[i].texture;
				}
			}
		}
	}
	
	static void PreparePolyLassoProfileButtons() {
		polyLassoProfileButtons = new Texture[Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles.Length];
		for (int i=0; i< Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles.Length; i++) {
			if (EditorGUIUtility.isProSkin) {
				polyLassoProfileButtons[i] = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[i].icon;
			}
			else {
				if (Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[i].iconLite != null) {
					polyLassoProfileButtons[i] = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[i].iconLite;
				}
				else {
					polyLassoProfileButtons[i] = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[i].icon;
				}
			}
		}
	}

	static void PreparePlaceToolProfileButtons() {
		placeToolProfileButtons = new Texture[Surforge.surforgeSettings.placeMeshes.placeMeshes.Length];
		for (int i=0; i< Surforge.surforgeSettings.placeMeshes.placeMeshes.Length; i++) {
			if (EditorGUIUtility.isProSkin) {
				placeToolProfileButtons[i] = Surforge.surforgeSettings.placeMeshes.placeMeshes[i].icon;
			}
			else {
				if (Surforge.surforgeSettings.placeMeshes.placeMeshes[i].iconLite != null) {
					placeToolProfileButtons[i] = Surforge.surforgeSettings.placeMeshes.placeMeshes[i].iconLite;
				}
				else {
					placeToolProfileButtons[i] = Surforge.surforgeSettings.placeMeshes.placeMeshes[i].icon;
				}
			}
		}
	}

	static void PrepareSkyboxButtons() {
		skyboxButtons = new Texture[Surforge.surforgeSettings.skyboxes.Length];
		for (int i=0; i < Surforge.surforgeSettings.skyboxes.Length; i++) {
			if (EditorGUIUtility.isProSkin) {
				skyboxButtons[i] = Surforge.surforgeSettings.skyboxes[i].icon;
			}
			else {
				if (Surforge.surforgeSettings.skyboxes[i].iconLite != null) {
					skyboxButtons[i] = Surforge.surforgeSettings.skyboxes[i].iconLite;
				}
				else {
					skyboxButtons[i] = Surforge.surforgeSettings.skyboxes[i].icon;
				}
			}
		}
	}

	static void PrepareMaterialButtons() {
		//return;
		//Debug.Log ("prepare material icons");

		if (Surforge.surforgeSettings.materialButtons == null) {
			Surforge.surforgeSettings.materialButtons = new Texture2D[Surforge.surforgeSettings.loadedMaterials.Length];
		}
		else {
			if (Surforge.surforgeSettings.materialButtons.Length != Surforge.surforgeSettings.loadedMaterials.Length) {
				Surforge.surforgeSettings.materialButtons = new Texture2D[Surforge.surforgeSettings.loadedMaterials.Length];
			}
		}


		for (int i=0; i < Surforge.surforgeSettings.loadedMaterials.Length; i++) {
			RenderMaterialIcon(Surforge.surforgeSettings.loadedMaterials[i], 164, i);
		}
		Surforge.surforgeSettings.materialIconsRenderedWithSkybox = Surforge.surforgeSettings.activeSkybox;
	}


	static RenderTexture materialPreviewRenderTexture;  
	static Camera renderMaterialIconCamera;
	static GameObject renderMaterialIcon;

	static void RenderMaterialIcon(Material material, int iconRes, int materialButtonIndex) {

		if (EditorGUIUtility.isProSkin) { 
			renderMaterialIconCamera = (Camera)Instantiate(Surforge.surforgeSettings.renderMaterialIconCameraPrefab);
		}
		else {
			renderMaterialIconCamera = (Camera)Instantiate(Surforge.surforgeSettings.renderMaterialIconCameraLitePrefab);
		}

		renderMaterialIcon = (GameObject)Instantiate(Surforge.surforgeSettings.renderMaterialIconPrefab.gameObject);

		renderMaterialIcon.transform.position = new Vector3(0,0, 6.15f);
		Renderer r =  (Renderer)renderMaterialIcon.GetComponent<Renderer>();
		if (r) {
			r.sharedMaterial = Material.Instantiate(material);
			r.sharedMaterial.SetTexture("_RGBAnoise", Surforge.surforgeSettings.rgbaNoise);
			r.sharedMaterial.SetTexture("_NormalMap", Surforge.surforgeSettings.renderMaterialIconNormal);
			r.sharedMaterial.SetTexture("_AoEdgesDirtDepth", Surforge.surforgeSettings.renderMaterialIconAoEdgesDirtDepth);
			if (Surforge.surforgeSettings.renderMaterialIconNoise != null) {
				r.sharedMaterial.SetTexture("_EmissionMask", Surforge.surforgeSettings.renderMaterialIconNoise);
			}

			if (PlayerSettings.colorSpace == ColorSpace.Linear) {
				r.sharedMaterial.SetInt("_LinearColorSpace", 1);
			}
			else {
				r.sharedMaterial.SetInt("_LinearColorSpace", 0);
			}
		}

		if (materialPreviewRenderTexture == null) materialPreviewRenderTexture = new RenderTexture(iconRes, iconRes, 24);
		
		renderMaterialIconCamera.targetTexture = materialPreviewRenderTexture;
		
		renderMaterialIconCamera.SetReplacementShader(Surforge.surforgeSettings.renderMaterialIconShader, "RenderMaterialIcon");  
		renderMaterialIconCamera.Render();
		
		RenderTexture.active = materialPreviewRenderTexture;


		if (Surforge.surforgeSettings.materialButtons[materialButtonIndex] == null) {
			Surforge.surforgeSettings.materialButtons[materialButtonIndex] = new Texture2D(iconRes, iconRes);
		}

		Surforge.surforgeSettings.materialButtons[materialButtonIndex].ReadPixels(new Rect(0, 0, materialPreviewRenderTexture.width, materialPreviewRenderTexture.height), 0, 0);
		Surforge.surforgeSettings.materialButtons[materialButtonIndex].Apply();

		//material icons not saving with the scene
		Surforge.surforgeSettings.materialButtons[materialButtonIndex].hideFlags = HideFlags.DontSaveInEditor;


		renderMaterialIconCamera.targetTexture = null;
		RenderTexture.active = null;
		if (renderMaterialIconCamera != null) DestroyImmediate(renderMaterialIconCamera.gameObject);
		if (r != null) {
			if (r.sharedMaterial != null) {
				DestroyImmediate(r.sharedMaterial);
			}
			DestroyImmediate(r);
		}
		if (renderMaterialIcon != null) DestroyImmediate(renderMaterialIcon.gameObject);

	}


	
	static void PrepareDecalButtons() {
		decalButtons = new Texture[Surforge.surforgeSettings.decalSets.decalSets.Length];
		for (int i = 0; i < Surforge.surforgeSettings.decalSets.decalSets.Length; i++) {
			decalButtons[i] = Surforge.surforgeSettings.decalSets.decalSets[i].icon;
		}
	}
	
	static void PrepareNoiseButtons() {
		noiseButtons = new Texture[Surforge.surforgeSettings.noisePresets.noisePresets.Length];
		for (int i = 0; i < Surforge.surforgeSettings.noisePresets.noisePresets.Length; i++) {
			noiseButtons[i] = Surforge.surforgeSettings.noisePresets.noisePresets[i].icon;
		}
	}

	static void PrepareShatterButtons() {
		shatterButtons = new Texture[Surforge.surforgeSettings.shatterPresets.shatterPresets.Length];
		for (int i = 0; i < Surforge.surforgeSettings.shatterPresets.shatterPresets.Length; i++) {
			shatterButtons[i] = Surforge.surforgeSettings.shatterPresets.shatterPresets[i].icon;
		}
	}
	
	
	
	void CreateGuiSkin() {
		GUI.skin.GetStyle("Tooltip").richText = true;



		//------ defaults hardcoded --- third party fix empty guistyle
		guiButtonStyle = new GUIStyle(GUI.skin.button);
		guiButtonStyle.fontSize = 0;
		guiButtonStyle.alignment = TextAnchor.MiddleCenter;
		guiButtonStyle.fixedWidth = 0;
		guiButtonStyle.fixedHeight = 0;
		guiButtonStyle.margin = new RectOffset(4,4,3,3);
		guiButtonStyle.padding.top = 2;
		guiButtonStyle.padding.bottom = 3;
		guiButtonStyle.padding.left = 6;
		guiButtonStyle.padding.right = 6;
		

		guiTextStyle =  new GUIStyle(GUI.skin.label);
		guiTextStyle.fontSize = 0;
		guiTextStyle.alignment = TextAnchor.UpperLeft;
		guiTextStyle.fixedWidth = 0;
		guiTextStyle.fixedHeight = 0;
		guiTextStyle.margin = new RectOffset(4,4,2,2);
		guiTextStyle.padding.top = 1;
		guiTextStyle.padding.bottom = 2;
		guiTextStyle.padding.left = 2;
		guiTextStyle.padding.right = 2;
		//-----------


		buttonStyle = new GUIStyle(guiButtonStyle);
		buttonStyle.alignment = TextAnchor.MiddleLeft;
		buttonStyle.fixedWidth = 36;
		buttonStyle.fixedHeight = 36;
		
		buttonStyleActive = new GUIStyle(buttonStyle);
		buttonStyleActive.normal = buttonStyleActive.active;
		
		layerButtonStyle = new GUIStyle(guiButtonStyle);
		layerButtonStyle.alignment = TextAnchor.MiddleLeft;
		
		contentPackButtonStyle = new GUIStyle(guiButtonStyle);
		contentPackButtonStyle.alignment = TextAnchor.MiddleLeft;
		
		buttonStyleMini = new GUIStyle(buttonStyle);
		buttonStyleMini.alignment = TextAnchor.MiddleLeft;
		buttonStyleMini.fixedWidth = 19;
		buttonStyleMini.fixedHeight = 19;
		buttonStyleMini.margin = new RectOffset(0,0,0,0);
		
		buttonStyleMiniSelected = new GUIStyle(buttonStyleMini);
		buttonStyleMiniSelected.normal = buttonStyleMiniSelected.active;

		//tabs
		tab = new GUIStyle(buttonStyleMini);
		tab.padding.top = 5;
		tab.padding.bottom = 5;
		tab.padding.left = 5;
		tab.padding.right = 5;

		tabSelected = new GUIStyle(tab);
		tabSelected.normal = tabSelected.active;


		buttonStyleMiniText = new GUIStyle(buttonStyleMini);
		buttonStyleMiniText.fixedWidth = 70;
		Color orangeColor = new Color(2, 0.6f, 0, 1);
		buttonStyleMiniText.normal.textColor = orangeColor;
		buttonStyleMiniText.active.textColor = orangeColor;
		buttonStyleMiniText.hover.textColor = orangeColor;
		buttonStyleMiniText.focused.textColor = orangeColor;

		buttonStyleMiniTextLite = new GUIStyle(buttonStyleMiniText);
		Color blueColor = new Color(0, 0.36f, 0.698f, 1);
		buttonStyleMiniTextLite.normal.textColor = blueColor;
		buttonStyleMiniTextLite.active.textColor = blueColor;
		buttonStyleMiniTextLite.hover.textColor = blueColor;
		buttonStyleMiniTextLite.focused.textColor = blueColor;

		
		activeToolName = new GUIStyle(guiTextStyle);
		activeToolName.alignment = TextAnchor.MiddleLeft;
		activeToolName.fixedHeight = 36;
		
		resolutionPopup = new GUIStyle(GUI.skin.FindStyle("popup"));
		resolutionPopup.fixedWidth = 80;

		antialiasingPopup = new GUIStyle(GUI.skin.FindStyle("popup"));
		antialiasingPopup.fixedWidth = 36;

		maskExportPopup = new GUIStyle(GUI.skin.FindStyle("popup"));
		maskExportPopup.fixedWidth = 68;

		aoModePopup = new GUIStyle(GUI.skin.FindStyle("popup"));
		aoModePopup.fixedWidth = 48;

		snapButton = new GUIStyle(buttonStyleMini);
		snapButton.normal.background = null;
		snapButton.fixedWidth = 21;
		snapButton.fixedHeight = 21;

		snapButtonActive = new GUIStyle(buttonStyleMini);
		snapButtonActive.normal.background = null;
		snapButtonActive.fixedWidth = 21;
		snapButtonActive.fixedHeight = 21;

		snapButtonTransparent = new GUIStyle(buttonStyleMini);
		snapButtonTransparent.normal.background = null;
		snapButtonTransparent.fixedWidth = 21;
		snapButtonTransparent.fixedHeight = 21;

		composerMaterialDrag = new GUIStyle(buttonStyleMini);
		composerMaterialDrag.normal.background = null;
		composerMaterialDrag.fixedWidth = 20;
		composerMaterialDrag.fixedHeight = 20;
		composerMaterialDrag.padding.top = 0;
		composerMaterialDrag.padding.bottom = 0;
		composerMaterialDrag.padding.left = 5;
		composerMaterialDrag.padding.right = 0;

		texturePreviewGuiLabel = new GUIStyle();
	
		snapToggleButton = new GUIStyle(buttonStyle);
		snapToggleButton.fixedWidth = 28;
		snapToggleButton.fixedHeight = 28;


		snapToggleButtonActive = new GUIStyle(buttonStyle);
		snapToggleButtonActive.normal = snapToggleButtonActive.active;
		snapToggleButtonActive.fixedWidth = 28;
		snapToggleButtonActive.fixedHeight = 28;

		layerHeightButtonStyle = new GUIStyle(buttonStyle);
		layerHeightButtonStyle.normal.background = null;
		layerHeightButtonStyle.fixedWidth = 21;
		layerHeightButtonStyle.fixedHeight = 21;

		layerHeightStyle = new GUIStyle(EditorStyles.label);
		layerHeightStyle.font = EditorStyles.boldFont;
		layerHeightStyle.alignment = TextAnchor.MiddleCenter;

		//poly lasso button style
		polyLassoButtonStyle = new GUIStyle(); 
		polyLassoButtonStyle.fixedWidth = 41;
		polyLassoButtonStyle.fixedHeight = 41;
		polyLassoButtonStyle.normal.background = polyLassoButtonBack;

		polyLassoButtonStyle.active.background = polyLassoButtonClick;

		//symmetry button styles
		toggleButton = new GUIStyle(guiButtonStyle);
		toggleButton.richText = true;

		toggleButtonActive = new GUIStyle(toggleButton);
		toggleButtonActive.normal = toggleButtonActive.active;

		smallToggleButton = new GUIStyle(guiButtonStyle);

		smallToggleButtonActive = new GUIStyle(smallToggleButton);
		smallToggleButtonActive.normal = smallToggleButtonActive.active;

		Color textColorSymmX = new Color(1, 0.4f, 0.4f, 1);
		smallToggleButtonSymmX = new GUIStyle(smallToggleButton);
		smallToggleButtonSymmX.normal.textColor = textColorSymmX;
		smallToggleButtonSymmX.active.textColor = textColorSymmX;
		smallToggleButtonSymmXActive = new GUIStyle(smallToggleButtonActive);
		smallToggleButtonSymmXActive.normal.textColor = textColorSymmX;
		smallToggleButtonSymmXActive.active.textColor = textColorSymmX;

		Color textColorSymmZ = new Color(0.25f, 0.65f, 1, 1);
		smallToggleButtonSymmZ = new GUIStyle(smallToggleButton);
		smallToggleButtonSymmZ.normal.textColor = textColorSymmZ;
		smallToggleButtonSymmZ.active.textColor = textColorSymmZ;
		smallToggleButtonSymmZActive = new GUIStyle(smallToggleButtonActive);
		smallToggleButtonSymmZActive.normal.textColor = textColorSymmZ;
		smallToggleButtonSymmZActive.active.textColor = textColorSymmZ;

		Color textColorSymmDiagonal = new Color(1, 0.4f, 1, 1);
		smallToggleButtonSymmDiagonal = new GUIStyle(smallToggleButton);
		smallToggleButtonSymmDiagonal.normal.textColor = textColorSymmDiagonal;
		smallToggleButtonSymmDiagonal.active.textColor = textColorSymmDiagonal;
		smallToggleButtonSymmDiagonalActive = new GUIStyle(smallToggleButtonActive);
		smallToggleButtonSymmDiagonalActive.normal.textColor = textColorSymmDiagonal;
		smallToggleButtonSymmDiagonalActive.active.textColor = textColorSymmDiagonal;


		groupToolsButton = new GUIStyle(buttonStyleMini);
		groupToolsButton.normal.background = null;
		groupToolsButton.fixedWidth = 25;
		groupToolsButton.fixedHeight = 25;

		lastActionLogStyle = new GUIStyle();
		lastActionLogStyle.richText = true;
		lastActionLogStyle.alignment = TextAnchor.MiddleCenter;


		texturePreviewInfoStyle = new GUIStyle(guiTextStyle);
		texturePreviewInfoStyle.normal.textColor = new Color(1, 1, 1, 0.75f);

		reloadMaterialsButtonStyle = new GUIStyle(guiButtonStyle);
	}
	
	
	static float togglersPosY;
	static float windowSizeX;

	static bool isDragStartedOnPreview;

	static double clickTime;
	static double doubleClickTime = 0.5;
	static Vector2 doubleClockMousePos;

	void CheckForPreviewControls() { 
		if ( Event.current.type == EventType.MouseDown) {
			Vector2 mousePos = Event.current.mousePosition;
			if ((mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax)) {
				isDragStartedOnPreview = true;
			}
			else {
				isDragStartedOnPreview = false;
			}
		}

		
		if ( (Event.current.type == EventType.ScrollWheel) && (!Event.current.control) && (!Event.current.shift) ) {
			Vector2 mousePos = Event.current.mousePosition;
			if ((mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax)) {
				mouseScrollWheel = Event.current.delta;
			}
		}
		else {
			mouseScrollWheel = Vector2.zero;
		}
		
		if ( (Event.current.type == EventType.MouseDrag) && ( (Event.current.button == 0) || ((Event.current.button == 1) && (!Event.current.alt)) ) ) {
			if (isDragStartedOnPreview) {
				mouseDelta = Event.current.delta;
			}
			else {
				mouseDelta = Vector2.zero;
			}
		}
		else {
			mouseDelta = Vector2.zero;
		}

		if ( (Event.current.type == EventType.MouseDrag) && (Event.current.button == 1) && (Event.current.alt) ) {
			if (isDragStartedOnPreview) {
				mouseScrollWheel = new Vector2(Event.current.delta.x * 0.25f * -1, Event.current.delta.y * 0.25f * -1);
			}
			else {
				mouseScrollWheel = Vector2.zero;
			}
		}

		if ( (Event.current.type == EventType.MouseDrag) && (Event.current.button == 2) ) {
			if (isDragStartedOnPreview) {
				Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.LookAt(Surforge.surforgeSettings.extentTexturePreview.previewCamera.transform);

				Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.position += 
					Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.right * Event.current.delta.x * 0.005f * modelSizeViewSensitivity;
				Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.position += 
					Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus.up * Event.current.delta.y * 0.005f * modelSizeViewSensitivity;
			}
		}


		if (( Event.current.type == EventType.MouseDown)  && (Event.current.button == 0) && (Event.current.delta == Vector2.zero)) {
			Vector2 mousePos = Event.current.mousePosition;
			if ((mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax)) {

				if ((EditorApplication.timeSinceStartup - clickTime) < doubleClickTime) {
					if (doubleClockMousePos == Event.current.mousePosition) {
						ResetPreviewCameraPosition();
						UpdatePreview();
					}
				}

			}
		}


		if ( Event.current.type == EventType.MouseUp) {
			isDragStartedOnPreview = false;
		}

		if ( Event.current.type == EventType.MouseDown) {
			clickTime = EditorApplication.timeSinceStartup;
			doubleClockMousePos = Event.current.mousePosition;
		}

	}

	static void DrawStartMenuHover(int buttonNum) {
		if (buttonNum == 0) return;
		float startMenuHeightOffset = 88.0f;
		Rect hoverRect = new Rect (0, startMenuHeightOffset + (buttonNum - 1) * 57, 253, 57);
		if (EditorGUIUtility.isProSkin) {
			GUI.Label(hoverRect, menuHover, logoGuiStyle);
		}
		else {
			GUI.Label(hoverRect, menuHoverLite, logoGuiStyle);
		}
	}

	static void DrawPolyLassoButtonHighlight(float offsetY, int profileNum, bool isHighlightMaterial) {
		int columnCounter = -1;
		int rowCounter = 0;

		float stepX = 41;
		float stepY = 41;


		for (int i=0; i < profileNum + 1; i++) {
			if (columnCounter == 2) {
				columnCounter = -1;
				rowCounter++;
			}
			columnCounter++;
		}

		if (!isHighlightMaterial) GUI.Label(new Rect (stepX * columnCounter, stepY * rowCounter + offsetY, 41, 41), polyLassoButtonActive);
		else GUI.Label(new Rect (stepX * columnCounter, stepY * rowCounter + offsetY, 41, 41), materialButtonActive);
	}


	static int dragStartedOverMaterial;
	static float materialDragDropOffsetY = 215;
	static bool materialDragNow;
	static Rect lastHoverMaterialRect;
	static Rect hoverIconRect;
	static Material tempMaterial;


	static void DrawMaterialDragDropIcons(float offsetY) { 
		bool isLinearSpace = false;
		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			isLinearSpace = true;
		}

		#if UNITY_2018
		#else
			if (isLinearSpace) GL.sRGBWrite = true;
		#endif


		GUILayout.BeginVertical();

		int materialIconsDrawn = 0;

		for (int m=0; m < Surforge.surforgeSettings.materialFoldersNames.Length; m++) {

			Surforge.surforgeSettings.materialFoldersFoldout[m] = EditorGUILayout.Foldout(Surforge.surforgeSettings.materialFoldersFoldout[m], Surforge.surforgeSettings.materialFoldersNames[m]);

			for (int a=0; a< Mathf.Ceil((float)Surforge.surforgeSettings.materialFoldersMatCount[m] / 3.0f); a++) {

				GUILayout.BeginHorizontal();

				for (int i=0; i<3; i++) {
					if ((i + a*3) <= (Surforge.surforgeSettings.materialFoldersMatCount[m] - 1)) {

						if (Surforge.surforgeSettings.materialFoldersFoldout[m]) {

							GUILayout.Label(Surforge.surforgeSettings.materialButtons[materialIconsDrawn + i + a*3], polyLassoButtonStyle);

							if (Event.current.type == EventType.Repaint &&
							    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
								lastHoverMaterialRect = GUILayoutUtility.GetLastRect();
							}

							if (!materialDragNow) {
								if (IsMouseOver()) dragStartedOverMaterial = materialIconsDrawn + i + a*3;
							}

						}

					}
				}
				GUILayout.EndHorizontal();  
			}
			materialIconsDrawn = materialIconsDrawn + Surforge.surforgeSettings.materialFoldersMatCount[m];

		}

		GUILayout.EndVertical();

		if (!materialDragNow) {
			hoverIconRect = new Rect(lastHoverMaterialRect.x, lastHoverMaterialRect.y + offsetY - materialScrollPosition.y, 41, 41);
		}

		#if UNITY_2018
		#else
			if (isLinearSpace) GL.sRGBWrite = false;
		#endif


		window.Repaint();
	}


	static void DrawMaterialDragDropLine() {

		Vector2 mouseStartPos = new Vector2(hoverIconRect.x + hoverIconRect.size.x * 0.5f, hoverIconRect.y + hoverIconRect.size.y * 0.5f);
		Vector2 mousePos = Event.current.mousePosition;
		
		Vector3[] dragDropLinePoints = new Vector3[2];
		dragDropLinePoints[0] = new Vector3(mouseStartPos.x, mouseStartPos.y, 0);
		dragDropLinePoints[1] = new Vector3(mousePos.x,mousePos.y,0);
		
		Handles.color = Color.white;
		Handles.DrawAAPolyLine(5.0f, dragDropLinePoints);
	}


	/*
	static void DrawPolyLassoCurveProfile(Vector2 start, Vector2 pos, PolyLassoProfile profile) {
		if (profile.icon == null) {

			float xOffset = 16.0f;
			float yOffset = 17.0f;
			float scale = 25.0f;

			Vector3[] profilePoints = new Vector3[profile.offsets.Length + 1];
			profilePoints[0] = new Vector3(start.x + pos.x, start.y + pos.y + yOffset * 2 + profile.heights[0] * scale * -1, 0);

			for (int i=1; i < profile.offsets.Length + 1; i++) {
				profilePoints[i] = new Vector3(start.x + pos.x + xOffset + profile.offsets[i-1] * scale, start.y + pos.y + yOffset * 2 + profile.heights[i-1] * scale * -1, 0);

				if (profilePoints[i].x > start.x + pos.x + xOffset * 2)  {
					profilePoints[i] = new Vector3 (start.x + pos.x + xOffset * 2, profilePoints[i].y, profilePoints[i].z);
				}
				if (profilePoints[i].y > start.y + pos.y + yOffset * 2)  {
					profilePoints[i] = new Vector3 (profilePoints[i].x, start.y + pos.y + yOffset * 2, profilePoints[i].z);
				}
			}

			Handles.color = new Color(profile.iconColor.r, profile.iconColor.g, profile.iconColor.b, 0.9f); 
			Handles.DrawPolyLine(profilePoints);

		}
	}
	*/


	static void ShowSurforgeAbout() {
		SurforgeAbout aboutWindow = GetWindow<SurforgeAbout>();

		aboutWindow.SetVersion(version);

		aboutWindow.titleContent.text = "About Surforge";
		aboutWindow.minSize = new Vector2(260, 400);
		aboutWindow.maxSize = new Vector2(260, 400);
		

	}


	static void PolyLassoProfileScaleIncrease() {
		polyLassoScale = polyLassoScale + 0.1f;
		if (polyLassoScale > 10.0f) polyLassoScale = 10.0f;
		if (window) window.Repaint();
		Surforge.LogAction("Poly Lasso: scale increase", " ] ", "");
	}

	static void PolyLassoProfileScaleDecrease() {
		polyLassoScale = polyLassoScale - 0.1f;
		if (polyLassoScale < 0.1f) polyLassoScale = 0.1f;
		if (window) window.Repaint();
		Surforge.LogAction("Poly Lasso: scale decrease", " [ ", "");
	}



	static string texturePreviewInfo = "";

	static int shaderMode;
	static Vector2 rightButtonDownPos;


	void OnGUI() {
		string textHeaderColor = textHeaderColorLite;
		string textHotkeyColor = textHotkeyColorLite;
		string textHotkeyColor2 = textHotkeyColor2Lite;
		if (EditorGUIUtility.isProSkin) {
			textHeaderColor = textHeaderColorDark;
			textHotkeyColor = textHotkeyColorDark;
			textHotkeyColor2 = textHotkeyColor2Dark;
		}
		string texturePreviewTooltip = "<b><color="+textHeaderColor+">Texture Preview </color></b>\n\n<color="+textHotkeyColor+">Space</color>: Render texture\n<color="+textHotkeyColor+">Double Click</color>: Reset Camera\n<color="+textHotkeyColor+">Left Button</color>: Rotate \n<color="+textHotkeyColor+">Middle Button</color>: Pan \n<color="+textHotkeyColor+">Scroll</color> or <color="+textHotkeyColor+">Alt + Right Button</color>: Zoom \n<color="+textHotkeyColor+">Shift + Left Button</color>: Rotate skybox \n<color="+textHotkeyColor+">Ctrl</color>: Select material under cursor \n<color="+textHotkeyColor+">Hold Ctrl</color>: Highlight UV island under cursor \n<color="+textHotkeyColor+">Ctrl + c</color>: Copy material under cursor \n<color="+textHotkeyColor+">Ctrl + v</color>: Paste material under cursor \n<color="+textHotkeyColor+">Ctrl + x</color>: Swap material under cursor \nwith last copied\n<color="+textHotkeyColor+">Ctrl + Scroll</color>: Cycle material under cursor \n<color="+textHotkeyColor+">Shift + Scroll</color>: Cycle dirts\n<color="+textHotkeyColor+">Ctrl + Space</color>: Reload mesh\n<color="+textHotkeyColor+">Right Button</color>: Preview maps"; 
		
		string materialSelectionTooltip = "<b><color="+textHeaderColor+">Select Material Set</color></b>\n\nMaterial sets are saved with the scene \n\n<color="+textHotkeyColor+">Left Click</color>: Select material set\n\n<color="+textHotkeyColor+">Scroll</color>: Next / Previous material set \n\n<color="+textHotkeyColor+">Right Click</color>: Rename material set";

		
		GUIContent[] matIDs = new GUIContent[11] {
			new GUIContent("Material 1", "<b><color="+textHeaderColor+">Material 1</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">1</color> in the scene view: Assign material mask 1 to selected objects\n\n<color="+textHotkeyColor+">Shift + 1</color> in the scene view: Assign material mask 1 to selection and similar objects"), 
			new GUIContent("Material 2", "<b><color="+textHeaderColor+">Material 2</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">2</color> in the scene view: Assign material mask 2 to selected objects\n\n<color="+textHotkeyColor+">Shift + 2</color> in the scene view: Assign material mask 2 to selection and similar objects"),
			new GUIContent("Material 3", "<b><color="+textHeaderColor+">Material 3</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">3</color> in the scene view: Assign material mask 3 to selected objects\n\n<color="+textHotkeyColor+">Shift + 3</color> in the scene view: Assign material mask 3 to selection and similar objects"),
			new GUIContent("Material 4", "<b><color="+textHeaderColor+">Material 4</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">4</color> in the scene view: Assign material mask 4 to selected objects\n\n<color="+textHotkeyColor+">Shift + 4</color> in the scene view: Assign material mask 4 to selection and similar objects"),
			new GUIContent("Material 5", "<b><color="+textHeaderColor+">Material 5</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">5</color> in the scene view: Assign material mask 5 to selected objects\n\n<color="+textHotkeyColor+">Shift + 5</color> in the scene view: Assign material mask 5 to selection and similar objects"),
			new GUIContent("Material 6", "<b><color="+textHeaderColor+">Material 6</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">6</color> in the scene view: Assign material mask 6 to selected objects\n\n<color="+textHotkeyColor+">Shift + 6</color> in the scene view: Assign material mask 6 to selection and similar objects"),
			new GUIContent("Material 7", "<b><color="+textHeaderColor+">Material 7</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">7</color> in the scene view: Assign material mask 7 to selected objects\n\n<color="+textHotkeyColor+">Shift + 7</color> in the scene view: Assign material mask 7 to selection and similar objects"),
			new GUIContent("Material 8", "<b><color="+textHeaderColor+">Material 8</color></b>\n\n<color="+textHotkeyColor+">Ctrl</color> on texture preview: Select material under cursor\n\n<color="+textHotkeyColor+">8</color> in the scene view: Assign material mask 8 to selected objects\n\n<color="+textHotkeyColor+">Shift + 8</color> in the scene view: Assign material mask 8 to selection and similar objects"),
			new GUIContent("Dirt", "Edit Dirt of this Material Set"),
			new GUIContent("Emission", "Edit Emission of this Material Set"),
			new GUIContent("Adjust", "Levels, HSBC of this Material Set") 
		};

		GUIContent symmetryGuiContent = new GUIContent("Symm", "<b><color="+textHeaderColor+">Symmetry</color> <color="+textHotkeyColor+">(S)</color></b>");
		GUIContent symmetryGuiContentX = new GUIContent("x", "Symmetry X axis");
		GUIContent symmetryGuiContentZ = new GUIContent("z", "Symmetry Z axis");
		GUIContent symmetryGuiContentDiagonal = new GUIContent("\\", "Diagonal Symmetry ");
		
		GUIContent seamlessGuiContent = new GUIContent("Seamless", "<b><color="+textHeaderColor+">Seamless mode on/off</color> <color="+textHotkeyColor+">(Alt + S)</color></b>\n\nSurrounds the texture area by 8 corresponding instances, making the seamless rendering available");



		if (!Application.isPlaying) {

			if (Surforge.surforgeSettings) {
				Event evt = Event.current;

				isScrollNeedUpdate = false;

				if (!window) {
					window = GetWindow<SurforgeInterface>();
				}

				CheckForGlobalHotkeys();

				previewRect = new Rect(windowMinSizePlusOne, -1, windowSizeX+2, windowSizeX+2);
				if (previewRect.height >= (window.position.size.y + 4)) {
					previewRect = new Rect(windowMinSizePlusOne, -1, window.position.size.y + 4, window.position.size.y + 4);
					isMaterialPanelSideLayout = true;
				}
				else {
					isMaterialPanelSideLayout = false;
				}

			    
				CheckForPreviewControls();
				Vector2 mousePos = Event.current.mousePosition;
				if ( ((mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax)) ||
				    isDragStartedOnPreview ) {
					UpdatePreview();
					window.Repaint();
				} 


				if (window != null) {
					togglersPosY = window.position.size.y;
					windowSizeX = window.position.size.x - windowMinSizePlusOne;
				}
		


				if (Surforge.surforgeSettings.layers != null) {
				
					if (buttonStyle == null) CreateGuiSkin();
				
					if (polyLassoProfileButtons == null) PreparePolyLassoProfileButtons();
					if (placeToolProfileButtons == null) PreparePlaceToolProfileButtons();
					if (decalButtons == null) PrepareDecalButtons();
					if (noiseButtons == null) PrepareNoiseButtons();
					if (skyboxButtons == null) PrepareSkyboxButtons(); 
					if (shatterButtons == null) PrepareShatterButtons();

					bool renderMaterialIconsRequired = false;
					if (Surforge.surforgeSettings.materialButtons == null) {
						renderMaterialIconsRequired = true;
					}
					else {
						if (Surforge.surforgeSettings.materialButtons.Length == 0) renderMaterialIconsRequired = true;
						else {
							if (Surforge.surforgeSettings.materialButtons[0] == null) { //this means we have loaded the saved scene, that contains no saved textures
								renderMaterialIconsRequired = true; 
								activeTool = 5; 
								Surforge.StartGPURender(); 
							}
						}
					}
					if (renderMaterialIconsRequired) {
						PrepareMaterialButtons();
						Surforge.surforgeSettings.materialIconsRenderedWithSkybox = -1;
					}
				
					GUILayout.BeginArea(new Rect(0, 0, 124, togglersPosY));  
				
					GUILayout.BeginHorizontal("box");
					if (activeTool != 0) {
						if(GUILayout.Button(new GUIContent(decalsIcon, "Decals, Deform, Shatter"), tab) ) {
							activeTool = 0;
						}
					}
					else {
						if(GUILayout.Button(new GUIContent(decalsIcon, "Decals, Deform, Shatter"), tabSelected ) ) {
						}
					}
				

					if (activeTool != 1) {
						if(GUILayout.Button(new GUIContent(materialsIcon, "Materials"), tab ) ) {
							activeTool = 1;
							if (Surforge.surforgeSettings) {
								Surforge.LogAction("Materials", "M", "");
								RepaintSceneView();
								if (Surforge.surforgeSettings.materialIconsRenderedWithSkybox != Surforge.surforgeSettings.activeSkybox) {
									PrepareMaterialButtons();
								}
							}
						}
					}
					else {
						if(GUILayout.Button(new GUIContent(materialsIcon, "Materials"), tabSelected ) ) {
						}
					}

				
				
					if (activeTool != 2) {
						if(GUILayout.Button(new GUIContent(limitsIcon, "Greebles"), tab ) ) {
							Surforge.ToggleLimitsToolActive();
							activeTool = 2;
						}
					}
					else {
						if(GUILayout.Button(new GUIContent(limitsIcon, "Greebles"), tabSelected ) ) {
						}
					}
				
				
					if (activeTool != 3) {
						if( GUILayout.Button(new GUIContent(renderIcon, "Render"), tab  ) ) {
							activeTool = 3;
						}
					}
					else {
						if( GUILayout.Button(new GUIContent(renderIcon, "Render"), tabSelected  ) ) {
						}
					}
				
				

					if (activeTool != 4) {
						if( GUILayout.Button(new GUIContent(placeIcon, "Add Detail"), tab  ) ) {
							Surforge.TogglePlaceToolActive();
							placeToolState = 0;

							Surforge.surforgeSettings.activePlaceMesh = oldSelectedPlaceToolProfileNum;
							selectedPlaceToolProfileNum = oldSelectedPlaceToolProfileNum;

							activeTool = 4;
						}
					}
					else {
						if( GUILayout.Button(new GUIContent(placeIcon, "Add Detail"), tabSelected  ) ) {
						}
					}

				
				
				
					if (activeTool != 5) {
						if( GUILayout.Button(new GUIContent(polygonLassoIcon, "Poly Lasso"), tab  ) ) {
							Surforge.TogglePolygonLassoActive();
						
							Surforge.surforgeSettings.activePolyLassoProfile = oldSelectedPolyLassoProfileNum;
							selectedPolyLassoProfileNum = oldSelectedPolyLassoProfileNum;
						
							polygonLassoPoints = new List<Vector3>();
						
							activeTool = 5;
						}
					}
					else {
						if( GUILayout.Button(new GUIContent(polygonLassoIcon, "Poly Lasso"), tabSelected  ) ) {
						}
					}


				
					GUILayout.EndHorizontal();






				
				
				
				
					GUILayout.BeginHorizontal("box");
					
					if (activeTool == 0) {
						Surforge.DeactivateLimitsTool();
						Surforge.DeactivatePolygonLassoTool();
						Surforge.DeactivatePlaceTool();
					
						if(GUILayout.Button(new GUIContent(decalsIcon, "<b><color="+textHeaderColor+">Add Decal</color></b>\n\nAdd decals to selected Poly Lasso objects: tiny geometry arranged along the edges or corners\n\nMore than one preset can be added"), buttonStyle ) ) {
							Surforge.AddDecalToPolyLassoObjects();
							activeTool = 0;
						}
						GUILayout.Label("Decals", activeToolName);
					}
						
	
					if (activeTool == 1) {
						Surforge.DeactivateLimitsTool();
						Surforge.DeactivatePolygonLassoTool();
						Surforge.DeactivatePlaceTool();
						
						if(GUILayout.Button(new GUIContent(materialsIcon, "<b><size=14><color="+textHeaderColor+">Materials</color> <color="+textHotkeyColor+">(M)</color></size></b> \n\nDrag-and-drop materials from the preset list to the preview"), buttonStyleActive ) ) {
							activeTool = 1;
						}
						GUILayout.Label("Materials", activeToolName);
					}

				
					if (activeTool == 2) {
						Surforge.LoadGreebles();
						PrepareContentPackButtons();
						if (setButtons == null) PrepareSetButtons();
						if (Surforge.surforgeSettings.greeblesVoxelized == false) {
							Surforge.surforgeSettings.greeblesVoxelized = true;
							Surforge.PrepareVoxelizedSet();
						}

						Surforge.DeactivatePolygonLassoTool();
						Surforge.DeactivatePlaceTool();
					
						GUIStyle limitsButtonStyle = buttonStyle;
						if (Surforge.surforgeSettings.isLimitsActive) limitsButtonStyle = buttonStyleActive;
						if(GUILayout.Button(new GUIContent(limitsIcon, "<b><size=14><color="+textHeaderColor+">Greebles</color> <color="+textHotkeyColor+">(G)</color></size></b> \n\nGreebles tool is voxel-based, so it tends to form rectangular patterns \n\nGreebles respond to neighbor greeble objects and not react with other objects\n\nGreebles tool designed for adding random tiny sci-fi details or creating \"pixel art style\" grid based textures. For most other tasks, use Poly Lasso and Detail Tool instead. \n\n<color="+textHotkeyColor+">Left Click</color>: Scatter \n\n<color="+textHotkeyColor+">Shift + Left Click</color>: Grow \n\n<color="+textHotkeyColor+">Right Click</color>: Reroll \n\n<color="+textHotkeyColor+">Shift + Right Click</color>: Remove in order \n\n<color="+textHotkeyColor+">Drag blue squares</color>: Change greebles region size"), limitsButtonStyle ) ) {
							Surforge.ToggleLimitsToolActive();
							activeTool = 2; 
						}
						GUILayout.Label("Greebles", activeToolName);
					}
				
					if (activeTool == 3) {
						Surforge.DeactivateLimitsTool();
						Surforge.DeactivatePolygonLassoTool();
						Surforge.DeactivatePlaceTool();
					
						if( GUILayout.Button(new GUIContent(renderIcon, "<b><size=14><color="+textHeaderColor+">Render</color> <color="+textHotkeyColor+">(Space)</color></size></b> \n\nRender scene to a texture\n\nIt should be done after scene changes and material masks assign to the scene objects\n\nMaterial editing is real-time and does not require re-render"), buttonStyle  ) ) {
							activeTool = 3;
							Surforge.StartGPURender();
						}
						GUILayout.Label("Render", activeToolName);
					}
				
					if (activeTool == 4) {
						Surforge.DeactivateLimitsTool();
						Surforge.DeactivatePolygonLassoTool();

						GUIStyle placeMeshButtonStyle = buttonStyle;
						if (Surforge.surforgeSettings.isPlaceToolActive) placeMeshButtonStyle = buttonStyleActive;
						if( GUILayout.Button(new GUIContent(placeIcon, "<b><size=14><color="+textHeaderColor+">Add Detail</color> <color="+textHotkeyColor+">(D)</color></size></b> \n\n\n<color="+textHotkeyColor+">Left Click</color>: Set objects \n\n<color="+textHotkeyColor+">Left Button + drag</color>: Rotate \n\n<color="+textHotkeyColor+">Left Button + Shift + drag</color>: Rotate and Scale \n\n<color="+textHotkeyColor+">Hold Ctrl</color>: Constraint Move, Rotate, Scale\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move object up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Object scale decrease / increase\n\n<color="+textHotkeyColor+">Right Click</color>: Flip object or randomize text\n\n<color="+textHotkeyColor+">Shift + Right Click</color>: Set symmetry center to UV island\n\n<color="+textHotkeyColor+">Ctrl + Shift + Right Click</color>: Reset symmetry axes\n\n<color="+textHotkeyColor+">Esc</color> or <color="+textHotkeyColor+">D</color>: exit tool"), placeMeshButtonStyle  ) ) {
							Surforge.TogglePlaceToolActive();
							placeToolState = 0;

							Surforge.surforgeSettings.placeToolVerticalOffset = 0;

							Surforge.surforgeSettings.activePlaceMesh = oldSelectedPlaceToolProfileNum;
							selectedPlaceToolProfileNum = oldSelectedPlaceToolProfileNum;

							activeTool = 4;
						}
						GUILayout.Label("Add Detail", activeToolName);
					}

				
					if (activeTool == 5) {
						Surforge.DeactivateLimitsTool();
						Surforge.DeactivatePlaceTool();
					
						GUIStyle polygonLassoButtonStyle = buttonStyle;  
						if (Surforge.surforgeSettings.isPolygonLassoActive) polygonLassoButtonStyle = buttonStyleActive;

						if( GUILayout.Button(new GUIContent(polygonLassoIcon, "<b><size=14><color="+textHeaderColor+">Poly Lasso</color> <color="+textHotkeyColor+">(A)</color></size></b> \n\n\n<color="+textHotkeyColor+">Left Click</color>: Set points \n\n<color="+textHotkeyColor+">Double Click</color> or <color="+textHotkeyColor+">Enter</color> after set points: Finish shape\n\n<color="+textHotkeyColor+">Double Click</color>: Fill UV island or background \n\n<color="+textHotkeyColor+">Ctrl + Left Click</color>: Split selected with current shape\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Profile scale decrease / increase\n\n<color="+textHotkeyColor+">Backspace</color>: Remove last point\n\n<color="+textHotkeyColor+">Hold Shift</color>: Snap to 45 degrees\n\n<color="+textHotkeyColor+">Numpad 5</color>: Repeat shape points left to right\n\n<color="+textHotkeyColor2+">Numpad Keys</color>: Continue shape with symmetry about last point in numpad key direction\n\n<color="+textHotkeyColor2+">Left Click, move, Right Click</color>: Set symmetry axes\n\n<color="+textHotkeyColor2+">Shift + Right Click</color>: Set symmetry center to UV island\n\n<color="+textHotkeyColor2+">Right Click</color>: Mirror last shape action (solid line)\n\n<color="+textHotkeyColor2+">Ctrl + Right Click</color>: Mirror last shape action (dotted line)\n\n<color="+textHotkeyColor2+">Ctrl + Shift + Right Click</color>: Reset symmetry axes\n\n<color="+textHotkeyColor+">Ctrl + Middle Click</color>: Arc mode toggle\n\n<color="+textHotkeyColor+">Ctrl + Scroll</color>: Arc curvature change\n\n<color="+textHotkeyColor+">Ctrl + Alt + Scroll</color>: Arc points density change\n\n<color="+textHotkeyColor+">Ctrl + Alt + Middle Click</color>: Set arc curvature to 1/4 circle\n\n<color="+textHotkeyColor2+">Alt + Shift + Right Click</color>: Set warp shape\n\n<color="+textHotkeyColor2+">Ctrl + Alt + Shift + Right Click</color>: Reset warp shape\n\n<color="+textHotkeyColor2+">Alt + Shift + Left Click</color>: Warp selected / Create warped shape / Repeated warp (if 2 similar PolyLasso objects selected)\n\n<color="+textHotkeyColor2+">Ctrl + Alt + Shift + Left Click</color>: Warped split\n\n<color="+textHotkeyColor2+">Esc</color> or <color="+textHotkeyColor2+">A</color>: exit tool"), polygonLassoButtonStyle  ) ) {
							Surforge.TogglePolygonLassoActive();
							
							Surforge.surforgeSettings.activePolyLassoProfile = oldSelectedPolyLassoProfileNum;
							selectedPolyLassoProfileNum = oldSelectedPolyLassoProfileNum;
						
							polygonLassoPoints = new List<Vector3>();
						
							activeTool = 5;
						}
						GUILayout.Label("Poly Lasso", activeToolName);
					}
				
				
				
					GUILayout.EndHorizontal();
				
				
				
					EditorGUILayout.Separator();
				
					if (activeTool == 0) {
						if( GUILayout.Button(new GUIContent("Remove decal", "<b><color="+textHeaderColor+">Remove decal</color></b>\n\nRemove last added decal from selected Poly Lasso objects"), guiButtonStyle)) {
							Surforge.RemoveLastDecal();
						}
					
						EditorGUILayout.Separator();
						selectedDecalNum = GUILayout.SelectionGrid(selectedDecalNum, decalButtons, 3,  buttonStyle);
						if (Surforge.surforgeSettings.activeDecal != selectedDecalNum) {
							Surforge.LogAction("Select decal", "", "");
							RepaintSceneView();
						}
						Surforge.surforgeSettings.activeDecal = selectedDecalNum;
					
						EditorGUILayout.Separator();
					}


					if (activeTool == 1) {

						GUILayout.Label("Assign Masks", guiTextStyle); 

						GUILayout.BeginHorizontal();
						if(GUILayout.Button(new GUIContent("1", "<b><color="+textHeaderColor+">Assign Material Mask 1 to selection</color></b>\n\n<color="+textHotkeyColor+">1</color>: Assign material mask 1 to selection\n\n<color="+textHotkeyColor+">Shift + 1</color>: Assign material mask 1 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) { 
							Surforge.AssignMaterialGroupToSelection(0);
						}
						if(GUILayout.Button(new GUIContent("2", "<b><color="+textHeaderColor+">Assign Material Mask 2 to selection</color></b>\n\n<color="+textHotkeyColor+">2</color>: Assign material mask 2 to selection\n\n<color="+textHotkeyColor+">Shift + 2</color>: Assign material mask 2 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(1);
						}
						if(GUILayout.Button(new GUIContent("3", "<b><color="+textHeaderColor+">Assign Material Mask 3 to selection</color></b>\n\n<color="+textHotkeyColor+">3</color>: Assign material mask 3 to selection\n\n<color="+textHotkeyColor+">Shift + 3</color>: Assign material mask 3 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(2);
						}
						if(GUILayout.Button(new GUIContent("4", "<b><color="+textHeaderColor+">Assign Material Mask 4 to selection</color></b>\n\n<color="+textHotkeyColor+">4</color>: Assign material mask 4 to selection\n\n<color="+textHotkeyColor+">Shift + 4</color>: Assign material mask 4 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(3);
						}
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						if(GUILayout.Button(new GUIContent("5", "<b><color="+textHeaderColor+">Assign Material Mask 5 to selection</color></b>\n\n<color="+textHotkeyColor+">5</color>: Assign material mask 5 to selection\n\n<color="+textHotkeyColor+">Shift + 5</color>: Assign material mask 5 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(4);
						}
						if(GUILayout.Button(new GUIContent("6", "<b><color="+textHeaderColor+">Assign Material Mask 6 to selection</color></b>\n\n<color="+textHotkeyColor+">6</color>: Assign material mask 6 to selection\n\n<color="+textHotkeyColor+">Shift + 6</color>: Assign material mask 6 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(5);
						}
						if(GUILayout.Button(new GUIContent("7", "<b><color="+textHeaderColor+">Assign Material Mask 7 to selection</color></b>\n\n<color="+textHotkeyColor+">7</color>: Assign material mask 7 to selection\n\n<color="+textHotkeyColor+">Shift + 7</color>: Assign material mask 7 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(6);
						}
						if(GUILayout.Button(new GUIContent("8", "<b><color="+textHeaderColor+">Assign Material Mask 8 to selection</color></b>\n\n<color="+textHotkeyColor+">8</color>: Assign material mask 8 to selection\n\n<color="+textHotkeyColor+">Shift + 8</color>: Assign material mask 8 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+</color>,<color="+textHotkeyColor+"> - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(7);
						}
						GUILayout.EndHorizontal();
						
						GUILayout.BeginHorizontal();
						if(GUILayout.Button(new GUIContent("E1", "<b><color="+textHeaderColor+">Assign Emission Mask 1 to selection</color></b>\n\n<color="+textHotkeyColor+">9</color>: Assign emission mask 1 to selection\n\n<color="+textHotkeyColor+">Shift + 9</color>: Assign emission mask 1 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+, - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale (offset for PolyLasso objects) decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) { 
							Surforge.AssignMaterialGroupToSelection(8);
						}
						if(GUILayout.Button(new GUIContent("E2", "<b><color="+textHeaderColor+">Assign Emission Mask 2 to selection</color></b>\n\n<color="+textHotkeyColor+">0</color>: Assign emission mask 2 to selection\n\n<color="+textHotkeyColor+">Shift + 0</color>: Assign emission mask 2 to selection and similar objects\n\n<color="+textHotkeyColor+">Shift + S</color>: Select similar objects\n\n<color="+textHotkeyColor+">Shift + M</color>: Select objects with the same material mask\n\n<color="+textHotkeyColor+">+, - </color>: Move selected objects up and down\n\n<color="+textHotkeyColor+">[</color>,<color="+textHotkeyColor+"> ] </color>: Selected objects scale (offset for PolyLasso objects) decrease / increase\n\n<color="+textHotkeyColor+">Shift + [</color>, <color="+textHotkeyColor+">] </color>: Selected PolyLasso objects shrink / expand"), guiButtonStyle )) {
							Surforge.AssignMaterialGroupToSelection(9);
						}
						GUILayout.EndHorizontal();

						
						GUILayout.BeginHorizontal();
						if(GUILayout.Button(new GUIContent(randomGroupsIcon, "<b><color="+textHeaderColor+">Assign random Material Masks to selection</color></b>\n\nIf no objects selected, assign random material masks to all objects"), groupToolsButton)) {
							Surforge.RandomGroups();
						}
						if(GUILayout.Button(new GUIContent(groupsShiftIcon, "<b><color="+textHeaderColor+">Shift selection Material Masks</color></b>\n\nIf no objects selected, shift material masks of all objects\n\n(Shift is a slight material mask randomize: some objects get mask+1, some will not change)"), groupToolsButton)) {
							Surforge.ShiftGroups();
						}
						if(GUILayout.Button(new GUIContent(similarGroupsIcon, "<b><color="+textHeaderColor+">Assign same Material Masks to similar objects</color></b>\n\nIf no objects selected, assign same material masks to all similar objects"), groupToolsButton)) {
							Surforge.SimilarGroups();
						}
						if(GUILayout.Button(new GUIContent(randomHeightIcon, "<b><color="+textHeaderColor+">Assign random vertical offset to selected PolyLasso objects</color></b>\n\nIf no objects selected, assign random vertical offset to all PolyLasso objects"), groupToolsButton)) {
							Surforge.RandomHeights();
						}
						if(GUILayout.Button(new GUIContent(similarHeightIcon, "<b><color="+textHeaderColor+">Assign same vertical offset to similar PolyLasso objects</color></b>\n\nIf no objects selected, assign same vertical offset to all similar PolyLasso objects"), groupToolsButton)) {
							Surforge.SimilarHeights();
						}
						GUILayout.EndHorizontal();




						EditorGUILayout.Separator();
						EditorGUILayout.BeginHorizontal();
						//GUILayout.Label("Materials", guiTextStyle); 
						bool allFolded = true;
						string foldButtonName = "unfold"; 
						for (int i=0; i <Surforge.surforgeSettings.materialFoldersFoldout.Length; i++) {
							if (Surforge.surforgeSettings.materialFoldersFoldout[i]) {
								allFolded = false;
								foldButtonName = "fold";
								break;
							}
						}
						if(GUILayout.Button(new GUIContent(foldButtonName, "<b><color="+textHeaderColor+">Fold / unfold materials</color></b>"), reloadMaterialsButtonStyle, new GUILayoutOption[] {GUILayout.Width(50)})) {

							for (int i=0; i <Surforge.surforgeSettings.materialFoldersFoldout.Length; i++) {
								if (allFolded) Surforge.surforgeSettings.materialFoldersFoldout[i] = true;
								else Surforge.surforgeSettings.materialFoldersFoldout[i] = false;
							}
						}

						GUILayout.FlexibleSpace();

						if(GUILayout.Button(new GUIContent("reload", "<b><color="+textHeaderColor+">Reload custom Materials</color></b>\n\nReload custom materials from <color="+textHotkeyColor+">Surforge/CustomPresets/Materials/</color> folder\n\nCustom materials will appear at the end of material list, after Surforge built-in materials.\n\nPlease use it after save, duplicate, edit, delete, rename your Custom Materials in the project to update your Material List"), reloadMaterialsButtonStyle)) {
							Surforge.ReloadProjectMaterials();

							PrepareMaterialButtons();
							GetWindow<SurforgeInterface>().Repaint();
						}
						EditorGUILayout.EndHorizontal();


						//check if material icon count actual or need reload
						bool materialIconCountActual = true;
						if (Surforge.surforgeSettings.materialButtons == null) {
							materialIconCountActual = false;
						}
						else {
							if (Surforge.surforgeSettings.materialButtons.Length != Surforge.surforgeSettings.loadedMaterials.Length) {
								materialIconCountActual = false;
							}
						}
						if (!materialIconCountActual) {
							PrepareMaterialButtons();
							GetWindow<SurforgeInterface>().Repaint();
						}


						//material icons scroll
						materialScrollPosition = EditorGUILayout.BeginScrollView(materialScrollPosition, GUILayout.Width(139), GUILayout.Height(window.position.size.y -287));  
						EditorGUILayout.BeginVertical();

						DrawMaterialDragDropIcons(materialDragDropOffsetY);   

						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();
					}

				
				
					if (activeTool == 2) {

						EditorGUILayout.BeginHorizontal();

						EditorGUILayout.BeginVertical();

						if( GUILayout.Button(new GUIContent("Scatter", "<b><color="+textHeaderColor+">Scatter greebles</color> <color="+textHotkeyColor+">(Left Click)</color></b> \n\nCreate greebles randomly on the construction plane in chosen volume\n\nGreebles designed to react with other nearby greebles and volume edges. Some will change their appearance, forming borders and corners. Some will just avoid intersections"), guiButtonStyle)) {
							Surforge.LogAction("Scatter greebles", "Left Click", "");
							Surforge.VoxelEngineScatter();
						}
					
						if( GUILayout.Button(new GUIContent("Grow", "<b><color="+textHeaderColor+">Grow greebles</color> <color="+textHotkeyColor+">(Shift + Left Click)</color></b> \n\nCreate greebles near other greebles in chosen volume\n\nThe grown greebles will be chosen and oriented according to greeble set design (tubes will grow other tubes in proper direction for example)\n\nNote, that not all greeble sets designed for growing. If nothing happens use Scatter instead"), guiButtonStyle)) {
							Surforge.LogAction("Grow greebles", "Shift + Left Click", "");
							Surforge.Grow();
						}
					
						if(GUILayout.Button(new GUIContent("Remove", "<b><color="+textHeaderColor+">Remove in order</color> <color="+textHotkeyColor+">(Shift + Right Click)</color></b> \n\nRemove greebles in order in which they were created\n\nSince greebles are voxel-based and stored in octree, deleting or moving them by hand will break their logic (greeble models still can be moved and rendered correctly)\n\nOn this stage, it is the only proper way to remove greebles (will be improved in next versions)"), guiButtonStyle)) {  
							Surforge.LogAction("Remove greebles in order", "Shift + Right Click", "");
							Surforge.SurforgeUndo();
						}
						if(GUILayout.Button(new GUIContent("Reroll", "<b><color="+textHeaderColor+">Reroll greebles</color> <color="+textHotkeyColor+">(Right Click)</color></b> \n\nRemove last added greebles and scatter/grow them again, randomizing their position and appearance"), guiButtonStyle)) {
							Surforge.LogAction("Reroll greebles", "Right Click", "");
							Surforge.Reroll();
						}

						EditorGUILayout.EndVertical();

						DrawLayerHeightControls();

						EditorGUILayout.EndHorizontal();
					
						EditorGUILayout.Separator();
						EditorGUILayout.Separator();

						greeblesToolScrollPosition = EditorGUILayout.BeginScrollView(greeblesToolScrollPosition, GUILayout.Width(139), GUILayout.Height(window.position.size.y -205));  
						EditorGUILayout.BeginVertical();
						selectedSetNum = GUILayout.SelectionGrid(selectedSetNum, setButtons, 3,  polyLassoButtonStyle);

						if(Surforge.surforgeSettings.activeSet != selectedSetNum) {
							Surforge.LogAction("Select greebles set", "", "");
						}
						Surforge.surforgeSettings.activeSet = selectedSetNum;

						DrawPolyLassoButtonHighlight(1, selectedSetNum, false);
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();
					
						EditorGUILayout.Separator();
					}
				
				
					if (activeTool == 3) {

						GUILayout.BeginHorizontal();
						GUILayout.Label("Res: ", guiTextStyle);
						Surforge.surforgeSettings.SetGpuRenderResolution (EditorGUILayout.Popup("", Surforge.surforgeSettings.GetGpuRenderResolution(), gpuRenderResolutions, resolutionPopup));
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						GUILayout.Label("AO mode: ", guiTextStyle); 
						if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11) {
							Surforge.surforgeSettings.SetAoMode (EditorGUILayout.Popup("", Surforge.surforgeSettings.GetAoMode(), aoModes, aoModePopup));
						}
						else {
							Surforge.surforgeSettings.SetAoMode (EditorGUILayout.Popup("", Surforge.surforgeSettings.GetAoMode(), aoModesNoSsao, aoModePopup));
						}
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						GUILayout.Label("Antialiasing: ", guiTextStyle);
						Surforge.surforgeSettings.SetSupersamplingMode (EditorGUILayout.Popup("", Surforge.surforgeSettings.GetSupersamplingMode(), supersamplingModes, antialiasingPopup));
						GUILayout.EndHorizontal();

					
						EditorGUILayout.Separator();
		
						GUILayout.Label(new GUIContent("Open model", "<b><color="+textHeaderColor+">Open model for texture preview</color></b> \n\nThe UVs will be shown on the Scene View and can be used as modeling guides"), guiTextStyle);
						//Surforge.surforgeSettings.model = (Mesh)EditorGUILayout.ObjectField(Surforge.surforgeSettings.model, typeof(Mesh), false);

						// -------multi asset mesh drag-drop--------------
						Surforge.surforgeSettings.modelObject = (Object)EditorGUILayout.ObjectField(Surforge.surforgeSettings.model, typeof(Object), true);

						if (Surforge.surforgeSettings.modelObject != null) {

							if (Surforge.surforgeSettings.modelObject.GetType() == typeof(Mesh)) {
								Surforge.surforgeSettings.model = (Mesh)Surforge.surforgeSettings.modelObject;
							}
							else {
								if (Surforge.surforgeSettings.modelObject.GetType() == typeof(MeshFilter)) {
									MeshFilter draggedMeshFilter = (MeshFilter)Surforge.surforgeSettings.modelObject;
									Surforge.surforgeSettings.model = (Mesh)draggedMeshFilter.sharedMesh;
								}
							}

						}
						else {
							Surforge.surforgeSettings.model = null;
						}
						//--------------------------------------------------

						if (Surforge.surforgeSettings.extentTexturePreview != null) {
							if (Surforge.surforgeSettings.extentTexturePreview.composer != null) {
								if (Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter != null) {
									if (Surforge.surforgeSettings.model != null) {
										if (Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh != Surforge.surforgeSettings.model) {
											RepaintSceneView(); 
										}
									}
								}
							}
						}


						GUILayout.Label(new GUIContent("Add AO map", "<b><color="+textHeaderColor+">Add AO map</color></b> \n\nAdd Ambient Occlusion map to be used along with Surforge rendered AO\n\nIf model has baked AO map, it goes here\n\nUpdate render to make it visible"), guiTextStyle);
						Surforge.surforgeSettings.modelBakedAO = (Texture2D)EditorGUILayout.ObjectField(Surforge.surforgeSettings.modelBakedAO, typeof(Texture2D), false);
					
						GUILayout.Label(new GUIContent("Add Normal map", "<b><color="+textHeaderColor+">Add Normal map</color></b> \n\nAdd tangentspace Normal map to be used along with Surforge rendered Normal map\n\nIf model has baked Normal map, it goes here\n\nUpdate render to make it visible"), guiTextStyle);
						Surforge.surforgeSettings.modelBakedNormal = (Texture2D)EditorGUILayout.ObjectField(Surforge.surforgeSettings.modelBakedNormal, typeof(Texture2D), false);

						GUILayout.Label(new GUIContent("Add Edge map", "<b><color="+textHeaderColor+">Add Edge map</color></b> \n\nAdd Edge map to be used along with Surforge worn edges\n\nIf model has baked Edges (contrast curvature) map, it goes here\n\nUpdate render to make it visible"), guiTextStyle);
						Surforge.surforgeSettings.modelBakedEdgeMap = (Texture2D)EditorGUILayout.ObjectField(Surforge.surforgeSettings.modelBakedEdgeMap, typeof(Texture2D), false);


						EditorGUILayout.Separator();
						EditorGUILayout.Separator();
					
						if(GUILayout.Button(new GUIContent("Export maps", "<b><color="+textHeaderColor+">Export Maps</color> <color="+textHotkeyColor+">(Ctrl + E)</color></b>\n\nExport maps for using with Unity Standard Shader (Specular setup)" ), guiButtonStyle)) {
							Surforge.ExportMapsGPU();  
							Surforge.LogAction("Export maps", "Ctrl + E", "");
						}
						GUILayout.BeginHorizontal();
						GUILayout.Label("Masks: ", guiTextStyle); 
						Surforge.surforgeSettings.SetMaskExportMode (EditorGUILayout.Popup("", Surforge.surforgeSettings.GetMaskExportMode(), maskExportModes, maskExportPopup));
						GUILayout.EndHorizontal();

						EditorGUILayout.Separator();
						EditorGUILayout.Separator();
						if(GUILayout.Button(new GUIContent("Screenshot", "<b><color="+textHeaderColor+">Capture screenshot of Texture Preview</color> </b>\n\nIt will be saved at <color="+textHotkeyColor+">Assets/Surforge/Screenshots/</color> \n\nScreenshots can be of 2 different layous: \nWireframe, Textured, materials (wireframe on) \nTextured, maps preview (wireframe off)" ), guiButtonStyle)) {
							if (Surforge.surforgeSettings.showWireframe) {
								Surforge.TakePreviewScreenshotWithMaterials();
							}
							else {
								if (shaderMode == 0) Surforge.TakePreviewScreenshotWithMaps(false);
								else Surforge.TakePreviewScreenshotWithMaps(true);
							}
						}
						//wireframe
						GUIContent showWireframeGuiContent = new GUIContent("Show wireframe", "<b><color="+textHeaderColor+">Show wireframe</color> </b>\n\nShow model copy with wireframe shader (for screenshots)\n\nYou can select it in Hierarchy and move to desired position");
						if (Surforge.surforgeSettings.showWireframe) {
							if(GUILayout.Button(showWireframeGuiContent, Surforge.surforgeSettings.showWireframe ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.showWireframe = false;
								if (Surforge.surforgeSettings.wireframeObject) {
									DestroyImmediate(Surforge.surforgeSettings.wireframeObject);
								}
							}
						}
						else {
							if(GUILayout.Button(showWireframeGuiContent, Surforge.surforgeSettings.showWireframe ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.showWireframe = true;
								if (!Surforge.surforgeSettings.wireframeObject) {
									Surforge.CreatePreviewWireframeMesh();
								}
							}
						}

						/*
						if ( GUILayout.Button("Show wireframe", guiButtonStyle) ) {  
							Surforge.surforgeSettings.showWireframe = !Surforge.surforgeSettings.showWireframe;
						}
						if (Surforge.surforgeSettings.showWireframe) {
							if (!Surforge.surforgeSettings.wireframeObject) {
								Surforge.CreatePreviewWireframeMesh();
							}
						}
						else {
							if (Surforge.surforgeSettings.wireframeObject) {
								DestroyImmediate(Surforge.surforgeSettings.wireframeObject);
							}
						}
						*/

						//floor
						/*
						if ( GUILayout.Button("Show floor", guiButtonStyle) ) {
							Surforge.surforgeSettings.showFloor = !Surforge.surforgeSettings.showFloor;
						}
						if (Surforge.surforgeSettings.showFloor) {
							if (!Surforge.surforgeSettings.floorObject) {
								Surforge.CreateFloor();
							}
						}
						else {
							if (Surforge.surforgeSettings.floorObject) {
								DestroyImmediate(Surforge.surforgeSettings.floorObject);
							}
						}
						*/


						//---------------------------------
						//---------------drag --------------
						GUILayout.BeginHorizontal();
						GUILayout.Label(AssetPreview.GetMiniTypeThumbnail(typeof(Material)), composerMaterialDrag); 
						GUILayout.Label("previewMat", guiTextStyle);
						GUILayout.EndHorizontal();
						Rect composerDragRect = GUILayoutUtility.GetLastRect(); 
						Event e = Event.current;
						if (composerDragRect.Contains(e.mousePosition) && e.type == EventType.MouseDrag) {
							Object dragObject = Surforge.surforgeSettings.composerMaterialAsset;

							DragAndDrop.PrepareStartDrag();
							DragAndDrop.objectReferences = new[] { dragObject };
							DragAndDrop.visualMode = DragAndDropVisualMode.Link;
							DragAndDrop.StartDrag("drag");
							Event.current.Use();
						}

						//------------end drag-------------------

						EditorGUILayout.Separator();
						EditorGUILayout.Separator();
						GUILayout.Label("Skybox:", guiTextStyle);

						//skybox icons scroll
						skyboxScrollPosition = EditorGUILayout.BeginScrollView(skyboxScrollPosition, GUILayout.Width(139), GUILayout.Height(window.position.size.y -455));  
						EditorGUILayout.BeginVertical();
						

						selectedSkyboxNum = GUILayout.SelectionGrid(selectedSkyboxNum, skyboxButtons, 3, polyLassoButtonStyle); 
						int oldSkybox = Surforge.surforgeSettings.activeSkybox;
							
						Surforge.surforgeSettings.activeSkybox = selectedSkyboxNum;
							
						if (oldSkybox == Surforge.surforgeSettings.activeSkybox) {

						}
						else {
							Surforge.LogAction("Skybox select", "", "");
							SetSkybox();
						}
						
						DrawPolyLassoButtonHighlight(1, selectedSkyboxNum, false); 


						
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();



					}



					if (activeTool == 4) {

						EditorGUILayout.BeginHorizontal();

						if (Surforge.surforgeSettings.symmetry) {
							if(GUILayout.Button(symmetryGuiContent, Surforge.surforgeSettings.symmetry ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.symmetry = false;
								Surforge.LogAction("Symmetry: off", "S", "");
								RepaintSceneView();
								Surforge.ChangePlaceToolPreview();
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContent, Surforge.surforgeSettings.symmetry ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.symmetry = true;
								Surforge.LogAction("Symmetry: on", "S", "");
								RepaintSceneView();
								Surforge.ChangePlaceToolPreview();
							}
						}
							
						//x
						if (Surforge.surforgeSettings.symmetryX) {
							if(GUILayout.Button(symmetryGuiContentX, Surforge.surforgeSettings.symmetryX ? smallToggleButtonSymmXActive : smallToggleButtonSymmX)) {
								Surforge.surforgeSettings.symmetryX = false;
								Surforge.ChangePlaceToolPreview();
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContentX, Surforge.surforgeSettings.symmetryX ? smallToggleButtonSymmXActive : smallToggleButtonSymmX)) { 
								Surforge.surforgeSettings.symmetryX = true;
								Surforge.ChangePlaceToolPreview();
							}
						}

						//z
						if (Surforge.surforgeSettings.symmetryZ) {
							if(GUILayout.Button(symmetryGuiContentZ, Surforge.surforgeSettings.symmetryZ ? smallToggleButtonSymmZActive : smallToggleButtonSymmZ)) {
								Surforge.surforgeSettings.symmetryZ = false;
								Surforge.ChangePlaceToolPreview();
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContentZ, Surforge.surforgeSettings.symmetryZ ? smallToggleButtonSymmZActive : smallToggleButtonSymmZ)) {
								Surforge.surforgeSettings.symmetryZ = true;
								Surforge.ChangePlaceToolPreview();
							}
						}

						//diagonal
						if (Surforge.surforgeSettings.symmetryDiagonal) {
							if(GUILayout.Button(symmetryGuiContentDiagonal, Surforge.surforgeSettings.symmetryDiagonal ? smallToggleButtonSymmDiagonalActive : smallToggleButtonSymmDiagonal)) {
								Surforge.surforgeSettings.symmetryDiagonal = false;
								Surforge.ChangePlaceToolPreview();
							} 
						}
						else {
							if(GUILayout.Button(symmetryGuiContentDiagonal, Surforge.surforgeSettings.symmetryDiagonal ? smallToggleButtonSymmDiagonalActive : smallToggleButtonSymmDiagonal)) {
								Surforge.surforgeSettings.symmetryDiagonal = true;
								Surforge.ChangePlaceToolPreview();
							}
						}


						EditorGUILayout.EndHorizontal();

						if (Surforge.surforgeSettings.seamless) {
							if(GUILayout.Button(seamlessGuiContent, Surforge.surforgeSettings.seamless ? toggleButtonActive : toggleButton)) {
								Surforge.DeactivateSeamlessMode();
								Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
							}
						}
						else {
							if(GUILayout.Button(seamlessGuiContent, Surforge.surforgeSettings.seamless ? toggleButtonActive : toggleButton)) {
								Surforge.ActivateSeamlessMode();
								Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
							}
						}


						EditorGUILayout.Separator();


						placeToolScrollPosition = EditorGUILayout.BeginScrollView(placeToolScrollPosition, GUILayout.Width(139), GUILayout.Height(window.position.size.y -205));  
						EditorGUILayout.BeginVertical();

						if ( Surforge.surforgeSettings.isPlaceToolActive) { 
							selectedPlaceToolProfileNum = GUILayout.SelectionGrid(selectedPlaceToolProfileNum, placeToolProfileButtons, 3, polyLassoButtonStyle);
							int oldActivePlaceMesh = Surforge.surforgeSettings.activePlaceMesh;

							Surforge.surforgeSettings.activePlaceMesh = selectedPlaceToolProfileNum;

							if (oldActivePlaceMesh == Surforge.surforgeSettings.activePlaceMesh) {
								Surforge.ChangePlaceToolPreview();
							}
							else {
								Surforge.LogAction("Detail select", "", "");

								Surforge.CreatePlaceToolPreview();
								Surforge.PlaceToolGenerateTexts();
								Surforge.CreatePlaceToolSymmetryPreviews();
							}
						}
						else {
							selectedPlaceToolProfileNum = -1;
							selectedPlaceToolProfileNum = GUILayout.SelectionGrid(selectedPlaceToolProfileNum, placeToolProfileButtons, 3, polyLassoButtonStyle);
							if (selectedPlaceToolProfileNum > -1) {
							}
						}
					
						DrawPolyLassoButtonHighlight(1, selectedPlaceToolProfileNum, false); 
					
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();
					}



				
					if (activeTool == 5) {

						EditorGUILayout.BeginHorizontal();
						
						if (Surforge.surforgeSettings.symmetry) {
							if(GUILayout.Button(symmetryGuiContent, Surforge.surforgeSettings.symmetry ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.symmetry = false;
								Surforge.LogAction("Symmetry: off", "S", "");
								RepaintSceneView();
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContent, Surforge.surforgeSettings.symmetry ? toggleButtonActive : toggleButton)) {
								Surforge.surforgeSettings.symmetry = true;
								Surforge.LogAction("Symmetry: on", "S", "");
								RepaintSceneView();
							}
						}
						
						//x
						if (Surforge.surforgeSettings.symmetryX) {
							if(GUILayout.Button(symmetryGuiContentX, Surforge.surforgeSettings.symmetryX ? smallToggleButtonSymmXActive : smallToggleButtonSymmX)) {
								Surforge.surforgeSettings.symmetryX = false;
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContentX, Surforge.surforgeSettings.symmetryX ? smallToggleButtonSymmXActive : smallToggleButtonSymmX)) { 
								Surforge.surforgeSettings.symmetryX = true;
							}
						}
						
						//z
						if (Surforge.surforgeSettings.symmetryZ) {
							if(GUILayout.Button(symmetryGuiContentZ, Surforge.surforgeSettings.symmetryZ ? smallToggleButtonSymmZActive : smallToggleButtonSymmZ)) {
								Surforge.surforgeSettings.symmetryZ = false;
							}
						}
						else {
							if(GUILayout.Button(symmetryGuiContentZ, Surforge.surforgeSettings.symmetryZ ? smallToggleButtonSymmZActive : smallToggleButtonSymmZ)) {
								Surforge.surforgeSettings.symmetryZ = true;
							}
						}
						
						//diagonal
						if (Surforge.surforgeSettings.symmetryDiagonal) {
							if(GUILayout.Button(symmetryGuiContentDiagonal, Surforge.surforgeSettings.symmetryDiagonal ? smallToggleButtonSymmDiagonalActive : smallToggleButtonSymmDiagonal)) {
								Surforge.surforgeSettings.symmetryDiagonal = false;
							} 
						}
						else {
							if(GUILayout.Button(symmetryGuiContentDiagonal, Surforge.surforgeSettings.symmetryDiagonal ? smallToggleButtonSymmDiagonalActive : smallToggleButtonSymmDiagonal)) {
								Surforge.surforgeSettings.symmetryDiagonal = true;
							}
						}
						
						
						EditorGUILayout.EndHorizontal();



						EditorGUILayout.BeginHorizontal();

						polyLassoScale = GUILayout.HorizontalSlider(polyLassoScale, 0.1f, 2.0f);
						polyLassoScale = polyLassoScale * 10.0f;
						polyLassoScale = Mathf.RoundToInt(polyLassoScale);
						polyLassoScale = polyLassoScale * 0.1f;
						string polyLassoScaleText = polyLassoScale.ToString();
						if (polyLassoScale == 1.0f) polyLassoScaleText = "1.0";

						GUILayout.Label(polyLassoScaleText, guiTextStyle, new GUILayoutOption[] {GUILayout.Width(22) } );
						EditorGUILayout.EndHorizontal();


						
						EditorGUILayout.BeginHorizontal();

						EditorGUILayout.BeginVertical();



						if (Surforge.surforgeSettings.seamless) {
							if(GUILayout.Button(seamlessGuiContent, Surforge.surforgeSettings.seamless ? toggleButtonActive : toggleButton)) {
								Surforge.DeactivateSeamlessMode();
								Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
							}
						}
						else {
							if(GUILayout.Button(seamlessGuiContent, Surforge.surforgeSettings.seamless ? toggleButtonActive : toggleButton)) {
								Surforge.ActivateSeamlessMode();
								Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
							}
						}
				
					
						if(GUILayout.Button(new GUIContent("Expand", "<b><color="+textHeaderColor+">Expand</color> <color="+textHotkeyColor+">( Shift + ] )</color></b>\n\nOffset selected Poly Lasso objects shape Out"), guiButtonStyle)) {
							Surforge.OffsetPolyLassoObjectsOut();
						}
						if(GUILayout.Button(new GUIContent("Shrink", "<b><color="+textHeaderColor+">Shrink</color> <color="+textHotkeyColor+">( Shift + [ )</color></b>\n\nOffset selected Poly Lasso objects shape Inside"), guiButtonStyle)) {
							Surforge.OffsetPolyLassoObjectsIn();
						}
						EditorGUILayout.EndVertical();

						DrawLayerHeightControls();

						EditorGUILayout.EndHorizontal();
					
						EditorGUILayout.Separator();


						//---- poly lasso profiles scrollview ----
						polyLassoScrollPosition = EditorGUILayout.BeginScrollView(polyLassoScrollPosition, GUILayout.Width(139), GUILayout.Height(window.position.size.y -260));  
						EditorGUILayout.BeginVertical();

						if ( Surforge.surforgeSettings.isPolygonLassoActive) {
							selectedPolyLassoProfileNum = GUILayout.SelectionGrid(selectedPolyLassoProfileNum, polyLassoProfileButtons, 3, polyLassoButtonStyle);
							Surforge.surforgeSettings.activePolyLassoProfile = selectedPolyLassoProfileNum;
							if (Surforge.surforgeSettings.activePolyLassoProfile != oldSelectedPolyLassoProfileNum) {
								Surforge.LogAction("Profile select", "", "");
							}
						}
						else {
							selectedPolyLassoProfileNum = -1;
							selectedPolyLassoProfileNum = GUILayout.SelectionGrid(selectedPolyLassoProfileNum, polyLassoProfileButtons, 3, polyLassoButtonStyle);
							if (selectedPolyLassoProfileNum > -1) {
								SetPolyLassoProfileToSelectedObjects(selectedPolyLassoProfileNum);
							}
						}

						DrawPolyLassoButtonHighlight(1, selectedPolyLassoProfileNum, false); 

						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();
   						//-------

					
						EditorGUILayout.Separator();
					}


					// deform moved here
					EditorGUILayout.Separator();
					EditorGUILayout.Separator();



					if (activeTool == 0) {
						GUILayout.BeginHorizontal("box");

						if(GUILayout.Button(new GUIContent(deformIcon, "<b><color="+textHeaderColor+">Deform selected Poly Lasso objects</color></b>\n\nDeform Poly Lasso object shape with chosen noise preset\n\nOnly one noise preset can be applied to the object (the last chosen)\n\nFurther Deform button pressing randomizes the result"), buttonStyle ) ) {
							Surforge.AddNoiseToPolyLassoObjects();
							activeTool = 0;
						}
						GUILayout.Label("Deform", activeToolName);

						GUILayout.EndHorizontal();
					}

					if (activeTool == 0) {
						if( GUILayout.Button(new GUIContent("Remove noise", "<b><color="+textHeaderColor+">Remove noise</color></b>\n\nRemove noise from selected Poly Lasso objects"), guiButtonStyle)) {
							Surforge.RemoveNoiseFromPolyLassoObjects();
						}
						EditorGUILayout.Separator();
						selectedNoiseNum = GUILayout.SelectionGrid(selectedNoiseNum, noiseButtons, 3,  buttonStyle);
						if (Surforge.surforgeSettings.activeNoisePreset != selectedNoiseNum) {
							Surforge.LogAction("Select noise preset", "", "");
							RepaintSceneView();
						}
						Surforge.surforgeSettings.activeNoisePreset = selectedNoiseNum;
						
						EditorGUILayout.Separator();
					}

					if (activeTool == 0) { 
						EditorGUILayout.Separator();
						EditorGUILayout.Separator();
						GUILayout.BeginHorizontal("box");
						
						if(GUILayout.Button(new GUIContent(shatterIcon, "<b><color="+textHeaderColor+">Shatter</color></b>\n\nShatter selected Poly Lasso objects into several parts"), buttonStyle ) ) {
							Surforge.ShatterPolyLassoObjects(); 
							activeTool = 0;
						}
						GUILayout.Label("Shatter", activeToolName);
						
						GUILayout.EndHorizontal();
						EditorGUILayout.Separator();
						selectedShatterNum = GUILayout.SelectionGrid(selectedShatterNum, shatterButtons, 3,  buttonStyle);
						if (Surforge.surforgeSettings.activeShatterPreset != selectedShatterNum) {
							Surforge.LogAction("Select shatter preset", "", "");
							RepaintSceneView();
						}
						Surforge.surforgeSettings.activeShatterPreset = selectedShatterNum;
					}




					//----------
					GUILayout.EndArea();







				
					if (GUI.changed) {
						if (oldSelectedContentPackNum != selectedContentPackNum) {
							oldSelectedContentPackNum = selectedContentPackNum;
							Surforge.PrepareVoxelizedSet();
						
							selectedSetNum = 0;
							Surforge.surforgeSettings.activeSet = selectedSetNum;
						}
					
						if ( Surforge.surforgeSettings.isPolygonLassoActive) {
							if (selectedPolyLassoProfileNum > -1) oldSelectedPolyLassoProfileNum = selectedPolyLassoProfileNum;
						}
						if (Surforge.surforgeSettings.isPlaceToolActive) {
							if (selectedPlaceToolProfileNum > -1) {
								oldSelectedPlaceToolProfileNum = selectedPlaceToolProfileNum;
								Surforge.surforgeSettings.placeToolVerticalOffset = 0;
							}
						}
						
					}
				
				
				
					//bottom togglers
					if ( ((activeTool == 0) && (window.position.size.y > 625) )||
						 (activeTool == 1) ||  (activeTool == 2) || 
					     ((activeTool == 3) && (window.position.size.y > 715) ) || 
					     (activeTool == 4) ||  (activeTool == 5) ) {
						GUILayout.BeginArea(new Rect(3, togglersPosY - 58, 300, togglersPosY)); 

						GUILayout.BeginHorizontal();

						if (Surforge.surforgeSettings.gloabalSnapActive) {
							if (Surforge.surforgeSettings.gridSnapActive) {
								if(GUI.Button(new Rect(28,10, 22, 22), new GUIContent(snapActiveIcon, "Snap to grid"), snapButton ) ) {
									Surforge.LogAction("Toggle snap to grid", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.gridSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(28,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to grid"), snapButtonTransparent ) ) {
									Surforge.LogAction("Toggle snap to grid", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.gridSnapActive = true;
								}
							}
					
							if (Surforge.surforgeSettings.uvSnapActive) {
								if(GUI.Button(new Rect(48,10, 22, 22), new GUIContent(snapActiveIcon, "Snap to UVs"), snapButton ) ) {
									Surforge.LogAction("Toggle snap to UVs", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.uvSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(48,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to UVs"), snapButtonTransparent ) ) {
									Surforge.LogAction("Toggle snap to UVs", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.uvSnapActive = true;
								}
							}
						
							if (Surforge.surforgeSettings.objectSnapActive) {
								if(GUI.Button(new Rect(68,10, 22, 22), new GUIContent(snapActiveIcon, "Snap to objects"), snapButton ) ) {
									Surforge.LogAction("Toggle snap to objects", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.objectSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(68,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to objects"), snapButtonTransparent ) ) {
									Surforge.LogAction("Toggle snap to objects", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.objectSnapActive = true;
								}
							}
					
							if (Surforge.surforgeSettings.selfSnapActive) {
								if(GUI.Button(new Rect(88,10, 22, 22), new GUIContent(snapActiveIcon, "Snap to active shape"), snapButton ) ) {
									Surforge.LogAction("Toggle snap to active shape", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.selfSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(88,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to active shape"), snapButtonTransparent ) ) {
									Surforge.LogAction("Toggle snap to active shape", "", "");
									RepaintSceneView();
									Surforge.surforgeSettings.selfSnapActive = true;
								}
							}
						}
						else {
							if (Surforge.surforgeSettings.gridSnapActive) {
								if(GUI.Button(new Rect(28,10, 22, 22), new GUIContent(snapIcon, "Snap to grid"), snapButton ) ) {
									Surforge.surforgeSettings.gridSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(28,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to grid"), snapButtonTransparent ) ) {
									Surforge.surforgeSettings.gridSnapActive = true;
								}
							}
					
							if (Surforge.surforgeSettings.uvSnapActive) {
								if(GUI.Button(new Rect(48,10, 22, 22), new GUIContent(snapIcon, "Snap to UVs"), snapButton ) ) {
									Surforge.surforgeSettings.uvSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(48,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to UVs"), snapButtonTransparent ) ) {
									Surforge.surforgeSettings.uvSnapActive = true;
								}
							}
					
							if (Surforge.surforgeSettings.objectSnapActive) {
								if(GUI.Button(new Rect(68,10, 22, 22), new GUIContent(snapIcon, "Snap to objects"), snapButton ) ) {
									Surforge.surforgeSettings.objectSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(68,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to objects"), snapButtonTransparent ) ) {
									Surforge.surforgeSettings.objectSnapActive = true;
								}
							}
					
							if (Surforge.surforgeSettings.selfSnapActive) {
								if(GUI.Button(new Rect(88,10, 22, 22), new GUIContent(snapIcon, "Snap to active shape"), snapButton ) ) {
									Surforge.surforgeSettings.selfSnapActive = false;
								}
							}
							else {
								if(GUI.Button(new Rect(88,10, 22, 22), new GUIContent(snapTransparentIcon, "Snap to active shape"), snapButtonTransparent ) ) {
									Surforge.surforgeSettings.selfSnapActive = true;
								}
							}
						}

			

						if (Surforge.surforgeSettings.gloabalSnapActive) {
							if(GUI.Button(new Rect(0,0, 28, 28), new GUIContent(snapToggleActiveIcon, "Toggle Snap\n\n<color="+textHotkeyColor+">Hold V</color>: Shape points snap"), snapToggleButtonActive ) ) {
								Surforge.LogAction("Snap: off", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.gloabalSnapActive = false;
							}
						}
						else {
							if(GUI.Button(new Rect(0,0, 28, 28), new GUIContent(snapToggleIcon, "Toggle Snap"), snapToggleButton ) ) {
								Surforge.LogAction("Snap: on", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.gloabalSnapActive = true;
							}
						}

						GUILayout.EndHorizontal();

				
						GUILayout.BeginHorizontal();
				
						if (Surforge.surforgeSettings.showGrid) {
							if(GUI.Button(new Rect(0,32, 22, 22), new GUIContent(gridIcon, "Show Grid"), buttonStyleMiniSelected ) ) {
								Surforge.LogAction("Hide Grid", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showGrid = false;
								showOctreeGrid = false;
							}
						}
						else {
							if(GUI.Button(new Rect(0,32, 22, 22), new GUIContent(gridIcon, "Show Grid"), buttonStyleMini ) ) {
								Surforge.LogAction("Show Grid", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showGrid = true;
								showOctreeGrid = true;
							}
						}
				
				
				
						if (Surforge.surforgeSettings.showUvs) {
							if(GUI.Button(new Rect(22,32, 22, 22), new GUIContent(uvsIcon, "Show UVs"), buttonStyleMiniSelected ) ) {
								Surforge.LogAction("Hide UVs", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showUvs = false;
								showUVs = false;
							}
						}
						else {
							if(GUI.Button(new Rect(22,32, 22, 22), new GUIContent(uvsIcon, "Show UVs"), buttonStyleMini ) ) {
								Surforge.LogAction("Show UVs", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showUvGrid = false;
								showUVsHelperGrid = false;
						
								Surforge.surforgeSettings.showUvs = true;
								showUVs = true;
							}
						}
				
				
				
						if (Surforge.surforgeSettings.showUvGrid) {
							if(GUI.Button(new Rect(44,32, 22, 22), new GUIContent(uvGridIcon, "Show UV Guides"), buttonStyleMiniSelected ) ) {
								Surforge.LogAction("Hide UV Guides", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showUvGrid = false;
								showUVsHelperGrid = false;
							}
						}
						else {
							if(GUI.Button(new Rect(44,32, 22, 22), new GUIContent(uvGridIcon, "Show UV Guides"), buttonStyleMini ) ) {
								Surforge.LogAction("Show UV Guides", "", "");
								RepaintSceneView();

								Surforge.surforgeSettings.showUvs = false;
								showUVs = false;
						
								Surforge.surforgeSettings.showUvGrid = true;
								showUVsHelperGrid = true;
						
								if (!uvGuidesUpdated) PrepareUVs();
							}
						}
				
				
						if (Surforge.surforgeSettings.showLastAction) {
							if(GUI.Button(new Rect(66,32, 22, 22), new GUIContent(lastActionIcon, "Show Actions"), buttonStyleMiniSelected ) ) {
								Surforge.LogAction("Hide Actions", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.showLastAction = false;  
							}
						}
						else {
							if(GUI.Button(new Rect(66,32, 22, 22), new GUIContent(lastActionIcon, "Show Actions"), buttonStyleMini ) ) {
								Surforge.LogAction("Show Actions", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.showLastAction = true;
							}
						}
				
				
						if (Surforge.surforgeSettings.showSymAxis) {
							if(GUI.Button(new Rect(88,32, 22, 22), new GUIContent(symAxisIcon, "Show Symmetry axes"), buttonStyleMiniSelected ) ) {
								Surforge.LogAction("Hide Symmetry axes", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.showSymAxis = false;
							}
						}
						else {
							if(GUI.Button(new Rect(88,32, 22, 22), new GUIContent(symAxisIcon, "Show Symmetry axes"), buttonStyleMini ) ) {
								Surforge.LogAction("Show Symmetry axes", "", "");
								RepaintSceneView();
								Surforge.surforgeSettings.showSymAxis = true;
							}
						}
				
				
						GUILayout.EndHorizontal();
						GUILayout.EndArea();
					}
				
				}


			
				if (materialEditor != null) {
	
					if (windowSizeX > windowMinSizePlusOne) {
						if (Surforge.surforgeSettings.extentTexturePreview != null) { 
							if (Surforge.surforgeSettings.extentTexturePreview.composer.gameObject != null) { ///
								if (Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture != null) {

									if (PlayerSettings.colorSpace == ColorSpace.Linear) {


										if (!Surforge.surforgeSettings.isPreviewRenderTextureLinear) {
											RenderTexture.active = null;
											Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture.Release();
											//Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

											Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24); 
											Surforge.surforgeSettings.extentTexturePreview.previewCamera.targetTexture = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
											RenderTexture.active = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
											Surforge.surforgeSettings.isPreviewRenderTextureLinear = true;
										}

										#if UNITY_2018
										#else
											GL.sRGBWrite = true;
										#endif

										GUI.Label(previewRect, new GUIContent(Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture, texturePreviewTooltip), texturePreviewGuiLabel);

										#if UNITY_2018
										#else
											GL.sRGBWrite = false; 
										#endif
									}
									else {

										if (Surforge.surforgeSettings.isPreviewRenderTextureLinear) {
											RenderTexture.active = null;
											Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture.Release();
											Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24); 
											Surforge.surforgeSettings.extentTexturePreview.previewCamera.targetTexture = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
											RenderTexture.active = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
											Surforge.surforgeSettings.isPreviewRenderTextureLinear = false;
										}



										GUI.Label(previewRect, new GUIContent(Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture, texturePreviewTooltip), texturePreviewGuiLabel);
									}

									//DrawTexturedRectangle(previewRect, Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture);



									//specular and glossiness charts in map preview mode
									Rect chartRect = new Rect (previewRect.x + previewRect.width * 0.125f * 7, previewRect.y, previewRect.width * 0.125f, previewRect.height);
									if (shaderMode == 1) {
										GUI.Label(chartRect, new GUIContent(albedoChart, texturePreviewTooltip), texturePreviewGuiLabel);
									}
									if (shaderMode == 3) {
										GUI.Label(chartRect, new GUIContent(specularChart, texturePreviewTooltip), texturePreviewGuiLabel);
									}
									if (shaderMode == 4) {
										GUI.Label(chartRect, new GUIContent(glossinessChart, texturePreviewTooltip), texturePreviewGuiLabel);
									}



									//Texture Preview Info: material names  
									if ( (mousePos.y < (window.position.size.y - 70)) && (mousePos.y > 215 ) ) { //check if mouse not outside materials list
										if ( (mousePos.x > hoverIconRect.xMin) && (mousePos.y > hoverIconRect.yMin) && (mousePos.x < hoverIconRect.xMax)&& (mousePos.y < hoverIconRect.yMax) ) {
									
											if (dragStartedOverMaterial < Surforge.surforgeSettings.loadedMaterials.Length) {
												texturePreviewInfo = Surforge.surforgeSettings.loadedMaterials[dragStartedOverMaterial].name;

												GUI.Label(previewRect, texturePreviewInfo, texturePreviewInfoStyle); 
											}

										}
									}





								}
							}
						}
					}


					if (isMaterialPanelSideLayout) {
						GUILayout.BeginArea(new Rect(previewRect.xMax, 0, window.position.size.x - windowMinSizePlusOne - previewRect.width, window.position.size.y));
		
					}
					else {
						GUILayout.BeginArea(new Rect(windowMinSizePlusOne, windowSizeX + 8, windowSizeX, togglersPosY -windowSizeX));
					}


				
					GUILayout.BeginHorizontal();
					if(GUILayout.Button(new GUIContent(newMaterialIcon, "<b><color="+textHeaderColor+">New material set</color></b>"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) {
							Surforge.LogAction("New material set", "", "");
							RepaintSceneView();

							Material newMaterial = new Material( (Material)(AssetDatabase.LoadAssetAtPath("Assets/Surforge/ComposerPresets/newMaterialPreset.mat", typeof(Material))) );
							Surforge.surforgeSettings.newMaterialSetsCount++;
							newMaterial.name = "custom material set " + Surforge.surforgeSettings.newMaterialSetsCount.ToString();
							newMaterial.SetFloat("_ShowID", 0);
							Surforge.surforgeSettings.activeShowID = 0;
							Surforge.surforgeSettings.sceneMaterials.Add(newMaterial);
							Surforge.surforgeSettings.activeSceneMaterialNumber = Surforge.surforgeSettings.sceneMaterials.Count - 1;

							Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber];

							UpdateMaterialEditor();
						}
					}
					if(GUILayout.Button(new GUIContent(copyMaterialIcon, "<b><color="+textHeaderColor+">Duplicate material set</color></b>"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) {
							Surforge.LogAction("Duplicate material set", "", "");
							RepaintSceneView();

							Material newMaterial = new Material( (Material)(Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial) );
							Surforge.surforgeSettings.newMaterialSetsCount++;
							newMaterial.name = "custom material set " + Surforge.surforgeSettings.newMaterialSetsCount.ToString();
							newMaterial.SetFloat("_ShowID", Surforge.surforgeSettings.activeShowID);
							Surforge.surforgeSettings.sceneMaterials.Add(newMaterial);
							Surforge.surforgeSettings.activeSceneMaterialNumber = Surforge.surforgeSettings.sceneMaterials.Count - 1;
					
							Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber];

							UpdateMaterialEditor();
						}
					}
					if(GUILayout.Button(new GUIContent(saveMaterialPresetIcon, "<b><color="+textHeaderColor+">Save material set </color></b>\n\nSave custom material set to <color="+textHotkeyColor+">Assets/Surforge/CustomPresets/MaterialSets/</color> folder\n\nThe existing custom set with the same name will be updated\n\nSaved custom sets will appear next time along with default sets"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) { 
							Surforge.LogAction("Save material set", "", "");
							RepaintSceneView();

							if(!Directory.Exists("Assets/Surforge/CustomPresets/MaterialSets/")) {
								Directory.CreateDirectory("Assets/Surforge/CustomPresets/MaterialSets/");
							}
							Material newMaterialPreset = new Material(Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber]);
							newMaterialPreset.SetFloat("_ShowID", 11);
							newMaterialPreset.SetFloat("_ShowMaps", 0);

							if (Surforge.surforgeSettings.editorPreviewAoEdgesDirtDepth != null) newMaterialPreset.SetTexture("_AoEdgesDirtDepth", Surforge.surforgeSettings.editorPreviewAoEdgesDirtDepth);
							if (Surforge.surforgeSettings.editorPreviewMasks != null) newMaterialPreset.SetTexture("_ObjectMasks", Surforge.surforgeSettings.editorPreviewMasks);
							if (Surforge.surforgeSettings.editorPreviewMasks2 != null) newMaterialPreset.SetTexture("_ObjectMasks2", Surforge.surforgeSettings.editorPreviewMasks2);
							if (Surforge.surforgeSettings.renderMaterialIconNoise != null) newMaterialPreset.SetTexture("_EmissionMask", Surforge.surforgeSettings.renderMaterialIconNoise);

							AssetDatabase.CreateAsset(newMaterialPreset, "Assets/Surforge/CustomPresets/MaterialSets/" + Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber].name + ".mat");
						}
					}

					if(GUILayout.Button(new GUIContent(deleteMaterialIcon, "<b><color="+textHeaderColor+">Delete material set</color></b>\n\nDelete material set from current scene.\n\nThis does not remove default or saved custom presets from the disk. They will appear again \nin the new scene."), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) {
							Surforge.LogAction("Delete material set", "", "");
							RepaintSceneView();

							if (Surforge.surforgeSettings.sceneMaterials.Count > 1) {
								int materialToRemoveNumber = Surforge.surforgeSettings.activeSceneMaterialNumber;

								DestroyImmediate(Surforge.surforgeSettings.sceneMaterials[materialToRemoveNumber]);
								Surforge.surforgeSettings.sceneMaterials.RemoveAt(materialToRemoveNumber);

								if (Surforge.surforgeSettings.activeSceneMaterialNumber >  (Surforge.surforgeSettings.sceneMaterials.Count -1)) {
									Surforge.surforgeSettings.activeSceneMaterialNumber = Surforge.surforgeSettings.sceneMaterials.Count -1;
								}

								Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber];

								UpdateMaterialEditor();

							}
						}
					}


					if (!isMaterialRenamingNow) {
						GUIContent[] sceneMaterialNames = new GUIContent[Surforge.surforgeSettings.sceneMaterials.Count];
						for (int i=0; i < Surforge.surforgeSettings.sceneMaterials.Count; i++) {
							GUIContent content = new GUIContent();
							content.text = Surforge.surforgeSettings.sceneMaterials[i].name;
							content.tooltip = materialSelectionTooltip;
							sceneMaterialNames[i] = content;
						}
						int oldMaterialNumber = Surforge.surforgeSettings.activeSceneMaterialNumber;

						Rect materialSelectionRect  = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.MinWidth(80)} );
						Surforge.surforgeSettings.activeSceneMaterialNumber = EditorGUI.Popup(materialSelectionRect, Surforge.surforgeSettings.activeSceneMaterialNumber, sceneMaterialNames);

						if ( Event.current.type == EventType.ScrollWheel) {
							Vector2 mousePosMaterial = Event.current.mousePosition;
							if ((mousePosMaterial.x > materialSelectionRect.xMin) && (mousePosMaterial.y > (materialSelectionRect.yMin - 5)) && (mousePosMaterial.x < materialSelectionRect.xMax)&& (mousePosMaterial.y < (materialSelectionRect.yMax + 5))) {
								if (Event.current.delta.y > 0) {
									if (Surforge.surforgeSettings.activeSceneMaterialNumber < (Surforge.surforgeSettings.sceneMaterials.Count - 1)) {
										Surforge.surforgeSettings.activeSceneMaterialNumber = Surforge.surforgeSettings.activeSceneMaterialNumber + 1;
									}
								}
								if (Event.current.delta.y < 0) {
									if (Surforge.surforgeSettings.activeSceneMaterialNumber > 0) {
										Surforge.surforgeSettings.activeSceneMaterialNumber = Surforge.surforgeSettings.activeSceneMaterialNumber - 1;
									}
								}
							}
						}


						if (oldMaterialNumber != Surforge.surforgeSettings.activeSceneMaterialNumber) {
							Surforge.LogAction("Select material set", "Left Click", "Scroll");
							Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
							if (r) {
								r.sharedMaterial = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber];
								r.sharedMaterial.SetFloat("_ShowID", Surforge.surforgeSettings.activeShowID);

								UpdateMaterialEditor();
								isScrollNeedUpdate = true;
							}
						}


						if ((Event.current.type == EventType.MouseUp) && ( Event.current.button == 1)) {
							if (!isMaterialRenamingNow) {  
								Surforge.LogAction("Rename material set", "Right Click", "");
								if (isMaterialPanelSideLayout) {
									renamingMaterialRect = new Rect(materialSelectionRect.x + windowMinSizePlusOne + previewRect.width, materialSelectionRect.y, materialSelectionRect.width , materialSelectionRect.height);
								}
								else {
									renamingMaterialRect = new Rect(materialSelectionRect.x + windowMinSizePlusOne, materialSelectionRect.y + windowSizeX + 8, materialSelectionRect.width , materialSelectionRect.height);
								}
								Vector2 mousePosMaterial = Event.current.mousePosition;
								if ((mousePosMaterial.x > materialSelectionRect.xMin) && (mousePosMaterial.y > (materialSelectionRect.yMin - 5)) && (mousePosMaterial.x < materialSelectionRect.xMax)&& (mousePosMaterial.y < (materialSelectionRect.yMax + 5))) {
									isMaterialRenamingNow = true;
									window.Repaint();
								}
							}
						}


					}
					else { //renaming material
						Rect materialSelectionRect  = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.MinWidth(80)} );
						Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber].name = GUI.TextField(materialSelectionRect, Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber].name);

						CheckMaterialNameUniq();

						if ( (Event.current.type == EventType.KeyUp) &&  ((Event.current.keyCode == KeyCode.KeypadEnter) ||(Event.current.keyCode == KeyCode.Return)) ){
							isMaterialRenamingNow = false;
							window.Repaint();
						}
					}


					if(GUILayout.Button(new GUIContent(swapMaterialsIcon, "<b><color="+textHeaderColor+">Swap materials </color></b>\n\nSwap materials in this set randomly"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) {
							Surforge.LogAction("Swap materials", "", "");
							RepaintSceneView();

							StoreGroups();
							SwapGroups();
							SetGroups();
						}
					}

					if(GUILayout.Button(new GUIContent(randomizeMaterialIcon, "<b><color="+textHeaderColor+">Randomize materials </color></b>\n\nAssign random material from random set for every material in this set"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) {
							Surforge.LogAction("Randomize materials", "", "");
							RepaintSceneView();

							for (int i=0; i < 8; i++) {
								SetMaterialFromRandomPresetToGroup(i);
							}
						}
					}
				
					GUILayout.EndHorizontal();

					EditorGUILayout.Separator(); 

					GUILayout.BeginHorizontal();

					float oldMaterialId = Surforge.surforgeSettings.activeShowID;

					Rect matIdSelectionRect  = EditorGUILayout.GetControlRect(new GUILayoutOption[] { GUILayout.MinWidth(0)} );
					if (EditorGUIUtility.isProSkin) {
						Surforge.surforgeSettings.activeShowID = EditorGUI.Popup(matIdSelectionRect, (int)Surforge.surforgeSettings.activeShowID, matIDs, buttonStyleMiniText);   
					}
					else {
						Surforge.surforgeSettings.activeShowID = EditorGUI.Popup(matIdSelectionRect, (int)Surforge.surforgeSettings.activeShowID, matIDs, buttonStyleMiniTextLite);   
					}


					if ( Event.current.type == EventType.ScrollWheel) {
						Vector2 mousePosMatID = Event.current.mousePosition;
						if ((mousePosMatID.x > matIdSelectionRect.xMin) && (mousePosMatID.y > (matIdSelectionRect.yMin - 2)) && (mousePosMatID.x < matIdSelectionRect.xMax)&& (mousePosMatID.y < (matIdSelectionRect.yMax + 2))) {
							if (Event.current.delta.y > 0) {
								if (Surforge.surforgeSettings.activeShowID < 10) {
									Surforge.surforgeSettings.activeShowID = Surforge.surforgeSettings.activeShowID + 1;
								}
							}
							if (Event.current.delta.y < 0) {
								if (Surforge.surforgeSettings.activeShowID > 0) {
									Surforge.surforgeSettings.activeShowID = Surforge.surforgeSettings.activeShowID - 1;
								}
							}
						}
					}     
					if (oldMaterialId != Surforge.surforgeSettings.activeShowID) {
						Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
						if (r) {
							r.sharedMaterial.SetFloat("_ShowID", Surforge.surforgeSettings.activeShowID );
							window.Repaint();
							isScrollNeedUpdate = true;
						}
					}


					if(GUILayout.Button(new GUIContent(saveMaterialPresetIcon, "<b><color="+textHeaderColor+">Save current material</color></b>\n\nSave current material to <color="+textHotkeyColor+">Assets/Surforge/CustomPresets/Materials/</color> folder\n\nSaved materials will appear next time along with default materials"), buttonStyleMini ) ) {
						if (!isMaterialRenamingNow) { 
							Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
							if (r) {
								if(!Directory.Exists("Assets/Surforge/CustomPresets/Materials/")) {
									Directory.CreateDirectory("Assets/Surforge/CustomPresets/Materials/");
								}
								Material newMaterial = new Material(Surforge.surforgeSettings.renderMaterialIconShader);
								Surforge.CopyMaterialToChosenID(r.sharedMaterial, (int)Surforge.surforgeSettings.activeShowID, newMaterial, 0); 
								newMaterial.SetFloat("_ShowID", 11); 
								AssetDatabase.CreateAsset(newMaterial, "Assets/Surforge/CustomPresets/Materials/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mat");
							}
						}
					}

					if (Event.current.type == EventType.Layout) {   
						Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
						if (r) { 
							r.sharedMaterial.SetFloat("_ShowID", (float)Surforge.surforgeSettings.activeShowID );
						}
					}

					GUILayout.EndHorizontal();
					EditorGUILayout.Separator();
					EditorGUILayout.Separator();

					if (!isScrollNeedUpdate) {
						if (isMaterialPanelSideLayout) {
							scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(window.position.size.x - windowMinSizePlusOne - previewRect.width), GUILayout.Height(window.position.size.y -40));
						}
						else {
							scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowSizeX), GUILayout.Height(togglersPosY-windowSizeX -50));
						}


						materialEditor.PropertiesGUI();


						EditorGUILayout.Separator();
				
						GUILayout.EndScrollView();
					}
				
					GUILayout.EndArea();

					
					
					if ((activeTool == 1) && (materialDragNow)) {
						Surforge.LogAction("Material drag-and-drop", "", "");
						DrawMaterialDragDropLine(); 

						Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
						    
						if (r) {
							RevertToTempMaterial(r.sharedMaterial);

							if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
								int materialUnderCursor = GetMaterialNumberUnderCursor(mousePos, previewRect);

								if (materialUnderCursor != 0) {
									Surforge.CopyMaterialToChosenID(Surforge.surforgeSettings.loadedMaterials[dragStartedOverMaterial], 0, r.sharedMaterial, materialUnderCursor - 1);
								}
							}
							r.enabled = true; //force material to update
						}
					}

					if (Event.current.type == EventType.MouseDown) {
						if ( (mousePos.y < (window.position.size.y - 70)) && (mousePos.y > 215 ) ) { //check if mouse not outside materials list
							if ( (mousePos.x > hoverIconRect.xMin) && (mousePos.y > hoverIconRect.yMin) && (mousePos.x < hoverIconRect.xMax)&& (mousePos.y < hoverIconRect.yMax) ) {
								materialDragNow = true;

								//MeshCollider update for asset scale reimport fix
								MeshFilter composerMeshFilter = (MeshFilter)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();
								MeshCollider composerMeshCollider = (MeshCollider)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
								composerMeshCollider.sharedMesh = null;
								composerMeshCollider.sharedMesh = composerMeshFilter.sharedMesh;

								Undo.RegisterCompleteObjectUndo(Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber], "material drag");

								Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
								if (r) {
									tempMaterial = Material.Instantiate(r.sharedMaterial);
								}
							}
						}
					}
					if (Event.current.type == EventType.MouseUp) {
						if (materialDragNow) {
							if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
								int materialUnderCursor = GetMaterialNumberUnderCursor(mousePos, previewRect);

								if (materialUnderCursor != 0) {
									Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
									if (r) {
										//Assign material
										//Surforge.CopyMaterialToChosenID(Surforge.surforgeSettings.loadedMaterials[dragStartedOverMaterial], 0, r.sharedMaterial, materialUnderCursor - 1);
									}

								}
							}
							else {
								Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
								if (r) {
									RevertToTempMaterial(r.sharedMaterial);
								}
							}
						}
						materialDragNow = false;
					}

				
				}
			


				if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
					int materialUnderCursor = GetMaterialNumberUnderCursor(mousePos, previewRect);

					if ( (Event.current.type == EventType.KeyUp ) && (Event.current.keyCode == KeyCode.C) ) {
						if (materialUnderCursor != 0) { 
							Surforge.LogAction("Copy material under cursor", "Ctrl + C", "");

							CopyMaterial(materialUnderCursor-1);
							window.Repaint();
						}
					}
					if ( (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.V) ) {
						if (materialUnderCursor != 0) {
							if (bufferMaterialGroup != null) {
								Surforge.LogAction("Paste material under cursor", "Ctrl + V", "");

								PasteMaterial(materialUnderCursor-1);
								window.Repaint();
							}
						}
					}
					if ( (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.X) ) {
						if (materialUnderCursor != 0) {
							if (bufferMaterialGroup != null) {
								Surforge.LogAction("Swap material under cursor", "Ctrl + X", "");
							
								SwapMaterial(materialUnderCursor-1);
								window.Repaint();
							}
						}
					}



					if (Event.current.control) {
						if (materialUnderCursor != 0) {
							Surforge.LogAction("Select material under cursor", "Ctrl", "");

							GUI.Label(new Rect(mousePos.x, mousePos.y, 80, 50), "  Material " + materialUnderCursor.ToString(), EditorStyles.toolbarButton);
							Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
							if (r) { 
								Surforge.surforgeSettings.activeShowID = (float)(materialUnderCursor - 1);
							}

						}
					}


					//shader mode
					if ( (Event.current.type == EventType.MouseDown) && (Event.current.button == 1) ) {
						rightButtonDownPos = mousePos;
					}
					if ( (Event.current.type == EventType.MouseUp) && (Event.current.button == 1) && (!Event.current.control) && (!Event.current.alt) && (!Event.current.shift) ) { ///
						if (mousePos == rightButtonDownPos) {
							GenericMenu menu = new GenericMenu();

							menu.AddItem(new GUIContent("Material"), shaderMode == 0 ? true : false, SetShaderMode, 0);
							menu.AddSeparator("");
							menu.AddItem(new GUIContent("maps"), shaderMode == 9 ? true : false, SetShaderMode, 9);
							menu.AddSeparator("");

							menu.AddItem(new GUIContent("albedo"), shaderMode == 1 ? true : false, SetShaderMode, 1);
							menu.AddItem(new GUIContent("occlusion"), shaderMode == 2 ? true : false, SetShaderMode, 2);
							menu.AddItem(new GUIContent("specular"), shaderMode == 3 ? true : false, SetShaderMode, 3);
							menu.AddItem(new GUIContent("glossiness"), shaderMode == 4 ? true : false, SetShaderMode, 4);
							menu.AddItem(new GUIContent("normal"), shaderMode == 5 ? true : false, SetShaderMode, 5);
							menu.AddItem(new GUIContent("emission"), shaderMode == 6 ? true : false, SetShaderMode, 6);
							//menu.AddItem(new GUIContent("transparency"), shaderMode == 7 ? true : false, SetShaderMode, 7);
							menu.AddItem(new GUIContent("height"), shaderMode == 8 ? true : false, SetShaderMode, 8);

							menu.ShowAsContext();
						}

					}

				


					if (( Event.current.type == EventType.ScrollWheel) &&  (Event.current.control)) {
						Surforge.LogAction("Cycle material under cursor", "Ctrl + Scroll", "");
						RepaintSceneView();

						if (!isCyclyngMaterialsUnderCursor) {
							if (materialUnderCursor != 0) {
								isCyclyngMaterialsUnderCursor = true;
								materialCycleStartSetNum = Surforge.surforgeSettings.activeSceneMaterialNumber;
								materialCycleStartMaterialNum = materialUnderCursor;
								materialCycleCount = 0;
							}
						}

						if (Event.current.delta.y > 0) {
							materialCycleCount++;
							if (materialCycleCount >= (Surforge.surforgeSettings.sceneMaterials.Count * 8)) materialCycleCount = 0;

							if (materialUnderCursor != 0) { 
								SetMaterialUnderCursor(materialCycleStartSetNum, materialCycleStartMaterialNum, materialCycleCount, materialUnderCursor);
								window.Repaint();
							}
						}
						if (Event.current.delta.y < 0) {
							materialCycleCount--;
							if (materialCycleCount < 0) materialCycleCount = Surforge.surforgeSettings.sceneMaterials.Count * 8;

							if (materialUnderCursor != 0) { 
								SetMaterialUnderCursor(materialCycleStartSetNum, materialCycleStartMaterialNum, materialCycleCount, materialUnderCursor);
								window.Repaint();
							}
						}
					}


					if (( Event.current.type == EventType.ScrollWheel) &&  (Event.current.shift)) {
						Surforge.LogAction("Cycle dirts", "Shift + Scroll", "");
						RepaintSceneView();

						if (!isCyclyngDirts) {
							if (materialUnderCursor != 0) {
								isCyclyngDirts = true;
								dirtCycleStartSetNum = Surforge.surforgeSettings.activeSceneMaterialNumber;
								dirtCycleCount = 0;
							}
						}

						if (Event.current.delta.y > 0) {
							dirtCycleCount++;
							if (dirtCycleCount >= Surforge.surforgeSettings.sceneMaterials.Count) dirtCycleCount = 0;

							if (materialUnderCursor != 0) { 
								SetCycledDirt(dirtCycleStartSetNum, dirtCycleCount);
								window.Repaint();
							}
						}
						if (Event.current.delta.y < 0) {
							dirtCycleCount--;
							if (dirtCycleCount < 0) dirtCycleCount = Surforge.surforgeSettings.sceneMaterials.Count;

							if (materialUnderCursor != 0) {
								SetCycledDirt(dirtCycleStartSetNum, dirtCycleCount);
								window.Repaint();
							}
						}

					}


				
				}
		


				//---- drop materials from Unity Editor to Surforge Texture Preview
				if (DragAndDrop.objectReferences.Length > 0) {
					if (CheckMaterialDragDropShaderMatch(DragAndDrop.objectReferences[0])) {
			
						switch (evt.type) {
						case EventType.DragUpdated:
						
							if (materialFromProjectDraggingNow) {
								Surforge.LogAction("Material drag-and-drop", "", "");
								
								Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
								if (r) {
									RevertToTempMaterial(r.sharedMaterial);
									
									if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
										DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
										int materialUnderCursor = GetMaterialNumberUnderCursor(mousePos, previewRect);
										
										if (materialUnderCursor != 0) {
											Material dropMat = (Material)DragAndDrop.objectReferences[0];
											if (dropMat.shader == Shader.Find("Hidden/Composer")) {
												CopyMaterialSet((Material)DragAndDrop.objectReferences[0], r.sharedMaterial);
											}
											else {
												dragDropFromProjectMat = CreateSurforgeMaterialFromProjectMaterial((Material)DragAndDrop.objectReferences[0]);
												Surforge.CopyMaterialToChosenID(dragDropFromProjectMat, 0, r.sharedMaterial, materialUnderCursor - 1);
											}
										}
									}
									r.enabled = true; //force material to update
								}
								
							}

							//Drag from Editor just started
							if (!materialFromProjectDraggingNow) {

								materialFromProjectDraggingNow = true;

								//MeshCollider update for asset scale reimport fix
								MeshFilter composerMeshFilter = (MeshFilter)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();
								MeshCollider composerMeshCollider = (MeshCollider)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
								composerMeshCollider.sharedMesh = null;
								composerMeshCollider.sharedMesh = composerMeshFilter.sharedMesh;

								Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
								if (r) {
									tempMaterial = Material.Instantiate(r.sharedMaterial);
								}
							}



							evt.Use();
							break; 

						case EventType.DragPerform:

							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
							if (evt.type == EventType.DragPerform) {  
								DragAndDrop.AcceptDrag (); 

								if (materialFromProjectDraggingNow) {
									if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
										Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
										if (r) {
											RevertToTempMaterial(r.sharedMaterial);

											Undo.RegisterCompleteObjectUndo(Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber], "material drag");
											
											int materialUnderCursor = GetMaterialNumberUnderCursor(mousePos, previewRect);
											
											if (materialUnderCursor != 0) {
												Material dropMat = (Material)DragAndDrop.objectReferences[0];
												if (dropMat.shader == Shader.Find("Hidden/Composer")) {
													CopyMaterialSet((Material)DragAndDrop.objectReferences[0], r.sharedMaterial);
												}
												else {
													dragDropFromProjectMat = CreateSurforgeMaterialFromProjectMaterial((Material)DragAndDrop.objectReferences[0]);
													Surforge.CopyMaterialToChosenID(dragDropFromProjectMat, 0, r.sharedMaterial, materialUnderCursor - 1);
												}
											}
											r.enabled = true; //force material to update
										}


									}
									else {
										Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
										if (r) {
											RevertToTempMaterial(r.sharedMaterial);
										}
									}
								}
								materialFromProjectDraggingNow = false;

							}
							evt.Use();
							break;
						
						case EventType.DragExited:

							if (evt.type == EventType.DragExited) {
								if ( (mousePos.x > previewRect.xMin) && (mousePos.y > previewRect.yMin) && (mousePos.x < previewRect.xMax)&& (mousePos.y < previewRect.yMax) ) {
								}
								else {
									if (materialFromProjectDraggingNow) {

										Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
										if (r) {
											RevertToTempMaterial(r.sharedMaterial);
										}
									}
								}
								materialFromProjectDraggingNow = false;
							}
							break;



						}


					}
				}
		
				//----

				Renderer composerRenderer = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
				if (composerRenderer != null) {
					composerRenderer.sharedMaterial.SetFloat("_ShowMaps", shaderMode);
				}


				if ( (Event.current.type == EventType.KeyUp) && ((Event.current.keyCode == KeyCode.LeftControl)||(Event.current.keyCode == KeyCode.RightControl)) ) {
					isCyclyngMaterialsUnderCursor = false;
				}
				if ( (Event.current.type == EventType.KeyUp) && ((Event.current.keyCode == KeyCode.LeftShift)||(Event.current.keyCode == KeyCode.RightShift)) ) {
					isCyclyngDirts = false;
				}

			}

			//surforge scene not loaded - show selcome screen
			else {


				//Texture2D assets not kept with window not closed on previous editor exit so reload if needed
				if (logoBackgroundTexture == null) logoBackgroundTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/logoBackground.psd", typeof(Texture2D));
				if (logoBackgroundTextureLite == null) logoBackgroundTextureLite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/logoBackgroundLite.psd", typeof(Texture2D));

				if (versionGuiStyle == null) {
					versionGuiStyle = new GUIStyle();
					versionGuiStyle.normal.textColor = new Color(1, 0.658f, 0, 1);
					versionGuiStyle.fontStyle = FontStyle.Bold;
				}
				if (versionGuiStyleLite == null) {
					versionGuiStyleLite = new GUIStyle();
					versionGuiStyleLite.normal.textColor = new Color(1, 0.658f, 0, 1);
					versionGuiStyleLite.fontStyle = FontStyle.Bold;
				}

				if (logoGuiStyle == null) {
					logoGuiStyle = new GUIStyle();
					logoGuiStyle.fontSize = 0;
					logoGuiStyle.alignment = TextAnchor.UpperLeft;
					logoGuiStyle.fixedWidth = 0;
					logoGuiStyle.fixedHeight = 0;
					logoGuiStyle.margin = new RectOffset(4,4,2,2);
					logoGuiStyle.padding.top = 1;
					logoGuiStyle.padding.bottom = 2;
					logoGuiStyle.padding.left = 2;
					logoGuiStyle.padding.right = 2;
				}


				if (logoBackgroundStyle == null) {
					logoBackgroundStyle = new GUIStyle();
					logoBackgroundStyle.normal.background = logoBackgroundTexture;
				}
				if (logoBackgroundStyleLite == null) {
					logoBackgroundStyleLite = new GUIStyle();
					logoBackgroundStyleLite.normal.background = logoBackgroundTextureLite;
				}
				if (startScreenButton == null) {
					startScreenButton = new GUIStyle(); 
				}


				if (EditorGUIUtility.isProSkin) {
					GUILayout.BeginHorizontal(logoBackgroundStyle);
				}
				else {
					GUILayout.BeginHorizontal(logoBackgroundStyleLite);
				}


				EditorGUIUtility.labelWidth = 289;

				Rect versionPos = new Rect(200, 60, 100, 100);

				if (EditorGUIUtility.isProSkin) {
					GUILayout.Label(surforgeLogo, logoGuiStyle);
					GUI.Label(versionPos, version, versionGuiStyle);
				}
				else {
					GUILayout.Label(surforgeLogoLite, logoGuiStyle);
					GUI.Label(versionPos, version, versionGuiStyleLite);
				}


				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();


				int hoverMenuPosition = 1;
				Vector2 menuHoverMousePos = Event.current.mousePosition;
				if (menuHoverMousePos.y > (88.0f + 57.0f)) hoverMenuPosition = 2;
				if (menuHoverMousePos.y > (88.0f + 57.0f * 2)) hoverMenuPosition = 3;
				if (menuHoverMousePos.y > (88.0f + 57.0f * 3)) hoverMenuPosition = 4;
				
				
				
				if ((menuHoverMousePos.y < 88.0f) || (menuHoverMousePos.y > (88.0f + 57.0f * 4))) hoverMenuPosition = 0;
				if ((menuHoverMousePos.x < 0) || (menuHoverMousePos.x > 235.0f) || (menuHoverMousePos.x > window.position.size.x)) hoverMenuPosition = 0;
				
				DrawStartMenuHover(hoverMenuPosition);


				EditorGUILayout.Separator();

				GUILayout.BeginHorizontal();

				if (EditorGUIUtility.isProSkin) {
					if (GUILayout.Button(startScreenNewTexture, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						SurforgeNewScene();
					}
				}
				else {
					if (GUILayout.Button(startScreenNewTextureLite, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						SurforgeNewScene();
					}
				}

				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();



				GUILayout.BeginHorizontal();

				string surforgeUserGuideLink = "http://surforge.com/assets/userGuide/SurforgeUserGuide.pdf";
				if (EditorGUIUtility.isProSkin) {
					if (GUILayout.Button(startScreenUserGuide, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						Application.OpenURL(surforgeUserGuideLink);
					}
				}
				else {
					if (GUILayout.Button(startScreenUserGuideLite, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						Application.OpenURL(surforgeUserGuideLink);
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();


				
				GUILayout.BeginHorizontal();

				string surforgeVideosLink = "http://surforge.com/index.php?id=4";
				if (EditorGUIUtility.isProSkin) {
					if (GUILayout.Button(startScreenVideos, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						Application.OpenURL(surforgeVideosLink);
					}
				}
				else {
					if (GUILayout.Button(startScreenVideosLite, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						Application.OpenURL(surforgeVideosLink);
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				
				GUILayout.BeginHorizontal();

				if (EditorGUIUtility.isProSkin) {
					if (GUILayout.Button(startScreenAbout, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						ShowSurforgeAbout();
					}
				}
				else {
					if (GUILayout.Button(startScreenAboutLite, startScreenButton, GUILayout.Width(253), GUILayout.Height(57))) {
						ShowSurforgeAbout();
					}
				}


				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();


				if (window) window.Repaint(); 
				

			}

		}

	} 


	void SetShaderMode(object shaderModeToSet) {
		shaderMode = (int)shaderModeToSet;
	}
	
	/*
	static Material texturePreviewGuiMaterial;
	
	static void CreateTexturePreviewGuiMaterial(RenderTexture renderTexture) {
		
		if (!texturePreviewGuiMaterial) {
			Shader shader = Shader.Find ("Unlit/Texture");
			texturePreviewGuiMaterial = new Material (shader);
			texturePreviewGuiMaterial.hideFlags = HideFlags.HideAndDontSave;
			texturePreviewGuiMaterial.SetTexture("_MainTex", renderTexture);
		}
		
	}

	
	static void DrawTexturedRectangle (Rect position, RenderTexture renderTexture) {
		bool isLinearColorSpace = false;
		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			isLinearColorSpace = true;
		}

		CreateTexturePreviewGuiMaterial(renderTexture);
		texturePreviewGuiMaterial.SetTexture("_MainTex", renderTexture); //TODO: not set texture every frame
		
		texturePreviewGuiMaterial.SetPass (0);

		if (isLinearColorSpace) GL.sRGBWrite = true;

		GL.Begin (GL.QUADS);
		GL.TexCoord2(0, 1);
		GL.Vertex3 (position.x, position.y, 0);

		GL.TexCoord2(1, 1);
		GL.Vertex3 (position.x + position.width, position.y, 0);

		GL.TexCoord2(1, 0);
		GL.Vertex3 (position.x + position.width, position.y + position.height, 0);

		GL.TexCoord2(0, 0);
		GL.Vertex3 (position.x, position.y + position.height, 0);
		GL.End (); 

		GL.sRGBWrite = false;
	}
	*/

	

	static void DrawLayerHeightControls() {
		EditorGUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(21) } );
		
		if(GUILayout.Button(new GUIContent(upperLayerIcon, "Move construction plane Up"), layerHeightButtonStyle ) ) {
			Surforge.LogAction("Move construction plane up", "", "");

			if (Surforge.surforgeSettings.limits == null) {
				Surforge.CreateLimits();
			}
			Surforge.surforgeSettings.limits.minY = Surforge.surforgeSettings.limits.minY + 0.5f;
			Surforge.surforgeSettings.limits.maxY = Surforge.surforgeSettings.limits.maxY + 0.5f;
			RepaintSceneView();
		}
		float layerHeight = 0;
		if (Surforge.surforgeSettings.limits != null) {
			layerHeight = (Surforge.surforgeSettings.limits.minY + 0.25f) / 0.5f;
		}
		GUILayout.Label(new GUIContent(layerHeight.ToString(), "Construction plane layer"), layerHeightStyle);
		if(GUILayout.Button(new GUIContent(bottomLayerIcon, "Move construction plane Down"), layerHeightButtonStyle ) ) {
			Surforge.LogAction("Move construction plane down", "", "");

			if (Surforge.surforgeSettings.limits == null) {
				Surforge.CreateLimits();
			}
			Surforge.surforgeSettings.limits.minY = Surforge.surforgeSettings.limits.minY - 0.5f;
			Surforge.surforgeSettings.limits.maxY = Surforge.surforgeSettings.limits.maxY - 0.5f;
			RepaintSceneView();
		}

		EditorGUILayout.EndVertical();
	}

	static void UpdateMaterialEditor() {
		if (materialEditor != null) DestroyImmediate (materialEditor);
		materialEditor = (MaterialEditor)Editor.CreateEditor (Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial);
		window.Repaint();
	}


	static void SetPolyLassoProfileToSelectedObjects(int profileNum) {
		Surforge.LogAction("Profile change", "", "");

		Object[] newSelection = new Object[Selection.objects.Length];

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				PolyLassoProfile profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[profileNum];

				List<Vector3> shape = Surforge.PolyLassoObjectToWorldShape(polyLassoObject);


				//set size 
				Transform pObjParent = polyLassoObject.gameObject.transform.parent;  
				polyLassoObject.gameObject.transform.parent = null;  
				Vector3 objLocalScale = polyLassoObject.gameObject.transform.localScale;
				Vector3 objLocalPosition = polyLassoObject.gameObject.transform.localPosition;
				Quaternion objLocalRotation = polyLassoObject.gameObject.transform.localRotation;
				
				GameObject relativeTransforms = new GameObject();
				relativeTransforms.transform.parent = polyLassoObject.transform;
				
				polyLassoObject.transform.localScale = Vector3.one;
				polyLassoObject.transform.localPosition = Vector3.zero;
				polyLassoObject.transform.localRotation = Quaternion.identity;
				
				shape = Surforge.PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, shape);


				float polyLassoObjectScale = polyLassoObject.gameObject.transform.localScale.x;
				if (polyLassoObject.gameObject.transform.localScale.y < polyLassoObjectScale) polyLassoObjectScale = polyLassoObject.gameObject.transform.localScale.y;
				if (polyLassoObject.gameObject.transform.localScale.z < polyLassoObjectScale) polyLassoObjectScale = polyLassoObject.gameObject.transform.localScale.z;

				if (!PolyLassoDensityDialog(shape, profile, polyLassoObjectScale)) {
					profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[0];
				}


				GameObject newObj =	Surforge.PolygonLassoBuildObject(null, false, shape, profile.bevelAmount, profile.bevelSteps, profile.offsets, profile.heights,
				                                polyLassoObject.decalSets, 
				                                polyLassoObject.inheritMatGroup,
				                                polyLassoObject.scatterOnShapeVerts, 
				                                polyLassoObject.trim,
				                                polyLassoObject.perpTrim,
				                                polyLassoObject.fitDecals,
				                                polyLassoObject.trimOffset,
				                                polyLassoObject.decalOffset, 
				                                polyLassoObject.decalOffsetRandom,
				                                polyLassoObject.decalGap, 
				                                polyLassoObject.decalGapRandom, 
				                                polyLassoObject.decalSize, 
				                                polyLassoObject.decalSizeRandom, 
				                                polyLassoObject.decalRotation, 
				                                polyLassoObject.decalRotationRandom,
				                                polyLassoObject.noise,
				                                polyLassoObject.shapeSubdiv,
				                                polyLassoObject.noise1Amount,
				                                polyLassoObject.noise1Frequency,
				                                polyLassoObject.noise2Amount,
				                                polyLassoObject.noise2Frequency,
				                                polyLassoObject.materialID,
				                                profile.isFloater,
				                                profile.isTube,
				                                profile.isOpen,
				                                profile.thickness,
				                                profile.section,
				                                profile.isAdaptive,
				                                profile.adaptiveStep,
				                                profile.lengthOffsets0,
				                                profile.lengthOffsets1,
				                                profile.lengthOffsets2,
				                                profile.heightOffsets0,
				                                profile.heightOffsets1,
				                                profile.heightOffsets2,
				                                profile.repeatSize,
				                                profile.lengthOffsetOrder,
				                                profile.heightOffsetOrder,
				                                profile.edgeWiseOffset,
				                                profile.lengthWiseOffset,
				                                profile.offsetMinEdge,
				                                profile.corner,
				                                profile.childProfileVerticalOffsets, 
				                                profile.childProfileDepthOffsets,
				                                profile.childProfileHorisontalOffsets,
				                                profile.childProfileMatGroups,
				                                profile.childProfileShapes,
				                                profile.followerProfiles, 
				                                profile.followerProfileVerticalOffsets, 
				                                profile.followerProfileDepthOffsets,
				                                profile.followerProfileMatGroups,
				                                profile.cutoff, 
				                                profile.cutoffTiling,
				                                profile.bumpMap, 
				                                profile.bumpMapIntensity, 
				                                profile.bumpMapTiling,
				                                profile.aoMap,
				                                profile.aoMapIntensity,
				                                profile.randomUvOffset,
				                                profile.stoneType,
				                                profile.allowIntersections,
				                                profile.overlapIntersections,
				                                profile.overlapAmount,
				                                profile.usedForOverlapping, 
				                                profile.overlapStartInvert, 
				                                profile.curveUVs ); 


				//set size
				newObj.transform.parent = null;
				newObj.transform.localScale = objLocalScale;
				newObj.transform.localPosition = objLocalPosition;
				newObj.transform.localRotation = objLocalRotation;
				newObj.transform.parent = Surforge.surforgeSettings.root.transform;

				newSelection[i] = newObj;

				polyLassoObject.transform.localScale = objLocalScale;
				polyLassoObject.transform.localPosition = objLocalPosition;
				polyLassoObject.transform.localRotation = objLocalRotation;
				polyLassoObject.transform.parent = pObjParent;

				
				if (Surforge.surforgeSettings.seamless) {
					Surforge.RemoveSeamlessInstances(polyLassoObject);
					polyLassoObject.deleting = true;
				}
				
				//snap to objects
				if (Surforge.surforgeSettings.polyLassoObjects == null) {
					Surforge.surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
				}
				else {
					Surforge.surforgeSettings.polyLassoObjects.Remove(polyLassoObject);
				}	
				
				Undo.DestroyObjectImmediate(polyLassoObject.gameObject);

			}

			else {
				newSelection[i] = gameObjects[i];
			}
		}

		Selection.objects = newSelection;
	}
	
	
	static Vector3 limitsPos;
	static int sceneCameraDirection;
	


	static float rootScaleX = 1.0f;
	static float rootScaleZ = 1.0f;
	static float snapRangeOffset = 1.0f; 

	static void GetRootScale() {
		rootScaleX =  Surforge.surforgeSettings.root.transform.localScale.x;
		rootScaleZ =  Surforge.surforgeSettings.root.transform.localScale.z;
		float snapRangeOffset = 1.0f; 
		if (rootScaleX > rootScaleZ) {
			snapRangeOffset = snapRangeOffset * rootScaleX;
		}
		else {
			snapRangeOffset = snapRangeOffset * rootScaleZ;
		}
	}



	private static void SurforgeOnScene(SceneView sceneview) {
		//Debug.Log (SceneView.sceneViews.Count);


		if (SceneView.sceneViews.Count > 1) {
			if (SceneView.mouseOverWindow != sceneview) {
				return;
			}
		}

	

		if (Surforge.surforgeSettings) {
			//check for new model selected
			if (Surforge.surforgeSettings.extentTexturePreview != null) {
				if (Surforge.surforgeSettings.extentTexturePreview.composer != null) {
					if (Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter != null) {
						if (Surforge.surforgeSettings.model != null) {
							if (Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh != Surforge.surforgeSettings.model) {
								Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh = Surforge.surforgeSettings.model;
								MeshCollider meshCollider = (MeshCollider)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
								meshCollider.sharedMesh = Surforge.surforgeSettings.model;
							}
						}
						else {
							Surforge.surforgeSettings.model = Surforge.surforgeSettings.cubePreviewModel;
							Surforge.surforgeSettings.extentTexturePreview.composer.meshFilter.sharedMesh = Surforge.surforgeSettings.cubePreviewModel;
							MeshCollider meshCollider = (MeshCollider)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshCollider>();
							meshCollider.sharedMesh = Surforge.surforgeSettings.model;
						}
					}
				}
			}

			GetRootScale();

			if (Surforge.surforgeSettings.seamlessNeedUpdate && Surforge.surforgeSettings.seamless) {
				Surforge.ResetAllSeamlessInstances();
				Surforge.surforgeSettings.seamlessNeedUpdate = false;
			}
		

			sceneCameraDirection = GetSceneCameraDirection();
					
			if (showOctreeGrid) ShowGrid();
		
			if (Surforge.surforgeSettings.extentTexturePreview != null) {
				if (Surforge.surforgeSettings.extentTexturePreview.composer != null) {
				
					if (Event.current.type == EventType.Repaint) {  
						uvsFound = false;
					
						CheckUVsChanged();
						ShowUVs();
					}
				
				}
			}
		
			if (Surforge.surforgeSettings.isBrushActive) PaintWithBrush();
			if (Surforge.surforgeSettings.isPolygonLassoActive) PolygonLasso();
			if (Surforge.surforgeSettings.isPlaceToolActive) PlaceTool();
			ShowTextureBorders();
			if (Surforge.surforgeSettings.isLimitsActive) {
				ShowLimits();
				GreeblesTool();
			}
			if (Surforge.surforgeSettings.showSymAxis) ShowMirrorPlanes();
			DrawPolygonLassoWarpShape();
		
			CheckForGlobalHotkeys();

			if (Surforge.surforgeSettings.isPolygonLassoActive || Surforge.surforgeSettings.isPlaceToolActive || Surforge.surforgeSettings.isLimitsActive) {
				sceneview.Repaint();
			}


			if (Surforge.surforgeSettings.texturePreviewUpdated) {
				window.Repaint();
				Surforge.surforgeSettings.texturePreviewUpdated = false;
			}


			if (materialDragNow) {
				if (EditorWindow.mouseOverWindow != window) {

					Renderer r = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
					if (r) {
						RevertToTempMaterial(r.sharedMaterial);
					}
					materialDragNow = false;
				}
			}


			if (lastActionLogStyle != null) {
				if (Surforge.surforgeSettings.showLastAction) {
					DrawLastActionLog(Surforge.surforgeSettings.lastActionText, Surforge.surforgeSettings.lastActionHotkey, Surforge.surforgeSettings.lastActionSecondHotkey, Surforge.surforgeSettings.lastActionTimer - Time.realtimeSinceStartup);
				}
			}
			UpdateSkyboxWindowRepaint();


		}

		//no sufrorge setttings found
		else {
			Surforge.ToggleEditorGrid(true);

			if (materialEditor != null) {
				DestroyImmediate (materialEditor);
			}
			if (window) window.Repaint();

			//find surforge settings object
			Surforge.surforgeSettings = FindObjectOfType<SurforgeSettings>();

			if (Surforge.surforgeSettings) {
				Surforge.VoxelEngineActivate();
				StartSurforge();
			}
		}

	}
	

	static float distance;
	static float xSpeed = 10.0f;
	static float ySpeed = 10.0f;
	
	static float yMinLimit = -89.99f;
	static float yMaxLimit = 89.99f;
	
	static float distanceMin = -10.0f;
	static float distanceMax = 100000.0f;
	
	static float x;
	static float y;
	
	static Vector2 mouseScrollWheel;
	static Vector2 mouseDelta;
	
	static Rect previewRect;

	static float modelSize = 1.0f;
	static float modelSizeViewSensitivity = 1.0f;

	static Vector3 modelRenderBoundsCenter = new Vector3(0, 45.0f, 84.0f);

	static void UpdateCameraFocusSettings() {

		if (texturePreviewModelRenderer != null) {

			if (!IsMeshValid(mesh)) return;

			float sizeX = texturePreviewModelRenderer.bounds.max.x - texturePreviewModelRenderer.bounds.min.x;
			float sizeY = texturePreviewModelRenderer.bounds.max.y - texturePreviewModelRenderer.bounds.min.y;
			float sizeZ = texturePreviewModelRenderer.bounds.max.z - texturePreviewModelRenderer.bounds.min.z; 
			
			modelSize = sizeX;
			if (sizeY > modelSize) modelSize = sizeY;
			if (sizeZ > modelSize) modelSize = sizeZ;
			
			//Debug.Log ("Model size: " + modelSize);

			modelSizeViewSensitivity = modelSize;

			if (!Surforge.surforgeSettings.seamless) {
				modelRenderBoundsCenter = texturePreviewModelRenderer.bounds.center;
			}
			else {
				modelRenderBoundsCenter = Surforge.surforgeSettings.extentTexturePreview.transform.position;
			}
		}
	}


	static float ClampAngle(float angle, float min, float max) {
		if (angle < -360.0f) angle += 360.0f;
		if (angle > 360.0f) angle -= 360.0f;
		return Mathf.Clamp(angle, min, max);
	}

	static void UpdatePreview() { 
		if (Surforge.surforgeSettings != null) {
			if (Surforge.surforgeSettings.extentTexturePreview != null) { 
				if (Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus != null) {
				
					UpdateCameraFocusSettings();

					Transform target = Surforge.surforgeSettings.extentTexturePreview.previewCameraFocus;


					bool skyboxRotation = false;
					Event e = Event.current;
					if (e != null) {
						if ((e.shift) && (e.button == 0)) {  
							skyboxRotation = true;
						}
					}

					// skybox rotation 
					if (skyboxRotation) {
						float skyboxRot = Surforge.surforgeSettings.extentTexturePreview.transform.eulerAngles.y;
						skyboxRot += mouseDelta.x * xSpeed * 0.02f;
						Surforge.surforgeSettings.extentTexturePreview.transform.eulerAngles = new Vector3(Surforge.surforgeSettings.extentTexturePreview.transform.eulerAngles.x, skyboxRot, Surforge.surforgeSettings.extentTexturePreview.transform.eulerAngles.z);
					} 

					// viewport rotation
					else {
				
						x += mouseDelta.x * xSpeed * distance * 0.02f * (float)(1.0f / modelSizeViewSensitivity);
						y += mouseDelta.y * ySpeed * distance * 0.02f * (float)(1.0f / modelSizeViewSensitivity);
				
				
						y = ClampAngle(y, yMinLimit, yMaxLimit);
				
						Quaternion rotation = Quaternion.Euler(y, x, 0);

						float mouseZoom =  mouseScrollWheel.y;
						if (Mathf.Abs(mouseScrollWheel.x) > Mathf.Abs(mouseScrollWheel.y)) mouseZoom = mouseScrollWheel.x;

						distance = Mathf.Clamp(distance + mouseZoom * 0.07f * modelSizeViewSensitivity, distanceMin, distanceMax);

						Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
						Vector3 position = rotation * negDistance + target.localPosition;
				
						Surforge.surforgeSettings.extentTexturePreview.previewCamera.transform.localRotation = rotation;
						Surforge.surforgeSettings.extentTexturePreview.previewCamera.transform.localPosition = position;
						
					}			

				
				}
			}
		}
	}

	
	static void CheckForGlobalHotkeys() {
		if (!isMaterialRenamingNow) {


			Event e = Event.current;
			if ( (e.type == EventType.KeyUp) &&  (e.keyCode == KeyCode.Space) && (!e.control)){
				Surforge.StartGPURender();
			}
			if ( (e.type == EventType.KeyUp) &&  (e.keyCode == KeyCode.Space) && (e.control)){
				if (mesh) {
					Surforge.surforgeSettings.modelNeedUpdate = true;
					Surforge.LogAction("Reload mesh", "Ctrl + Space", "");
				}
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Space) ) {
				Event.current.Use(); 	
			}
			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.E) && (!e.shift) && (e.control) && (!e.alt)){
				Surforge.ExportMapsGPU();  
				Surforge.LogAction("Export maps", "Ctrl + E", "");
			}

			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.S) && (e.shift) && (!e.control) && (!e.alt)){
				Surforge.SelectSimilar();  
			}

			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.V) ) {
				if (!isPolyLassoObjectPointSnapping) Surforge.LogAction("Shape points snap", "Hold V", "");
				isPolyLassoObjectPointSnapping = true;
			}
			if ( (e.type == EventType.KeyUp) &&  (e.keyCode == KeyCode.V) ) {
				isPolyLassoObjectPointSnapping = false;
			}


			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.S) && (!e.shift) && (!e.control) && (!e.alt)){
				if(Surforge.surforgeSettings.symmetry) {
					Surforge.surforgeSettings.symmetry = false;
					Surforge.LogAction("Symmetry: off", "S", "");
					Surforge.ChangePlaceToolPreview();
					window.Repaint();
				}	
				else {
					Surforge.surforgeSettings.symmetry = true;
					Surforge.LogAction("Symmetry: on", "S", "");
					Surforge.ChangePlaceToolPreview();
					window.Repaint();
				}
				  
			}
			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.S) && (!e.shift) && (!e.control) && (e.alt)){
				if(Surforge.surforgeSettings.seamless) {
					Surforge.DeactivateSeamlessMode();
					Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
					window.Repaint();

				}	
				else {
					Surforge.ActivateSeamlessMode();
					Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
					window.Repaint();
				}
				
			}

			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.M) && (e.shift) && (!e.control) && (!e.alt)){
				Surforge.SelectSameGroup();
			}

			if ( (e.type == EventType.KeyDown) &&  (e.keyCode == KeyCode.M) && (!e.shift) && (!e.control) && (!e.alt)){
				activeTool = 1;
				if (Surforge.surforgeSettings) {
					Surforge.LogAction("Materials", "M", "");
					if (Surforge.surforgeSettings.materialIconsRenderedWithSkybox != Surforge.surforgeSettings.activeSkybox) {
						PrepareMaterialButtons();
					}
				}
				GetWindow<SurforgeInterface>().Repaint();		  
				Event.current.Use();
				 
			}


			if ( (e.type == EventType.KeyUp) && (e.keyCode == KeyCode.G) && (!e.shift) && (!e.control) && (!e.alt)) {  
				Surforge.ToggleLimitsToolActive();
				activeTool = 2;

				GetWindow<SurforgeInterface>().Repaint();		  
				Event.current.Use(); 
			}


			if ( (e.type == EventType.KeyDown) && (e.keyCode == KeyCode.D) && (!e.shift) && (!e.control) && (!e.alt)) {  
				Surforge.TogglePlaceToolActive();
				placeToolState = 0;
				
				Surforge.surforgeSettings.activePlaceMesh = oldSelectedPlaceToolProfileNum;
				selectedPlaceToolProfileNum = oldSelectedPlaceToolProfileNum;
				
				activeTool = 4;

				GetWindow<SurforgeInterface>().Repaint();

				Event.current.Use(); 
			}


			if ( (e.type == EventType.KeyUp) && (e.keyCode == KeyCode.A) && (!e.shift) && (!e.control) && (!e.alt)) {
				Surforge.TogglePolygonLassoActive();
			
				Surforge.surforgeSettings.activePolyLassoProfile = oldSelectedPolyLassoProfileNum;
				selectedPolyLassoProfileNum = oldSelectedPolyLassoProfileNum;
			
				polygonLassoPoints = new List<Vector3>();
			
				activeTool = 5;
			
				GetWindow<SurforgeInterface>().Repaint();

				Event.current.Use(); 
			}

			bool mouseInsideWindow = false;
			EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
			if (mouseOverWindow != null) {
				if (mouseOverWindow.GetType() == typeof(SurforgeInterface)) {
					mouseInsideWindow = true;
				}
			}

			if (Surforge.surforgeSettings) {
				if (Surforge.surforgeSettings.isLimitsActive) {
					if ((Event.current.type == EventType.KeyUp) &&  (Event.current.keyCode == KeyCode.Escape)) {
						Surforge.ToggleLimitsToolActive();
						GetWindow<SurforgeInterface>().Repaint();
					}
				}

				if (Surforge.surforgeSettings.isPolygonLassoActive) {
					if ((Event.current.type == EventType.KeyUp) &&  (Event.current.keyCode == KeyCode.Escape)) {
						if (polygonLassoPoints != null) {
							polygonLassoPoints.Clear();
							Surforge.TogglePolygonLassoActive();
							GetWindow<SurforgeInterface>().Repaint();
						}
					}
				}

				if (Surforge.surforgeSettings.isPlaceToolActive) {

					if ((Event.current.type == EventType.KeyUp) &&  (Event.current.keyCode == KeyCode.Escape)) {
						Surforge.TogglePlaceToolActive();
						GetWindow<SurforgeInterface>().Repaint();
					}


					if ( (Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.KeypadPlus) || (Event.current.keyCode == KeyCode.Equals)) ) {
						if (!mouseInsideWindow) Surforge.MovePlaceToolUp();
					}
					if ( (Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.KeypadMinus) || (Event.current.keyCode == KeyCode.Minus)) ) {
						if (!mouseInsideWindow) Surforge.MovePlaceToolDown();
					}
				

					if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.LeftBracket) ) { 
						if (!mouseInsideWindow) {
							Surforge.LogAction("Scale decrease", " [ ", "");
							Surforge.surforgeSettings.placeToolPreview.transform.localScale *= 0.95f;
						}
					}
					if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.RightBracket) ) {
						if (!mouseInsideWindow) {
							Surforge.LogAction("Scale increase", " ] ", "");
							Surforge.surforgeSettings.placeToolPreview.transform.localScale *= 1.05f;
						}
					}
				}
				else {

					if ( (Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.KeypadPlus) || (Event.current.keyCode == KeyCode.Equals)) ) {
						if (!mouseInsideWindow) Surforge.MoveObjectsUp();
					}
					if ( (Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.KeypadMinus) || (Event.current.keyCode == KeyCode.Minus)) ) {
						if (!mouseInsideWindow) Surforge.MoveObjectsDown();
					}


					if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.LeftBracket) && (Event.current.shift)) { 
						if (!mouseInsideWindow) Surforge.OffsetPolyLassoObjectsIn();
					}
					if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.RightBracket) && (Event.current.shift)) {
						if (!mouseInsideWindow) Surforge.OffsetPolyLassoObjectsOut();
					}


					if (Surforge.surforgeSettings.isPolygonLassoActive) {
						if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.LeftBracket) && (!Event.current.shift)) { 
							if (!mouseInsideWindow) {
								PolyLassoProfileScaleDecrease();
							}
						}
						if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.RightBracket) && (!Event.current.shift)) {
							if (!mouseInsideWindow) {
								PolyLassoProfileScaleIncrease();
							}
						}
					}

					else {

						if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.LeftBracket) && (!Event.current.shift)) { 
							if (!mouseInsideWindow) Surforge.StepPolyLassoProfileFeaturesScaleDecrease();
						}
						if ( (Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.RightBracket) && (!Event.current.shift)) {
							if (!mouseInsideWindow) Surforge.StepPolyLassoProfileFeaturesScaleIncrease();
						}
					}

				}


				//warp shape
				if ((e.type == EventType.MouseDown)  && e.button == 1) { 
					if ((e.alt) && (e.shift) && (!e.control)) {
						if (Surforge.surforgeSettings) {
							bool warpShapeFromPolyLasso = false;
							if (polygonLassoPoints != null) {
								if (polygonLassoPoints.Count > 0) {
									warpShapeFromPolyLasso = true;
								}
							}
							if (warpShapeFromPolyLasso) {
								Surforge.surforgeSettings.warpShape = polygonLassoPoints;
								Surforge.SetWarpShapeCenterLinePoint();
								Surforge.LogAction("Set warp shape", "Alt + Shift + Right Click", "");
								
								polygonLassoPoints = new List<Vector3>();
							}
							else {
								if (Selection.activeGameObject != null) {
									PolyLassoObject pObj = (PolyLassoObject)Selection.activeGameObject.GetComponent<PolyLassoObject>();
									if (pObj != null) {
										Surforge.surforgeSettings.warpShape = Surforge.PolyLassoObjectToWorldShape(pObj);
										Surforge.SetWarpShapeCenterLinePoint();
										Surforge.LogAction("Set warp shape from selected", "Alt + Shift + Right Click", "");
									}
								}
							}
						}
						
					}
					if ((e.alt) && (e.shift) && (e.control)) {
						if (Surforge.surforgeSettings) {
							Surforge.surforgeSettings.warpShape = new List<Vector3>();
							Surforge.surforgeSettings.warpShapeCenterLinePoint = Vector3.zero;
							Surforge.LogAction("Reset warp shape", "Ctrl + Alt + Shift + Right Click", "");
						}
					}
				}
				
				if ((e.type == EventType.MouseDown)  && e.button == 0) { 
					if ((e.alt) && (e.shift)) {
						if (Surforge.surforgeSettings) {
							bool warpShapeFromPolyLasso = false;
							if (polygonLassoPoints != null) {
								if (polygonLassoPoints.Count > 0) {
									warpShapeFromPolyLasso = true;
								}
							}
							if (warpShapeFromPolyLasso) {
								if (!e.control) {
									WarpPolyLassoObject(polygonLassoPoints, null, true, false, false);
									Surforge.LogAction("Create warped shape", "Alt + Shift + Left Click", "");
								}
								else {
									WarpPolyLassoObject(polygonLassoPoints, null, true, true, false);
									Surforge.LogAction("Warped split", "Ctrl + Alt + Shift + Left Click", ""); 
								}
							}
							else {
								if (Selection.activeGameObject != null) {
									List<PolyLassoObject> pObjsToWarp = new List<PolyLassoObject>();
									for (int i=0; i < Selection.gameObjects.Length; i++) {
										PolyLassoObject pObj = (PolyLassoObject)Selection.gameObjects[i].GetComponent<PolyLassoObject>();
										if (pObj != null) {
											pObjsToWarp.Add(pObj);
										}
									}
									bool repeatedWarping = false;
									if (pObjsToWarp.Count == 2) {
										if (Mathf.Abs(Surforge.GetPolyLassoObjectShapeLength(pObjsToWarp[0]) - Surforge.GetPolyLassoObjectShapeLength(pObjsToWarp[1])) < 0.01f)  {
											repeatedWarping = true;
											//Debug.Log("Repeated warp: " + repeatedWarping);
										}
									}
									if ((repeatedWarping) && (!e.control)) {
										Surforge.LogAction("Repeated warp selected", "Alt + Shift + Left Click", "");

										//TODO: distance between shape centers
										float shapeCenterDistance = Vector2.Distance( Surforge.GetPolyLassoObjectShapeCenter(pObjsToWarp[0]), Surforge.GetPolyLassoObjectShapeCenter(pObjsToWarp[1]));
										if (shapeCenterDistance < 0.1f) shapeCenterDistance = 0.1f;
										//Debug.Log("shape center dist: " + shapeCenterDistance);

										float xLength = Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX;
										//Debug.Log("xLength: " + xLength);

										//TODO: correct with root scale
										int repeatCount = Mathf.RoundToInt( xLength / shapeCenterDistance);
										//Debug.Log("repeat count: " + repeatCount);

										float correctedDistance = xLength / (float)repeatCount;
										//Debug.Log("correctedDistance: " + correctedDistance);

										float multRate = correctedDistance / shapeCenterDistance;
										//Debug.Log("multRate: " + multRate);
										List<PolyLassoObject> repeatedInstances = new List<PolyLassoObject>();
										GameObject repeatedScaleRateTransform = new GameObject();

										for (int i=0; i < repeatCount; i++) {
											PolyLassoObject repeatedPObj = (PolyLassoObject)Instantiate(pObjsToWarp[0]);
											repeatedPObj.transform.position = new Vector3(repeatedPObj.transform.position.x + shapeCenterDistance * i, 
											                                              repeatedPObj.transform.position.y,
											                                              repeatedPObj.transform.position.z);
											repeatedPObj.transform.parent = repeatedScaleRateTransform.transform;
											repeatedInstances.Add(repeatedPObj);
										}
										repeatedScaleRateTransform.transform.localScale = repeatedScaleRateTransform.transform.localScale * multRate;

										float allShapesCenterOffset = 0;

										for (int i=0; i < repeatedInstances.Count; i++) {
											repeatedInstances[i].transform.parent = Surforge.surforgeSettings.root.transform;
											allShapesCenterOffset = allShapesCenterOffset + Surforge.GetPolyLassoObjectShapeCenter(repeatedInstances[i]).x;
										}
										float repeatedShapesXoffset = allShapesCenterOffset / (float)repeatedInstances.Count;
										for (int i=0; i < repeatedInstances.Count; i++) {
											repeatedInstances[i].transform.position = new Vector3(repeatedInstances[i].transform.position.x - repeatedShapesXoffset,
											                                                      repeatedInstances[i].transform.position.y,
											                                                      repeatedInstances[i].transform.position.z);
											WarpPolyLassoObject(Surforge.PolyLassoObjectToWorldShape(repeatedInstances[i]), repeatedInstances[i], false, false, true);
										}

										if (repeatedScaleRateTransform != null) DestroyImmediate(repeatedScaleRateTransform.gameObject);

										if (pObjsToWarp[0].gameObject != null) DestroyImmediate(pObjsToWarp[0].gameObject);
										if (pObjsToWarp[1].gameObject != null) DestroyImmediate(pObjsToWarp[1].gameObject);
									}

									else {
										for (int i=0; i<pObjsToWarp.Count; i++) {
											if (pObjsToWarp[i] != null) {
												if (!e.control) {
													WarpPolyLassoObject(Surforge.PolyLassoObjectToWorldShape(pObjsToWarp[i]), pObjsToWarp[i], false, false, false);
													Surforge.LogAction("Warp selected", "Alt + Shift + Left Click", "");
												}

											}
										}
									}


									
								}
							}
							
						}
					}
				}


			}



			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha1) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(0);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha2) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(1);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha3) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(2);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha4) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(3);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha5) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(4);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha6) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(5);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha7) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(6);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha8) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(7);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha9) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(8);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			if ( Event.current.isKey && (Event.current.keyCode == KeyCode.Alpha0) ) {
				if (!mouseInsideWindow) {
					Surforge.AssignMaterialGroupToSelection(9);
					if (Event.current.shift) Surforge.SimilarGroups();
				}
				Event.current.Use(); 
			}
			
		}
		else {	

			if (Event.current.type == EventType.MouseDown) {
				Vector2 mousePosCancelRenaming = Event.current.mousePosition;
				if ((mousePosCancelRenaming.x < renamingMaterialRect.xMin) || (mousePosCancelRenaming.y < (renamingMaterialRect.yMin - 5)) || (mousePosCancelRenaming.x > renamingMaterialRect.xMax)|| (mousePosCancelRenaming.y > (renamingMaterialRect.yMax + 5)))  {
					isMaterialRenamingNow = false;
					window.Repaint();
				}
			}

		}

		if ((Event.current.type == EventType.MouseUp) && (Event.current.button == 1) && Event.current.shift) { 
			if (window != null) {
				if(EditorWindow.mouseOverWindow != window) { 

					if (((activeTool == 4) || (activeTool == 5)) && (Surforge.surforgeSettings.limits != null)) { 
						if (!Event.current.alt) {
							if (!Event.current.control) Surforge.LogAction("Set symmetry center to UV island", "Shift + Right Click", "");
							else Surforge.LogAction("Reset symmetry axes", "Ctrl + Shift + Right Click", "");
						}

						if (activeTool == 5) {
							if (!Surforge.surforgeSettings.isPolygonLassoActive) {
								SetPlaceToolSymmetry();
								SetMirrorPlanesToPlaceToolSymmetryPoint();
								SetPlaceToolSymmetryParent();
							}
						}
						else {
							SetPlaceToolSymmetry();
							SetMirrorPlanesToPlaceToolSymmetryPoint();
							SetPlaceToolSymmetryParent();
						}
					}

				}
			}
		}

		
		if ( (Event.current.type == EventType.MouseUp) && (Event.current.button == 1) && (!Event.current.alt) && (Event.current.control) && (Event.current.shift)) {
			Surforge.surforgeSettings.symmetryPoint = Vector3.zero;
			Surforge.surforgeSettings.mirrorLineSolid = new List<Vector3> {Vector3.zero, new Vector3(0, 0, 1.0f)};
			Surforge.surforgeSettings.mirrorLineDotted = new List<Vector3> {Vector3.zero, new Vector3(1.0f, 0, 0)};
			SetPlaceToolSymmetryParent();
		}


	}

	static void SetPlaceToolSymmetry() {
		List<Vector3> shapeForSymmetry = new List<Vector3>();

		if (Surforge.surforgeSettings.placeToolPreview == null) {
			shapeForSymmetry = GetUvBorderPointsAroundPoint(new Vector3(0, Surforge.surforgeSettings.limits.minY, 0));
			Vector3 hitPoint = new Vector3();

			Surforge.CreatePlaceToolPlane(); 

			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit, 1000.0f)) {
				if (hit.collider.gameObject != null) {
					hitPoint = new Vector3(hit.point.x, Surforge.surforgeSettings.limits.minY, hit.point.z);
				}
			}
			Surforge.DestroyPlaceToolPlane();
			shapeForSymmetry = GetUvBorderPointsAroundPoint(hitPoint);
		}
		else {
			shapeForSymmetry = GetUvBorderPointsAroundPoint(Surforge.surforgeSettings.placeToolPreview.transform.position);
		}


		Surforge.surforgeSettings.symmetryPoint = Vector3.zero;

		if (shapeForSymmetry.Count == 0) {
			Surforge.surforgeSettings.symmetryPoint = Vector3.zero;
			return;
		}

		float minX = Mathf.Infinity;
		float minZ = Mathf.Infinity;
		float maxX = Mathf.NegativeInfinity;
		float maxZ = Mathf.NegativeInfinity;
		for (int i=0; i < shapeForSymmetry.Count; i++) {
			if (shapeForSymmetry[i].x > maxX) maxX = shapeForSymmetry[i].x;
			if (shapeForSymmetry[i].x < minX) minX = shapeForSymmetry[i].x;
			if (shapeForSymmetry[i].z > maxZ) maxZ = shapeForSymmetry[i].z;
			if (shapeForSymmetry[i].z < minZ) minZ = shapeForSymmetry[i].z;
		}
		Surforge.surforgeSettings.symmetryPoint = new Vector3((minX + maxX) * 0.5f, 0, (minZ + maxZ) * 0.5f);
	}

	static void SetMirrorPlanesToPlaceToolSymmetryPoint() {
		Surforge.surforgeSettings.mirrorLineSolid = new List<Vector3>();
		Surforge.surforgeSettings.mirrorLineSolid.Add(Surforge.surforgeSettings.symmetryPoint);
		Vector3 blueAxisDirection = (new Vector3(Surforge.surforgeSettings.symmetryPoint.x, Surforge.surforgeSettings.symmetryPoint.y, Surforge.surforgeSettings.symmetryPoint.z + 1.0f)  - Surforge.surforgeSettings.symmetryPoint).normalized;
		Surforge.surforgeSettings.mirrorLineSolid.Add(Surforge.surforgeSettings.symmetryPoint + blueAxisDirection);
		
		Surforge.surforgeSettings.mirrorLineDotted = new List<Vector3>();
		Surforge.surforgeSettings.mirrorLineDotted.Add(Surforge.surforgeSettings.symmetryPoint);
		Vector3 redAxisDirection = (new Vector3(Surforge.surforgeSettings.symmetryPoint.x + 1.0f, Surforge.surforgeSettings.symmetryPoint.y, Surforge.surforgeSettings.symmetryPoint.z) - Surforge.surforgeSettings.symmetryPoint).normalized;
		Surforge.surforgeSettings.mirrorLineDotted.Add(Surforge.surforgeSettings.symmetryPoint + redAxisDirection);
	}


	
	static bool uvsFound;
	static Color uvLines = new Color(1.5f, 0.5f, 0.5f, 0.1f); 
	static Color uvLinesBorder = new Color(1.5f, 0.5f, 0.5f, 1.0f); 
	static Color uvHelperLines0 = new Color(0.5f, 0.5f, 0.5f, 0.2f); 
	static Color uvHelperLines1 = new Color(0.8f, 0.8f, 0.8f, 0.5f);

	static Color uvHelperLines2 = new Color(0.5f, 0.5f, 0.5f, 0.5f); 
	static Color uvHelperLines3 = new Color(1.0f, 1.0f, 1.0f, 0.7f); 
	
	static Vector3[] uvPoints;
	
	static List<Vector3> uvHelperSnapPoints;

	static Vector3P[] uvVectorPairs;
	static List<Vector3P> uvVectorPairsBorder;
	static List<int> uvVectorPairsBorderIDs;
	
	static List<Vector3P> uvHelperVectorPairs;
	
	static List<Line2d> linesX;
	static List<Line2d> linesZ;

	static Mesh mesh;
	static Mesh oldMesh;
	
	static bool uvGuidesUpdated; 

	static Renderer texturePreviewModelRenderer;

	static int oldMeshInstanceID;


	static bool IsMeshValid(Mesh meshToCheck) {
		bool result = true;
		if (meshToCheck.vertices.Length < 3) result = false;
		return result;
	}


	static void CheckUVsChanged() { 
		MeshFilter meshFilter = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();
		if (meshFilter != null) {

			mesh = meshFilter.sharedMesh;

			if (!IsMeshValid(mesh)) return;

			if ( (mesh != oldMesh) || (Surforge.surforgeSettings.modelNeedUpdate == true) )  {

				if (MeshChangeNotRelatedToSeamlessPreview(mesh) || (!uvGuidesUpdated) ) {

					texturePreviewModelRenderer = (Renderer)Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();

					oldMesh = mesh;

					Surforge.surforgeSettings.modelNeedUpdate = false;


					Surforge.surforgeSettings.showUvs = false;
					showUVs = false;
					
					Surforge.surforgeSettings.showUvGrid = true;
					showUVsHelperGrid = true;


					PrepareUVs();

					if (mesh == Surforge.surforgeSettings.cubePreviewModel) {
						Surforge.surforgeSettings.extentTexturePreview.composer.ChangeSemlessPreviewMesh();
					}

					if (oldMeshInstanceID != mesh.GetInstanceID()) {
						UpdateCameraFocusSettings(); 
						ResetPreviewCameraPosition();
						UpdatePreview();
						if (window) window.Repaint();
					}

					oldMeshInstanceID = mesh.GetInstanceID();
				}

			}
			
		}
	}



	static bool MeshChangeNotRelatedToSeamlessPreview(Mesh mesh) {
		bool result = true;

		if ( ( (oldMesh == Surforge.surforgeSettings.cubePreviewModel) ||
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[0]) || 
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[1]) || 
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[2]) || 
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[3]) || 
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[4]) || 
		       (oldMesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[5]) ) 
		    && 
		    ( (mesh == Surforge.surforgeSettings.cubePreviewModel) ||
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[0]) || 
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[1]) || 
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[2]) || 
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[3]) || 
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[4]) || 
		      (mesh == Surforge.surforgeSettings.extentTexturePreview.composer.seamlessPreviewMeshes[5]) ) ) {

			result = false;
		}

		return result;
	}


	//--------------------------------------- uv islands ---------------------------

	/// Builds an array of edges that connect to only one triangle.
	/// In other words, the outline of the mesh    
	public static Edge[] BuildManifoldEdges(Mesh mesh)
	{
		// Build a edge list for all unique edges in the mesh
		Edge[] edges = BuildEdges(mesh.vertexCount, mesh.triangles);
		
		// We only want edges that connect to a single triangle
		List<Edge> culledEdges = new List<Edge>();
		foreach (Edge edge in edges)
		{
			if (edge.faceIndex[0] == edge.faceIndex[1])
			{
				culledEdges.Add(edge);
			}
		}
		
		return culledEdges.ToArray();
	}

	public static Edge[] BuildAllEdges(Mesh mesh) {

		Edge[] edges = BuildEdges(mesh.vertexCount, mesh.triangles);

		return edges;
	}
	
	/// Builds an array of unique edges
	/// This requires that your mesh has all vertices welded. However on import, Unity has to split
	/// vertices at uv seams and normal seams. Thus for a mesh with seams in your mesh you
	/// will get two edges adjoining one triangle.
	/// Often this is not a problem but you can fix it by welding vertices 
	/// and passing in the triangle array of the welded vertices.
	public static Edge[] BuildEdges(int vertexCount, int[] triangleArray) {
		int maxEdgeCount = triangleArray.Length;
		int[] firstEdge = new int[vertexCount + maxEdgeCount];
		int nextEdge = vertexCount;
		int triangleCount = triangleArray.Length / 3;
		
		for (int a = 0; a < vertexCount; a++)
			firstEdge[a] = -1;
		
		// First pass over all triangles. This finds all the edges satisfying the
		// condition that the first vertex index is less than the second vertex index
		// when the direction from the first vertex to the second vertex represents
		// a counterclockwise winding around the triangle to which the edge belongs.
		// For each edge found, the edge index is stored in a linked list of edges
		// belonging to the lower-numbered vertex index i. This allows us to quickly
		// find an edge in the second pass whose higher-numbered vertex index is i.
		Edge[] edgeArray = new Edge[maxEdgeCount];
		
		int edgeCount = 0;
		for (int a = 0; a < triangleCount; a++)
		{
			int i1 = triangleArray[a * 3 + 2];
			for (int b = 0; b < 3; b++)
			{
				int i2 = triangleArray[a * 3 + b];
				if (i1 < i2)
				{
					Edge newEdge = new Edge();
					newEdge.vertexIndex[0] = i1;
					newEdge.vertexIndex[1] = i2;
					newEdge.faceIndex[0] = a;
					newEdge.faceIndex[1] = a;
					edgeArray[edgeCount] = newEdge;
					
					int edgeIndex = firstEdge[i1];
					if (edgeIndex == -1)
					{
						firstEdge[i1] = edgeCount;
					}
					else
					{
						while (true)
						{
							int index = firstEdge[nextEdge + edgeIndex];
							if (index == -1)
							{
								firstEdge[nextEdge + edgeIndex] = edgeCount;
								break;
							}
							
							edgeIndex = index;
						}
					}
					
					firstEdge[nextEdge + edgeCount] = -1;
					edgeCount++;
				}
				
				i1 = i2;
			}
		}
		
		// Second pass over all triangles. This finds all the edges satisfying the
		// condition that the first vertex index is greater than the second vertex index
		// when the direction from the first vertex to the second vertex represents
		// a counterclockwise winding around the triangle to which the edge belongs.
		// For each of these edges, the same edge should have already been found in
		// the first pass for a different triangle. Of course we might have edges with only one triangle
		// in that case we just add the edge here
		// So we search the list of edges
		// for the higher-numbered vertex index for the matching edge and fill in the
		// second triangle index. The maximum number of comparisons in this search for
		// any vertex is the number of edges having that vertex as an endpoint.
		
		for (int a = 0; a < triangleCount; a++)
		{
			int i1 = triangleArray[a * 3 + 2];
			for (int b = 0; b < 3; b++)
			{
				int i2 = triangleArray[a * 3 + b];
				if (i1 > i2)
				{
					bool foundEdge = false;
					for (int edgeIndex = firstEdge[i2]; edgeIndex != -1; edgeIndex = firstEdge[nextEdge + edgeIndex])
					{
						Edge edge = edgeArray[edgeIndex];
						if ((edge.vertexIndex[1] == i1) && (edge.faceIndex[0] == edge.faceIndex[1]))
						{
							edgeArray[edgeIndex].faceIndex[1] = a;
							foundEdge = true;
							break;
						}
					}
					
					if (!foundEdge)
					{
						Edge newEdge = new Edge();
						newEdge.vertexIndex[0] = i1;
						newEdge.vertexIndex[1] = i2;
						newEdge.faceIndex[0] = a;
						newEdge.faceIndex[1] = a;
						edgeArray[edgeCount] = newEdge;
						edgeCount++;
					}
				}
				
				i1 = i2;
			}
		}
		
		Edge[] compactedEdges = new Edge[edgeCount];
		for (int e = 0; e < edgeCount; e++)
			compactedEdges[e] = edgeArray[e];
		
		return compactedEdges;
	}


	public class Edge {
		// The indiex to each vertex
		public int[] vertexIndex = new int[2];
		// The index into the face.
		// (faceindex[0] == faceindex[1] means the edge connects to only one triangle)
		public int[] faceIndex = new int[2];

		public bool sorted;

		public bool welded;
		public int minIndex;
		public int maxIndex;
	}





// ------------------------------------------------------------------------------


	static List<Vector2> MergeDuplicatePoints2D(List<Vector2> points) {
		List<Vector2> result = new List<Vector2>();
		for (int i=0; i < points.Count; i++) {
			bool isPointUniq = true;
			for (int d=0; d < result.Count; d++) {
				if ( (Mathf.Abs(points[i].x - points[d].x) < 0.0001f) &&
				     (Mathf.Abs(points[i].y - points[d].y) < 0.0001f) ){
					isPointUniq = false;
					break;
				}
			}
			if (isPointUniq) result.Add(points[i]);
		}
		return result;
	}


	static void CreateUvHelperPairs(Vector2[] shape, Line2d line, bool isVertical, float limitsMinY, int uvIslandIndex) {
		if (shape.Length < 3) return; 

		List<Vector2> interPoints = new List<Vector2>(); 

		for (int i=0; i< shape.Length; i++) {
			Vector2 shapePointA = shape[i];
			Vector2 shapePointB = shape[0];
			if (i != (shape.Length-1)) shapePointB = shape[i+1];

			if (Surforge.TestLinesIntersection(line.start, line.end, shapePointA, shapePointB)) {
				if (Surforge.TestSegmentIntersection(line.start, line.end, shapePointA, shapePointB)) {
					Vector2 intr = Surforge.LineIntersectionPoint(line.start, line.end, shapePointA, shapePointB);
					interPoints.Add(intr);
				}
				else {
					if (isVertical) { 
						if (Mathf.Abs(line.start.x - shapePointA.x) < 0.0001f) {
							Vector2 intr = new Vector2(shapePointA.x, shapePointA.y);
							interPoints.Add(intr);
						}
						if (Mathf.Abs(line.start.x - shapePointB.x) < 0.0001f) {
							Vector2 intr = new Vector2(shapePointB.x, shapePointB.y);
							interPoints.Add(intr);
						}
					}
					else {
						if (Mathf.Abs(line.start.y - shapePointA.y) < 0.0001f) {
							Vector2 intr = new Vector2(shapePointA.x, shapePointA.y);
							interPoints.Add(intr);
						}
						if (Mathf.Abs(line.start.y - shapePointB.y) < 0.0001f) {
							Vector2 intr = new Vector2(shapePointB.x, shapePointB.y);
							interPoints.Add(intr);
						}
					}
				}
			}
		}

		if (interPoints.Count < 2) return;

		//merge duplicate points
		int mergeRepeats = interPoints.Count;
		for (int i=0; i< mergeRepeats; i++) {
			interPoints = MergeDuplicatePoints2D(interPoints);
		}


		if (interPoints.Count < 2) return;

		//sort intersection points
		List<Vector2> sortedInterPoints = new List<Vector2>();
		int tryes = interPoints.Count;

		for (int i=0; i < tryes; i++) {
			float minPos = Mathf.Infinity;
			int minPosIndex = 0;
			for (int p=0; p < interPoints.Count; p++) {
				if (isVertical) {
					if (interPoints[p].y < minPos) {
						minPos = interPoints[p].y;
						minPosIndex = p;
					}
				}
				else {
					if (interPoints[p].x < minPos) {
						minPos = interPoints[p].x;
						minPosIndex = p;
					}
				}
			}
			sortedInterPoints.Add(interPoints[minPosIndex]);
			interPoints.RemoveAt(minPosIndex);
			if (interPoints.Count < 1) break;
		}



		//helperPoints
		for (int i=0; i< sortedInterPoints.Count; i=i+2) {
			if ( (i+1) < sortedInterPoints.Count) {
				Vector3P pair = new Vector3P();
				if (isVertical) pair.isVertical = true;
				pair.a = new Vector3(sortedInterPoints[i].x, limitsMinY, sortedInterPoints[i].y);
				pair.b = new Vector3(sortedInterPoints[i+1].x, limitsMinY, sortedInterPoints[i+1].y);

				pair.subdLevel = line.lineBrightness;

				pair.uvIslandIndex = uvIslandIndex;

				uvHelperVectorPairs.Add(pair);
			}
		}

		for (int i=0; i < sortedInterPoints.Count; i++) {
			uvHelperSnapPoints.Add(new Vector3(sortedInterPoints[i].x, limitsMinY, sortedInterPoints[i].y));
		}

	
	}



	public class UvIsland {
		public int[] vertexIndex;
		public int[] faceIndex;

		public Edge[] border;

		public Bounds2D bounds;

		public Edge[] edges;

		//---
		public List<Vector3P> pairs;

		///---
		public List<Edge> edgesList;
		public List<Edge> borderList;
	}
	

	static List<RecursiveGraphSercher> searchersResult;

	static int searchesCounter = 0;
	static bool[] searcStartedGraphNodes;

	public class RecursiveGraphSercher {
		public List<int> visitedVerts;

		public SurforgeAdjacencyList<int> graph;

		public void Search() {
			searchesCounter++;

			List<int> neighbours = graph.FindNeighbours(visitedVerts[visitedVerts.Count -1]);

			/*
			Debug.Log ("------------------");
			Debug.Log ("visitedVerts[0]: " + visitedVerts[0] + "visitedVerts[visitedVerts.Count-1]: " + visitedVerts[visitedVerts.Count-1]);
			for (int i=0; i< neighbours.Count; i++) {
				Debug.Log (neighbours[i]);
			}
			*/

			for (int i=0; i< neighbours.Count; i++) {
				if (!visitedVerts.Contains(neighbours[i])) {
					RecursiveGraphSercher searcher = new RecursiveGraphSercher();
					searcher.graph = graph;
					searcher.visitedVerts = new List<int>();
					for (int n=0; n< visitedVerts.Count; n++) {
						searcher.visitedVerts.Add(visitedVerts[n]);
					}
					searcher.visitedVerts.Add(neighbours[i]);
					searcher.Search();
				}
			}	 

			if (visitedVerts.Count > 2) {
				if (neighbours.Contains(visitedVerts[0])) {
					searchersResult.Add(this); //add only closed searhes

					for (int i=0; i< visitedVerts.Count; i++) {
						searcStartedGraphNodes[visitedVerts[i]] = true;
					}
				}
			}


		}
	}



	static void QuickSort(RecursiveGraphSercher[] a, int l, int r) { 
		RecursiveGraphSercher temp;
		RecursiveGraphSercher x = a[l + (r - l) / 2];
		
		int i = l;
		int j = r;
		
		while (i <= j)
		{
			while (a[i].visitedVerts.Count < x.visitedVerts.Count) i++;
			while (a[j].visitedVerts.Count > x.visitedVerts.Count) j--;
			if (i <= j)
			{
				temp = a[i];
				a[i] = a[j];
				a[j] = temp;
				i++;
				j--;
			}
		}
		if (i < r)
			QuickSort(a, i, r);
		
		if (l < j)
			QuickSort(a, l, j);
	}


	public class UvFilter {
		public Edge edge;
		public int index;
		public int x;
		public int y;

		public int vertexNum;
	}


	static void QuickSortUvFilterX(UvFilter[] a, int l, int r) { 
		UvFilter temp;
		UvFilter x = a[l + (r - l) / 2];
		
		int i = l;
		int j = r;
		
		while (i <= j)
		{
			while (a[i].x < x.x) i++;
			while (a[j].x > x.x) j--;
			if (i <= j)
			{
				temp = a[i];
				a[i] = a[j];
				a[j] = temp;
				i++;
				j--;
			}
		}
		if (i < r)
			QuickSortUvFilterX(a, i, r);
		
		if (l < j)
			QuickSortUvFilterX(a, l, j);
	}

	static void QuickSortUvFilterY(UvFilter[] a, int l, int r) { 
		UvFilter temp;
		UvFilter x = a[l + (r - l) / 2];
		
		int i = l;
		int j = r;
		
		while (i <= j)
		{
			while (a[i].y < x.y) i++;
			while (a[j].y > x.y) j--;
			if (i <= j)
			{
				temp = a[i];
				a[i] = a[j];
				a[j] = temp;
				i++;
				j--;
			}
		}
		if (i < r)
			QuickSortUvFilterY(a, i, r);
		
		if (l < j)
			QuickSortUvFilterY(a, l, j);
	}


	static void QuickSortEdgeMinIndex(Edge[] a, int l, int r) { 
		Edge temp;
		Edge x = a[l + (r - l) / 2];
		
		int i = l;
		int j = r;
		
		while (i <= j)
		{
			while (a[i].minIndex < x.minIndex) i++;
			while (a[j].minIndex > x.minIndex) j--;
			if (i <= j)
			{
				temp = a[i];
				a[i] = a[j];
				a[j] = temp;
				i++;
				j--;
			}
		}
		if (i < r)
			QuickSortEdgeMinIndex(a, i, r);
		
		if (l < j)
			QuickSortEdgeMinIndex(a, l, j);
	}

	static void QuickSortEdgeMaxIndex(Edge[] a, int l, int r) { 
		Edge temp;
		Edge x = a[l + (r - l) / 2];
		
		int i = l;
		int j = r;
		
		while (i <= j)
		{
			while (a[i].maxIndex < x.maxIndex) i++;
			while (a[j].maxIndex > x.maxIndex) j--;
			if (i <= j)
			{
				temp = a[i];
				a[i] = a[j];
				a[j] = temp;
				i++;
				j--;
			}
		}
		if (i < r)
			QuickSortEdgeMaxIndex(a, i, r);
		
		if (l < j)
			QuickSortEdgeMaxIndex(a, l, j);
	}




	static int UvFloatToInt(float f) {
		return Mathf.RoundToInt(f * 10000);
	}

	static Edge[] FilterUvIslandBorder(Edge[] border, Vector2[] uvs) { 

		//searched graph nodes array
		searcStartedGraphNodes = new bool[uvs.Length];

		//filter verts
		UvFilter[] uvFilters = new UvFilter[border.Length * 2];

		for (int i=0; i< border.Length; i++) {
			for (int n=0; n< border[i].vertexIndex.Length; n++) {
				UvFilter uvFilter = new UvFilter();
				uvFilter.edge = border[i];
				uvFilter.index = border[i].vertexIndex[n];

				uvFilter.x = UvFloatToInt(uvs[border[i].vertexIndex[n]].x);
				uvFilter.y = UvFloatToInt(uvs[border[i].vertexIndex[n]].y);

				uvFilter.vertexNum = n;

				uvFilters[i*2 + n] = uvFilter;
			}
		}

		QuickSortUvFilterX(uvFilters, 0, uvFilters.Length-1);


		int oldX = Mathf.RoundToInt(Mathf.NegativeInfinity);
		List<UvFilter> sortY = new List<UvFilter>();
		for (int i=0; i< uvFilters.Length; i++) {
			if (uvFilters[i].x == oldX) {
				sortY.Add(uvFilters[i]);
			}
			else {
				UvFilter[] sortedY = sortY.ToArray();
				if (sortedY.Length > 1) {
					QuickSortUvFilterY(sortedY, 0, sortedY.Length-1);
				}

				for (int s=0; s < sortedY.Length; s++) {
					uvFilters[i - sortedY.Length + s] = sortedY[s];
				}

				oldX = uvFilters[i].x;
				sortY = new List<UvFilter>();
				sortY.Add(uvFilters[i]);
			}
		}
		UvFilter[] sortedYfin = sortY.ToArray();
		if (sortedYfin.Length > 1) {
			QuickSortUvFilterY(sortedYfin, 0, sortedYfin.Length-1);
		}
		
		for (int s=0; s < sortedYfin.Length; s++) {
			uvFilters[uvFilters.Length - sortedYfin.Length + s] = sortedYfin[s];
		}


		//replace
		int oldXr = uvFilters[0].x;
		int oldYr = uvFilters[0].y;

		int indexToSet = uvFilters[0].index;
		for (int i=0; i< uvFilters.Length; i++) {

			if ((uvFilters[i].x == oldXr) && (uvFilters[i].y == oldYr)) {

				uvFilters[i].edge.vertexIndex[uvFilters[i].vertexNum] = indexToSet;

			}
			else {
				indexToSet = uvFilters[i].index;

				oldXr = uvFilters[i].x;
				oldYr = uvFilters[i].y;
			}

		}

		//set min max index
		for (int i=0; i< border.Length; i++) {
			border[i].minIndex = border[i].vertexIndex[0];
			border[i].maxIndex = border[i].vertexIndex[1];
			if (border[i].minIndex > border[i].maxIndex) {
				border[i].minIndex = border[i].vertexIndex[1];
				border[i].maxIndex = border[i].vertexIndex[0];
			}
		}

		//sort for min than max index
		QuickSortEdgeMinIndex(border, 0, border.Length-1);

		int oldMin = Mathf.RoundToInt(Mathf.NegativeInfinity);
		List<Edge> sortMax = new List<Edge>();
		for (int i=0; i< border.Length; i++) {
			if (border[i].minIndex == oldMin) {
				sortMax.Add(border[i]);
			}
			else {
				Edge[] sortedMax = sortMax.ToArray();
				if (sortedMax.Length > 1) {
					QuickSortEdgeMaxIndex(sortedMax, 0, sortedMax.Length-1);
				}
				
				for (int s=0; s < sortedMax.Length; s++) {
					border[i - sortedMax.Length + s] = sortedMax[s];
				}
				
				oldMin = border[i].minIndex;
				sortMax = new List<Edge>();
				sortMax.Add(border[i]);
			}
		}
		Edge[] sortedMaxFin = sortMax.ToArray();
		if (sortedMaxFin.Length > 1) {
			QuickSortEdgeMaxIndex(sortedMaxFin, 0, sortedMaxFin.Length-1);
		}
		
		for (int s=0; s < sortedMaxFin.Length; s++) {
			border[border.Length - sortedMaxFin.Length + s] = sortedMaxFin[s];
		}


		//mark edges with matching both min and max index as welded
		int oldMinR = -1;
		int oldMaxR = -1;

		Edge lastEdge = border[0];
		for (int i=0; i< border.Length; i++) {
			
			if ((border[i].minIndex == oldMinR) && (border[i].maxIndex == oldMaxR)) {
				
				border[i].welded = true;
				lastEdge.welded = true;
				
			}
			else {
				lastEdge = border[i];

				oldMinR = border[i].minIndex;
				oldMaxR = border[i].maxIndex;
			}
			
		}

		//remove welded border edges
		List<Edge> weldedFreeBorder = new List<Edge>();
		for (int i=0; i< border.Length; i++) {
			if (!border[i].welded) {
				weldedFreeBorder.Add(border[i]);
			}
		}

		//assign welded free array
		if (weldedFreeBorder.Count > 2) {
			border = weldedFreeBorder.ToArray();  ///  
		}


		/*
		//debug
		for (int i=0; i< border.Length; i++) {
			Debug.Log ("Edge: " + i + "   minIndex: " + border[i].minIndex + "   maxIndex: " + border[i].maxIndex);
		}
		*/



		///// graph
		SurforgeAdjacencyList<int> vertexGraph = new SurforgeAdjacencyList<int>(border[0].vertexIndex[0]);

		int maxVertex = 0;


		for (int i=0; i< border.Length; i++) {
			if (border[i].vertexIndex[0] != border[i].vertexIndex[1]) {
				vertexGraph.AddEdge(border[i].vertexIndex[0], border[i].vertexIndex[1]);
			}
			if (border[i].vertexIndex[0] > maxVertex) maxVertex = border[i].vertexIndex[0];
			if (border[i].vertexIndex[1] > maxVertex) maxVertex = border[i].vertexIndex[1];
		}


		//check for nodes with more than 2 neighbours count
		int multiNeighbourNodesCount = 0;
		for (int i=0; i< maxVertex; i++) {
			if (vertexGraph.Contains(i)) {
				if (vertexGraph.FindNeighbours(i).Count > 2) {
					multiNeighbourNodesCount++;
				}
			}
		}
		//Debug.Log("multiNeighbourNodesCount: " + multiNeighbourNodesCount);

		if (multiNeighbourNodesCount > 4) return border;


		searchersResult = new List<RecursiveGraphSercher> ();


		for (int i=0; i< maxVertex; i++) {
			if (vertexGraph.Contains(i)) {
				if (!searcStartedGraphNodes[i]) {
					RecursiveGraphSercher searcher = new RecursiveGraphSercher();
					searcher.graph = vertexGraph;
					searcher.visitedVerts = new List<int>();
					searcher.visitedVerts.Add(i);
					searcStartedGraphNodes[i] = true;
					searcher.Search();
				}
			}
		}



		RecursiveGraphSercher[] sortedSearchersResult = searchersResult.ToArray();

		//Debug.Log ("sortedSearchersResult: " + sortedSearchersResult.Length);

		if (sortedSearchersResult.Length < 2) {
			return border;
		}

		QuickSort(sortedSearchersResult, 0, sortedSearchersResult.Length-1);

		searchersResult = new List<RecursiveGraphSercher> ();
		searchersResult.AddRange(sortedSearchersResult);
		searchersResult.Reverse();

		Bounds2D maxBounds = new Bounds2D();

		List<Edge> lastBorder = new List<Edge>();

		int counter = 0;
		for (int k=0; k < searchersResult.Count; k++) {
			if (searchersResult[k].visitedVerts.Count < 2) {
				return border;
			}


			List<Edge> currentBorder = new List<Edge>();

			for (int i=0; i < searchersResult[k].visitedVerts.Count; i++) {
				Edge edge = new Edge();
				if (i == (searchersResult[k].visitedVerts.Count - 1)) {
					edge.vertexIndex[0] = searchersResult[k].visitedVerts[i];
					edge.vertexIndex[1] = searchersResult[k].visitedVerts[0];
				}
				else {
					edge.vertexIndex[0] = searchersResult[k].visitedVerts[i];
					edge.vertexIndex[1] = searchersResult[k].visitedVerts[i+1];
				}
				currentBorder.Add(edge);
			}

			Bounds2D currentBounds = GetUvIslandBorderBounds(currentBorder.ToArray(), uvs);


			if (counter == 0) {
				//maxBounds = currentBounds;

				maxBounds = new Bounds2D(); //TODO: check if can just copy
				maxBounds.maxX = currentBounds.maxX;
				maxBounds.minX = currentBounds.minX;
				maxBounds.maxY = currentBounds.maxY;
				maxBounds.minY = currentBounds.minY;

				lastBorder = new List<Edge>();
				lastBorder.AddRange(currentBorder.ToArray());
			}
			else {
				if (!IsUvIslandBorderBoundsEqual(currentBounds, maxBounds)) {
					return lastBorder.ToArray();
				}
				else {
					lastBorder = new List<Edge>();
					lastBorder.AddRange(currentBorder.ToArray());
				}
			}




			counter++;
		}

		return border;
	}




	static int[] meshVertexIndexMarks;

	static List<UvIsland> CreateUvIslands(Mesh mesh, Edge[] edges) {

		List<UvIsland> result = new List<UvIsland>();

		int uvIslandIndex = 0;
		meshVertexIndexMarks = new int[mesh.vertices.Length];

		for (int i=0; i<meshVertexIndexMarks.Length; i++) {
			meshVertexIndexMarks[i] = 0;
		}

		int meshTrisCount = mesh.triangles.Length / 3;


		///// graph
		SurforgeAdjacencyList<int> vertexGraph = new SurforgeAdjacencyList<int>(0);

		for (int i=0; i< edges.Length; i++) {
			vertexGraph.AddEdge(edges[i].vertexIndex[0], edges[i].vertexIndex[1]);
		}


		for (int i=0; i< meshTrisCount; i++) {
			uvIslandIndex++;

			int startPoint = 0;
			bool unmarkedPointFound = false;

			for (int s=0; s< meshVertexIndexMarks.Length; s++) {
				if (meshVertexIndexMarks[s] == 0) {
					startPoint = s;
					meshVertexIndexMarks[s] = uvIslandIndex;
					unmarkedPointFound = true;
					break;
				}
			}


			if (!unmarkedPointFound) break;


			SurforgeBreadthFirstSearch search = new SurforgeBreadthFirstSearch();
			search.BreadthFirstSearch(vertexGraph, startPoint);

			List<int> foundVerts = new List<int>();
			for (int s=0; s< search.edgeTo.Length; s++) {
				if (search.edgeTo[s] != -1) {
					meshVertexIndexMarks[s] = uvIslandIndex;
					foundVerts.Add(s);
				}
			}

			UvIsland uvIsland = new UvIsland();
			uvIsland.vertexIndex = foundVerts.ToArray();
			uvIsland.edgesList = new List<Edge>();
			uvIsland.borderList = new List<Edge>();
			result.Add(uvIsland);
		}

		return result;
	}




	static List<UvIsland> RemoveDuplicateUvIslands(List<UvIsland> islands) {

		List<UvIsland> result = new List<UvIsland>();

		for (int i=0; i < islands.Count; i++) {
			bool isIslandUniq = true;
			for (int s=0; s < result.Count; s++) {
				if (islands[i].border.Length == result[s].border.Length) {

					if(IsUvIslandBorderBoundsEqual(islands[i].bounds, result[s].bounds)) {

						isIslandUniq = false;
						break;
						
					}

				}
			}  

			if (isIslandUniq) {
				result.Add(islands[i]);
			}
		}

		return result; 
	}


	static bool IsUvIslandBorderBoundsEqual(Bounds2D boundsA, Bounds2D boundsB) {
		bool result = true;

		float t = 0.001f;

		if ((Mathf.Abs(boundsA.minX - boundsB.minX) > t) ||
			(Mathf.Abs(boundsA.maxX - boundsB.maxX) > t) ||

			(Mathf.Abs(boundsA.minY - boundsB.minY) > t) ||
			(Mathf.Abs(boundsA.maxY - boundsB.maxY) > t) ) {

			result = false;
		}

		return result;
	}

	public class Bounds2D {
		public float minX;
		public float maxX;
		
		public float minY;
		public float maxY;
	}


	static Bounds2D GetUvIslandBorderBounds (Edge[] border, Vector2[] uvs) {
		Bounds2D result = new Bounds2D(); 

		result.minX = Mathf.Infinity;
		result.maxX = Mathf.NegativeInfinity;
		
		result.minY = Mathf.Infinity;
		result.maxY = Mathf.NegativeInfinity;
		
		for (int i=0; i< border.Length; i++) {
			if (uvs[border[i].vertexIndex[0]].x < result.minX) result.minX = uvs[border[i].vertexIndex[0]].x;
			if (uvs[border[i].vertexIndex[0]].x > result.maxX) result.maxX = uvs[border[i].vertexIndex[0]].x;
			
			if (uvs[border[i].vertexIndex[0]].y < result.minY) result.minY = uvs[border[i].vertexIndex[0]].y;
			if (uvs[border[i].vertexIndex[0]].y > result.maxY) result.maxY = uvs[border[i].vertexIndex[0]].y;
		}

		return result;
	}



	static List<UvIsland> uvIslands;
	static List<UvIsland> uniqUvIslands;

	static void PrepareUVs() {
		uvGuidesUpdated = false;

		uvVectorPairsBorder = new List<Vector3P>(); 
		uvVectorPairsBorderIDs = new List<int>();
		uvHelperVectorPairs = new List<Vector3P>();
	
		uvHelperSnapPoints = new List<Vector3>(); 

		int uvGrisDensityPow = Surforge.surforgeSettings.uvGridStep;
		if (uvGrisDensityPow < 1) uvGrisDensityPow = 1;
		if (uvGrisDensityPow > 5) uvGrisDensityPow = 5;
		float uvGridDensity = Mathf.Pow(2, uvGrisDensityPow);
	
		float uvSizeX = Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX;
		float uvSizeZ = Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ;
		float uvSize = uvSizeX;
		if (uvSizeZ > uvSizeX) uvSize = uvSizeZ;
	
		float limitsMinY = -0.5f;
		if (Surforge.surforgeSettings.limits != null) limitsMinY = Surforge.surforgeSettings.limits.minY;
	
		float uvOffsetX = Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f;
		float uvOffsetZ = Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f;
	
		Handles.color = uvLines;
	
		MeshFilter meshFilter = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();
		if (meshFilter != null) {

			Mesh mesh = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>().sharedMesh;


			if (mesh == null) return;
			if (mesh.uv.Length < 3) {
				Debug.Log ("Mesh has no UV information. Please create UV layout for your mesh");
				return;
			}
			

			Vector2[] uvs = mesh.uv;


			//Build all edges
			Edge[] edges = BuildAllEdges(mesh);

				
			//Build all border
			List<Edge> culledEdges = new List<Edge>();
			for (int i=0; i < edges.Length; i++) { 
				if (edges[i].faceIndex[0] == edges[i].faceIndex[1]) {
					culledEdges.Add(edges[i]);
				}
			}
			Edge[] border = culledEdges.ToArray();




			//----Create UV islands----
			
			uvIslands = CreateUvIslands(mesh, edges);

			//Debug.Log("uvIslands count: " + uvIslands.Count);

			// -- new border edges assign --
			for (int i=0; i< border.Length; i++) {
				uvIslands[meshVertexIndexMarks[border[i].vertexIndex[0]]-1].borderList.Add(border[i]);
			}

			// -- FILTER BORDER ---
			for (int u=0; u < uvIslands.Count; u++) {
				uvIslands[u].border = FilterUvIslandBorder(uvIslands[u].borderList.ToArray(), uvs);

				//uvIslands[u].border = uvIslands[u].borderList.ToArray();
			}


			// -- assign bounds to UV islands --
			for (int u=0; u < uvIslands.Count; u++) {
				uvIslands[u].bounds = GetUvIslandBorderBounds(uvIslands[u].border, uvs); 
			}



			// -- new edges assign --
			for (int i=0; i< edges.Length; i++) {
				uvIslands[meshVertexIndexMarks[edges[i].vertexIndex[0]]-1].edgesList.Add(edges[i]);
			}


			// -- filter UV islands --
			uniqUvIslands = RemoveDuplicateUvIslands(uvIslands); 


			// -- create filtered edges array --
			List<Edge> fileterdEdges = new List<Edge>();

			for (int u=0; u < uniqUvIslands.Count; u++) {
				uniqUvIslands[u].edges = uniqUvIslands[u].edgesList.ToArray();
				for (int i=0; i< uniqUvIslands[u].edges.Length; i++) {
					fileterdEdges.Add(uniqUvIslands[u].edges[i]);
				}
			}


			// -- build uvVectorPairs and uvPoints from filtered edges
			List<Vector3P> filteredUvVectorPairs = new List<Vector3P>();
			bool[] uvPointIndexes = new bool[uvs.Length];

			for (int i=0; i< fileterdEdges.Count; i++) {
				Vector3P pair = new Vector3P();
				pair.index_a = fileterdEdges[i].vertexIndex[0];
				pair.index_b = fileterdEdges[i].vertexIndex[1];
				pair.a = new Vector3((uvs[pair.index_a].x - 0.5f) * uvSize + uvOffsetX, limitsMinY, (uvs[pair.index_a].y - 0.5f) * uvSize + uvOffsetZ);
				pair.b = new Vector3((uvs[pair.index_b].x - 0.5f) * uvSize + uvOffsetX, limitsMinY, (uvs[pair.index_b].y - 0.5f) * uvSize + uvOffsetZ);

				filteredUvVectorPairs.Add(pair);

				uvPointIndexes[pair.index_a] = true;
				uvPointIndexes[pair.index_b] = true;
			}
			uvVectorPairs = filteredUvVectorPairs.ToArray();


			// -- create uniq UV points
			List<Vector3> filteredUvPoints = new List<Vector3>();
			for (int i=0; i< uvPointIndexes.Length; i++) {
				if (uvPointIndexes[i] == true) {
					filteredUvPoints.Add(new Vector3((uvs[i].x - 0.5f) * uvSize + uvOffsetX, limitsMinY, (uvs[i].y - 0.5f) * uvSize + uvOffsetZ));
				}
			}
			uvPoints = filteredUvPoints.ToArray();
			//Debug.Log("filtered uvPoints: "  + uvPoints.Length);



			//-- create uvVectorPairsBorder --

			int uvBorderCounter = 0;


			for (int u=0; u < uniqUvIslands.Count; u++) {

				uniqUvIslands[u].pairs = new List<Vector3P>();

				for (int i=0; i < uniqUvIslands[u].border.Length; i++) {
					Vector3P pair = new Vector3P();
					pair.a = new Vector3((uvs[uniqUvIslands[u].border[i].vertexIndex[0]].x - 0.5f) * uvSize + uvOffsetX, limitsMinY, (uvs[uniqUvIslands[u].border[i].vertexIndex[0]].y - 0.5f) * uvSize + uvOffsetZ);
					pair.b = new Vector3((uvs[uniqUvIslands[u].border[i].vertexIndex[1]].x - 0.5f) * uvSize + uvOffsetX, limitsMinY, (uvs[uniqUvIslands[u].border[i].vertexIndex[1]].y - 0.5f) * uvSize + uvOffsetZ);

					pair.index_a = uniqUvIslands[u].border[i].vertexIndex[0];
					pair.index_b = uniqUvIslands[u].border[i].vertexIndex[1];

					uvVectorPairsBorder.Add(pair);  //
					uvVectorPairsBorderIDs.Add (uvBorderCounter);//

					uniqUvIslands[u].pairs.Add(pair);
				}
				uvBorderCounter++;
			}



			
			if (showUVsHelperGrid) {


				for (int d=0; d < uniqUvIslands.Count; d++) {

					float minX = Mathf.Infinity;
					float minZ = Mathf.Infinity;
					float maxX = Mathf.NegativeInfinity;
					float maxZ = Mathf.NegativeInfinity;
					for (int i=0; i < uniqUvIslands[d].pairs.Count; i++) {
						if (uniqUvIslands[d].pairs[i].a.x > maxX) maxX = uniqUvIslands[d].pairs[i].a.x;
						if (uniqUvIslands[d].pairs[i].a.x < minX) minX = uniqUvIslands[d].pairs[i].a.x;
						if (uniqUvIslands[d].pairs[i].a.z > maxZ) maxZ = uniqUvIslands[d].pairs[i].a.z;
						if (uniqUvIslands[d].pairs[i].a.z < minZ) minZ = uniqUvIslands[d].pairs[i].a.z;
					}

					Vector3 shapeCenter = new Vector3((minX + maxX) * 0.5f, 0, (minZ + maxZ) * 0.5f);

					List<Vector3> borderShape = GetBorderShape(uniqUvIslands[d].pairs, false); 

					Vector2[] borderShape2d = Surforge.Points3DTo2D(borderShape.ToArray());



					bool isIslandFullUvSpaceZ = false;
					if (Mathf.Approximately(minZ, Surforge.surforgeSettings.textureBorders.minZ) && Mathf.Approximately(maxZ, Surforge.surforgeSettings.textureBorders.maxZ)) {
						isIslandFullUvSpaceZ = true;
					}
					
					bool isIslandFullUvSpaceX = false;
					if (Mathf.Approximately(minX, Surforge.surforgeSettings.textureBorders.minX) && Mathf.Approximately(maxX, Surforge.surforgeSettings.textureBorders.maxX)) {
						isIslandFullUvSpaceX = true;
					}



					linesX = new List<Line2d>();
					float stepZ = Surforge.surforgeSettings.textureBorders.maxZ / uvGridDensity;
					if (isIslandFullUvSpaceZ && isIslandFullUvSpaceX) stepZ = Surforge.surforgeSettings.textureBorders.maxZ / 2.0f;
					int fullStepsZ = (int)Mathf.Floor((float)(maxZ - minZ) *0.5f / stepZ);


					Line2d centerLineX = new Line2d();
					centerLineX.start = new Vector2(-50.0f, shapeCenter.z );  
					centerLineX.end = new Vector2(centerLineX.start.x + 100.0f, centerLineX.start.y);
					if (isIslandFullUvSpaceZ) {
						centerLineX.lineBrightness = 3;
					}
					else {
						centerLineX.lineBrightness = 1;
					}
					linesX.Add(centerLineX);

					for (int i=1; i < fullStepsZ+1; i++) {
						Line2d line = new Line2d();
						line.start = new Vector2(-50.0f, shapeCenter.z + stepZ * i);
						line.end = new Vector2(line.start.x + 100.0f, line.start.y);
						if ((isIslandFullUvSpaceZ) && (i == fullStepsZ/2)) line.lineBrightness = 2;
						linesX.Add(line);
					}
					for (int i=1; i < fullStepsZ+1; i++) {
						Line2d line = new Line2d();
						line.start = new Vector2(-50.0f, shapeCenter.z - stepZ * i);
						line.end = new Vector2(line.start.x + 100.0f, line.start.y);
						if ((isIslandFullUvSpaceZ) && (i == fullStepsZ/2)) line.lineBrightness = 2;
						linesX.Add(line);
					}




					linesZ = new List<Line2d>();
					float stepX = Surforge.surforgeSettings.textureBorders.maxX / uvGridDensity;
					if (isIslandFullUvSpaceZ && isIslandFullUvSpaceX) stepX = Surforge.surforgeSettings.textureBorders.maxX / 2.0f;
					int fullStepsX = (int)Mathf.Floor((float)(maxX - minX) *0.5f / stepX);


					Line2d centerLineZ = new Line2d();
					centerLineZ.start = new Vector2( shapeCenter.x, -50.0f);
					centerLineZ.end = new Vector2(centerLineZ.start.x, centerLineZ.start.y + 100.0f);
					if (isIslandFullUvSpaceX) {
						centerLineZ.lineBrightness = 3;
					}
					else {
						centerLineZ.lineBrightness = 1;
					}
					linesZ.Add(centerLineZ);

					for (int i=1; i < fullStepsX+1; i++) {
						Line2d line = new Line2d();
						line.start = new Vector2(shapeCenter.x + stepX * i, -50.0f);
						line.end = new Vector2(line.start.x, line.start.y + 100.0f);
						if ((isIslandFullUvSpaceX) && (i == fullStepsX/2)) line.lineBrightness = 2;
						linesZ.Add(line);
					}
					for (int i=1; i < fullStepsX+1; i++) { 
						Line2d line = new Line2d();
						line.start = new Vector2(shapeCenter.x - stepX * i, -50.0f);
						line.end = new Vector2(line.start.x, line.start.y + 100.0f);
						if ((isIslandFullUvSpaceX) && (i == fullStepsX/2)) line.lineBrightness = 2;
						linesZ.Add(line);
					}




					for (int i=0; i < linesX.Count; i++) {
						CreateUvHelperPairs(borderShape2d, linesX[i], false, limitsMinY, d);
					}

					for (int i=0; i < linesZ.Count; i++) {
						CreateUvHelperPairs(borderShape2d, linesZ[i], true, limitsMinY, d);
					}


					//Add border shape points
					for (int i=0; i < borderShape2d.Length; i++) {
						uvHelperSnapPoints.Add(new Vector3(borderShape2d[i].x, limitsMinY, borderShape2d[i].y));
					}



					

				}


				
				//find intersections
				for (int p =0; p < uvHelperVectorPairs.Count; p++) {
					for (int o =0; o < uvHelperVectorPairs.Count; o++) {
						if (uvHelperVectorPairs[p].uvIslandIndex == uvHelperVectorPairs[o].uvIslandIndex) {
							if (p != o) {
							
								Vector2 ps1 = new Vector2(uvHelperVectorPairs[p].a.x, uvHelperVectorPairs[p].a.z);
								Vector2 pe1 = new Vector2(uvHelperVectorPairs[p].b.x, uvHelperVectorPairs[p].b.z);
								Vector2 ps2 = new Vector2(uvHelperVectorPairs[o].a.x, uvHelperVectorPairs[o].a.z);
								Vector2 pe2 = new Vector2(uvHelperVectorPairs[o].b.x, uvHelperVectorPairs[o].b.z);
							
								if (Surforge.TestLinesIntersection(ps1, pe1, ps2, pe2)) {
									if (Surforge.TestSegmentIntersection(ps1, pe1, ps2, pe2)) {
										Vector2 intersection = Surforge.LineIntersectionPoint(ps1, pe1, ps2, pe2);
										uvHelperSnapPoints.Add(new Vector3(intersection.x, uvHelperVectorPairs[p].a.y, intersection.y)); 
									}
								}
							}
						}
					}
				}


				uvGuidesUpdated = true;

			}

		}
		Surforge.surforgeSettings.skyboxNeedWindowUpdate = true;
		Surforge.surforgeSettings.skyboxWindowRepaintTimer = Time.realtimeSinceStartup + 2.0f; 
	}
	
	
	static void CombineBorderIDs(int newIds, int oldIds) {
		for (int i=0; i < uvVectorPairsBorderIDs.Count; i++) {
			if (uvVectorPairsBorderIDs[i] == oldIds) uvVectorPairsBorderIDs[i] = newIds;
		}
	}
	
	static int GetBorderIDsTotal() {
		List<int> uniqIds = new List<int>();
		
		for (int d=0; d< uvVectorPairsBorder.Count; d++) {
			bool isIdUniq = true;
			for (int i=0; i < uniqIds.Count; i++) {
				if (uvVectorPairsBorderIDs[d] == uniqIds[i]) {
					isIdUniq = false;
					break;
				}
			}
			if (isIdUniq) {
				uniqIds.Add(uvVectorPairsBorderIDs[d]);
			}
		}
		return uniqIds.Count;
	}

	static List<int> GetBorderIDsList() {
		List<int> uniqIds = new List<int>();
		
		for (int d=0; d< uvVectorPairsBorder.Count; d++) {
			bool isIdUniq = true;
			for (int i=0; i < uniqIds.Count; i++) {
				if (uvVectorPairsBorderIDs[d] == uniqIds[i]) {
					isIdUniq = false;
					break;
				}
			}
			if (isIdUniq) {
				uniqIds.Add(uvVectorPairsBorderIDs[d]);
			}
		}
		return uniqIds;
	}
	
	
	static void RemoveNonUniqVectorPairs(List<Vector3P> pairsToFilter) {
		List<Vector3P> pairsTmp = new List<Vector3P>(); 
		for (int p = 0; p < pairsToFilter.Count; p++) {
			pairsTmp.Add(pairsToFilter[p]);
		}
		pairsToFilter.Clear();
		
		for (int p = 0; p < pairsTmp.Count; p++) {
			bool isPointUniq = true;
			for (int o = 0; o < pairsToFilter.Count; o++) {
				if ( (Vector3Equal(pairsTmp[p].a, pairsToFilter[o].a)) && (Vector3Equal(pairsTmp[p].b, pairsToFilter[o].b)) ) {
					isPointUniq = false;
					break;
				}
			}
			if (isPointUniq) pairsToFilter.Add(pairsTmp[p]);
		}
	}
	
	
	static void RemoveNonUniqHelperPoints(List<Vector3> pointsToFilter, List<int> borderIDs, List<int> subdLevels) {
		List<Vector3> pointsTmp = new List<Vector3>(); 
		for (int p = 0; p < pointsToFilter.Count; p++) {
			pointsTmp.Add(pointsToFilter[p]);
		}
		pointsToFilter.Clear();
		
		List<int> borderIDsTmp = new List<int>(); 
		for (int p = 0; p < borderIDs.Count; p++) {
			borderIDsTmp.Add(borderIDs[p]);
		}
		borderIDs.Clear();
		
		List<int> subdLevelsTmp = new List<int>(); 
		for (int p = 0; p < subdLevels.Count; p++) {
			subdLevelsTmp.Add(subdLevels[p]);
		}
		subdLevels.Clear();
		
		for (int p = 0; p < pointsTmp.Count; p++) {
			bool isPointUniq = true;
			for (int o = 0; o < pointsToFilter.Count; o++) {
				if (Vector3Equal(pointsTmp[p], pointsToFilter[o])) {
					isPointUniq = false;
					break;
				}
			}
			if (isPointUniq) {
				pointsToFilter.Add(pointsTmp[p]);
				borderIDs.Add (borderIDsTmp[p]);
				subdLevels.Add (subdLevelsTmp[p]);
			}
		}
	}
	
	
	static bool TestFloatsEqualTreshold(float a, float b) {
		bool result = false;
		float treshold = 0.02f; 
		if ( (a < (b + treshold)) && (a > (b - treshold)) ) {
			result = true;
		}
		return result;
	}


	static bool Vector3Equal(Vector3 a, Vector3 b) {
		float threshold = 0.00001f; 
		
		if (Vector3.Distance(a, b) > threshold) return false;
		else return true;
	}


	static Material lineMaterial;

	static void CreateLineMaterial() {

		if (!lineMaterial) {
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt ("_ZWrite", 0);
			lineMaterial.SetInt ("_ZTest", 0);
		}

	}
	

	
	static void ShowUVs() {
		if (Surforge.surforgeSettings.limits == null) Surforge.CreateLimits();

		MeshFilter meshFilter = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();
		if (meshFilter != null) {
			uvsFound = true;
		}

		
		CreateLineMaterial();
		lineMaterial.SetPass (0);

		GL.PushMatrix ();
		// Draw lines
		GL.Begin (GL.LINES);



		if (showUVsHelperGrid) { 

			for (int n=0; n < uvHelperVectorPairs.Count; n++) {
				if (uvHelperVectorPairs[n].subdLevel == 0) GL.Color(uvHelperLines0);
				if (uvHelperVectorPairs[n].subdLevel == 1) GL.Color(uvHelperLines1);
				if (uvHelperVectorPairs[n].subdLevel == 2) GL.Color(uvHelperLines2);
				if (uvHelperVectorPairs[n].subdLevel == 3) GL.Color(uvHelperLines3);
				GL.Vertex3 (uvHelperVectorPairs[n].a.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvHelperVectorPairs[n].a.z * rootScaleZ);
				GL.Vertex3 (uvHelperVectorPairs[n].b.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvHelperVectorPairs[n].b.z * rootScaleZ);
			}

		}

		
		if (showUVs) {

			GL.Color(uvLines);
			for (int n=0; n < uvVectorPairs.Length; n++) {
				GL.Vertex3 (uvVectorPairs[n].a.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvVectorPairs[n].a.z * rootScaleZ);
				GL.Vertex3 (uvVectorPairs[n].b.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvVectorPairs[n].b.z * rootScaleZ);
			}
		}


		if (showUVsHelperGrid || showUVs) {

			GL.Color(uvLinesBorder);
			for (int n=0; n < uvVectorPairsBorder.Count; n++) {
				GL.Vertex3 (uvVectorPairsBorder[n].a.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvVectorPairsBorder[n].a.z * rootScaleZ);
				GL.Vertex3 (uvVectorPairsBorder[n].b.x * rootScaleX, Surforge.surforgeSettings.limits.minY, uvVectorPairsBorder[n].b.z * rootScaleZ);
			}
		}

		//show texture model match point
		bool showModelMatchPoint = false;
		if (Event.current != null) {
			if (Event.current.control) {
				showModelMatchPoint = true;

				//Debug.Log ("ctrl pressed");
				if (window != null) {
					if(EditorWindow.mouseOverWindow == window) {
						EditorWindow.mouseOverWindow.Focus();
						 
					}
				}

			}
		}
		if (showModelMatchPoint && isCursorOverModel) {
			float uvXsize = (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * rootScaleX;
			float uvZsize = (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * rootScaleZ;
			Vector3 matchedUvPoint = new Vector3( uvXsize * texturePointMatchModelPoint.x -  uvXsize * 0.5f, 
			                                     0, 
			                                     uvZsize * texturePointMatchModelPoint.y - uvZsize * 0.5f);  

			List<Vector3> highlightedBorder =  GetUvBorderPointsAroundPoint(matchedUvPoint);

			GL.Color(Color.white);
			for (int n=0; n < highlightedBorder.Count; n++) {
				Vector3 p1 = new Vector3(highlightedBorder[n].x, Surforge.surforgeSettings.limits.minY, highlightedBorder[n].z);
				Vector3 p2 = new Vector3();
				if (n == (highlightedBorder.Count - 1)) p2 = new Vector3(highlightedBorder[0].x, Surforge.surforgeSettings.limits.minY, highlightedBorder[0].z);
				else p2 = new Vector3(highlightedBorder[n+1].x, Surforge.surforgeSettings.limits.minY, highlightedBorder[n+1].z);

				GL.Vertex3(p1.x, p1.y, p1.z);
				GL.Vertex3(p2.x, p2.y, p2.z);
			}
		}



		GL.End ();
		GL.PopMatrix ();
	}
	
	
	
	
	static Color gridLines = new Color(0.5f, 0.5f, 0.5f, 0.2f); 
	static Color gridMediumLines = new Color(0.5f, 0.5f, 0.5f, 0.2f);  
	static Color gridBrightLines = new Color(0.8f, 0.8f, 0.8f, 0.5f); 
	
	static void ShowGrid() {
		Handles.color = gridLines;

		float rootRateX = rootScaleX;
		float rootRateZ = rootScaleZ;

		int gridLinesX = Mathf.RoundToInt((Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * rootRateX);
		int gridLinesZ = Mathf.RoundToInt((Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * rootRateZ);

		int scaledMinX = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.minX * rootRateX);
		int scaledMaxX = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxX * rootRateX);

		int scaledMinZ = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.minZ * rootRateZ);
		int scaledMaxZ = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxZ * rootRateZ);

		float limitsMinY = -0.5f;
		if (Surforge.surforgeSettings.limits != null) limitsMinY = Surforge.surforgeSettings.limits.minY;

		for (int x=0; x <= scaledMaxX; x++) {
			Handles.DrawLine(new Vector3(x, limitsMinY, scaledMaxZ), new Vector3(x, limitsMinY, scaledMinZ));
		}

		for (int x=scaledMinX; x < 0; x++) {
			Handles.DrawLine(new Vector3(x, limitsMinY, scaledMaxZ), new Vector3(x, limitsMinY, scaledMinZ));
		}

		for (int z=0; z <= scaledMaxZ; z++) {
			Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, z), new Vector3(scaledMinX, limitsMinY, z ));
		}

		for (int z=scaledMinZ; z < 0; z++) {
			Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, z), new Vector3(scaledMinX, limitsMinY, z ));
		}

		

		if ((gridLinesX % 2) == 0) {
			Handles.color = gridBrightLines;
			Handles.DrawLine(new Vector3(scaledMinX + gridLinesX * 0.5f, limitsMinY, scaledMaxZ), new Vector3(scaledMinX + gridLinesX * 0.5f, limitsMinY, scaledMinZ));
			if ((gridLinesX % 4) == 0) {
				Handles.color = gridMediumLines;
				Handles.DrawLine(new Vector3(scaledMinX + gridLinesX * 0.25f, limitsMinY, scaledMaxZ), new Vector3(scaledMinX + gridLinesX * 0.25f, limitsMinY, scaledMinZ));
				Handles.DrawLine(new Vector3(scaledMinX + (gridLinesX * 0.25f) * 3, limitsMinY, scaledMaxZ), new Vector3(scaledMinX + (gridLinesX * 0.25f) * 3, limitsMinY, scaledMinZ));
				
			}
		}
		else if ((gridLinesX % 3) == 0) {
			Handles.color = gridBrightLines;
			Handles.DrawLine(new Vector3(scaledMinX + gridLinesX / 3, limitsMinY, scaledMaxZ), new Vector3(scaledMinX + gridLinesX / 3, limitsMinY, scaledMinZ));
			Handles.DrawLine(new Vector3(scaledMinX + (gridLinesX / 3) * 2, limitsMinY, scaledMaxZ), new Vector3(scaledMinX + (gridLinesX / 3) * 2, limitsMinY, scaledMinZ));
		}
		
		
		
		
		if ((gridLinesZ % 2) == 0) {
			Handles.color = gridBrightLines;
			Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, scaledMinZ + gridLinesZ * 0.5f), new Vector3(scaledMinX, limitsMinY, scaledMinZ + gridLinesZ * 0.5f));
			if ((gridLinesZ % 4) == 0) {
				Handles.color = gridMediumLines;
				Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, scaledMinZ + gridLinesZ * 0.25f), new Vector3(scaledMinX, limitsMinY, scaledMinZ + gridLinesZ * 0.25f));
				Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, scaledMinZ + (gridLinesZ * 0.25f) * 3), new Vector3(scaledMinX, limitsMinY, scaledMinZ + (gridLinesZ * 0.25f) * 3));
			}
		}
		else if ((gridLinesZ % 3) == 0) {
			Handles.color = gridBrightLines;
			Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, scaledMinZ + gridLinesZ / 3), new Vector3(scaledMinX, limitsMinY, scaledMinZ + gridLinesZ / 3));
			Handles.DrawLine(new Vector3(scaledMaxX, limitsMinY, scaledMinZ + (gridLinesZ / 3) * 2), new Vector3(scaledMinX, limitsMinY, scaledMinZ + (gridLinesZ / 3)*2));		
		}

	}
	
	
	
	static Vector3 handleMaxX;
	static Vector3 handleMinX;
	
	static Vector3 handleMaxY;
	static Vector3 handleMinY;
	
	static Vector3 handleMaxZ;
	static Vector3 handleMinZ;
	
	static Vector3 snapLimit = new Vector3(11, 11, 11);
	static float handleSizeMult = 0.065f;
	
	static Color limitsBorder = new Color(0, 0.95f, 2, 2); 	//blue
	static Color limitsThinBorder = new Color(0, 0.95f, 2, 0); 	//blue transparent
	static Color limitsPlane = new Color(0, 0.5f, 2, 0.01f);
	
	
	
	static float RoundTo05(float value) {
		float result = 0;
		float abs = Mathf.Abs(value);
		result = Mathf.Floor(abs) + 0.5f;
		if (value < 0) result = result * -1;
		return result;
	}

	static float RoundTo025And075(float value) {
		float result = 0;
		float abs = Mathf.Abs(value);
		if (abs - Mathf.Floor(abs) < 0.5f)  {
			result = Mathf.Floor(abs) + 0.25f;
		}
		else {
			result = Mathf.Floor(abs) + 0.75f;
		}
		if (value < 0) result = result * -1;
		return result;
	}


	

	static Color mirrorPlane = new Color(0.7f, 0.5f, 1.5f, 2);

	static void ShowMirrorPlanes() {
		if (Surforge.surforgeSettings) {
			if ((Surforge.surforgeSettings.textureBorders) && (Surforge.surforgeSettings.limits) ) {

				Vector2 topLeft = new Vector2(Surforge.surforgeSettings.textureBorders.minZ * rootScaleX, Surforge.surforgeSettings.textureBorders.maxX * rootScaleZ);
				Vector2 topRight = new Vector2(Surforge.surforgeSettings.textureBorders.maxZ * rootScaleX, Surforge.surforgeSettings.textureBorders.maxX * rootScaleZ);
				Vector2 bottomLeft = new Vector2(Surforge.surforgeSettings.textureBorders.minZ * rootScaleX, Surforge.surforgeSettings.textureBorders.minX * rootScaleZ);
				Vector2 bottomRight = new Vector2(Surforge.surforgeSettings.textureBorders.maxZ * rootScaleX, Surforge.surforgeSettings.textureBorders.minX * rootScaleZ);

				Vector2 blueDirection = new Vector2(Surforge.surforgeSettings.mirrorLineSolid[1].x, Surforge.surforgeSettings.mirrorLineSolid[1].z) - new Vector2(Surforge.surforgeSettings.mirrorLineSolid[0].x, Surforge.surforgeSettings.mirrorLineSolid[0].z);
				Vector2 bluePoint0 = new Vector2(Surforge.surforgeSettings.mirrorLineSolid[0].x, Surforge.surforgeSettings.mirrorLineSolid[0].z) + blueDirection * 1000.0f;
				Vector2 bluePoint1 = new Vector2(Surforge.surforgeSettings.mirrorLineSolid[0].x, Surforge.surforgeSettings.mirrorLineSolid[0].z) + blueDirection * -1000.0f;

				Vector2 redDirection = new Vector2(Surforge.surforgeSettings.mirrorLineDotted[1].x, Surforge.surforgeSettings.mirrorLineDotted[1].z) - new Vector2(Surforge.surforgeSettings.mirrorLineDotted[0].x, Surforge.surforgeSettings.mirrorLineDotted[0].z);
				Vector2 redPoint0 = new Vector2(Surforge.surforgeSettings.mirrorLineDotted[0].x, Surforge.surforgeSettings.mirrorLineDotted[0].z) + redDirection * 1000.0f;
				Vector2 redPoint1 = new Vector2(Surforge.surforgeSettings.mirrorLineDotted[0].x, Surforge.surforgeSettings.mirrorLineDotted[0].z) + redDirection * -1000.0f;

				List<Vector2> blueIntersections = new List<Vector2>();
				if (Surforge.TestSegmentIntersection(topLeft, topRight, bluePoint0, bluePoint1)) blueIntersections.Add (Surforge.LineIntersectionPoint(topLeft, topRight, bluePoint0, bluePoint1));
				if (Surforge.TestSegmentIntersection(bottomRight, bottomLeft, bluePoint0, bluePoint1)) blueIntersections.Add (Surforge.LineIntersectionPoint(bottomRight, bottomLeft, bluePoint0, bluePoint1));
				if (Surforge.TestSegmentIntersection(topRight, bottomRight, bluePoint0, bluePoint1)) blueIntersections.Add (Surforge.LineIntersectionPoint(topRight, bottomRight, bluePoint0, bluePoint1));
				if (Surforge.TestSegmentIntersection(bottomLeft, topLeft, bluePoint0, bluePoint1)) blueIntersections.Add (Surforge.LineIntersectionPoint(bottomLeft, topLeft, bluePoint0, bluePoint1));

				List<Vector2> redIntersections = new List<Vector2>();
				if (Surforge.TestSegmentIntersection(topLeft, topRight, redPoint0, redPoint1)) redIntersections.Add (Surforge.LineIntersectionPoint(topLeft, topRight, redPoint0, redPoint1));
				if (Surforge.TestSegmentIntersection(bottomRight, bottomLeft, redPoint0, redPoint1)) redIntersections.Add (Surforge.LineIntersectionPoint(bottomRight, bottomLeft, redPoint0, redPoint1));
				if (Surforge.TestSegmentIntersection(topRight, bottomRight, redPoint0, redPoint1)) redIntersections.Add (Surforge.LineIntersectionPoint(topRight, bottomRight, redPoint0, redPoint1));
				if (Surforge.TestSegmentIntersection(bottomLeft, topLeft, redPoint0, redPoint1)) redIntersections.Add (Surforge.LineIntersectionPoint(bottomLeft, topLeft, redPoint0, redPoint1));


				Handles.color = mirrorPlane; 
				if (blueIntersections.Count > 1) {
					Handles.DrawLine(new Vector3(blueIntersections[0].x, Surforge.surforgeSettings.limits.minY, blueIntersections[0].y), new Vector3(blueIntersections[1].x, Surforge.surforgeSettings.limits.minY, blueIntersections[1].y));	
				}
				if (redIntersections.Count > 1) {
					Handles.DrawDottedLine(new Vector3(redIntersections[0].x, Surforge.surforgeSettings.limits.minY, redIntersections[0].y), new Vector3(redIntersections[1].x, Surforge.surforgeSettings.limits.minY, redIntersections[1].y), 4.0f);	
				}

			}
		}

	}


	static void ShowLimits() {
		Handles.color = limitsBorder;
		
		Vector3 centerHandle = new Vector3();
		if (hideVolumeBorders) centerHandle = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		else centerHandle = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		
		Vector3 centerHandleOld = new Vector3(centerHandle.x, centerHandle.y, centerHandle.z);
		
		centerHandle = Handles.FreeMoveHandle(centerHandle + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxX) * handleSizeMult, snapLimit, Handles.DotCap);
		centerHandle = centerHandle - new Vector3(0.25f, 0, 0.25f);

		if (sceneCameraDirection == 1) {
			centerHandle = new Vector3(centerHandle.x, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, centerHandle.z);
			centerHandleOld = new Vector3(centerHandleOld.x, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, centerHandleOld.z);
		}
		if (sceneCameraDirection == 2) {
			centerHandle = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, centerHandle.y, centerHandle.z);
			centerHandleOld = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, centerHandleOld.y, centerHandleOld.z);
		}
		if (sceneCameraDirection == 0) {
			centerHandle = new Vector3(centerHandle.x, centerHandle.y, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
			centerHandleOld = new Vector3(centerHandleOld.x, centerHandleOld.y, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		}
		
		
		Surforge.surforgeSettings.limits.maxX = Surforge.surforgeSettings.limits.maxX + (centerHandle.x - centerHandleOld.x);
		Surforge.surforgeSettings.limits.minX = Surforge.surforgeSettings.limits.minX + (centerHandle.x - centerHandleOld.x); 
		
		Surforge.surforgeSettings.limits.maxY = Surforge.surforgeSettings.limits.maxY + (centerHandle.y - centerHandleOld.y);
		Surforge.surforgeSettings.limits.minY = Surforge.surforgeSettings.limits.minY + (centerHandle.y - centerHandleOld.y); 
		
		Surforge.surforgeSettings.limits.maxZ = Surforge.surforgeSettings.limits.maxZ + (centerHandle.z - centerHandleOld.z);
		Surforge.surforgeSettings.limits.minZ = Surforge.surforgeSettings.limits.minZ + (centerHandle.z - centerHandleOld.z); 
		
		
		handleMaxX = new Vector3(Surforge.surforgeSettings.limits.maxX, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (sceneCameraDirection != 2) {
			if (hideVolumeBorders) {
				Vector3 handleMaxXFlat = new Vector3(Surforge.surforgeSettings.limits.maxX, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
				handleMaxX = Handles.FreeMoveHandle(handleMaxXFlat + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxX) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMaxX = handleMaxX - new Vector3(0.25f, 0, 0.25f);
			}
			else {
				handleMaxX = Handles.FreeMoveHandle(handleMaxX + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxX) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMaxX = handleMaxX - new Vector3(0.25f, 0, 0.25f);
			}
		}
		handleMaxX = new Vector3(RoundTo025And075(handleMaxX.x), Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (handleMaxX.x < Surforge.surforgeSettings.limits.minX) handleMaxX = new Vector3(Surforge.surforgeSettings.limits.minX, handleMaxX.y, handleMaxX.z);
		Surforge.surforgeSettings.limits.maxX = handleMaxX.x;
		
		handleMinX = new Vector3(Surforge.surforgeSettings.limits.minX, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (sceneCameraDirection != 2) {
			if (hideVolumeBorders) {
				Vector3 handleMinXFlat = new Vector3(Surforge.surforgeSettings.limits.minX, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
				handleMinX = Handles.FreeMoveHandle(handleMinXFlat + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMinX) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMinX = handleMinX - new Vector3(0.25f, 0, 0.25f);
			}
			else {
				handleMinX = Handles.FreeMoveHandle(handleMinX + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMinX) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMinX = handleMinX - new Vector3(0.25f, 0, 0.25f);
			}
		}
		handleMinX = new Vector3(RoundTo025And075(handleMinX.x), Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (handleMinX.x > Surforge.surforgeSettings.limits.maxX) handleMinX = new Vector3(Surforge.surforgeSettings.limits.maxX, handleMinX.y, handleMinX.z);
		Surforge.surforgeSettings.limits.minX = handleMinX.x;
		
		handleMaxY = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.maxY, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (sceneCameraDirection != 1) {
			handleMaxY = Handles.FreeMoveHandle(handleMaxY + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxY) * handleSizeMult, snapLimit, Handles.DotCap);
			handleMaxY = handleMaxY - new Vector3(0.25f, 0, 0.25f);
		}
		handleMaxY = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, RoundTo025And075(handleMaxY.y), Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (handleMaxY.y < Surforge.surforgeSettings.limits.minY) handleMaxY = new Vector3(handleMaxY.x, Surforge.surforgeSettings.limits.minY, handleMaxY.z);
		Surforge.surforgeSettings.limits.maxY = handleMaxY.y;
		
		handleMinY = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (sceneCameraDirection != 1) {
			handleMinY = Handles.FreeMoveHandle(handleMinY + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMinY) * handleSizeMult, snapLimit, Handles.DotCap);
			handleMinY = handleMinY - new Vector3(0.25f, 0, 0.25f);
		}
		handleMinY = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, RoundTo025And075(handleMinY.y), Surforge.surforgeSettings.limits.minZ + (Surforge.surforgeSettings.limits.maxZ - Surforge.surforgeSettings.limits.minZ) * 0.5f);
		if (handleMinY.y > Surforge.surforgeSettings.limits.maxY) handleMinY = new Vector3(handleMinY.x, Surforge.surforgeSettings.limits.maxY, handleMinY.z);
		Surforge.surforgeSettings.limits.minY = handleMinY.y;
		
		handleMaxZ = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.maxZ);
		if (sceneCameraDirection != 0) {
			if (hideVolumeBorders) {
				Vector3 handleMaxZFlat = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.maxZ);
				handleMaxZ = Handles.FreeMoveHandle(handleMaxZFlat + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxZ) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMaxZ = handleMaxZ - new Vector3(0.25f, 0, 0.25f);
			}
			else {
				handleMaxZ = Handles.FreeMoveHandle(handleMaxZ + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMaxZ) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMaxZ = handleMaxZ - new Vector3(0.25f, 0, 0.25f);
			}
		}
		handleMaxZ = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, RoundTo025And075(handleMaxZ.z));
		if (handleMaxZ.z < Surforge.surforgeSettings.limits.minZ) handleMaxZ = new Vector3(handleMaxZ.x, handleMaxZ.y, Surforge.surforgeSettings.limits.minZ);
		Surforge.surforgeSettings.limits.maxZ = handleMaxZ.z;
		
		handleMinZ = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, Surforge.surforgeSettings.limits.minZ);
		if (sceneCameraDirection != 0) { 
			if (hideVolumeBorders) {
				Vector3 handleMinZFlat = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY, Surforge.surforgeSettings.limits.minZ);
				handleMinZ = Handles.FreeMoveHandle(handleMinZFlat + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMinZ) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMinZ = handleMinZ - new Vector3(0.25f, 0, 0.25f);
			}
			else {
				handleMinZ = Handles.FreeMoveHandle(handleMinZ + new Vector3(0.25f, 0, 0.25f), Quaternion.identity, HandleUtility.GetHandleSize(handleMinZ) * handleSizeMult, snapLimit, Handles.DotCap);
				handleMinZ = handleMinZ - new Vector3(0.25f, 0, 0.25f);
			}
		}
		handleMinZ = new Vector3(Surforge.surforgeSettings.limits.minX + (Surforge.surforgeSettings.limits.maxX - Surforge.surforgeSettings.limits.minX) * 0.5f, Surforge.surforgeSettings.limits.minY + (Surforge.surforgeSettings.limits.maxY - Surforge.surforgeSettings.limits.minY) * 0.5f, RoundTo025And075(handleMinZ.z));
		if (handleMinZ.z > Surforge.surforgeSettings.limits.maxZ) handleMinZ = new Vector3(handleMinZ.x, handleMinZ.y, Surforge.surforgeSettings.limits.maxZ);
		Surforge.surforgeSettings.limits.minZ = handleMinZ.z;
		
		
		ShowLimitsLines(Surforge.surforgeSettings.limits, limitsPlane, limitsBorder, true);
		
		Surforge.CheckLimitsChanged();
	}
	
	
	static Vector3 textureBordersHandleMaxX;
	static Vector3 textureBordersHandleMinX;
	
	static Vector3 textureBordersHandleMaxZ;
	static Vector3 textureBordersHandleMinZ;
	
	static Color textureBordersBorder = new Color(1, 1, 1, 2); 
	
	static void ShowTextureBorders() {
		
		bool limitsActive = Surforge.surforgeSettings.isLimitsActive;
		
		Handles.color = textureBordersBorder;
		
		if (Surforge.surforgeSettings.showTextureEdges) {
			
			textureBordersHandleMaxX = new Vector3(Surforge.surforgeSettings.textureBorders.maxX, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f);
			if (!limitsActive) textureBordersHandleMaxX = Handles.FreeMoveHandle(textureBordersHandleMaxX, Quaternion.identity, HandleUtility.GetHandleSize(textureBordersHandleMaxX) * handleSizeMult, snapLimit, Handles.DotCap);
			textureBordersHandleMaxX = new Vector3(RoundTo025And075(textureBordersHandleMaxX.x), Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f);
			if (textureBordersHandleMaxX.x < Surforge.surforgeSettings.textureBorders.minX) textureBordersHandleMaxX = new Vector3(Surforge.surforgeSettings.textureBorders.minX, textureBordersHandleMaxX.y, textureBordersHandleMaxX.z);
			Surforge.surforgeSettings.textureBorders.maxX = textureBordersHandleMaxX.x;
			
			textureBordersHandleMinX = new Vector3(Surforge.surforgeSettings.textureBorders.minX, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f);
			if (!limitsActive) textureBordersHandleMinX = Handles.FreeMoveHandle(textureBordersHandleMinX, Quaternion.identity, HandleUtility.GetHandleSize(textureBordersHandleMinX) * handleSizeMult, snapLimit, Handles.DotCap);
			textureBordersHandleMinX = new Vector3(RoundTo025And075(textureBordersHandleMinX.x), Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f);
			if (textureBordersHandleMinX.x > Surforge.surforgeSettings.textureBorders.maxX) textureBordersHandleMinX = new Vector3(Surforge.surforgeSettings.textureBorders.maxX, textureBordersHandleMinX.y, textureBordersHandleMinX.z);
			Surforge.surforgeSettings.textureBorders.minX = textureBordersHandleMinX.x;
			
			
			textureBordersHandleMaxZ = new Vector3(Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.maxZ);
			if (!limitsActive) textureBordersHandleMaxZ = Handles.FreeMoveHandle(textureBordersHandleMaxZ, Quaternion.identity, HandleUtility.GetHandleSize(textureBordersHandleMaxZ) * handleSizeMult, snapLimit, Handles.DotCap);
			textureBordersHandleMaxZ = new Vector3(Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, RoundTo025And075(textureBordersHandleMaxZ.z));
			if (textureBordersHandleMaxZ.z < Surforge.surforgeSettings.textureBorders.minZ) textureBordersHandleMaxZ = new Vector3(textureBordersHandleMaxZ.x, textureBordersHandleMaxZ.y, Surforge.surforgeSettings.textureBorders.minZ);
			Surforge.surforgeSettings.textureBorders.maxZ = textureBordersHandleMaxZ.z;
			
			textureBordersHandleMinZ = new Vector3(Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, Surforge.surforgeSettings.textureBorders.minZ);
			if (!limitsActive) textureBordersHandleMinZ = Handles.FreeMoveHandle(textureBordersHandleMinZ, Quaternion.identity, HandleUtility.GetHandleSize(textureBordersHandleMinZ) * handleSizeMult, snapLimit, Handles.DotCap);
			textureBordersHandleMinZ = new Vector3(Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f, Surforge.surforgeSettings.textureBorders.minY + (Surforge.surforgeSettings.textureBorders.maxY - Surforge.surforgeSettings.textureBorders.minY) * 0.5f, RoundTo025And075(textureBordersHandleMinZ.z));
			if (textureBordersHandleMinZ.z > Surforge.surforgeSettings.textureBorders.maxZ) textureBordersHandleMinZ = new Vector3(textureBordersHandleMinZ.x, textureBordersHandleMinZ.y, Surforge.surforgeSettings.textureBorders.maxZ);
			Surforge.surforgeSettings.textureBorders.minZ = textureBordersHandleMinZ.z;
			
		}
		
		float limitsMinY = -0.5f;
		if (Surforge.surforgeSettings.limits != null) {
			limitsMinY = Surforge.surforgeSettings.limits.minY;
			Surforge.surforgeSettings.textureBorders.minY = limitsMinY;
			Surforge.surforgeSettings.textureBorders.maxY = limitsMinY;
		}
		
		if (Surforge.surforgeSettings.showTextureEdges) {
			Handles.Label (new Vector3(Surforge.surforgeSettings.textureBorders.minX + (Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX) * 0.5f, Surforge.surforgeSettings.textureBorders.minY, Surforge.surforgeSettings.textureBorders.maxZ + 5.0f), Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX).ToString()); 
			Handles.Label (new Vector3(Surforge.surforgeSettings.textureBorders.maxX + 5.0f, Surforge.surforgeSettings.textureBorders.minY, Surforge.surforgeSettings.textureBorders.minZ + (Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ) * 0.5f), Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ).ToString()); 
		}

		
		Surforge.UpdateTexturePreviewPosition();
	}
	
	
	
	static void ShowLimitsLines(SurforgeLimits limits, Color plane, Color border, bool isLimits) {
		if (isLimits && hideVolumeBorders) Handles.color = limitsThinBorder;
		
		Vector3 xyz = new Vector3(limits.maxX + 0.25f, limits.maxY, limits.maxZ + 0.25f);
		Vector3 mxyz = new Vector3(limits.minX + 0.25f, limits.maxY, limits.maxZ + 0.25f);
		Vector3 xmyz = new Vector3(limits.maxX + 0.25f, limits.minY, limits.maxZ + 0.25f);
		Vector3 xymz = new Vector3(limits.maxX + 0.25f, limits.maxY, limits.minZ + 0.25f);
		
		Vector3 mxmymz = new Vector3(limits.minX + 0.25f, limits.minY, limits.minZ + 0.25f);
		Vector3 xmymz = new Vector3(limits.maxX + 0.25f, limits.minY, limits.minZ + 0.25f);
		Vector3 mxymz = new Vector3(limits.minX + 0.25f, limits.maxY, limits.minZ + 0.25f);
		Vector3 mxmyz = new Vector3(limits.minX + 0.25f, limits.minY, limits.maxZ + 0.25f);
		
		Handles.DrawSolidRectangleWithOutline(new Vector3[4] {xyz, mxyz, mxymz, xymz }, plane, border);
		
		Handles.DrawSolidRectangleWithOutline(new Vector3[4] {xyz, xymz, xmymz, xmyz }, plane, border);
		Handles.DrawSolidRectangleWithOutline(new Vector3[4] {mxyz, mxymz, mxmymz, mxmyz }, plane, border);
		
		Handles.DrawSolidRectangleWithOutline(new Vector3[4] {xyz, mxyz, mxmyz, xmyz }, plane, border);
		Handles.DrawSolidRectangleWithOutline(new Vector3[4] {xymz, mxymz, mxmymz, xmymz }, plane, border);
		
		if (isLimits && hideVolumeBorders) {
			Handles.color = limitsBorder;
			Handles.DrawSolidRectangleWithOutline(new Vector3[4] {xmyz, mxmyz, mxmymz, xmymz }, plane, border);
		}
	}
	
	static bool hideVolumeBorders;
	
	static int GetSceneCameraDirection() {
		int direction = 0;
		
		if (Camera.current != null) {
			Vector3 editorCameraRotation = Camera.current.transform.eulerAngles;
			float x = Mathf.Round(editorCameraRotation.x / 90) * 90;
			float y = Mathf.Round(editorCameraRotation.y / 90) * 90;
			
			if ((x == 90.0f) || (x == 270.0f)) {
				direction = 1;
			}
			else {
				if ((y == 90.0f) || (y == 270.0f)) {
					direction = 2;
				}
				
			}
			
			if ( (editorCameraRotation.x > 80.0f) && (editorCameraRotation.x < 100.0f) ) {
				hideVolumeBorders = true;
			}
			else hideVolumeBorders = false;
		}
		
		return direction;
	}
	
	
	
	static void PaintWithBrush() {
		Tools.current = Tool.None;
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		RaycastModelsInScene();
		CheckMouseClickForBrush();
		//HandleUtility.Repaint();
	}
	
	
	static void CheckMouseClickForBrush() {
		Event e = Event.current;
		
		//Check the event type and make sure it's left click.
		if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown)  && e.button == 0) {
			
			if ( !(e.alt ) ) {
				if (brushSelectedNode != null) {
					Surforge.GrowWithBrush(brushSelectedNode);
				}
				//e.Use();  //Eat the event so it doesn't propagate through the editor.
			}
		}
	}
	
	
	static void RaycastModelsInScene() {  //selects only if have collider component
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1000.0f)) {
			if (hit.collider.gameObject != null) {
				//bool brush = Handles.Button(hit.point, Quaternion.identity, 2, 2, Handles.CubeCap);
				Vector3 pointRounded = new Vector3(Mathf.RoundToInt(hit.point.x),
				                                   Mathf.RoundToInt(hit.point.y),
				                                   Mathf.RoundToInt(hit.point.z));
				RaycastOctreeNode(pointRounded);
			}	
		}
	}
	
	
	static void RaycastOctreeNode(Vector3 point) {
		brushSelectedNode = Surforge.surforgeSettings.octree.GetNodeByPointFromRoot(point, 8);
		if (brushSelectedNode != null) {
			DrawNodeBounds(brushSelectedNode);
			
		}
		
		
	}
	
	
	
	public void OnDestroy() {
		Surforge.ToggleEditorGrid(true);
		SceneView.onSceneGUIDelegate -= SurforgeOnScene;
		
		if (materialEditor != null) {
			DestroyImmediate (materialEditor);
		}
		if (Surforge.surforgeSettings) {
			if (Surforge.surforgeSettings.placeToolPreview != null) {
				DestroyImmediate(Surforge.surforgeSettings.placeToolPreview.gameObject);
			}
		}

		isSurforgeDelegateRunning = false;
	}
	
	
	
	
	static void DrawNodeBounds(SurforgeOctreeNode node) {
		Vector3 point0 = new Vector3(node.center.x + node.halfSize*1.5f, node.center.y + node.halfSize*1.5f, node.center.z + node.halfSize*1.5f);
		Vector3 point1 = new Vector3(node.center.x - node.halfSize*1.5f, node.center.y - node.halfSize*1.5f, node.center.z - node.halfSize*1.5f);
		Vector3 point2 = new Vector3(node.center.x + node.halfSize*1.5f, node.center.y - node.halfSize*1.5f, node.center.z - node.halfSize*1.5f);
		Vector3 point3 = new Vector3(node.center.x - node.halfSize*1.5f, node.center.y + node.halfSize*1.5f, node.center.z - node.halfSize*1.5f);
		Vector3 point4 = new Vector3(node.center.x - node.halfSize*1.5f, node.center.y - node.halfSize*1.5f, node.center.z + node.halfSize*1.5f);
		Vector3 point5 = new Vector3(node.center.x + node.halfSize*1.5f, node.center.y - node.halfSize*1.5f, node.center.z + node.halfSize*1.5f);
		Vector3 point6 = new Vector3(node.center.x - node.halfSize*1.5f, node.center.y + node.halfSize*1.5f, node.center.z + node.halfSize*1.5f);
		Vector3 point7 = new Vector3(node.center.x + node.halfSize*1.5f, node.center.y + node.halfSize*1.5f, node.center.z - node.halfSize*1.5f);
		Handles.DrawLine(point1, point2);
		Handles.DrawLine(point1, point4);
		Handles.DrawLine(point1, point3);
		Handles.DrawLine(point4, point5);
		Handles.DrawLine(point2, point5);
		Handles.DrawLine(point5, point0);
		Handles.DrawLine(point4, point6);
		Handles.DrawLine(point2, point7);
		Handles.DrawLine(point3, point6);
		Handles.DrawLine(point0, point7);
		Handles.DrawLine(point6, point0);
		Handles.DrawLine(point7, point3);
	}
	


	// --- greebles tool ---
	static void GreeblesTool() {
		Event e = Event.current;

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		if (Surforge.surforgeSettings) {
			if ((!e.shift) && (!e.control) && (!e.alt)) { 
				if ((e.type == EventType.MouseDown)  && (e.button == 0) && (e.type != EventType.MouseDrag)) {
					Surforge.LogAction("Scatter greebles", "Left Click", "");
					Surforge.VoxelEngineScatter();
					e.Use();  
				}
				if ((e.type == EventType.MouseDown)  && (e.button == 1) && (e.type != EventType.MouseDrag)) {
					Surforge.LogAction("Reroll greebles", "Right Click", "");
					Surforge.Reroll();
					e.Use();  
				}
			}
			if ((e.shift) && (!e.control) && (!e.alt)) { 
				if ((e.type == EventType.MouseDown)  && (e.button == 0) && (e.type != EventType.MouseDrag)) {
					Surforge.LogAction("Grow greebles", "Shift + Left Click", "");
					Surforge.Grow();
				}
				if ((e.type == EventType.MouseDown)  && (e.button == 1) && (e.type != EventType.MouseDrag)) {
					Surforge.LogAction("Remove greebles in order", "Shift + Right Click", "");
					Surforge.SurforgeUndo();
					e.Use();  
				}
			}

		}

		if (Tools.current != Tool.None)  {
			Surforge.lastUsedTool = Tools.current;
			
			Surforge.ToggleLimitsToolActive();
			GetWindow<SurforgeInterface>().Repaint();
		}
	}
	

	// --- place tool ---
	static Vector3 placeToolSnappedPoint;
	static bool placeToolControlPositionFixed;
	static int placeToolState;
	static bool placeToolIsRotationStarted;

	static void PlaceTool() {
	
		Event e = Event.current;

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1000.0f)) {
			if (hit.collider.gameObject != null) {

				if (placeToolState == 0) { //move
				
					Vector3 newPlaceToolSnappedPoint = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z));

					if (e.control) { 
						Surforge.LogAction("Move constraint", "Hold Ctrl", "");

						if (!placeToolControlPositionFixed) {
							placeToolSnappedPoint = new Vector3(newPlaceToolSnappedPoint.x, newPlaceToolSnappedPoint.y, newPlaceToolSnappedPoint.z);
							placeToolControlPositionFixed = true;
						}

						if (Vector3.Distance(placeToolSnappedPoint, hit.point) > 0.55f) {
							placeToolSnappedPoint = new Vector3(newPlaceToolSnappedPoint.x, newPlaceToolSnappedPoint.y, newPlaceToolSnappedPoint.z);
						}
					}
					else { 
						placeToolControlPositionFixed = false;
						placeToolSnappedPoint = hit.point;
					}

				}

				if (placeToolState == 1) { 

					//rotate
					if (Surforge.surforgeSettings.placeToolPreview != null) {

						Vector3 rotationLookPoint = new Vector3(hit.point.x, placeToolSnappedPoint.y + Surforge.surforgeSettings.placeToolVerticalOffset, hit.point.z);

						float rotationLookPointDistance = Vector3.Distance(rotationLookPoint, new Vector3(placeToolSnappedPoint.x, 
						                                                                                  placeToolSnappedPoint.y + Surforge.surforgeSettings.placeToolVerticalOffset,
						                                                                                  placeToolSnappedPoint.z));
					
						//Debug.Log(rotationLookPoint); //multiple sneneview issue, different pos


						if ( rotationLookPointDistance > 2.0f) { 
							placeToolIsRotationStarted = true;
						}
						if (placeToolIsRotationStarted) {
							if (e.control) { 
								Surforge.LogAction("Rotate constraint", "Left Button + Ctrl + Drag", "");
							}
							else {
								Surforge.LogAction("Rotate", "Left Button + Drag", "");
							}

							Surforge.surforgeSettings.placeToolPreview.transform.LookAt(rotationLookPoint);
						
							if (e.control) { 
								Vector3 stepEulerAngles = Surforge.surforgeSettings.placeToolPreview.transform.eulerAngles;
								int rotationSteps = Mathf.RoundToInt(stepEulerAngles.y) / 15;
								stepEulerAngles = new Vector3(stepEulerAngles.x, (float)rotationSteps * 15.0f, stepEulerAngles.z);
								Surforge.surforgeSettings.placeToolPreview.transform.eulerAngles = stepEulerAngles;
							}
						}

					}


					if (e.shift) {  //scale
						if (Surforge.surforgeSettings.placeToolPreview != null) {
							if (placeToolIsRotationStarted) {
								if (e.control) { 
									Surforge.LogAction("Rotate and Scale constraint", "Left Button + Shift + Ctrl + Drag", "");
								}
								else {
									Surforge.LogAction("Rotate and Scale", "Left Button + Shift + Drag", "");
								}

								float scale = (hit.point - placeToolSnappedPoint).magnitude * 0.1f;

								if (e.control) { 
									scale = Mathf.Round(scale * 10) * 0.1f;
								}

								if (scale < 0.1f) scale = 0.1f;
								if (scale > 10) scale = 10.0f;

								bool wasXflipped = false;
								if (Surforge.surforgeSettings.placeToolPreview.transform.localScale.x < 0) wasXflipped = true;

								Surforge.surforgeSettings.placeToolPreview.transform.localScale = Vector3.one * scale;
								if (wasXflipped) {
									Surforge.surforgeSettings.placeToolPreview.transform.localScale = new Vector3( -Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
								                                                                          Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
								                                                                          Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
								}

							}
						}
					}

				}


				//move preview object
				bool isTextAsset = Surforge.surforgeSettings.placeMeshes.placeMeshes[Surforge.surforgeSettings.activePlaceMesh].isText;

				if (Surforge.surforgeSettings.placeToolPreview != null) {
					Surforge.surforgeSettings.placeToolPreview.transform.position = placeToolSnappedPoint + new Vector3(0, Surforge.surforgeSettings.placeToolVerticalOffset, 0);
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmX != null) {    
					Surforge.surforgeSettings.placeToolPreviewSymmX.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x * -1,
					                                                                             Surforge.surforgeSettings.placeToolPreview.transform.localPosition.y,
					                                                                             Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z);
					if (isTextAsset) {
						Surforge.surforgeSettings.placeToolPreviewSymmX.transform.localScale = new Vector3( Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					else {
						Surforge.surforgeSettings.placeToolPreviewSymmX.transform.localScale = new Vector3( -Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					Surforge.surforgeSettings.placeToolPreviewSymmX.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.x,
					                                                                                     -Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.y,
					                                                                                     Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.z);

				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmZ != null) {
					Surforge.surforgeSettings.placeToolPreviewSymmZ.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x,
					                                                                                  Surforge.surforgeSettings.placeToolPreview.transform.localPosition.y,
					                                                                                  Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z * -1);
					if (isTextAsset) {
						Surforge.surforgeSettings.placeToolPreviewSymmZ.transform.localScale = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					else {
						Surforge.surforgeSettings.placeToolPreviewSymmZ.transform.localScale = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                               -Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					Surforge.surforgeSettings.placeToolPreviewSymmZ.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.x,
					                                                                                     -Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.y,
					                                                                                     Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.z);
					
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmXZ != null) {
					Surforge.surforgeSettings.placeToolPreviewSymmXZ.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x * -1,
					                                                                                   Surforge.surforgeSettings.placeToolPreview.transform.localPosition.y,
					                                                                                   Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z * -1);
					if (isTextAsset) {
						Surforge.surforgeSettings.placeToolPreviewSymmXZ.transform.localScale = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                                Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                                Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					else {
						Surforge.surforgeSettings.placeToolPreviewSymmXZ.transform.localScale = new Vector3(-Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                               -Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					}
					Surforge.surforgeSettings.placeToolPreviewSymmXZ.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.x,
					                                                                                      Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.y,
					                                                                                      Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.z);
					
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonal != null) {
					Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(0, 0), new Vector2(-1.0f, 1.0f),  new Vector2(Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x, Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z) );
					Vector2 perp = new Vector2(Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x - perpPoint.x, Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z - perpPoint.y);
					perp = perp * -1.0f;
					Vector3 diagonalSymmPosition = new Vector3( Surforge.surforgeSettings.placeToolPreview.transform.localPosition.x + perp.x + perp.x, Surforge.surforgeSettings.placeToolPreview.transform.localPosition.y, Surforge.surforgeSettings.placeToolPreview.transform.localPosition.z + perp.y + perp.y);

					Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition = diagonalSymmPosition;
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale = new Vector3( -Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
					                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
					                                                                               Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.x,
					                                                                                            -(Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.y + 90.0f),
					                                                                                            Surforge.surforgeSettings.placeToolPreview.transform.localEulerAngles.z);
					
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX != null) {
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.x * -1,
					                                                                                          Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.y,
					                                                                                          Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.z);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX.transform.localScale = new Vector3( Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.x,
					                                                                                       Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.y,
					                                                                                       -Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.z);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.x,
					                                                                                             -(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.y + 180.0f),
					                                                                                             Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.z);
					
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ != null) {
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.x,
					                                                                                          Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.y,
					                                                                                          Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.z * -1);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ.transform.localScale = new Vector3( -Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.x,
					                                                                                       Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.y,
					                                                                                       Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.z);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.x,
					                                                                                             -(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.y + 180.0f),
					                                                                                             Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.z);
					
				}
				if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) {
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ.transform.localPosition = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.x * -1,
					                                                                                           Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.y,
					                                                                                           Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localPosition.z * -1);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ.transform.localScale = new Vector3( Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.x,
					                                                                                       Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.y,
					                                                                                       Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localScale.z);
					Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ.transform.localEulerAngles = new Vector3(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.x,
					                                                                                              (Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.y + 180.0f),
					                                                                                              Surforge.surforgeSettings.placeToolPreviewSymmDiagonal.transform.localEulerAngles.z);
					
				}


			}	
		}

		if ((e.type == EventType.MouseUp)  && (e.button == 1) && (!e.alt) && (!e.shift)) {
			if (Surforge.surforgeSettings.placeMeshes.placeMeshes[Surforge.surforgeSettings.activePlaceMesh].isText) {
				Surforge.LogAction("Randomize text", "Right Click", "");
				Surforge.PlaceToolGenerateTexts();
				Surforge.CreatePlaceToolSymmetryPreviews(); 
			}
			else {
				bool rightButtonFlip = true;
				if (Surforge.surforgeSettings.placeMeshes.placeMeshes[Surforge.surforgeSettings.activePlaceMesh].shuffleArray != null) {
					if (Surforge.surforgeSettings.placeMeshes.placeMeshes[Surforge.surforgeSettings.activePlaceMesh].shuffleArray.Length > 0) {
						Surforge.LogAction("Shuffle variants", "Right Click", "");
						rightButtonFlip = false;
						Surforge.PlaceToolShuffle();
						Surforge.CreatePlaceToolSymmetryPreviews(); 
					}
				}

				if (rightButtonFlip) {
					Surforge.LogAction("Flip object", "Right Click", "");
					Surforge.surforgeSettings.placeToolPreview.transform.localScale = new Vector3( -Surforge.surforgeSettings.placeToolPreview.transform.localScale.x,
						                                                                        Surforge.surforgeSettings.placeToolPreview.transform.localScale.y,
						                                                                        Surforge.surforgeSettings.placeToolPreview.transform.localScale.z);
				}
			}
		}


		if ((e.type == EventType.MouseDown)  && (e.button == 0) && (!e.alt) ) {
			if (placeToolState == 0) {
				placeToolState = 1;
			}
		}

		if ((e.type == EventType.MouseUp)  && e.button == 0) {
			
			if ( placeToolState == 1 ) {

				if (placeToolIsRotationStarted) {
					placeToolState = 0;
					placeToolIsRotationStarted = false;

					e.Use();
				}
				else {
					//placeObject
					Surforge.LogAction("Set object", "Left Click", "");

					Selection.activeGameObject = null;

					List<GameObject> newPlaceToolSelection = new List<GameObject>();

					newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreview));

					if (Surforge.surforgeSettings.placeToolPreviewSymmX) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmX));
					if (Surforge.surforgeSettings.placeToolPreviewSymmZ) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmZ));
					if (Surforge.surforgeSettings.placeToolPreviewSymmXZ) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmXZ));

					if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonal) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmDiagonal));
					if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmDiagonalX));
					if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmDiagonalZ));
					if (Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ) newPlaceToolSelection.Add(PlaceToolPlaceObject(Surforge.surforgeSettings.placeToolPreviewSymmDiagonalXZ));

					Object[] newSelection = new Object[newPlaceToolSelection.Count];
					for (int i=0; i< newSelection.Length; i++) {
						newSelection[i] = newPlaceToolSelection[i];
					}
					Selection.objects = newSelection;

					e.Use(); 
				}
			}
		}

		if (Tools.current != Tool.None)  {
			Surforge.lastUsedTool = Tools.current;

			Surforge.TogglePlaceToolActive(); 
			GetWindow<SurforgeInterface>().Repaint();
		}
	}
	
	static GameObject PlaceToolPlaceObject(GameObject placeToolPreview) {
		GameObject obj = (GameObject)Instantiate (placeToolPreview);
		obj.transform.position = placeToolPreview.transform.position;
		obj.transform.rotation = placeToolPreview.transform.rotation;
		obj.transform.localScale = placeToolPreview.transform.localScale;
		obj.transform.parent = Surforge.surforgeSettings.root.transform;

		AddMeshCollider(obj);

		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			renderer.sharedMaterial = Material.Instantiate(renderer.sharedMaterial); 
			SetPlaceObjectTextures(renderer.sharedMaterial);
		}
		foreach (Transform child in obj.transform) {
			AddMeshCollider(child.gameObject);

			Renderer childRenderer = (Renderer)child.gameObject.GetComponent<Renderer>();
			if (childRenderer) {
				childRenderer.sharedMaterial = Material.Instantiate(childRenderer.sharedMaterial); 
			}
		}

		placeToolState = 0;
		placeToolIsRotationStarted = false;
		Surforge.RegisterUndoPlaceMeshObject(obj);

		Surforge.CreateSeamlessInstancesPlaceMeshForPlaceTool(obj);

		return obj;
	}

	static void SetPlaceObjectTextures(Material mat) {
		if (Surforge.surforgeSettings) {
			PlaceMesh placeMesh = Surforge.surforgeSettings.placeMeshes.placeMeshes[Surforge.surforgeSettings.activePlaceMesh];
			if (placeMesh) {
				float offsetX = 0;
				float offsetZ = 0;
				if (placeMesh.randomUvOffset) {
					offsetX	= Random.Range(-1.0f, 1.0f);
					offsetZ	= Random.Range(-1.0f, 1.0f);
				}
				if (placeMesh.bumpMap) {
					mat.SetTexture("_BumpMap", placeMesh.bumpMap);
					mat.SetTextureScale("_BumpMap", placeMesh.bumpMapTiling);
					mat.SetFloat("_BumpIntensity", placeMesh.bumpMapIntensity);
					mat.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetZ));
				}
				if (placeMesh.aoMap) {
					mat.SetTexture("_AO", placeMesh.aoMap);
					mat.SetTextureScale("_AO", placeMesh.bumpMapTiling);
					mat.SetTextureOffset("_AO", new Vector2(offsetX, offsetZ));
					mat.SetFloat("_AOIntensity", placeMesh.aoMapIntensity);
				}
			}
		}
	}


	static void AddMeshCollider(GameObject obj) {
		MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
		if (meshFilter) {
			MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = meshFilter.sharedMesh;
		}
	}


	
	// --- polygonal lasso ---
	static List<Vector3> polygonLassoPoints;
	static Vector3 snappedPoint;
	
	static Color polygonLassoPointColor = new Color(2, 0.6f, 0, 1);  //orange
	static Color polygonLassoCurrentPointColor = new Color(2, 0.8f, 0, 1);  //orange
	static Color polygonLassoLineColor = new Color(2, 0.5f, 0, 4);

	static Color polygonLassoProfileSizeColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
	
	static void PolygonLasso() {
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		
		if (polygonLassoPoints != null) {
			if (polygonLassoPoints.Count > 0) {
				if (Surforge.surforgeSettings.seamless) {
					DrawPolygonLassoSeamlessPoints();
				}
			}
		}

		if (Surforge.surforgeSettings.symmetry) DrawPolygonLassoMirrorPoints();
		
		DrawPolygonLasso();
		DrawPolygonLassoChildPoints();
		
		RaycastPolygonLasso();
		SetPolyLassoChildPoints();

		CheckMouseClickPolygonLasso();
		
		
		if (Tools.current != Tool.None)  {
			Surforge.lastUsedTool = Tools.current;

			if (polygonLassoPoints != null) {
				polygonLassoPoints.Clear();
				Surforge.TogglePolygonLassoActive();
				GetWindow<SurforgeInterface>().Repaint();
			}
		}
	}
	
	//poly lasso child points (arc)
	static List<Vector3> polygonLassoChildPoints;
	static float polyLassoArcCurvature = 0;
	static float lastPolyLassoArcCurvature = 0.20711f;
	static int polyLassoArcDensity = 3; 

	static Vector2 polyLassoArcCircleCenter = Vector2.zero;
	static float polyLassoArcCircleRadius;

	static void SetPolyLassoChildPoints() {
		polygonLassoChildPoints = new List<Vector3>();

		//snappedPoint
		if (polyLassoArcCurvature != 0) {
			if ((polygonLassoPoints.Count > 0) && (Mathf.Abs(polyLassoArcCurvature) > 0.01f )) {
				Vector2 pointA = new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z);
				Vector2 pointC = new Vector2(snappedPoint.x, snappedPoint.z);

				Vector2 segmentMiddle = new Vector2((pointA.x + pointC.x) * 0.5f,
				                                    (pointA.y + pointC.y) * 0.5f);

				Vector2 arcOffsetVector = pointC - segmentMiddle;
				arcOffsetVector = new Vector2(-arcOffsetVector.y, arcOffsetVector.x);
				if (polyLassoArcCurvature < 0) {
					arcOffsetVector = arcOffsetVector * -1;
				}

				Vector2 arcOffsetPoint = segmentMiddle + arcOffsetVector.normalized * Mathf.Abs(polyLassoArcCurvature) * Vector2.Distance(pointA, pointC);

				polygonLassoChildPoints.Add(new Vector3(pointA.x, polygonLassoPoints[0].y, pointA.y));
				polygonLassoChildPoints.Add(new Vector3(arcOffsetPoint.x, polygonLassoPoints[0].y, arcOffsetPoint.y));
				polygonLassoChildPoints.Add(new Vector3(pointC.x, polygonLassoPoints[0].y, pointC.y));

				Vector2 triangleABMiddle = new Vector2((pointA.x + arcOffsetPoint.x) * 0.5f,
				                                       (pointA.y + arcOffsetPoint.y) * 0.5f);

				Vector2 triangleCBMiddle = new Vector2((pointC.x + arcOffsetPoint.x) * 0.5f,
				                                       (pointC.y + arcOffsetPoint.y) * 0.5f);
				Vector2 perpVectorAB = pointA - triangleABMiddle;
				perpVectorAB = new Vector2(-perpVectorAB.y, perpVectorAB.x);
				Vector2 perPointAB = triangleABMiddle + perpVectorAB;

				Vector2 perpVectorCB = pointC - triangleCBMiddle;
				perpVectorCB = new Vector2(-perpVectorCB.y, perpVectorCB.x);
				Vector2 perpPointCB = triangleCBMiddle + perpVectorCB;

				if (Surforge.TestLinesIntersection(triangleABMiddle, perPointAB, triangleCBMiddle, perpPointCB)) {
					polyLassoArcCircleCenter = Surforge.LineIntersectionPoint(triangleABMiddle, perPointAB, triangleCBMiddle, perpPointCB);
				}
				polyLassoArcCircleRadius = Vector2.Distance(arcOffsetPoint, polyLassoArcCircleCenter);

				for (int i=0; i< polyLassoArcDensity; i++) {
					polygonLassoChildPoints = DividePolyLassoChildArc(polygonLassoChildPoints, polyLassoArcCircleCenter, polyLassoArcCircleRadius);
				}


			}
		}
	}

	static List<Vector3> DividePolyLassoChildArc(List<Vector3> points, Vector2 polyLassoArcCircleCenter, float polyLassoArcCircleRadius) {
		List<Vector3> result = new List<Vector3>();

		for (int i=0; i < points.Count-1; i++) {
			Vector2 pointA = new Vector2(points[i].x, points[i].z);
			Vector2 pointB = new Vector2(points[i+1].x, points[i+1].z);

			Vector2 centerVector = new Vector2( (pointA.x + pointB.x) *0.5f, (pointA.y + pointB.y) * 0.5f) - polyLassoArcCircleCenter;
			centerVector = centerVector.normalized * polyLassoArcCircleRadius;
			Vector2 middlePoint = polyLassoArcCircleCenter + centerVector;

			result.Add(points[i]);
			result.Add(new Vector3(middlePoint.x, points[0].y, middlePoint.y));
		}
		result.Add(points[points.Count-1]);

		return result;
	}
	



	//grids and snaping interface
	static bool showOctreeGrid;
	static bool showUVs;
	static bool showUVsHelperGrid;
	
	static Vector3[] snapGuideToDrawSelfLinesX = new Vector3[0];
	static Vector3[] snapGuideToDrawSelfLinesZ = new Vector3[0];
	static Vector3[] snapGuideToDrawSelfLinesCrossX = new Vector3[0];
	static Vector3[] snapGuideToDrawSelfLinesCrossZ = new Vector3[0];
	static Vector3[] snapGuideToDrawSelfLinesCrossHelpers = new Vector3[0];

	static bool isPolyLassoObjectPointSnapping;


	static void MarkUvVectorPairsSnapRange(Vector3 raycastedHitPoint) {

		if (uvVectorPairs != null) {
			for (int i=0; i < uvVectorPairs.Length; i++) {
				uvVectorPairs[i].inSnapRange = false;

				float maxX = uvVectorPairs[i].a.x * rootScaleX;
				float minX = uvVectorPairs[i].b.x * rootScaleX;
				if (minX > maxX) {
					maxX = uvVectorPairs[i].b.x * rootScaleX;
					minX = uvVectorPairs[i].a.x * rootScaleX;
				}
				float maxZ = uvVectorPairs[i].a.z * rootScaleZ;
				float minZ = uvVectorPairs[i].b.z * rootScaleZ;
				if (minZ > maxZ) {
					maxZ = uvVectorPairs[i].b.z * rootScaleZ;
					minZ = uvVectorPairs[i].a.z * rootScaleZ;
				}

				if ( (raycastedHitPoint.x > (minX - snapRangeOffset)) && 
				     (raycastedHitPoint.x < (maxX + snapRangeOffset)) &&
				     (raycastedHitPoint.z > (minZ - snapRangeOffset)) && 
				     (raycastedHitPoint.z < (maxZ + snapRangeOffset)) ) {

					uvVectorPairs[i].inSnapRange = true;
				}
				    

			}
		}
	}


	static void RaycastPolygonLasso() {  //selects only if have collider component

		float selfSnapTreshold = 0.25f;
		float snapTreshold = 0.55f;
		float polyLassoObjectSnapTreshold = 0.55f;

		if (isPolyLassoObjectPointSnapping) {
			polyLassoObjectSnapTreshold = 0.55f * 100;
		}

		snapState = 0;

		ClearCrossHelperLines();
		
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Surforge.surforgeSettings.polygonLassoPlane != null) {
			MeshCollider collider = (MeshCollider)Surforge.surforgeSettings.polygonLassoPlane.GetComponent<MeshCollider>();
			
			if (collider.Raycast(ray, out hit, 1000.0f)) {


				MarkUvVectorPairsSnapRange(hit.point);
				
				
				Event e = Event.current;

				if (!Surforge.surforgeSettings.gloabalSnapActive) {
					snappedPoint = hit.point;
				}
				else {
					bool shiftMode = false;
					if (!e.alt) {
						if ((e.shift) && (polygonLassoPoints.Count > 0)) {
							shiftMode = true;
						}
					}
				
					if (shiftMode) {
						Surforge.LogAction("Snap to 45 degrees", "Hold Shift", "");
					
						Vector3 shiftAlignedPoint = new Vector3();
						Vector3 lastPoint = polygonLassoPoints[polygonLassoPoints.Count-1];
					
						Vector2 ps1 = new Vector2();
						Vector2 pe1 = new Vector2();
						Vector2 ps2 = new Vector2();
						Vector2 pe2 = new Vector2();
					
						int alignDir = 0;
					
						float cos = Surforge.GetCosOfAngleBetweenVectors(new Vector2(0, -1), new Vector3(lastPoint.x - hit.point.x, lastPoint.z - hit.point.z));
						float angle = Mathf.Acos(cos) * (180.0f / Mathf.PI);
						if (lastPoint.x > hit.point.x) angle = 360.0f - angle; 
					
						if ( (angle > 337.5f) || (angle <= 22.5f) ) { //0
							shiftAlignedPoint = new Vector3(lastPoint.x, hit.point.y, hit.point.z);
						
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x, lastPoint.z+1);
						
							alignDir = 0;
						}
					
						if ( (angle > 22.5f) && (angle <= 67.5f) ) { //45
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x - 1, lastPoint.z-1);
						
							ps2 = new Vector2(hit.point.x, hit.point.z);
							pe2 = new Vector2();
						
							if (angle < 45.0f) pe2 = new Vector2(ps2.x, ps2.y - 1.0f);
							else pe2 = new Vector2(ps2.x - 1.0f, ps2.y);
						
							Vector2 intersectionPoint = new Vector2();
							if (Surforge.TestLinesIntersection(ps1, pe1, ps2, pe2)) {
								intersectionPoint = Surforge.LineIntersectionPoint(ps1, pe1, ps2, pe2);
							}
							shiftAlignedPoint = new Vector3(intersectionPoint.x, lastPoint.y, intersectionPoint.y);
						
							alignDir = 45;
						}
					
						if ( (angle > 67.5f) && (angle <= 112.5f) ) { //90
							shiftAlignedPoint = new Vector3(hit.point.x, hit.point.y, lastPoint.z);
						
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x + 1, lastPoint.z);
						
							alignDir = 90;
						}
					
						if ( (angle > 112.5f) && (angle <= 157.5f) ) { //135
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x + 1, lastPoint.z-1);
						
							ps2 = new Vector2(hit.point.x, hit.point.z);
							pe2 = new Vector2();
						
							if (angle < 135.0f) pe2 = new Vector2(ps2.x - 1.0f, ps2.y);
							else pe2 = new Vector2(ps2.x, ps2.y + 1.0f);
						
							Vector2 intersectionPoint = new Vector2();
							if (Surforge.TestLinesIntersection(ps1, pe1, ps2, pe2)) {
								intersectionPoint = Surforge.LineIntersectionPoint(ps1, pe1, ps2, pe2);
							}
							shiftAlignedPoint = new Vector3(intersectionPoint.x, lastPoint.y, intersectionPoint.y);
						
							alignDir = 135;
						}
					
						if ( (angle > 157.5f) && (angle <= 202.5f) ) { //180
							shiftAlignedPoint = new Vector3(lastPoint.x, hit.point.y, hit.point.z);
							
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x, lastPoint.z+1);
						
							alignDir = 180;
						}
					
						if ( (angle > 202.5f) && (angle <= 247.5f) ) { //225
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x - 1, lastPoint.z-1);
						
							ps2 = new Vector2(hit.point.x, hit.point.z);
							pe2 = new Vector2();
						
							if (angle < 225.0f) pe2 = new Vector2(ps2.x, ps2.y + 1.0f);
							else pe2 = new Vector2(ps2.x + 1.0f, ps2.y);
						
							Vector2 intersectionPoint = new Vector2();
							if (Surforge.TestLinesIntersection(ps1, pe1, ps2, pe2)) {
								intersectionPoint = Surforge.LineIntersectionPoint(ps1, pe1, ps2, pe2);
							}
							shiftAlignedPoint = new Vector3(intersectionPoint.x, lastPoint.y, intersectionPoint.y);
						
							alignDir = 225;
						}
					
						if ( (angle > 247.5f) && (angle <= 292.5f) ) { //270
							shiftAlignedPoint = new Vector3(hit.point.x, hit.point.y, lastPoint.z);
						
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x + 1, lastPoint.z);
						
							alignDir = 270;
						}
					
						if ( (angle > 292.5f) && (angle <= 337.5f) ) { //315
							ps1 = new Vector2(lastPoint.x, lastPoint.z);
							pe1 = new Vector2(lastPoint.x + 1, lastPoint.z-1);
						
							ps2 = new Vector2(hit.point.x, hit.point.z);
							pe2 = new Vector2();
						
							if (angle < 315.0f) pe2 = new Vector2(ps2.x + 1.0f, ps2.y);
							else pe2 = new Vector2(ps2.x, ps2.y - 1.0f);
						
							Vector2 intersectionPoint = new Vector2();
							if (Surforge.TestLinesIntersection(ps1, pe1, ps2, pe2)) {
								intersectionPoint = Surforge.LineIntersectionPoint(ps1, pe1, ps2, pe2);
							}
							shiftAlignedPoint = new Vector3(intersectionPoint.x, lastPoint.y, intersectionPoint.y);
						
							alignDir = 315;
						}
					
					
						snappedPoint = shiftAlignedPoint;
						
						if ( ((Surforge.surforgeSettings.gridSnapActive == false) || (showOctreeGrid == false)) && ((Surforge.surforgeSettings.uvSnapActive == false) || (uvsFound == false)) ) {
							snappedPoint = shiftAlignedPoint;
						}
					
					
					
						//snap to lines
						if ((Surforge.surforgeSettings.uvSnapActive == true) && (uvsFound == true) && ( (showUVs) || (showUVsHelperGrid) ) ) {
						
							//snap to UV lines 
							if (showUVsHelperGrid == false) { 
							
								if (uvPoints != null) {
									if (uvPoints.Length > 0) {
									
										
										if ( (alignDir == 0) || (alignDir == 180) ) {
										
											Vector3 snappedPointZ = GetSnappedLineZuvShift(hit.point);
											float distZ = Vector3.Distance(snappedPointZ, hit.point);
										
											if (distZ < snapTreshold) {
												snappedPoint = snappedPointZ;
											}
										}
										if ( (alignDir == 90) || (alignDir == 270) ) {
										
											Vector3 snappedPointX = GetSnappedLineXuvShift(hit.point);
											float distX = Vector3.Distance(snappedPointX, hit.point);
										
											if (distX < snapTreshold) {
												snappedPoint = snappedPointX;
											}
										}
									
										if ( (alignDir == 45) || (alignDir == 225) ) {
											Vector3 snappedPointRightUp = GetSnappedLineDiagonalUvShift(hit.point, true);
											float distRightUp = Vector3.Distance(snappedPointRightUp, hit.point);
										
											if (distRightUp < snapTreshold) {
												snappedPoint = snappedPointRightUp;
											}
										}
									
										if ( (alignDir == 135) || (alignDir == 315) ) {
											Vector3 snappedPointLeftUp = GetSnappedLineDiagonalUvShift(hit.point, false);
											float distLeftUp = Vector3.Distance(snappedPointLeftUp, hit.point);
										
											if (distLeftUp < snapTreshold) {
												snappedPoint = snappedPointLeftUp;
											}
										}
									
									
									
									
									
									
									}
								}
							}
						
							else { //snap to helper lines
								if (uvHelperSnapPoints != null) {
									if (uvHelperSnapPoints.Count > 0) {
									
										Vector3 newSnappedLineX = GetSnappedLineX(hit.point);
										Vector3 newSnappedLineZ = GetSnappedLineZ(hit.point);
									
									
										//snap to x line
										Vector2 intersectionPointX = new Vector2();
										if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSnappedLineX.x, newSnappedLineX.z), new Vector2(newSnappedLineX.x, newSnappedLineX.z + 1)) ) {
											intersectionPointX = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSnappedLineX.x, newSnappedLineX.z), new Vector2(newSnappedLineX.x, newSnappedLineX.z + 1));
										}
									
										Vector3 snappedPointX = new Vector3(intersectionPointX.x, lastPoint.y, intersectionPointX.y);
									
										//snap to z line
										Vector2 intersectionPointZ = new Vector2();
										if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSnappedLineZ.x, newSnappedLineZ.z), new Vector2(newSnappedLineZ.x + 1, newSnappedLineZ.z)) ) {
											intersectionPointZ = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSnappedLineZ.x, newSnappedLineZ.z), new Vector2(newSnappedLineZ.x + 1, newSnappedLineZ.z));
										}
									
										Vector3 snappedPointZ = new Vector3(intersectionPointZ.x, lastPoint.y, intersectionPointZ.y);
									
										float distX = Vector3.Distance(new Vector3 (newSnappedLineX.x, hit.point.y, hit.point.z), hit.point);
										float distZ = Vector3.Distance(new Vector3 (hit.point.x, hit.point.y, newSnappedLineZ.z), hit.point);
										
										if ( (alignDir == 0) || (alignDir == 180) || (alignDir == 90) || (alignDir == 270) ) {
											if ( (alignDir == 0) || (alignDir == 180) ) {
												if (distZ < snapTreshold) {
													snappedPoint = snappedPointZ;

													snapState = 6;
												}
											}
											if ( (alignDir == 90) || (alignDir == 270) ) {
												if (distX < snapTreshold) {
													snappedPoint = snappedPointX;

													snapState = 6;
												}
											}
										}
										else {
											if (distX < distZ) {
												if (distX < snapTreshold) {
													snappedPoint = snappedPointX;

													snapState = 6;
												}
											}
											else {
												if (distZ < snapTreshold) {
													snappedPoint = snappedPointZ;

													snapState = 6;
												}
											}
										}
									
									
									
									}
								}
							}
						}
					
					
					
						// snap to grid lines 
						if ((Surforge.surforgeSettings.gridSnapActive == true) &&  (showOctreeGrid == true) ) {
						
							Vector3 newSelfSnappedLineX = GetGridSnappedLineX(hit.point);
							Vector3 newSelfSnappedLineZ = GetGridSnappedLineZ(hit.point);
						
						
							//snap to x line
							Vector2 intersectionPointX = new Vector2();
							if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z), new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z + 1)) ) {
								intersectionPointX = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z), new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z + 1));
							}
						
							Vector3 snappedPointX = new Vector3(intersectionPointX.x, lastPoint.y, intersectionPointX.y);
						
							//snap to z line
							Vector2 intersectionPointZ = new Vector2();
							if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSelfSnappedLineZ.x, newSelfSnappedLineZ.z), new Vector2(newSelfSnappedLineZ.x + 1, newSelfSnappedLineZ.z)) ) {
								intersectionPointZ = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSelfSnappedLineZ.x, newSelfSnappedLineZ.z), new Vector2(newSelfSnappedLineZ.x + 1, newSelfSnappedLineZ.z));
							}
						
							Vector3 snappedPointZ = new Vector3(intersectionPointZ.x, lastPoint.y, intersectionPointZ.y);
						
							float distX = Vector3.Distance(new Vector3 (newSelfSnappedLineX.x, hit.point.y, hit.point.z), hit.point);
							float distZ = Vector3.Distance(new Vector3 (hit.point.x, hit.point.y, newSelfSnappedLineZ.z), hit.point);
						
							if ( (alignDir == 0) || (alignDir == 180) || (alignDir == 90) || (alignDir == 270) ) {
								if ( (alignDir == 0) || (alignDir == 180) ) {
									if (distZ < snapTreshold) {
										snappedPoint = snappedPointZ;
									}
								}
								if ( (alignDir == 90) || (alignDir == 270) ) {
									if (distX < snapTreshold) {
										snappedPoint = snappedPointX;
									}
								}
							}
							else {
								if (distX < distZ) {
									if (distX < snapTreshold) {
										snappedPoint = snappedPointX;
									}
								}
								else {
									if (distZ < snapTreshold) {
										snappedPoint = snappedPointZ;
									}
								}
							}
							
						}
					
					
					
						//snap to self lines
						if (Surforge.surforgeSettings.selfSnapActive) {
						
							Vector3 newSelfSnappedLineX = GetSelfSnappedLineX(hit.point);
							Vector3 newSelfSnappedLineZ = GetSelfSnappedLineZ(hit.point);
						
						
							//snap to x line
							Vector2 intersectionPointX = new Vector2();
							if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z), new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z + 1)) ) {
								intersectionPointX = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z), new Vector2(newSelfSnappedLineX.x, newSelfSnappedLineX.z + 1));
							}
						
							Vector3 snappedPointX = new Vector3(intersectionPointX.x, lastPoint.y, intersectionPointX.y);
						
							//snap to z line
							Vector2 intersectionPointZ = new Vector2();
							if (Surforge.TestLinesIntersection                      (ps1, pe1, new Vector2(newSelfSnappedLineZ.x, newSelfSnappedLineZ.z), new Vector2(newSelfSnappedLineZ.x + 1, newSelfSnappedLineZ.z)) ) {
								intersectionPointZ = Surforge.LineIntersectionPoint (ps1, pe1, new Vector2(newSelfSnappedLineZ.x, newSelfSnappedLineZ.z), new Vector2(newSelfSnappedLineZ.x + 1, newSelfSnappedLineZ.z));
							}
						
							Vector3 snappedPointZ = new Vector3(intersectionPointZ.x, lastPoint.y, intersectionPointZ.y);
						
							float distX = Vector3.Distance(new Vector3 (newSelfSnappedLineX.x, hit.point.y, hit.point.z), hit.point);
							float distZ = Vector3.Distance(new Vector3 (hit.point.x, hit.point.y, newSelfSnappedLineZ.z), hit.point);
						
							if ( (alignDir == 0) || (alignDir == 180) || (alignDir == 90) || (alignDir == 270) ) {
								if ( (alignDir == 0) || (alignDir == 180) ) {
									if (distZ < snapTreshold) {
										snappedPoint = snappedPointZ;
										if (polygonLassoPoints.Count > selfSnappedLineIndexZ) snapGuideToDrawSelfLinesZ = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexZ]};

										snapState = 4;
									}
								}
								if ( (alignDir == 90) || (alignDir == 270) ) {
									if (distX < snapTreshold) {
										snappedPoint = snappedPointX;
										if (polygonLassoPoints.Count > selfSnappedLineIndexX) snapGuideToDrawSelfLinesX = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexX]};

										snapState = 4;
									}
								}
							}
							else {
								if (distX < distZ) {
									if (distX < snapTreshold) {
										snappedPoint = snappedPointX;
										if (polygonLassoPoints.Count > selfSnappedLineIndexX) snapGuideToDrawSelfLinesX = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexX]};

										snapState = 4;
									}
								}
								else {
									if (distZ < snapTreshold) {
										snappedPoint = snappedPointZ;
										if (polygonLassoPoints.Count > selfSnappedLineIndexZ) snapGuideToDrawSelfLinesZ = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexZ]};

										snapState = 4;
									}
								}
							}
						
						}



						//snap to Poly Lasso Objects Lines 
						if ((Surforge.surforgeSettings.polyLassoObjects != null) && (Surforge.surforgeSettings.objectSnapActive)) {
							if (Surforge.surforgeSettings.polyLassoObjects.Count > 0) {
	

								if ( (alignDir == 0) || (alignDir == 180) ) {
								
									Vector3 snappedPointZ = GetSnappedLineZobjectLineShift(hit.point);
									float distZ = Vector3.Distance(snappedPointZ, hit.point);
								
									if (distZ < polyLassoObjectSnapTreshold) {
										snappedPoint = snappedPointZ;
										ClearCrossHelperLines();

										snapState = 2;
									}
								}
								if ( (alignDir == 90) || (alignDir == 270) ) {
								
									Vector3 snappedPointX = GetSnappedLineXobjectLineShift(hit.point);
									float distX = Vector3.Distance(snappedPointX, hit.point);
								
									if (distX < polyLassoObjectSnapTreshold) {
										snappedPoint = snappedPointX;
										ClearCrossHelperLines();

										snapState = 2;
									}
								}
							
								if ( (alignDir == 45) || (alignDir == 225) ) {
									Vector3 snappedPointRightUp = GetSnappedLineDiagonalObjectLineShift(hit.point, true);
									float distRightUp = Vector3.Distance(snappedPointRightUp, hit.point);
								
									if (distRightUp < polyLassoObjectSnapTreshold) {
										snappedPoint = snappedPointRightUp;
										ClearCrossHelperLines();

										snapState = 2;
									}
								}
							
								if ( (alignDir == 135) || (alignDir == 315) ) {
									Vector3 snappedPointLeftUp = GetSnappedLineDiagonalObjectLineShift(hit.point, false);
									float distLeftUp = Vector3.Distance(snappedPointLeftUp, hit.point);
								
									if (distLeftUp < polyLassoObjectSnapTreshold) {
										snappedPoint = snappedPointLeftUp;
										ClearCrossHelperLines();

										snapState = 2;
									}
								}
							
							
							}
						}



					
					
					}
				
				
					//--------------------------------------------------- without shift pressed
					else {  

						if ( (Surforge.surforgeSettings.gridSnapActive == false) ||  (showOctreeGrid == false) ) {
							snappedPoint = hit.point;
						}

					
						if ((Surforge.surforgeSettings.gridSnapActive == true) && (showOctreeGrid == true) && ( ((showUVs == false) && (showUVsHelperGrid == false)) || (Surforge.surforgeSettings.uvSnapActive == false)) ) {

							Vector3 newSnappedPoint = new Vector3(Mathf.Round(hit.point.x),hit.point.y, Mathf.Round(hit.point.z));
						
							if (Vector3.Distance(snappedPoint, hit.point) > snapTreshold) {
								snappedPoint = new Vector3(newSnappedPoint.x, newSnappedPoint.y, newSnappedPoint.z);
							}

						}
					
						bool snappedToUvPoint = false;
						bool snappedToHelpersPoint = false;

						if ( (Surforge.surforgeSettings.uvSnapActive == true) && ( (showUVs) || (showUVsHelperGrid) )  && (uvsFound == true) ) {
						
							// UV snap (partly optimized) 
							if (showUVsHelperGrid == false) {
							
								if (uvPoints != null) {
									if (uvPoints.Length > 0) {


										if ((Surforge.surforgeSettings.gridSnapActive == false) ||  (showOctreeGrid == false) ) {
											snappedPoint = hit.point;
										
											Vector3 newSnappedPointX = GetSnappedLineXuv(hit.point); //(optimized)
											Vector3 newSnappedPointZ = GetSnappedLineZuv(hit.point); //(optimized)
										
											float distX = Vector3.Distance(newSnappedPointX, hit.point);
											float distZ = Vector3.Distance(newSnappedPointZ, hit.point);
										
											if (distX < distZ) {
												if (distX < snapTreshold) {
													snappedPoint = newSnappedPointX;
												}
											}
											else {
												if (distZ < snapTreshold) {
													snappedPoint = newSnappedPointZ;
												}
											}
										}

									

										//snap to UV points (optimized)

										Vector3 newSnappedPoint = new Vector3(uvPoints[0].x * rootScaleX, uvPoints[0].y, uvPoints[0].z * rootScaleZ);
										float closestDistance = Mathf.Infinity;

										for (int i=0; i < uvPoints.Length; i++) {

											bool inSnapRange = false;
											
											if ( (hit.point.x > (uvPoints[i].x * rootScaleX - snapRangeOffset)) && 
											    (hit.point.x < (uvPoints[i].x * rootScaleX + snapRangeOffset)) &&
											    (hit.point.z > (uvPoints[i].z * rootScaleZ - snapRangeOffset)) && 
											    (hit.point.z < (uvPoints[i].z * rootScaleZ + snapRangeOffset)) ) {
												
												inSnapRange = true;
											}

											if (inSnapRange) {
												float newDistance = Vector3.Distance (new Vector3(uvPoints[i].x * rootScaleX, uvPoints[i].y, uvPoints[i].z * rootScaleZ), hit.point);
												if (newDistance < closestDistance) {
													closestDistance = newDistance;
													newSnappedPoint = new Vector3(uvPoints[i].x * rootScaleX, uvPoints[i].y, uvPoints[i].z * rootScaleZ);
												}
											}

										}

										if (Vector3.Distance(newSnappedPoint, hit.point) < snapTreshold) {
											snappedPoint = newSnappedPoint;
											snappedToUvPoint = true;
										}
										else {
											if ((Surforge.surforgeSettings.gridSnapActive == true) &&  (showOctreeGrid == true)) {

												newSnappedPoint = new Vector3(Mathf.Round(hit.point.x),hit.point.y, Mathf.Round(hit.point.z));
											
												if (Vector3.Distance(snappedPoint, hit.point) > snapTreshold) {
													snappedPoint = new Vector3(newSnappedPoint.x, newSnappedPoint.y, newSnappedPoint.z);
												}

											}
										}

									
									}
								}
							
							}
						
							// helpers snap

							else {  
								if (uvHelperSnapPoints != null) {
									if (uvHelperSnapPoints.Count > 0) {
									
										if ((Surforge.surforgeSettings.gridSnapActive == false) ||  (showOctreeGrid == false) ) {
											Vector3 newSnappedLineX = GetSnappedLineX(hit.point);
											Vector3 newSnappedLineZ = GetSnappedLineZ(hit.point);
										
										
											float distX = Vector3.Distance(new Vector3 (newSnappedLineX.x, hit.point.y, hit.point.z), hit.point);
											float distZ = Vector3.Distance(new Vector3 (hit.point.x, hit.point.y, newSnappedLineZ.z), hit.point);
										
											if (distX < distZ) {
												if (distX < snapTreshold) {
													snappedPoint = new Vector3 (newSnappedLineX.x, hit.point.y, hit.point.z);

													snapState = 6;
												}
											}
											else {
												if (distZ < snapTreshold) {
													snappedPoint = new Vector3 (hit.point.x, hit.point.y, newSnappedLineZ.z);

													snapState = 6;
												}
											}
										}
									
										//snap to Helper points
									
										Vector3 newSnappedPoint = new Vector3( uvHelperSnapPoints[0].x * rootScaleX, 
										                                      hit.point.y,
										                                      uvHelperSnapPoints[0].z * rootScaleZ); 
										float closestDistance = Vector3.Distance (newSnappedPoint, hit.point);
										for (int i=0; i < uvHelperSnapPoints.Count; i++) {
											float newDistance = Vector3.Distance (new Vector3( uvHelperSnapPoints[i].x * rootScaleX, 
											                                                  hit.point.y,
											                                                  uvHelperSnapPoints[i].z * rootScaleZ), hit.point);
											if (newDistance < closestDistance) {
												closestDistance = newDistance;
												newSnappedPoint = new Vector3( uvHelperSnapPoints[i].x * rootScaleX, 
												                              hit.point.y,
												                              uvHelperSnapPoints[i].z * rootScaleZ);
											}
										}
										if (Vector3.Distance(newSnappedPoint, hit.point) < snapTreshold) {
											snappedPoint = newSnappedPoint;
											snappedToHelpersPoint = true;

											snapState = 5;
										}
										else {
											if ((Surforge.surforgeSettings.gridSnapActive == true) &&  (showOctreeGrid == true)) {
												newSnappedPoint = new Vector3(Mathf.Round(hit.point.x),hit.point.y, Mathf.Round(hit.point.z));
											
												if (Vector3.Distance(snappedPoint, hit.point) > snapTreshold) {
													snappedPoint = new Vector3(newSnappedPoint.x, newSnappedPoint.y, newSnappedPoint.z);

													snapState = 5;
												}
											}
										}
									
									
									}
								}
							}

						}


					
						crossIndex = new List<int>();
						//snap to self lines
					
						if ( (!snappedToUvPoint) && (!snappedToHelpersPoint) && (polygonLassoPoints.Count > 0) && (Surforge.surforgeSettings.selfSnapActive) ) {
						
							Vector3 newSelfSnappedLineX = GetSelfSnappedLineX(hit.point);
							Vector3 newSelfSnappedLineZ = GetSelfSnappedLineZ(hit.point);
						
						
						
							float selfDistX = Vector3.Distance(new Vector3 (newSelfSnappedLineX.x, hit.point.y, hit.point.z), hit.point);
							float selfDistZ = Vector3.Distance(new Vector3 (hit.point.x, hit.point.y, newSelfSnappedLineZ.z), hit.point);
						
							if (selfDistX < selfDistZ) {
								if (selfDistX < selfSnapTreshold) {
									snappedPoint = new Vector3 (newSelfSnappedLineX.x, hit.point.y, hit.point.z);
									if (polygonLassoPoints.Count > selfSnappedLineIndexX) {
										snapGuideToDrawSelfLinesX = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexX]};
										crossIndex.Add(selfSnappedLineIndexX);

										snapState = 4;
									}
								}
							}
							else {
								if (selfDistZ < selfSnapTreshold) {
									snappedPoint = new Vector3 (hit.point.x, hit.point.y, newSelfSnappedLineZ.z);
									if (polygonLassoPoints.Count > selfSnappedLineIndexZ) {
										snapGuideToDrawSelfLinesZ = new Vector3[] {snappedPoint, polygonLassoPoints[selfSnappedLineIndexZ]};
										crossIndex.Add(selfSnappedLineIndexZ);

										snapState = 4;
									}
								}
							}
						}
					
					
						//snap to cross of self lines
					
						if ( (!snappedToUvPoint) && (!snappedToHelpersPoint) && (Surforge.surforgeSettings.selfSnapActive) )  {
						
							List<Vector3> crossPointsSelf = new List<Vector3>();
							crossPointsSelfIndexesX = new List<int>();
							crossPointsSelfIndexesZ = new List<int>();
							for (int i=0; i < polygonLassoPoints.Count; i++) {
								for (int j=0; j < polygonLassoPoints.Count; j++) {
									if (Surforge.TestLinesIntersection                             (new Vector2(polygonLassoPoints[i].x, polygonLassoPoints[i].z) , new Vector2(polygonLassoPoints[i].x +1, polygonLassoPoints[i].z), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z +1)) ) {
										Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(polygonLassoPoints[i].x, polygonLassoPoints[i].z) , new Vector2(polygonLassoPoints[i].x +1, polygonLassoPoints[i].z), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z +1));
										Vector3 snappedPoint = new Vector3(intersectionPoint.x, polygonLassoPoints[j].y, intersectionPoint.y);
										crossPointsSelf.Add(snappedPoint);
										crossPointsSelfIndexesX.Add (i);
										crossPointsSelfIndexesZ.Add (j);
									}
								}
							}
						
							if (crossPointsSelf.Count > 0) {
								int closestCrossSelfIndex = 0;
								Vector3 selfCrossSnappedPoint = crossPointsSelf[0];
								float closestDistanceSelfCross = Vector3.Distance (selfCrossSnappedPoint, hit.point);
								for (int i=0; i < crossPointsSelf.Count; i++) {
									float newDistance = Vector3.Distance (crossPointsSelf[i], hit.point);
									if (newDistance < closestDistanceSelfCross) {
										closestDistanceSelfCross = newDistance;
										selfCrossSnappedPoint = crossPointsSelf[i];
										closestCrossSelfIndex = i;
									}
								}
								if (Vector3.Distance(selfCrossSnappedPoint, hit.point) < selfSnapTreshold) {
									snappedPoint = selfCrossSnappedPoint;
									snapGuideToDrawSelfLinesCrossX = new Vector3[] {snappedPoint, polygonLassoPoints[crossPointsSelfIndexesX[closestCrossSelfIndex]]};
									snapGuideToDrawSelfLinesCrossZ = new Vector3[] {snappedPoint, polygonLassoPoints[crossPointsSelfIndexesZ[closestCrossSelfIndex]]};
								
									crossIndex.Add(crossPointsSelfIndexesX[closestCrossSelfIndex]);
									crossIndex.Add(crossPointsSelfIndexesZ[closestCrossSelfIndex]);

									snapState = 3;
								}
							}
							
						}
					
					
						RemovePolyLassoIndexesForCheckCrossIntersectionsNonUniqs();
					
						//snap to cross helper lines

						if ((showUVsHelperGrid == true) && (!snappedToHelpersPoint) && (uvHelperSnapPoints != null) && (uvsFound == true) && (polygonLassoPoints.Count > 0)  && (Surforge.surforgeSettings.uvSnapActive == true)) {
						
							List<Vector3> crossPoints = new List<Vector3>();
							List<int> crossPointsIndexes = new List<int>();
						
							for (int i=0; i < uvHelperVectorPairs.Count; i++) {
								if (uvHelperVectorPairs[i].isVertical) {
									float pMax = 0;
									float pMin = 0;
									if (uvHelperVectorPairs[i].a.z * rootScaleZ < uvHelperVectorPairs[i].b.z * rootScaleZ) {
										pMax = uvHelperVectorPairs[i].b.z * rootScaleZ;
										pMin = uvHelperVectorPairs[i].a.z * rootScaleZ;
									}
									else {
										pMax = uvHelperVectorPairs[i].a.z * rootScaleZ;
										pMin = uvHelperVectorPairs[i].b.z * rootScaleZ;
									}
								
									for (int j=0; j < crossIndex.Count; j++) {
										if ( (polygonLassoPoints[crossIndex[j]].z > pMin) && (polygonLassoPoints[crossIndex[j]].z <= pMax) ) {
											if (Surforge.TestLinesIntersection                             (new Vector2(uvHelperVectorPairs[i].a.x * rootScaleX, uvHelperVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvHelperVectorPairs[i].b.x * rootScaleX, uvHelperVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z) )) {
												Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvHelperVectorPairs[i].a.x * rootScaleX, uvHelperVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvHelperVectorPairs[i].b.x * rootScaleX, uvHelperVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z) );
												Vector3 snappedPoint = new Vector3(intersectionPoint.x, hit.point.y, intersectionPoint.y);
												crossPoints.Add(snappedPoint);
											
												crossPointsIndexes.Add(crossIndex[j]);
											}
										}
									}
									
								}
							}
						
						
							for (int i=0; i < uvHelperVectorPairs.Count; i++) {
								if (!uvHelperVectorPairs[i].isVertical) {
									float pMax = 0;
									float pMin = 0;
									if (uvHelperVectorPairs[i].a.x * rootScaleX < uvHelperVectorPairs[i].b.x * rootScaleX) {
										pMax = uvHelperVectorPairs[i].b.x * rootScaleX;
										pMin = uvHelperVectorPairs[i].a.x * rootScaleX;
									}
									else {
										pMax = uvHelperVectorPairs[i].a.x * rootScaleX;
										pMin = uvHelperVectorPairs[i].b.x * rootScaleX;
									}
								
									for (int j=0; j < crossIndex.Count; j++) {
										if ( (polygonLassoPoints[crossIndex[j]].x > pMin) && (polygonLassoPoints[crossIndex[j]].x <= pMax) ) {
											if (Surforge.TestLinesIntersection                             (new Vector2(uvHelperVectorPairs[i].a.x * rootScaleX, uvHelperVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvHelperVectorPairs[i].b.x * rootScaleX, uvHelperVectorPairs[i].b.z * rootScaleZ),  new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z+1)) ) {
												Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvHelperVectorPairs[i].a.x * rootScaleX, uvHelperVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvHelperVectorPairs[i].b.x * rootScaleX, uvHelperVectorPairs[i].b.z * rootScaleZ),  new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z+1));
												Vector3 snappedPoint = new Vector3(intersectionPoint.x, hit.point.y, intersectionPoint.y);
												crossPoints.Add(snappedPoint);
											
												crossPointsIndexes.Add(crossIndex[j]);
											}
										}
									}
								
								}
							}
						
						
							if (crossPoints.Count > 0) {
								int closestCrossIndex = 0;
								Vector3 selfCrossSnappedPoint = crossPoints[0];
								float closestDistanceSelfCross = Vector3.Distance (selfCrossSnappedPoint, hit.point);
								for (int i=0; i < crossPoints.Count; i++) {
									float newDistance = Vector3.Distance (crossPoints[i], hit.point);
									if (newDistance < closestDistanceSelfCross) {
										closestDistanceSelfCross = newDistance;
										selfCrossSnappedPoint = crossPoints[i];
									
										closestCrossIndex = i;
									}
								}
								if (Vector3.Distance(selfCrossSnappedPoint, hit.point) < selfSnapTreshold) {
									snappedPoint = selfCrossSnappedPoint;
								
									snapGuideToDrawSelfLinesCrossHelpers = new Vector3[] {snappedPoint, polygonLassoPoints[crossPointsIndexes[closestCrossIndex]]};

									snapState = 5;
								}
							}
						}

					
					
					
						//snap to cross UV lines (partly optimized)

						if ((Surforge.surforgeSettings.uvSnapActive == true) && (uvsFound == true) && (!snappedToUvPoint) && (polygonLassoPoints.Count > 0) ) {

							List<Vector3> crossPoints = new List<Vector3>();
							if (showUVsHelperGrid == false) {
								crossPoints = GetSnappedPointsUVCross(hit.point); //(optimized) 
							}
							else {
								crossPoints = GetSnappedPointsUvBorderCross(hit.point);
							}


						
							if (crossPoints.Count > 0) {
								int closestCrossIndex = 0;
								Vector3 selfCrossSnappedPoint = crossPoints[0];
								float closestDistanceSelfCross = Vector3.Distance (selfCrossSnappedPoint, hit.point);
								for (int i=0; i < crossPoints.Count; i++) {
									float newDistance = Vector3.Distance (crossPoints[i], hit.point);
									if (newDistance < closestDistanceSelfCross) {
										closestDistanceSelfCross = newDistance;
										selfCrossSnappedPoint = crossPoints[i];
									
										closestCrossIndex = i;
									}
								}
								if (Vector3.Distance(selfCrossSnappedPoint, hit.point) < snapTreshold) {
									snappedPoint = selfCrossSnappedPoint;
								
									snapGuideToDrawSelfLinesCrossHelpers = new Vector3[] {snappedPoint, polygonLassoPoints[crossPointsIndexesUV[closestCrossIndex]]};
								
								}
							}
						
						} 




						//snap to Poly Lasso Objects Lines 
						if ((Surforge.surforgeSettings.polyLassoObjects != null) && (Surforge.surforgeSettings.objectSnapActive)) {
							if (Surforge.surforgeSettings.polyLassoObjects.Count > 0) {
									
								Vector3 newSnappedPointX = GetSnappedLineXobject(hit.point);
								Vector3 newSnappedPointZ = GetSnappedLineZobject(hit.point);

							
								float distX = Vector3.Distance(newSnappedPointX, hit.point);
								float distZ = Vector3.Distance(newSnappedPointZ, hit.point);
								
								if (distX < distZ) {
									if (distX < polyLassoObjectSnapTreshold) {
										snappedPoint = newSnappedPointX;
										ClearCrossHelperLines();
										snapState = 2;
									}
								}
								else {
									if (distZ < polyLassoObjectSnapTreshold) {
										snappedPoint = newSnappedPointZ;
										ClearCrossHelperLines();
										snapState = 2;
									}
								}


							}
						}





						//snap to Poly Lasso Objects Cross Lines
						if ((Surforge.surforgeSettings.polyLassoObjects != null) && (Surforge.surforgeSettings.objectSnapActive)) {
							if (Surforge.surforgeSettings.polyLassoObjects.Count > 0) {

								List<Vector3> crossPoints = GetSnappedPointsObjectLinesCross(hit.point);

								if (crossPoints.Count > 0) {
									int closestCrossIndex = 0;
									Vector3 selfCrossSnappedPoint = crossPoints[0];
									float closestDistanceSelfCross = Vector3.Distance (selfCrossSnappedPoint, hit.point);
									for (int i=0; i < crossPoints.Count; i++) {
										float newDistance = Vector3.Distance (crossPoints[i], hit.point);
										if (newDistance < closestDistanceSelfCross) {
											closestDistanceSelfCross = newDistance;
											selfCrossSnappedPoint = crossPoints[i];
										
											closestCrossIndex = i;
										}
									}
									if (Vector3.Distance(selfCrossSnappedPoint, hit.point) < polyLassoObjectSnapTreshold) {
										snappedPoint = selfCrossSnappedPoint;
									
										snapGuideToDrawSelfLinesCrossHelpers = new Vector3[] {snappedPoint, polygonLassoPoints[crossPointsIndexesUV[closestCrossIndex]]};

										snapState = 1;
									}
								}

							}
						}





						//snap to Poly Lasso Objects points
						if ((Surforge.surforgeSettings.polyLassoObjects != null) && (Surforge.surforgeSettings.objectSnapActive)) {
							if (Surforge.surforgeSettings.polyLassoObjects.Count > 0) {

								List<Vector3> objectsPoints = new List<Vector3>();
								objectsPoints = GetObjectsSnapPoints();

								if (objectsPoints.Count > 0) {

									Vector3 objectsSnappedPoint = objectsPoints[0];
									float closestDistanceObjects = Vector3.Distance (objectsSnappedPoint, hit.point);
									for (int i=0; i < objectsPoints.Count; i++) {
										float newDistance = Vector3.Distance (new Vector3(objectsPoints[i].x, hit.point.y, objectsPoints[i].z) , hit.point);
										if (newDistance < closestDistanceObjects) {
											closestDistanceObjects = newDistance;
											objectsSnappedPoint = new Vector3(objectsPoints[i].x, hit.point.y, objectsPoints[i].z);
										}
									}
									if (Vector3.Distance(objectsSnappedPoint, hit.point) < polyLassoObjectSnapTreshold) {
										snappedPoint = objectsSnappedPoint;
							
										ClearCrossHelperLines();
										snapState = 1;
									}
										
								}

							}
						}


					
					
						//snap to cross grid lines

						if ((Surforge.surforgeSettings.gridSnapActive == true) &&  (showOctreeGrid == true) && (!snappedToUvPoint)) {
						
							List<Vector3> crossPoints = SnapToCrossGridLines();
						
						
							if (crossPoints.Count > 0) {
								int closestCrossIndex = 0;
								Vector3 selfCrossSnappedPoint = crossPoints[0];
								float closestDistanceSelfCross = Vector3.Distance (selfCrossSnappedPoint, hit.point);
								for (int i=0; i < crossPoints.Count; i++) {
									float newDistance = Vector3.Distance (crossPoints[i], hit.point);
									if (newDistance < closestDistanceSelfCross) {
										closestDistanceSelfCross = newDistance;
										selfCrossSnappedPoint = crossPoints[i];
									
										closestCrossIndex = i;
									}
								}
								if (Vector3.Distance(selfCrossSnappedPoint, hit.point) < snapTreshold) {
									snappedPoint = selfCrossSnappedPoint;
								
									snapGuideToDrawSelfLinesCrossHelpers = new Vector3[] {snappedPoint, polygonLassoPoints[crossGridLineIndexes[closestCrossIndex]]};
								
								}
							}
						
						}
					
					
					
						//snap to self points
						if ((polygonLassoPoints.Count > 1) && (!snappedToUvPoint)) {
						
							Vector3 selfSnappedPoint = polygonLassoPoints[0];
							float closestDistanceSelf = Vector3.Distance (selfSnappedPoint, hit.point);
							for (int i=0; i < polygonLassoPoints.Count; i++) {
								float newDistance = Vector3.Distance (polygonLassoPoints[i], hit.point);
								if (newDistance < closestDistanceSelf) {
									closestDistanceSelf = newDistance;
									selfSnappedPoint = polygonLassoPoints[i];
								}
							}
							if (Vector3.Distance(selfSnappedPoint, hit.point) < selfSnapTreshold) {
								snappedPoint = selfSnappedPoint;
							
								ClearCrossHelperLines();
							}
						}
					
					
					}
				}	
			}
			
		}
	}
	
	static int selfSnappedLineIndexX;
	static int selfSnappedLineIndexZ;
	static List<int> crossPointsSelfIndexesX;
	static List<int> crossPointsSelfIndexesZ;
	
	static List<int> crossIndex;


	static List<Vector3> GetObjectsSnapPoints() {
		List<Vector3> result = new List<Vector3>();
		for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects.Count; i++) {
			if (Surforge.surforgeSettings.polyLassoObjects[i] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[i].shapePointPairs != null) {
					for (int p=0; p < Surforge.surforgeSettings.polyLassoObjects[i].shapePointPairs.Count; p++) {
						result.Add(Surforge.surforgeSettings.polyLassoObjects[i].shapePointPairs[p].a);
					}
				}
			}
		}
		return result;
	}



	
	static void RemovePolyLassoIndexesForCheckCrossIntersectionsNonUniqs() {
		List<int> tmp = new List<int>();
		for (int i=0; i< crossIndex.Count; i++) {
			tmp.Add(crossIndex[i]);
		}
		crossIndex.Clear();
		
		for (int i=0; i< tmp.Count; i++) {
			bool uniq = true;
			for (int j=0; j< crossIndex.Count; j++) {
				if (tmp[i] == crossIndex[j]) {
					uniq = false;
					break;
				}
			}
			if (uniq) crossIndex.Add(tmp[i]);
		}
	}
	
	static List<int> crossGridLineIndexes;
	
	static List<Vector3> SnapToCrossGridLines() {
		List<Vector3> crossPoints = new List<Vector3>();
		
		crossGridLineIndexes = new List<int>();
		
		int gridLinesX = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX);
		int gridLinesZ = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ);
		
		for (int i=0; i < gridLinesX; i++) {
			for (int j=0; j < polygonLassoPoints.Count; j++) {
				if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.textureBorders.maxX, Surforge.surforgeSettings.textureBorders.minZ + i) , new Vector2(Surforge.surforgeSettings.textureBorders.minX, Surforge.surforgeSettings.textureBorders.minZ + i), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x , polygonLassoPoints[j].z+1)) ) {
					Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.textureBorders.maxX, Surforge.surforgeSettings.textureBorders.minZ + i) , new Vector2(Surforge.surforgeSettings.textureBorders.minX, Surforge.surforgeSettings.textureBorders.minZ + i), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x , polygonLassoPoints[j].z+1));
					Vector3 snappedPoint = new Vector3(intersectionPoint.x, polygonLassoPoints[j].y, intersectionPoint.y);
					crossPoints.Add(snappedPoint);
					
					crossGridLineIndexes.Add (j);
				}
			}
		}
		
		
		for (int i=0; i < gridLinesZ; i++) {
			for (int j=0; j < polygonLassoPoints.Count; j++) {
				if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.textureBorders.minX + i, Surforge.surforgeSettings.textureBorders.maxZ) , new Vector2(Surforge.surforgeSettings.textureBorders.minX + i, Surforge.surforgeSettings.textureBorders.minZ), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x +1, polygonLassoPoints[j].z )) ) {
					Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.textureBorders.minX + i, Surforge.surforgeSettings.textureBorders.maxZ) , new Vector2(Surforge.surforgeSettings.textureBorders.minX + i, Surforge.surforgeSettings.textureBorders.minZ), new Vector2(polygonLassoPoints[j].x, polygonLassoPoints[j].z), new Vector2(polygonLassoPoints[j].x +1, polygonLassoPoints[j].z ));
					Vector3 snappedPoint = new Vector3(intersectionPoint.x, polygonLassoPoints[j].y, intersectionPoint.y);
					crossPoints.Add(snappedPoint);
					
					crossGridLineIndexes.Add (j);
				}
			}
		}
		return crossPoints;
	}
	
	
	static Vector3 GetGridSnappedLineX(Vector3 hitPoint) {
		int gridLinesX = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX);
		
		Vector3 newGridSnappedLineX = new Vector3();
		float closestDistanceXGrid = Mathf.Infinity;
		
		for (int i=0; i < gridLinesX; i++) {
			float newDistanceX = Vector3.Distance (new Vector3(Surforge.surforgeSettings.textureBorders.minX + i, hitPoint.y, hitPoint.z), hitPoint);
			if (newDistanceX < closestDistanceXGrid) {
				closestDistanceXGrid = newDistanceX;
				newGridSnappedLineX = new Vector3(Surforge.surforgeSettings.textureBorders.minX + i, hitPoint.y, hitPoint.z);
			}
		}
		return newGridSnappedLineX;
	}
	
	
	static Vector3 GetGridSnappedLineZ(Vector3 hitPoint) {
		int gridLinesZ = Mathf.RoundToInt(Surforge.surforgeSettings.textureBorders.maxZ - Surforge.surforgeSettings.textureBorders.minZ);
		
		Vector3 newGridSnappedLineZ = new Vector3();
		float closestDistanceZGrid = Mathf.Infinity;
		
		for (int i=0; i < gridLinesZ; i++) {
			float newDistanceZ = Vector3.Distance (new Vector3(hitPoint.x, hitPoint.y, Surforge.surforgeSettings.textureBorders.minZ + i), hitPoint);
			if (newDistanceZ < closestDistanceZGrid) {
				closestDistanceZGrid = newDistanceZ;
				newGridSnappedLineZ = new Vector3(hitPoint.x, hitPoint.y, Surforge.surforgeSettings.textureBorders.minZ + i);
			}
		}
		return newGridSnappedLineZ;
	}
	
	
	
	static Vector3 GetSelfSnappedLineX(Vector3 hitPoint) {
		Vector3 newSelfSnappedLineX = new Vector3();
		float closestDistanceXSelf = Mathf.Infinity;
		for (int i=0; i < polygonLassoPoints.Count; i++) {
			float newDistanceX = Vector3.Distance (new Vector3(polygonLassoPoints[i].x, hitPoint.y, hitPoint.z), hitPoint);
			if (newDistanceX < closestDistanceXSelf) {
				closestDistanceXSelf = newDistanceX;
				newSelfSnappedLineX = polygonLassoPoints[i];
				selfSnappedLineIndexX = i;
			}
		}
		return newSelfSnappedLineX;
	}
	
	
	static Vector3 GetSelfSnappedLineZ(Vector3 hitPoint) {
		Vector3 newSelfSnappedLineZ = new Vector3();
		float closestDistanceZSelf = Mathf.Infinity;
		for (int i=0; i < polygonLassoPoints.Count; i++) {
			float newDistanceZ = Vector3.Distance (new Vector3(hitPoint.x, hitPoint.y, polygonLassoPoints[i].z), hitPoint);
			if (newDistanceZ < closestDistanceZSelf) {
				closestDistanceZSelf = newDistanceZ;
				newSelfSnappedLineZ = polygonLassoPoints[i];
				selfSnappedLineIndexZ = i;
			}
		}
		return newSelfSnappedLineZ;
	}



	static List<Vector3> GetSnappedPointsObjectLinesCross(Vector3 hitPoint){
		crossPointsIndexesUV = new List<int>();
		
		List<Vector3> crossPoints = new List<Vector3>();

		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {
						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
						}
						for (int j=0; j < crossIndex.Count; j++) {
							if ( (polygonLassoPoints[crossIndex[j]].z > pMin) && (polygonLassoPoints[crossIndex[j]].z <= pMax) ) {
								Vector3 snappedPoint = new Vector3();
								if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z)) ) {
									Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z));
									snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
									
									crossPoints.Add(snappedPoint);
									crossPointsIndexesUV.Add(crossIndex[j]);
								}
							}
						}


					}
				}
			}
		} 


		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {

						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
						}
						for (int j=0; j < crossIndex.Count; j++) {
							if ( (polygonLassoPoints[crossIndex[j]].x > pMin) && (polygonLassoPoints[crossIndex[j]].x <= pMax) ) {
								Vector3 snappedPoint = new Vector3();
								if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1)) ) {
									Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1));
									snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
									
									crossPoints.Add(snappedPoint);
									crossPointsIndexesUV.Add(crossIndex[j]);
								}
							} 
						}

					}
				}
			}
		}

		return crossPoints; 
	}


	
	
	static List<int> crossPointsIndexesUV;

	//optimized
	static List<Vector3> GetSnappedPointsUVCross(Vector3 hitPoint) {
		crossPointsIndexesUV = new List<int>();
		
		List<Vector3> crossPoints = new List<Vector3>();
		
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.z * rootScaleZ < uvVectorPairs[i].b.z * rootScaleZ) {
					pMax = uvVectorPairs[i].b.z * rootScaleZ;
					pMin = uvVectorPairs[i].a.z * rootScaleZ;
				}
				else {
					pMax = uvVectorPairs[i].a.z * rootScaleZ;
					pMin = uvVectorPairs[i].b.z * rootScaleZ;
				}
				for (int j=0; j < crossIndex.Count; j++) {
					if ( (polygonLassoPoints[crossIndex[j]].z > pMin) && (polygonLassoPoints[crossIndex[j]].z <= pMax) ) {
						Vector3 snappedPoint = new Vector3();
						if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z)) ) {
							Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z));
							snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
						
							crossPoints.Add(snappedPoint);
							crossPointsIndexesUV.Add(crossIndex[j]);
						}
					}
				}

			}
			
		}
		
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.x * rootScaleX < uvVectorPairs[i].b.x * rootScaleX) {
					pMax = uvVectorPairs[i].b.x * rootScaleX;
					pMin = uvVectorPairs[i].a.x * rootScaleX;
				}
				else {
					pMax = uvVectorPairs[i].a.x * rootScaleX;
					pMin = uvVectorPairs[i].b.x * rootScaleX;
				}
				for (int j=0; j < crossIndex.Count; j++) {
					if ( (polygonLassoPoints[crossIndex[j]].x > pMin) && (polygonLassoPoints[crossIndex[j]].x <= pMax) ) {
						Vector3 snappedPoint = new Vector3();
						if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1)) ) {
							Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1));
							snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
						
							crossPoints.Add(snappedPoint);
							crossPointsIndexesUV.Add(crossIndex[j]);
						}
					} 
				}

			}
			
		}
		return crossPoints;
	}


	static List<Vector3> GetSnappedPointsUvBorderCross(Vector3 hitPoint) {
		crossPointsIndexesUV = new List<int>();
		
		List<Vector3> crossPoints = new List<Vector3>();
		
		for (int i=0; i < uvVectorPairsBorder.Count; i++) {
			float pMax = 0;
			float pMin = 0;
			if (uvVectorPairsBorder[i].a.z * rootScaleZ < uvVectorPairsBorder[i].b.z * rootScaleZ) {
				pMax = uvVectorPairsBorder[i].b.z * rootScaleZ;
				pMin = uvVectorPairsBorder[i].a.z * rootScaleZ;
			}
			else {
				pMax = uvVectorPairsBorder[i].a.z * rootScaleZ;
				pMin = uvVectorPairsBorder[i].b.z * rootScaleZ;
			}
			for (int j=0; j < crossIndex.Count; j++) {
				if ( (polygonLassoPoints[crossIndex[j]].z > pMin) && (polygonLassoPoints[crossIndex[j]].z <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairsBorder[i].a.x * rootScaleX, uvVectorPairsBorder[i].a.z * rootScaleZ) , new Vector2(uvVectorPairsBorder[i].b.x * rootScaleX, uvVectorPairsBorder[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairsBorder[i].a.x * rootScaleX, uvVectorPairsBorder[i].a.z * rootScaleZ) , new Vector2(uvVectorPairsBorder[i].b.x * rootScaleX, uvVectorPairsBorder[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x +1, polygonLassoPoints[crossIndex[j]].z));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
						
						crossPoints.Add(snappedPoint);
						crossPointsIndexesUV.Add(crossIndex[j]);
					}
				}
			}
			
		}
		
		for (int i=0; i < uvVectorPairsBorder.Count; i++) {
			float pMax = 0;
			float pMin = 0;
			if (uvVectorPairsBorder[i].a.x * rootScaleX < uvVectorPairsBorder[i].b.x * rootScaleX) {
				pMax = uvVectorPairsBorder[i].b.x * rootScaleX;
				pMin = uvVectorPairsBorder[i].a.x * rootScaleX;
			}
			else {
				pMax = uvVectorPairsBorder[i].a.x * rootScaleX;
				pMin = uvVectorPairsBorder[i].b.x * rootScaleX;
			}
			for (int j=0; j < crossIndex.Count; j++) {
				if ( (polygonLassoPoints[crossIndex[j]].x > pMin) && (polygonLassoPoints[crossIndex[j]].x <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairsBorder[i].a.x * rootScaleX, uvVectorPairsBorder[i].a.z * rootScaleZ) , new Vector2(uvVectorPairsBorder[i].b.x * rootScaleX, uvVectorPairsBorder[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairsBorder[i].a.x * rootScaleX, uvVectorPairsBorder[i].a.z * rootScaleZ) , new Vector2(uvVectorPairsBorder[i].b.x * rootScaleX, uvVectorPairsBorder[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z) , new Vector2(polygonLassoPoints[crossIndex[j]].x, polygonLassoPoints[crossIndex[j]].z +1));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
						
						crossPoints.Add(snappedPoint);
						crossPointsIndexesUV.Add(crossIndex[j]);
					}
				} 
			}
			
		}
		return crossPoints;
	}

	
	//optimized
	static Vector3 GetSnappedLineDiagonalUvShift(Vector3 hitPoint, bool isRightUp) {
		Vector3 newSnappedLine = new Vector3();
		float closestDistance = Mathf.Infinity;
		
		Vector2 diagonalEnd = new Vector2();
		if (isRightUp) {
			diagonalEnd = new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z +1);
		}
		else {
			diagonalEnd = new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z -1);
			
		}
		
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMaxX = 0;
				float pMinX = 0;
				float pMaxZ = 0;
				float pMinZ = 0;
				if (uvVectorPairs[i].a.z  * rootScaleZ < uvVectorPairs[i].b.z  * rootScaleZ) {
					pMaxZ = uvVectorPairs[i].b.z  * rootScaleZ;
					pMinZ = uvVectorPairs[i].a.z  * rootScaleZ;
				}
				else {
					pMaxZ = uvVectorPairs[i].a.z  * rootScaleZ;
					pMinZ = uvVectorPairs[i].b.z  * rootScaleZ;
				}
			
				if (uvVectorPairs[i].a.x  * rootScaleX < uvVectorPairs[i].b.x * rootScaleX) {
					pMaxX = uvVectorPairs[i].b.x * rootScaleX;
					pMinX = uvVectorPairs[i].a.x * rootScaleX;
				}
				else {
					pMaxX = uvVectorPairs[i].a.x * rootScaleX;
					pMinX = uvVectorPairs[i].b.x * rootScaleX;
				}
			
				if ( ((hitPoint.z > pMinZ) && (hitPoint.z <= pMaxZ)) || ((hitPoint.x > pMinX) && (hitPoint.x <= pMaxX)) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), diagonalEnd) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), diagonalEnd);
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
					}
				
					float newDistance = Vector3.Distance (snappedPoint, hitPoint);
					if (newDistance < closestDistance) {
						closestDistance = newDistance;
						newSnappedLine = snappedPoint;
					}
				}

			}
			
		}
		return newSnappedLine;
	}
	
	
	//optimized
	static Vector3 GetSnappedLineXuvShift(Vector3 hitPoint) {
		Vector3 newSnappedLineX = new Vector3();
		float closestDistanceX = Mathf.Infinity;
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.z * rootScaleZ < uvVectorPairs[i].b.z * rootScaleZ) {
					pMax = uvVectorPairs[i].b.z * rootScaleZ;
					pMin = uvVectorPairs[i].a.z * rootScaleZ;
				}
				else {
					pMax = uvVectorPairs[i].a.z * rootScaleZ;
					pMin = uvVectorPairs[i].b.z * rootScaleZ;
				}
				if ( (hitPoint.z > pMin) && (hitPoint.z <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
					}
				
					float newDistanceX = Vector3.Distance (snappedPoint, hitPoint);
					if (newDistanceX < closestDistanceX) {
						closestDistanceX = newDistanceX;
						newSnappedLineX = snappedPoint;
					}
				}

			}
			
		}
		return newSnappedLineX;
	}

	//optimized
	static Vector3 GetSnappedLineZuvShift(Vector3 hitPoint) {
		Vector3 newSnappedLineZ = new Vector3();
		float closestDistanceZ = Mathf.Infinity;
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.x * rootScaleX < uvVectorPairs[i].b.x * rootScaleX) {
					pMax = uvVectorPairs[i].b.x * rootScaleX;
					pMin = uvVectorPairs[i].a.x * rootScaleX;
				}
				else {
					pMax = uvVectorPairs[i].a.x * rootScaleX;
					pMin = uvVectorPairs[i].b.x * rootScaleX;
				}
				if ( (hitPoint.x > pMin) && (hitPoint.x <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z +1)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z +1));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
					}
				
					float newDistanceZ = Vector3.Distance (snappedPoint, hitPoint);
					if (newDistanceZ < closestDistanceZ) {
						closestDistanceZ = newDistanceZ;
						newSnappedLineZ = snappedPoint;
					}
				} 

			}
			
		}
		return newSnappedLineZ;
	}




	static Vector3 GetSnappedLineDiagonalObjectLineShift(Vector3 hitPoint, bool isRightUp) {
		Vector3 newSnappedLine = new Vector3();
		float closestDistance = Mathf.Infinity;
		
		Vector2 diagonalEnd = new Vector2();
		if (isRightUp) {
			diagonalEnd = new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z +1);
		}
		else {
			diagonalEnd = new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z -1);
			
		}

		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {
						float pMaxX = 0;
						float pMinX = 0;
						float pMaxZ = 0;
						float pMinZ = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z) {
							pMaxZ = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
							pMinZ = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
						}
						else {
							pMaxZ = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
							pMinZ = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
						}
						
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x) {
							pMaxX = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
							pMinX = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
						}
						else {
							pMaxX = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
							pMinX = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
						}
						
						if ( ((hitPoint.z > pMinZ) && (hitPoint.z <= pMaxZ)) || ((hitPoint.x > pMinX) && (hitPoint.x <= pMaxX)) ) {
							Vector3 snappedPoint = new Vector3();
							if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), diagonalEnd) ) {
								Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), diagonalEnd);
								snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
							}
							
							float newDistance = Vector3.Distance (snappedPoint, hitPoint);
							if (newDistance < closestDistance) {
								closestDistance = newDistance;
								newSnappedLine = snappedPoint;
							}
						}

					}
				}
			}
		}

		return newSnappedLine;
	}




	static Vector3 GetSnappedLineXobjectLineShift(Vector3 hitPoint) {
		Vector3 newSnappedLineX = new Vector3();
		float closestDistanceX = Mathf.Infinity;
		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {
						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
						}
						if ( (hitPoint.z > pMin) && (hitPoint.z <= pMax) ) {
							Vector3 snappedPoint = new Vector3();
							if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z)) ) {
								Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x + 1 , polygonLassoPoints[polygonLassoPoints.Count-1].z));
								snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
							}
							
							float newDistanceX = Vector3.Distance (snappedPoint, hitPoint);
							if (newDistanceX < closestDistanceX) {
								closestDistanceX = newDistanceX;
								newSnappedLineX = snappedPoint;
							}
						}

					}
				}
			}
		}

		return newSnappedLineX;
	}



	static Vector3 GetSnappedLineZobjectLineShift(Vector3 hitPoint) {
		Vector3 newSnappedLineZ = new Vector3();
		float closestDistanceZ = Mathf.Infinity;

		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {
						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
						}
						if ( (hitPoint.x > pMin) && (hitPoint.x <= pMax) ) {
							Vector3 snappedPoint = new Vector3();
							if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z +1)) ) {
								Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z), new Vector2(polygonLassoPoints[polygonLassoPoints.Count-1].x, polygonLassoPoints[polygonLassoPoints.Count-1].z +1));
								snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
							}
							
							float newDistanceZ = Vector3.Distance (snappedPoint, hitPoint);
							if (newDistanceZ < closestDistanceZ) {
								closestDistanceZ = newDistanceZ;
								newSnappedLineZ = snappedPoint;
							}
						} 

					}
				}
			}
		}

		return newSnappedLineZ;
	}





	static Vector3 GetSnappedLineXobject(Vector3 hitPoint) {
		Vector3 newSnappedLineX = new Vector3();
		float closestDistanceX = Mathf.Infinity;

		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {

						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z;
						}
						if ( (hitPoint.z > pMin) && (hitPoint.z <= pMax) ) {
							Vector3 snappedPoint = new Vector3();
							if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x + 1 , hitPoint.z)) ) {
								Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x + 1 , hitPoint.z));
								snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
							}
							
							float newDistanceX = Vector3.Distance (snappedPoint, hitPoint);
							if (newDistanceX < closestDistanceX) {
								closestDistanceX = newDistanceX;
								newSnappedLineX = snappedPoint;
							}
						}

					}
				}
			}
		}

		return newSnappedLineX;
	}



	static Vector3 GetSnappedLineZobject(Vector3 hitPoint) {
		Vector3 newSnappedLineZ = new Vector3();
		float closestDistanceZ = Mathf.Infinity;

		for (int s=0; s < Surforge.surforgeSettings.polyLassoObjects.Count; s++) {
			if (Surforge.surforgeSettings.polyLassoObjects[s] != null) {
				if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs != null) {
					for (int i=0; i < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs.Count; i++) {

						float pMax = 0;
						float pMin = 0;
						if (Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x < Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x) {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
						}
						else {
							pMax = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x;
							pMin = Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x;
						}
						if ( (hitPoint.x > pMin) && (hitPoint.x <= pMax) ) {
							Vector3 snappedPoint = new Vector3();
							if (Surforge.TestLinesIntersection                             (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x, hitPoint.z +1)) ) {
								Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].a.z) , new Vector2(Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.x, Surforge.surforgeSettings.polyLassoObjects[s].shapePointPairs[i].b.z), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x, hitPoint.z +1));
								snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
							}
							
							float newDistanceZ = Vector3.Distance (snappedPoint, hitPoint);
							if (newDistanceZ < closestDistanceZ) {
								closestDistanceZ = newDistanceZ;
								newSnappedLineZ = snappedPoint;
							}
						} 

					}
				}
			}
		}

		return newSnappedLineZ;
	}

	
	
	static Vector3 GetSnappedLineXuv(Vector3 hitPoint) {
		Vector3 newSnappedLineX = new Vector3();
		float closestDistanceX = Mathf.Infinity;
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.z * rootScaleZ < uvVectorPairs[i].b.z * rootScaleZ) {
					pMax = uvVectorPairs[i].b.z * rootScaleZ;
					pMin = uvVectorPairs[i].a.z * rootScaleZ;
				}
				else {
					pMax = uvVectorPairs[i].a.z * rootScaleZ;
					pMin = uvVectorPairs[i].b.z * rootScaleZ;
				}
				if ( (hitPoint.z > pMin) && (hitPoint.z <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x + 1 , hitPoint.z)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x + 1 , hitPoint.z));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
					}
				
					float newDistanceX = Vector3.Distance (snappedPoint, hitPoint);
					if (newDistanceX < closestDistanceX) {
						closestDistanceX = newDistanceX;
						newSnappedLineX = snappedPoint;
					}
				}

			}
			
		}
		return newSnappedLineX;
	}
	
	static Vector3 GetSnappedLineZuv(Vector3 hitPoint) {
		Vector3 newSnappedLineZ = new Vector3();
		float closestDistanceZ = Mathf.Infinity;
		for (int i=0; i < uvVectorPairs.Length; i++) {

			if (uvVectorPairs[i].inSnapRange) {

				float pMax = 0;
				float pMin = 0;
				if (uvVectorPairs[i].a.x * rootScaleX < uvVectorPairs[i].b.x * rootScaleX) {
					pMax = uvVectorPairs[i].b.x * rootScaleX;
					pMin = uvVectorPairs[i].a.x * rootScaleX;
				}
				else {
					pMax = uvVectorPairs[i].a.x * rootScaleX;
					pMin = uvVectorPairs[i].b.x * rootScaleX;
				}
				if ( (hitPoint.x > pMin) && (hitPoint.x <= pMax) ) {
					Vector3 snappedPoint = new Vector3();
					if (Surforge.TestLinesIntersection                             (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x, hitPoint.z +1)) ) {
						Vector2 intersectionPoint = Surforge.LineIntersectionPoint (new Vector2(uvVectorPairs[i].a.x * rootScaleX, uvVectorPairs[i].a.z * rootScaleZ) , new Vector2(uvVectorPairs[i].b.x * rootScaleX, uvVectorPairs[i].b.z * rootScaleZ), new Vector2(hitPoint.x, hitPoint.z), new Vector2(hitPoint.x, hitPoint.z +1));
						snappedPoint = new Vector3(intersectionPoint.x, hitPoint.y, intersectionPoint.y);
					}
				
					float newDistanceZ = Vector3.Distance (snappedPoint, hitPoint);
					if (newDistanceZ < closestDistanceZ) {
						closestDistanceZ = newDistanceZ;
						newSnappedLineZ = snappedPoint;
					}
				} 

			}
			
		}
		return newSnappedLineZ;
	}
	
	
	static Vector3 GetSnappedLineX(Vector3 hitPoint) {
		Vector3 newSnappedLineX = new Vector3();
		float closestDistanceX = Mathf.Infinity;
		for (int i=0; i < uvHelperVectorPairs.Count; i++) {
			if (uvHelperVectorPairs[i].isVertical) {
				float pMax = 0;
				float pMin = 0;
				if (uvHelperVectorPairs[i].a.z * rootScaleZ < uvHelperVectorPairs[i].b.z * rootScaleZ) {
					pMax = uvHelperVectorPairs[i].b.z * rootScaleZ;
					pMin = uvHelperVectorPairs[i].a.z * rootScaleZ;
				}
				else {
					pMax = uvHelperVectorPairs[i].a.z * rootScaleZ;
					pMin = uvHelperVectorPairs[i].b.z * rootScaleZ;
				}
				if ( (hitPoint.z > pMin) && (hitPoint.z <= pMax) ) {
					float newDistanceX = Vector3.Distance (new Vector3(uvHelperVectorPairs[i].a.x * rootScaleX, hitPoint.y, hitPoint.z), hitPoint);
					if (newDistanceX < closestDistanceX) {
						closestDistanceX = newDistanceX;
						newSnappedLineX = uvHelperVectorPairs[i].a  * rootScaleX;
					}
				}
			}
		}
		return newSnappedLineX;
	}
	
	static Vector3 GetSnappedLineZ(Vector3 hitPoint) {
		Vector3 newSnappedLineZ = new Vector3();
		float closestDistanceZ = Mathf.Infinity;
		for (int i=0; i < uvHelperVectorPairs.Count; i++) {
			if (!uvHelperVectorPairs[i].isVertical) {
				float pMax = 0;
				float pMin = 0;
				if (uvHelperVectorPairs[i].a.x  * rootScaleX < uvHelperVectorPairs[i].b.x * rootScaleX) {
					pMax = uvHelperVectorPairs[i].b.x * rootScaleX;
					pMin = uvHelperVectorPairs[i].a.x * rootScaleX;
				}
				else {
					pMax = uvHelperVectorPairs[i].a.x * rootScaleX;
					pMin = uvHelperVectorPairs[i].b.x * rootScaleX;
				}
				if ( (hitPoint.x > pMin) && (hitPoint.x <= pMax) ) {
					float newDistanceZ = Vector3.Distance (new Vector3(hitPoint.x, hitPoint.y, uvHelperVectorPairs[i].a.z * rootScaleZ), hitPoint);
					if (newDistanceZ < closestDistanceZ) {
						closestDistanceZ = newDistanceZ;
						newSnappedLineZ = uvHelperVectorPairs[i].a * rootScaleZ;
					}
				}
			}
		}
		return newSnappedLineZ;
	}
	
	
	static List<Vector3> symPointTmp;
	
	static void CheckMouseClickPolygonLasso() {
		Event e = Event.current;

		if (e.type == EventType.ScrollWheel) {
			if ((e.control) && (!e.alt)) {
				Surforge.LogAction("Arc curvature change", "Ctrl + Scroll", "");
				polyLassoArcCurvature = polyLassoArcCurvature + e.delta.y * 0.005f * -1;
				e.Use();
			}
			if ((e.control) && (e.alt)) {
				Surforge.LogAction("Arc points density change", "Ctrl + Alt + Scroll", "");

				if (e.delta.y < 0) { 
					polyLassoArcDensity = polyLassoArcDensity + 1;
				}
				else {
					if (e.delta.y > 0) {
						polyLassoArcDensity = polyLassoArcDensity - 1;
					}
				}
				if (polyLassoArcDensity > 5) polyLassoArcDensity = 5;
				if (polyLassoArcDensity < 1) polyLassoArcDensity = 1;

				e.Use();
			}
		}
		if ((e.type == EventType.MouseDown)  && e.button == 2) {

			if ((e.alt) && (e.control)) {
				Surforge.LogAction("Set arc curvature to 1/4 circle", "Ctrl + Alt + Middle Click", "");

				int direction = 1;
				if (polyLassoArcCurvature < 0) direction = -1;
				polyLassoArcCurvature = 0.20711f * direction; //magic: curvature for 1/4 circle in result, experimental
				e.Use();
			}
			else {
				if (e.control) {
					Surforge.LogAction("Arc mode toggle", "Ctrl + Middle Click", "");

					if (polyLassoArcCurvature != 0) {
						lastPolyLassoArcCurvature = polyLassoArcCurvature;
						polyLassoArcCurvature = 0;
					}
					else {
						polyLassoArcCurvature = lastPolyLassoArcCurvature;
					}
					e.Use();
				}
			}
		}
		
		if ((e.type == EventType.MouseDown)  && e.button == 0) {

			Surforge.LogAction("Set point", "Left Click", "");
			
			if ((e.control) && (polygonLassoPoints.Count > 0) ) {
				if ( !(e.alt ) ) {
					AddPolygonLassoPoint();
					if ( Surforge.surforgeSettings.symmetry && (Surforge.surforgeSettings.symmetryX ||Surforge.surforgeSettings.symmetryZ || Surforge.surforgeSettings.symmetryDiagonal) ) {
						PolygonLassoSplitWithSymmetry();
					}
					else PolygonLassoSplit();
				}
			}
			else {
				if ( !(e.alt ) ) {
					AddPolygonLassoPoint();
				}
			}
			
		}


		
		if ((e.type == EventType.KeyUp) &&  (e.keyCode == KeyCode.Backspace)) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 0) {
					Surforge.LogAction("Remove last point", "Backspace", "");
					polygonLassoPoints.Remove(polygonLassoPoints[polygonLassoPoints.Count - 1]);
				}
			}
		}

		
		if ( ((e.type == EventType.KeyUp) &&  ((e.keyCode == KeyCode.Return) ||(e.keyCode == KeyCode.KeypadEnter)) ) ||
		    ((e.isMouse && e.type == EventType.MouseDown && e.clickCount == 2) && (e.shift) ) ){
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 2) {
					Surforge.LogAction("Finish shape", "Double Click", "Enter");

					if ( Surforge.surforgeSettings.symmetry && (Surforge.surforgeSettings.symmetryX ||Surforge.surforgeSettings.symmetryZ || Surforge.surforgeSettings.symmetryDiagonal) ) {
						PolygonLassoBuildObjectsWithSymmetry();
					}
					else PolygonLassoBuildObject();
				}
			}
		}
		
		
		//symmetry
		if ((e.type == EventType.KeyUp) &&  (e.keyCode == KeyCode.Keypad5 )) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 1) {
					Surforge.LogAction("Repeat shape points, left to right", "Numpad 5", "");
					
					float offset = GetOffsetForContinueShape(polygonLassoPoints);
					symPointTmp = new List<Vector3>();
					for (int i = 0; i < polygonLassoPoints.Count; i++) {
						symPointTmp.Add(new Vector3( polygonLassoPoints[i].x + offset, polygonLassoPoints[i].y, polygonLassoPoints[i].z));
					}
				
					for (int i = 0; i< symPointTmp.Count; i++) {
						polygonLassoPoints.Add(symPointTmp[i]);
					}
					if (IsShapeLastAndFirstPointsMatch(polygonLassoPoints)) polygonLassoPoints.RemoveAt(polygonLassoPoints.Count-1);
				}
			}
		}

		
		if ((e.type == EventType.KeyUp) &&  ((e.keyCode == KeyCode.Keypad6) ||(e.keyCode == KeyCode.Keypad4)) ) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 1) {
					Surforge.LogAction("Continue shape with symmetry", "Numpad keys", "");
					
					Vector3 symPoint = polygonLassoPoints[polygonLassoPoints.Count-1];
					symPointTmp = new List<Vector3>();
					for (int i = polygonLassoPoints.Count-2 ; i >=0; i--) {
						symPointTmp.Add(new Vector3( symPoint.x - polygonLassoPoints[i].x + symPoint.x, polygonLassoPoints[i].y, polygonLassoPoints[i].z));
					}
					for (int i = 0; i< symPointTmp.Count; i++) {
						polygonLassoPoints.Add(symPointTmp[i]);
					}
					if (IsShapeLastAndFirstPointsMatch(polygonLassoPoints)) polygonLassoPoints.RemoveAt(polygonLassoPoints.Count-1);
				}
			}
		}
		
		
		if ((e.type == EventType.KeyUp) &&  ((e.keyCode == KeyCode.Keypad8) ||(e.keyCode == KeyCode.Keypad2)) ) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 1) {
					Surforge.LogAction("Continue shape with symmetry", "Numpad keys", "");
					
					Vector3 symPoint = polygonLassoPoints[polygonLassoPoints.Count-1];
					symPointTmp = new List<Vector3>();
					for (int i = polygonLassoPoints.Count-2 ; i >=0; i--) {
						symPointTmp.Add(new Vector3( polygonLassoPoints[i].x, polygonLassoPoints[i].y, symPoint.z - polygonLassoPoints[i].z + symPoint.z));
					}
					for (int i = 0; i< symPointTmp.Count; i++) {
						polygonLassoPoints.Add(symPointTmp[i]);
					}
					if (IsShapeLastAndFirstPointsMatch(polygonLassoPoints)) polygonLassoPoints.RemoveAt(polygonLassoPoints.Count-1);
				}
			}
		}
		
		
		//diagonal symmetry
		
		if ((e.type == EventType.KeyUp) &&  ((e.keyCode == KeyCode.Keypad9) || (e.keyCode == KeyCode.Keypad1)) ) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 1) {
					Surforge.LogAction("Continue shape with symmetry", "Numpad keys", "");
					
					Vector3 symPoint = polygonLassoPoints[polygonLassoPoints.Count-1];
					symPointTmp = new List<Vector3>();
					for (int i = polygonLassoPoints.Count-2 ; i >=0; i--) {
						
						Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(symPoint.x, symPoint.z), new Vector2(symPoint.x + 1, symPoint.z-1),  new Vector2(polygonLassoPoints[i].x, polygonLassoPoints[i].z) );
						Vector2 perp = new Vector2(polygonLassoPoints[i].x - perpPoint.x, polygonLassoPoints[i].z - perpPoint.y);
						perp = perp * -1.0f;
						symPointTmp.Add(new Vector3( polygonLassoPoints[i].x + perp.x + perp.x, polygonLassoPoints[i].y, polygonLassoPoints[i].z + perp.y + perp.y));
					}
					for (int i = 0; i< symPointTmp.Count; i++) {
						polygonLassoPoints.Add(symPointTmp[i]);
					}
					if (IsShapeLastAndFirstPointsMatch(polygonLassoPoints)) polygonLassoPoints.RemoveAt(polygonLassoPoints.Count-1);
				}
			}
		}
		
		if ((e.type == EventType.KeyUp) &&  ((e.keyCode == KeyCode.Keypad7) || (e.keyCode == KeyCode.Keypad3)) ) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count > 1) {
					Surforge.LogAction("Continue shape with symmetry", "Numpad keys", "");
					
					Vector3 symPoint = polygonLassoPoints[polygonLassoPoints.Count-1];
					symPointTmp = new List<Vector3>();
					for (int i = polygonLassoPoints.Count-2 ; i >=0; i--) {
						
						Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(symPoint.x, symPoint.z), new Vector2(symPoint.x - 1, symPoint.z-1),  new Vector2(polygonLassoPoints[i].x, polygonLassoPoints[i].z) );
						Vector2 perp = new Vector2(polygonLassoPoints[i].x - perpPoint.x, polygonLassoPoints[i].z - perpPoint.y);
						perp = perp * -1.0f;
						symPointTmp.Add(new Vector3( polygonLassoPoints[i].x + perp.x + perp.x, polygonLassoPoints[i].y, polygonLassoPoints[i].z + perp.y + perp.y));
					}
					for (int i = 0; i< symPointTmp.Count; i++) {
						polygonLassoPoints.Add(symPointTmp[i]);
					}
					if (IsShapeLastAndFirstPointsMatch(polygonLassoPoints)) polygonLassoPoints.RemoveAt(polygonLassoPoints.Count-1);
				}
			}
		}
		
		//symmetry last shape about selected line
		if ( ((e.type == EventType.MouseDown)  && e.button == 1) && (!e.alt) && (!(e.control && e.shift)) ) {
			if (polygonLassoPoints != null) {
				if (polygonLassoPoints.Count == 1) {

					if (Vector2.Distance(new Vector2(polygonLassoPoints[0].x, polygonLassoPoints[0].z), new Vector2(snappedPoint.x, snappedPoint.z)) > 0.1f) {
						Surforge.LogAction("Set symmetry axes", "Left Click, move, Right Click", "");

						Surforge.surforgeSettings.mirrorLineSolid = new List<Vector3>();
						Surforge.surforgeSettings.mirrorLineSolid.Add(polygonLassoPoints[0]);
						Vector3 blueAxisDirection = (new Vector3(snappedPoint.x, polygonLassoPoints[0].y, snappedPoint.z)  - polygonLassoPoints[0]).normalized;
						Surforge.surforgeSettings.mirrorLineSolid.Add(polygonLassoPoints[0] + blueAxisDirection);

						Surforge.surforgeSettings.mirrorLineDotted = new List<Vector3>();
						Surforge.surforgeSettings.mirrorLineDotted.Add(polygonLassoPoints[0]);
						Vector3 redAxisDirection = (new Vector3(snappedPoint.x, polygonLassoPoints[0].y, snappedPoint.z) - polygonLassoPoints[0]).normalized;
						redAxisDirection = new Vector3(redAxisDirection.z, redAxisDirection.y, -redAxisDirection.x);
						Surforge.surforgeSettings.mirrorLineDotted.Add(polygonLassoPoints[0] + redAxisDirection);

						Surforge.surforgeSettings.symmetryPoint = polygonLassoPoints[0];
						SetPlaceToolSymmetryParent();

						polygonLassoPoints.Clear();
						GetWindow<SurforgeInterface>().Repaint();
					}
				}
				else {
					if (polygonLassoPoints.Count == 0) {
						if (!e.shift) {
							if (Surforge.surforgeSettings.mirrorLineSolid != null) {
								if (Surforge.surforgeSettings.mirrorLineSolid.Count == 2) {
									if (lastPolygonLassoPoints != null) {
										if (e.control) {
											Surforge.LogAction("Mirror last shape action (dotted line)", "Ctrl + Right Click", "");
											SymmetryLastShapeAboutSelectedLine(Surforge.surforgeSettings.mirrorLineDotted);
										}
										else {
											Surforge.LogAction("Mirror last shape action (solid line)", "Right Click", "");
											SymmetryLastShapeAboutSelectedLine(Surforge.surforgeSettings.mirrorLineSolid);
										}
									}
								}
							}
						}
						else {
							SetPlaceToolSymmetry();
							SetMirrorPlanesToPlaceToolSymmetryPoint();   
							SetPlaceToolSymmetryParent();
						}
					}
				}
				
			}
		}
		
	}


	static float GetOffsetForContinueShape(List<Vector3> shape) {
		 
		float shapePointMinX = Mathf.Infinity;
		for (int i=0; i < shape.Count; i++) {
			if (shape[i].x < shapePointMinX) shapePointMinX = shape[i].x;
		}
		
		float shapePointMaxX = Mathf.NegativeInfinity;
		for (int i=0; i < shape.Count; i++) {
			if (shape[i].x > shapePointMaxX) shapePointMaxX = shape[i].x;
		}


		return shapePointMaxX - shapePointMinX;
	}


	static void SetPlaceToolSymmetryParent() {
		if (Surforge.surforgeSettings.symmetryParent) {
			Vector3 symmetryAxisDirection =  Surforge.surforgeSettings.mirrorLineSolid[1] - Surforge.surforgeSettings.mirrorLineSolid[0];		
			Surforge.surforgeSettings.symmetryParent.transform.position = Surforge.surforgeSettings.symmetryPoint; 
			Surforge.surforgeSettings.symmetryParent.transform.rotation = Quaternion.LookRotation(symmetryAxisDirection, Vector3.up);
		}
	}

	
	static void SymmetryLastShapeAboutSelectedLine(List<Vector3> symLine) {
		symPointTmp = new List<Vector3>();
		
		for (int i=0; i < lastPolygonLassoPoints.Count; i++) {
			
			Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(symLine[0].x, symLine[0].z), new Vector2(symLine[1].x, symLine[1].z),  new Vector2(lastPolygonLassoPoints[i].x, lastPolygonLassoPoints[i].z) );
			Vector2 perp = new Vector2(lastPolygonLassoPoints[i].x - perpPoint.x, lastPolygonLassoPoints[i].z - perpPoint.y);
			perp = perp * -1.0f;
			symPointTmp.Add(new Vector3( lastPolygonLassoPoints[i].x + perp.x + perp.x, lastPolygonLassoPoints[i].y, lastPolygonLassoPoints[i].z + perp.y + perp.y));
		}
		
		polygonLassoPoints = new List<Vector3>();
		
		for (int i = 0; i< symPointTmp.Count; i++) {
			polygonLassoPoints.Add(symPointTmp[i]);
		}
		if (lastPolyLassoWasSplit) PolygonLassoSplit();
		else {
			
			GameObject[] gameObjects = Selection.gameObjects;
			
			PolygonLassoBuildObject();
			
			Object[] newSelection = new Object[gameObjects.Length + 1];
			for (int i=0; i<gameObjects.Length; i++) {
				newSelection[i] = gameObjects[i];
			}
			newSelection[newSelection.Length-1] = Selection.gameObjects[0];
			Selection.objects = newSelection;
			
		}
	}
	
	
	static bool IsShapeLastAndFirstPointsMatch(List<Vector3> points) {
		if (Vector3Equal(points[0], points[points.Count-1])) return true;
		else return false;
	}
	
	
	static Color selfLinesSnapGuidesColor = new Color(1.5f, 1.5f, 0.9f, 4);
	
	static float dottedSnapGuidesLineSpacing = 10.0f;
	
	static void ClearCrossHelperLines() {
		snapGuideToDrawSelfLinesX = new Vector3[0];
		snapGuideToDrawSelfLinesZ = new Vector3[0];
		snapGuideToDrawSelfLinesCrossX = new Vector3[0];
		snapGuideToDrawSelfLinesCrossZ = new Vector3[0];
		snapGuideToDrawSelfLinesCrossHelpers = new Vector3[0];
	}


	static Color snapMarkerObjectColor = new Color(0, 2, 2, 4);  //blue
	static Color snapMarkerSelfColor = new Color(1.5f, 1.5f, 0.9f, 4); //white
	static Color snapMarkerHelpersColor = Color.yellow;

	static void DrawPolygonLasso() {
		
		//draw snap guides
		Handles.color = selfLinesSnapGuidesColor;
		
		if ( snapGuideToDrawSelfLinesCrossHelpers.Length != 0)  {
			if (snapGuideToDrawSelfLinesCrossHelpers.Length != 0) Handles.DrawDottedLine(snapGuideToDrawSelfLinesCrossHelpers[0], snapGuideToDrawSelfLinesCrossHelpers[1], dottedSnapGuidesLineSpacing);
		}
		else {
			if ( (snapGuideToDrawSelfLinesCrossX.Length != 0) || (snapGuideToDrawSelfLinesCrossZ.Length != 0) ) {
				if (snapGuideToDrawSelfLinesCrossX.Length != 0) Handles.DrawDottedLine(snapGuideToDrawSelfLinesCrossX[0], snapGuideToDrawSelfLinesCrossX[1], dottedSnapGuidesLineSpacing);
				if (snapGuideToDrawSelfLinesCrossZ.Length != 0) Handles.DrawDottedLine(snapGuideToDrawSelfLinesCrossZ[0], snapGuideToDrawSelfLinesCrossZ[1], dottedSnapGuidesLineSpacing);
			}
			else {
				if (snapGuideToDrawSelfLinesX.Length != 0) Handles.DrawDottedLine(snapGuideToDrawSelfLinesX[0], snapGuideToDrawSelfLinesX[1], dottedSnapGuidesLineSpacing);
				if (snapGuideToDrawSelfLinesZ.Length != 0) Handles.DrawDottedLine(snapGuideToDrawSelfLinesZ[0], snapGuideToDrawSelfLinesZ[1], dottedSnapGuidesLineSpacing);
			}
		}
		
		Handles.color = polygonLassoCurrentPointColor;
		Handles.CubeCap(0, snappedPoint, Quaternion.identity, HandleUtility.GetHandleSize(snappedPoint) * 0.06f);

		//snap circle
		if (snapState == 1) {
			Handles.color = snapMarkerObjectColor;
			Handles.CircleCap(0, snappedPoint, Quaternion.Euler(90.0f, 0, 0), HandleUtility.GetHandleSize(snappedPoint) * 0.1f);
		}
		if (snapState == 2) {
			Handles.color = snapMarkerObjectColor;
			DrawSnapRectangle(snappedPoint);
		}
		if (snapState == 3) {
			Handles.color = snapMarkerSelfColor;
			Handles.CircleCap(0, snappedPoint, Quaternion.Euler(90.0f, 0, 0), HandleUtility.GetHandleSize(snappedPoint) * 0.1f);
		}
		if (snapState == 4) {
			Handles.color = snapMarkerSelfColor;
			DrawSnapRectangle(snappedPoint);
		}
		if (snapState == 5) {
			Handles.color = snapMarkerHelpersColor;
			Handles.CircleCap(0, snappedPoint, Quaternion.Euler(90.0f, 0, 0), HandleUtility.GetHandleSize(snappedPoint) * 0.1f);
		}
		if (snapState == 6) {
			Handles.color = snapMarkerHelpersColor;
			DrawSnapRectangle(snappedPoint);
		}
		
		if (polygonLassoPoints != null) {
			
			Vector3[] points = polygonLassoPoints.ToArray();
			
			for (int i=0; i < polygonLassoPoints.Count; i++) {
				Handles.color = polygonLassoPointColor;
				
				Handles.CubeCap(0, polygonLassoPoints[i], Quaternion.identity, HandleUtility.GetHandleSize(polygonLassoPoints[i]) * 0.06f);
			}
			
			Handles.color = polygonLassoLineColor;
			Handles.DrawPolyLine(points);

			bool drawFinalSegment = true;
			if (polygonLassoChildPoints != null) {
				if (polygonLassoChildPoints.Count > 0) {
					drawFinalSegment = false;
				}
			}
			if (drawFinalSegment) {
				if (points.Length > 0) Handles.DrawPolyLine(new Vector3[2] { points[points.Length-1], snappedPoint} );
			}

			//draw polyLassoToolScale
			Handles.color = polygonLassoProfileSizeColor;
			Handles.CircleCap(0, snappedPoint, Quaternion.Euler(90.0f, 0, 0), HandleUtility.GetHandleSize(snappedPoint) * polyLassoScale * 0.1f);
		}
	}


	static Color warpShapeColor = new Color(1, 0.5f, 1, 0.5f);
	static Color warpShapeCenterLinePointColor = new Color(1, 0.5f, 1, 0.25f);

	static void DrawPolygonLassoWarpShape() {
		if (Surforge.surforgeSettings) {
			if (Surforge.surforgeSettings.warpShape != null) {
				if (Surforge.surforgeSettings.warpShape.Count > 0) {
					Vector3[] points = Surforge.surforgeSettings.warpShape.ToArray();
					Handles.color = warpShapeColor;
					Handles.DrawPolyLine(points);
					Handles.DrawPolyLine(new Vector3[] {points[0], points[points.Length-1]});

					Handles.color = warpShapeCenterLinePointColor;
					Handles.DrawPolyLine(new Vector3[] {points[0], Surforge.surforgeSettings.warpShapeCenterLinePoint});
					Handles.DrawPolyLine(new Vector3[] {points[0], points[0] + (Surforge.surforgeSettings.warpShapeCenterLinePoint - points[0]) * -1.0f});

					Handles.color = warpShapeColor;
					Handles.CubeCap(0, points[0], Quaternion.identity, HandleUtility.GetHandleSize(points[0]) * 0.06f);
				}
			}
		}
	}


	static Color polyLassoArcCircleColor = new Color(1, 1, 1, 0.2f);
	static Color polyLassoArcCircleCenterColor = new Color(1, 1, 1, 0.5f);

	static void DrawPolygonLassoChildPoints() {
		if (polygonLassoChildPoints != null) {
			
			Vector3[] points = polygonLassoChildPoints.ToArray();
			
			for (int i=0; i < polygonLassoChildPoints.Count; i++) {
				Handles.color = polygonLassoPointColor;
				
				Handles.CubeCap(0, polygonLassoChildPoints[i], Quaternion.identity, HandleUtility.GetHandleSize(polygonLassoChildPoints[i]) * 0.06f);
			}
			
			Handles.color = polygonLassoLineColor * 0.5f;
			Handles.DrawPolyLine(points);

			if (polygonLassoPoints.Count > 0) {
				if (polygonLassoChildPoints.Count > 0)  {
					Handles.color = polyLassoArcCircleColor;
					Vector3 circleCenter = new Vector3(polyLassoArcCircleCenter.x, polygonLassoPoints[0].y, polyLassoArcCircleCenter.y);
					Handles.DrawWireDisc(circleCenter, Vector3.up, polyLassoArcCircleRadius);

					Handles.color = polyLassoArcCircleCenterColor;
					Handles.CubeCap(0, circleCenter, Quaternion.identity, HandleUtility.GetHandleSize(circleCenter) * 0.06f);
				}

			}
		}
	}


	static void DrawPolygonLassoMirrorPoints() {
		List<Vector3> polygonLassoPointsSymmDraw = new List<Vector3>();
		for (int i=0; i < polygonLassoPoints.Count; i++) {
			polygonLassoPointsSymmDraw.Add(polygonLassoPoints[i]);
		}
		if (polygonLassoChildPoints != null) {
			if (polygonLassoChildPoints.Count > 0) {
				for (int i=1; i < polygonLassoChildPoints.Count-1; i++) {
					polygonLassoPointsSymmDraw.Add(polygonLassoChildPoints[i]);
				}
			}
		}

		  
		if(Surforge.surforgeSettings.symmetryX) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPointsSymmDraw);
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineSolid, snappedPoint);
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}
		if(Surforge.surforgeSettings.symmetryZ) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, polygonLassoPointsSymmDraw);
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineDotted, snappedPoint);
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}
		if ( (Surforge.surforgeSettings.symmetryX) && (Surforge.surforgeSettings.symmetryZ) ) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPointsSymmDraw));
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineDotted, GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineSolid, snappedPoint));
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}

		//diagonal
		if (Surforge.surforgeSettings.symmetryDiagonal) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineSolid, new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f)));

			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, polygonLassoPointsSymmDraw);
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(diagonal, snappedPoint);
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}

		//diagonalX
		if ((Surforge.surforgeSettings.symmetryX) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPointsSymmDraw));
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(diagonal, GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineSolid, snappedPoint));
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}

		//diagonalZ
		if ((Surforge.surforgeSettings.symmetryZ) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, polygonLassoPointsSymmDraw));
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(diagonal, GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineDotted, snappedPoint));
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}

		//diagonal XZ
		if ( (Surforge.surforgeSettings.symmetryX) && (Surforge.surforgeSettings.symmetryZ) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, polygonLassoPointsSymmDraw);
			Vector3 mirrorActivePoint = GetPolyLassoPointSymmLine(diagonal, snappedPoint);
			symmPolyLine.Add(mirrorActivePoint);
			Vector3[] mirrorPoints = symmPolyLine.ToArray();   
			DrawPolygonLassoMirrorPointsHandles(mirrorPoints, mirrorActivePoint);
		}  



		
	}

	static Color polygonLassoMirrorPointColor = new Color(0.7f, 0.8f, 1.5f, 2);

	static void DrawPolygonLassoMirrorPointsHandles(Vector3[] line, Vector3 activePoint) {
		Handles.color = polygonLassoMirrorPointColor;
		Handles.DrawPolyLine(line);
		Handles.color = polygonLassoMirrorPointColor;
		for (int i=0; i < line.Length; i++) {
			Handles.CubeCap(0, line[i], Quaternion.identity, HandleUtility.GetHandleSize(line[i]) * 0.06f);
		}
		Handles.CubeCap(0, activePoint, Quaternion.identity, HandleUtility.GetHandleSize(activePoint) * 0.06f);
	}

	static List<Vector3> GetPolyLassoPointsSymmLine(List<Vector3> symLine, List<Vector3> sourcePoints) {  
		List<Vector3> result = new List<Vector3>();
			
		for (int i=0; i < sourcePoints.Count; i++) {
				
			Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(symLine[0].x, symLine[0].z), new Vector2(symLine[1].x, symLine[1].z),  new Vector2(sourcePoints[i].x, sourcePoints[i].z) );
			Vector2 perp = new Vector2(sourcePoints[i].x - perpPoint.x, sourcePoints[i].z - perpPoint.y);
			perp = perp * -1.0f;
			result.Add(new Vector3( sourcePoints[i].x + perp.x + perp.x, sourcePoints[i].y, sourcePoints[i].z + perp.y + perp.y));
		}

		return result;  
	}

	static Vector3 GetPolyLassoPointSymmLine(List<Vector3> symLine, Vector3 sourcePoint) {  
		Vector3 result = new Vector3();

		Vector2 perpPoint = Surforge.PerpendicularPointToSegment( new Vector2(symLine[0].x, symLine[0].z), new Vector2(symLine[1].x, symLine[1].z),  new Vector2(sourcePoint.x, sourcePoint.z) );
		Vector2 perp = new Vector2(sourcePoint.x - perpPoint.x, sourcePoint.z - perpPoint.y);
		perp = perp * -1.0f;
		result = new Vector3( sourcePoint.x + perp.x + perp.x, sourcePoint.y, sourcePoint.z + perp.y + perp.y);

		return result;  
	}


	static void DrawSnapRectangle(Vector3 snappedPoint) {
		float offset = HandleUtility.GetHandleSize(snappedPoint) * 0.06f;
		Handles.DrawPolyLine(new Vector3[5] { new Vector3(snappedPoint.x + offset, snappedPoint.y, snappedPoint.z + offset),
			new Vector3(snappedPoint.x + offset, snappedPoint.y, snappedPoint.z - offset),
			new Vector3(snappedPoint.x - offset, snappedPoint.y, snappedPoint.z - offset),
			new Vector3(snappedPoint.x - offset, snappedPoint.y, snappedPoint.z + offset),
			new Vector3(snappedPoint.x + offset, snappedPoint.y, snappedPoint.z + offset)} );
	}
	

	
	static Color polygonLassoSeamlessPointColor = new Color(1, 1, 1, 0.5f);
	static void DrawPolygonLassoSeamlessPoints() {
		Handles.color = polygonLassoSeamlessPointColor;
		
		Vector3[] pointsSeamlessX = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessMX = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessZ = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessMZ = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessXZ = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessMXZ = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessXMZ = new Vector3[polygonLassoPoints.Count];
		Vector3[] pointsSeamlessMXMZ = new Vector3[polygonLassoPoints.Count];
		
		float offsetZ = Mathf.Abs(Surforge.surforgeSettings.textureBorders.minZ - Surforge.surforgeSettings.textureBorders.maxZ) * rootScaleZ;
		float offsetX = Mathf.Abs(Surforge.surforgeSettings.textureBorders.minX - Surforge.surforgeSettings.textureBorders.maxX) * rootScaleX;
		
		for (int i=0; i < polygonLassoPoints.Count; i++) {
			Vector3 X = new Vector3(polygonLassoPoints[i].x + offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z); 
			Vector3 MX = new Vector3(polygonLassoPoints[i].x - offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z); 
			Vector3 Z = new Vector3(polygonLassoPoints[i].x, polygonLassoPoints[i].y, polygonLassoPoints[i].z + offsetZ); 
			Vector3 MZ = new Vector3(polygonLassoPoints[i].x, polygonLassoPoints[i].y, polygonLassoPoints[i].z - offsetZ); 
			Vector3 XZ = new Vector3(polygonLassoPoints[i].x + offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z + offsetZ); 
			Vector3 MXZ = new Vector3(polygonLassoPoints[i].x - offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z + offsetZ); 
			Vector3 XMZ = new Vector3(polygonLassoPoints[i].x + offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z - offsetZ); 
			Vector3 MXMZ = new Vector3(polygonLassoPoints[i].x - offsetX, polygonLassoPoints[i].y, polygonLassoPoints[i].z - offsetZ); 
			
			pointsSeamlessX[i] = X;
			pointsSeamlessMX[i] = MX;
			pointsSeamlessZ[i] = Z;
			pointsSeamlessMZ[i] = MZ;
			pointsSeamlessXZ[i] = XZ;
			pointsSeamlessMXZ[i] = MXZ;
			pointsSeamlessXMZ[i] = XMZ;
			pointsSeamlessMXMZ[i] = MXMZ;
		}
		
		Handles.DrawPolyLine(pointsSeamlessX);
		Handles.DrawPolyLine(pointsSeamlessMX);
		Handles.DrawPolyLine(pointsSeamlessZ);
		Handles.DrawPolyLine(pointsSeamlessMZ);
		Handles.DrawPolyLine(pointsSeamlessXZ);
		Handles.DrawPolyLine(pointsSeamlessMXZ);
		Handles.DrawPolyLine(pointsSeamlessXMZ);
		Handles.DrawPolyLine(pointsSeamlessMXMZ);
	}
	
	
	static void AddPolygonLassoPoint() {
		if (polygonLassoPoints != null) {
			Vector3 polygonLassoPoint = new Vector3(snappedPoint.x, snappedPoint.y, snappedPoint.z);
			
			bool addPointAllowed = true;
			bool constructionFinished = false;
			bool addLastArcPoints = false;

			//UV island fill doubleckick timer
			if (polygonLassoPoints != null) {
				if (Surforge.surforgeSettings) {
					if (polygonLassoPoints.Count == 0) {
						Surforge.surforgeSettings.doubleclickTimer = Time.realtimeSinceStartup + 0.5f; 
					}
					else {
						if (polygonLassoPoints.Count == 1) {
							if ((Surforge.surforgeSettings.doubleclickTimer - Time.realtimeSinceStartup) > 0) {
								if (Vector3.Distance(polygonLassoPoint, polygonLassoPoints[0]) < 1.0f) {
									constructionFinished = true;  
								}
							}
						}
					}
				}
			}
			
			for (int i=0; i< polygonLassoPoints.Count; i++) {
				if (polygonLassoPoints[i] == polygonLassoPoint) {
					addPointAllowed = false;
					if ((i == 0) || (i == (polygonLassoPoints.Count - 1))) constructionFinished = true; 
					if ((i == 0) && (polygonLassoChildPoints.Count != 0) && (polygonLassoPoints.Count > 1)) addLastArcPoints = true;
					break;
				}
			}
			
			if (addPointAllowed) {
				if (polygonLassoChildPoints.Count == 0) {
					polygonLassoPoints.Add(polygonLassoPoint);
				}
				else {
					for (int s=1; s< polygonLassoChildPoints.Count; s++) {
						polygonLassoPoints.Add(polygonLassoChildPoints[s]);
					}
				}
			}
			if (addLastArcPoints) {
				for (int s=1; s< polygonLassoChildPoints.Count-1; s++) {
					polygonLassoPoints.Add(polygonLassoChildPoints[s]);
				}
			}


			if (constructionFinished) {
				if (!Event.current.control) {
					Surforge.LogAction("Finish shape", "Double Click", "Enter");

					if ( Surforge.surforgeSettings.symmetry && (Surforge.surforgeSettings.symmetryX ||Surforge.surforgeSettings.symmetryZ || Surforge.surforgeSettings.symmetryDiagonal) ) {
						PolygonLassoBuildObjectsWithSymmetry();
					}
					else PolygonLassoBuildObject();
				}
				else {
					polygonLassoPoints.Add(polygonLassoPoint);
					if ( Surforge.surforgeSettings.symmetry && (Surforge.surforgeSettings.symmetryX ||Surforge.surforgeSettings.symmetryZ || Surforge.surforgeSettings.symmetryDiagonal) ) {
						PolygonLassoSplitWithSymmetry();
					}
					else PolygonLassoSplit();
				}
			}
			
		}
	}
	
	
	static List<Vector3> lastPolygonLassoPoints;
	static bool lastPolyLassoWasSplit;
	
	static void StoreLastPolygonLassoPoints() {
		lastPolygonLassoPoints = new List<Vector3>();
		
		for (int i=0; i< polygonLassoPoints.Count; i++) {
			lastPolygonLassoPoints.Add(polygonLassoPoints[i]);
		}
	}

	static List<Vector3P>[] GetSortedBorderVectorPairs(int[] ids) {
		List<Vector3P>[] idSortedVectorPairs = new List<Vector3P>[ids.Length];
		for (int i=0; i < idSortedVectorPairs.Length; i++) {
			List<Vector3P> listV3P = new List<Vector3P>();
			idSortedVectorPairs[i] = listV3P;
		}
		
		for (int i=0; i < uvVectorPairsBorderIDs.Count; i++) {
			for (int d=0; d < ids.Length; d++) {
				if (uvVectorPairsBorderIDs[i] == ids[d]) {
					idSortedVectorPairs[d].Add (uvVectorPairsBorder[i]);
					break;
				}
			}
		}
		return idSortedVectorPairs;
	}

	static List<Vector3> GetUvBorderPointsAroundPoint(Vector3 point) {
		List<Vector3> result = new List<Vector3>();

		int[] ids = GetBorderIDsList().ToArray();

		List<Vector3P>[] idSortedVectorPairs = GetSortedBorderVectorPairs(ids);
	
		bool shapeAroundPointFound = false;
		for (int i=0; i < idSortedVectorPairs.Length; i++) {
			result = GetBorderShape(idSortedVectorPairs[i], true);
			Vector2[] result2d = Surforge.Points3DTo2D(result.ToArray());
			if (Surforge.IsPointInPolygon(result2d.Length, result2d, point.x, point.z, true)) {
				shapeAroundPointFound = true;
				break;
			}
		}

		if (shapeAroundPointFound) return result;
		else return new List<Vector3>();
	}

	static List<Vector3> GetUvIslandBorderShape(UvIsland uvIsland, bool applyRootScale) {
		List<Vector3> result = new List<Vector3>();

		return result;
	}

	static List<Vector3> GetBorderShape(List<Vector3P> borderPairs, bool applyRootScale) {
		List<Vector3> sortedPoints = new List<Vector3>();
		sortedPoints.Add (borderPairs[0].a);

		for( int j=0; j< borderPairs.Count; j++) {

			for (int i=0; i< borderPairs.Count; i++) {
				if ( (borderPairs[i].a == sortedPoints[sortedPoints.Count-1]) ) {
					bool match = false;
					for (int d=0; d < sortedPoints.Count; d++) {
						if (borderPairs[i].b == sortedPoints[d]) {
							match = true;
							break;
						}
					}
					if (!match) sortedPoints.Add (borderPairs[i].b);
				
				}
				else {
					if ( (borderPairs[i].b == sortedPoints[sortedPoints.Count-1]) ) {
						bool match = false;
						for (int d=0; d < sortedPoints.Count; d++) {
							if (borderPairs[i].a == sortedPoints[d]) {
								match = true;
								break;
							}
						}
						if (!match) sortedPoints.Add (borderPairs[i].a);
					}
				}
			}

		}

		for (int j=0; j < sortedPoints.Count; j++) {
			if (applyRootScale) {
				sortedPoints[j] = new Vector3(sortedPoints[j].x * rootScaleX, 
				                              sortedPoints[j].y,
				                              sortedPoints[j].z * rootScaleZ);
			}
			else {
				sortedPoints[j] = new Vector3(sortedPoints[j].x, sortedPoints[j].y, sortedPoints[j].z);
			}
		}

		return sortedPoints;
	}


	static List<List<Vector3>>  GetPolygonLassoSymmetryShapes() {
		List<List<Vector3>> polyLassoSymShapes = new List<List<Vector3>>();
		polyLassoSymShapes.Add (polygonLassoPoints);
		
		if(Surforge.surforgeSettings.symmetryX) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPoints);
			polyLassoSymShapes.Add (symmPolyLine);
		}
		if(Surforge.surforgeSettings.symmetryZ) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, polygonLassoPoints);
			polyLassoSymShapes.Add (symmPolyLine);
		}
		if ( (Surforge.surforgeSettings.symmetryX) && (Surforge.surforgeSettings.symmetryZ) ) {
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPoints));
			polyLassoSymShapes.Add (symmPolyLine);
		}
		
		//diagonal
		if (Surforge.surforgeSettings.symmetryDiagonal) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (GetPolyLassoPointSymmLine(Surforge.surforgeSettings.mirrorLineSolid, new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                                                                                           Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                                                                                           (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f)));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, polygonLassoPoints);
			polyLassoSymShapes.Add (symmPolyLine);
		}
		
		//diagonalX
		if ((Surforge.surforgeSettings.symmetryX) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineSolid, polygonLassoPoints));
			polyLassoSymShapes.Add (symmPolyLine);
		}
		
		//diagonalZ
		if ((Surforge.surforgeSettings.symmetryZ) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, GetPolyLassoPointsSymmLine(Surforge.surforgeSettings.mirrorLineDotted, polygonLassoPoints));
			polyLassoSymShapes.Add (symmPolyLine);
		}
		
		//diagonal XZ
		if ( (Surforge.surforgeSettings.symmetryX) && (Surforge.surforgeSettings.symmetryZ) &&(Surforge.surforgeSettings.symmetryDiagonal)) {  
			List<Vector3> diagonal = new List<Vector3>();
			diagonal.Add(Surforge.surforgeSettings.mirrorLineSolid[0]);
			diagonal.Add (new Vector3((Surforge.surforgeSettings.mirrorLineSolid[1].x + Surforge.surforgeSettings.mirrorLineDotted[1].x) * 0.5f,
			                          Surforge.surforgeSettings.mirrorLineSolid[1].y,
			                          (Surforge.surforgeSettings.mirrorLineSolid[1].z + Surforge.surforgeSettings.mirrorLineDotted[1].z) * 0.5f));
			
			List<Vector3> symmPolyLine = GetPolyLassoPointsSymmLine(diagonal, polygonLassoPoints);
			polyLassoSymShapes.Add (symmPolyLine);
		}
		
		List<List<Vector3>> result = MergeTwoShapesFromList(polyLassoSymShapes);
		
		//merge shapes
		for (int i=0; i < polyLassoSymShapes.Count-1; i++) {
			result = MergeTwoShapesFromList(result);
		}

		return result;
	}


	static List<List<Vector3>> MergeTwoShapesFromList (List<List<Vector3>> shapesToMerge) {
		List<List<Vector3>> result = new List<List<Vector3>>();
		
		bool matchFound = false;
		int matchIndexA = 0;
		int matchIndexB = 0;
		
		for (int a = 0; a < shapesToMerge.Count; a++) {
			for (int b = 0; b < shapesToMerge.Count; b++) {
				if (a != b) {
					if (CheckShapesCanBeMerged(shapesToMerge[a], shapesToMerge[b])) {
						matchFound = true; 
						matchIndexA = a;
						matchIndexB = b;
						break;
					}
				}
			}
			if (matchFound) break;
		}
		
		if (!matchFound) {
			for (int i=0; i< shapesToMerge.Count; i++) {
				result.Add(shapesToMerge[i]);
			}
		}
		
		else {
			result.Add(MergeTwoShapes(shapesToMerge[matchIndexA], shapesToMerge[matchIndexB]));
			for (int i=0; i< shapesToMerge.Count; i++) {
				if ((i != matchIndexA) && (i != matchIndexB)) {
					result.Add(shapesToMerge[i]);
				}
			}
		}
		
		return result;
	}
	
	
	static bool CheckShapesCanBeMerged(List<Vector3> shapeA, List<Vector3> shapeB) {
		bool result = false;
		
		if (Vector3Equal(shapeA[0], shapeB[0]) || 
		    Vector3Equal(shapeA[0], shapeB[shapeB.Count-1]) ||
		    Vector3Equal(shapeA[shapeA.Count-1], shapeB[0]) ||
		    Vector3Equal(shapeA[shapeA.Count-1], shapeB[shapeB.Count-1])) {
			
			result = true;
		}
		
		for (int a=0; a < shapeA.Count; a++) {
			for (int b=0; b < shapeB.Count; b++) {
				if (!( ((a == 0) && ( b == 0)) || 
				      ((a == 0) && ( b == shapeB.Count-1)) ||
				      ((a == shapeA.Count-1) && (b == 0)) ||
				      ((a == shapeA.Count-1) && (b == shapeB.Count-1)) ) ) {
					
					if (Vector3Equal(shapeA[a], shapeB[b])) {
						result = false;
						break;
					}
				}
			}
			if (!result) break;
		}
		
		return result;
	}
	
	
	static List<Vector3> MergeTwoShapes(List<Vector3> shapeA, List<Vector3> shapeB) {
		List<Vector3> result = new List<Vector3>();
		
		if (Vector3Equal(shapeA[0], shapeB[shapeB.Count-1])) {
			for (int i=0; i< shapeB.Count; i++) {
				result.Add(shapeB[i]);
			}
			for (int i=1; i< shapeA.Count; i++) {
				result.Add(shapeA[i]);
			}
		}
		else {
			if (Vector3Equal(shapeA[shapeA.Count-1], shapeB[0])) {
				for (int i=0; i < shapeA.Count; i++) {
					result.Add(shapeA[i]);
				}
				for (int i=1; i < shapeB.Count; i++) {
					result.Add(shapeB[i]);
				}
			}
			else {
				if (Vector3Equal(shapeA[0], shapeB[0])) {
					shapeA.Reverse();
					for (int i=0; i < shapeA.Count; i++) {
						result.Add(shapeA[i]);
					}
					for (int i=1; i < shapeB.Count; i++) {
						result.Add(shapeB[i]);
					}
				}
				else {
					shapeB.Reverse();
					for (int i=0; i < shapeA.Count; i++) {
						result.Add(shapeA[i]);
					}
					for (int i=1; i < shapeB.Count; i++) {
						result.Add(shapeB[i]);
					}
				}
			}
		}
		
		if (Vector3Equal(result[0], result[result.Count-1])) {
			result.RemoveAt(result.Count-1);
		}
		
		return result;
	}




	static void PolygonLassoSplitWithSymmetry() {  
		Surforge.LogAction("Split", "Ctrl + Left Click", "");

		List<List<Vector3>> shapesToSplitWith = GetPolygonLassoSymmetryShapes();

		for (int i=0; i< shapesToSplitWith.Count; i++) {

			Vector3[] pointsToCheck = shapesToSplitWith[i].ToArray();
		
			StoreLastPolygonLassoPoints();
			lastPolyLassoWasSplit = true;
		
			if (Surforge.CheckIfShapeClockwise(pointsToCheck)) shapesToSplitWith[i].Reverse();
		
			Surforge.SplitSelectedPolyLassoObjects(shapesToSplitWith[i]);
		}
		polygonLassoPoints.Clear();

		Object[] newSelection = new Object[Selection.objects.Length];
		for (int i=0; i< Selection.objects.Length; i++) {
			newSelection[i] = Selection.objects[i];
		}
		Selection.objects = newSelection;
	}


	static void PolygonLassoBuildObjectsWithSymmetry() {

		List<List<Vector3>> shapesToBuild = GetPolygonLassoSymmetryShapes();

		//build shapes
		List<GameObject> newObjs = new List<GameObject>();

		bool uvIslandFilled = false;

		for (int i=0; i< shapesToBuild.Count; i++) {
			if (uvIslandFilled) break;

			Vector3[] pointsToCheck = shapesToBuild[i].ToArray();
			
			if (pointsToCheck.Length < 3) {
				if (pointsToCheck.Length > 0) {
					Surforge.LogAction("Fill UV island", "Double Click", "");

					shapesToBuild[i] = GetUvBorderPointsAroundPoint(pointsToCheck[0]);
					SetPolyLassoShapeHeight(shapesToBuild[i], -0.25f);
					pointsToCheck = shapesToBuild[i].ToArray();
					uvIslandFilled = true;
					if (shapesToBuild[i].Count < 3) return;
				}
			}
			
			
			StoreLastPolygonLassoPoints();
			lastPolyLassoWasSplit = false;
			
			if (!Surforge.CheckIfShapeClockwise(pointsToCheck)) shapesToBuild[i].Reverse();
			
			int selectedProfile;
			if (Surforge.surforgeSettings.activePolyLassoProfile < 0) selectedProfile = 0;
			else selectedProfile = Surforge.surforgeSettings.activePolyLassoProfile;
			PolyLassoProfile profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[selectedProfile];


			if (!PolyLassoDensityDialog(shapesToBuild[i], profile, polyLassoScale)) {
				profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[0];
			}


			GameObject newObj = Surforge.PolygonLassoBuildObject(null, false, shapesToBuild[i], profile.bevelAmount, profile.bevelSteps, profile.offsets, profile.heights,
			                                                   new List<DecalSet>(), 
			                                                   new List<bool>(),
			                                                   new List<bool>(), 
			                                                   new List<bool>(), 
			                                                   new List<bool>(), 
			                                                   new List<bool>(), 
			                                                   new List<float>(), 
			                                                   new List<float>(),
			                                                   new List<float>(), 
			                                                   new List<float>(), 
			                                                   new List<float>(),
			                                                   new List<float>(), 
			                                                   new List<float>(), 
			                                                   new List<float>(), 
			                                                   new List<float>(),
			                                                   false,
			                                                   1.0f,
			                                                   new Vector2(1,1),
			                                                   0.5f,
			                                                   new Vector2(1,1),
			                                                   0.5f,
			                                                   0,
			                                                   profile.isFloater,
			                                                   profile.isTube,
			                                                   profile.isOpen,
			                                                   profile.thickness,
			                                                   profile.section,
			                                                   profile.isAdaptive,
			                                                   profile.adaptiveStep,
			                                                   profile.lengthOffsets0,
			                                                   profile.lengthOffsets1,
			                                                   profile.lengthOffsets2,
			                                                   profile.heightOffsets0,
			                                                   profile.heightOffsets1,
			                                                   profile.heightOffsets2,
			                                                   profile.repeatSize,
			                                                   profile.lengthOffsetOrder,
			                                                   profile.heightOffsetOrder,
			                                                   profile.edgeWiseOffset,
			                                                   profile.lengthWiseOffset,
			                                                   profile.offsetMinEdge,
			                                                   profile.corner,
			                                                   profile.childProfileVerticalOffsets, 
			                                                   profile.childProfileDepthOffsets,
			                                                   profile.childProfileHorisontalOffsets,
			                                                   profile.childProfileMatGroups,
			                                                   profile.childProfileShapes,
			                                                   profile.followerProfiles, 
			                                                   profile.followerProfileVerticalOffsets, 
			                                                   profile.followerProfileDepthOffsets,
			                                                   profile.followerProfileMatGroups,
			                                                   profile.cutoff, 
			                                                   profile.cutoffTiling,
			                                                   profile.bumpMap, 
			                                                   profile.bumpMapIntensity, 
			                                                   profile.bumpMapTiling,
			                                                   profile.aoMap,
			                                                   profile.aoMapIntensity,
			                                                   profile.randomUvOffset,
			                                                   profile.stoneType,
			                                                   profile.allowIntersections,
			                                                   profile.overlapIntersections,
			                                                   profile.overlapAmount,
			                                                   profile.usedForOverlapping, 
			                                                   profile.overlapStartInvert, 
			                                                   profile.curveUVs);

			newObj = (GameObject)Surforge.ChangePolyLassoProfileFeaturesScale((PolyLassoObject)newObj.GetComponent<PolyLassoObject>(), false, true, polyLassoScale);

			newObjs.Add(newObj);
		}

		Selection.objects = newObjs.ToArray();
		
		polygonLassoPoints = new List<Vector3>();

	}


	static void SetPolyLassoShapeHeight(List<Vector3> shape, float height) {
		for (int i=0; i < shape.Count; i++) {
			shape[i] = new Vector3(shape[i].x, height, shape[i].z);
		}
	}



	static bool PolyLassoDensityDialog(List<Vector3> shape, PolyLassoProfile profile, float lassoScale) {
		bool result = true;
	
		bool displayDialog = false;

		bool isAdaptive = false;

		if (profile.isAdaptive) {
			isAdaptive = true;
		}
		for (int i=0; i<profile.followerProfiles.Length; i++) {
			if (profile.followerProfiles[i].isAdaptive) {
				isAdaptive = true;
				break;
			}
		}

		if (isAdaptive) {

			float shapeLength = 0;
			
			for (int i=0; i < shape.Count; i++) {
				Vector3 pointA = shape[i];
				Vector3 pointB = new Vector3();
				
				if (i == (shape.Count-1)) pointB = shape[0];
				else pointB = shape[i+1];
				
				shapeLength = shapeLength + Vector3.Distance(pointA, pointB);
			}

			float scaledShapeLength = shapeLength * (float)(1.0f / lassoScale);

			//Debug.Log ("shapeLength: " + shapeLength + ",   scaledShapeLength: " + scaledShapeLength);

			if (scaledShapeLength >= 600) {
				displayDialog = true;
			}
		}

		if (displayDialog) {
			result = false;
			if (EditorUtility.DisplayDialog("High density shape", "You are about to create large shape with a complex Poly Lasso preset of detailed scale. This could be slow.", "Ok", "Use simple")) {
				result = true;
			}
		}
		return result;

	}



	static void PolygonLassoBuildObject() {
		Vector3[] pointsToCheck = polygonLassoPoints.ToArray();

	
		if (pointsToCheck.Length < 3) {
			if (pointsToCheck.Length > 0) {
				Surforge.LogAction("Fill UV island", "Double Click", "");

				polygonLassoPoints = GetUvBorderPointsAroundPoint(pointsToCheck[0]);
				SetPolyLassoShapeHeight(polygonLassoPoints, -0.25f);
				pointsToCheck = polygonLassoPoints.ToArray();
				if (polygonLassoPoints.Count < 3) return;
			}
		}

		
		StoreLastPolygonLassoPoints();
		lastPolyLassoWasSplit = false;
		
		if (!Surforge.CheckIfShapeClockwise(pointsToCheck)) polygonLassoPoints.Reverse();
		
		int selectedProfile;
		if (Surforge.surforgeSettings.activePolyLassoProfile < 0) selectedProfile = 0;
		else selectedProfile = Surforge.surforgeSettings.activePolyLassoProfile;
		PolyLassoProfile profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[selectedProfile];

		if (!PolyLassoDensityDialog(polygonLassoPoints, profile, polyLassoScale)) {
			profile = Surforge.surforgeSettings.polyLassoProfiles.polyLassoProfiles[0];
		}
		
		GameObject newObj = Surforge.PolygonLassoBuildObject(null, false, polygonLassoPoints, profile.bevelAmount, profile.bevelSteps, profile.offsets, profile.heights,
		                                                   new List<DecalSet>(), 
		                                                   new List<bool>(),
		                                                   new List<bool>(), 
		                                                   new List<bool>(), 
		                                                   new List<bool>(), 
		                                                   new List<bool>(), 
		                                                   new List<float>(), 
		                                                   new List<float>(),
		                                                   new List<float>(), 
		                                                   new List<float>(), 
		                                                   new List<float>(),
		                                                   new List<float>(), 
		                                                   new List<float>(), 
		                                                   new List<float>(), 
		                                                   new List<float>(),
		                                                   false,
		                                                   1.0f,
		                                                   new Vector2(1,1),
		                                                   0.5f,
		                                                   new Vector2(1,1),
		                                                   0.5f,
		                                                   0,
		                                                   profile.isFloater,
		                                                   profile.isTube,
		                                                   profile.isOpen,
		                                                   profile.thickness,
		                                                   profile.section,
		                                                   profile.isAdaptive,
		                                                   profile.adaptiveStep,
		                                                   profile.lengthOffsets0,
		                                                   profile.lengthOffsets1,
		                                                   profile.lengthOffsets2,
		                                                   profile.heightOffsets0,
		                                                   profile.heightOffsets1,
		                                                   profile.heightOffsets2,
		                                                   profile.repeatSize,
		                                                   profile.lengthOffsetOrder,
		                                                   profile.heightOffsetOrder,
		                                                   profile.edgeWiseOffset,
		                                                   profile.lengthWiseOffset,
		                                                   profile.offsetMinEdge,
		                                                   profile.corner,
		                                                   profile.childProfileVerticalOffsets, 
		                                                   profile.childProfileDepthOffsets,
		                                                   profile.childProfileHorisontalOffsets,
		                                                   profile.childProfileMatGroups,
		                                                   profile.childProfileShapes,
		                                                   profile.followerProfiles, 
		                                                   profile.followerProfileVerticalOffsets, 
		                                                   profile.followerProfileDepthOffsets,
		                                                   profile.followerProfileMatGroups,
		                                                   profile.cutoff, 
		                                                   profile.cutoffTiling,
		                                                   profile.bumpMap, 
		                                                   profile.bumpMapIntensity, 
		                                                   profile.bumpMapTiling,
		                                                   profile.aoMap,
		                                                   profile.aoMapIntensity,
		                                                   profile.randomUvOffset,
		                                                   profile.stoneType,
		                                                   profile.allowIntersections,
		                                                   profile.overlapIntersections,
		                                                   profile.overlapAmount,
		                                                   profile.usedForOverlapping, 
		                                                   profile.overlapStartInvert, 
		                                                   profile.curveUVs);

		newObj = (GameObject)Surforge.ChangePolyLassoProfileFeaturesScale((PolyLassoObject)newObj.GetComponent<PolyLassoObject>(), false, true, polyLassoScale);

		Selection.activeObject = newObj;
		
		polygonLassoPoints = new List<Vector3>();
	}
	
	static void PolygonLassoSplit() {
		Surforge.LogAction("Split", "Ctrl + Left Click", "");

		Vector3[] pointsToCheck = polygonLassoPoints.ToArray();
		
		StoreLastPolygonLassoPoints();
		lastPolyLassoWasSplit = true;
		
		if (Surforge.CheckIfShapeClockwise(pointsToCheck)) polygonLassoPoints.Reverse();
		
		Surforge.SplitSelectedPolyLassoObjects(polygonLassoPoints);


		Object[] newSelection = new Object[Selection.objects.Length];
		for (int i=0; i< Selection.objects.Length; i++) {
			newSelection[i] = Selection.objects[i];
		}
		Selection.objects = newSelection;


		polygonLassoPoints.Clear();
		
	}



	//swap materials
	static MaterialGroup[] materialGroups;


	static void StoreGroups() {
		materialGroups = new MaterialGroup[8];
		
		for (int i=0; i < 8; i++) {
			MaterialGroup materialGroup = GetMaterialGroup(Surforge.surforgeSettings.activeSceneMaterialNumber, i);
			materialGroups[i] = materialGroup;
		}
	}


	static void SwapGroups() {
		for (int i = materialGroups.Length - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			MaterialGroup tmp = materialGroups[i];
			materialGroups[i] = materialGroups[r];
			materialGroups[r] = tmp;
		}
	}


	static void SetGroups() {
		for (int i=0; i < 8; i++) {
			SetMaterialGroup(i, materialGroups[i]);
		}
	}


	static void SetMaterialFromRandomPresetToGroup(int grupNum) {
		int randomPresetNum = Random.Range(0, Surforge.surforgeSettings.sceneMaterials.Count);
		int randomGroupNum = Random.Range(0,8);

		MaterialGroup materialGroup = GetMaterialGroup(randomPresetNum, randomGroupNum);
		SetMaterialGroup(grupNum, materialGroup);
	}

	static MaterialGroup GetMaterialGroup(int materialSetNum, int groupNum) {
		if (groupNum > 7) groupNum = 7;

		MaterialGroup materialGroup = new MaterialGroup();

		materialGroup._Tint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_Tint" + groupNum);
		materialGroup._SpecularTint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_SpecularTint" + groupNum);
		
		materialGroup._SpecularIntensity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_SpecularIntensity" + groupNum);
		materialGroup._SpecularContrast = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "SpecularContrast");
		materialGroup._SpecularBrightness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "SpecularBrightness");
		
		materialGroup._Glossiness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Glossiness" + groupNum);
		materialGroup._GlossinessIntensity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_GlossinessIntensity" + groupNum);
		materialGroup._GlossinessContrast = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "GlossinessContrast");
		materialGroup._GlossinessBrightness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "GlossinessBrightness");
		
		materialGroup._Paint1Intensity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint1Intensity");
		materialGroup._Paint2Intensity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint2Intensity");
		materialGroup._WornEdgesNoiseMix = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetVector("_" + groupNum + "WornEdgesNoiseMix");
		materialGroup._WornEdgesAmount = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "WornEdgesAmount");
		materialGroup._WornEdgesOpacity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "WornEdgesOpacity");
		materialGroup._WornEdgesContrast = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "WornEdgesContrast");
		materialGroup._WornEdgesBorder = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "WornEdgesBorder");
		materialGroup._WornEdgesBorderTint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_" + groupNum + "WornEdgesBorderTint");
		materialGroup._UnderlyingDiffuseTint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_" + groupNum + "UnderlyingDiffuseTint");
		materialGroup._UnderlyingSpecularTint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_" + groupNum + "UnderlyingSpecularTint");
		materialGroup._UnderlyingDiffuse = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "UnderlyingDiffuse");
		materialGroup._UnderlyingSpecular = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "UnderlyingSpecular");
		materialGroup._UnderlyingGlossiness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "UnderlyingGlossiness");
		
		materialGroup._NormalsStrength = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_NormalsStrength" + groupNum);
		materialGroup._BumpMapStrength = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_BumpMapStrength" + groupNum);

		materialGroup._OcclusionMapStrength = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_OcclusionMapStrength" + groupNum);

		materialGroup._Paint1Specular = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint1Specular");
		materialGroup._Paint1Glossiness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint1Glossiness");
		materialGroup._Paint1Color = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_" + groupNum + "Paint1Color");
		materialGroup._Paint1MaskTex = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_" + groupNum + "Paint1MaskTex");
		materialGroup._Paint1MaskTex_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_" + groupNum + "Paint1MaskTex");
		materialGroup._Paint1MaskTex_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_" + groupNum + "Paint1MaskTex");
		materialGroup._Paint1NoiseMix = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetVector("_" + groupNum + "Paint1NoiseMix");

		materialGroup._Paint2Specular = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint2Specular");
		materialGroup._Paint2Glossiness = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint2Glossiness");
		materialGroup._Paint2Color = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_" + groupNum + "Paint2Color");
		materialGroup._Paint2MaskTex = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_" + groupNum + "Paint2MaskTex");
		materialGroup._Paint2MaskTex_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_" + groupNum + "Paint2MaskTex");
		materialGroup._Paint2MaskTex_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_" + groupNum + "Paint2MaskTex");
		materialGroup._Paint2NoiseMix = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetVector("_" + groupNum + "Paint2NoiseMix");

			
		materialGroup._Texture = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_Texture" + groupNum);
		materialGroup._Texture_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_Texture" + groupNum);
		materialGroup._Texture_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_Texture" + groupNum);

		materialGroup._BumpMap = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_BumpMap" + groupNum);
		materialGroup._BumpMap_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_BumpMap" + groupNum);
		materialGroup._BumpMap_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_BumpMap" + groupNum);

		materialGroup._OcclusionMap = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_OcclusionMap" + groupNum);

		materialGroup._SpecularMap = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_SpecularMap" + groupNum);
		materialGroup._SpecularMap_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_SpecularMap" + groupNum);
		materialGroup._SpecularMap_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_SpecularMap" + groupNum);

		materialGroup._UseSpecularMap = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_UseSpecularMap" + groupNum);
		materialGroup._GlossinessFromAlpha = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_GlossinessFromAlpha" + groupNum);

		materialGroup._EmissionMap = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_EmissionMap" + groupNum);
		materialGroup._EmissionMap_Scale = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureScale("_EmissionMap" + groupNum);
		materialGroup._EmissionMap_Offset = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTextureOffset("_EmissionMap" + groupNum);
		materialGroup._EmissionMapTint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_EmissionMapTint" + groupNum);
		materialGroup._EmissionMapIntensity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_EmissionMapIntensity" + groupNum);


		materialGroup._GlobalTransparency = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "GlobalTransparency");
		materialGroup._AlbedoTransparency = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "AlbedoTransparency");
		materialGroup._Paint1Transparency = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint1Transparency");
		materialGroup._Paint2Transparency = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "Paint2Transparency");

		materialGroup._MaterialRotation = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_" + groupNum + "MaterialRotation");

		return materialGroup;
	}


	static void SetMaterialGroup(int groupNum, MaterialGroup materialGroup) {
		if (groupNum > 7) groupNum = 7;

		Material composerMaterial = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial;

		composerMaterial.SetColor("_Tint" + groupNum, materialGroup._Tint);
		composerMaterial.SetColor("_SpecularTint" + groupNum, materialGroup._SpecularTint);
		
		composerMaterial.SetFloat("_SpecularIntensity" + groupNum, materialGroup._SpecularIntensity);
		composerMaterial.SetFloat("_" + groupNum + "SpecularContrast", materialGroup._SpecularContrast);
		composerMaterial.SetFloat("_" + groupNum + "SpecularBrightness", materialGroup._SpecularBrightness);
		
		composerMaterial.SetFloat("_Glossiness" + groupNum, materialGroup._Glossiness);
		composerMaterial.SetFloat("_GlossinessIntensity" + groupNum, materialGroup._GlossinessIntensity);
		composerMaterial.SetFloat("_" + groupNum + "GlossinessContrast", materialGroup._GlossinessContrast);
		composerMaterial.SetFloat("_" + groupNum + "GlossinessBrightness", materialGroup._GlossinessBrightness);
		
		composerMaterial.SetFloat("_" + groupNum + "Paint1Intensity", materialGroup._Paint1Intensity);
		composerMaterial.SetFloat("_" + groupNum + "Paint2Intensity", materialGroup._Paint2Intensity);
		composerMaterial.SetVector("_" + groupNum + "WornEdgesNoiseMix", materialGroup._WornEdgesNoiseMix);
		composerMaterial.SetFloat("_" + groupNum + "WornEdgesAmount", materialGroup._WornEdgesAmount);
		composerMaterial.SetFloat("_" + groupNum + "WornEdgesOpacity", materialGroup._WornEdgesOpacity);
		composerMaterial.SetFloat("_" + groupNum + "WornEdgesContrast", materialGroup._WornEdgesContrast);
		composerMaterial.SetFloat("_" + groupNum + "WornEdgesBorder", materialGroup._WornEdgesBorder);
		composerMaterial.SetColor("_" + groupNum + "WornEdgesBorderTint", materialGroup._WornEdgesBorderTint);
		composerMaterial.SetColor("_" + groupNum + "UnderlyingDiffuseTint", materialGroup._UnderlyingDiffuseTint);
		composerMaterial.SetColor("_" + groupNum + "UnderlyingSpecularTint", materialGroup._UnderlyingSpecularTint);
		composerMaterial.SetFloat("_" + groupNum + "UnderlyingDiffuse", materialGroup._UnderlyingDiffuse);
		composerMaterial.SetFloat("_" + groupNum + "UnderlyingSpecular", materialGroup._UnderlyingSpecular);
		composerMaterial.SetFloat("_" + groupNum + "UnderlyingGlossiness", materialGroup._UnderlyingGlossiness);

		composerMaterial.SetFloat("_NormalsStrength" + groupNum, materialGroup._NormalsStrength);
		composerMaterial.SetFloat("_BumpMapStrength" + groupNum, materialGroup._BumpMapStrength);

		composerMaterial.SetFloat("_OcclusionMapStrength" + groupNum, materialGroup._OcclusionMapStrength);

		composerMaterial.SetFloat("_" + groupNum + "Paint1Specular", materialGroup._Paint1Specular);
		composerMaterial.SetFloat("_" + groupNum + "Paint1Glossiness", materialGroup._Paint1Glossiness);
		composerMaterial.SetColor("_" + groupNum + "Paint1Color", materialGroup._Paint1Color);
		composerMaterial.SetTexture("_" + groupNum + "Paint1MaskTex", materialGroup._Paint1MaskTex);
		composerMaterial.SetTextureScale("_" + groupNum + "Paint1MaskTex", materialGroup._Paint1MaskTex_Scale);
		composerMaterial.SetTextureOffset("_" + groupNum + "Paint1MaskTex", materialGroup._Paint1MaskTex_Offset);
		composerMaterial.SetVector("_" + groupNum + "Paint1NoiseMix", materialGroup._Paint1NoiseMix);

		composerMaterial.SetFloat("_" + groupNum + "Paint2Specular", materialGroup._Paint2Specular);
		composerMaterial.SetFloat("_" + groupNum + "Paint2Glossiness", materialGroup._Paint2Glossiness);
		composerMaterial.SetColor("_" + groupNum + "Paint2Color", materialGroup._Paint2Color);
		composerMaterial.SetTexture("_" + groupNum + "Paint2MaskTex", materialGroup._Paint2MaskTex);
		composerMaterial.SetTextureScale("_" + groupNum + "Paint2MaskTex", materialGroup._Paint2MaskTex_Scale);
		composerMaterial.SetTextureOffset("_" + groupNum + "Paint2MaskTex", materialGroup._Paint2MaskTex_Offset);
		composerMaterial.SetVector("_" + groupNum + "Paint2NoiseMix", materialGroup._Paint2NoiseMix);


		composerMaterial.SetTexture("_Texture" + groupNum, materialGroup._Texture);
		composerMaterial.SetTextureScale("_Texture" + groupNum, materialGroup._Texture_Scale);
		composerMaterial.SetTextureOffset("_Texture" + groupNum, materialGroup._Texture_Offset);

		composerMaterial.SetTexture("_BumpMap" + groupNum, materialGroup._BumpMap);
		composerMaterial.SetTextureScale("_BumpMap" + groupNum, materialGroup._BumpMap_Scale);
		composerMaterial.SetTextureOffset("_BumpMap" + groupNum, materialGroup._BumpMap_Offset);

		composerMaterial.SetTexture("_OcclusionMap" + groupNum, materialGroup._OcclusionMap); 

		composerMaterial.SetTexture("_SpecularMap" + groupNum, materialGroup._SpecularMap);
		composerMaterial.SetTextureScale("_SpecularMap" + groupNum, materialGroup._SpecularMap_Scale);
		composerMaterial.SetTextureOffset("_SpecularMap" + groupNum, materialGroup._SpecularMap_Offset);

		composerMaterial.SetFloat("_UseSpecularMap" + groupNum, materialGroup._UseSpecularMap);
		composerMaterial.SetFloat("_GlossinessFromAlpha" + groupNum, materialGroup._GlossinessFromAlpha);

		composerMaterial.SetTexture("_EmissionMap" + groupNum, materialGroup._EmissionMap);
		composerMaterial.SetTextureScale("_EmissionMap" + groupNum, materialGroup._EmissionMap_Scale);
		composerMaterial.SetTextureOffset("_EmissionMap" + groupNum, materialGroup._EmissionMap_Offset);
		composerMaterial.SetColor("_EmissionMapTint" + groupNum, materialGroup._EmissionMapTint);
		composerMaterial.SetFloat("_EmissionMapIntensity" + groupNum, materialGroup._EmissionMapIntensity);

		composerMaterial.SetFloat("_" + groupNum + "GlobalTransparency", materialGroup._GlobalTransparency);
		composerMaterial.SetFloat("_" + groupNum + "AlbedoTransparency", materialGroup._AlbedoTransparency);
		composerMaterial.SetFloat("_" + groupNum + "Paint1Transparency", materialGroup._Paint1Transparency);
		composerMaterial.SetFloat("_" + groupNum + "Paint2Transparency", materialGroup._Paint2Transparency);

		composerMaterial.SetFloat("_" + groupNum + "MaterialRotation", materialGroup._MaterialRotation);
	}
	


	static void RevertToTempMaterial(Material dest) {
		dest.CopyPropertiesFromMaterial(tempMaterial);
	}



	static bool isCursorOverModel;
	static Vector2 texturePointMatchModelPoint;


	static int GetMaterialNumberUnderCursor(Vector2 mousePos, Rect previewRect) {
		int result = 0;

		float mouseInRectX = mousePos.x - 126.0f;
		float mouseInRectY = mousePos.y + 1.0f;

		float mousePercentX = mouseInRectX / (float)(previewRect.width);
		float mousePercentY = mouseInRectY / (float)(previewRect.height);

		if (mousePercentX < 0) mousePercentX = 0;
		if (mousePercentX > 1.0f) mousePercentX = 1.0f;
		if (mousePercentY < 0) mousePercentY = 0;
		if (mousePercentY > 1.0f) mousePercentY = 1.0f;

		RaycastHit hit;

		if (Surforge.surforgeSettings != null) {
			if (Surforge.surforgeSettings.extentTexturePreview != null) {
				if (Surforge.surforgeSettings.extentTexturePreview.previewCamera != null) {

					Ray ray = Surforge.surforgeSettings.extentTexturePreview.previewCamera.ViewportPointToRay(new Vector3(mousePercentX, (1.0f - mousePercentY), 0));

					if (Physics.Raycast(ray, out hit)) {
						isCursorOverModel = true;
						texturePointMatchModelPoint = hit.textureCoord;

						Material activeMaterial = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial;
						Texture2D tex = (Texture2D)activeMaterial.GetTexture("_ObjectMasks");
						Texture2D tex2 = (Texture2D)activeMaterial.GetTexture("_ObjectMasks2");

						if ((tex != null) && (tex2 != null)) {
							Color maskColor = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
							Color maskColor2 = tex2.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);

							//Debug.Log ("x: " + mousePercentX + "   y: " + mousePercentY + "   color: " + maskColor);

							if ( (maskColor.r > maskColor.g) && (maskColor.r > maskColor.b) && (maskColor.r > maskColor.a) ) result = 1; 
							if ( (maskColor.g > maskColor.r) && (maskColor.g > maskColor.b) && (maskColor.g > maskColor.a) ) result = 2; 
							if ( (maskColor.b > maskColor.r) && (maskColor.b > maskColor.g) && (maskColor.b > maskColor.a) ) result = 3; 
							if ( (maskColor.a > maskColor.r) && (maskColor.a > maskColor.g) && (maskColor.a > maskColor.b) ) result = 4; 

							if ( (maskColor2.r > maskColor2.g) && (maskColor2.r > maskColor2.b) && (maskColor2.r > maskColor2.a) ) result = 5; 
							if ( (maskColor2.g > maskColor2.r) && (maskColor2.g > maskColor2.b) && (maskColor2.g > maskColor2.a) ) result = 6; 
							if ( (maskColor2.b > maskColor2.r) && (maskColor2.b > maskColor2.g) && (maskColor2.b > maskColor2.a) ) result = 7; 
							if ( (maskColor2.a > maskColor2.r) && (maskColor2.a > maskColor2.g) && (maskColor2.a > maskColor2.b) ) result = 8; 
						}

					}
					else {
						isCursorOverModel = false;
					}

				}
			}
		}

		return result;
	}



	static MaterialGroup bufferMaterialGroup;
	static int lastCopiedMaterialNum = 0;

	static void CopyMaterial(int materialNum) {
		bufferMaterialGroup = GetMaterialGroup(Surforge.surforgeSettings.activeSceneMaterialNumber, materialNum);
		lastCopiedMaterialNum = materialNum;
	}

	static void PasteMaterial(int materialNum) {
		SetMaterialGroup(materialNum, bufferMaterialGroup);
	}

	static void SwapMaterial(int materialNum) {
		MaterialGroup swapMaterialGroup = GetMaterialGroup(Surforge.surforgeSettings.activeSceneMaterialNumber, materialNum);
		SetMaterialGroup(materialNum, bufferMaterialGroup);
		SetMaterialGroup(lastCopiedMaterialNum, swapMaterialGroup);
	}


	static bool isCyclyngMaterialsUnderCursor;
	static int materialCycleStartSetNum;
	static int materialCycleStartMaterialNum;
	static int materialCycleCount;

	static void SetMaterialUnderCursor(int materialCycleStartSetNum, int materialCycleStartMaterialNum, int materialCycleCount, int materialNumTo) {

		int materialNumFrom = materialCycleStartMaterialNum;
		int setNumFrom = materialCycleStartSetNum;

		for (int i=0; i< materialCycleCount; i++) {
			materialNumFrom++;
			if (materialNumFrom > 7) {
				materialNumFrom = 0;
				setNumFrom++;
			}
			if (setNumFrom > (Surforge.surforgeSettings.sceneMaterials.Count - 1)) {
				setNumFrom = 0;
			}
		}

		//Debug.Log (setNumFrom + "   " + materialNumFrom);


		MaterialGroup cycledMaterialGroup = GetMaterialGroup(setNumFrom, materialNumFrom);
		SetMaterialGroup(materialNumTo - 1, cycledMaterialGroup);

	}

	static DirtSettings GetDirt(int materialSetNum) {

		DirtSettings dirtSettings = new DirtSettings();

		dirtSettings._Dirt1Tint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_Dirt1Tint");
		dirtSettings._DirtNoise1Mix = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetVector("_DirtNoise1Mix");
		dirtSettings._Dirt1Amount = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt1Amount");
		dirtSettings._Dirt1Contrast = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt1Contrast");
		dirtSettings._Dirt1Opacity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt1Opacity");
		dirtSettings._DirtTexture1 = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_DirtTexture1");

		dirtSettings._Dirt2Tint = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetColor("_Dirt2Tint");
		dirtSettings._DirtNoise2Mix = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetVector("_DirtNoise2Mix");
		dirtSettings._Dirt2Amount = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt2Amount");
		dirtSettings._Dirt2Contrast = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt2Contrast");
		dirtSettings._Dirt2Opacity = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetFloat("_Dirt2Opacity");
		dirtSettings._DirtTexture2 = Surforge.surforgeSettings.sceneMaterials[materialSetNum].GetTexture("_DirtTexture2");

		return dirtSettings;
	}

	static void SetDirt(DirtSettings dirtSettings) {
		Material composerMaterial = Surforge.surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial;

		composerMaterial.SetColor ("_Dirt1Tint", dirtSettings._Dirt1Tint);
		composerMaterial.SetVector ("_DirtNoise1Mix", dirtSettings._DirtNoise1Mix);
		composerMaterial.SetFloat ("_Dirt1Amount", dirtSettings._Dirt1Amount);
		composerMaterial.SetFloat ("_Dirt1Contrast", dirtSettings._Dirt1Contrast);
		composerMaterial.SetFloat ("_Dirt1Opacity", dirtSettings._Dirt1Opacity);
		composerMaterial.SetTexture ("_DirtTexture1", dirtSettings._DirtTexture1);

		composerMaterial.SetColor ("_Dirt2Tint", dirtSettings._Dirt2Tint);
		composerMaterial.SetVector ("_DirtNoise2Mix", dirtSettings._DirtNoise2Mix);
		composerMaterial.SetFloat ("_Dirt2Amount", dirtSettings._Dirt2Amount);
		composerMaterial.SetFloat ("_Dirt2Contrast", dirtSettings._Dirt2Contrast);
		composerMaterial.SetFloat ("_Dirt2Opacity", dirtSettings._Dirt2Opacity);
		composerMaterial.SetTexture ("_DirtTexture2", dirtSettings._DirtTexture2);
	}


	static bool isCyclyngDirts;
	static int dirtCycleStartSetNum;
	static int dirtCycleCount;

	static void SetCycledDirt(int dirtCycleStartSetNum, int dirtCycleCount) {

		int setNumFrom = dirtCycleStartSetNum;
		
		for (int i=0; i< dirtCycleCount; i++) {
			setNumFrom++;

			if (setNumFrom > (Surforge.surforgeSettings.sceneMaterials.Count - 1)) {
				setNumFrom = 0;
			}
		}

		DirtSettings newDirtSettings = GetDirt(setNumFrom);
		SetDirt(newDirtSettings);
	}

	static void CheckMaterialNameUniq() {

		string nameToFilter = Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber].name;
		string filteredName = "";

		char[] forbeddenCharacters = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

		for (int i=0; i< nameToFilter.Length; i++) {
			bool isCharacterValid = true;
			for (int m=0; m < forbeddenCharacters.Length; m++) {
				if ( nameToFilter[i] == forbeddenCharacters[m] ) {
					isCharacterValid = false;
					break;
				}
			}
			if (isCharacterValid) {
				filteredName = filteredName + nameToFilter[i];
			}
		}


		bool nameMatchFound = false;
		int nameEnding = 0;
		string nameToCheck = filteredName;

		for (int n=0; n < 1000; n++) {

			for (int i=0; i< Surforge.surforgeSettings.sceneMaterials.Count; i++) {
				if (i != Surforge.surforgeSettings.activeSceneMaterialNumber) {
					if (nameToCheck == Surforge.surforgeSettings.sceneMaterials[i].name) {
						nameMatchFound = true;
						break;
					}
				}
			}
			if (!nameMatchFound) {
				break;
			}
			else {
				nameEnding++;
				nameMatchFound = false;
				nameToCheck = filteredName + "_" + nameEnding.ToString();
			}
		}

		Surforge.surforgeSettings.sceneMaterials[Surforge.surforgeSettings.activeSceneMaterialNumber].name = nameToCheck;

	}

	static bool IsMouseOver() {
		return Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
	} 

	static void UpdateSkyboxWindowRepaint() {
		if (Surforge.surforgeSettings.skyboxNeedWindowUpdate) {
			if ((Surforge.surforgeSettings.skyboxWindowRepaintTimer - Time.realtimeSinceStartup) < 0) {
				Surforge.surforgeSettings.skyboxNeedWindowUpdate = false;
			}
			if (window) window.Repaint();
			RepaintSceneView();
		}
	}

	
	static void DrawLastActionLog(string action, string hotkey, string secondHotkey, float alphaTimer) {

		float alpha = alphaTimer;
		if (alphaTimer > 1.0f) alpha = 1.0f;
		if (alphaTimer < 0) alpha = 0;

		int alphaInt = (int)(255 *  alpha);
		string alphaHex = alphaInt.ToString("X");
		if (alphaHex.ToCharArray().Length < 2) {
			alphaHex = "0" + alphaHex;
		}

		if (hotkey != "") {
			if (secondHotkey != "") {
				hotkey = "(" + hotkey + "</color> <color=#d2d2d2" + alphaHex +">or</color> <color=#ffa500" + alphaHex +">" + secondHotkey +")";
			}
			else {
				hotkey = "(" + hotkey + ")";
			}
		}

		int textLength = action.Length + hotkey.Length;
		int maxStringLength = 30;

		GUIContent actionText = new GUIContent("<b><size=32><color=#d2d2d2" + alphaHex +">" + action + "</color> <color=#ffa500" + alphaHex +">" + hotkey+ "</color></size></b>");

		if (textLength > maxStringLength) {
			actionText = new GUIContent("<b><size=32><color=#d2d2d2" + alphaHex +">" + action + "</color> \n<color=#ffa500" + alphaHex +">" + hotkey+ "</color></size></b>");
		}


		Handles.BeginGUI( );
		if ((textLength > maxStringLength) && (hotkey != "")) {
			GUILayout.BeginArea( new Rect( Screen.width * 0.5f - 500, Screen.height * 0.9f - 50, 1000, 100) );
		}
		else {
			GUILayout.BeginArea( new Rect( Screen.width * 0.5f - 500, Screen.height * 0.94f - 50, 1000, 100) );
		}

		GUILayout.Label(actionText, lastActionLogStyle);
		
		GUILayout.EndArea( );
		Handles.EndGUI( );

		if (alpha > 0) RepaintSceneView();
	}


	// -- shape warp--
	static bool IsShapeNeedFitAndRescale(Vector3[] shape) {
		bool result = false;
		for (int i=0; i < shape.Length; i++) {
			if ( ((shape[i].x - 0.1f) > Surforge.surforgeSettings.textureBorders.maxX) || ((shape[i].x + 0.1f) < Surforge.surforgeSettings.textureBorders.minX) ){
				result = true;
				break;
			}
		}
		return result;
	}

	static Vector3[] FitShapeToCanvasLeftX(Vector3[] shape) {
		Vector3[] result = new Vector3[shape.Length];

		float shapePointMinX = Mathf.Infinity;
		for (int i=0; i < shape.Length; i++) {
			if (shape[i].x < shapePointMinX) shapePointMinX = shape[i].x;
		}

		float offset = Surforge.surforgeSettings.textureBorders.minX - shapePointMinX;
		for (int i=0; i < shape.Length; i++) {
			result[i] = new Vector3(shape[i].x + offset, shape[i].y, shape[i].z);
		}

		return result;
	}

	static float GetShapeCanvasRelativeScale(Vector3[] shape) {
		float result = 1.0f;

		float shapePointMaxX = Mathf.NegativeInfinity;
		for (int i=0; i < shape.Length; i++) {
			if (shape[i].x > shapePointMaxX) shapePointMaxX = shape[i].x;
		}
		result = (float)(shapePointMaxX - Surforge.surforgeSettings.textureBorders.minX) / (float)(Surforge.surforgeSettings.textureBorders.maxX - Surforge.surforgeSettings.textureBorders.minX);

		return Mathf.Abs(result);
	}


	static void WarpPolyLassoObject(List<Vector3> shapeToWarp, PolyLassoObject pObj, bool isOpen, bool isSplit, bool repeatedWarp) {
		if (Surforge.surforgeSettings.warpShape == null) return;
		if (Surforge.surforgeSettings.warpShape.Count < 3) return;

		List<Vector3> warpedShape = new List<Vector3>();
		float shapeDistance = 0;

		//make shape for warping adaptive
		int[] subdValues = Surforge.GetShapeSubdivideValuesVector3(shapeToWarp.ToArray(), 0.5f);
		Vector3[] shape = Surforge.SubdivideShapeVector3(shapeToWarp.ToArray(), subdValues, isOpen);

		Vector3[] warpShape = new Vector3[Surforge.surforgeSettings.warpShape.Count];
		Surforge.surforgeSettings.warpShape.CopyTo(warpShape, 0);

		//check shape need fit and rescale
		bool fitNeeded = IsShapeNeedFitAndRescale(shape);
		float shapeScale = 1.0f;

		//fit canvas if outside canvas and scale if needed
		if ((fitNeeded) && (!repeatedWarp)) {
			shape = FitShapeToCanvasLeftX(shape);
			shapeScale = GetShapeCanvasRelativeScale(shape);
		}

		//percent code
		List<Vector2> percentCodedShape = new List<Vector2>();
		for (int i=0; i < shape.Length; i++) {
			float x = Surforge.surforgeSettings.textureBorders.maxX + shape[i].x;
			float pX = x / (float)(Surforge.surforgeSettings.textureBorders.maxX * 2);
			pX = pX / (float)shapeScale;
			if (!repeatedWarp) {
				if (pX == 1.0f) pX = 0.999999f;
				if (pX < 0) pX = 0;
			}
			else {
				if (pX == 1.0f) pX = 0.999999f;
				if (pX > 1.0f) pX = pX - 1.0f;
				if (pX < 0) pX = pX + 1.0f;
			}
			percentCodedShape.Add(new Vector2(pX, shape[i].z / (float)shapeScale));
		}

		//warp
		for (int i=0; i < percentCodedShape.Count; i++) {

			List<Vector3> offsettedWarpShape = new List<Vector3>();

			Vector2[] shape2d = Surforge.RemoveAllSandglass(Surforge.CreateShapeOutline(  Surforge.Points3DTo2D(warpShape), percentCodedShape[i].y, false)  );
					
			offsettedWarpShape = Surforge.Points2DTo3DList(shape2d, shape[0].y);
			if (offsettedWarpShape.Count > 2) offsettedWarpShape = Surforge.FitShapeStartPoint(offsettedWarpShape);

			shapeDistance = Surforge.GetShapeLength2D(offsettedWarpShape);

			float targetDistFromStart = shapeDistance * percentCodedShape[i].x;
			Vector3 warpedPoint = Surforge.GetWarpedPoint(targetDistFromStart, offsettedWarpShape.ToArray(), shape[0].y);
			warpedShape.Add(warpedPoint);
		}

		//optimize
		bool[] vertsToKeep = new bool[warpedShape.Count];
		for (int i=0; i<warpedShape.Count; i++) {
			if ( !(Surforge.CheckOptimizeShapePointSkip(warpedShape.ToArray(), i) )) {
				vertsToKeep[i] = true;
			}
		}
		List<Vector3> warpedShapeOptimized = new List<Vector3>();
		for (int i=0; i<warpedShape.Count; i++) {
			if (vertsToKeep[i] == true) {
				warpedShapeOptimized.Add(warpedShape[i]);
			}
		}

			       
		if (isSplit) {
			SplitWarped(warpedShapeOptimized);
		}
		else {
			BuildWarpedShape(warpedShapeOptimized, pObj);
		}

	}

	static void SplitWarped(List<Vector3> warpedShape) {
		Vector3[] pointsToCheck = warpedShape.ToArray();
		
		StoreLastPolygonLassoPoints();   
		lastPolyLassoWasSplit = true;
		
		if (Surforge.CheckIfShapeClockwise(pointsToCheck)) warpedShape.Reverse();
		
		Surforge.SplitSelectedPolyLassoObjects(warpedShape);
		
		polygonLassoPoints.Clear(); 
	}


	static void BuildWarpedShape(List<Vector3> warpedShape, PolyLassoObject pObj) {
		//build shape
		if (pObj == null) {
			if (warpedShape.Count > 2) {
				polygonLassoPoints = new List<Vector3>();
				for (int i=0; i < warpedShape.Count; i++) {
					polygonLassoPoints.Add(warpedShape[i]);
				}
				PolygonLassoBuildObject();
			}
		}
		
		// or rebuild object with new shape  
		else {
			GameObject originalTransformForCeltic = new GameObject();
			originalTransformForCeltic.transform.parent = pObj.transform.parent;
			originalTransformForCeltic.transform.localScale = pObj.transform.localScale;
			originalTransformForCeltic.transform.localPosition = pObj.transform.localPosition;
			originalTransformForCeltic.transform.localRotation = pObj.transform.localRotation;
			
			Transform pObjParent = pObj.gameObject.transform.parent;  
			pObj.gameObject.transform.parent = null;  
			Vector3 objLocalScale = pObj.gameObject.transform.localScale;
			Vector3 objLocalPosition = pObj.gameObject.transform.localPosition;
			Quaternion objLocalRotation = pObj.gameObject.transform.localRotation;
			
			GameObject relativeTransforms = new GameObject();
			relativeTransforms.transform.parent = pObj.transform;
			
			pObj.transform.localScale = Vector3.one;
			pObj.transform.localPosition = Vector3.zero;
			pObj.transform.localRotation = Quaternion.identity;
			
			warpedShape = Surforge.PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, warpedShape);

			
			GameObject warpedObject = Surforge.PolygonLassoBuildObject(originalTransformForCeltic, false, warpedShape, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
			                                                         pObj.decalSets, 
			                                                         pObj.inheritMatGroup,
			                                                         pObj.scatterOnShapeVerts, 
			                                                         pObj.trim,
			                                                         pObj.perpTrim,
			                                                         pObj.fitDecals,
			                                                         pObj.trimOffset,
			                                                         pObj.decalOffset, 
			                                                         pObj.decalOffsetRandom,
			                                                         pObj.decalGap, 
			                                                         pObj.decalGapRandom,
			                                                         pObj.decalSize, 
			                                                         pObj.decalSizeRandom, 
			                                                         pObj.decalRotation, 
			                                                         pObj.decalRotationRandom,
			                                                         pObj.noise,
			                                                         pObj.shapeSubdiv,
			                                                         pObj.noise1Amount,
			                                                         pObj.noise1Frequency,
			                                                         pObj.noise2Amount,
			                                                         pObj.noise2Frequency,
			                                                         pObj.materialID,
			                                                         pObj.isFloater,
			                                                         pObj.isTube,
			                                                         pObj.isOpen,
			                                                         pObj.thickness,
			                                                         pObj.section,
			                                                         pObj.isAdaptive,
			                                                         pObj.adaptiveStep,
			                                                         pObj.lengthOffsets0,
			                                                         pObj.lengthOffsets1,
			                                                         pObj.lengthOffsets2,
			                                                         pObj.heightOffsets0,
			                                                         pObj.heightOffsets1,
			                                                         pObj.heightOffsets2,
			                                                         pObj.repeatSize,
			                                                         pObj.lengthOffsetOrder,
			                                                         pObj.heightOffsetOrder,
			                                                         pObj.edgeWiseOffset,
			                                                         pObj.lengthWiseOffset,
			                                                         pObj.offsetMinEdge,
			                                                         pObj.corner,
			                                                         pObj.childProfileVerticalOffsets, 
			                                                         pObj.childProfileDepthOffsets,
			                                                         pObj.childProfileHorisontalOffsets,
			                                                         pObj.childProfileMatGroups,
			                                                         pObj.childProfileShapes,
			                                                         pObj.followerProfiles, 
			                                                         pObj.followerProfileVerticalOffsets, 
			                                                         pObj.followerProfileDepthOffsets,
			                                                         pObj.followerProfileMatGroups,
			                                                         pObj.cutoff, 
			                                                         pObj.cutoffTiling,
			                                                         pObj.bumpMap, 
			                                                         pObj.bumpMapIntensity, 
			                                                         pObj.bumpMapTiling,
			                                                         pObj.aoMap,
			                                                         pObj.aoMapIntensity,
			                                                         pObj.randomUvOffset,
			                                                         pObj.stoneType,
			                                                         pObj.allowIntersections,
			                                                         pObj.overlapIntersections,
			                                                         pObj.overlapAmount,
			                                                         pObj.usedForOverlapping, 
			                                                         pObj.overlapStartInvert, 
			                                                         pObj.curveUVs);
			
			if (warpedObject != null) {
				//set size
				warpedObject.transform.parent = null;
				warpedObject.transform.localScale = objLocalScale;
				warpedObject.transform.localPosition = objLocalPosition;
				warpedObject.transform.localRotation = objLocalRotation;
				warpedObject.transform.parent = Surforge.surforgeSettings.root.transform;
			}
			
			pObj.transform.localScale = objLocalScale;
			pObj.transform.localPosition = objLocalPosition;
			pObj.transform.localRotation = objLocalRotation;
			pObj.transform.parent = pObjParent;

			
			if (Surforge.surforgeSettings.seamless) {
				Surforge.RemoveSeamlessInstances(pObj);
				pObj.deleting = true;
			}
			
			//snap to objects
			if (Surforge.surforgeSettings.polyLassoObjects == null) {
				Surforge.surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
			}
			else {
				Surforge.surforgeSettings.polyLassoObjects.Remove(pObj);
			}	
			
			Undo.DestroyObjectImmediate(pObj.gameObject);

			Object[] newSelection = new Object[Selection.objects.Length+1];
			for (int s = 0; s < Selection.objects.Length; s++) {
				newSelection[s] = Selection.objects[s];
			}
			if (warpedObject != null) {
				newSelection[newSelection.Length-1] = warpedObject;
			}
			Selection.objects = newSelection;
			
		}
	}


	static bool CheckMaterialDragDropShaderMatch(Object dragDropObject) {
		bool match = false;
		if (dragDropObject != null) {
			if (dragDropObject.GetType() == typeof(Material)) {
				Material mat = (Material)dragDropObject;
				if (mat.shader == Shader.Find("Standard (Specular setup)")) {
					match = true;
				}
				if (mat.shader == Shader.Find("Hidden/SurforgeSingle")) {
					match = true;
				}
				if (mat.shader == Shader.Find("Hidden/Composer")) {
					match = true;
				}
			}
		}
		return match;
	}



	static Material dragDropFromProjectMat;
	static bool materialFromProjectDraggingNow;

	static Material CreateSurforgeMaterialFromProjectMaterial(Material projectMat) {
		Material result = new Material(Shader.Find("Hidden/SurforgeSingle"));

		if (projectMat.shader == Shader.Find("Standard (Specular setup)")) {
			Surforge.CopyStandardSpecularMaterialToChosenID(projectMat, 0, result, 0);
		}

		if (projectMat.shader == Shader.Find("Hidden/SurforgeSingle")) {
			Surforge.CopyMaterialToChosenID(projectMat, 0, result, 0);
		}

		return result;
	}

	static void CopyMaterialSet(Material sourceMaterial, Material destMaterial) {
		for (int i=0; i<8; i++) {
			Surforge.CopyMaterialToChosenID(sourceMaterial, i, destMaterial, i);
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


	/*
	void OnInspectorUpdate() {
		if (window != null) {

			if(EditorWindow.mouseOverWindow == window) {
				EditorWindow.mouseOverWindow.Focus();
			}


		}
	}
	*/

	
	
}