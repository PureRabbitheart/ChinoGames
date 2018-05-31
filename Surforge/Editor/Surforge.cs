using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MIConvexHull; 



public class Surforge : MonoBehaviour {  

	
	public static SurforgeSettings surforgeSettings;
	static List<SurforgeVoxelizedSet> voxelizedSets = new List<SurforgeVoxelizedSet>();
	static List<SurforgeSceneCluster> sceneClusters = new List<SurforgeSceneCluster>();
	static int gridOffset;
	static int[] fourVariants = new int[4] {0, 1, 2, 3};
	static int[] scatterFourVariants = new int[4] {19, 20, 7, 10};
	static int[] allVariants = new int[24] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23};
	public static List<SurforgeLink> lastPaintedLinks = new List<SurforgeLink>();
	static List<Transform> allRotations = new List<Transform>();
	static List<SurforgeSceneCluster> sceneClustersToRemoveAfterBeveling;
	static List<SurforgeSceneCluster> sceneClustersToRemove;
	static List<SurforgeSceneCluster> sceneClustersBeforeLimitsActivation;
	static int scatterLastPriority = 0;
	static SurforgeLimits limitsForCompare;
	static SurforgePattern pattern;
	static Material backgroundQuadMaterial;
		
	

	
	static void GetSettings() {
		Undo.undoRedoPerformed -= OnUndoRedo;
		Undo.undoRedoPerformed += OnUndoRedo;

		SurforgeSettings es = (SurforgeSettings)GameObject.FindObjectOfType(typeof(SurforgeSettings));
		if (es == null) {
			SetupProject();

			CreateSettingsObject();
			
			CreateParentObject();
			ClearArrays();
	
			CreateOctree();


			int res = Mathf.RoundToInt (Mathf.Pow(2, 10+ surforgeSettings.GetGpuRenderResolution()));
			CreateTextures(res);


			FillTexture(surforgeSettings.objectMasks, res, new Color(0,0,0,1)); 
			FillTexture(surforgeSettings.objectMasks2, res, new Color(0,0,0,0)); 
			FillTexture(surforgeSettings.normalMap, res, new Color(0.5f,0.5f,1,1));
			FillTexture(surforgeSettings.aoEdgesDirtDepth, res, new Color(1, 0, 1, 1));

			FillTexture(surforgeSettings.emissionMap, res, new Color(0,0,0,1));
			FillTexture(surforgeSettings.labelsMap, res, new Color(0,0,0,0));
			FillTexture(surforgeSettings.labelsAlpha, res, new Color(0,0,0,0));


		}
		else {
			surforgeSettings = es;
			surforgeSettings.showUvGrid = true;
			surforgeSettings.showUvs = false;
		}

		LoadSkyboxes();

		if (surforgeSettings.sceneMaterials == null) {
			CreateSceneMaterialsList(); //Material Sets
		}
		else {
			if (surforgeSettings.sceneMaterials.Count == 0) {
				CreateSceneMaterialsList(); //Material Sets
			}
		}

		LoadCustomMaterialPresets(); //Material Sets

		ReloadProjectMaterials(); //Meterials (NOT Material Sets)

	}


	public static void SetupProject() {
		//Debug.Log("setup project");
		QualitySettings.shadowDistance = 10000.0f;
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowCascade4Split = new Vector3(0.046f, 0.2f, 0.541f); 
	}


	static void CreateSceneMaterialsList() {
		surforgeSettings.sceneMaterials = new List<Material>();
		for (int i=0; i < surforgeSettings.composerPresets.presets.Count; i++) {
			Material material = new Material(surforgeSettings.composerPresets.presets[i]);
			material.SetFloat("_ShowID", 0);
			surforgeSettings.sceneMaterials.Add(material);
		}
	}

	static void LoadCustomMaterialPresets() {
		if(Directory.Exists("Assets/Surforge/CustomPresets/MaterialSets/")) {
			DirectoryInfo dirInfo = new DirectoryInfo("Assets/Surforge/CustomPresets/MaterialSets/");
			FileInfo[] fileInfo = dirInfo.GetFiles("*.mat");

			for (int i=0; i < fileInfo.Length; i++) {
				bool isPresetNameExists = false;
				for (int m=0; m < surforgeSettings.sceneMaterials.Count; m++) {
					if (surforgeSettings.sceneMaterials[m].name + ".mat" == fileInfo[i].Name) {
						isPresetNameExists = true;
						break;
					}
				}
				if (!isPresetNameExists) {
					Material newMaterial = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Surforge/CustomPresets/MaterialSets/" + fileInfo[i].Name, typeof(Material)) );
					newMaterial.SetFloat("_ShowID", 0);
					surforgeSettings.sceneMaterials.Add(newMaterial);
				}
			}

		}
	}




	public static void ReloadProjectMaterials() {
		
		//check for custom materials
		List<Material> foundCustomMaterials = new List<Material>(); 
		if(Directory.Exists("Assets/Surforge/CustomPresets/Materials/")) {
			DirectoryInfo customMaterialsDirectoryInfo = new DirectoryInfo("Assets/Surforge/CustomPresets/Materials/");
			FileInfo[] customMaterialsFileInfo = customMaterialsDirectoryInfo.GetFiles("*.mat");
			for (int i=0; i < customMaterialsFileInfo.Length; i++) {
				Material newMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Surforge/CustomPresets/Materials/" + customMaterialsFileInfo[i].Name, typeof(Material));
				if (newMaterial != null) {
					if (newMaterial.shader.name == "Hidden/SurforgeSingle") {
						foundCustomMaterials.Add(newMaterial);
					}
				}
			}
		}
		
		
		List<Material> foundMaterials = new List<Material>(); 
		
		DirectoryInfo materialsDirectoryInfo = new DirectoryInfo("Assets/Surforge/SurforgeMaterials");
		DirectoryInfo[] materialFolders = materialsDirectoryInfo.GetDirectories();
		
		if (foundCustomMaterials.Count > 0) {
			surforgeSettings.materialFoldersNames = new string[materialFolders.Length + 1];
			surforgeSettings.materialFoldersMatCount = new int[materialFolders.Length + 1];
			surforgeSettings.materialFoldersFoldout = new bool[materialFolders.Length + 1];
			for (int i=0; i< surforgeSettings.materialFoldersFoldout.Length; i++) {
				surforgeSettings.materialFoldersFoldout[i] = true;
			}
		}
		else {
			surforgeSettings.materialFoldersNames = new string[materialFolders.Length];
			surforgeSettings.materialFoldersMatCount = new int[materialFolders.Length];
			surforgeSettings.materialFoldersFoldout = new bool[materialFolders.Length];
			for (int i=0; i< surforgeSettings.materialFoldersFoldout.Length; i++) {
				surforgeSettings.materialFoldersFoldout[i] = true;
			}
		}
		
		for (int i=0; i < materialFolders.Length; i++) {
			surforgeSettings.materialFoldersNames[i] = materialFolders[i].Name;
			
			string[] materialGuids = AssetDatabase.FindAssets("t:Material", new string[] {"Assets/Surforge/SurforgeMaterials" + "/" + materialFolders[i].Name });
			
			int foundMatCount = 0;
			for (int m=0; m< materialGuids.Length; m++) {
				string path = AssetDatabase.GUIDToAssetPath(materialGuids[m]);
				Material material = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));
				if (material != null) {
					if (material.shader.name == "Hidden/SurforgeSingle") {
						foundMaterials.Add(material);
						foundMatCount++;
					}
				}
			}
			surforgeSettings.materialFoldersMatCount[i] = foundMatCount;
		}
		
		if (foundCustomMaterials.Count > 0) {
			surforgeSettings.materialFoldersNames[surforgeSettings.materialFoldersNames.Length - 1] = "Custom";
			surforgeSettings.materialFoldersMatCount[surforgeSettings.materialFoldersMatCount.Length - 1] = foundCustomMaterials.Count;
			for (int i=0; i < foundCustomMaterials.Count; i++) {
				foundMaterials.Add(foundCustomMaterials[i]);
			}
		}
		
		surforgeSettings.materials = foundMaterials.ToArray();



		List<Material> reloadedMaterials = new List<Material>();

		for (int i=0; i < surforgeSettings.materials.Length; i++) {
			reloadedMaterials.Add(surforgeSettings.materials[i]);
		}

		surforgeSettings.loadedMaterials = new Material[reloadedMaterials.Count];
		reloadedMaterials.CopyTo(surforgeSettings.loadedMaterials); 

		//Debug.Log ("materials reloaded");
	}




	static void FillTexture(Texture2D texture, int res, Color color) {
		int mipCount = Mathf.Min(3, texture.mipmapCount);
		for( int mip = 0; mip < mipCount; ++mip ) {
			Color[] cols = texture.GetPixels( mip );
			for( int i = 0; i < cols.Length; ++i ) {
				cols[i] = color;
			}
			texture.SetPixels( cols, mip );
		}
		texture.Apply(true);
	}


	public static void PrepareVoxelizedSet() {
		//Debug.Log ("clusters voxelized");

		CreateVoxilizedClusters();
		CreateAllRotations();
		HideRotationsFromHierarch();
	}
	
	
	static bool IsActiveLayerEmpty() {
		bool empty = true;
		if (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Count > 0) empty = false;
		return empty;
	}
	

	public static void CreateSettingsObject() {

		GameObject settingsObj = new GameObject();
		surforgeSettings = settingsObj.AddComponent<SurforgeSettings>();
		surforgeSettings.name = "settings";

		surforgeSettings.maxBranches = 8;

		surforgeSettings.materialGroups = new Shader[11];
		surforgeSettings.materialGroups[0] = Shader.Find("Hidden/Id0");
		surforgeSettings.materialGroups[1] = Shader.Find("Hidden/Id1");
		surforgeSettings.materialGroups[2] = Shader.Find("Hidden/Id2");
		surforgeSettings.materialGroups[3] = Shader.Find("Hidden/Id3");
		surforgeSettings.materialGroups[4] = Shader.Find("Hidden/Id4");
		surforgeSettings.materialGroups[5] = Shader.Find("Hidden/Id5");
		surforgeSettings.materialGroups[6] = Shader.Find("Hidden/Id6");
		surforgeSettings.materialGroups[7] = Shader.Find("Hidden/Id7");
		surforgeSettings.materialGroups[8] = Shader.Find("Hidden/EmissionId0");
		surforgeSettings.materialGroups[9] = Shader.Find("Hidden/EmissionId1");
		surforgeSettings.materialGroups[10] = Shader.Find("Hidden/EmissionId2");

		surforgeSettings.floaterMaterialGroups = new Shader[8];
		surforgeSettings.floaterMaterialGroups[0] = Shader.Find("Hidden/Id0Floater");
		surforgeSettings.floaterMaterialGroups[1] = Shader.Find("Hidden/Id1Floater");
		surforgeSettings.floaterMaterialGroups[2] = Shader.Find("Hidden/Id2Floater");
		surforgeSettings.floaterMaterialGroups[3] = Shader.Find("Hidden/Id3Floater");
		surforgeSettings.floaterMaterialGroups[4] = Shader.Find("Hidden/Id4Floater");
		surforgeSettings.floaterMaterialGroups[5] = Shader.Find("Hidden/Id5Floater");
		surforgeSettings.floaterMaterialGroups[6] = Shader.Find("Hidden/Id6Floater");
		surforgeSettings.floaterMaterialGroups[7] = Shader.Find("Hidden/Id7Floater");

		surforgeSettings.exportDiffuse = Shader.Find("Hidden/ExportDiffuse");
		surforgeSettings.exportNormals = Shader.Find("Hidden/ExportNormals");
		surforgeSettings.exportSpecular = Shader.Find("Hidden/ExportSpecular");
		surforgeSettings.exportGlossiness = Shader.Find("Hidden/ExportGlossiness");
		surforgeSettings.exportAo = Shader.Find("Hidden/ExportAo");
		surforgeSettings.exportEmission = Shader.Find("Hidden/ExportEmission");
		surforgeSettings.exportHeightmap = Shader.Find("Hidden/ExportHeightmap");
		surforgeSettings.exportMask = Shader.Find("Hidden/ExportMask");
		surforgeSettings.exportMaskSeparate = Shader.Find("Hidden/ExportMaskSeparate");
		surforgeSettings.exportAlpha = Shader.Find("Hidden/ExportAlpha");

		surforgeSettings.grayBackgroundPost = Shader.Find("Hidden/GrayBackgroundPost");

		Mesh previewCube = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Surforge/TexturePreview/PreviewMeshes/preview_Cube.fbx", typeof(Mesh));
		surforgeSettings.model = previewCube;
		surforgeSettings.cubePreviewModel = previewCube;

		surforgeSettings.normalsBakedAddPost = Shader.Find("Hidden/NormalsBakedAddPost");


		surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();


		surforgeSettings.renderMaterialIconCameraPrefab = (Camera)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/renderMaterialIconCamera.prefab", typeof(Camera));
		surforgeSettings.renderMaterialIconCameraLitePrefab = (Camera)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/renderMaterialIconCameraLite.prefab", typeof(Camera));
		surforgeSettings.renderMaterialIconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/renderMaterialIcon.prefab", typeof(GameObject));
		surforgeSettings.renderMaterialIconShader = Shader.Find("Hidden/SurforgeSingle");
		surforgeSettings.rgbaNoise = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Textures/noiseRGBA_01.psd", typeof(Texture2D));
		surforgeSettings.renderMaterialIconNormal = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/materialIconRender_normal.png", typeof(Texture2D));
		surforgeSettings.renderMaterialIconAoEdgesDirtDepth = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/materialIconRender_aoEdgesDirtDepth.png", typeof(Texture2D));
		surforgeSettings.renderMaterialIconNoise = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/materialIconRender_noise.psd", typeof(Texture2D));
		surforgeSettings.editorPreviewAoEdgesDirtDepth = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/editorPreview_aoEdgesDirtDepth.png", typeof(Texture2D));
		surforgeSettings.editorPreviewMasks = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/editorPreview_masks.psd", typeof(Texture2D));
		surforgeSettings.editorPreviewMasks2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/editorPreview_masks2.psd", typeof(Texture2D));

		surforgeSettings.uvGridStep = 4;
		surforgeSettings.seamlessHighlight = (Material)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/seamlessHighlight.mat", typeof(Material));
		surforgeSettings.mapsExportCameraPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/mapsExportCamera.prefab", typeof(GameObject));
		surforgeSettings.bucketDownscale = Shader.Find("Hidden/BucketDownscale");
		surforgeSettings.bucketsCollect = Shader.Find("Hidden/BucketsCollect");

		///////
	
		surforgeSettings.polyLassoProfiles = (PolyLassoProfiles)AssetDatabase.LoadAssetAtPath("Assets/Surforge/PolyLassoProfiles/polyLassoProfiles.prefab", typeof(PolyLassoProfiles));

		surforgeSettings.decalSets = (DecalSets)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Decals/decalSets.prefab", typeof(DecalSets));

		surforgeSettings.noisePresets = (NoisePresets)AssetDatabase.LoadAssetAtPath("Assets/Surforge/NoisePresets/noisePresets.prefab", typeof(NoisePresets));

		surforgeSettings.composerPresets = (SurforgeComposerPresets)AssetDatabase.LoadAssetAtPath("Assets/Surforge/ComposerPresets/composerPresets.prefab", typeof(SurforgeComposerPresets));

		surforgeSettings.placeMeshes = (PlaceMeshes)AssetDatabase.LoadAssetAtPath("Assets/Surforge/AddDetailTool/placeMeshes.prefab", typeof(PlaceMeshes));

		surforgeSettings.shatterPresets = (ShatterPresets)AssetDatabase.LoadAssetAtPath("Assets/Surforge/ShatterPresets/shatterPresets.prefab", typeof(ShatterPresets));

	}


	public static void LoadSkyboxes() {
		string[] skyboxGuids = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/Surforge/Skybox"});
		List<SurforgeSkybox> foundSkyboxes = new List<SurforgeSkybox>();
		for (int i=0; i< skyboxGuids.Length; i++) {
			string path = AssetDatabase.GUIDToAssetPath(skyboxGuids[i]);
			SurforgeSkybox skybox = (SurforgeSkybox)AssetDatabase.LoadAssetAtPath(path, typeof(SurforgeSkybox));
			if (skybox != null) foundSkyboxes.Add(skybox);
		}
		surforgeSettings.skyboxes = foundSkyboxes.ToArray();
	}


	public static void LoadGreebles() {
		if (surforgeSettings.contentPacks == null) {
			surforgeSettings.contentPacks = new SurforgeContentPack[1];
			surforgeSettings.contentPacks[0] = (SurforgeContentPack)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GreeblesTool/greeblesTool.prefab", typeof(SurforgeContentPack));
		}
	}


	public static void VoxelEngineActivate () {
		GetSettings();
		CreateTextureBorders();
		if (surforgeSettings.backgroundQuad == null) CreateBackgroundQuad();
		if (surforgeSettings.layers.Count < 1) CreateNewLayer();
	}
	

	static void ClearArrays() {
		surforgeSettings.layers = new List<SurforgeLayer>();
		sceneClusters.Clear();
		lastPaintedLinks.Clear();
	}

	static void CreateParentObject() {
		surforgeSettings.root = new GameObject();
		SurforgeRoot surforgeRoot = (SurforgeRoot)surforgeSettings.root.AddComponent<SurforgeRoot>();
		surforgeRoot.surforgeSettings = surforgeSettings;
		surforgeSettings.root.name = "root";
	}
	
	public static void NewLayer () {
		GetSettings();
		CreateNewLayer();
		surforgeSettings.currentLayer = surforgeSettings.layers.Count-1;
	}
	
	static void CreateNewLayer() {
		GameObject obj = new GameObject();
		obj.AddComponent<SurforgeLayer>();
		obj.transform.parent = surforgeSettings.transform;
		surforgeSettings.layers.Add (obj.GetComponent<SurforgeLayer>());
		obj.name = "Layer" + (surforgeSettings.layers.Count-1).ToString();
	}
	

	

	static void HideRotationsFromHierarch() {
		foreach (Transform r in allRotations) {
			r.gameObject.hideFlags = HideFlags.HideInHierarchy;
		}
	}




	public static void GrowWithBrush(SurforgeOctreeNode brushSelectedNode) {
		List<SurforgeLink> foundLinks =  new List<SurforgeLink>();
		List<SurforgeOctreeNode> neighbourNodes = new List<SurforgeOctreeNode>();

		brushSelectedNode.SearchForLinksInNodeChildren(foundLinks);
		brushSelectedNode.SearchForNeighbors(neighbourNodes, surforgeSettings.octree);

		foreach (SurforgeOctreeNode neighbour in neighbourNodes) {
			neighbour.SearchForLinksInNodeChildren(foundLinks);
		}

		if (foundLinks.Count > 0) {
			foreach (SurforgeLink link in foundLinks) {
				SurforgeSceneCluster sCluster = (SurforgeSceneCluster)link.node.block.sCluster;
				if ( (!IsBlockActive(link)) && (sCluster.layer == surforgeSettings.currentLayer) ) {
				
					Int3 position = new Int3();
					position.x = link.x;
					position.y = link.y;
					position.z = link.z;

					if (GrowNewClusters(position, link, true)) {
						surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
						surforgeSettings.lastAction = 2;
					}

				}
			}

		}


		else { 
			ScatterWithBrush(brushSelectedNode, neighbourNodes);
		}

	}

	static void ScatterWithBrush(SurforgeOctreeNode brushSelectedNode, List<SurforgeOctreeNode> neighbourNodes) {
		List<SurforgeLink> foundLinks =  new List<SurforgeLink>();
		
		brushSelectedNode.SearchForCoversInNodeChildren(foundLinks);
		
		foreach (SurforgeOctreeNode neighbour in neighbourNodes) {
			neighbour.SearchForCoversInNodeChildren(foundLinks);
		}
		
		if (foundLinks.Count > 0) {
			foreach (SurforgeLink cover in foundLinks) {
				SurforgeSceneCluster sCluster = (SurforgeSceneCluster)cover.node.block.sCluster;
				if ( (!IsBlockActive(cover)) && (sCluster.layer == (surforgeSettings.currentLayer - 1)) ) {
					
					Int3 position = new Int3();
					position.x = cover.x;
					position.y = cover.y;
					position.z = cover.z;

					if (ScatterNewClusters(position, cover, true)) {
						surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
						surforgeSettings.lastAction = 2;
					}
					
				}
			}
			
		}

	}





	public static List<SurforgeLink> GetFreeLinksFromCurrentLayerShuffleForEveryCluster() {
		List<SurforgeLink> currentLayerFreeLinks = new List<SurforgeLink>();
		foreach (SurforgeSceneCluster sCluster in surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters) {
			List<SurforgeLink> clusterLinks = new List<SurforgeLink>();
			foreach (SurforgeLink link in sCluster.links) {
				clusterLinks.Add (link);
			}
			ShuffleLinks(clusterLinks);
			currentLayerFreeLinks.AddRange(clusterLinks);
		}
		return currentLayerFreeLinks;
	}
	

	public static List<SurforgeLink> GetFreeLinksFromCurrentLayer() {
		List<SurforgeLink> currentLayerFreeLinks = new List<SurforgeLink>();
		foreach (SurforgeSceneCluster sCluster in surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters) {
			foreach (SurforgeLink link in sCluster.links) {
				currentLayerFreeLinks.Add (link);
			}
		}
		return currentLayerFreeLinks;
	}



	public static List<SurforgeLink> GetFreeCoversFromPreviousLayer() {
		if (surforgeSettings.currentLayer == 0) return null;
		List<SurforgeLink> previousLayerFreeCovers = new List<SurforgeLink>();
		foreach (SurforgeSceneCluster sCluster in surforgeSettings.layers[surforgeSettings.currentLayer-1].sceneClusters) {
			foreach (SurforgeLink cover in sCluster.covers) {
				previousLayerFreeCovers.Add (cover);
			}
		}
		return previousLayerFreeCovers;
	}

	static List<SurforgeLink> AddLimitsCovers(List<SurforgeLink> coversAddTo) {
		if (coversAddTo == null) coversAddTo = new List<SurforgeLink>();

		int lengthX = Mathf.RoundToInt(surforgeSettings.limits.maxX *2  - surforgeSettings.limits.minX *2 );
		int lengthZ = Mathf.RoundToInt(surforgeSettings.limits.maxZ *2  - surforgeSettings.limits.minZ *2 ); 

		for (int x = 0; x < lengthX; x++) {
			for (int z = 0; z < lengthZ; z++) {
				SurforgeLink cover = (SurforgeLink)ScriptableObject.CreateInstance(typeof(SurforgeLink));
				cover.x = Mathf.RoundToInt(surforgeSettings.limits.minX *2 + x + 0.5f);
				cover.y = Mathf.RoundToInt(surforgeSettings.limits.minY *2 + 0.5f);
				cover.z = Mathf.RoundToInt(surforgeSettings.limits.minZ *2 + z + 0.5f);


				cover.isTemporaryLink = true;
				coversAddTo.Add (cover);

				if (IsNumberEven(lengthZ)) {
					if ((x == 0) && (z == Mathf.FloorToInt(lengthZ * 0.5f) )) cover.centerZ = true;
					if (( x == lengthX-1) && (z == Mathf.FloorToInt(lengthZ * 0.5f) - 1)) cover.centerZ = true;
				}
				else {
					if ((x == 0) && (z == Mathf.FloorToInt(lengthZ * 0.5f) )) cover.centerZ = true;
					if (( x == lengthX-1) && (z == Mathf.FloorToInt(lengthZ * 0.5f))) cover.centerZ = true;
				}


				if (IsNumberEven(lengthX)) {
					if ((z == 0) && (x == Mathf.FloorToInt(lengthX * 0.5f) - 1)) cover.centerX = true;
					if (( z == lengthZ-1) && (x == Mathf.FloorToInt(lengthX * 0.5f))) cover.centerX = true;
				}
				else {
					if ((z == 0) && (x == Mathf.FloorToInt(lengthX * 0.5f) )) cover.centerX = true;
					if (( z == lengthZ-1) && (x == Mathf.FloorToInt(lengthX * 0.5f))) cover.centerX = true;
				}

			}
		}

		return coversAddTo;
	}


	public static void VoxelBevelEdges () {
		sceneClustersToRemoveAfterBeveling = new List<SurforgeSceneCluster>();

		for (int i = 0; i < surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Count; i++) {
			if (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i].vCluster.voxelizedBevelClusters != null) {
				if (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i].vCluster.voxelizedBevelClusters.Length > 0) {
					VBevelEdges (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i]);
				}
			}
		}

		foreach (SurforgeSceneCluster sClusterToRemove in sceneClustersToRemoveAfterBeveling) {
			surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Remove(sClusterToRemove);
			DestroyImmediate(sClusterToRemove.gameObject);
		}
	}

	static void VBevelEdges(SurforgeSceneCluster sCluster ) {
		int rotationMode = 3;
		ShuffleFourVariants();
		bool result = false;
		for (int i = 0; i< sCluster.vCluster.voxelizedBevelClusters.Length; i++ ) {

			for (int v = 0; v < fourVariants.Length; v++ ) {
				if (AddBevelCluster(sCluster.vCluster.voxelizedBevelClusters[i], sCluster, fourVariants[v], rotationMode)) {
					result = true;
					break;
				}
			}
			if (result) break;
		}
	}


	static bool AddBevelCluster(SurforgeVoxelizedCluster vCluster, SurforgeSceneCluster oldSCluster, int variant, int rotationMode ) {
		bool result = false;
		bool ivy = false;
		bool addBevel = true;


		Int3 oldCenterOffset = GetClusterCenterOffset(oldSCluster.vCluster.clusterAllRotations[oldSCluster.variant]);
		Int3 offset = GetClusterCenterOffset(vCluster.clusterAllRotations[variant]);

		Int3 newPostition = new Int3();

		newPostition.x = oldSCluster.positionX - oldCenterOffset.x + offset.x;
		newPostition.y = oldSCluster.positionY - oldCenterOffset.y + offset.y;
		newPostition.z = oldSCluster.positionZ - oldCenterOffset.z + offset.z;


		if (CombinedCheck(vCluster, vCluster.clusterAllRotations[variant], newPostition, offset, ivy, addBevel, oldSCluster)) {

			SurforgeSceneCluster sCluster = VAddSceneCluster(vCluster, newPostition, allRotations[variant], variant, oldSCluster.group); 
			AddClusterToOctree(vCluster.clusterAllRotations[variant], newPostition, offset, false, sCluster);
			result = true;

			sceneClustersToRemoveAfterBeveling.Add (oldSCluster);					
		}
	
		return result;
	}


	public static void Reroll() {
		if (surforgeSettings.isLimitsActive) {
			if (surforgeSettings.lastAction == 1) {
				SurforgeUndo();
				Grow();
			}
			else {
				if (surforgeSettings.lastAction == 0) {
					SurforgeUndo();
					VoxelEngineScatter();
				}
			}
		}
	}



	public static void SurforgeUndo() {
	
		scatterLastPriority = 0;
		sceneClustersToRemove =  new List<SurforgeSceneCluster>();

		for (int i=0; i < surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Count; i++) {

			if (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i].generation == 
			    surforgeSettings.layers[surforgeSettings.currentLayer].generation - 1 ) {

				RemoveSceneCluster(surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i]);

				sceneClustersToRemove.Add (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[i]);
			}

		}

		foreach (SurforgeSceneCluster sClusterToRemove in sceneClustersToRemove) {
			//Debug.Log (sClusterToRemove);
			surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Remove(sClusterToRemove);
			DestroyImmediate(sClusterToRemove.gameObject);
		}

		surforgeSettings.layers[surforgeSettings.currentLayer].generation-- ;

	}

	static void RemoveSceneCluster(SurforgeSceneCluster sCluster) {
		//Debug.Log (sCluster);

		if (sCluster.vCluster.clusterAllRotations == null) {
			ReconstructClusterGridAndRotations(sCluster.vCluster);
		}
		Int3 centerOffset = GetClusterCenterOffset(sCluster.vCluster.clusterAllRotations[sCluster.variant]);

		Int3 position = new Int3();
		position.x = sCluster.positionX;
		position.y = sCluster.positionY;
		position.z = sCluster.positionZ;

		RemoveClusterFromOctree(sCluster.vCluster.clusterAllRotations[sCluster.variant], position, centerOffset);
	}


	static void ReconstructClusterGridAndRotations(SurforgeVoxelizedCluster vCluster) {
		vCluster.clusterGrid = ClusterToVoxels((SurforgeCluster)vCluster.clusterPrefab);

		if (vCluster.freeform) {
			vCluster.clusterAllRotations = GetClusterGridALLRotations(vCluster.clusterGrid);
		}
		else {
			vCluster.clusterAllRotations = GetClusterGridFourRotations(vCluster.clusterGrid);
		}
	}


	
	public static void Grow() {
		if (surforgeSettings.isLimitsActive) {
			if (surforgeSettings.layers.Count < 1) CreateNewLayer();
			if (IsActiveLayerEmpty()) {
				VoxelEngineScatter();
			}
			else {
				surforgeSettings.lastAction = 1;

				if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[surforgeSettings.activeSet].growMode == 0) {
					GrowShuffle ();
				}
				if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[surforgeSettings.activeSet].growMode == 1) {
					GrowBranches ();
				}
				if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[surforgeSettings.activeSet].growMode == 2) {
					GrowBruteFromOldest ();
				}

			}
		}
	}


	public static void GrowShuffle () {
		bool atLeastOneClusterAdded = false;

		List<SurforgeLink> links = GetFreeLinksFromCurrentLayer();
		ShuffleLinks(links); 

		int branchesLeft = surforgeSettings.maxBranches;

		for (int i = links.Count - 1; i >= 0; i--) {
			if (branchesLeft > 0 ) {
				if (!IsBlockActive(links[i])) {

					Int3 position = new Int3();
					position.x = links[i].x;
					position.y = links[i].y;
					position.z = links[i].z;

					if (GrowNewClusters(position, links[i], false)) {

						branchesLeft--;
						atLeastOneClusterAdded = true;
					}

				}
	
			}
		}
		if (atLeastOneClusterAdded) {
			surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
			surforgeSettings.canIvyOnGlueproof = false;
		}
		else surforgeSettings.canIvyOnGlueproof = true;
	}

	public static void GrowBranches () {
		bool atLeastOneClusterAdded = false;
		
		List<SurforgeLink> links = GetFreeLinksFromCurrentLayerShuffleForEveryCluster();
		int branchesLeft = surforgeSettings.maxBranches;
		
		for (int i = links.Count - 1; i >= 0; i--) {
			if (branchesLeft > 0 ) {
				if (!IsBlockActive(links[i])) {
					
					Int3 position = new Int3();
					position.x = links[i].x;
					position.y = links[i].y;
					position.z = links[i].z;
					
					if (GrowNewClusters(position, links[i], false)) {
						
						branchesLeft--;
						atLeastOneClusterAdded = true;
					}
					
				}
				
			}
		}
		if (atLeastOneClusterAdded) {
			surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
			surforgeSettings.canIvyOnGlueproof = false;
		}
		else surforgeSettings.canIvyOnGlueproof = true;
	}

	public static void GrowBruteFromOldest () {
		bool atLeastOneClusterAdded = false;

		bool isScattering = false;
		bool isPainting = false;


		int branchesLeft = surforgeSettings.maxBranches;
		for (int h = 0; h < branchesLeft; h++) {


			List<SurforgeSceneCluster> sceneClusters = new List<SurforgeSceneCluster>();

			for (int c = 0; c < surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Count; c++) {
				sceneClusters.Add (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters[c]);
			}

			bool result = false;

			ShuffleVoxelizedClusters();
			for (int q = 0; q < sceneClusters.Count; q++) {

				ShuffleVoxelizedClusters();
				for (int i = 0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++ ) {

					int rotationMode = 0;
			
					if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].fixedRotation) {
						rotationMode = 1;
						if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].rotationFlip) rotationMode = 2;
					}


					if (!voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].freeform) {
						rotationMode = 3;
						ShuffleFourVariants();
						for (int v = 0; v < fourVariants.Length; v++) {


							for (int n = 0 ; n < sceneClusters[q].links.Count; n++) {

								if (!IsBlockActive(sceneClusters[q].links[n])) {
								
									Int3 position = new Int3();
									position.x = sceneClusters[q].links[n].x;
									position.y = sceneClusters[q].links[n].y;
									position.z = sceneClusters[q].links[n].z;
								
							
									if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], position, fourVariants[v], rotationMode, sceneClusters[q].links[n], isPainting, isScattering)) {	

										atLeastOneClusterAdded = true;
										result = true;
										break;
									}	
								}	
							}
							if (result) break;

			
						}
					}

					else {
						ShuffleAllVariants();
						for (int v = 0; v < allVariants.Length; v++) {


							for (int n=0; n < sceneClusters[q].links.Count; n++) {
								if (!IsBlockActive(sceneClusters[q].links[n])) {
							
									Int3 position = new Int3();
									position.x = sceneClusters[q].links[n].x;
									position.y = sceneClusters[q].links[n].y;
									position.z = sceneClusters[q].links[n].z;
							
							
									if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], position, fourVariants[v], rotationMode, sceneClusters[q].links[n], isPainting, isScattering)) {	

										atLeastOneClusterAdded = true;
										result = true;
										break;
									}	
								}	
							}
							if (result) break;


						}
					}


					if (result) break;
			
				}
			
		
			if (result) break;
			}

	
		}

		if (atLeastOneClusterAdded) {
			surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
			surforgeSettings.canIvyOnGlueproof = false;
		}
		else surforgeSettings.canIvyOnGlueproof = true;
	}

	
	public static void VoxelEngineScatter () {
		if (surforgeSettings != null) {
			if (surforgeSettings.isLimitsActive) {
				if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[surforgeSettings.activeSet].haveBorder &&
				    surforgeSettings.isLimitsActive) {
					ScatterByPriority();
				}
				else {
					DefaultScatter ();
				}
			}
		}
	}

	static void DefaultScatter () {
		surforgeSettings.lastAction = 0;

		if (surforgeSettings.layers.Count < 1) CreateNewLayer();

		bool atLeastOneClusterAdded = false;
		List<SurforgeLink> covers = GetFreeCoversFromPreviousLayer();
		if (surforgeSettings.isLimitsActive) covers = AddLimitsCovers(covers);

		if (covers != null) {
			ShuffleCovers(covers); 
			int branchesLeft = surforgeSettings.maxBranches;
			for (int i = covers.Count - 1; i >= 0; i--) {
				if (branchesLeft > 0 ) {
					if (!IsBlockActive(covers[i])) {
					
						Int3 position = new Int3();
						position.x = covers[i].x;
						position.y = covers[i].y;
						position.z = covers[i].z;
					
						if (ScatterNewClusters(position, covers[i], false)) {
					
							branchesLeft--;
							atLeastOneClusterAdded = true;
						}
					
					
					}
				
				}
			}

			if (atLeastOneClusterAdded) {
				surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
				surforgeSettings.canIvyOnGlueproof = false;
			}
			else surforgeSettings.canIvyOnGlueproof = true;
		}
		else {
			Grow();
		}

	}


	static void ScatterByPriority() {
		surforgeSettings.lastAction = 0;
		if (surforgeSettings.layers.Count < 1) CreateNewLayer();

		bool atLeastOneClusterAdded = false;
		List<SurforgeLink> covers = GetFreeCoversFromPreviousLayer();
		if (surforgeSettings.isLimitsActive) covers = AddLimitsCovers(covers);
		
		if (covers != null) {
			ShuffleCovers(covers);

			if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[surforgeSettings.activeSet].pattern) {
				ShuffleVoxelizedClustersPattern();
			}
			else {
				ShuffleVoxelizedClusters();
			}
			int branchesLeft = surforgeSettings.maxBranches;
			for (int c = 0; c < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; c++ ) {

				for (int i = covers.Count - 1; i >= 0; i--) {
					if (branchesLeft > 0 ) {
						if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c].priority < scatterLastPriority) continue;

						if (!IsBlockActive(covers[i])) {

							if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c].mismatchPattern == false) {

								Int3 position = new Int3();
								position.x = covers[i].x;
								position.y = covers[i].y;
								position.z = covers[i].z;
							
								//border center clusters
								if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c].center) {
									if ( !((covers[i].centerX == true) || (covers[i].centerZ == true)) ) continue;
									else {
										int length = 0;
										if (covers[i].centerX == true) length = Mathf.RoundToInt(surforgeSettings.limits.maxX *2 - surforgeSettings.limits.minX *2);
										else length = Mathf.RoundToInt(surforgeSettings.limits.maxZ *2 - surforgeSettings.limits.minZ *2);

										if ( IsNumberEven(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c].length) != IsNumberEven(length) ) {
											continue;
										}
									}
								}


								if (ScatterNewCluster(position, covers[i], false,voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c] )) {

									branchesLeft--;
									atLeastOneClusterAdded = true;
									scatterLastPriority = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[c].priority;

									ShuffleVoxelizedClusters();
								}



							
							}
						}
						
					}
				}
				
	

			}

			if (atLeastOneClusterAdded) {
				surforgeSettings.layers[surforgeSettings.currentLayer].generation++;
				surforgeSettings.canIvyOnGlueproof = false;
			}
			else surforgeSettings.canIvyOnGlueproof = true;
		}
		


	}


	static bool GrowNewClusters(Int3 offset, SurforgeLink link, bool isPainting) {
		bool result = false;
		ShuffleVoxelizedClusters();
		for (int i = 0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++ ) {

			bool isScattering = false;
			int rotationMode = 0;

			if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].fixedRotation) {
				rotationMode = 1;
				if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].rotationFlip) rotationMode = 2;
			}

			if (!voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].freeform) {
				rotationMode = 3;
				ShuffleFourVariants();
				for (int v = 0; v < fourVariants.Length; v++) {
					if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], offset, fourVariants[v], rotationMode, link, isPainting, isScattering)) {
						result = true;
						break;
					}
				}
			}
			else {
				ShuffleAllVariants();
				for (int v = 0; v < allVariants.Length; v++) {
					if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], offset, allVariants[v], rotationMode, link, isPainting, isScattering)) {
						result = true;
						break;
					}
				}
			}

			if (result) break;

		}
		return result;
	}

	static bool ScatterNewCluster(Int3 offset, SurforgeLink link, bool isPainting, SurforgeVoxelizedCluster cluster) {
		bool result = false;
	
			
		bool isScattering = true;
		int rotationMode = 3;
			
		if (!cluster.freeform) {
			if (!cluster.fixedRotation) {
				ShuffleFourVariants();
				for (int v = 0; v < fourVariants.Length; v++) {
					if (VAddNewCluster(cluster, offset, fourVariants[v], rotationMode, link, isPainting, isScattering)) {
						result = true;
						break;
					}
				}
			}
			else {
				if (VAddNewCluster(cluster, offset, 0, rotationMode, link, isPainting, isScattering)) {
					result = true;
				}
			}
		}
		else {
			ShuffleScatterFourVariants();
			for (int v = 0; v < scatterFourVariants.Length; v++ ) {
				if (VAddNewCluster(cluster, offset, scatterFourVariants[v], rotationMode, link, isPainting, isScattering)) {
					result = true;
					break;
				}
			}
		}
		
		return result;
	}


	static bool ScatterNewClusters(Int3 offset, SurforgeLink link, bool isPainting) {
		bool result = false;
		ShuffleVoxelizedClusters();
		for (int i = 0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++ ) {

			bool isScattering = true;
			int rotationMode = 3;

			if (!voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].freeform) {
				if (!voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].fixedRotation) {
					ShuffleFourVariants();
					for (int v = 0; v < fourVariants.Length; v++) {
						if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], offset, fourVariants[v], rotationMode, link, isPainting, isScattering)) {
							result = true;
							break;
						}
					}
				}
				else {
					if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], offset, 0, rotationMode, link, isPainting, isScattering)) {
						result = true;
					}
				}
			}
			else {
				ShuffleScatterFourVariants();
				for (int v = 0; v < scatterFourVariants.Length; v++ ) {
					if (VAddNewCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i], offset, scatterFourVariants[v], rotationMode, link, isPainting, isScattering)) {
						result = true;
						break;
					}
				}
			}

			if (result) break;

		}
		return result;
	}


	static bool VAddNewCluster(SurforgeVoxelizedCluster vCluster, Int3 offset, int variant, 
	                           int rotationMode, SurforgeLink link, bool isPainting, bool isScattering) {
		bool result = false;

		bool ivy = vCluster.ivy;
		if ( (isScattering) && (!vCluster.freeform) ) ivy = false;
		if (vCluster.border) ivy = vCluster.ivy;

		Int3 centerOffset = GetClusterCenterOffset(vCluster.clusterAllRotations[variant]);




		if ( CheckLinkType(vCluster.clusterAllRotations[variant], vCluster, link, isScattering)) { 
			if (MatchRotation(variant, rotationMode, link)) {
				if (CombinedCheck(vCluster, vCluster.clusterAllRotations[variant], offset, centerOffset, ivy, false, null)) {

					int group = 0;
					if (link.isTemporaryLink) {
						surforgeSettings.groupCounter++ ;
						group = surforgeSettings.groupCounter;
					}
					else {
						if (link.node.block) { //was added to fix grow+reroll null reference
							SurforgeSceneCluster sClusterG = (SurforgeSceneCluster)link.node.block.sCluster;
							group = sClusterG.group;
							if (link.node.block.linkType != vCluster.type) {
								surforgeSettings.groupCounter++ ;
								group = surforgeSettings.groupCounter;
							}
						}
					}


					SurforgeSceneCluster sCluster = VAddSceneCluster(vCluster, offset, allRotations[variant], variant, group);
					AddClusterToOctree(vCluster.clusterAllRotations[variant], offset, centerOffset, isPainting, sCluster);
					result = true;
							
				}
			
			}
		}

		return result;
	}

	

	static bool CheckLinkType(SurforgeBlock[,,] cGrid, SurforgeVoxelizedCluster vCluster, SurforgeLink link, bool isScattering) {
		if (isScattering) return true;
		bool result = true;

		if (vCluster.type != "") {
			if (link.node != null) {
				if (link.node.block != null) {
					if ( link.node.block.linkType != "")  {
						if (link.node.block.linkType != vCluster.type) result = false;
					}
				}
			}
		}

		return result;
	}


	

	static void CreateInitialCluster() {
		Int3 position = new Int3();
		position.x = gridOffset;
		position.y = gridOffset;
		position.z = gridOffset;
		
		Int3 centerOffset = GetClusterCenterOffset(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[0].clusterGrid);
		
		SurforgeSceneCluster sCluster = VAddSceneCluster(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[0], 
		                                               position, allRotations[0], 0, surforgeSettings.groupCounter);
		AddClusterToOctree(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[0].clusterGrid, position, centerOffset, false, sCluster);
	}
	


	static SurforgeBlock[,,] CopyClusterGrid(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[,,] newGrid = new SurforgeBlock[cGrid.GetLength(0), cGrid.GetLength(1), cGrid.GetLength(2)];
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {
					SurforgeBlock newBlock = new SurforgeBlock();
					newBlock.isActive = cGrid[x,y,z].isActive;
					newBlock.blockType = cGrid[x,y,z].blockType;
					newBlock.rotation = cGrid[x,y,z].rotation;
					newBlock.linkType = cGrid[x,y,z].linkType;
					newBlock.neverGlueTo = cGrid[x,y,z].neverGlueTo;
					newGrid[x,y,z] = newBlock;
				}
			}
		}
		return newGrid;
	}

	static Int3 GetClusterCenterOffset(SurforgeBlock[,,] cGrid) {
		Int3 offset = new Int3();
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {
					if (cGrid[x,y,z].blockType == 1) {
						offset.x = x;
						offset.y = y;
						offset.z = z;
						return offset;
					}
				}
			}
		}
		return offset;
	}

	
	static SurforgeSceneCluster VAddSceneCluster(SurforgeVoxelizedCluster vCluster, Int3 position, Transform rotationTransform, int variant, int group) {
		GameObject obj = new GameObject();
		obj.AddComponent<SurforgeSceneCluster>();
		SurforgeSceneCluster sCluster = (SurforgeSceneCluster)obj.GetComponent<SurforgeSceneCluster>();

		GameObject model = (GameObject)Instantiate(vCluster.model);
		model.transform.parent = sCluster.transform;
		//model.isStatic = true;

		sCluster.transform.position = new Vector3(position.x - gridOffset, position.y - gridOffset, position.z -gridOffset ) * 0.5f; //* 0.5f _CHANGE
		sCluster.transform.position = sCluster.transform.position + new Vector3(0.25f, 0, 0.25f);
		sCluster.transform.rotation = rotationTransform.rotation;

		sCluster.transform.localScale = sCluster.transform.localScale * 0.5f; // _CHANGE

		sCluster.transform.parent = surforgeSettings.root.transform;
		sCluster.generation = surforgeSettings.layers[surforgeSettings.currentLayer].generation;
		sCluster.vCluster = vCluster;

		//sCluster.position = position;
		sCluster.positionX = position.x;
		sCluster.positionY = position.y;
		sCluster.positionZ = position.z;

		sCluster.variant = variant;
		sCluster.type = vCluster.type;
		sCluster.group = group;
		sCluster.layer = surforgeSettings.currentLayer;

		sceneClusters.Add (sCluster);
		surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Add (sCluster);
		return sCluster;
	}




	static void CreateVoxilizedClusters() {
		voxelizedSets.Clear();
		for (int s = 0; s < surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets.Length; s++) {
			SurforgeVoxelizedSet vSet = new SurforgeVoxelizedSet();
			SurforgeVoxelizedCluster[] vClusters = new SurforgeVoxelizedCluster[surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters.Count];
			vSet.voxelizedClusters = vClusters;

			for (int i = 0; i < surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters.Count; i++) {
				SurforgeVoxelizedCluster vCluster = (SurforgeVoxelizedCluster)ScriptableObject.CreateInstance(typeof(SurforgeVoxelizedCluster));

				SurforgeBlock[,,] cGrid = ClusterToVoxels(surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i]);
				vCluster.clusterGrid = cGrid;
				vCluster.model = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].model;
				vCluster.type = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].type;
				vCluster.fixedRotation = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].fixedRotation;
				vCluster.rotationFlip = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].rotationFlip;
				vCluster.freeform = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].freeform;
				vCluster.ivy = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].ivy;
				vCluster.crosslink = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].crosslink;
				vCluster.border = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].border;
				vCluster.corner = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].corner;
				vCluster.center = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].center;
				vCluster.priority = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].priority;
				vCluster.length = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].length;

				vCluster.clusterPrefab = (MonoBehaviour)surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i]; //save undo test

				if (vCluster.freeform) {
					vCluster.clusterAllRotations = GetClusterGridALLRotations(vCluster.clusterGrid);
				}
				else {
					vCluster.clusterAllRotations = GetClusterGridFourRotations(vCluster.clusterGrid);
				}


				vSet.voxelizedClusters[i] = vCluster;

				if (surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters.Count > 0) {
					SurforgeVoxelizedCluster[] bClusters = new SurforgeVoxelizedCluster[surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters.Count];
					vCluster.voxelizedBevelClusters = bClusters;
					for (int b = 0; b < surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters.Count; b++) {
						SurforgeVoxelizedCluster bCluster = (SurforgeVoxelizedCluster)ScriptableObject.CreateInstance(typeof(SurforgeVoxelizedCluster));

						SurforgeBlock[,,] bGrid = ClusterToVoxels(surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b]);
						bCluster.clusterGrid = bGrid;
						bCluster.model = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].model;
						bCluster.type = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].type;
						bCluster.fixedRotation = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].fixedRotation;
						bCluster.rotationFlip = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].rotationFlip;
						bCluster.freeform = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].freeform;
						bCluster.ivy = surforgeSettings.contentPacks[Surforge.surforgeSettings.activeContentPack].sets[s].clusters[i].bevelClusters[b].ivy;

						if (bCluster.freeform) {
							bCluster.clusterAllRotations = GetClusterGridALLRotations(bCluster.clusterGrid);
						}
						else {
							bCluster.clusterAllRotations = GetClusterGridFourRotations(bCluster.clusterGrid);
						}


						vCluster.voxelizedBevelClusters[b] = bCluster;
					}
				}

			}
			voxelizedSets.Add (vSet);
		}
	}


	
	static void RemoveClusterFromOctree(SurforgeBlock[,,] cGrid, Int3 offset, Int3 centerOffset) {
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {

					if ( !(   ((cGrid[x,y,z].blockType == 0) && (cGrid[x,y,z].isActive == false)) || 
					       (cGrid[x,y,z].blockType == 3) ||
					       (cGrid[x,y,z].blockType == 5) ||
					       (cGrid[x,y,z].blockType == 6) ||
					       (cGrid[x,y,z].blockType == 7)) ) {


						int gX = x + offset.x - centerOffset.x;
						int gY = y + offset.y - centerOffset.y;
						int gZ = z + offset.z - centerOffset.z;

						System.UInt64 morton = (System.UInt64)mortonEncode_LUT((uint)gX, (uint)gY, (uint)gZ);
						int maxLevel = GetMortonMaxLevel();

						SurforgeOctreeNode node = (SurforgeOctreeNode)surforgeSettings.octree.FindChildWithMorton(morton, maxLevel);
					
						if (node != null) {
							if (node.block != null) {

				
								if (cGrid[x,y,z].blockType == 4) {
									if ((node.block.blockType == 4) && (!node.block.isActive)) node.block = null;
								}
								if ((cGrid[x,y,z].blockType == 0) || (cGrid[x,y,z].blockType == 10) || (cGrid[x,y,z].blockType == 1)) {
									if ((node.block.blockType == 2) || (node.block.blockType == 4) ) node.block.isActive = false;
									if ((node.block.blockType == 0) || (node.block.blockType == 10) || (node.block.blockType == 1)) node.block = null;
								}
								if (cGrid[x,y,z].blockType == 2) {
									if (node.block.blockType == 2) {
										if (node.block.wasCover) {
											node.block.blockType = 4;
											node.block.wasCover = false;
											node.block.linkType = "";
											node.block.rotation = 0;
										}
										else node.block = null;
									}
								}


							}
						}

				



					}


				}
			}
		}
	}



	static void AddClusterToOctree(SurforgeBlock[,,] cGrid, Int3 offset, Int3 centerOffset, bool isPainting, SurforgeSceneCluster sCluster) {
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {

					if ( !(   ((cGrid[x,y,z].blockType == 0) && (cGrid[x,y,z].isActive == false)) || 
					          (cGrid[x,y,z].blockType == 3) ||
					          (cGrid[x,y,z].blockType == 5) ||
					          (cGrid[x,y,z].blockType == 6) ||
					      	  (cGrid[x,y,z].blockType == 7)) ) {

						int gX = x + offset.x - centerOffset.x;
						int gY = y + offset.y - centerOffset.y;
						int gZ = z + offset.z - centerOffset.z;

						System.UInt64 morton = (System.UInt64)mortonEncode_LUT((uint)gX, (uint)gY, (uint)gZ);
						int maxLevel = GetMortonMaxLevel();


						SurforgeOctreeNode node = (SurforgeOctreeNode)surforgeSettings.octree.FindChildWithMorton(morton, maxLevel);
						if (node == null) {
							node = surforgeSettings.octree.CreateBranchWithMorton(morton, maxLevel);
							SerializedSurforgeBlock newBlock = (SerializedSurforgeBlock)ScriptableObject.CreateInstance(typeof(SerializedSurforgeBlock));
							node.block = newBlock;
						}
						SerializedSurforgeBlock block = node.block;

						if (block == null) {
							block = (SerializedSurforgeBlock)ScriptableObject.CreateInstance(typeof(SerializedSurforgeBlock));
							node.block = block;
							block.sCluster = sCluster; //assign group for new blocks
						}
						else {
							if (!block.isActive) block.sCluster = sCluster; //assign group for not active existing blocks 
																			//(to provide group for current cluster links)
						}


						if (cGrid[x,y,z].isActive)  {
							block.isActive = true;
							if (cGrid[x,y,z].neverGlueTo) block.neverGlueTo = true;
						}
						
						if (block.blockType != 2 )  {   //never override links. override covers only with links
							//block.linkType = sCluster.type; //comented for match other links solve
							if ((block.blockType == 0) || (block.blockType == 10) || (block.blockType == 1)) {
								block.linkType = cGrid[x,y,z].linkType;
							}

							if (!block.isActive) {
								if (block.blockType == 4) {
									if (cGrid[x,y,z].blockType == 2) {
										block.blockType = cGrid[x,y,z].blockType;
										block.wasCover = true;
									}
								}
								else {
									block.blockType = cGrid[x,y,z].blockType;
								}
							}
						}

						
						if (cGrid[x,y,z].blockType == 2 ) {
							block.rotation = cGrid[x,y,z].rotation;
							block.linkType = cGrid[x,y,z].linkType;
						}
							
						if ((block.blockType == 2) && (!block.isActive)) {

							SurforgeLink link =  (SurforgeLink)ScriptableObject.CreateInstance(typeof(SurforgeLink));
							link.x = gX;
							link.y = gY;
							link.z = gZ;
							
							link.node = node;

							sCluster.links.Add (link);

							if (isPainting) {
								lastPaintedLinks.Add (link);
								if (lastPaintedLinks.Count > 30) lastPaintedLinks.RemoveAt(0);
							}

						}

						if ((block.blockType == 4) && (!block.isActive)) {

							SurforgeLink cover = (SurforgeLink)ScriptableObject.CreateInstance(typeof(SurforgeLink)); 
							cover.x = gX;
							cover.y = gY;
							cover.z = gZ;
							
							cover.node = node;

							sCluster.covers.Add(cover);
						}


					}

				}
			}
		}
	}
	

	static bool CombinedCheck(SurforgeVoxelizedCluster vCluster, SurforgeBlock[,,] cGrid, Int3 offset, 
	                          Int3 centerOffset, bool ivy, bool addBevel, SurforgeSceneCluster oldSCluster) {
		bool singleTrimCheck = false;
		bool canIvy = false;

		if ((addBevel) || (!vCluster.freeform)) {
			singleTrimCheck = true;
			
		}

		if (!ivy) canIvy = true;
		if (surforgeSettings.layers[surforgeSettings.currentLayer].sceneClusters.Count < 4) {
			if ((!vCluster.freeform) &&(!vCluster.border)) canIvy = true;
		}

		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {

					int gX = x + offset.x - centerOffset.x;
					int gY = y + offset.y - centerOffset.y;
					int gZ = z + offset.z - centerOffset.z;
					SerializedSurforgeBlock block = GetBlock(gX, gY, gZ);

					//limits check
					if (surforgeSettings.isLimitsActive) {
						if (cGrid[x,y,z].isActive) {
							if ((cGrid[x,y,z].blockType == 0) || (cGrid[x,y,z].blockType == 1) || (cGrid[x,y,z].blockType == 10)) { 
								if ((gX > surforgeSettings.limits.maxX *2) || (gX < surforgeSettings.limits.minX *2)) return false;
								if ((gY > surforgeSettings.limits.maxY *2) || (gY < surforgeSettings.limits.minY *2)) return false;
								if ((gZ > surforgeSettings.limits.maxZ *2) || (gZ < surforgeSettings.limits.minZ *2)) return false;
							}
						}
					}

					//scatter beyound the edges check
					if ( (!vCluster.freeform) && (surforgeSettings.currentLayer > 0) ) {
						if (cGrid[x,y,z].blockType == 7) {
							if (block == null) return false;
							else {
								if (!block.isActive) return false;
							}
						}
					}

					//have free space for cluster
					if (!addBevel) {
						if (cGrid[x,y,z].isActive) {
							if (block != null) {
								if (block.isActive) return false; 
							}
						
						}
					}


					// match other links
					if (cGrid[x,y,z].blockType == 2) {
						if (block !=null) {
							SurforgeSceneCluster sClusterM = (SurforgeSceneCluster)block.sCluster;
							if ( (sClusterM.vCluster.crosslink) || (vCluster.crosslink) ) {
								if ( (block.isActive) && ((block.blockType == 0) || (block.blockType == 1) || (block.blockType == 10)) ) {
									if (block.linkType != cGrid[x,y,z].linkType) return false;
								}
							}
						}
					}


					//voxels only on cover check
					if (cGrid[x,y,z].blockType == 10) {
						if (block == null) return false;
						if ( !((block.blockType == 4) || (block.blockType == 2)) ) return false;
					}


					if (!addBevel) {
						//glue check

						if (vCluster.border) { //glue to borders check
							if (cGrid[x,y,z].blockType == 5) {
								if (block == null) {
									if ((gX < surforgeSettings.limits.maxX *2) && (gX > surforgeSettings.limits.minX *2) && 
									    (gZ < surforgeSettings.limits.maxZ *2) && (gZ > surforgeSettings.limits.minZ *2)) return false;
								}
								else {
									if ((!block.isActive) || (block.neverGlueTo)) {
										if ((gX < surforgeSettings.limits.maxX *2) && (gX > surforgeSettings.limits.minX *2) && 
										    (gZ < surforgeSettings.limits.maxZ *2) && (gZ > surforgeSettings.limits.minZ *2)) return false;
									}
									if (!sceneClustersBeforeLimitsActivation.Contains((SurforgeSceneCluster)block.sCluster)) return false;
								}
							}
						}

						else {
							if (cGrid[x,y,z].blockType == 5) {
								if (block == null) return false;
								else {
									if ((!block.isActive) || (block.neverGlueTo)) return false;
								}
							}
						}

					
						
						//void check
						if (vCluster.border) { //void to borders check
							if (cGrid[x,y,z].blockType == 6) {
								if (block != null) {
									if (block.isActive) return false;
								}
								if ((gX > surforgeSettings.limits.maxX *2) || (gX < surforgeSettings.limits.minX *2)) return false;
								if ((gY > surforgeSettings.limits.maxY *2) || (gY < surforgeSettings.limits.minY *2)) return false;
								if ((gZ > surforgeSettings.limits.maxZ *2) || (gZ < surforgeSettings.limits.minZ *2)) return false;
							}
						}
						else {
							if (cGrid[x,y,z].blockType == 6) {
								if (block != null) {
									if (block.isActive) return false;
								}
							}
						}
					}

					else {

						//glue check for bevel
						if (cGrid[x,y,z].blockType == 5) {
							if (block == null) return false;
							else {
								SurforgeSceneCluster sClusterG2 = (SurforgeSceneCluster)block.sCluster;
								if ((!block.isActive) || (block.neverGlueTo) || (sClusterG2.group != oldSCluster.group)) return false;
							}
						}


						//void check for bevel
						if (cGrid[x,y,z].blockType == 6) {
							if (block != null) {
								if (block.isActive) {
									SurforgeSceneCluster sClusterG3 = (SurforgeSceneCluster)block.sCluster;
									if (sClusterG3.group == oldSCluster.group) {
										return false;
									}
								}
							}
						}

					}



					//prevent single trim check
					if (surforgeSettings.isLimitsActive) {
						if (!singleTrimCheck) {
							if (cGrid[x,y,z].blockType == 2) {
								if (! ((gX > surforgeSettings.limits.maxX *2) || (gX < surforgeSettings.limits.minX *2)||
								       (gY > surforgeSettings.limits.maxY *2) || (gY < surforgeSettings.limits.minY *2)||
								       (gZ > surforgeSettings.limits.maxZ *2) || (gZ < surforgeSettings.limits.minZ *2)) ) {

									if (block != null) {	
										if  (!block.isActive) {  //never match filled vox with outgoing link
											singleTrimCheck = true; 
										}
									}
									else singleTrimCheck = true;
								}

							}
						}
					}
					else {
						if (!singleTrimCheck) {
							if (cGrid[x,y,z].blockType == 2) {
								if (block != null) {	
									if  (!block.isActive) {  //never match filled vox with outgoing link
										singleTrimCheck = true; 
									}
								}
								else singleTrimCheck = true;
							}
						}
					}

					//ivy connection check
					if (!canIvy) {

						if (cGrid[x,y,z].blockType == 3) {
							if (block != null)  {
								if (block.isActive) {
									if (!block.neverGlueTo) canIvy = true;
									else {
										if (surforgeSettings.canIvyOnGlueproof) canIvy = true;
									}
								}
							}
							if (gY < surforgeSettings.limits.minY *2) {
								canIvy = true;
							}
						}

					}

					
					
				}
			}
		}

		if (!singleTrimCheck) return false;
		if (!canIvy) return false;

		return true;
	}
	


	static SurforgeBlock[,,] ClusterToVoxels(SurforgeCluster cluster) {
		Int3 dimensions = GetClusterDimensions(cluster);
		int size = GetLargestDimension(dimensions);
		Int3 offset = GetClusterOffset(cluster);

		SurforgeBlock[,,] clusterGrid = CreateVoxelGrid(size+1, size+1, size+1);
	
		foreach (Transform v in cluster.voxels) {

			SurforgeRig rig = (SurforgeRig)v.GetComponent<SurforgeRig>();
			int x = Mathf.RoundToInt(v.position.x) + offset.x;
			int y = Mathf.RoundToInt(v.position.y) + offset.y;
			int z = Mathf.RoundToInt(v.position.z) + offset.z;

			if ( (rig.blockType == 0) || (rig.blockType == 10) ) { 
				if (cluster.neverGlueTo) clusterGrid[x,y,z].neverGlueTo = true; //mark voxels "never glue to it"
				clusterGrid[x,y,z].linkType = rig.linkType;
			}

			clusterGrid[x,y,z].isActive = true;

			if (rig.blockType == 2) { //set links
				clusterGrid[x,y,z].blockType = 2;
				clusterGrid[x,y,z].isActive = false;

				//set link rotations
				//int dX = Mathf.RoundToInt(rig.transform.rotation.eulerAngles.x);
				int dY = Mathf.RoundToInt(rig.transform.rotation.eulerAngles.y);
				int dZ = Mathf.RoundToInt(rig.transform.rotation.eulerAngles.z);

				if (  (dZ == 0)    || (dZ == 360)  ) {
					if (  (dY == 0)    || (dY == 360)  ) clusterGrid[x,y,z].rotation = 0;
					if (  (dY == -90)  || (dY == 270)  ) clusterGrid[x,y,z].rotation = 1;
					if (  (dY == -180) || (dY == 180)  ) clusterGrid[x,y,z].rotation = 2;
					if (  (dY == 90)   || (dY == -270) ) clusterGrid[x,y,z].rotation = 3;
				}
				if (  (dZ == 90)   || (dZ == -270) ) {
					if (  (dY == 0)    || (dY == 360)  ) clusterGrid[x,y,z].rotation = 20;
					if (  (dY == -90)  || (dY == 270)  ) clusterGrid[x,y,z].rotation = 23;
					if (  (dY == -180) || (dY == 180)  ) clusterGrid[x,y,z].rotation = 22;
					if (  (dY == 90)   || (dY == -270) ) clusterGrid[x,y,z].rotation = 21;
				}
				//TODO: Reading for all link directions from model


				//Debug.Log("degrees: " +dY.ToString () + " rotation: " + clusterGrid[x,y,z].rotation.ToString ());

				//set link type
				clusterGrid[x,y,z].linkType = rig.linkType;
			
			}
			if (rig.blockType == 3) { //set ivy 
				clusterGrid[x,y,z].blockType = 3;
				clusterGrid[x,y,z].isActive = false;
			}
			if (rig.blockType == 4) { //set cover
				clusterGrid[x,y,z].blockType = 4;
				clusterGrid[x,y,z].isActive = false;
			}
			if (rig.blockType == 5) { //set glue
				clusterGrid[x,y,z].blockType = 5;
				clusterGrid[x,y,z].isActive = false;
			}
			if (rig.blockType == 6) { //set void
				clusterGrid[x,y,z].blockType = 6;
				clusterGrid[x,y,z].isActive = false;
			}
			if (rig.blockType == 7) { //set scatterEdges
				clusterGrid[x,y,z].blockType = 7;
				clusterGrid[x,y,z].isActive = false;
			}
			if (rig.blockType == 10) { //set voxels only instance on covers
				clusterGrid[x,y,z].blockType = 10;
			}



			//zero point allways block type 1 (center)
			if (v.transform.position == Vector3.zero) { //set center
				clusterGrid[x,y,z].blockType = 1;
			}

		}
		return clusterGrid;
	}


	static Int3 GetClusterDimensions(SurforgeCluster cluster) {
		int minX = 0;
		int minY = 0; 
		int minZ = 0;
		int maxX = 0;
		int maxY = 0; 
		int maxZ = 0;
		foreach (Transform v in cluster.voxels) {
			if (Mathf.RoundToInt(v.position.x) < minX) minX = Mathf.RoundToInt(v.position.x);
			if (Mathf.RoundToInt(v.position.y) < minY) minY = Mathf.RoundToInt(v.position.y);
			if (Mathf.RoundToInt(v.position.z) < minZ) minZ = Mathf.RoundToInt(v.position.z);
			if (Mathf.RoundToInt(v.position.x) > maxX) maxX = Mathf.RoundToInt(v.position.x);
			if (Mathf.RoundToInt(v.position.y) > maxY) maxY = Mathf.RoundToInt(v.position.y);
			if (Mathf.RoundToInt(v.position.z) > maxZ) maxZ = Mathf.RoundToInt(v.position.z);
		}
		Int3 result = new Int3();
		result.x = maxX - minX;
		result.y = maxY - minY;
		result.z = maxZ - minZ;
		return result;
	}

	static int GetLargestDimension(Int3 dimensions) {
		int largestSize = dimensions.x;
		if (dimensions.y > largestSize) largestSize = dimensions.y;
		if (dimensions.z > largestSize) largestSize = dimensions.z;
		return largestSize;
	}

	static Int3 GetClusterOffset(SurforgeCluster cluster) {
		int minX = Mathf.RoundToInt(cluster.voxels[0].position.x);
		int minY = Mathf.RoundToInt(cluster.voxels[0].position.y);
		int minZ = Mathf.RoundToInt(cluster.voxels[0].position.z);
		foreach (Transform v in cluster.voxels) {
			if (Mathf.RoundToInt(v.position.x) < minX) minX = Mathf.RoundToInt(v.position.x);
			if (Mathf.RoundToInt(v.position.y) < minY) minY = Mathf.RoundToInt(v.position.y);
			if (Mathf.RoundToInt(v.position.z) < minZ) minZ = Mathf.RoundToInt(v.position.z);
		}
		Int3 result = new Int3();
		result.x = Mathf.Abs (minX);
		result.y = Mathf.Abs (minY);
		result.z = Mathf.Abs (minZ);
		return result;
	}
	
	static SurforgeBlock[,,] CreateVoxelGrid(int xSize, int ySize, int zSize) {
		SurforgeBlock[,,] newGrid = new SurforgeBlock[xSize, ySize, zSize];;

		for(int x = 0; x < xSize; x++) {
			for(int y = 0; y < ySize; y++) {
				for(int z = 0; z < zSize; z++) {
					SurforgeBlock block = new SurforgeBlock();
					newGrid[x,y,z] = block;
				}
			}
		}
		return newGrid;
	}
	
	static void ShuffleFourVariants() {
		for (int i = fourVariants.Length - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			int tmp = fourVariants[i];
			fourVariants[i] = fourVariants[r];
			fourVariants[r] = tmp;
		}
	}

	static void ShuffleScatterFourVariants() {
		for (int i = scatterFourVariants.Length - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			int tmp = scatterFourVariants[i];
			scatterFourVariants[i] = scatterFourVariants[r];
			scatterFourVariants[r] = tmp;
		}
	}

	static void ShuffleAllVariants() {
		for (int i = allVariants.Length - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			int tmp = allVariants[i];
			allVariants[i] = allVariants[r];
			allVariants[r] = tmp;
		}
	}
	

	static void ShuffleVoxelizedClustersPattern() { //for pattern sets (horisontal tiled)
		pattern = new SurforgePattern();
		GetPatternElementsDimensions();
		SetupPattern();

		ShuffleVoxelizedClusters();

		//Debug.Log ("Corner length: " + pattern.cornerLength + " Length: " + pattern.length);
		MarkClustersMismatchPattern();
	}

	static void MarkClustersMismatchPattern() {
		for (int i = 0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++) {
			if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].corner) {
				if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length == pattern.cornerLength) {
					voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].mismatchPattern = false;
				}
				else {
					voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].mismatchPattern = true;
				}
			}
			else {
				if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length == pattern.length) {
					voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].mismatchPattern = false;
				}
				else {
					voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].mismatchPattern = true;
				}
			}
		}
	}

	static void SetupPattern() {
		int limitsLength = Mathf.RoundToInt( surforgeSettings.limits.maxZ *2 - surforgeSettings.limits.minZ *2 );

		for (int i = pattern.cornerMaxLength; i > pattern.cornerMinLength - 1; i--) {
			if (IsPatternMatchLimitsLength(limitsLength, i, i)) {
				pattern.cornerLength = i;
				pattern.length = i;
				break;
			}
			if ((i-1) >= pattern.minLength ) {
				if (IsPatternMatchLimitsLength(limitsLength, i, i-1)) {
					pattern.cornerLength = i;
					pattern.length = i-1;
					break;
				}
			}


			if ((i-2) >= pattern.minLength ) {
				if (IsPatternMatchLimitsLength(limitsLength, i, i-2)) {
					pattern.cornerLength = i;
					pattern.length = i-2;
					break;
				}
			}

			if ((i-3) >= pattern.minLength ) {
				if (IsPatternMatchLimitsLength(limitsLength, i, i-3)) {
					pattern.cornerLength = i;
					pattern.length = i-3;
					break;
				}
			}


			if ((i+1) <= pattern.minLength) {
				if (IsPatternMatchLimitsLength(limitsLength, i, i+1)) {
					pattern.cornerLength = i;
					pattern.length = i+1;
					break;
				}
			}



			if ((i+2) <= pattern.minLength) {
				if (IsPatternMatchLimitsLength(limitsLength, i, i+2)) {
					pattern.cornerLength = i;
					pattern.length = i+2;
					break;
				}
			}


		}

	}

	static bool IsPatternMatchLimitsLength(int limitsLength, int cornerLength, int clusterLength) {
		if ( (cornerLength * 2) > limitsLength ) return false;

		int limitsMinusCorners = limitsLength - cornerLength * 2;
		float modulo = limitsMinusCorners % clusterLength;
		if (modulo == 0) return true;
		else return false;
	}

	static void GetPatternElementsDimensions() {
		pattern.cornerMaxLength = 0;
		pattern.cornerMinLength = 100000;

		pattern.maxLength = 0;
		pattern.minLength = 100000; 

		for (int i=0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++) {
			if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length > 0) {

				if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].corner) {
					if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length < pattern.cornerMinLength) {
						pattern.cornerMinLength = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length;
					}
					if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length > pattern.cornerMaxLength) {
						pattern.cornerMaxLength = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length;
					}
				}

				else {
					if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length < pattern.minLength) {
						pattern.minLength = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length;
					}
					if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length > pattern.maxLength) {
						pattern.maxLength = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].length;
					}
				}

			}
		}

	}

	static void ShuffleVoxelizedClusters() { //with priority
		for (int i = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			SurforgeVoxelizedCluster tmp = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i];
			voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i] = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[r];
			voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[r] = tmp;
		}

		int maxPriority = GetClustersMaxPriority();
		if (maxPriority > 0) QuickSort(voxelizedSets[surforgeSettings.activeSet].voxelizedClusters, 0, 
		                               voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length - 1);

	}


	static void QuickSort(SurforgeVoxelizedCluster[] a, int l, int r) {
		SurforgeVoxelizedCluster temp;
		SurforgeVoxelizedCluster x = a[l + (r - l) / 2];

		int i = l;
		int j = r;

		while (i <= j)
		{
			while (a[i].priority < x.priority) i++;
			while (a[j].priority > x.priority) j--;
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




	static int GetClustersMaxPriority() {
		int maxPriority = 0;
		for (int i=0; i < voxelizedSets[surforgeSettings.activeSet].voxelizedClusters.Length; i++) {
			if (voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].priority > maxPriority) {
				maxPriority = voxelizedSets[surforgeSettings.activeSet].voxelizedClusters[i].priority;
			}
		}
		return maxPriority;
	}

	static void ShuffleLinks(List<SurforgeLink> links) {
		for (int i = links.Count - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			SurforgeLink tmp = links[i];
			links[i] = links[r];
			links[r] = tmp;
		}
	}

	static void ShuffleCovers(List<SurforgeLink> covers) {
		for (int i = covers.Count - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			SurforgeLink tmp = covers[i];
			covers[i] = covers[r];
			covers[r] = tmp;
		}
	}


	static void ShuffleLastPaintedLinks() {
		for (int i = lastPaintedLinks.Count - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			SurforgeLink tmp = lastPaintedLinks[i];
			lastPaintedLinks[i] = lastPaintedLinks[r];
			lastPaintedLinks[r] = tmp;
		}
	}


	static bool IsBlockActive(SurforgeLink link) {
		if (link.isTemporaryLink) { // TODO: check if node present and isActive for temporary links
			if (link.node == null) return false;
		}

		if (link.node.block != null) {
			return link.node.block.isActive;
		}

		return false;
	}








	static bool MatchRotation(int rotation, int rotationMode, SurforgeLink link) {
		if (rotationMode == 0) return MatchRotationFreeform(rotation, link);
		if (rotationMode == 1) return MatchFixedRotations(rotation, link);
		if (rotationMode == 2) return MatchFlipRotations(rotation, link);
		if (rotationMode == 3) return true;

		return false;
	}


	static bool MatchFixedRotations(int rotation, SurforgeLink link) {
		bool match = false;
		if (link.node.block.rotation == rotation) match = true;
		return match;
	}

	static bool MatchFlipRotations(int rotation, SurforgeLink link) {
		bool match = false;

		SerializedSurforgeBlock block = link.node.block;
		
		if ((block.rotation == 0) || 
		    (block.rotation == 12)) {
			if ( (rotation == 0) || (rotation == 12) ) match = true;
		}
		if ((block.rotation == 4) || 
		    (block.rotation == 16)) {
			if ( (rotation == 4) || (rotation == 16) ) match = true;
		}


		if ((block.rotation == 1) || 
		    (block.rotation == 13) ) {
			
			if ( (rotation == 1) || (rotation == 13) ) match = true;
		}
		if ((block.rotation == 9) || 
		    (block.rotation == 23) ) {
			
			if ( (rotation == 9) || (rotation == 23) ) match = true;
		}


		if ((block.rotation == 2) || 
		    (block.rotation == 14))   {
			
			if ( (rotation == 2) ||(rotation == 14) ) match = true;
		}
		if ((block.rotation == 6) || 
		    (block.rotation == 18))   {
			
			if ( (rotation == 6) ||(rotation == 18) ) match = true;
		}

		if ((block.rotation == 3) || 
		    (block.rotation == 15) )   {

			if ( (rotation == 3) || (rotation == 15) ) match = true;
		}
		if ((block.rotation == 11) || 
		    (block.rotation == 21) )   {
		
			if ( (rotation == 11) || (rotation == 21) ) match = true;
		}

		if ((block.rotation == 5) || 
		    (block.rotation == 17) )   {

			if ( (rotation == 5) || (rotation == 17) ) match = true;
		}
		if ((block.rotation == 8) || 
		    (block.rotation == 22) )   {

			if ( (rotation == 8) || (rotation == 22) ) match = true;
		}

		if ((block.rotation == 7) || 
		    (block.rotation == 19) )   {

			if ( (rotation == 7) || (rotation == 19) ) match = true;
		}
		if ((block.rotation == 10) || 
		    (block.rotation == 20) )   {

			if ( (rotation == 10) || (rotation == 20) ) match = true;
		}

		return match;
	}

	
	static bool MatchRotationFreeform(int rotation, SurforgeLink link) {
		bool match = false;

		SerializedSurforgeBlock block = link.node.block;
		if (link.node.block) { //was added to fix grow+reroll null reference
		
			if ((block.rotation == 0) || 
			    (block.rotation == 4) ||
			    (block.rotation == 12) ||
			    (block.rotation == 16))   {
			
				if ( (rotation == 0) || (rotation == 4) || (rotation == 12)|| (rotation == 16) ) match = true;
			}
			if ((block.rotation == 1) || 
			    (block.rotation == 9) ||
			    (block.rotation == 13) ||
			    (block.rotation == 23))   {
			
				if ( (rotation == 1) || (rotation == 9) || (rotation == 13)|| (rotation == 23) ) match = true;
			}
			if ((block.rotation == 2) || 
			    (block.rotation == 6) ||
			    (block.rotation == 14) ||
			    (block.rotation == 18))   {
			
				if ( (rotation == 2) || (rotation == 6) || (rotation == 14)|| (rotation == 18) ) match = true;
			}
			if ((block.rotation == 3) || 
			    (block.rotation == 11) ||
			    (block.rotation == 15) ||
			    (block.rotation == 21))   {
			
				if ( (rotation == 3) || (rotation == 11) || (rotation == 15)|| (rotation == 21) ) match = true;
			}
			if ((block.rotation == 5) || 
			    (block.rotation == 8) ||
			    (block.rotation == 17) ||
			    (block.rotation == 22))   {
			
				if ( (rotation == 5) || (rotation == 8) || (rotation == 17)|| (rotation == 22) ) match = true;
			}
			if ((block.rotation == 7) || 
			    (block.rotation == 10) ||
			    (block.rotation == 19) ||
			    (block.rotation == 20))   {
			
				if ( (rotation == 7) || (rotation == 10) || (rotation == 19)|| (rotation == 20) ) match = true;
			}
		}
		
		return match;
	}


	static SurforgeBlock[][,,] GetClusterGridFourRotations(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[][,,] clusterRotations = new SurforgeBlock[24][,,];
		
		//around +Y
		clusterRotations[0] = cGrid; //0
		
		SurforgeBlock[,,] rotation1y = CopyClusterGrid(cGrid);
		rotation1y = RotateClusterGrid90Y(rotation1y);
		clusterRotations[1] = rotation1y; //1
		
		SurforgeBlock[,,] rotation2y = CopyClusterGrid(rotation1y);
		rotation2y = RotateClusterGrid90Y(rotation2y);
		clusterRotations[2] = rotation2y; //2
		
		SurforgeBlock[,,] rotation3y = CopyClusterGrid(rotation2y);
		rotation3y = RotateClusterGrid90Y(rotation3y);
		clusterRotations[3] = rotation3y; //3

		return clusterRotations;
	}
		

	static SurforgeBlock[][,,] GetClusterGridALLRotations(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[][,,] clusterRotations = new SurforgeBlock[24][,,];
		
		//around +Y
		clusterRotations[0] = cGrid; //0
		
		SurforgeBlock[,,] rotation1y = CopyClusterGrid(cGrid);
		rotation1y = RotateClusterGrid90Y(rotation1y);
		clusterRotations[1] = rotation1y; //1
		
		SurforgeBlock[,,] rotation2y = CopyClusterGrid(rotation1y);
		rotation2y = RotateClusterGrid90Y(rotation2y);
		clusterRotations[2] = rotation2y; //2
		
		SurforgeBlock[,,] rotation3y = CopyClusterGrid(rotation2y);
		rotation3y = RotateClusterGrid90Y(rotation3y);
		clusterRotations[3] = rotation3y; //3
		
		
		//around +Z
		SurforgeBlock[,,] rotation0z = CopyClusterGrid(cGrid);
		rotation0z = RotateClusterGrid90X(rotation0z);
		rotation0z = RotateClusterGrid90X(rotation0z);
		rotation0z = RotateClusterGrid90X(rotation0z);
		clusterRotations[4] = rotation0z; //4
		
		SurforgeBlock[,,] rotation1z = CopyClusterGrid(rotation0z);
		rotation1z = RotateClusterGrid90Z(rotation1z);
		clusterRotations[5] = rotation1z; //5
		
		SurforgeBlock[,,] rotation2z = CopyClusterGrid(rotation1z);
		rotation2z = RotateClusterGrid90Z(rotation2z);
		clusterRotations[6] = rotation2z; //6
		
		SurforgeBlock[,,] rotation3z = CopyClusterGrid(rotation2z);
		rotation3z = RotateClusterGrid90Z(rotation3z);
		clusterRotations[7] = rotation3z; //7
		
		
		//+X
		SurforgeBlock[,,] rotation0x = CopyClusterGrid(cGrid);
		rotation0x = RotateClusterGrid90Z(rotation0x);
		clusterRotations[8] = rotation0x; //8
		
		SurforgeBlock[,,] rotation1x = CopyClusterGrid(rotation0x);
		rotation1x = RotateClusterGrid90X(rotation1x);
		clusterRotations[9] = rotation1x; //9
		
		SurforgeBlock[,,] rotation2x = CopyClusterGrid(rotation1x);
		rotation2x = RotateClusterGrid90X(rotation2x);
		clusterRotations[10] = rotation2x; //10
		
		SurforgeBlock[,,] rotation3x = CopyClusterGrid(rotation2x);
		rotation3x = RotateClusterGrid90X(rotation3x);
		clusterRotations[11] = rotation3x; //11
		
		
		//-Y
		SurforgeBlock[,,] rotation0yM = CopyClusterGrid(cGrid);
		rotation0yM = RotateClusterGrid90X(rotation0yM);
		rotation0yM = RotateClusterGrid90X(rotation0yM);
		clusterRotations[12] = rotation0yM; //12
		
		SurforgeBlock[,,] rotation1yM = CopyClusterGrid(rotation0yM);
		rotation1yM = RotateClusterGrid90Y(rotation1yM);
		clusterRotations[13] = rotation1yM; //13
		
		SurforgeBlock[,,] rotation2yM = CopyClusterGrid(rotation1yM);
		rotation2yM = RotateClusterGrid90Y(rotation2yM);
		clusterRotations[14] = rotation2yM; //14
		
		SurforgeBlock[,,] rotation3yM = CopyClusterGrid(rotation2yM);
		rotation3yM = RotateClusterGrid90Y(rotation3yM);
		clusterRotations[15] = rotation3yM; //15
		
		
		//-Z
		SurforgeBlock[,,] rotation0zM = CopyClusterGrid(cGrid);
		rotation0zM = RotateClusterGrid90X(rotation0zM);
		clusterRotations[16] = rotation0zM; //16
		
		SurforgeBlock[,,] rotation1zM = CopyClusterGrid(rotation0zM);
		rotation1zM = RotateClusterGrid90Z(rotation1zM);
		clusterRotations[17] = rotation1zM; //17
		
		SurforgeBlock[,,] rotation2zM = CopyClusterGrid(rotation1zM);
		rotation2zM = RotateClusterGrid90Z(rotation2zM);
		clusterRotations[18] = rotation2zM; //18
		
		SurforgeBlock[,,] rotation3zM = CopyClusterGrid(rotation2zM);
		rotation3zM = RotateClusterGrid90Z(rotation3zM);
		clusterRotations[19] = rotation3zM; //19
		
		
		//-X
		SurforgeBlock[,,] rotation0xM = CopyClusterGrid(cGrid);
		rotation0xM = RotateClusterGrid90Z(rotation0xM);
		rotation0xM = RotateClusterGrid90Z(rotation0xM);
		rotation0xM = RotateClusterGrid90Z(rotation0xM);
		clusterRotations[20] = rotation0xM; //20
		
		SurforgeBlock[,,] rotation1xM = CopyClusterGrid(rotation0xM);
		rotation1xM = RotateClusterGrid90X(rotation1xM);
		clusterRotations[21] = rotation1xM; //21
		
		SurforgeBlock[,,] rotation2xM = CopyClusterGrid(rotation1xM);
		rotation2xM = RotateClusterGrid90X(rotation2xM);
		clusterRotations[22] = rotation2xM; //22
		
		SurforgeBlock[,,] rotation3xM = CopyClusterGrid(rotation2xM);
		rotation3xM = RotateClusterGrid90X(rotation3xM);
		clusterRotations[23] = rotation3xM; //23
		
		
		return clusterRotations;
	}
	
	
	static void CreateAllRotations() {
		allRotations.Clear();
		
		//+Y
		Transform y0 = new GameObject().transform;
		allRotations.Add (y0); //0
		
		Transform y1 = new GameObject().transform;
		y1.Rotate(Vector3.up, -90.0f);
		allRotations.Add (y1); //1
		
		Transform y2 = new GameObject().transform;
		y2.Rotate(Vector3.up, -180.0f);
		allRotations.Add (y2); //2
		
		Transform y3 = new GameObject().transform;
		y3.Rotate(Vector3.up, -270.0f);
		allRotations.Add (y3); //3
		
		
		//+Z
		Transform z0 = new GameObject().transform;
		z0.Rotate(Vector3.right, -270.0f);
		allRotations.Add (z0); //4
		
		Transform z1 = new GameObject().transform;
		z1.Rotate(Vector3.right, -270.0f);
		z1.Rotate(Vector3.up, -90.0f);
		allRotations.Add (z1); //5
		
		Transform z2 = new GameObject().transform;
		z2.Rotate(Vector3.right, -270.0f);
		z2.Rotate(Vector3.up, -180.0f);
		allRotations.Add (z2); //6
		
		Transform z3 = new GameObject().transform;
		z3.Rotate(Vector3.right, -270.0f);
		z3.Rotate(Vector3.up, -270.0f);
		allRotations.Add (z3); //7
		
		
		//+X
		Transform x0 = new GameObject().transform;
		x0.Rotate(Vector3.forward, -90.0f);
		allRotations.Add (x0); //8
		
		Transform x1 = new GameObject().transform;
		x1.Rotate(Vector3.forward, -90.0f);
		x1.Rotate(Vector3.up, -90.0f);
		allRotations.Add (x1); //9
		
		Transform x2 = new GameObject().transform;
		x2.Rotate(Vector3.forward, -90.0f);
		x2.Rotate(Vector3.up, -180.0f);
		allRotations.Add (x2); //10
		
		Transform x3 = new GameObject().transform;
		x3.Rotate(Vector3.forward, -90.0f);
		x3.Rotate(Vector3.up, -270.0f);
		allRotations.Add (x3); //11
		
		
		//-Y
		Transform y0m = new GameObject().transform;
		y0m.Rotate(Vector3.right, -180.0f);
		allRotations.Add (y0m); //12
		
		Transform y1m = new GameObject().transform;
		y1m.Rotate(Vector3.right, -180.0f);
		y1m.Rotate(Vector3.down, -90.0f);
		allRotations.Add (y1m); //13
		
		Transform y2m = new GameObject().transform;
		y2m.Rotate(Vector3.right, -180.0f);
		y2m.Rotate(Vector3.down, -180.0f);
		allRotations.Add (y2m); //14
		
		Transform y3m = new GameObject().transform;
		y3m.Rotate(Vector3.right, -180.0f);
		y3m.Rotate(Vector3.down, -270.0f);
		allRotations.Add (y3m); //15
		
		
		//-Z
		Transform z0m = new GameObject().transform;
		z0m.Rotate(Vector3.left, -270.0f);
		allRotations.Add (z0m); //16
		
		Transform z1m = new GameObject().transform;
		z1m.Rotate(Vector3.left, -270.0f);
		z1m.Rotate(Vector3.down, -90.0f);
		allRotations.Add (z1m); //17
		
		Transform z2m = new GameObject().transform;
		z2m.Rotate(Vector3.left, -270.0f);
		z2m.Rotate(Vector3.down, -180.0f);
		allRotations.Add (z2m); //18
		
		Transform z3m = new GameObject().transform;
		z3m.Rotate(Vector3.left, -270.0f);
		z3m.Rotate(Vector3.down, -270.0f);
		allRotations.Add (z3m); //19
		
		
		//-X
		Transform x0m = new GameObject().transform;
		x0m.Rotate(Vector3.back, -90.0f);
		allRotations.Add (x0m); //20
		
		Transform x1m = new GameObject().transform;
		x1m.Rotate(Vector3.back, -90.0f);
		x1m.Rotate(Vector3.down, -90.0f);
		allRotations.Add (x1m); //21
		
		Transform x2m = new GameObject().transform;
		x2m.Rotate(Vector3.back, -90.0f);
		x2m.Rotate(Vector3.down, -180.0f);
		allRotations.Add (x2m); //22
		
		Transform x3m = new GameObject().transform;
		x3m.Rotate(Vector3.back, -90.0f);
		x3m.Rotate(Vector3.down, -270.0f);
		allRotations.Add (x3m); //23
		
	}


	static SurforgeBlock[,,] RotateClusterGrid90Y(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[,,] rotatedGrid = new SurforgeBlock[cGrid.GetLength(0), cGrid.GetLength(1), cGrid.GetLength(2)];
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {
					rotatedGrid[cGrid.GetLength(0) - 1 -x,y,z] = cGrid[z,y,x];
					
					//link rotations
					if(cGrid[x,y,z].blockType == 2) {
						bool rotated = false;
						if ((cGrid[x,y,z].rotation == 0) && (!rotated)) {
							cGrid[x,y,z].rotation = 1;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 1) && (!rotated)) {
							cGrid[x,y,z].rotation = 2;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 2) && (!rotated)) {
							cGrid[x,y,z].rotation = 3;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 3) && (!rotated)) {
							cGrid[x,y,z].rotation = 0;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 4) && (!rotated)) {
							cGrid[x,y,z].rotation = 23;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 5) && (!rotated)) {
							cGrid[x,y,z].rotation = 22;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 6) && (!rotated)) {
							cGrid[x,y,z].rotation = 21;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 7) && (!rotated)) {
							cGrid[x,y,z].rotation = 20;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 8) && (!rotated)) {
							cGrid[x,y,z].rotation = 5;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 9) && (!rotated)) {
							cGrid[x,y,z].rotation = 6;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 10) && (!rotated)) {
							cGrid[x,y,z].rotation = 7;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 11) && (!rotated)) {
							cGrid[x,y,z].rotation = 4;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 12) && (!rotated)) {
							cGrid[x,y,z].rotation = 13;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 13) && (!rotated)) {
							cGrid[x,y,z].rotation = 14;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 14) && (!rotated)) {
							cGrid[x,y,z].rotation = 15;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 15) && (!rotated)) {
							cGrid[x,y,z].rotation = 12;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 16) && (!rotated)) {
							cGrid[x,y,z].rotation = 9;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 17) && (!rotated)) {
							cGrid[x,y,z].rotation = 8;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 18) && (!rotated)) {
							cGrid[x,y,z].rotation = 11;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 19) && (!rotated)) {
							cGrid[x,y,z].rotation = 10;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 20) && (!rotated)) {
							cGrid[x,y,z].rotation = 19;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 21) && (!rotated)) {
							cGrid[x,y,z].rotation = 16;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 22) && (!rotated)) {
							cGrid[x,y,z].rotation = 17;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 23) && (!rotated)) {
							cGrid[x,y,z].rotation = 18;
							rotated = true;
						}
					}
					
						
				}
			}
		}
		return rotatedGrid;
	}
	
	static SurforgeBlock[,,] RotateClusterGrid90Z(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[,,] rotatedGrid = new SurforgeBlock[cGrid.GetLength(0), cGrid.GetLength(1), cGrid.GetLength(2)];
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {
					rotatedGrid[x, cGrid.GetLength(0) - 1 -y, z] = cGrid[y,x,z];

					//link rotations
					if(cGrid[x,y,z].blockType == 2) {
						bool rotated = false;
						if ((cGrid[x,y,z].rotation == 0) && (!rotated)) {
							cGrid[x,y,z].rotation = 8;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 1) && (!rotated)) {
							cGrid[x,y,z].rotation = 9;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 2) && (!rotated)) {
							cGrid[x,y,z].rotation = 10;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 3) && (!rotated)) {
							cGrid[x,y,z].rotation = 11;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 4) && (!rotated)) {
							cGrid[x,y,z].rotation = 5;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 5) && (!rotated)) {
							cGrid[x,y,z].rotation = 6;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 6) && (!rotated)) {
							cGrid[x,y,z].rotation = 7;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 7) && (!rotated)) {
							cGrid[x,y,z].rotation = 4;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 8) && (!rotated)) {
							cGrid[x,y,z].rotation = 14;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 9) && (!rotated)) {
							cGrid[x,y,z].rotation = 13;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 10) && (!rotated)) {
							cGrid[x,y,z].rotation = 12;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 11) && (!rotated)) {
							cGrid[x,y,z].rotation = 15;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 12) && (!rotated)) {
							cGrid[x,y,z].rotation = 22;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 13) && (!rotated)) {
							cGrid[x,y,z].rotation = 23;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 14) && (!rotated)) {
							cGrid[x,y,z].rotation = 20;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 15) && (!rotated)) {
							cGrid[x,y,z].rotation = 21;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 16) && (!rotated)) {
							cGrid[x,y,z].rotation = 17;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 17) && (!rotated)) {
							cGrid[x,y,z].rotation = 18;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 18) && (!rotated)) {
							cGrid[x,y,z].rotation = 19;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 19) && (!rotated)) {
							cGrid[x,y,z].rotation = 16;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 20) && (!rotated)) {
							cGrid[x,y,z].rotation = 0;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 21) && (!rotated)) {
							cGrid[x,y,z].rotation = 3;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 22) && (!rotated)) {
							cGrid[x,y,z].rotation = 2;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 23) && (!rotated)) {
							cGrid[x,y,z].rotation = 1;
							rotated = true;
						}
					}
					

				}
			}
		}
		return rotatedGrid;
	}
	
	static SurforgeBlock[,,] RotateClusterGrid90X(SurforgeBlock[,,] cGrid) {
		SurforgeBlock[,,] rotatedGrid = new SurforgeBlock[cGrid.GetLength(0), cGrid.GetLength(1), cGrid.GetLength(2)];
		for(int x = 0; x < cGrid.GetLength(0); x++) {
			for(int y = 0; y < cGrid.GetLength(1); y++) {
				for(int z = 0; z < cGrid.GetLength(2); z++) {
					rotatedGrid[x, y, cGrid.GetLength(0) - 1 -z] = cGrid[x,z,y];
					
					//link rotations
					if(cGrid[x,y,z].blockType == 2) {
						bool rotated = false;
						if ((cGrid[x,y,z].rotation == 0) && (!rotated)) {
							cGrid[x,y,z].rotation = 16;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 1) && (!rotated)) {
							cGrid[x,y,z].rotation = 19;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 2) && (!rotated)) {
							cGrid[x,y,z].rotation = 18;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 3) && (!rotated)) {
							cGrid[x,y,z].rotation = 17;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 4) && (!rotated)) {
							cGrid[x,y,z].rotation = 0;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 5) && (!rotated)) {
							cGrid[x,y,z].rotation = 1;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 6) && (!rotated)) {
							cGrid[x,y,z].rotation = 2;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 7) && (!rotated)) {
							cGrid[x,y,z].rotation = 3;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 8) && (!rotated)) {
							cGrid[x,y,z].rotation = 9;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 9) && (!rotated)) {
							cGrid[x,y,z].rotation = 10;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 10) && (!rotated)) {
							cGrid[x,y,z].rotation = 11;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 11) && (!rotated)) {
							cGrid[x,y,z].rotation = 8;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 12) && (!rotated)) {
							cGrid[x,y,z].rotation = 4;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 13) && (!rotated)) {
							cGrid[x,y,z].rotation = 7;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 14) && (!rotated)) {
							cGrid[x,y,z].rotation = 6;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 15) && (!rotated)) {
							cGrid[x,y,z].rotation = 5;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 16) && (!rotated)) {
							cGrid[x,y,z].rotation = 12;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 17) && (!rotated)) {
							cGrid[x,y,z].rotation = 13;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 18) && (!rotated)) {
							cGrid[x,y,z].rotation = 14;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 19) && (!rotated)) {
							cGrid[x,y,z].rotation = 15;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 20) && (!rotated)) {
							cGrid[x,y,z].rotation = 21;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 21) && (!rotated)) {
							cGrid[x,y,z].rotation = 22;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 22) && (!rotated)) {
							cGrid[x,y,z].rotation = 23;
							rotated = true;
						}
						if ((cGrid[x,y,z].rotation == 23) && (!rotated)) {
							cGrid[x,y,z].rotation = 20;
							rotated = true;
						}
					}

				}
			}
		}
		return rotatedGrid;
	}


	public static void ToggleBrushActive() {
		if (surforgeSettings.isBrushActive) surforgeSettings.isBrushActive = false;
		else surforgeSettings.isBrushActive = true;
	}

	public static Tool lastUsedTool = Tool.None;

	public static void TogglePolygonLassoActive() {
		if (surforgeSettings.isPolygonLassoActive) {
			LogAction("Poly Lasso: exit", "Esc", "A");
			surforgeSettings.isPolygonLassoActive = false;
			DestroyPolygonLassoPlane();

			Tools.current = lastUsedTool;
		}
		else {
			LogAction("Poly Lasso", "A", "");
			surforgeSettings.isPolygonLassoActive = true;
			CreatePolygonLassoPlane();

			if (Tools.current != Tool.None) lastUsedTool = Tools.current;
			Tools.current = Tool.None;
		}
	}

	public static void TogglePlaceToolActive() {
		if (surforgeSettings.isPlaceToolActive) {
			LogAction("Add Detail: exit", "Esc","D");
			surforgeSettings.isPlaceToolActive = false;
			DestroyPlaceToolPlane();
			DestroyPlaceToolPreview();

			Tools.current = lastUsedTool;
		}
		else {
			LogAction("Add Detail", "D", "");
			surforgeSettings.isPlaceToolActive = true;
			CreatePlaceToolPlane();
			CreatePlaceToolPreview();
			PlaceToolGenerateTexts();
			CreatePlaceToolSymmetryPreviews();

			if (Tools.current != Tool.None) lastUsedTool = Tools.current;
			Tools.current = Tool.None;
		}
	}

	public static void DeactivatePlaceTool() {
		surforgeSettings.isPlaceToolActive = false;
		DestroyPlaceToolPlane();
		DestroyPlaceToolPreview();
	}

	public static void DeactivatePolygonLassoTool() {
		surforgeSettings.isPolygonLassoActive = false;
		DestroyPolygonLassoPlane();
	}

	public static void ToggleLimitsToolActive() {
		if (surforgeSettings.isLimitsActive) {
			LogAction("Greebles: exit", "Esc", "G");
			surforgeSettings.isLimitsActive = false;
			scatterLastPriority = 0;

			Tools.current = lastUsedTool;
		}
		else {
			LogAction("Greebles", "G", "");
			if (surforgeSettings.limits == null) CreateLimits();
			surforgeSettings.isLimitsActive = true;
			CopyLimitsForCompare();
			AddSceneClustersBeforeLimitsActivationToArray();

			if (Tools.current != Tool.None) lastUsedTool = Tools.current;
			Tools.current = Tool.None;
		}
	}

	public static void DeactivateLimitsTool() {
		surforgeSettings.isLimitsActive = false;
		scatterLastPriority = 0;
	}

	public static void CheckLimitsChanged() {
		if (limitsForCompare != null) {
			if ((limitsForCompare.maxX != surforgeSettings.limits.maxX) || (limitsForCompare.minX != surforgeSettings.limits.minX) || 
			    (limitsForCompare.maxY != surforgeSettings.limits.maxY) || (limitsForCompare.minY != surforgeSettings.limits.minY) ||
			    (limitsForCompare.maxZ != surforgeSettings.limits.maxZ) || (limitsForCompare.minZ != surforgeSettings.limits.minZ) ) {
				scatterLastPriority = 0;
				CopyLimitsForCompare();
				AddSceneClustersBeforeLimitsActivationToArray();
				UpdatePolygonLassoPlaneTransform();
				UpdatePlaceToolPlaneTransform();
			}
		}
		else {
			CopyLimitsForCompare();
		}
	}

	static void CopyLimitsForCompare() {
		limitsForCompare = (SurforgeLimits)ScriptableObject.CreateInstance(typeof(SurforgeLimits));
		limitsForCompare.maxX = surforgeSettings.limits.maxX;
		limitsForCompare.minX = surforgeSettings.limits.minX;
		limitsForCompare.maxY = surforgeSettings.limits.maxY;
		limitsForCompare.minY = surforgeSettings.limits.minY;
		limitsForCompare.maxZ = surforgeSettings.limits.maxZ;
		limitsForCompare.minZ = surforgeSettings.limits.minZ;
	}

	public static void AddSceneClustersBeforeLimitsActivationToArray() {
		sceneClustersBeforeLimitsActivation = new List<SurforgeSceneCluster>();
		foreach (Transform child in surforgeSettings.root.transform) {
			SurforgeSceneCluster sCluster = (SurforgeSceneCluster)child.gameObject.GetComponent<SurforgeSceneCluster>();
			if (sCluster != null) {
				sceneClustersBeforeLimitsActivation.Add (sCluster);
			}
		}
	}

	public static void CreateLimits() {
		surforgeSettings.limits = (SurforgeLimits)ScriptableObject.CreateInstance(typeof(SurforgeLimits));
		surforgeSettings.limits.maxX = 7.75f;
		surforgeSettings.limits.minX = -8.0f;

		surforgeSettings.limits.maxY = 0.75f;
		surforgeSettings.limits.minY = -0.25f;

		surforgeSettings.limits.maxZ = 7.75f;
		surforgeSettings.limits.minZ = -8.0f;
	}

	static void CreateTextureBorders() {
		surforgeSettings.textureBorders = (SurforgeLimits)ScriptableObject.CreateInstance(typeof(SurforgeLimits));

		surforgeSettings.textureBorders.maxX = 32.0f;
		surforgeSettings.textureBorders.minX = -32.0f;

		surforgeSettings.textureBorders.maxZ = 32.0f;
		surforgeSettings.textureBorders.minZ = -32.0f;

		surforgeSettings.textureBorders.maxY = -0.25f;
		surforgeSettings.textureBorders.minY = -0.25f;


	}

	
	static bool IsNumberEven(int number) {
		if (number %2==0) return true;
		else return false;
	}





	// OCTREE

	static int maxDepth = 8;

	//Debug octree gizmos
	//static OctreeGizmosDraw octreeGizmos;
	

	static void CreateOctree() {

		gridOffset = 0;

		surforgeSettings.octree = (SurforgeOctree)ScriptableObject.CreateInstance(typeof(SurforgeOctree));  
		surforgeSettings.octree.center = new Vector3(-0.5f,-0.5f, -0.5f);
		surforgeSettings.octree.boundsOffsetTable = new Vector3[8]
		{
			new Vector3 (0.5f,  0.5f,  0.5f),
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f)
		};

		surforgeSettings.octree.SetHalfSizeByMaxDepth(maxDepth);
		surforgeSettings.octree.Split();


		//Debug octree gizmos
		//octreeGizmos = (OctreeGizmosDraw)Instantiate ( surforgeSettings.octreeDrawPrefab);
		//octreeGizmos.octree = surforgeSettings.octree;
	}



	//Morton
	static System.UInt64 SplitBy3 (uint a) {
		System.UInt64 x = a & 0x1fffff; // we only look at the first 21 bits
		x = (x | x << 32) & 0x1f00000000ffff;  // shift left 32 bits, OR with self, and 00011111000000000000000000000000000000001111111111111111
		x = (x | x << 16) & 0x1f0000ff0000ff;  // shift left 32 bits, OR with self, and 00011111000000000000000011111111000000000000000011111111
		x = (x | x << 8)  & 0x100f00f00f00f00f; // shift left 32 bits, OR with self, and 0001000000001111000000001111000000001111000000001111000000000000
		x = (x | x << 4)  & 0x10c30c30c30c30c3; // shift left 32 bits, OR with self, and 0001000011000011000011000011000011000011000011000011000100000000
		x = (x | x << 2)  & 0x1249249249249249;
		return x;
	}

	static System.UInt64 MortonEncodeMagicbits( uint x, uint y, uint z) {
		System.UInt64 answer = 0;
		answer |= SplitBy3(x) | SplitBy3(y) << 1 | SplitBy3(z) << 2; 
		return answer;
	}

	static SerializedSurforgeBlock GetBlock(int x, int y, int z) {
		System.UInt64 morton = (System.UInt64)mortonEncode_LUT((uint) x, (uint) y, (uint) z);
		int maxLevel = GetMortonMaxLevel();
		SurforgeOctreeNode o = surforgeSettings.octree.FindChildWithMorton(morton, maxLevel);
		if (o != null) {
			return o.block;
		}
		else return null;
	}



	/*
	[MenuItem ("Surforge/Octree/Debug all mortons in tree")]
	static void DebugAllMortonsTest() {
		surforgeSettings.octree.DebugAllMortons(GetMortonMaxLevel());
	
	}
	*/

	static int GetMortonMaxLevel() {
		return maxDepth;
	}

	


	//Morton LUT method 

	static uint[] morton256_x = new uint[256]
	{
		0x00000000,
		0x00000001, 0x00000008, 0x00000009, 0x00000040, 0x00000041, 0x00000048, 0x00000049, 0x00000200,
		0x00000201, 0x00000208, 0x00000209, 0x00000240, 0x00000241, 0x00000248, 0x00000249, 0x00001000,
		0x00001001, 0x00001008, 0x00001009, 0x00001040, 0x00001041, 0x00001048, 0x00001049, 0x00001200,
		0x00001201, 0x00001208, 0x00001209, 0x00001240, 0x00001241, 0x00001248, 0x00001249, 0x00008000,
		0x00008001, 0x00008008, 0x00008009, 0x00008040, 0x00008041, 0x00008048, 0x00008049, 0x00008200,
		0x00008201, 0x00008208, 0x00008209, 0x00008240, 0x00008241, 0x00008248, 0x00008249, 0x00009000,
		0x00009001, 0x00009008, 0x00009009, 0x00009040, 0x00009041, 0x00009048, 0x00009049, 0x00009200,
		0x00009201, 0x00009208, 0x00009209, 0x00009240, 0x00009241, 0x00009248, 0x00009249, 0x00040000,
		0x00040001, 0x00040008, 0x00040009, 0x00040040, 0x00040041, 0x00040048, 0x00040049, 0x00040200,
		0x00040201, 0x00040208, 0x00040209, 0x00040240, 0x00040241, 0x00040248, 0x00040249, 0x00041000,
		0x00041001, 0x00041008, 0x00041009, 0x00041040, 0x00041041, 0x00041048, 0x00041049, 0x00041200,
		0x00041201, 0x00041208, 0x00041209, 0x00041240, 0x00041241, 0x00041248, 0x00041249, 0x00048000,
		0x00048001, 0x00048008, 0x00048009, 0x00048040, 0x00048041, 0x00048048, 0x00048049, 0x00048200,
		0x00048201, 0x00048208, 0x00048209, 0x00048240, 0x00048241, 0x00048248, 0x00048249, 0x00049000,
		0x00049001, 0x00049008, 0x00049009, 0x00049040, 0x00049041, 0x00049048, 0x00049049, 0x00049200,
		0x00049201, 0x00049208, 0x00049209, 0x00049240, 0x00049241, 0x00049248, 0x00049249, 0x00200000,
		0x00200001, 0x00200008, 0x00200009, 0x00200040, 0x00200041, 0x00200048, 0x00200049, 0x00200200,
		0x00200201, 0x00200208, 0x00200209, 0x00200240, 0x00200241, 0x00200248, 0x00200249, 0x00201000,
		0x00201001, 0x00201008, 0x00201009, 0x00201040, 0x00201041, 0x00201048, 0x00201049, 0x00201200,
		0x00201201, 0x00201208, 0x00201209, 0x00201240, 0x00201241, 0x00201248, 0x00201249, 0x00208000,
		0x00208001, 0x00208008, 0x00208009, 0x00208040, 0x00208041, 0x00208048, 0x00208049, 0x00208200,
		0x00208201, 0x00208208, 0x00208209, 0x00208240, 0x00208241, 0x00208248, 0x00208249, 0x00209000,
		0x00209001, 0x00209008, 0x00209009, 0x00209040, 0x00209041, 0x00209048, 0x00209049, 0x00209200,
		0x00209201, 0x00209208, 0x00209209, 0x00209240, 0x00209241, 0x00209248, 0x00209249, 0x00240000,
		0x00240001, 0x00240008, 0x00240009, 0x00240040, 0x00240041, 0x00240048, 0x00240049, 0x00240200,
		0x00240201, 0x00240208, 0x00240209, 0x00240240, 0x00240241, 0x00240248, 0x00240249, 0x00241000,
		0x00241001, 0x00241008, 0x00241009, 0x00241040, 0x00241041, 0x00241048, 0x00241049, 0x00241200,
		0x00241201, 0x00241208, 0x00241209, 0x00241240, 0x00241241, 0x00241248, 0x00241249, 0x00248000,
		0x00248001, 0x00248008, 0x00248009, 0x00248040, 0x00248041, 0x00248048, 0x00248049, 0x00248200,
		0x00248201, 0x00248208, 0x00248209, 0x00248240, 0x00248241, 0x00248248, 0x00248249, 0x00249000,
		0x00249001, 0x00249008, 0x00249009, 0x00249040, 0x00249041, 0x00249048, 0x00249049, 0x00249200,
		0x00249201, 0x00249208, 0x00249209, 0x00249240, 0x00249241, 0x00249248, 0x00249249
	};

	// pre-shifted table for Y coordinates (1 bit to the left)
	static uint[] morton256_y = new uint[256]
	{
		0x00000000,
		0x00000002, 0x00000010, 0x00000012, 0x00000080, 0x00000082, 0x00000090, 0x00000092, 0x00000400,
		0x00000402, 0x00000410, 0x00000412, 0x00000480, 0x00000482, 0x00000490, 0x00000492, 0x00002000,
		0x00002002, 0x00002010, 0x00002012, 0x00002080, 0x00002082, 0x00002090, 0x00002092, 0x00002400,
		0x00002402, 0x00002410, 0x00002412, 0x00002480, 0x00002482, 0x00002490, 0x00002492, 0x00010000,
		0x00010002, 0x00010010, 0x00010012, 0x00010080, 0x00010082, 0x00010090, 0x00010092, 0x00010400,
		0x00010402, 0x00010410, 0x00010412, 0x00010480, 0x00010482, 0x00010490, 0x00010492, 0x00012000,
		0x00012002, 0x00012010, 0x00012012, 0x00012080, 0x00012082, 0x00012090, 0x00012092, 0x00012400,
		0x00012402, 0x00012410, 0x00012412, 0x00012480, 0x00012482, 0x00012490, 0x00012492, 0x00080000,
		0x00080002, 0x00080010, 0x00080012, 0x00080080, 0x00080082, 0x00080090, 0x00080092, 0x00080400,
		0x00080402, 0x00080410, 0x00080412, 0x00080480, 0x00080482, 0x00080490, 0x00080492, 0x00082000,
		0x00082002, 0x00082010, 0x00082012, 0x00082080, 0x00082082, 0x00082090, 0x00082092, 0x00082400,
		0x00082402, 0x00082410, 0x00082412, 0x00082480, 0x00082482, 0x00082490, 0x00082492, 0x00090000,
		0x00090002, 0x00090010, 0x00090012, 0x00090080, 0x00090082, 0x00090090, 0x00090092, 0x00090400,
		0x00090402, 0x00090410, 0x00090412, 0x00090480, 0x00090482, 0x00090490, 0x00090492, 0x00092000,
		0x00092002, 0x00092010, 0x00092012, 0x00092080, 0x00092082, 0x00092090, 0x00092092, 0x00092400,
		0x00092402, 0x00092410, 0x00092412, 0x00092480, 0x00092482, 0x00092490, 0x00092492, 0x00400000,
		0x00400002, 0x00400010, 0x00400012, 0x00400080, 0x00400082, 0x00400090, 0x00400092, 0x00400400,
		0x00400402, 0x00400410, 0x00400412, 0x00400480, 0x00400482, 0x00400490, 0x00400492, 0x00402000,
		0x00402002, 0x00402010, 0x00402012, 0x00402080, 0x00402082, 0x00402090, 0x00402092, 0x00402400,
		0x00402402, 0x00402410, 0x00402412, 0x00402480, 0x00402482, 0x00402490, 0x00402492, 0x00410000,
		0x00410002, 0x00410010, 0x00410012, 0x00410080, 0x00410082, 0x00410090, 0x00410092, 0x00410400,
		0x00410402, 0x00410410, 0x00410412, 0x00410480, 0x00410482, 0x00410490, 0x00410492, 0x00412000,
		0x00412002, 0x00412010, 0x00412012, 0x00412080, 0x00412082, 0x00412090, 0x00412092, 0x00412400,
		0x00412402, 0x00412410, 0x00412412, 0x00412480, 0x00412482, 0x00412490, 0x00412492, 0x00480000,
		0x00480002, 0x00480010, 0x00480012, 0x00480080, 0x00480082, 0x00480090, 0x00480092, 0x00480400,
		0x00480402, 0x00480410, 0x00480412, 0x00480480, 0x00480482, 0x00480490, 0x00480492, 0x00482000,
		0x00482002, 0x00482010, 0x00482012, 0x00482080, 0x00482082, 0x00482090, 0x00482092, 0x00482400,
		0x00482402, 0x00482410, 0x00482412, 0x00482480, 0x00482482, 0x00482490, 0x00482492, 0x00490000,
		0x00490002, 0x00490010, 0x00490012, 0x00490080, 0x00490082, 0x00490090, 0x00490092, 0x00490400,
		0x00490402, 0x00490410, 0x00490412, 0x00490480, 0x00490482, 0x00490490, 0x00490492, 0x00492000,
		0x00492002, 0x00492010, 0x00492012, 0x00492080, 0x00492082, 0x00492090, 0x00492092, 0x00492400,
		0x00492402, 0x00492410, 0x00492412, 0x00492480, 0x00492482, 0x00492490, 0x00492492
	};

	static uint[] morton256_z = new uint[256]
	{
		0x00000000,
		0x00000004, 0x00000020, 0x00000024, 0x00000100, 0x00000104, 0x00000120, 0x00000124, 0x00000800,
		0x00000804, 0x00000820, 0x00000824, 0x00000900, 0x00000904, 0x00000920, 0x00000924, 0x00004000,
		0x00004004, 0x00004020, 0x00004024, 0x00004100, 0x00004104, 0x00004120, 0x00004124, 0x00004800,
		0x00004804, 0x00004820, 0x00004824, 0x00004900, 0x00004904, 0x00004920, 0x00004924, 0x00020000,
		0x00020004, 0x00020020, 0x00020024, 0x00020100, 0x00020104, 0x00020120, 0x00020124, 0x00020800,
		0x00020804, 0x00020820, 0x00020824, 0x00020900, 0x00020904, 0x00020920, 0x00020924, 0x00024000,
		0x00024004, 0x00024020, 0x00024024, 0x00024100, 0x00024104, 0x00024120, 0x00024124, 0x00024800,
		0x00024804, 0x00024820, 0x00024824, 0x00024900, 0x00024904, 0x00024920, 0x00024924, 0x00100000,
		0x00100004, 0x00100020, 0x00100024, 0x00100100, 0x00100104, 0x00100120, 0x00100124, 0x00100800,
		0x00100804, 0x00100820, 0x00100824, 0x00100900, 0x00100904, 0x00100920, 0x00100924, 0x00104000,
		0x00104004, 0x00104020, 0x00104024, 0x00104100, 0x00104104, 0x00104120, 0x00104124, 0x00104800,
		0x00104804, 0x00104820, 0x00104824, 0x00104900, 0x00104904, 0x00104920, 0x00104924, 0x00120000,
		0x00120004, 0x00120020, 0x00120024, 0x00120100, 0x00120104, 0x00120120, 0x00120124, 0x00120800,
		0x00120804, 0x00120820, 0x00120824, 0x00120900, 0x00120904, 0x00120920, 0x00120924, 0x00124000,
		0x00124004, 0x00124020, 0x00124024, 0x00124100, 0x00124104, 0x00124120, 0x00124124, 0x00124800,
		0x00124804, 0x00124820, 0x00124824, 0x00124900, 0x00124904, 0x00124920, 0x00124924, 0x00800000,
		0x00800004, 0x00800020, 0x00800024, 0x00800100, 0x00800104, 0x00800120, 0x00800124, 0x00800800,
		0x00800804, 0x00800820, 0x00800824, 0x00800900, 0x00800904, 0x00800920, 0x00800924, 0x00804000,
		0x00804004, 0x00804020, 0x00804024, 0x00804100, 0x00804104, 0x00804120, 0x00804124, 0x00804800,
		0x00804804, 0x00804820, 0x00804824, 0x00804900, 0x00804904, 0x00804920, 0x00804924, 0x00820000,
		0x00820004, 0x00820020, 0x00820024, 0x00820100, 0x00820104, 0x00820120, 0x00820124, 0x00820800,
		0x00820804, 0x00820820, 0x00820824, 0x00820900, 0x00820904, 0x00820920, 0x00820924, 0x00824000,
		0x00824004, 0x00824020, 0x00824024, 0x00824100, 0x00824104, 0x00824120, 0x00824124, 0x00824800,
		0x00824804, 0x00824820, 0x00824824, 0x00824900, 0x00824904, 0x00824920, 0x00824924, 0x00900000,
		0x00900004, 0x00900020, 0x00900024, 0x00900100, 0x00900104, 0x00900120, 0x00900124, 0x00900800,
		0x00900804, 0x00900820, 0x00900824, 0x00900900, 0x00900904, 0x00900920, 0x00900924, 0x00904000,
		0x00904004, 0x00904020, 0x00904024, 0x00904100, 0x00904104, 0x00904120, 0x00904124, 0x00904800,
		0x00904804, 0x00904820, 0x00904824, 0x00904900, 0x00904904, 0x00904920, 0x00904924, 0x00920000,
		0x00920004, 0x00920020, 0x00920024, 0x00920100, 0x00920104, 0x00920120, 0x00920124, 0x00920800,
		0x00920804, 0x00920820, 0x00920824, 0x00920900, 0x00920904, 0x00920920, 0x00920924, 0x00924000,
		0x00924004, 0x00924020, 0x00924024, 0x00924100, 0x00924104, 0x00924120, 0x00924124, 0x00924800,
		0x00924804, 0x00924820, 0x00924824, 0x00924900, 0x00924904, 0x00924920, 0x00924924
	};

	static System.UInt64 mortonEncode_LUT(uint x, uint y, uint z){
		System.UInt64 answer = 0;
		answer =    morton256_z[(z >> 16) & 0xFF ] | // we start by shifting the third byte, since we only look at the first 21 bits
			morton256_y[(y >> 16) & 0xFF ] |
				morton256_x[(x >> 16) & 0xFF ];
		answer = answer << 48 | morton256_z[(z >> 8) & 0xFF ] | // shifting second byte
			morton256_y[(y >> 8) & 0xFF ] |
				morton256_x[(x >> 8) & 0xFF ];
		answer = answer << 24 |
			morton256_z[(z) & 0xFF ] | // first byte
				morton256_y[(y) & 0xFF ] |
				morton256_x[(x) & 0xFF ];
		return answer;
	}








	//--------- Texture preview ---------------
	/*
	public static void ToggleTexturePreview() {
		if (surforgeSettings.extentTexturePreview) DestroyImmediate(surforgeSettings.extentTexturePreview.gameObject);
		else {
			InstantiateTexturePreview();
		}
	}
	*/

	public static void ActivateTexturePreview() {
		if (surforgeSettings.extentTexturePreview == null) {
			InstantiateTexturePreview();
		}
	}

	static void InstantiateTexturePreview() {
		surforgeSettings.extentTexturePreview = (SurforgeTexturePreview)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Surforge/TexturePreview/texturePreview.prefab", typeof(SurforgeTexturePreview)));
		surforgeSettings.extentTexturePreview.surforgeSettings = surforgeSettings;
		surforgeSettings.extentTexturePreview.gameObject.name = "texturePreview";
		surforgeSettings.extentTexturePreview.composer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		
		surforgeSettings.extentTexturePreview.composer.surforgeSettings = surforgeSettings;
		surforgeSettings.extentTexturePreview.composer.meshFilter = (MeshFilter)surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();

		Renderer renderer = surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();
		Material[] materials = new Material[1];
		materials[0] = renderer.sharedMaterial;


		renderer.sharedMaterials = materials;

		renderer.sharedMaterial.SetTexture("_NormalMap", surforgeSettings.normalMap);
		renderer.sharedMaterial.SetTexture("_ObjectMasks", surforgeSettings.objectMasks);
		renderer.sharedMaterial.SetTexture("_ObjectMasks2", surforgeSettings.objectMasks2);
		renderer.sharedMaterial.SetTexture("_AoEdgesDirtDepth", surforgeSettings.aoEdgesDirtDepth);
		renderer.sharedMaterial.SetTexture("_EmissionMask", surforgeSettings.emissionMap);

		renderer.sharedMaterial.SetTexture("_LabelTexture", surforgeSettings.labelsMap);

		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			renderer.sharedMaterial.SetInt("_LinearColorSpace", 1);
			//surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			surforgeSettings.isPreviewRenderTextureLinear = true; 
		}
		else {
			renderer.sharedMaterial.SetInt("_LinearColorSpace", 0); 
			//surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24); 
			surforgeSettings.isPreviewRenderTextureLinear = false;
		}

		surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24); 

		surforgeSettings.extentTexturePreview.previewCamera.targetTexture = surforgeSettings.extentTexturePreview.previewRenderTexture;
		
		RenderTexture.active = surforgeSettings.extentTexturePreview.previewRenderTexture;
		
		UpdateTexturePreviewPosition(); 

		surforgeSettings.extentTexturePreview.previewCameraFocus.position =  surforgeSettings.extentTexturePreview.composer.transform.position;
	}

	
	public static void UpdateTexturePreviewPosition() {
		if (surforgeSettings.extentTexturePreview != null) {
			
			surforgeSettings.extentTexturePreview.transform.position = Vector3.zero;
			surforgeSettings.extentTexturePreview.cameraHolder.position = new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			                                                          0,
			                                                          surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);


			surforgeSettings.extentTexturePreview.transform.position = new Vector3(0, 45.0f, 84.0f);

		}

		UpdateBackgroundQuadTransform();
		UpdatePolygonLassoPlaneTransform();
		UpdatePlaceToolPlaneTransform();
	}



	public static void CreatePolygonLassoPlane() {
		DestroyPolygonLassoPlane();

		surforgeSettings.polygonLassoPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
		MeshRenderer meshRenderer = (MeshRenderer)surforgeSettings.polygonLassoPlane.GetComponent<MeshRenderer>();
		DestroyImmediate(meshRenderer);
		MeshFilter meshFilter = (MeshFilter)surforgeSettings.polygonLassoPlane.GetComponent<MeshFilter>();
		DestroyImmediate(meshFilter);

		surforgeSettings.polygonLassoPlane.name = "polyLassoRaycast";

		if (surforgeSettings.limits != null) {
			surforgeSettings.polygonLassoPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			                                                                    surforgeSettings.limits.minY,
			                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		}
		else {
			surforgeSettings.polygonLassoPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
		                                                 	 -0.25f,
		                                                 	 surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		}

		surforgeSettings.polygonLassoPlane.transform.localScale = new Vector3((Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder) * 4.0f, 
		                                                  (Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder) * 4.0f,
		                                                  1);

		surforgeSettings.polygonLassoPlane.transform.Rotate(Vector3.right, 90.0f);
		surforgeSettings.polygonLassoPlane.transform.parent = surforgeSettings.root.transform;
	}

	public static void CreatePlaceToolPlane() {
		DestroyPlaceToolPlane();
		
		surforgeSettings.placeToolPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
		MeshRenderer meshRenderer = (MeshRenderer)surforgeSettings.placeToolPlane.GetComponent<MeshRenderer>();
		DestroyImmediate(meshRenderer);
		MeshFilter meshFilter = (MeshFilter)surforgeSettings.placeToolPlane.GetComponent<MeshFilter>();
		DestroyImmediate(meshFilter);
		
		surforgeSettings.placeToolPlane.name = "placeToolRaycast";
		
		if (surforgeSettings.limits != null) {
			surforgeSettings.placeToolPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			                                                                    surforgeSettings.limits.minY,
			                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		}
		else {
			surforgeSettings.placeToolPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			                                                                    -0.25f,
			                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		}
		
		surforgeSettings.placeToolPlane.transform.localScale = new Vector3((Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder) * 4.0f, 
		                                                                    (Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder) * 4.0f,
		                                                                    1);
		
		surforgeSettings.placeToolPlane.transform.Rotate(Vector3.right, 90.0f);
		surforgeSettings.placeToolPlane.transform.parent = surforgeSettings.root.transform;
	}

	public static void CreatePlaceToolPreview() {
		DestroyPlaceToolPreview();

		surforgeSettings.symmetryParent = new GameObject();
		surforgeSettings.symmetryParent.name = "Add Detail Tool";

		Vector3 symmetryAxisDirection =  surforgeSettings.mirrorLineSolid[1] - surforgeSettings.mirrorLineSolid[0];

		surforgeSettings.symmetryParent.transform.position = surforgeSettings.symmetryPoint; 
		surforgeSettings.symmetryParent.transform.rotation = Quaternion.LookRotation(symmetryAxisDirection, Vector3.up);



		surforgeSettings.placeToolPreview = (GameObject)Instantiate(surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].gameObject);
		if (surforgeSettings.placeToolPreview != null) {
			CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreview);

			MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreview.GetComponent<MeshCollider>();
			if (meshCollider != null) DestroyImmediate(meshCollider);

			SurforgeTextGenerator textGen = (SurforgeTextGenerator)surforgeSettings.placeToolPreview.GetComponent<SurforgeTextGenerator>();
			if (textGen) {
				textGen.Generate();
			}
			foreach (Transform child in surforgeSettings.placeToolPreview.transform) {
				MeshCollider childMeshCollider = (MeshCollider)child.gameObject.GetComponent<MeshCollider>();
				if (childMeshCollider != null) DestroyImmediate(childMeshCollider);

				SurforgeTextGenerator childTextGen = (SurforgeTextGenerator)child.gameObject.GetComponent<SurforgeTextGenerator>();
				if (childTextGen) {
					childTextGen.Generate();
				}
			}
			surforgeSettings.placeToolPreview.transform.parent = surforgeSettings.symmetryParent.transform;
		}

	}

	public static void CreateSeamlessInstancesPlaceMeshForPlaceTool(GameObject obj) {
		if (surforgeSettings) {
			if (surforgeSettings.seamless) {
				PlaceMesh placeMeshObj = (PlaceMesh)obj.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) {
					placeMeshObj.surforgeSettings = surforgeSettings;
					placeMeshObj.seamlessInstances = new GameObject[8];
					CreateSeamlessInstancesPlaceMesh(placeMeshObj);
				}
			}
			else {
				PlaceMesh placeMeshObj = (PlaceMesh)obj.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) {
					placeMeshObj.surforgeSettings = surforgeSettings;
				}
			}
		}
	}

	public static void PlaceToolGenerateTexts() {
		if (surforgeSettings.placeToolPreview != null) {
			SurforgeTextGenerator textGen = (SurforgeTextGenerator)surforgeSettings.placeToolPreview.GetComponent<SurforgeTextGenerator>();
			if (textGen) {
				textGen.Generate();
			}
			foreach (Transform child in surforgeSettings.placeToolPreview.transform) {
				SurforgeTextGenerator childTextGen = (SurforgeTextGenerator)child.gameObject.GetComponent<SurforgeTextGenerator>();
				if (childTextGen) {
					childTextGen.Generate();
				}
			}
		}
	}

	public static void PlaceToolShuffle() {
		if (surforgeSettings.placeToolPreview != null) {
			if (surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleArray != null) {
				if (surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleArray.Length > 0) {

					int shuffleRandom = Random.Range((int)0, (int)surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleArray.Length);
					Vector3 placeToolScale = Vector3.one;
					if (surforgeSettings.placeToolPreview != null) {
						placeToolScale = surforgeSettings.placeToolPreview.transform.localScale;
					}

					if (surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleScale) {
						placeToolScale = placeToolScale * Random.Range(0.5f, 1.5f);
					}


					DestroyPlaceToolPreview();


					surforgeSettings.symmetryParent = new GameObject();
					surforgeSettings.symmetryParent.name = "Add Detail Tool";
					
					Vector3 symmetryAxisDirection =  surforgeSettings.mirrorLineSolid[1] - surforgeSettings.mirrorLineSolid[0];
					
					surforgeSettings.symmetryParent.transform.position = surforgeSettings.symmetryPoint; 
					surforgeSettings.symmetryParent.transform.rotation = Quaternion.LookRotation(symmetryAxisDirection, Vector3.up);

					
					
					
					surforgeSettings.placeToolPreview = (GameObject)Instantiate(surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleArray[shuffleRandom].gameObject);
					if (surforgeSettings.placeToolPreview != null) {
						surforgeSettings.placeToolPreview.transform.localScale = placeToolScale;

						if (surforgeSettings.placeMeshes.placeMeshes[surforgeSettings.activePlaceMesh].shuffleRotation) {
							surforgeSettings.placeToolPreview.transform.localEulerAngles = new Vector3(surforgeSettings.placeToolPreview.transform.localEulerAngles.x,
							                                                                         Random.Range(0, 360.0f),
							                                                                         surforgeSettings.placeToolPreview.transform.localEulerAngles.z);
						}

						CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreview);
						
						MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreview.GetComponent<MeshCollider>();
						if (meshCollider != null) DestroyImmediate(meshCollider);
						
						SurforgeTextGenerator textGen = (SurforgeTextGenerator)surforgeSettings.placeToolPreview.GetComponent<SurforgeTextGenerator>();
						if (textGen) {
							textGen.Generate();
						}
						foreach (Transform child in surforgeSettings.placeToolPreview.transform) {
							MeshCollider childMeshCollider = (MeshCollider)child.gameObject.GetComponent<MeshCollider>();
							if (childMeshCollider != null) DestroyImmediate(childMeshCollider);
							
							SurforgeTextGenerator childTextGen = (SurforgeTextGenerator)child.gameObject.GetComponent<SurforgeTextGenerator>();
							if (childTextGen) {
								childTextGen.Generate();
							}
						}
						surforgeSettings.placeToolPreview.transform.parent = surforgeSettings.symmetryParent.transform;
					}


				}
			}
		}
	}
	
	
	public static void CreatePlaceToolSymmetryPreviews() {
		if (surforgeSettings.placeToolPreview) {
			if (surforgeSettings.symmetry && surforgeSettings.symmetryX) {
				if (surforgeSettings.placeToolPreviewSymmX != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmX.gameObject);
				surforgeSettings.placeToolPreviewSymmX = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmX != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmX);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmX.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmX.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryZ) {
				if (surforgeSettings.placeToolPreviewSymmZ != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmZ.gameObject);
				surforgeSettings.placeToolPreviewSymmZ = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmZ != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmZ);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmZ.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmZ.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryX && surforgeSettings.symmetryZ) {
				if (surforgeSettings.placeToolPreviewSymmXZ != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmXZ.gameObject);
				surforgeSettings.placeToolPreviewSymmXZ = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmXZ != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmXZ);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmXZ.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmXZ.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryDiagonal) {
				if (surforgeSettings.placeToolPreviewSymmDiagonal != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonal.gameObject);
				surforgeSettings.placeToolPreviewSymmDiagonal = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmDiagonal != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmDiagonal);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmDiagonal.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmDiagonal.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryDiagonal && surforgeSettings.symmetryX) {
				if (surforgeSettings.placeToolPreviewSymmDiagonalX != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalX.gameObject);
				surforgeSettings.placeToolPreviewSymmDiagonalX = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmDiagonalX != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmDiagonalX);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmDiagonalX.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmDiagonalX.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryDiagonal && surforgeSettings.symmetryZ) {
				if (surforgeSettings.placeToolPreviewSymmDiagonalZ != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalZ.gameObject);
				surforgeSettings.placeToolPreviewSymmDiagonalZ = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmDiagonalZ != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmDiagonalZ);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmDiagonalZ.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmDiagonalZ.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		
			if (surforgeSettings.symmetry && surforgeSettings.symmetryDiagonal && surforgeSettings.symmetryX && surforgeSettings.symmetryZ) {
				if (surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalXZ.gameObject);
				surforgeSettings.placeToolPreviewSymmDiagonalXZ = (GameObject)Instantiate(surforgeSettings.placeToolPreview);
				if (surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) {
					CreateSeamlessInstancesPlaceMeshForPlaceTool(surforgeSettings.placeToolPreviewSymmDiagonalXZ);
					MeshCollider meshCollider = (MeshCollider)surforgeSettings.placeToolPreviewSymmDiagonalXZ.GetComponent<MeshCollider>();
					if (meshCollider != null) DestroyImmediate(meshCollider);
					surforgeSettings.placeToolPreviewSymmDiagonalXZ.transform.parent = surforgeSettings.symmetryParent.transform;
				}
			}
		}
	}

	public static void ChangePlaceToolPreview() {
		Vector3 oldPlaceToolLocalScale = new Vector3();
		Vector3 oldPlaceToolEulerAngles = new Vector3();

		List<string> oldTexts = new List<string>();

		if (surforgeSettings.placeToolPreview != null) {
			oldPlaceToolLocalScale = surforgeSettings.placeToolPreview.transform.localScale;
			oldPlaceToolEulerAngles = surforgeSettings.placeToolPreview.transform.eulerAngles;

			TextMesh textMesh = (TextMesh)surforgeSettings.placeToolPreview.gameObject.GetComponent<TextMesh>();
			if (textMesh) {
				oldTexts.Add (textMesh.text);
			}
			foreach (Transform child in surforgeSettings.placeToolPreview.transform) {
				TextMesh childTextMesh = (TextMesh)child.gameObject.GetComponent<TextMesh>();
				if (childTextMesh) {
					oldTexts.Add (childTextMesh.text);
				}
			}
		}

		CreatePlaceToolPreview();
		PlaceToolGenerateTexts();


		if (surforgeSettings.placeToolPreview != null) {
			surforgeSettings.placeToolPreview.transform.localScale = oldPlaceToolLocalScale;
			surforgeSettings.placeToolPreview.transform.eulerAngles = oldPlaceToolEulerAngles;

			TextMesh textMesh = (TextMesh)surforgeSettings.placeToolPreview.gameObject.GetComponent<TextMesh>();
			if (textMesh) {
				if( oldTexts.Count > 0) {
					textMesh.text = oldTexts[0];
				}
			}
			int counter = 0;
			foreach (Transform child in surforgeSettings.placeToolPreview.transform) {
				TextMesh childTextMesh = (TextMesh)child.gameObject.GetComponent<TextMesh>();
				if (childTextMesh) {
					if( oldTexts.Count > counter) {
						childTextMesh.text = oldTexts[counter];
					}

					counter++;
				}
			}
		}

		CreatePlaceToolSymmetryPreviews();

	}


	static void UpdatePolygonLassoPlaneTransform() {
		if (surforgeSettings.polygonLassoPlane != null) {

			if (surforgeSettings.limits != null) {
				surforgeSettings.polygonLassoPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
				                                                                    surforgeSettings.limits.minY,
				                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
			}

			else {
				surforgeSettings.polygonLassoPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			    	                                              -0.25f,
			        	                                          surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
			}

			surforgeSettings.polygonLassoPlane.transform.localScale = new Vector3((Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder) * 4.0f, 
			                                                  (Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder) * 4.0f,
			                                                  1);
		}
	}

	static void UpdatePlaceToolPlaneTransform() {
		if (surforgeSettings.placeToolPlane != null) {
			
			if (surforgeSettings.limits != null) {
				surforgeSettings.placeToolPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
				                                                                    -0.25f,
				                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
			}
			
			else {
				surforgeSettings.placeToolPlane.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
				                                                                    -0.25f,
				                                                                    surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
			}
			
			surforgeSettings.placeToolPlane.transform.localScale = new Vector3((Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder) * 4.0f, 
			                                                                    (Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder) * 4.0f,
			                                                                    1);
		}
	}

	public static void DestroyPolygonLassoPlane() {
		if (surforgeSettings.polygonLassoPlane != null) DestroyImmediate(surforgeSettings.polygonLassoPlane.gameObject);
	}

	public static void DestroyPlaceToolPlane() {
		if (surforgeSettings.placeToolPlane != null) DestroyImmediate(surforgeSettings.placeToolPlane.gameObject);
	}

	public static void DestroyPlaceToolPreview() {
		if (surforgeSettings.placeToolPreview != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreview.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreview.gameObject);
		}

		if (surforgeSettings.placeToolPreviewSymmX != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmX.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmX.gameObject);
		}
		if (surforgeSettings.placeToolPreviewSymmZ != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmZ.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmZ.gameObject);
		}
		if (surforgeSettings.placeToolPreviewSymmXZ != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmXZ.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmXZ.gameObject);
		}

		if (surforgeSettings.placeToolPreviewSymmDiagonal != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmDiagonal.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonal.gameObject);
		}
		if (surforgeSettings.placeToolPreviewSymmDiagonalX != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmDiagonalX.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalX.gameObject);
		}
		if (surforgeSettings.placeToolPreviewSymmDiagonalZ != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmDiagonalZ.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalZ.gameObject);
		}
		if (surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) {
			DestroyPlaceToolPreviewSeamless(surforgeSettings.placeToolPreviewSymmDiagonalXZ.gameObject);
			DestroyImmediate(surforgeSettings.placeToolPreviewSymmDiagonalXZ.gameObject);
		}

		if (surforgeSettings.symmetryParent != null) DestroyImmediate(surforgeSettings.symmetryParent);
	}

	static void DestroyPlaceToolPreviewSeamless(GameObject obj) {
		if (surforgeSettings) {
			if (surforgeSettings.seamless) {
				PlaceMesh placeMeshObj = (PlaceMesh)obj.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) {
					RemoveSeamlessInstancesPlaceMesh(placeMeshObj);
				}
			}
		}
	}

	static void CreateBackgroundQuad() { 
		surforgeSettings.backgroundQuadBorder = 2.0f;

		surforgeSettings.backgroundQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		if (backgroundQuadMaterial == null) backgroundQuadMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Surforge/Editor/Interface/Gui/extentBackground.mat", typeof(Material));
		surforgeSettings.backgroundQuad.GetComponent<Renderer>().sharedMaterial = backgroundQuadMaterial; 
		surforgeSettings.backgroundQuad.name = "background";
		surforgeSettings.backgroundQuad.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
		                                                  -0.25f,
		                                                  surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		
		surforgeSettings.backgroundQuad.transform.localScale = new Vector3(Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder, 
		                                                  Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder,
		                                                  1);
		surforgeSettings.backgroundQuad.transform.Rotate(Vector3.right, 90.0f);
		surforgeSettings.backgroundQuad.transform.parent = surforgeSettings.root.transform;
	} 

	static void UpdateBackgroundQuadTransform() {
		if (surforgeSettings.backgroundQuad != null) {
			surforgeSettings.backgroundQuad.transform.position =  new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
			                                                  -0.25f,
			                                                  surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		
			surforgeSettings.backgroundQuad.transform.localScale = new Vector3(Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) + surforgeSettings.backgroundQuadBorder, 
			                                                  Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) + surforgeSettings.backgroundQuadBorder,
		 	                                                 1);
		}
	}






	//--- GPU Render------
	static GPURender gpuRender;

	static void CreateTextures(int res) {
		bool isLinearColorSpace = false;
		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			isLinearColorSpace = true;
		}

		if (isLinearColorSpace) {
			//render result
			if (surforgeSettings.normalMap == null)	{
				surforgeSettings.normalMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true); 
			}
			else {
				if (!surforgeSettings.isNormalMapLinear) {
					DestroyImmediate(surforgeSettings.normalMap);
					surforgeSettings.normalMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true); 
					surforgeSettings.isNormalMapLinear = true;
				}
			}

			if (surforgeSettings.emissionMap == null)		 {
				surforgeSettings.emissionMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isEmissionMapLinear) {
					DestroyImmediate(surforgeSettings.emissionMap);
					surforgeSettings.emissionMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
					surforgeSettings.isEmissionMapLinear = true;
				}
			}

			if (surforgeSettings.objectMasks == null)		 {
				surforgeSettings.objectMasks = new Texture2D(res, res, TextureFormat.ARGB32, false, true);
			}
			else {
				if (!surforgeSettings.isObjectMasksLinear) {
					DestroyImmediate(surforgeSettings.objectMasks);
					surforgeSettings.objectMasks = new Texture2D(res, res, TextureFormat.ARGB32, false, true);
					surforgeSettings.isObjectMasksLinear = true;
				}
			}

			if (surforgeSettings.objectMasks2 == null)		 {
				surforgeSettings.objectMasks2 = new Texture2D(res, res, TextureFormat.ARGB32, false, true);
			}
			else {
				if (!surforgeSettings.isObjectMasks2Linear) {
					DestroyImmediate(surforgeSettings.objectMasks2);
					surforgeSettings.objectMasks2 = new Texture2D(res, res, TextureFormat.ARGB32, false, true);
					surforgeSettings.isObjectMasks2Linear = true;
				}
			}

			if (surforgeSettings.aoEdgesDirtDepth == null)	 {
				surforgeSettings.aoEdgesDirtDepth = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isAoEdgesDirtDepthLinear) {
					DestroyImmediate(surforgeSettings.aoEdgesDirtDepth);
					surforgeSettings.aoEdgesDirtDepth = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
					surforgeSettings.isAoEdgesDirtDepthLinear = true;
				}
			}

			if (surforgeSettings.labelsMap == null)			 {
				surforgeSettings.labelsMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isLabelsMapLinear) {
					DestroyImmediate(surforgeSettings.labelsMap);
					surforgeSettings.labelsMap = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
					surforgeSettings.isLabelsMapLinear = true;
				}
			}

			if (surforgeSettings.labelsAlpha == null)		 {
				surforgeSettings.labelsAlpha = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isLabelsAlphaLinear) {
					DestroyImmediate(surforgeSettings.labelsAlpha);
					surforgeSettings.labelsAlpha = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
					surforgeSettings.isLabelsAlphaLinear = true;
				}
			}
			
			//render tmp
			if (surforgeSettings.renderTexture == null)		 {
				surforgeSettings.renderTexture = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear); 
			}
			else {
				if (!surforgeSettings.isRenderTextureLinear) {
					surforgeSettings.renderTexture.Release();
					surforgeSettings.renderTexture = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear); 
					surforgeSettings.isRenderTextureLinear = true;
				}
			}

			if (surforgeSettings.renderTexture2 == null)	 {
				surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear); 
			}
			else {
				if (!surforgeSettings.isRenderTexture2Linear) {
					surforgeSettings.renderTexture2.Release();
					surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					surforgeSettings.isRenderTexture2Linear = true;
				}
			}


			if (surforgeSettings.edgesRenderTexture == null) {
				surforgeSettings.edgesRenderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			}
			else {
				if (!surforgeSettings.isEdgesRenderTextureLinear) {
					surforgeSettings.edgesRenderTexture.Release();
					surforgeSettings.edgesRenderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					surforgeSettings.isEdgesRenderTextureLinear = true;
				}
			}

			if (surforgeSettings.glowRenderTexture == null)	 {
				surforgeSettings.glowRenderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			}
			else {
				if (!surforgeSettings.isGlowRenderTextureLinear) {
					surforgeSettings.glowRenderTexture.Release();
					surforgeSettings.glowRenderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					surforgeSettings.isGlowRenderTextureLinear = true;
				}
			}

			
			if (surforgeSettings.edgesTexture == null)		 {
				surforgeSettings.edgesTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isEdgesTextureLinear) {
					DestroyImmediate(surforgeSettings.edgesTexture);
					surforgeSettings.edgesTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);
					surforgeSettings.isEdgesTextureLinear = true;
				}
			}

			if (surforgeSettings.glowTexture == null)		 {
				surforgeSettings.glowTexture = new Texture2D(512, 512, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isGlowTextureLinear) {
					DestroyImmediate(surforgeSettings.glowTexture);
					surforgeSettings.glowTexture = new Texture2D(512, 512, TextureFormat.ARGB32, true, true);
					surforgeSettings.isGlowTextureLinear = true;
				}
			}

			
			if (surforgeSettings.bucketTexture == null)		 {
				surforgeSettings.bucketTexture = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
			}
			else {
				if (!surforgeSettings.isBucketTextureLinear) {
					DestroyImmediate(surforgeSettings.bucketTexture);
					surforgeSettings.bucketTexture = new Texture2D(res, res, TextureFormat.ARGB32, true, true);
					surforgeSettings.isBucketTextureLinear = true;
				}
			}

		}

		else {
			//render result
			if (surforgeSettings.normalMap == null)			 {
				surforgeSettings.normalMap = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isNormalMapLinear) {
					DestroyImmediate(surforgeSettings.normalMap);
					surforgeSettings.normalMap = new Texture2D(res, res);
					surforgeSettings.isNormalMapLinear = false;
				}
			}

			if (surforgeSettings.emissionMap == null)		 {
				surforgeSettings.emissionMap = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isEmissionMapLinear) {
					DestroyImmediate(surforgeSettings.emissionMap);
					surforgeSettings.emissionMap = new Texture2D(res, res);
					surforgeSettings.isEmissionMapLinear = false;
				}
			}

			if (surforgeSettings.objectMasks == null)		 {
				surforgeSettings.objectMasks = new Texture2D(res, res, TextureFormat.ARGB32, false, false);
			}
			else {
				if (surforgeSettings.isObjectMasksLinear) {
					DestroyImmediate(surforgeSettings.objectMasks);
					surforgeSettings.objectMasks = new Texture2D(res, res, TextureFormat.ARGB32, false, false);
					surforgeSettings.isObjectMasksLinear = false;
				}
			}

			if (surforgeSettings.objectMasks2 == null)		 {
				surforgeSettings.objectMasks2 = new Texture2D(res, res, TextureFormat.ARGB32, false, false);
			}
			else {
				if (surforgeSettings.isObjectMasks2Linear) {
					DestroyImmediate(surforgeSettings.objectMasks2);
					surforgeSettings.objectMasks2 = new Texture2D(res, res, TextureFormat.ARGB32, false, false);
					surforgeSettings.isObjectMasks2Linear = false;
				}
			}

			if (surforgeSettings.aoEdgesDirtDepth == null)	 {
				surforgeSettings.aoEdgesDirtDepth = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isAoEdgesDirtDepthLinear) {
					DestroyImmediate(surforgeSettings.aoEdgesDirtDepth);
					surforgeSettings.aoEdgesDirtDepth = new Texture2D(res, res);
					surforgeSettings.isAoEdgesDirtDepthLinear = false;
				}
			}

			if (surforgeSettings.labelsMap == null)			 {
				surforgeSettings.labelsMap = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isLabelsMapLinear) {
					DestroyImmediate(surforgeSettings.labelsMap);
					surforgeSettings.labelsMap = new Texture2D(res, res);
					surforgeSettings.isLabelsMapLinear = false;
				}
			}

			if (surforgeSettings.labelsAlpha == null)		 {
				surforgeSettings.labelsAlpha = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isLabelsAlphaLinear) {
					DestroyImmediate(surforgeSettings.labelsAlpha);
					surforgeSettings.labelsAlpha = new Texture2D(res, res);
					surforgeSettings.isLabelsAlphaLinear = false;
				}
			}


			//render tmp
			if (surforgeSettings.renderTexture == null)		 {
				surforgeSettings.renderTexture = new RenderTexture(res, res, 24); 
			}
			else {
				if (surforgeSettings.isRenderTextureLinear) {
					surforgeSettings.renderTexture.Release();
					surforgeSettings.renderTexture = new RenderTexture(res, res, 24); 
					surforgeSettings.isRenderTextureLinear = false;
				}
			}

			if (surforgeSettings.renderTexture2 == null)	 {
				surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24); 
			}
			else {
				if (surforgeSettings.isRenderTexture2Linear) {
					surforgeSettings.renderTexture2.Release();
					surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24); 
					surforgeSettings.isRenderTexture2Linear = false;
				}
			}


			if (surforgeSettings.edgesRenderTexture == null) {
				surforgeSettings.edgesRenderTexture = new RenderTexture(1024, 1024, 24);
			}
			else {
				if (surforgeSettings.isEdgesRenderTextureLinear) {
					surforgeSettings.edgesRenderTexture.Release();
					surforgeSettings.edgesRenderTexture = new RenderTexture(1024, 1024, 24);
					surforgeSettings.isEdgesRenderTextureLinear = false;
				}
			}

			if (surforgeSettings.glowRenderTexture == null)	 {
				surforgeSettings.glowRenderTexture = new RenderTexture(512, 512, 24,RenderTextureFormat.ARGB32);
			}
			else {
				if (surforgeSettings.isGlowRenderTextureLinear) {
					surforgeSettings.glowRenderTexture.Release();
					surforgeSettings.glowRenderTexture = new RenderTexture(512, 512, 24,RenderTextureFormat.ARGB32);
					surforgeSettings.isGlowRenderTextureLinear = false;
				}
			}

			if (surforgeSettings.edgesTexture == null)		 {
				surforgeSettings.edgesTexture = new Texture2D(1024, 1024);
			}
			else {
				if (surforgeSettings.isEdgesTextureLinear) {
					DestroyImmediate(surforgeSettings.edgesTexture);
					surforgeSettings.edgesTexture = new Texture2D(1024, 1024);
					surforgeSettings.isEdgesTextureLinear = false;
				}
			}

			if (surforgeSettings.glowTexture == null)		 {
				surforgeSettings.glowTexture = new Texture2D(512, 512);
			}
			else {
				if (surforgeSettings.isGlowTextureLinear) {
					DestroyImmediate(surforgeSettings.glowTexture);
					surforgeSettings.glowTexture = new Texture2D(512, 512);
					surforgeSettings.isGlowTextureLinear = false;
				}
			}


			if (surforgeSettings.bucketTexture == null)		 {
				surforgeSettings.bucketTexture = new Texture2D(res, res);
			}
			else {
				if (surforgeSettings.isBucketTextureLinear) {
					DestroyImmediate(surforgeSettings.bucketTexture);
					surforgeSettings.bucketTexture = new Texture2D(res, res);
					surforgeSettings.isBucketTextureLinear = false;
				}
			}

		}

		//set resolution 

		//render result
		if (surforgeSettings.normalMap.width != res)			surforgeSettings.normalMap.Resize(res, res);
		if (surforgeSettings.emissionMap.width != res)			surforgeSettings.emissionMap.Resize(res, res);
		if (surforgeSettings.objectMasks.width != res)			surforgeSettings.objectMasks.Resize(res, res);
		if (surforgeSettings.objectMasks2.width != res)			surforgeSettings.objectMasks2.Resize(res, res);
		if (surforgeSettings.aoEdgesDirtDepth.width != res)		surforgeSettings.aoEdgesDirtDepth.Resize(res, res);
		if (surforgeSettings.labelsMap.width != res)			surforgeSettings.labelsMap.Resize(res, res);
		if (surforgeSettings.labelsAlpha.width != res)			surforgeSettings.labelsAlpha.Resize(res, res);
				
		//render tmp
		if (surforgeSettings.renderTexture.width != res) {
			surforgeSettings.renderTexture.Release();
			if (isLinearColorSpace) {
				surforgeSettings.renderTexture = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			}
			else {
				surforgeSettings.renderTexture = new RenderTexture(res, res, 24);
			}
		}
		if (surforgeSettings.renderTexture2.width != res) {
			surforgeSettings.renderTexture2.Release();
			if (isLinearColorSpace) {
				surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear); 
			}
			else {
				surforgeSettings.renderTexture2 = new RenderTexture(res, res, 24); 
			}
		}

				
		if (surforgeSettings.bucketTexture.width != res)		surforgeSettings.bucketTexture.Resize(res, res); 


		//texture not saving with the scene
		if (surforgeSettings.normalMap != null)  surforgeSettings.normalMap.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.emissionMap != null)  surforgeSettings.emissionMap.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.objectMasks != null)  surforgeSettings.objectMasks.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.objectMasks2 != null)  surforgeSettings.objectMasks2.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.aoEdgesDirtDepth != null)  surforgeSettings.aoEdgesDirtDepth.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.labelsMap != null)  surforgeSettings.labelsMap.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.labelsAlpha != null)  surforgeSettings.labelsAlpha.hideFlags = HideFlags.DontSaveInEditor;

		if (surforgeSettings.renderTexture != null)  surforgeSettings.renderTexture.hideFlags = HideFlags.DontSaveInEditor;
		if (surforgeSettings.renderTexture2 != null)  surforgeSettings.renderTexture2.hideFlags = HideFlags.DontSaveInEditor;

		if (surforgeSettings.bucketTexture != null)  surforgeSettings.bucketTexture.hideFlags = HideFlags.DontSaveInEditor;
	}



	static Material usedSkybox;


	public static void StartGPURender() {
		LogAction("Render", "Space", "");

		int res = Mathf.RoundToInt (Mathf.Pow(2, 10+ surforgeSettings.GetGpuRenderResolution()));

		CreateTextures(res);

		gpuRender = (GPURender)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/gpuRender.prefab", typeof(GPURender)));
		gpuRender.SetSurforgeSettings(surforgeSettings);
		gpuRender.SetRenderingRes(res);
		gpuRender.SetAoMode(surforgeSettings.GetAoMode());

		int supersamplingMode = surforgeSettings.GetSupersamplingMode();

		Vector3 gpuCameraCenter = GetGpuRenderCameraCenter();
		gpuRender.SetCameraCenter(gpuCameraCenter);

		gpuRender.SetSupersamplingMode(supersamplingMode);
	}

	static Vector3 GetGpuRenderCameraCenter() {
		float frustumHeight = Mathf.Max(Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ),
		                                Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX));

		
		float distance = Mathf.Abs( frustumHeight * 0.5f / Mathf.Tan(1.0f * 0.5f * Mathf.Deg2Rad));

		Vector3 result = new Vector3 (surforgeSettings.textureBorders.minX - (surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * 0.5f,
		                              distance,
		                              surforgeSettings.textureBorders.minZ - (surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * 0.5f);
		
			
		return result;
	}






	//--- GPU export Maps ---
	static Camera gpuMapsExportCamera;
	static CameraPost gpuMapsExportCameraPost;
	
	static Texture2D exportDiffuse;
	static Texture2D exportMapSpecular;
	static Texture2D exportMapGlossiness;
	static Texture2D exportMapAo;
	static Texture2D exportEmissionMap;
	static Texture2D exportHeightMap;
	static Texture2D exportMask;
	
	static string exportPath;
	static string textureName;
	

	//export maps optimized
	static Texture2D export1;
	static Texture2D export2;
	static byte[] textureBytes;


	//unlit export
	//static Camera unlitExportCamera;
	//static GameObject unlitExportQuad;



	public static void ExportMapsGPU() {
		bool isLinearColorSpace = false;
		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			isLinearColorSpace = true;
		}

		exportPath = null;
		textureName = null;

		string path = EditorUtility.SaveFilePanel("Save texture as PNG", surforgeSettings.exportPath, surforgeSettings.textureName + ".png", "png");
		
		if ((surforgeSettings != null) && (path != null) && (path != "") && (surforgeSettings.extentTexturePreview != null)) {
			char[] slash = "/".ToCharArray();
			string[] pathElements = path.Split(slash);
			
			for (int i = 0; i < pathElements.Length - 1; i++) {  
				exportPath = exportPath + pathElements[i] + "/";
			}
			//Debug.Log (exportPath);
			surforgeSettings.exportPath = exportPath;
			
			char[] dot = ".".ToCharArray();
			string[] textureNameElements = pathElements[pathElements.Length-1].Split(dot);
			textureName = textureNameElements[0];
			//Debug.Log (textureName);
			surforgeSettings.textureName = textureName;
			
			if (surforgeSettings.normalMap != null) {

				if (export1 == null) export1 = new Texture2D(surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, TextureFormat.ARGB32, false);
				if (export2 == null) export2 = new Texture2D(surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, TextureFormat.ARGB32, false);

				if (export1.width != surforgeSettings.aoEdgesDirtDepth.width) export1.Resize(surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, TextureFormat.ARGB32, false);
				if (export2.width != surforgeSettings.aoEdgesDirtDepth.width) export2.Resize(surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, TextureFormat.ARGB32, false);



				//-----export unlit-----------
				/*
				CreateUnlitExportPlaneAndCamera();
				ExportUnlit(export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, false);
				ExportBytesGPU(export1, "_Baked");
				*/







				//-----------------------------



				CreateGpuMapsExportCamera(); 


				ExportMapGpu(surforgeSettings.exportNormals, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				ExportBytesGPU(export1, "_NormalMap");


				ExportMapGpu(surforgeSettings.exportDiffuse, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);

				if (surforgeSettings.exportAlpha == null) {
					surforgeSettings.exportAlpha = Shader.Find("Hidden/ExportAlpha");
				}
				ExportMapGpu(surforgeSettings.exportAlpha, export2, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);


				WriteTgaAlbedo(export1, export2, exportPath + textureName + "_Albedo" + ".tga");

				ExportMapGpu(surforgeSettings.exportAo, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				ExportBytesGPU(export1, "_Occlusion");

				ExportMapGpu(surforgeSettings.exportSpecular, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportGlossiness, export2, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				WriteTgaSpecGloss(export1, export2, exportPath + textureName + "_Specular" + ".tga");

				ExportMapGpu(surforgeSettings.exportEmission, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				ExportBytesGPU(export1, "_Emission");

				ExportMapGpu(surforgeSettings.exportHeightmap, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, isLinearColorSpace);
				ExportBytesGPU(export1, "_HeightMap");

				int maskExportType = surforgeSettings.GetMaskExportMode();

				if (maskExportType == 1) {
					ExportMapGpuMasks(surforgeSettings.exportMask, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 0, false);
					ExportBytesGPU(export1, "_ID");
				}
				if (maskExportType == 2) {
					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 0, true);
					ExportBytesGPU(export1, "_Mask 1");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 1, true);
					ExportBytesGPU(export1, "_Mask 2");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 2, true);
					ExportBytesGPU(export1, "_Mask 3");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 3, true);
					ExportBytesGPU(export1, "_Mask 4");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 4, true);
					ExportBytesGPU(export1, "_Mask 5");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 5, true);
					ExportBytesGPU(export1, "_Mask 6");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 6, true);
					ExportBytesGPU(export1, "_Mask 7");

					ExportMapGpuMasks(surforgeSettings.exportMaskSeparate, export1, surforgeSettings.aoEdgesDirtDepth.width, surforgeSettings.aoEdgesDirtDepth.height, 7, true);
					ExportBytesGPU(export1, "_Mask 8");
				}

				gpuMapsExportCamera.targetTexture = null;
				RenderTexture.active = null;
				if (gpuMapsExportCamera != null) DestroyImmediate(gpuMapsExportCamera.gameObject);
			}
			
			AssetDatabase.Refresh();

			CreateMaterialForExportedMaps(path, isLinearColorSpace);
		}
	}
	

	static void CreateMaterialForExportedMaps(string path, bool isLinearColorSpace) {
		char[] slash = "/".ToCharArray();
		string[] pathElements = path.Split(slash);

		string pathForMaterialExport = "";
		bool assetsFolderFoundInPath = false;
		for (int i = 0; i < pathElements.Length - 1; i++) {  
			if (pathElements[i] == "Assets") {
				assetsFolderFoundInPath = true;
			}
			if (assetsFolderFoundInPath) {
				if (i == (pathElements.Length - 2)) {
					pathForMaterialExport = pathForMaterialExport + pathElements[i];
				}
				else {
					pathForMaterialExport = pathForMaterialExport + pathElements[i] + "/";
				}
			}
		}

		if (!assetsFolderFoundInPath) return;


		string materialPathAndName = pathForMaterialExport + "/" + textureName  + ".mat";

		string[] lookFor = new string[] {pathForMaterialExport};

		string[] guidsNormalMap = AssetDatabase.FindAssets (textureName + "_NormalMap t:Texture2D", lookFor);

		if (guidsNormalMap.Length > 0) {
			TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_NormalMap.png");
			textureImporter.textureType = TextureImporterType.NormalMap;
			AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_NormalMap.png");
		}

		//linear maps import mode
		if (isLinearColorSpace) {
			string[] guidsAlbedo = AssetDatabase.FindAssets (textureName + "_Albedo t:Texture2D", lookFor);
			if (guidsAlbedo.Length > 0) {
				TextureImporter textureImporterAlbedo = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_Albedo.tga");
				textureImporterAlbedo.linearTexture = true;
				AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_Albedo.tga");
			}

			string[] guidsEmission = AssetDatabase.FindAssets (textureName + "_Emission t:Texture2D", lookFor);
			if (guidsEmission.Length > 0) {
				TextureImporter textureImporterEmission = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_Emission.png");
				textureImporterEmission.linearTexture = true;
				AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_Emission.png");
			}

			string[] guidsHeightMap = AssetDatabase.FindAssets (textureName + "_HeightMap t:Texture2D", lookFor);
			if (guidsHeightMap.Length > 0) {
				TextureImporter textureImporterHeightMap = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_HeightMap.png");
				textureImporterHeightMap.linearTexture = true;
				AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_HeightMap.png");
			}

			string[] guidsOcclusion = AssetDatabase.FindAssets (textureName + "_Occlusion t:Texture2D", lookFor);
			if (guidsOcclusion.Length > 0) {
				TextureImporter textureImporterOcclusion = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_Occlusion.png");
				textureImporterOcclusion.linearTexture = true;
				AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_Occlusion.png");
			}

			string[] guidsSpecular = AssetDatabase.FindAssets (textureName + "_Specular t:Texture2D", lookFor);
			if (guidsSpecular.Length > 0) {
				TextureImporter textureImporterSpecular = (TextureImporter)TextureImporter.GetAtPath(pathForMaterialExport + "/" + textureName + "_Specular.tga");
				textureImporterSpecular.linearTexture = true;
				AssetDatabase.ImportAsset(pathForMaterialExport + "/" + textureName + "_Specular.tga");
			}

		}


		string[] guidsMaterial = AssetDatabase.FindAssets (textureName + " t:Material", lookFor);

		if (guidsMaterial.Length == 0) {

			Material matForExportedTextures = new Material( Shader.Find("Standard (Specular setup)") );
			if (isLinearColorSpace) {
				matForExportedTextures.SetColor("_EmissionColor", new Color(2,2,2));
			}
			else {
				matForExportedTextures.SetColor("_EmissionColor", new Color(5,5,5));
			}

			Texture2D exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_Albedo.tga", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_MainTex", exportedTexture);
			}
			
			 exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_NormalMap.png", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_BumpMap", exportedTexture);
			}

			exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_Occlusion.png", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_OcclusionMap", exportedTexture);
			}

			exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_Specular.tga", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_SpecGlossMap", exportedTexture);
			}

			exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_Emission.png", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_EmissionMap", exportedTexture);
			}

			exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForMaterialExport + "/" + textureName + "_HeightMap.png", typeof(Texture2D));
			if (exportedTexture != null) {
				matForExportedTextures.SetTexture("_ParallaxMap", exportedTexture);
			}

			
			AssetDatabase.CreateAsset( matForExportedTextures, materialPathAndName ); 
		}

		AssetDatabase.SaveAssets();

	}



	/*
	//---unlit export ---

	static void CreateUnlitExportPlaneAndCamera() {
		GameObject unlitExportCameraObj = new GameObject ("unlitExportCamera");
		unlitExportCamera = unlitExportCameraObj.AddComponent<Camera>(); 
		unlitExportCameraObj.transform.position = new Vector3(0, 0, -1.0f);


		unlitExportQuad = GameObject.CreatePrimitive (PrimitiveType.Quad);
		Renderer unlitExportQuadRenderer = unlitExportQuad.GetComponent<Renderer>();

		Renderer composerRenderer = surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>();

		unlitExportQuadRenderer.sharedMaterial = composerRenderer.sharedMaterial;
	}



	static void ExportUnlit(Texture2D texture, int resX, int resY, bool isMask) {
				
		if (surforgeSettings.renderTexture == null) surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);
		
		if (surforgeSettings.renderTexture.width != resX) {
			surforgeSettings.renderTexture.Release();
			surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);
		}
	
		
		unlitExportCamera.targetTexture = surforgeSettings.renderTexture;
		
		unlitExportCamera.Render(); 
		
		RenderTexture.active = surforgeSettings.renderTexture;
		texture.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		
		texture.Apply();
		
		unlitExportCamera.targetTexture = null;
		RenderTexture.active = null;
	}

	
	//---------------------
	*/



	static void CreateGpuMapsExportCamera() {
		GameObject obj = (GameObject)Instantiate(surforgeSettings.mapsExportCameraPrefab); 
		obj.name = "maps export camera";

		gpuMapsExportCamera = (Camera)obj.GetComponent<Camera>();

		CameraPost[] cameraPosts = (CameraPost[])obj.GetComponents<CameraPost>();
		for (int i=0; i<cameraPosts.Length; i++) {
			if (cameraPosts[i].mat == null) {
				gpuMapsExportCameraPost = cameraPosts[i];
				break;
			}
		}

	}
	


	static void ExportMapGpu(Shader shader, Texture2D texture, int resX, int resY, bool isLinearColorSpace) {
		gpuMapsExportCameraPost.mat = new Material(surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial);
		gpuMapsExportCameraPost.mat.shader = shader;
		if (isLinearColorSpace) {
			gpuMapsExportCameraPost.mat.SetInt("_LinearColorSpace", 1);
		}
		else {
			gpuMapsExportCameraPost.mat.SetInt("_LinearColorSpace", 0);
		}


		if (surforgeSettings.renderTexture == null) surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);

		if (surforgeSettings.renderTexture.width != resX) {
			surforgeSettings.renderTexture.Release();
			surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);
		}


		CameraPost[] cameraPosts = gpuMapsExportCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
		}

		gpuMapsExportCamera.targetTexture = surforgeSettings.renderTexture;

		gpuMapsExportCamera.Render(); 

		RenderTexture.active = surforgeSettings.renderTexture;
		texture.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);

		texture.Apply();

		gpuMapsExportCamera.targetTexture = null;
		RenderTexture.active = null;
	}


	static void ExportMapGpuMasks(Shader shader, Texture2D texture, int resX, int resY, int maskID, bool separateMasks) {
		gpuMapsExportCameraPost.mat = new Material(surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial);
		gpuMapsExportCameraPost.mat.shader = shader;

		if (separateMasks) gpuMapsExportCameraPost.mat.SetFloat("_MaskID", (float)maskID );
		gpuMapsExportCameraPost.mat.SetTexture("_Masks", surforgeSettings.objectMasks);
		gpuMapsExportCameraPost.mat.SetTexture("_Masks2", surforgeSettings.objectMasks2);
		
		if (surforgeSettings.renderTexture == null) surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);
		
		if (surforgeSettings.renderTexture.width != resX) {
			surforgeSettings.renderTexture.Release();
			surforgeSettings.renderTexture = new RenderTexture(resX, resY, 24);
		}

		CameraPost[] cameraPosts = gpuMapsExportCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
		}
		
		gpuMapsExportCamera.targetTexture = surforgeSettings.renderTexture;
		
		gpuMapsExportCamera.Render(); 
		RenderTexture.active = surforgeSettings.renderTexture;
		texture.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		texture.Apply();
		
		gpuMapsExportCamera.targetTexture = null;
		RenderTexture.active = null;
	}



	static void ExportBytesGPU(Texture2D texture, string suffix) {
		textureBytes = texture.EncodeToPNG();
		File.WriteAllBytes(exportPath + textureName + suffix + ".png", textureBytes);
	}




	static void WriteTgaSpecGloss(Texture2D textureSpec, Texture2D textureGloss, string path) {
		int width = textureSpec.width;
		int height = textureSpec.height;
		bool alpha = true;

		byte[] rawSpec = textureSpec.GetRawTextureData();
		byte[] rawGloss = textureGloss.GetRawTextureData();
		byte[] data = new byte[rawSpec.Length];

		for (int i=0; i< rawSpec.Length; i = i + 4) {
			for (int p=3; p >= 0; p--) {
				if (p==3) data[i+p] = rawGloss[i+ 3];
				else data[i+p] = rawSpec[i+ 3-p];
			}
		}
		
		byte[] header = new byte[18];
		header[2] = 2; // uncompressed, true-color image
		header[12] = (byte) ((width >> 0) & 0xFF);
		header[13] = (byte) ((width >> 8) & 0xFF);
		header[14] = (byte) ((height >> 0) & 0xFF);
		header[15] = (byte) ((height >> 8) & 0xFF);
		header[16] = (byte) (alpha ? 32 : 24); // bits per pixel
		header[17] = System.Convert.ToByte("00000001", 2);

		byte[] result = new byte[data.Length + header.Length]; 
		for (int i=0; i< result.Length; i++) {
			if (i < 18) result[i] = header[i];
			else result[i] = data[i-18];
		}

		File.WriteAllBytes(path, result);

	}


	static void WriteTgaAlbedo(Texture2D textureAlbedo, Texture2D textureAlpha, string path) {
		int width = textureAlbedo.width;
		int height = textureAlbedo.height;
		bool alpha = true;
		
		byte[] rawAlbedo = textureAlbedo.GetRawTextureData();
		byte[] rawAlpha = textureAlpha.GetRawTextureData();
		byte[] data = new byte[rawAlbedo.Length];
		
		for (int i=0; i< rawAlbedo.Length; i = i + 4) {
			for (int p=3; p >= 0; p--) {
				if (p==3) data[i+p] = rawAlpha[i+ 3];
				else data[i+p] = rawAlbedo[i+ 3-p];
			}
		}
		
		byte[] header = new byte[18];
		header[2] = 2; // uncompressed, true-color image
		header[12] = (byte) ((width >> 0) & 0xFF);
		header[13] = (byte) ((width >> 8) & 0xFF);
		header[14] = (byte) ((height >> 0) & 0xFF);
		header[15] = (byte) ((height >> 8) & 0xFF);
		header[16] = (byte) (alpha ? 32 : 24); // bits per pixel
		header[17] = System.Convert.ToByte("00000001", 2);
		
		byte[] result = new byte[data.Length + header.Length]; 
		for (int i=0; i< result.Length; i++) {
			if (i < 18) result[i] = header[i];
			else result[i] = data[i-18];
		}
		
		File.WriteAllBytes(path, result);
	}


	
	public static void CreateFloor() {
		if (surforgeSettings.extentTexturePreview) {
			surforgeSettings.floorObject = (GameObject)Instantiate (surforgeSettings.extentTexturePreview.floorPrefab);
			surforgeSettings.floorObject.transform.position = surforgeSettings.extentTexturePreview.composer.transform.position;
			surforgeSettings.floorObject.transform.parent = surforgeSettings.extentTexturePreview.composer.transform;
			surforgeSettings.floorObject.transform.localPosition = new Vector3(0, -0.5f, 0);
		}
	}

	public static void CreatePreviewWireframeMesh() {
		if (surforgeSettings.extentTexturePreview) {
			surforgeSettings.wireframeObject = new GameObject ("wireframe");
			MeshFilter wireframeMeshFilter = surforgeSettings.wireframeObject.AddComponent<MeshFilter>();
			Renderer wireframeObjectRenderer =surforgeSettings.wireframeObject.AddComponent<MeshRenderer>();
			wireframeObjectRenderer.sharedMaterial = new Material(surforgeSettings.extentTexturePreview.wireframeShader);

			if (surforgeSettings.extentTexturePreview.composer) {
				surforgeSettings.wireframeObject.transform.parent = surforgeSettings.extentTexturePreview.transform;

				surforgeSettings.wireframeObject.transform.localScale = surforgeSettings.extentTexturePreview.composer.transform.localScale;
				surforgeSettings.wireframeObject.transform.localPosition = surforgeSettings.extentTexturePreview.composer.transform.localPosition + new Vector3(-1.2f, 0, 0);
				surforgeSettings.wireframeObject.transform.localRotation = surforgeSettings.extentTexturePreview.composer.transform.localRotation;
				surforgeSettings.wireframeObject.layer = surforgeSettings.extentTexturePreview.composer.gameObject.layer;

				MeshFilter composerMeshfilter = surforgeSettings.extentTexturePreview.composer.GetComponent<MeshFilter>();

				Mesh wireframeMesh = new Mesh();
				Vector3[] wireframeVerts = new Vector3[composerMeshfilter.sharedMesh.triangles.Length];
				int[] wireframeTriangles = new int[composerMeshfilter.sharedMesh.triangles.Length];
				Color[] rgbVertexColors = new Color[composerMeshfilter.sharedMesh.triangles.Length];
				Vector2[] wireframeUVs = new Vector2[composerMeshfilter.sharedMesh.triangles.Length];
				int counter = 0;
				for (int i=0; i < composerMeshfilter.sharedMesh.triangles.Length; i++) {
					wireframeTriangles[i] = i;
					wireframeVerts[i] = composerMeshfilter.sharedMesh.vertices[composerMeshfilter.sharedMesh.triangles[i]];
					wireframeUVs[i] = composerMeshfilter.sharedMesh.uv[composerMeshfilter.sharedMesh.triangles[i]];

					if (counter == 0) rgbVertexColors[i] = new Color(1,0,0,1);
					if (counter == 1) rgbVertexColors[i] = new Color(0,1,0,1);
					if (counter == 2) rgbVertexColors[i] = new Color(0,0,1,1);

					counter++;
					if (counter >= 3) counter = 0;
				}
				wireframeMesh.vertices = wireframeVerts;
				wireframeMesh.triangles = wireframeTriangles;
				wireframeMesh.uv = wireframeUVs;
				wireframeMesh.colors = rgbVertexColors;

				wireframeMeshFilter.sharedMesh = wireframeMesh;
				wireframeMeshFilter.sharedMesh.RecalculateNormals();
				wireframeMeshFilter.sharedMesh.RecalculateBounds();
			}

		}
	}
	

	public static void TakePreviewScreenshotWithMaterials() {
		List<GameObject> materialPreviewObjects = new List<GameObject>();

		if (surforgeSettings.extentTexturePreview) {

			float offset = 2.75f;

			for (int i=0; i<8; i++) {
				GameObject materialPreviewObject = (GameObject)Instantiate(Surforge.surforgeSettings.renderMaterialIconPrefab.gameObject, Vector3.zero, Quaternion.identity);
				materialPreviewObjects.Add(materialPreviewObject);
				materialPreviewObject.transform.position = surforgeSettings.extentTexturePreview.previewCamera.transform.position;
				materialPreviewObject.transform.parent = surforgeSettings.extentTexturePreview.previewCamera.transform;
				materialPreviewObject.transform.localPosition = new Vector3 (- (offset*8) * 0.5f + offset * 0.5f + i*offset, -7.5f, 35.0f);
				Renderer r =  (Renderer)materialPreviewObject.GetComponent<Renderer>();
				if (r) {
					r.sharedMaterial = new Material(Surforge.surforgeSettings.renderMaterialIconShader);

					CopyMaterialToChosenID(surforgeSettings.extentTexturePreview.composer.GetComponent<Renderer>().sharedMaterial, i, r.sharedMaterial, 0);

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
			}


		}

		RenderTexture.active = null;

		int resWidth = 4096; 
		int resHeight = 2730;
		
		Camera screenshotCamera = surforgeSettings.extentTexturePreview.previewCamera;
		if (surforgeSettings.extentTexturePreview.previewRenderTexture != null) {
			surforgeSettings.extentTexturePreview.previewRenderTexture.Release();
		}
		
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		screenshotCamera.targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		screenshotCamera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenshotCamera.targetTexture = null;
		RenderTexture.active = null; 
		if (rt != null) rt.Release();
		if (rt != null) DestroyImmediate(rt);
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = ScreenShotName(resWidth, resHeight); 
		System.IO.File.WriteAllBytes(filename, bytes);
		AssetDatabase.Refresh();
		
		if (!Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture) { 
			Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24);
		}
		Surforge.surforgeSettings.extentTexturePreview.previewCamera.targetTexture = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
		RenderTexture.active = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;

		for (int i=0; i < materialPreviewObjects.Count; i++) {
			if (materialPreviewObjects[i] != null) DestroyImmediate(materialPreviewObjects[i].gameObject);
		}
	}


	public static void TakePreviewScreenshotWithMaps(bool isShaderInMapsMode) {
		if (surforgeSettings.normalMap != null) {
			bool isLinearColorSpace = false;
			if (PlayerSettings.colorSpace == ColorSpace.Linear) {
				isLinearColorSpace = true;
			}

			List<GameObject> screenshotPolyObjects = new List<GameObject>();

			if (!isShaderInMapsMode) {
				int screenMapsRes = 1024;
			
				if (exportDiffuse == null) 		 exportDiffuse = new Texture2D(screenMapsRes, screenMapsRes);
				if (exportMapSpecular == null) 	 exportMapSpecular = new Texture2D(screenMapsRes, screenMapsRes);
				if (exportMapGlossiness == null) exportMapGlossiness = new Texture2D(screenMapsRes, screenMapsRes);
				if (exportMapAo == null) 		 exportMapAo = new Texture2D(screenMapsRes, screenMapsRes);
				if (exportEmissionMap == null) 	 exportEmissionMap = new Texture2D(screenMapsRes, screenMapsRes);
				if (exportMask == null) 		 exportMask = new Texture2D(screenMapsRes, screenMapsRes);		
				if (exportHeightMap == null) 	 exportHeightMap = new Texture2D(screenMapsRes, screenMapsRes);			
	
				CreateGpuMapsExportCamera();
			
				ExportMapGpu(surforgeSettings.exportDiffuse, exportDiffuse, screenMapsRes, screenMapsRes, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportAo, exportMapAo, screenMapsRes, screenMapsRes, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportSpecular, exportMapSpecular, screenMapsRes, screenMapsRes, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportGlossiness, exportMapGlossiness, screenMapsRes, screenMapsRes, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportEmission, exportEmissionMap, screenMapsRes, screenMapsRes, isLinearColorSpace);
				ExportMapGpu(surforgeSettings.exportHeightmap, exportHeightMap, screenMapsRes, screenMapsRes, isLinearColorSpace);

				ExportMapGpuMasks(surforgeSettings.exportMask, exportMask, screenMapsRes, screenMapsRes, 0, false);


				gpuMapsExportCamera.targetTexture = null;
				RenderTexture.active = null;
				if (gpuMapsExportCamera != null) DestroyImmediate(gpuMapsExportCamera.gameObject);

			

				float screenshotPolyObjectSize = 8.2f;

				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportDiffuse, screenshotPolyObjectSize, false));
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportMask, screenshotPolyObjectSize, false));
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportHeightMap, screenshotPolyObjectSize, false));
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportEmissionMap, screenshotPolyObjectSize, false));
				if (isLinearColorSpace) {
					screenshotPolyObjects.Add(CreateScreenshotPolyObject(surforgeSettings.normalMap, screenshotPolyObjectSize, true));
				}
				else {
					screenshotPolyObjects.Add(CreateScreenshotPolyObject(surforgeSettings.normalMap, screenshotPolyObjectSize, false));
				}
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportMapSpecular, screenshotPolyObjectSize, false));
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportMapGlossiness, screenshotPolyObjectSize, false));
				screenshotPolyObjects.Add(CreateScreenshotPolyObject(exportMapAo, screenshotPolyObjectSize, false));




				int counter = 0;
				for (int i=0; i < screenshotPolyObjects.Count; i++) {
					float yStep = 16.38f;

					if (i < 4) {
						screenshotPolyObjects[i].transform.localPosition = new Vector3(57.33f , 4 * yStep * 0.5f - counter * yStep - yStep * 0.5f, 122.28f);
					}
					else {
						screenshotPolyObjects[i].transform.localPosition = new Vector3(40.93f , 4 * yStep * 0.5f - counter * yStep - yStep * 0.5f, 122.28f);
					}
					counter++;
					if (counter >= 4) counter = 0;
				}

			}

			int resWidth = 4096; 
			int resHeight = 2048;

			Camera screenshotCamera = surforgeSettings.extentTexturePreview.previewCamera;
			if (surforgeSettings.extentTexturePreview.previewRenderTexture != null) {
				surforgeSettings.extentTexturePreview.previewRenderTexture.Release();
			}

			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			screenshotCamera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(3072, resHeight, TextureFormat.RGB24, false);
			screenshotCamera.Render();
			RenderTexture.active = rt;

			if (!isShaderInMapsMode) screenShot.ReadPixels(new Rect(1024, 0, resWidth-1024, resHeight), 0, 0);
			else screenShot.ReadPixels(new Rect(512, 0, resWidth - 512, resHeight), 0, 0);

			screenshotCamera.targetTexture = null;
			RenderTexture.active = null; 
			if (rt != null) rt.Release();
			if (rt != null) DestroyImmediate(rt);
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(3072, resHeight);
			System.IO.File.WriteAllBytes(filename, bytes);
			AssetDatabase.Refresh();
			//Debug.Log(string.Format("Took screenshot to: {0}", filename));

			if (!Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture) { 
				Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture = new RenderTexture(1024, 1024, 24);
			}
			Surforge.surforgeSettings.extentTexturePreview.previewCamera.targetTexture = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;
			RenderTexture.active = Surforge.surforgeSettings.extentTexturePreview.previewRenderTexture;


			for (int i=0; i < screenshotPolyObjects.Count; i++) {
				if (screenshotPolyObjects[i] != null) DestroyImmediate(screenshotPolyObjects[i].gameObject);
			}

		}
	}


	public static GameObject CreateScreenshotPolyObject(Texture2D map, float screenshotPolyObjectSize, bool powTexture) {
		GameObject screenshotPoly = (GameObject)Instantiate(surforgeSettings.extentTexturePreview.mapOnScreenshotPrefab);
		screenshotPoly.transform.position = surforgeSettings.extentTexturePreview.previewCamera.transform.position;
		screenshotPoly.transform.localScale = new Vector3(screenshotPolyObjectSize, screenshotPolyObjectSize, screenshotPolyObjectSize);
		screenshotPoly.transform.parent = surforgeSettings.extentTexturePreview.previewCamera.transform;
		screenshotPoly.transform.localEulerAngles = Vector3.zero;
		Renderer screenshotPolyRenderer = (Renderer)screenshotPoly.GetComponent<Renderer>();
		if (screenshotPolyRenderer) {
			Material screenshotPolyMatInstance;
			screenshotPolyMatInstance = Material.Instantiate(screenshotPolyRenderer.sharedMaterial);
			screenshotPolyRenderer.sharedMaterial = screenshotPolyMatInstance;
			screenshotPolyRenderer.sharedMaterial.SetTexture("_MainTex", map);
			if (powTexture) {
				screenshotPolyRenderer.sharedMaterial.SetInt("_PowTexture", 1);
			}
			else {
				screenshotPolyRenderer.sharedMaterial.SetInt("_PowTexture", 0);
			}
		}
		return screenshotPoly;
	}


	public static string ScreenShotName(int width, int height) {
		return string.Format("{0}/Surforge/Screenshots/screen_{1}x{2}_{3}.png", 
		                     Application.dataPath, 
		                     width, height, 
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
	



	public static void ToggleEditorGrid(bool state) {
		Assembly asm = Assembly.GetAssembly(typeof(Editor));
		var showGrid = asm.GetType("UnityEditor.AnnotationUtility").GetProperty("showGrid", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		showGrid.SetValue(null, state, null);
	}






	// -- polygon lasso built objects -----

	static float[][] PrepareSolidOffsetArray(float[] offsets, int size) {
		float[][] result = new float[size][];

		for (int i=0; i < size; i++) {
			if (offsets != null) {
				if (offsets.Length > 0 ) {
					result[i] = new float[offsets.Length];
					for (int s=0; s < result[i].Length; s++) {
						result[i][s] = offsets[s];
					}
				}
				else {
					result[i] = new float[1];
					result[i][0] = 0;
				}
			}
			else {
				result[i] = new float[1];
				result[i][0] = 0;
			}

		}

		return result;
	}

	static float[][] PrepareDetailedffsetArray(float[] offsets0, float[] offsets1, float[] offsets2, int size, int[] order) {
		float[][] result = new float[size][];
		
		for (int i=0; i < size; i++) {
			int currentOrderValue = 0;
			if (order.Length > i) currentOrderValue = order[i];
			else currentOrderValue = order[order.Length - 1];

			if (currentOrderValue == 0) {
				if (offsets0 != null) {
					if (offsets0.Length > 0) {
						result[i] = new float[offsets0.Length];
						for (int s=0; s < result[i].Length; s++) {
							result[i][s] = offsets0[s];
						}
					}
					else {
						result[i] = new float[1];
						result[i][0] = 0;
					}
				}
				else {
					result[i] = new float[1];
					result[i][0] = 0;
				}

			}
			if (currentOrderValue == 1) {
				if (offsets1 != null) {
					if (offsets1.Length > 0) {
						result[i] = new float[offsets1.Length];
						for (int s=0; s < result[i].Length; s++) {
							result[i][s] = offsets1[s];
						}
					}
					else {
						result[i] = new float[1];
						result[i][0] = 0;
					}
				}
				else {
					result[i] = new float[1];
					result[i][0] = 0;
				}

			}
			if (currentOrderValue == 2) {
				if (offsets2 != null) {
					if (offsets2.Length > 0) {
						result[i] = new float[offsets2.Length];
						for (int s=0; s < result[i].Length; s++) {
							result[i][s] = offsets2[s];
						}
					}
					else {
						result[i] = new float[1];
						result[i][0] = 0;
					}
				}
				else {
					result[i] = new float[1];
					result[i][0] = 0;
				}

			}
		}
		
		return result;
	}
	
	
	public static void RebuildPolyLassoObject(GameObject originalTransformForCeltic, PolyLassoObject obj, float bevelAmount, int bevelSteps, float[] offsets, float[] heights,
	                                          List<DecalSet> decalSets, 
	                                          List<bool> inheritMatGroup,
	                                          List<bool> scatterOnShapeVerts, 
	                                          List<bool> trim, 
	                                          List<bool> perpTrim, 
	                                          List<bool> fitDecals, 
	                                          List<float> trimOffset, 
	                                          List<float> decalOffset, 
	                                          List<float> decalOffsetRandom,
	                                          List<float> decalGap, 
	                                          List<float> decalGapRandom, 
	                                          List<float> decalSize, 
	                                          List<float> decalSizeRandom,
	                                          List<float> decalRotation, 
	                                          List<float> decalRotationRandom,
	                                          bool noise,
	                                          float shapeSubdiv,
	                                          Vector2 noise1Amount,
	                                          float noise1Frequency,
	                                          Vector2 noise2Amount,
	                                          float noise2Frequency,
	                                          int materialID,
	                                          bool isFloater,
	                                          bool isTube,
	                                          bool isOpen,
	                                          float thickness,
	                                          Vector2[] section,
	                                          bool isAdaptive,
	                                          float adaptiveStep,
	                                          float[] lengthOffsets0,
	                                          float[] lengthOffsets1,
	                                          float[] lengthOffsets2,
	                                          float[] heightOffsets0,
	                                          float[] heightOffsets1,
	                                          float[] heightOffsets2,
	                                          int repeatSize,
	                                          int[] lengthOffsetOrder,
	                                          int[] heightOffsetOrder,
	                                          bool edgeWiseOffset,
	                                          bool lengthWiseOffset,
	                                          float offsetMinEdge,
	                                          PolyLassoCorner corner,
	                                          float[] childProfileVerticalOffsets, 
	                                          float[] childProfileDepthOffsets,
	                                          int[] childProfileHorisontalOffsets,
	                                          int[] childProfileMatGroups,
	                                          PolyLassoRelativeShape[] childProfileShapes,
	                                          PolyLassoProfile[] followerProfiles, 
	                                          float[] followerProfileVerticalOffsets, 
	                                          float[] followerProfileDepthOffsets,
	                                          int[] followerProfileMatGroups,
	                                          Texture2D cutoff, 
	                                          Vector2 cutoffTiling,
	                                          Texture2D bumpMap, 
	                                          float bumpMapIntensity, 
	                                          Vector2 bumpMapTiling,
	                                          Texture2D aoMap,
	                                          float aoMapIntensity,
	                                          bool randomUvOffset,
	                                          int stoneType,
	                                          bool allowIntersections,
	                                          bool overlapIntersections,
	                                          float overlapAmount,
	                                          bool usedForOverlapping, 
	                                          bool overlapStartInvert, 
	                                          bool curveUVs) {

		GameObject transformForCelticBack = new GameObject();

		float[] overlapAmounts = new float[0];
		Vector3[] overlapIntersectedShape = new Vector3[0];
		RemoveOldDetails(obj);

		Transform objParent = obj.gameObject.transform.parent;
		obj.gameObject.transform.parent = null;
		Vector3 objLocalScale = obj.gameObject.transform.localScale;
		Vector3 objLocalPosition = obj.gameObject.transform.localPosition;
		Quaternion objLocalRotation = obj.gameObject.transform.localRotation;

		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;

		//---prepare offset arrays-----
		float[][] lengthOffsets = new float[offsets.Length][];

		if (lengthOffsetOrder == null) {
			lengthOffsets = PrepareSolidOffsetArray(lengthOffsets0, offsets.Length);
		}
		else {
			if (lengthOffsetOrder.Length == 0) {
				lengthOffsets = PrepareSolidOffsetArray(lengthOffsets0, offsets.Length);
			}
			else {
				lengthOffsets = PrepareDetailedffsetArray(lengthOffsets0, lengthOffsets1, lengthOffsets2, offsets.Length, lengthOffsetOrder);
			}
		}

		float[][] heightOffsets = new float[offsets.Length][];
		if (heightOffsetOrder == null) {
			heightOffsets = PrepareSolidOffsetArray(heightOffsets0, offsets.Length);
		}
		else {
			if (heightOffsetOrder.Length == 0) {
				heightOffsets = PrepareSolidOffsetArray(heightOffsets0, offsets.Length);
			}
			else {
				heightOffsets = PrepareDetailedffsetArray(heightOffsets0, heightOffsets1, heightOffsets2, offsets.Length, heightOffsetOrder);
			}
		} 



		//---------

		MeshCollider meshCollider = (MeshCollider)obj.GetComponent<MeshCollider>();
		if (meshCollider != null) DestroyImmediate(meshCollider);

		MeshFilter meshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
		if (meshFilter != null) DestroyImmediate(meshFilter);


		Vector2[] vertices2D = new Vector2 [obj.shape.Count];

		for (int i=0; i< obj.shape.Count; i++) {
			Vector2 vertex2d = new Vector2(obj.shape[i].x, obj.shape[i].z);
			vertices2D[i] = vertex2d;
		}

		Vector2[] vertsA = CreateShapeOutline(vertices2D, offsets[0], isOpen);

		Vector2[] vertices2DBeveled = MultiBevel(vertsA, bevelAmount, bevelSteps, isOpen);



		//---follower profiles---
		if (followerProfiles != null) {
			for (int j=0; j < followerProfiles.Length; j++) {
				Vector2[] followerShape = CreateShapeOutline(vertices2D, followerProfileDepthOffsets[j], isOpen);
				List<Vector2> followerShapeList = new List<Vector2>();
				for (int s=0; s < followerShape.Length; s++) {
					followerShapeList.Add(followerShape[s]);
				}
				if (originalTransformForCeltic != null) {
					BuildChildShape(GameObject.Instantiate(originalTransformForCeltic), obj, followerShapeList, followerProfileVerticalOffsets[j], followerProfileMatGroups[j], followerProfiles[j]);						
				}
				else {
					BuildChildShape(originalTransformForCeltic, obj, followerShapeList, followerProfileVerticalOffsets[j], followerProfileMatGroups[j], followerProfiles[j]);						
				}
			}
		}	
		//-----------------------




		//---Profile is TUBE-----------------------------
		if (isTube) {
			// without noise deformer
			if (!noise) {

				int[] subdivideValues = new int[0];
				if (isAdaptive) {
					subdivideValues = GetShapeSubdivideValues(vertices2DBeveled, adaptiveStep);
				}

				Vector3[] pointsT = Points2DTo3D(vertices2DBeveled, 0);
				PrepareTubeMesh(section, obj.gameObject, pointsT, thickness, heights[0], isOpen);

				if (isAdaptive) {

					SubdivideAndCreateLengthOffsetedShapeEdgewise(vertices2DBeveled, isOpen, subdivideValues, edgeWiseOffset, lengthOffsets[0], heightOffsets[0], repeatSize, offsetMinEdge, corner, offsets.Length, 0, childProfileVerticalOffsets, childProfileDepthOffsets, childProfileHorisontalOffsets, childProfileMatGroups, childProfileShapes, obj, true);
					if (!edgeWiseOffset) CreateNotEdgeWiseOffset(lengthOffsets[0], heightOffsets[0]);
					if (lengthWiseOffset) CreateLengthWiseOffset(lengthOffsets[0], heightOffsets[0]);

					vertices2DBeveled = SubdivideShape(vertices2DBeveled, subdivideValues, isOpen);
					pointsT = Points2DTo3D(vertices2DBeveled, 0);
					  

					for (int i=0; i < pointsT.Length; i++) {
				
						Vector3[] pointsS = TubeSectionTo3DPointsOnCurve(section, pointsT, i, thickness * lengthOffsetedShape[i], heights[0], isOpen);


						if ( !(CheckOptimizeShapePointSkip(pointsT, i) && CheckOptimizeShapeLengthOffsetsSkip(lengthOffsetedShape, i, true)) ) {
							ExtrudeOutline(obj.gameObject, pointsS, true); 
						}

					}
				}

				else {
					//intersection overlapping
					if (overlapIntersections) {
						if (originalTransformForCeltic != null) {
							originalTransformForCeltic.transform.position = new Vector3(originalTransformForCeltic.transform.position.x,
							                                                            obj.transform.position.y,
							                                                            originalTransformForCeltic.transform.position.z);
							pointsT = PolyLassoObjectToWorldShapeGameObjectAndShapeArray(originalTransformForCeltic, pointsT);

							transformForCelticBack.transform.parent = originalTransformForCeltic.transform;
							originalTransformForCeltic.transform.localScale = Vector3.one;
							originalTransformForCeltic.transform.localPosition = Vector3.zero;
							originalTransformForCeltic.transform.localRotation = Quaternion.identity;
						}


						//debug
						//surforgeSettings.warpShape = new List<Vector3>();
						//for (int k=0; k < pointsT.Length; k++) surforgeSettings.warpShape.Add(pointsT[k]);

						Vector3[] pointsTold = new Vector3[pointsT.Length];
						pointsT.CopyTo(pointsTold, 0);

						List<Vector3> oPointsCurrent = new List<Vector3>();
						List<float> oHeightsCurrent = new List<float>();
						pointsT = AddOverlapPointsToShape(pointsT, pointsT, true, out oPointsCurrent, out oHeightsCurrent, null);

						List<Vector3> oPoints = new List<Vector3>();
						List<float> oHeights = new List<float>();
						pointsT = AddOverlapPointsOtherShapes( pointsT, out oPoints, out oHeights);

						overlapAmounts = CreateShapeOverlapAmountArray(pointsT, overlapAmount, pointsTold, oPoints, oHeights, overlapStartInvert);
						overlapAmounts = SmoothOverlapAmountArray(pointsT, overlapAmounts);

						if (originalTransformForCeltic != null) {
							pointsT = PolyLassoObjectToWorldShapeGameObjectAndShapeArray(transformForCelticBack, pointsT);
						}

						overlapIntersectedShape = new Vector3[pointsT.Length];
						pointsT.CopyTo(overlapIntersectedShape, 0);
					}

					for (int i=0; i < pointsT.Length; i++) {
						Vector3[] pointsS = TubeSectionTo3DPointsOnCurve(section, pointsT, i, thickness, heights[0], isOpen);
						if (overlapIntersections) pointsS = ApplyIntersectionOverlapOffset(pointsS, overlapAmounts[i]);
						ExtrudeOutline(obj.gameObject, pointsS, true); 
					}

					//TODO: this counters reaction on previous overlaps. Counter in repeated warp only, use in other modes?
					overlapAmounts = new float[0];
				}

				if (!isOpen) {
					Vector3[] startPointsS = TubeSectionTo3DPointsOnCurve(section, pointsT, 0, thickness, heights[0], isOpen);
					ExtrudeOutline(obj.gameObject, startPointsS, true); 
				}


			}
		}


		//---Profile is NO TUBE-------------------------
		else {

			// without noise deformer
			if (!noise) {


				if (isAdaptive) {

					if (corner != null) {
						vertices2D = MultiBevel(vertices2D, bevelAmount, 0, isOpen); 
					}
					else {
						vertices2D = MultiBevel(vertices2D, bevelAmount, bevelSteps, isOpen); 
					}


					int[] subdivideValues = GetShapeSubdivideValues(vertices2D, adaptiveStep);

					Vector2[] subdividedVertices2D = SubdivideAndCreateLengthOffsetedShapeEdgewise(vertices2D, isOpen, subdivideValues, edgeWiseOffset, lengthOffsets[0], heightOffsets[0], repeatSize, offsetMinEdge, corner, offsets.Length, 0, childProfileVerticalOffsets, childProfileDepthOffsets, childProfileHorisontalOffsets, childProfileMatGroups, childProfileShapes, obj, true);
					if (!edgeWiseOffset) CreateNotEdgeWiseOffset(lengthOffsets[0], heightOffsets[0]);
					Vector2[] verts = ModifyShapeWithLengthOffsets(subdividedVertices2D, offsets[0], lengthOffsetedShape, cornerCirclesOffsetMask, cornerCirclesOffsetVectors, cornerCirclesOffsets, cornerPoints, placeOnCircleVectors, startAndEndOfCirclesMask, circleCenterOffset);
		
					//----details---
					if (corner != null) {
						if (corner.cornerDetailPrefab != null) {
							PolygonLassoInstantiateCornerCircleDetails(obj, circleCentersMetConditions, corner.cornerDetailPrefab, corner.cornerDetailSize, corner.isCornerDetailCornerOriented, heights[corner.cornerDetailHeightNum]);
						}
					}
					//-----------


					//build cap
					List<Vector2> optimizedVerts = new List<Vector2>();
					for (int i=0; i<lengthOffsetedShape.Length; i++) {
						if ( !(CheckOptimizeShapePointSkip(Points2DTo3D(verts, heights[0]), i) && CheckOptimizeShapeLengthOffsetsSkip(lengthOffsetedShape, i, true)) )  {
							optimizedVerts.Add(verts[i]);
						}
					}
					Vector2[] capVerts = optimizedVerts.ToArray();


					Vector2[] vertsClean = RemoveAllSandglass(capVerts);
					TriangulateShape(vertsClean, obj.gameObject, heights[0]);


					//prepare loft shape verts
					Vector3[][] loftPoints = new Vector3[offsets.Length][];
					loftPoints[0] = Points2DTo3D(verts, heights[0]);

					bool[] vertsToKeep = new bool[lengthOffsetedShape.Length];

					for (int x=1; x < offsets.Length; x++) {

						subdividedVertices2D = SubdivideAndCreateLengthOffsetedShapeEdgewise(vertices2D, isOpen, subdivideValues, edgeWiseOffset, lengthOffsets[x], heightOffsets[x], repeatSize, offsetMinEdge, corner, offsets.Length, x, childProfileVerticalOffsets, childProfileDepthOffsets, childProfileHorisontalOffsets, childProfileMatGroups, childProfileShapes, obj, false);
						if (!edgeWiseOffset) CreateNotEdgeWiseOffset(lengthOffsets[x], heightOffsets[x]);

						verts = ModifyShapeWithLengthOffsets(subdividedVertices2D, offsets[x], lengthOffsetedShape, cornerCirclesOffsetMask, cornerCirclesOffsetVectors, cornerCirclesOffsets, cornerPoints, placeOnCircleVectors, startAndEndOfCirclesMask, circleCenterOffset);
						Vector3[] pointsB = ModyfyShapeWithHeightOffsets(verts, heights[x], heightOffsetedShape);

						loftPoints[x] = pointsB;

						for (int i=0; i<lengthOffsetedShape.Length; i++) {
							if ( !(CheckOptimizeShapePointSkip(Points2DTo3D(verts, heights[0]), i) && 
							       CheckOptimizeShapeLengthOffsetsSkip(lengthOffsetedShape, i, true) &&
							       CheckOptimizeShapeLengthOffsetsSkip(heightOffsetedShape, i, true)   ) )  {
								vertsToKeep[i] = true;
							}
						}
					}	


					//remove optimized loft shape verts
					List<Vector3>[] optimizedVerts3D = new List<Vector3>[offsets.Length];

					for (int x=0; x < offsets.Length; x++) {
						optimizedVerts3D[x] = new List<Vector3>();
						for (int i=0; i<lengthOffsetedShape.Length; i++) {
							if (vertsToKeep[i])  {
								optimizedVerts3D[x].Add (loftPoints[x][i]);
							}
						}
					}

					// build loft
					AddVertexRowToMesh(obj.gameObject, optimizedVerts3D[0].ToArray());

					for (int x=1; x < offsets.Length; x++) {
						ExtrudeOutline(obj.gameObject, optimizedVerts3D[x].ToArray(), false); 
					}



				}

				else {
					Vector2[] vertsClean = RemoveAllSandglass(vertsA);
					Vector2[] vertsCleanBeveled = MultiBevel(vertsClean, bevelAmount, bevelSteps, isOpen);

					TriangulateShape(vertsCleanBeveled, obj.gameObject, heights[0]);

					Vector2[] vertsBeveled = MultiBevel(  CreateShapeOutline(vertices2D, offsets[0], isOpen), bevelAmount, bevelSteps, isOpen);
					Vector3[] pointsB = Points2DTo3D(vertsBeveled, heights[0]);
					AddVertexRowToMesh(obj.gameObject, pointsB);

					for (int x=1; x < offsets.Length; x++) {
						vertsBeveled = MultiBevel(  CreateShapeOutline(vertices2D, offsets[x], isOpen), bevelAmount, bevelSteps, isOpen);

						pointsB = Points2DTo3D(vertsBeveled, heights[x]);
						ExtrudeOutline(obj.gameObject, pointsB, false); 
					}


					// stoneType convex hull
					if (stoneType != 0) {
						CreateConvexHull(obj.gameObject, obj.shape, stoneType, bevelAmount, bevelSteps);
					}


				}
			}

			//----noise deformer ---
			else {
				int[] subdivideValues = GetShapeSubdivideValues(vertices2DBeveled, shapeSubdiv);
				vertices2DBeveled = SubdivideShape(vertices2DBeveled, subdivideValues, isOpen);

				float randomModRange = 1000.0f;
				Vector3 mod = new Vector3(Random.Range(-randomModRange, randomModRange), Random.Range(-randomModRange, randomModRange), Random.Range(-randomModRange, randomModRange));
				Vector3[] noiseDerivates = GetNoiseDeformerDerivates(vertices2DBeveled, mod, noise1Amount, noise1Frequency, noise2Amount, noise2Frequency);

				Vector2[] vertices2DBeveledDeformed = ApplyNoiseDeformer(vertices2DBeveled, noiseDerivates);


				for (int d =0; d < 1000; d++) {
					if (TestShapeIntersections(vertices2DBeveledDeformed)) {
						mod = new Vector3(Random.Range(-randomModRange, randomModRange), Random.Range(-randomModRange, randomModRange), Random.Range(-randomModRange, randomModRange));
						noiseDerivates = GetNoiseDeformerDerivates(vertices2DBeveled, mod, noise1Amount, noise1Frequency, noise2Amount, noise2Frequency);
						vertices2DBeveledDeformed = ApplyNoiseDeformer(vertices2DBeveled, noiseDerivates);
					}
					else break;
				}


				TriangulateShape(vertices2DBeveledDeformed, obj.gameObject, heights[0]);
				obj.deformedBorder = vertices2DBeveledDeformed;

				for (int x=1; x < offsets.Length; x++) {
					Vector2[] verts = MultiBevel(  CreateShapeOutline(vertices2D, offsets[x], isOpen), bevelAmount, bevelSteps, isOpen);

					verts = SubdivideShape(verts, subdivideValues, isOpen);
					noiseDerivates = GetNoiseDeformerDerivates(verts, mod, noise1Amount, noise1Frequency, noise2Amount, noise2Frequency);
					Vector2[] vertsDeformed = ApplyNoiseDeformer(verts, noiseDerivates);

					Vector3[] pointsB = Points2DTo3D(vertsDeformed, heights[x]);
					ExtrudeOutline(obj.gameObject, pointsB, false);
				}

				// stoneType convex hull
				if (stoneType != 0) {
					CreateConvexHull(obj.gameObject, obj.shape, stoneType, bevelAmount, bevelSteps);

					// stoneType 3d noise
					if (stoneType == 2) {
						Vector3[] noiseDerivatesForMesh = GetNoiseDeformerDerivatesForMesh(obj.gameObject, mod, noise1Amount, noise1Frequency, noise2Amount, noise2Frequency);
						Apply3DNoise(obj.gameObject, noiseDerivatesForMesh);
					}
				}

				
			}
		}

		if (originalTransformForCeltic != null) DestroyImmediate(originalTransformForCeltic);
		if (transformForCelticBack != null) DestroyImmediate(transformForCelticBack);

		MakePlanarUVs(obj, randomUvOffset);
		
		obj.gameObject.AddComponent<MeshCollider>();


		obj.bevelAmount = bevelAmount;
		obj.bevelSteps = bevelSteps;
		obj.offsets = offsets;
		obj.heights = heights;

		obj.decalSets = decalSets;

		obj.inheritMatGroup = inheritMatGroup;
		obj.scatterOnShapeVerts = scatterOnShapeVerts;
		obj.trim = trim;
		obj.perpTrim = perpTrim;
		obj.fitDecals = fitDecals;

		obj.trimOffset = trimOffset;
		obj.decalOffset = decalOffset;
		obj.decalOffsetRandom = decalOffsetRandom;
		obj.decalGap = decalGap;
		obj.decalGapRandom = decalGapRandom;
		obj.decalSize = decalSize;
		obj.decalSizeRandom = decalSizeRandom;
		obj.decalRotation = decalRotation;
		obj.decalRotationRandom = decalRotationRandom;

		obj.noise = noise;
		obj.shapeSubdiv = shapeSubdiv;
		obj.noise1Amount = noise1Amount;
		obj.noise1Frequency = noise1Frequency;
		obj.noise2Amount = noise2Amount;
		obj.noise2Frequency = noise2Frequency;
		obj.materialID = materialID;
		obj.isFloater = isFloater;
		obj.isTube = isTube;
		obj.isOpen = isOpen;
		obj.thickness = thickness;
		obj.section = section;
		obj.isAdaptive = isAdaptive;
		obj.adaptiveStep = adaptiveStep;
		obj.lengthOffsets0 = lengthOffsets0;
		obj.lengthOffsets1 = lengthOffsets1;
		obj.lengthOffsets2 = lengthOffsets2;
		obj.heightOffsets0 = heightOffsets0;
		obj.heightOffsets1 = heightOffsets1;
		obj.heightOffsets2 = heightOffsets2;
		obj.repeatSize = repeatSize;
		obj.lengthOffsetOrder = lengthOffsetOrder;
		obj.heightOffsetOrder = heightOffsetOrder;
		obj.edgeWiseOffset = edgeWiseOffset;
		obj.lengthWiseOffset = lengthWiseOffset;
		obj.offsetMinEdge = offsetMinEdge;
		obj.corner = corner;
		obj.childProfileVerticalOffsets = childProfileVerticalOffsets; 
		obj.childProfileDepthOffsets = childProfileDepthOffsets;
		obj.childProfileHorisontalOffsets = childProfileHorisontalOffsets;
		obj.childProfileMatGroups = childProfileMatGroups;
		obj.childProfileShapes = childProfileShapes;
		obj.followerProfiles = followerProfiles; 
		obj.followerProfileVerticalOffsets = followerProfileVerticalOffsets; 
		obj.followerProfileDepthOffsets = followerProfileDepthOffsets;
		obj.followerProfileMatGroups = followerProfileMatGroups;
		obj.cutoff = cutoff; 
		obj.cutoffTiling = cutoffTiling;
		obj.bumpMap = bumpMap; 
		obj.bumpMapIntensity = bumpMapIntensity; 
		obj.bumpMapTiling = bumpMapTiling;
		obj.aoMap = aoMap;
		obj.aoMapIntensity = aoMapIntensity;
		obj.randomUvOffset = randomUvOffset;
		obj.stoneType = stoneType;
		obj.allowIntersections = allowIntersections;
		obj.overlapIntersections = overlapIntersections;
		obj.overlapAmount = overlapAmount;
		obj.overlapAmounts = overlapAmounts;
		obj.overlapIntersectedShape = overlapIntersectedShape;
		obj.usedForOverlapping = usedForOverlapping;
		obj.overlapStartInvert = overlapStartInvert; 
		obj.curveUVs = curveUVs;


		Material material = new Material( surforgeSettings.materialGroups[materialID]);
		if (isFloater && (materialID < 8)) {
			material = new Material( surforgeSettings.floaterMaterialGroups[materialID]);
		}
		Renderer objRenderer = (Renderer)obj.gameObject.GetComponent<Renderer>();
		objRenderer.sharedMaterial = material;
		if (cutoff) {
			objRenderer.sharedMaterial.SetTexture("_MainTex", cutoff);
			objRenderer.sharedMaterial.SetTextureScale("_MainTex", cutoffTiling);
		}
		if (bumpMap) {
			objRenderer.sharedMaterial.SetTexture("_BumpMap", bumpMap);
			objRenderer.sharedMaterial.SetTextureScale("_BumpMap", bumpMapTiling);
			objRenderer.sharedMaterial.SetFloat("_BumpIntensity", bumpMapIntensity);
		}
		if (aoMap) {
			objRenderer.sharedMaterial.SetTexture("_AO", aoMap);
			if (bumpMap) objRenderer.sharedMaterial.SetTextureScale("_AO", bumpMapTiling);
			objRenderer.sharedMaterial.SetFloat("_AOIntensity", aoMapIntensity);
		}


		RebuildDecals(obj);

		obj.transform.localScale = objLocalScale;
		obj.transform.localPosition = objLocalPosition;
		obj.transform.localRotation = objLocalRotation;
		obj.transform.parent = objParent;

		if (surforgeSettings.seamless) {
			if (!obj.isChild) RebuildSeamlessInstances(obj);
		}

	}

	static Vector3[] AddOverlapPointsOtherShapes(Vector3[] shape, out List<Vector3> oPoints, out List<float> oHeights) {
		oPoints = new List<Vector3>();
		oHeights = new List<float>();

		Vector3[] result = new Vector3[shape.Length];
		for (int i=0; i< shape.Length; i++) {
			result[i] = shape[i];
		}

		Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
		for (int i=0; i < transforms.Length; i++) {
			GameObject obj = transforms[i].gameObject;
			if (obj) {
				PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();
				if (pObj) {
					if ((pObj.isTube) && (pObj.usedForOverlapping)) {
						List<Vector3> oPointsCurrent = new List<Vector3>();
						List<float> oHeightsCurrent = new List<float>();

						bool haveOverlapIntersectedShape = false;
						if (pObj.overlapIntersectedShape != null) {
							if (pObj.overlapIntersectedShape.Length > 0) {
								haveOverlapIntersectedShape = true;
							}
						}

						if (!haveOverlapIntersectedShape) {
							result = AddOverlapPointsToShape(result, PolyLassoObjectToWorldShape(pObj).ToArray(), false, out oPointsCurrent, out oHeightsCurrent, pObj);
						}
						else {
							result = AddOverlapPointsToShape(result, PolyLassoObjectToWorldShapeGameObjectAndShapeArray(pObj.gameObject, pObj.overlapIntersectedShape), false, out oPointsCurrent, out oHeightsCurrent, pObj);
						}

						for (int s=0; s< oPointsCurrent.Count; s++) {
							oPoints.Add(oPointsCurrent[s]);
							oHeights.Add(oHeightsCurrent[s]);
						}

					}
				}
			}
		}

		return result;
	}
	

	static Vector3[] AddOverlapPointsToShape(Vector3[] shape, Vector3[] shapeOther, bool sameShape, out List<Vector3> oPointsCurrent, out List<float> oHeightsCurrent, PolyLassoObject pObj) {   
		oPointsCurrent = new List<Vector3>();
		oHeightsCurrent = new List<float>();

		List<Vector3> result = new List<Vector3>();

		for (int i=0; i < shape.Length; i++) {
			result.Add(shape[i]);

			Vector2 ps1 = new Vector2(shape[i].x, shape[i].z); 
			Vector2 pe1 = new Vector2(shape[0].x, shape[0].z); 
			if (i < (shape.Length - 1)) {
				ps1 = new Vector2(shape[i].x, shape[i].z); 
				pe1 = new Vector2(shape[i+1].x, shape[i+1].z);
			}
			List<Vector3> intersectionPoints = new List<Vector3>();

			for (int m=0; m < shapeOther.Length; m++) {
				bool allowIntersect = false;
				if ((i != m) && (i != (m+1)) && (i != (m-1)) && ( !((i==0)&&(m == (shape.Length - 1))) ) && ( !((m==0)&&(i == (shape.Length - 1)))) ){
					allowIntersect = true;
				}
				if (sameShape == false) {
					allowIntersect = true;
				}

				if (allowIntersect) {
					Vector2 ps2 = new Vector2(shapeOther[m].x, shapeOther[m].z);
					Vector2 pe2 = new Vector2(shapeOther[0].x, shapeOther[0].z);
					if (m < (shapeOther.Length - 1)) {
						ps2 = new Vector2(shapeOther[m].x, shapeOther[m].z);
						pe2 = new Vector2(shapeOther[m+1].x, shapeOther[m+1].z);
					}
					
					if (TestLinesIntersection(ps1, pe1, ps2, pe2)) {
						if (TestSegmentIntersection(ps1, pe1, ps2, pe2)) {
							
							Vector2 intersectionPoint = LineIntersectionPoint(ps1, pe1, ps2, pe2);
							if ( (!IsPointsEqual(ps1, intersectionPoint)) && (!IsPointsEqual(pe1, intersectionPoint)) ) {
								intersectionPoints.Add( new Vector3(intersectionPoint.x, shape[0].y, intersectionPoint.y));

								if (sameShape == false) {
									if (pObj != null) {
										if (pObj.overlapAmounts != null) {
											if (pObj.overlapAmounts.Length > 0) {
												oPointsCurrent.Add(new Vector3(intersectionPoint.x, shape[0].y, intersectionPoint.y));

												float hs = pObj.overlapAmounts[m];
												float he = pObj.overlapAmounts[0];
												if (m < (shapeOther.Length - 1)) he = pObj.overlapAmounts[m+1];

												float segmentDist = Vector2.Distance(ps2, pe2);
												float intersPointDist = Vector2.Distance(ps2, intersectionPoint);

												oHeightsCurrent.Add(Mathf.Lerp(hs, he, intersPointDist / segmentDist));

											}
										}
									}
								}


							}
						}
					}
				}
			}
			intersectionPoints = MergeShapeDuplicatePoints(intersectionPoints);
			intersectionPoints = SortVector3ListByDistanceToPoint(intersectionPoints, shape[i]);

			for (int n=0; n< intersectionPoints.Count; n++) {
				result.Add(intersectionPoints[n]);
			}
		}

		return result.ToArray();
	}

	static float[] CreateShapeOverlapAmountArray(Vector3[] shape, float overlapAmount, Vector3[] shapeWithoutIntersections, List<Vector3> oPoints, List<float> oHeights, bool overlapStartInvert) {
		float[] result = new float[shape.Length];
		int direction = 1;
		if (overlapStartInvert) direction = -1;

		for (int i=0; i< shape.Length; i++) {
			bool match = false;
			for (int m=0; m< shapeWithoutIntersections.Length; m++) {
				if (IsPointsEqual3D(shape[i], shapeWithoutIntersections[m])) {
					match = true;
					break;
				}                                   
			}
			if (!match) {
				bool hMatch = false;
				float addHeight = 0;
				for (int s=0; s< oPoints.Count; s++) {
					if (IsPointsEqual3D(shape[i], oPoints[s])) {
						hMatch = true;
						addHeight = oHeights[s];
						break;
					}
				}

				if (!hMatch) {
					result[i] = overlapAmount * direction;
					direction = direction * -1;
				}
				else {
					result[i] = overlapAmount * direction + addHeight;
					direction = direction * -1;

				}
			}
		}
		
		return result;
	}
	

	static float[] SmoothOverlapAmountArray(Vector3[] shape, float[] overlapAmounts) {
		List<float> result = new List<float>();

		List<List<float>> heightArrays = new List<List<float>>();
		List<List<float>> pointDistancesFromStart = new List<List<float>>();
		List<float> distances = new List<float>();

		float distance = 0;
		List<float> heightArray = new List<float>();
		heightArray.Add(overlapAmounts[0]);

		List<float> pointDistanceFromStart = new List<float>();
		pointDistanceFromStart.Add(0);

		//split to arrays
		for (int i=0; i < shape.Length; i++) {
			Vector3 pointA = shape[i];
			int pointBindex = 0;
			if (i == (shape.Length - 1)) pointBindex = 0;
			else pointBindex = i+1;
			Vector3 pointB = shape[pointBindex];

			heightArray.Add(overlapAmounts[pointBindex]);

			distance = distance + Vector3.Distance(pointA, pointB);

			pointDistanceFromStart.Add(distance);

			if (overlapAmounts[pointBindex] != 0) {
				heightArrays.Add(heightArray);
				distances.Add(distance);
				pointDistancesFromStart.Add(pointDistanceFromStart);

				heightArray = new List<float>();
				heightArray.Add(overlapAmounts[pointBindex]);
				distance = 0;

				pointDistanceFromStart = new List<float>();
				pointDistanceFromStart.Add(0);
			}
		}
		heightArrays.Add(heightArray);
		distances.Add(distance);
		pointDistancesFromStart.Add(pointDistanceFromStart);

		//smooth heights
		for (int i=0; i< heightArrays.Count; i++) {
			for (int s=0; s < heightArrays[i].Count-1; s++) {
				float pointDistance = 0;
	
				pointDistance = pointDistancesFromStart[i][s];
				float pointRate = (float)pointDistance / (float)distances[i];

				float resultH = Mathf.Lerp(heightArrays[i][0], heightArrays[i][heightArrays[i].Count-1], pointRate);
				if (pointDistance <=  (distances[i] * 0.5f)) {
					resultH = Mathf.SmoothStep(heightArrays[i][0], resultH, (float)pointDistance / ( (float)distances[i] * 0.5f) );
				}
				else {
					resultH = Mathf.SmoothStep(heightArrays[i][heightArrays[i].Count-1], resultH,  (float)(distances[i] - pointDistance) / (float)(distances[i] * 0.5f) );
				}
				 
				heightArrays[i][s] = resultH;

			}
		}


		//combine
		for (int i=0; i< heightArrays.Count; i++) {
			for (int s=0; s < heightArrays[i].Count-1; s++) {
				result.Add(heightArrays[i][s]);
			}
		}

		return result.ToArray();
	}

	static Vector3[] ApplyIntersectionOverlapOffset(Vector3[] points, float offset) {
		Vector3[] result = new Vector3[points.Length];
		for (int i=0; i< points.Length; i++) {
			result[i] = new Vector3(points[i].x, points[i].y + offset, points[i].z);
		}
		return result;
	}


	static void MakePlanarUVs(PolyLassoObject pObj, bool random) {
		float offsetX = 0;
		float offsetZ = 0;
		if (random) {
			offsetX	= Random.Range(-1.0f, 1.0f);
			offsetZ	= Random.Range(-1.0f, 1.0f);
		}

		MeshFilter meshFilter = (MeshFilter)pObj.GetComponent<MeshFilter>();
		if (meshFilter) {
			Mesh mesh = (Mesh)meshFilter.sharedMesh;
			Vector3[] vertices = mesh.vertices;
			Vector2[] uvs = new Vector2[vertices.Length];
		
			for (int i=0; i < uvs.Length; i++) {
				uvs[i] = new Vector2(vertices[i].x + 0.5f + offsetX, vertices[i].z + offsetZ);
			}
			mesh.uv = uvs;
		}
	}


	static void PolygonLassoInstantiateCornerCircleDetails(PolyLassoObject obj, List<Vector2> circleCentersMetConditions, GameObject cornerDetailPrefab, float cornerDetailSize, bool isCornerDetailCornerOriented, float height) {
		if (obj.details == null) {
			obj.details = new List<GameObject>();
		}
		Renderer renderer = (Renderer)cornerDetailPrefab.GetComponent<Renderer>();
		Material mat = new Material(renderer.sharedMaterial);
		for (int i=0; i < circleCentersMetConditions.Count; i++) {
			GameObject detail = (GameObject)Instantiate(cornerDetailPrefab, new Vector3(circleCentersMetConditions[i].x, obj.gameObject.transform.position.y + height, circleCentersMetConditions[i].y), Quaternion.identity);
			Renderer r = (Renderer)detail.GetComponent<Renderer>();
			r.sharedMaterial = mat;
			detail.transform.localScale = detail.transform.localScale * cornerDetailSize;
			detail.transform.parent = obj.gameObject.transform;
			obj.details.Add(detail);
		}
	}


	public static GameObject PolygonLassoBuildObject(GameObject originalTransformForCeltic, bool isChild, List<Vector3> points, float bevelAmount, int bevelSteps, float[] offsets, float[] heights,
	                                                 List<DecalSet> decalSets, 
	                                                 List<bool> inheritMatGroup,
	                                                 List<bool> scatterOnShapeVerts, 
	                                                 List<bool> trim, 
	                                                 List<bool> perpTrim, 
	                                                 List<bool> fitDecals,
	                                                 List<float> trimOffset, 
	                                                 List<float> decalOffset,
	                                                 List<float> decalOffsetRandom,
	                                                 List<float> decalGap, 
	                                                 List<float> decalGapRandom, 
	                                                 List<float> decalSize, 
	                                                 List<float> decalSizeRandom, 
	                                                 List<float> decalRotation, 
	                                                 List<float> decalRotationRandom,
	                                                 bool noise,
	                                                 float shapeSubdiv,
	                                                 Vector2 noise1Amount,
	                                                 float noise1Frequency,
	                                                 Vector2 noise2Amount,
	                                                 float noise2Frequency,
	                                                 int materialID,
	                                                 bool isFloater,
	                                                 bool isTube,
	                                                 bool isOpen,
	                                                 float thickness,
	                                                 Vector2[] section,
	                                                 bool isAdaptive,
	                                                 float adaptiveStep,
	                                                 float[] lengthOffsets0,
	                                                 float[] lengthOffsets1,
	                                                 float[] lengthOffsets2,
	                                                 float[] heightOffsets0,
	                                                 float[] heightOffsets1,
	                                                 float[] heightOffsets2,
	                                                 int repeatSize,
	                                                 int[] lengthOffsetOrder,
	                                                 int[] heightOffsetOrder,
	                                                 bool edgeWiseOffset,
	                                                 bool lengthWiseOffset,
	                                                 float offsetMinEdge,
	                                                 PolyLassoCorner corner,
	                                                 float[] childProfileVerticalOffsets, 
	                                                 float[] childProfileDepthOffsets,
	                                                 int[] childProfileHorisontalOffsets,
	                                                 int[] childProfileMatGroups,
	                                                 PolyLassoRelativeShape[] childProfileShapes,
	                                                 PolyLassoProfile[] followerProfiles, 
	                                                 float[] followerProfileVerticalOffsets, 
	                                                 float[] followerProfileDepthOffsets,
	                                                 int[] followerProfileMatGroups,
	                                                 Texture2D cutoff, 
	                                                 Vector2 cutoffTiling,
	                                                 Texture2D bumpMap, 
	                                                 float bumpMapIntensity, 
	                                                 Vector2 bumpMapTiling,
	                                                 Texture2D aoMap,
	                                                 float aoMapIntensity,
	                                                 bool randomUvOffset,
	                                                 int stoneType,
	                                                 bool allowIntersections,
	                                                 bool overlapIntersections,
	                                                 float overlapAmount,
	                                                 bool usedForOverlapping, 
	                                                 bool overlapStartInvert, 
	                                                 bool curveUVs) {

		if ((isOpen)||(allowIntersections)) {	 
		}
		else {
			Vector2[] cleanShape = RemoveAllSandglass(Points3DTo2D( points.ToArray()));
			points = Points2DTo3DList(cleanShape, points[0].y);
		}

		GameObject polygonLassoCreatedObject = new GameObject();
		polygonLassoCreatedObject.name = "polyLassoObject";
		PolyLassoObject polyLassoObject = (PolyLassoObject)polygonLassoCreatedObject.AddComponent<PolyLassoObject>();
		polyLassoObject.isChild = isChild;

		polyLassoObject.surforgeSettings = surforgeSettings;

		polyLassoObject.shape = points;

		polygonLassoCreatedObject.transform.parent = surforgeSettings.root.transform;

		polygonLassoCreatedObject.AddComponent(typeof(MeshRenderer));


		polygonLassoCreatedObject.transform.position = new Vector3(0, points[0].y, 0);

		List<DecalSet> newDecalSets = new List<DecalSet>();
		for (int i=0; i<decalSets.Count; i++) {
			newDecalSets.Add (decalSets[i]);
		}

		RebuildPolyLassoObject(originalTransformForCeltic, polyLassoObject, bevelAmount, bevelSteps, offsets, heights,
		                       newDecalSets, 
		                       new List<bool>(inheritMatGroup),
		                       new List<bool>(scatterOnShapeVerts), 
		                       new List<bool> (trim), 
		                       new List<bool> (perpTrim), 
		                       new List<bool> (fitDecals),
		                       new List<float>(trimOffset), 
		                       new List<float>(decalOffset), 
		                       new List<float>(decalOffsetRandom), 
		                       new List<float>(decalGap), 
		                       new List<float>(decalGapRandom),
		                       new List<float>(decalSize), 
		                       new List<float>(decalSizeRandom), 
		                       new List<float>(decalRotation),
		                       new List<float>(decalRotationRandom),
		                       noise,
		                       shapeSubdiv,
		                       noise1Amount,
		                       noise1Frequency,
		                       noise2Amount,
		                       noise2Frequency,
		                       materialID,
		                       isFloater,
		                       isTube,
		                       isOpen,
		                       thickness,
		                       section,
		                       isAdaptive,
		                       adaptiveStep,
		                       lengthOffsets0,
		                       lengthOffsets1,
		                       lengthOffsets2,
		                       heightOffsets0,
		                       heightOffsets1,
		                       heightOffsets2,
		                       repeatSize,
		                       lengthOffsetOrder,
		                       heightOffsetOrder,
		                       edgeWiseOffset,
		                       lengthWiseOffset,
		                       offsetMinEdge,
		                       corner,
		                       childProfileVerticalOffsets, 
		                       childProfileDepthOffsets,
		                       childProfileHorisontalOffsets,
		                       childProfileMatGroups,
		                       childProfileShapes,
		                       followerProfiles, 
		                       followerProfileVerticalOffsets, 
		                       followerProfileDepthOffsets,
		                       followerProfileMatGroups,
		                       cutoff, 
		                       cutoffTiling,
		                       bumpMap, 
		                       bumpMapIntensity, 
		                       bumpMapTiling,
		                       aoMap,
		                       aoMapIntensity,
		                       randomUvOffset,
		                       stoneType,
		                       allowIntersections,
		                       overlapIntersections,
		                       overlapAmount,
		                       usedForOverlapping, 
		                       overlapStartInvert, 
		                       curveUVs);

	
		RegisterUndoPolyLassoCreatedObject(polyLassoObject);

		//snap to objects
		if (surforgeSettings.polyLassoObjects == null) {
			surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
		}
		surforgeSettings.polyLassoObjects.Add (polygonLassoCreatedObject.GetComponent<PolyLassoObject>());



		return polygonLassoCreatedObject;
	}





	static Vector2[] ModifyShapeWithLengthOffsets(Vector2[] vertices2D, float sourceDistance, float[] lengthOffsets, bool[] cornerCirclesOffsetMask, Vector2[] cornerCirclesOffsetVectors, float[] cornerCirclesOffsets, Vector2[] cornerPoints, Vector2[] placeOnCircleVectors, int[] startAndEndOfCirclesMask, float[] circleCenterOffset) { 

		if (sourceDistance == 0) sourceDistance = 0.0001f;
		
		bool isOutside = true;

		Vector2[] outlineVerts = new Vector2[vertices2D.Length];
		
		Vector2 pointA;
		Vector2 pointB;
		Vector2 pointC;

		float distance = 0;

		for (int i=0; i < vertices2D.Length; i++) {

			if (cornerCirclesOffsetMask[i]) {
				distance = sourceDistance;
			}
			else {
				if (lengthOffsets.Length > i) { //test array size TODO: check for consistency
					distance = sourceDistance - lengthOffsets[i] ;  
				}
				else distance = sourceDistance;
			}


			if (distance < 0) isOutside = false;
			else isOutside = true;


			if (i==0) {
				pointA = vertices2D[vertices2D.Length-1];
				pointB = vertices2D[i];
				pointC = vertices2D[i+1];
			}
			else {
				if (i == (vertices2D.Length-1)){
					pointA = vertices2D[i-1];
					pointB = vertices2D[i];
					pointC = vertices2D[0];
				}
				else {
					pointA = vertices2D[i-1];
					pointB = vertices2D[i];
					pointC = vertices2D[i+1];
				}
			}
			
			
			Vector2 offsetedPoint = new Vector2();
			
			Vector2 v1 = pointC - pointB;
			Vector2 v2 = pointA - pointB;
			
			float cos = GetCosOfAngleBetweenVectors(v1, v2);
			float actualDistance = 0;
			
			if (Mathf.Approximately(cos, -1) || Mathf.Approximately(cos, 1)) {
				actualDistance = distance;
				Vector2 perp = new Vector2(-v1.y, v1.x); 
				offsetedPoint =  pointB + perp.normalized * actualDistance;
				
				if (IsPointInPolygon(vertices2D.Length, vertices2D, offsetedPoint.x, offsetedPoint.y, isOutside)) {
					offsetedPoint =  pointB + perp.normalized * -actualDistance;
				}
			}
			
			else {
				Vector2 offsetVectorNew = v1.normalized + v2.normalized;
				
				float div = 2 * Mathf.Sqrt((1-cos)/2 );
				if (div == 0) actualDistance = distance;
				else actualDistance = (distance * 2) / div;
				
				offsetedPoint =  pointB + offsetVectorNew.normalized * actualDistance;
				
				if (IsPointInPolygon(vertices2D.Length, vertices2D, offsetedPoint.x, offsetedPoint.y, isOutside)) {
					offsetedPoint =  pointB + offsetVectorNew.normalized * -actualDistance;
				}
			}

			if (cornerCirclesOffsetMask[i]) {
				outlineVerts[i] = cornerPoints[i] +  placeOnCircleVectors[i] * (cornerCirclesOffsets[i] -distance);

				if (startAndEndOfCirclesMask[i] != 0) { // start end points fix
					//outlineVerts[i] = outlineVerts[i] + new Vector2(placeOnCircleVectors[i].y, -placeOnCircleVectors[i].x) * distance * startAndEndOfCirclesMask[i];
				}
				else {
					outlineVerts[i] = outlineVerts[i] + cornerCirclesOffsetVectors[i].normalized * circleCenterOffset[i];
				}
			}
			else {
				outlineVerts[i] = offsetedPoint;
			}

		}
		
		return outlineVerts;
	}


	static float[] GetLengthOffsetedShape (Vector2[] shape, float[] lengthOffsets) {
		float[] lengthOffsetedShape = new float[shape.Length];
	
		int lengthOffsetsCounter = 0;
		for (int i=0; i < shape.Length; i++) {
			lengthOffsetedShape[i] = lengthOffsets[lengthOffsetsCounter];
		
			lengthOffsetsCounter++;
			if (lengthOffsetsCounter >= lengthOffsets.Length) lengthOffsetsCounter = 0;
		}
		return lengthOffsetedShape;
	}


	public static bool CheckOptimizeShapePointSkip(Vector3[] shape, int index) {
		bool result = false;
		float difference = 0;

		if (index < (shape.Length - 1)) {
			difference = Mathf.Abs(GetShapeVertexAngle(shape, index, false) - GetShapeVertexAngle(shape, index + 1, false));
		}
		else {
			difference = Mathf.Abs(GetShapeVertexAngle(shape, index, false) - GetShapeVertexAngle(shape, 0, false));
		}

		if (difference < 0.5f) result = true;

		return result;
	}

	static float GetShapeCornerAngleAtIndex(Vector2[] shape, int index) { 
		Vector2 v0;
		Vector2 v1;
		Vector2 v2;

		if (index == 0) {
			v0 = shape[shape.Length - 1];
			v1 = shape[index];
			v2 = shape[index+1];
		}
		else {
			if (index == (shape.Length - 1)) {
				v0 = shape[index - 1];
				v1 = shape[index];
				v2 = shape[0];
			}
			else {
				v0 = shape[index - 1];
				v1 = shape[index];
				v2 = shape[index+1];
			}
		}

		float cos = GetCosOfAngleBetweenVectors( (v0-v1).normalized, (v2-v1).normalized );
		float result = Mathf.Acos(cos) * (180 / Mathf.PI);

		return result;
	}


	static bool CheckOptimizeShapeLengthOffsetsSkip(float[] shapeLengthOffsets, int index, bool isCreased) {
		bool result = false;

		if (shapeLengthOffsets.Length < 4) {
			return false;
		}

		float forwardDifference = 0;
		float forwardForwardDifference = 0;
		float backDifference = 0;
		float backBackDifference = 0;

		if (index == 1) {
			forwardForwardDifference = forwardDifference = Mathf.Abs( shapeLengthOffsets[index + 1] - shapeLengthOffsets[index + 2] );
			forwardDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index + 1] );
			backDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index - 1] );
			backBackDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[shapeLengthOffsets.Length - 1] );
		}
		else {
			if (index == 0) {
				forwardForwardDifference = forwardDifference = Mathf.Abs( shapeLengthOffsets[index + 1] - shapeLengthOffsets[index + 2] );
				forwardDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index + 1] );
				backDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[shapeLengthOffsets.Length - 1] );
				backBackDifference = Mathf.Abs( shapeLengthOffsets[shapeLengthOffsets.Length - 1] - shapeLengthOffsets[shapeLengthOffsets.Length - 2] );
			}
			else {
				if (index == (shapeLengthOffsets.Length - 2)) {
					forwardForwardDifference = forwardDifference = Mathf.Abs( shapeLengthOffsets[index + 1] - shapeLengthOffsets[0] );
					forwardDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index + 1] );
					backDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index - 1] );
					backBackDifference = Mathf.Abs( shapeLengthOffsets[index - 1] - shapeLengthOffsets[index - 2] );
				}
				else {
					if (index == (shapeLengthOffsets.Length - 1)) {
						forwardForwardDifference = Mathf.Abs( shapeLengthOffsets[0] - shapeLengthOffsets[1] );
						forwardDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[0] );
						backDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index - 1] );
						backBackDifference = Mathf.Abs( shapeLengthOffsets[index - 1] - shapeLengthOffsets[index - 2] );
					}
					else {
						forwardForwardDifference = forwardDifference = Mathf.Abs( shapeLengthOffsets[index + 1] - shapeLengthOffsets[index + 2] );
						forwardDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index + 1] );
						backDifference = Mathf.Abs( shapeLengthOffsets[index] - shapeLengthOffsets[index - 1] );
						backBackDifference = Mathf.Abs( shapeLengthOffsets[index - 1] - shapeLengthOffsets[index - 2] );
					}
				}
			}
		}

		if (isCreased) {
			if ((forwardDifference < 0.01f) && (forwardForwardDifference < 0.01f) && (backDifference < 0.01f) && (backBackDifference < 0.01f)) result = true;
		}
		else {
			if ((forwardDifference < 0.01f)  && (backDifference < 0.01f) ) result = true;
		}
		
		return result;
	}


	public static void CollapseSubdiv(PolyLassoObject pObj) {
		Debug.Log ("collapse subdiv");
	}


	public static Vector2[] MultiBevel(Vector2[] points, float bevel, int steps, bool isOpen) {
		Vector2[] result = points;

		float multiBevel = bevel;

		for (int i=0; i< steps; i++) {
			result = Bevel2DShape(result, multiBevel, isOpen);
			multiBevel = multiBevel * 0.5f;
		}

		return result;
	}


	static float GetBeveledLineLength(Vector2 v1, Vector2 v2, float bevel) {
		float cos = (v1.x * v2.x + v1.y * v2.y ) / (Mathf.Sqrt (Mathf.Pow (v1.x, 2) + Mathf.Pow (v1.y, 2)) * Mathf.Sqrt (Mathf.Pow (v2.x, 2) + Mathf.Pow (v2.y, 2)));
		return bevel / Mathf.Sqrt(2 - 2 * cos);
	}
	
	public static float GetCosOfAngleBetweenVectors(Vector2 v1, Vector2 v2) {
		return Vector2.Dot(v1.normalized, v2.normalized);
	}

	static float DegreesToRadians(float angle) {
		return (Mathf.PI / 180.0f) * angle;
	}

	static Vector2 GetSegmentMiddle(Vector2 v1, Vector2 v2) {
		return new Vector2((v1.x + v2.x) * 0.5f, (v1.y + v2.y) * 0.5f); 
	}




	static Vector2[] Bevel2DShape (Vector2[] points, float bevel, bool isOpen) {
		Vector2[] result = new Vector2[points.Length * 2];

		for (int i=0; i< points.Length; i++) {

			if (i == 0) {
				Vector2 p1 = points[points.Length - 1] - points[i];  
				Vector2 p2 = points[i+1] - points[i];
				float bevelLength = GetBeveledLineLength(p1, p2, bevel);

				float side1Length = Mathf.Sqrt( Mathf.Pow( (points[points.Length - 1].x - points[i].x), 2) + Mathf.Pow( (points[points.Length - 1].y - points[i].y), 2) );
				float side2Length = Mathf.Sqrt( Mathf.Pow( (points[i+1].x - points[i].x), 2) + Mathf.Pow( (points[i+1].y - points[i].y), 2) );

				float factor1 = side1Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));
				float factor2 = side2Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));

				if (((side1Length - (bevelLength * 2)) <= bevel) || ((side2Length - (bevelLength * 2)) <= bevel)) {
					p1.Normalize();
					p1 = p1 * Mathf.Min (factor1, factor2) ;

					p2.Normalize();
					p2 = p2 * Mathf.Min (factor1, factor2) ;
				}
				else {
					p1.Normalize();
					p1 = p1 * bevelLength;

					p2.Normalize();
					p2 = p2 * bevelLength;
				}
				if (isOpen) result[i*2] = points[i];
				else result[i*2] = p1 + points[i];

				result[i*2 + 1] = p2 + points[i];


			}
			else {
				if (i == (points.Length -1) ) {
					Vector2 p1 = points[i-1] - points[i];
					Vector2 p2 = points[0] - points[i];
					float bevelLength = GetBeveledLineLength(p1, p2, bevel);

					float side1Length = Mathf.Sqrt( Mathf.Pow( (points[i-1].x - points[i].x), 2) + Mathf.Pow( (points[i-1].y - points[i].y), 2) );
					float side2Length = Mathf.Sqrt( Mathf.Pow( (points[0].x - points[i].x), 2) + Mathf.Pow( (points[0].y - points[i].y), 2) );

					float factor1 = side1Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));
					float factor2 = side2Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));

					if (((side1Length - (bevelLength * 2)) <= bevel) || ((side2Length - (bevelLength * 2)) <= bevel)) {
						p1.Normalize();
						p1 = p1 * Mathf.Min (factor1, factor2) ;
						
						p2.Normalize();
						p2 = p2 * Mathf.Min (factor1, factor2) ;
					}
					else {
						p1.Normalize();
						p1 = p1 * bevelLength;
						
						p2.Normalize();
						p2 = p2 * bevelLength;
					}
					result[i*2] = p1 + points[i];

					if (isOpen) result[i*2 + 1] = points[points.Length -1];
					else result[i*2 + 1] = p2 + points[i];
			
				}
				else {
					Vector2 p1 = points[i-1] - points[i];
					Vector2 p2 = points[i+1] - points[i];
					float bevelLength = GetBeveledLineLength(p1, p2, bevel);

					float side1Length = Mathf.Sqrt( Mathf.Pow( (points[i-1].x - points[i].x), 2) + Mathf.Pow( (points[i-1].y - points[i].y), 2) );
					float side2Length = Mathf.Sqrt( Mathf.Pow( (points[i+1].x - points[i].x), 2) + Mathf.Pow( (points[i+1].y - points[i].y), 2) );

					float factor1 = side1Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));
					float factor2 = side2Length / (2.0f + Mathf.Sqrt (2 - 2 * GetCosOfAngleBetweenVectors(p1, p2)));

					if (((side1Length - (bevelLength * 2)) <= bevel) || ((side2Length - (bevelLength * 2)) <= bevel)) {
						p1.Normalize();
						p1 = p1 * Mathf.Min (factor1, factor2) ;
						
						p2.Normalize();
						p2 = p2 * Mathf.Min (factor1, factor2) ;
					}
					else {
						p1.Normalize();
						p1 = p1 * bevelLength;
						
						p2.Normalize();
						p2 = p2 * bevelLength;
					}
					result[i*2] = p1 + points[i];
					result[i*2 + 1] = p2 + points[i];
				
				}
			}


		}

		return result;
	}

	public static bool CheckIfShapeClockwise(List<Vector2> points) {
		float summ = 0;
		for (int i=0; i< points.Count; i++) {
			if ( i == points.Count-1) {
				summ = summ + ((points[0].x - points[i].x) * (points[0].y + points[i].y));
			}
			else {
				summ = summ + ((points[i+1].x - points[i].x) * (points[i+1].y + points[i].y));
			}
		}
		if(summ >= 0) return true;
		else return false;
	}
	

	public static bool CheckIfShapeClockwise(Vector3[] points) {
		float summ = 0;
		for (int i=0; i< points.Length; i++) {
			if ( i == points.Length-1) {
				summ = summ + ((points[0].x - points[i].x) * (points[0].z + points[i].z));
			}
			else {
				summ = summ + ((points[i+1].x - points[i].x) * (points[i+1].z + points[i].z));
			}
		}
		if(summ >= 0) return true;
		else return false;
	}
	

	static void AddVertexRowToMesh(GameObject obj, Vector3[] pointsB) {

		MeshFilter sourceMeshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
		Mesh sourceMesh = (Mesh)sourceMeshFilter.sharedMesh;
		
		Vector3[] sourceVertices = sourceMesh.vertices;
		int[] sourceTriangles = sourceMesh.triangles;

		Vector3[] vertices = new Vector3[sourceVertices.Length + pointsB.Length];
		for (int v=0; v< sourceVertices.Length; v++) {
			vertices[v] = sourceVertices[v];
		}
		for (int w=0; w < pointsB.Length; w++) {
			vertices[sourceVertices.Length + w] = pointsB[w];
		}

		int[] triangles = new int[sourceTriangles.Length + pointsB.Length * 6]; 
		for (int t=0; t < sourceTriangles.Length; t++) {
			triangles[t] = sourceTriangles[t];
		}

		sourceMesh.vertices = vertices;
		sourceMesh.triangles = triangles;
		sourceMesh.RecalculateNormals();
		sourceMesh.RecalculateBounds();
	}


	//ConvexHull 
	static void CreateConvexHull(GameObject obj, List<Vector3> shape, int stoneType, float bevelAmount, int bevelSteps) {
		//get center vertex and bounds
		float centerX = 0;
		float centerZ = 0;
		
		float minX = Mathf.Infinity;
		float maxX = Mathf.NegativeInfinity;
		
		float minZ = Mathf.Infinity;
		float maxZ = Mathf.NegativeInfinity;
		
		for (int i=0; i< shape.Count; i++) {
			centerX = centerX + shape[i].x;
			centerZ = centerZ + shape[i].z;
			
			if (shape[i].x < minX) minX = shape[i].x;
			if (shape[i].x > maxX) maxX = shape[i].x;
			
			if (shape[i].z < minZ) minZ = shape[i].z;
			if (shape[i].z > maxZ) maxZ = shape[i].z;
		}
		centerX = centerX / (float)shape.Count;
		centerZ = centerZ / (float)shape.Count;



		MeshFilter sourceMeshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
		Mesh sourceMesh = (Mesh)sourceMeshFilter.sharedMesh;

		List<Vector3>  sourceVertices = new List<Vector3>();
		for (int i=0; i< sourceMesh.vertices.Length; i++) {
			sourceVertices.Add(sourceMesh.vertices[i]);
		}


		// add random points
		Vector2[] poly = new Vector2[shape.Count];
		for (int i=0; i<poly.Length; i++) {
			poly[i] = new Vector2(shape[i].x, shape[i].z);
		}


		// stone type 1 settings
		if (stoneType == 1 ) {
			float randomSize = 20.0f;
			for (int i=0; i< 50; i++) {
				Vector3 newPoint = new Vector3(centerX + Random.Range(-randomSize, randomSize), shape[0].y+ 2.0f + Random.Range(-randomSize, randomSize) * 0.02f , centerZ + Random.Range(-randomSize, randomSize));
				if (IsPointInPolygon(poly.Length, poly, newPoint.x, newPoint.z, true)) {
					sourceVertices.Add(newPoint);
				}
			}
		}


		// stone type 2 settings
		if (stoneType == 2 ) {
			float randomSize = 10.0f;
			for (int i=0; i< 50; i++) {
				Vector3 newPoint = new Vector3(centerX + Random.Range(-randomSize, randomSize), shape[0].y+ 2.0f + Random.Range(-randomSize, randomSize) * 0.1f , centerZ + Random.Range(-randomSize, randomSize));
				if (IsPointInPolygon(poly.Length, poly, newPoint.x, newPoint.z, true)) {
					sourceVertices.Add(newPoint);
				}
			}
		}

		// stone type 3 (gem A) settings
		if (stoneType == 3 ) {
			Vector3 newPoint = new Vector3(centerX, shape[0].y+ 2.0f, centerZ);
			if (IsPointInPolygon(poly.Length, poly, newPoint.x, newPoint.z, true)) {
				sourceVertices.Add(newPoint);
			}
		}

		// stone type 4 (gem B) settings
		if (stoneType == 4 ) {
			Vector3 newPoint = new Vector3(centerX, shape[0].y+ 1.6f, centerZ);
			if (IsPointInPolygon(poly.Length, poly, newPoint.x, newPoint.z, true)) {
				sourceVertices.Add(newPoint);
			}

			float minDist = Vector2.Distance(new Vector2(shape[0].x, shape[0].z), new Vector2(centerX, centerZ));
			for (int i=0; i< shape.Count; i++) {
				float newDist = Vector2.Distance(new Vector2(shape[i].x, shape[i].z), new Vector2(centerX, centerZ));
				if (newDist < minDist) minDist = newDist;
			}
			
			List<Vector3> offsettedGemShape = new List<Vector3>();
			Vector2[] shape2d = MultiBevel(  RemoveAllSandglass(CreateShapeOutline( Points3DTo2D(shape.ToArray()), minDist * -0.35f, false)  ), bevelAmount, bevelSteps, false);
			offsettedGemShape = Points2DTo3DList(shape2d, shape[0].y+ 1.5f);

			List<Vector3> offsettedGemShapeInner = new List<Vector3>();
			Vector2[] shape2dInner = MultiBevel(  RemoveAllSandglass(CreateShapeOutline( Points3DTo2D(shape.ToArray()), minDist * -0.4f, false)  ), bevelAmount, bevelSteps, false);
			offsettedGemShapeInner = Points2DTo3DList(shape2dInner, shape[0].y+ 1.6f);

			for (int i=0; i<offsettedGemShape.Count; i++) {
				if (IsPointInPolygon(poly.Length, poly, offsettedGemShape[i].x, offsettedGemShape[i].z, true)) {
					sourceVertices.Add(offsettedGemShape[i]);
				}
			}
			for (int i=0; i<offsettedGemShapeInner.Count; i++) {
				if (IsPointInPolygon(poly.Length, poly, offsettedGemShapeInner[i].x, offsettedGemShapeInner[i].z, true)) {
					sourceVertices.Add(offsettedGemShapeInner[i]);
				}
			}
		}



		Vertex3[] vertices = new Vertex3[sourceVertices.Count];
		for (int i=0; i< sourceVertices.Count; i++) {
			vertices[i] = new Vertex3(sourceVertices[i].x, sourceVertices[i].y, sourceVertices[i].z);
		}

		ConvexHull<Vertex3, Face3> convexHull = ConvexHull.Create<Vertex3, Face3>(vertices);

		List<Vertex3> convexHullVertices = new List<Vertex3>(convexHull.Points);
		List<Face3> convexHullFaces = new List<Face3>(convexHull.Faces);
		List<int> convexHullIndices = new List<int>();
		
		foreach(Face3 f in convexHullFaces)
		{
			convexHullIndices.Add(convexHullVertices.IndexOf(f.Vertices[0]));
			convexHullIndices.Add(convexHullVertices.IndexOf(f.Vertices[1]));
			convexHullIndices.Add(convexHullVertices.IndexOf(f.Vertices[2]));
		}

		List<Vector3> resultMeshVertices = new List<Vector3>();

		for (int i=0; i < convexHullIndices.Count; i++) {
			foreach(Face3 f in convexHullFaces) {
				bool found = false;
				for (int t=0; t < 3; t++) {
					if (convexHullVertices.IndexOf(f.Vertices[t]) == i) {
						resultMeshVertices.Add(f.Vertices[t].ToVector3());
						found = true;
						break;
					}
				}
				if (found) break;
			}
		}


		sourceMesh.Clear(); 

		sourceMesh.vertices = resultMeshVertices.ToArray();
		sourceMesh.triangles = convexHullIndices.ToArray();

		sourceMesh.RecalculateNormals();
		sourceMesh.RecalculateBounds();

		  
		//subdivide
		if (stoneType == 2) {

			Mesh meshSubd = sourceMesh;
			MeshHelper.Subdivide(meshSubd, 3);  
			sourceMesh = meshSubd;
		}

	}


	static void Apply3DNoise(GameObject obj, Vector3[] deformerDerivates) {
		MeshFilter sourceMeshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();

		if (sourceMeshFilter) {
			Mesh sourceMesh = (Mesh)sourceMeshFilter.sharedMesh;

			int[] meshTriangles = sourceMesh.triangles;
			Vector3[] offsettedVerts = new Vector3[sourceMesh.vertices.Length];

			for (int i=0; i < sourceMesh.vertices.Length; i++) {
			
				offsettedVerts[i] = sourceMesh.vertices[i] + new Vector3(deformerDerivates[i].x, deformerDerivates[i].y, deformerDerivates[i].z);
			}


			sourceMesh.Clear(); 

			sourceMesh.vertices = offsettedVerts;
			sourceMesh.triangles = meshTriangles;

			sourceMesh.RecalculateNormals();
			sourceMesh.RecalculateBounds();

		}


	}

	static Vector3[] GetNoiseDeformerDerivatesForMesh(GameObject obj, Vector3 mod, Vector2 noise1Amount, float noise1Frequency, Vector2 noise2Amount, float noise2Frequency) {
		MeshFilter sourceMeshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
		Mesh sourceMesh = (Mesh)sourceMeshFilter.sharedMesh;


		Vector3[] result = new Vector3[sourceMesh.vertices.Length];
		
		for (int i=0; i < sourceMesh.vertices.Length; i++) {
			Vector3 point = new Vector3(sourceMesh.vertices[i].x, sourceMesh.vertices[i].y, sourceMesh.vertices[i].z) + mod;
			float frequency = noise1Frequency; //0.2f;
			int octaves = 8;
			float lacunarity = 4.0f;
			float persistence = 0.1f;
			
			NoiseMethod method = Noise.methods[1][2];
			NoiseSample sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
			
			float frequency2 = noise2Frequency; //1.0f;
			int octaves2 = 8;
			float lacunarity2 = 1.0f;
			float persistence2 = 0.05f;
			
			NoiseSample sample2 = Noise.Sum(method, point, frequency2, octaves2, lacunarity2, persistence2);
			
			
			result[i] = new Vector3(sample.derivative.x * noise1Amount.x + sample2.derivative.x * noise2Amount.x,
			                        sample.derivative.y * noise1Amount.x + sample2.derivative.y * noise2Amount.x,
			                        sample.derivative.z * noise1Amount.y + sample2.derivative.z * noise2Amount.y);
		}
		return result;
	}
	




	static void ExtrudeOutline(GameObject obj, Vector3[] pointsB, bool isTube) {
		bool isShapeClockwise = CheckIfShapeClockwise(pointsB);
		if (isTube) isShapeClockwise = false;

		MeshFilter sourceMeshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
		Mesh sourceMesh = (Mesh)sourceMeshFilter.sharedMesh;

		Vector3[] sourceVertices = sourceMesh.vertices;
		int[] sourceTriangles = sourceMesh.triangles;


		Vector3[] vertices = new Vector3[sourceVertices.Length + pointsB.Length];
		for (int v=0; v< sourceVertices.Length; v++) {
			vertices[v] = sourceVertices[v];
		}
		for (int w=0; w < pointsB.Length; w++) {
			vertices[sourceVertices.Length + w] = pointsB[w];
		}


		int[] triangles = new int[sourceTriangles.Length + pointsB.Length * 6]; 
		for (int t=0; t < sourceTriangles.Length; t++) {
			triangles[t] = sourceTriangles[t];
		}


		for (int i=0; i < pointsB.Length; i++) {
			if (i == (pointsB.Length - 1)) {
				if (!isShapeClockwise) {
					triangles[sourceTriangles.Length + i*6] = sourceVertices.Length - pointsB.Length + i;
					triangles[sourceTriangles.Length + i*6 + 1] = sourceVertices.Length - pointsB.Length;
					triangles[sourceTriangles.Length + i*6 + 2] = sourceVertices.Length + i;
				
					triangles[sourceTriangles.Length + i*6 + 3] = sourceVertices.Length + i;
					triangles[sourceTriangles.Length + i*6 + 4] = sourceVertices.Length - pointsB.Length;
					triangles[sourceTriangles.Length + i*6 + 5] = sourceVertices.Length;
				}
				else {
					triangles[sourceTriangles.Length + i*6] = sourceVertices.Length + i;
					triangles[sourceTriangles.Length + i*6 + 1] = sourceVertices.Length - pointsB.Length;
					triangles[sourceTriangles.Length + i*6 + 2] = sourceVertices.Length - pointsB.Length + i;

					triangles[sourceTriangles.Length + i*6 + 3] = sourceVertices.Length;
					triangles[sourceTriangles.Length + i*6 + 4] = sourceVertices.Length - pointsB.Length;
					triangles[sourceTriangles.Length + i*6 + 5] = sourceVertices.Length + i;
				}
			}

			else {
				if (!isShapeClockwise) {
					triangles[sourceTriangles.Length + i*6]     = sourceVertices.Length - pointsB.Length +  i;
					triangles[sourceTriangles.Length + i*6 + 1] = sourceVertices.Length - pointsB.Length +  i + 1;
					triangles[sourceTriangles.Length + i*6 + 2] = sourceVertices.Length + i;

					triangles[sourceTriangles.Length + i*6 + 3] = sourceVertices.Length + i;
					triangles[sourceTriangles.Length + i*6 + 4] = sourceVertices.Length - pointsB.Length + i + 1;
					triangles[sourceTriangles.Length + i*6 + 5] = sourceVertices.Length + i + 1;
				}
				else {
					triangles[sourceTriangles.Length + i*6] = sourceVertices.Length + i;
					triangles[sourceTriangles.Length + i*6 + 1] = sourceVertices.Length - pointsB.Length +  i + 1;
					triangles[sourceTriangles.Length + i*6 + 2]     = sourceVertices.Length - pointsB.Length +  i;

					triangles[sourceTriangles.Length + i*6 + 3] = sourceVertices.Length + i + 1;
					triangles[sourceTriangles.Length + i*6 + 4] = sourceVertices.Length - pointsB.Length + i + 1;
					triangles[sourceTriangles.Length + i*6 + 5] = sourceVertices.Length + i;
				}
			}
		}


		sourceMesh.vertices = vertices;
		sourceMesh.triangles = triangles;
		sourceMesh.RecalculateNormals();
		sourceMesh.RecalculateBounds();
	}


	static Vector3[] TubeSectionTo3DPointsOnCurve (Vector2[] section, Vector3[] curvePoints, int pointNumber, float thickness, float height, bool isOpen) {
		Vector3[] result = new Vector3[section.Length];

		for (int i=0; i< section.Length; i++) {

			float rad = DegreesToRadians(GetShapeVertexAngle(curvePoints, pointNumber, isOpen));

			Vector3 sectionPoint3d = new Vector3(0, section[i].y * thickness + height, section[i].x * thickness); 

			float c = Mathf.Cos(-rad);
			float s = Mathf.Sin(-rad);

			float x1 = sectionPoint3d.x * c - sectionPoint3d.z * s;
			float z1 = sectionPoint3d.x * s + sectionPoint3d.z * c;

			float y1 = sectionPoint3d.y;

			sectionPoint3d = new Vector3(x1, y1, z1);

			Vector3 point3d = new Vector3(curvePoints[pointNumber].x + sectionPoint3d.x, curvePoints[pointNumber].y + sectionPoint3d.y, curvePoints[pointNumber].z + sectionPoint3d.z);


			result[i] = point3d; 

		}
		return result;
	}
	

	public static List<Vector3> Points2DTo3DList (Vector2[] vertices2D, float height) {
		List<Vector3> result = new List<Vector3>();
		for (int i=0; i< vertices2D.Length; i++) {
			Vector3 point3d = new Vector3(vertices2D[i].x, height, vertices2D[i].y);
			result.Add (point3d); 
		}
		return result;
	}

	static Vector3[] Points2DTo3D (Vector2[] vertices2D, float height) {
		Vector3[] result = new Vector3[vertices2D.Length];
		for (int i=0; i< vertices2D.Length; i++) {
			Vector3 point3d = new Vector3(vertices2D[i].x, height, vertices2D[i].y);
			result[i] = point3d; 
		}
		return result;
	}

	static Vector3[] ModyfyShapeWithHeightOffsets(Vector2[] vertices2D, float height, float[] heightOffsets) {
		Vector3[] result = new Vector3[vertices2D.Length];
		for (int i=0; i< vertices2D.Length; i++) {
			Vector3 point3d;
			if (i < heightOffsets.Length) {
				point3d = new Vector3(vertices2D[i].x, height + heightOffsets[i], vertices2D[i].y);
			}
			else {
				point3d = new Vector3(vertices2D[i].x, height, vertices2D[i].y);
			}
			result[i] = point3d; 
		}
		return result;
	}


	public static Vector2[] Points3DTo2D (Vector3[] vertices3D) {
		Vector2[] result = new Vector2[vertices3D.Length];
		for (int i=0; i< vertices3D.Length; i++) {
			Vector3 point2d = new Vector2(vertices3D[i].x, vertices3D[i].z);
			result[i] = point2d; 
		}
		return result;
	}


	static void TriangulateShape(Vector2[] vertices2D, GameObject obj, float height) {
		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();
		
		// Create the Vector3 vertices
		Vector3[] vertices = Points2DTo3D(vertices2D, height);
		
		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		// Set up game object with mesh;
		MeshFilter filter = obj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
	}

	static void PrepareTubeMesh(Vector2[] section, GameObject obj, Vector3[] curvePoints, float thickness, float height, bool isOpen) {

		Vector3[] vertices = TubeSectionTo3DPointsOnCurve(section, curvePoints, 0, thickness, height, isOpen);
		int[] indices = new int[] {0, 1, 2};

		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		
		// Set up game object with mesh;
		MeshFilter filter = obj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
	}
	

	/*
	[MenuItem ("Surforge/Test fix sandglass #m")]
	public static void TestFixSandglassShape() {
		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				Vector2[] testShape = new Vector2[polyLassoObject.shape.Count];
				for (int m=0; m < testShape.Length; m++) {
					testShape[m] = new Vector3(polyLassoObject.shape[m].x, polyLassoObject.shape[m].z);
				}

				FixSandglassShape(testShape);
			}
		}

	}
	*/


	public static Vector2[] RemoveAllSandglass(Vector2[] shape) {
		for (int i=0; i<100; i++) {
			int count = shape.Length;
			shape = FixSandglassShape(shape);
			if (shape.Length == count) return shape;
		}
		return shape;
	}

	
	public static Vector2[] FixSandglassShape(Vector2[] shape) { 
		Vector2[]  result = new Vector2[shape.Length];
		for (int i=0; i< shape.Length; i++) {
			result[i] =  shape[i];
		}

		bool interstctionFound = false;
		Vector2 intersectionPoint = new Vector2();

		int intersectionIndexMin = 0;
		int intersectionIndexMax = 0;

		for (int i=0; i < shape.Length; i++) {
			Vector2 ps1 = new Vector2(shape[i].x, shape[i].y); 
			Vector2 pe1 = new Vector2(shape[0].x, shape[0].y); 
			if (i < (shape.Length - 1)) {
				ps1 = new Vector2(shape[i].x, shape[i].y); 
				pe1 = new Vector2(shape[i+1].x, shape[i+1].y);
			}

			for (int m=0; m < shape.Length; m++) {
				if ((i != m) && (i != (m+1)) && (i != (m-1)) && ( !((i==0)&&(m == (shape.Length - 1))) ) && ( !((m==0)&&(i == (shape.Length - 1)))) ) {
					Vector2 ps2 = new Vector2(shape[m].x, shape[m].y);
					Vector2 pe2 = new Vector2(shape[0].x, shape[0].y);
					if (m < (shape.Length - 1)) {
						ps2 = new Vector2(shape[m].x, shape[m].y);
						pe2 = new Vector2(shape[m+1].x, shape[m+1].y);
					}

					if (TestLinesIntersection(ps1, pe1, ps2, pe2)) {
						if (TestSegmentIntersection(ps1, pe1, ps2, pe2)) {
	
							intersectionPoint = LineIntersectionPoint(ps1, pe1, ps2, pe2);
							interstctionFound = true;

							if (i < m) {
								intersectionIndexMin = i;
								intersectionIndexMax = m;
							}
							else {
								intersectionIndexMin = m;
								intersectionIndexMax = i;
							}

							//Debug.Log ("Intersection! " + intersectionPoint);
							break;
						}
					}


				}
			}
			if (interstctionFound) break;
		}
		if (!interstctionFound) return result;

		else {
			List<Vector2> editedShape = new List<Vector2>();
			for (int i=0; i < shape.Length; i++) {
				editedShape.Add (shape[i]);
			}
			editedShape.Insert(intersectionIndexMin+1, intersectionPoint);
			editedShape.Insert(intersectionIndexMax+2, intersectionPoint);

			result = editedShape.ToArray();

			List<Vector2> shapeA = new List<Vector2>();
			List<Vector2> shapeB = new List<Vector2>();

			bool switchedToNextShape = false;

			for (int i=0; i < editedShape.Count; i++) {
				if (!switchedToNextShape) {
					shapeA.Add(editedShape[i]);
				}
				else {
					shapeB.Add(editedShape[i]);
				}

				if (i == (intersectionIndexMin+1)) switchedToNextShape = true;
				if (i == (intersectionIndexMax+2)) switchedToNextShape = false;
			}

			if ( GetShapePerimeterLength(shapeA.ToArray()) > GetShapePerimeterLength(shapeB.ToArray()) ) {
				if (!CheckIfShapeClockwise(shapeA)) shapeA.Reverse();
				result = shapeA.ToArray();
			}
			else {
				if (!CheckIfShapeClockwise(shapeB)) shapeB.Reverse();
				result = shapeB.ToArray();
			}


			/*
			Debug.Log ("Shape A: ");
			for (int i=0; i < shapeA.Count; i++) {
				Debug.Log (shapeA[i]);
			}
			Debug.Log (" ");
			Debug.Log ("Shape B: ");
			for (int i=0; i < shapeB.Count; i++) {
				Debug.Log (shapeB[i]);
			}
			*/
		}

		//for (int i=0; i < result.Length; i++) {
		//	Debug.Log (result[i]);
		//}




		return result;
	}


	static float GetShapePerimeterLength(Vector2[] shape) {
		float result = 0;

		for (int i=0; i < shape.Length; i++) {
			Vector2 v1 = shape[i];
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = shape[0];
			else v2 = shape[i+1];

			float distance = Vector2.Distance(v1, v2);
			result = result + distance;
		}
		return result;
	}


	static bool CheckIfOutlineValid(Vector2[] vertices2D, float distance) {
		bool result = true;

		for (int i=0; i < vertices2D.Length; i++) {
			Vector2 pointA;
			Vector2 pointB;
			pointA = vertices2D[i];
		
			if (i == (vertices2D.Length - 1)) pointB = vertices2D[0];
			else pointB = vertices2D[i+1];

			float sideLength = Mathf.Sqrt( Mathf.Pow( (pointB.x - pointA.x), 2) + Mathf.Pow( (pointB.y - pointA.y), 2) );
			if (sideLength <= Mathf.Abs(distance * 4)) {
				result = false;
				break;
			}
			
		}

		return result;
	}

	static bool CheckVector2Consistency(Vector2 vectorToCheck) {
		if (float.IsNaN(vectorToCheck.x) || float.IsNaN(vectorToCheck.y) || float.IsInfinity(vectorToCheck.x) || float.IsInfinity(vectorToCheck.y)) return false;
		else return true;
	}


	public static Vector2[] CreateShapeOutline(Vector2[] vertices2D, float distance, bool isOpen) {
		if (distance == 0) distance = 0.0001f;

		Vector2[] outlineVerts = new Vector2[vertices2D.Length];

		Vector2 pointA;
		Vector2 pointB;
		Vector2 pointC;
		for (int i=0; i < vertices2D.Length; i++) {
			if (i==0) {
				pointA = vertices2D[vertices2D.Length-1];
				pointB = vertices2D[i];
				pointC = vertices2D[i+1];
			}
			else {
				if (i == (vertices2D.Length-1)){
					pointA = vertices2D[vertices2D.Length-2];
					pointB = vertices2D[i];
					pointC = vertices2D[0];
				}
				else {
					pointA = vertices2D[i-1];
					pointB = vertices2D[i];
					pointC = vertices2D[i+1];
				}
			}


			Vector2 offsetedPoint = new Vector2();

			Vector2 v1 = pointC - pointB;
			Vector2 v2 = pointA - pointB;

			if (isOpen) {
				if (i == 0) v2 = v1 * -1;
				if (i == (vertices2D.Length-1)) v1 = v2 * -1;
			}
		
			float cos = GetCosOfAngleBetweenVectors(v1, v2);
			float actualDistance = 0;
		
			if (Mathf.Approximately(cos, -1) || Mathf.Approximately(cos, 1)) {
				actualDistance = distance;
				Vector2 perp = new Vector2(-v1.y, v1.x); 
				offsetedPoint =  pointB + perp.normalized * actualDistance;
			}

			else {
				Vector2 offsetVectorNew = v1.normalized + v2.normalized;
	
				float div = 2 * Mathf.Sqrt((1-cos)/2 );
				if (div == 0) actualDistance = distance;
				else actualDistance = (distance * 2) / div;

				offsetedPoint =  pointB + offsetVectorNew.normalized * actualDistance;


				if (CheckIfShapeClockwise(Points2DTo3D(new Vector2[] { pointA, pointB, pointC }, 0 ) ) ) {
					offsetedPoint =  pointB + offsetVectorNew.normalized * -actualDistance;
				}
			}

			outlineVerts[i] = offsetedPoint;
		}

		return outlineVerts;
	}



	public static bool IsPointInPolygon(int polyCorners, Vector2[] poly, float x, float y, bool isOutside) {
		
		int   i, j=polyCorners-1;
		bool  oddNodes = false;
		
		for (i=0; i<polyCorners; i++) {
			if ((poly[i].y< y && poly[j].y>=y
			     ||   poly[j].y< y && poly[i].y>=y)
			    &&  (poly[i].x<=x || poly[j].x<=x)) {
				oddNodes^=(poly[i].x+(y-poly[i].y)/(poly[j].y-poly[i].y)*(poly[j].x-poly[i].x)<x); 
			}
			j=i; 
		}

		if (isOutside) return oddNodes;
		else return !oddNodes; 
	}


	public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)  {
		// Get A,B,C of first line - points : ps1 to pe1
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		float C1 = A1*ps1.x+B1*ps1.y;
		
		// Get A,B,C of second line - points : ps2 to pe2
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;
		float C2 = A2*ps2.x+B2*ps2.y;
		
		// Get delta (if delta == 0) lines are parallel
		float delta = A1*B2 - A2*B1;
		
		// now return the Vector2 intersection point
		return new Vector2(
			(B2*C1 - B1*C2)/delta,
			(A1*C2 - A2*C1)/delta
			);
	}

	public static bool TestLinesIntersection(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;

		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;

		// Get delta (if delta == 0) lines are parallel
		float delta = A1*B2 - A2*B1;

		if (Mathf.Approximately(delta, 0)) return false;
		else return true;
	}


	//----split poly lasso objects with line----
	
	public static void SplitSelectedPolyLassoObjects(List<Vector3> splitLine) {
		GameObject[] gameObjects = Selection.gameObjects;

		for (int i=0; i< gameObjects.Length; i++) {
			if (gameObjects[i] != null) {
				PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			
				if (polyLassoObject != null) {

					List<Vector3> pObjWorldShape = new List<Vector3>();
					pObjWorldShape = PolyLassoObjectToWorldShape(polyLassoObject);

					List<List<Vector3>> splitLines = PrepareSplitLinesList(pObjWorldShape, splitLine);

					for (int s=0; s < splitLines.Count; s++) {

						if (CheckIfShapeClockwise(splitLines[s].ToArray())) splitLines[s].Reverse();

						if (SplitPolyLassoObject(polyLassoObject, splitLines[s])) {

							SplitSelectedPolyLassoObjects(splitLine);
							return;
						}
					}


				}
			}
		}

	}

	public static List<Vector3> PolyLassoObjectToWorldShape(PolyLassoObject polyLassoObject) {
		Transform objParent = polyLassoObject.gameObject.transform.parent;
		polyLassoObject.gameObject.transform.parent = null;

		List<Vector3> result = new List<Vector3>();

		for (int u=0; u < polyLassoObject.shape.Count; u++) {
			Vector3 worldPoint = new Vector3(polyLassoObject.shape[u].x + polyLassoObject.gameObject.transform.position.x,
			                                 polyLassoObject.shape[u].y + polyLassoObject.gameObject.transform.position.y,
			                                 polyLassoObject.shape[u].z + polyLassoObject.gameObject.transform.position.z);

			worldPoint = RotatePointAroundPivot(worldPoint, polyLassoObject.gameObject.transform.position, polyLassoObject.gameObject.transform.rotation.eulerAngles, polyLassoObject.gameObject.transform.localScale );
			worldPoint = new Vector3(worldPoint.x, polyLassoObject.gameObject.transform.position.y, worldPoint.z);

			result.Add (worldPoint);
		}
		polyLassoObject.gameObject.transform.parent = objParent;

		return result;
	}

	public static List<Vector3> PolyLassoObjectToWorldShapeGameObjectAndShape(GameObject obj, List<Vector3> shape) {
		Transform objParent = obj.transform.parent;
		obj.transform.parent = null;
		
		List<Vector3> result = new List<Vector3>();
		
		for (int u=0; u < shape.Count; u++) {
			Vector3 worldPoint = new Vector3(shape[u].x + obj.transform.position.x,
			                                 shape[u].y + obj.transform.position.y,
			                                 shape[u].z + obj.transform.position.z);
			
			worldPoint = RotatePointAroundPivot(worldPoint, obj.transform.position, obj.transform.rotation.eulerAngles, obj.transform.localScale );
			worldPoint = new Vector3(worldPoint.x, obj.transform.position.y, worldPoint.z);
			
			result.Add (worldPoint);
		}
		obj.transform.parent = objParent;
		
		return result;
	}

	public static Vector3[] PolyLassoObjectToWorldShapeGameObjectAndShapeArray(GameObject obj, Vector3[] shape) {
		Transform objParent = obj.transform.parent;
		obj.transform.parent = null;
		
		List<Vector3> result = new List<Vector3>();
		
		for (int u=0; u < shape.Length; u++) {
			Vector3 worldPoint = new Vector3(shape[u].x + obj.transform.position.x,
			                                 shape[u].y + obj.transform.position.y,
			                                 shape[u].z + obj.transform.position.z);
			
			worldPoint = RotatePointAroundPivot(worldPoint, obj.transform.position, obj.transform.rotation.eulerAngles, obj.transform.localScale );
			worldPoint = new Vector3(worldPoint.x, obj.transform.position.y, worldPoint.z);
			
			result.Add (worldPoint);
		}
		obj.transform.parent = objParent;
		
		return result.ToArray();
	}


	static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles, Vector3 localScale) {
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = new Vector3(dir.x * localScale.x, dir.y * localScale.y, dir.z * localScale.z);
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}


	static List<List<Vector3>> PrepareSplitLinesList (List<Vector3> shape, List<Vector3> splitLine) {
		List<List<Vector3>> result = new List<List<Vector3>>();

		List<Vector3> interPoints = new List<Vector3>();
		List<int> interIndexes = new List<int>();

		//get intersection points
		for (int i=0; i < splitLine.Count-1; i++) {
			for (int n=0; n < shape.Count; n++) {
				Vector2 splitLinePointA = new Vector2(splitLine[i].x, splitLine[i].z);
				Vector2 splitLinePointB = new Vector2(splitLine[i+1].x, splitLine[i+1].z);

				Vector2 shapePointA = new Vector2(shape[n].x, shape[n].z);
				Vector2 shapePointB = new Vector2();
				if (n == (shape.Count-1)) shapePointB = new Vector2(shape[0].x, shape[0].z);
				else shapePointB = new Vector2(shape[n+1].x, shape[n+1].z);

				if (TestLinesIntersection(splitLinePointA, splitLinePointB, shapePointA, shapePointB)) {
					if (TestSegmentIntersection(splitLinePointA, splitLinePointB, shapePointA, shapePointB)) {
						Vector2 intr = LineIntersectionPoint(splitLinePointA, splitLinePointB, shapePointA, shapePointB);
						interPoints.Add(new Vector3(intr.x, splitLine[0].y, intr.y));
						interIndexes.Add (i);
					}
				}
			}
		}

		//remove duplicated points
		List<Vector3> interPointsUniq = new List<Vector3>();
		List<int> interIndexesUniq = new List<int>();

		for (int i=0; i < interPoints.Count; i++) {

			bool uniq = true;
			for (int n=0; n < interPointsUniq.Count; n++) {
				if (IsPointsEqual3D(interPoints[i], interPointsUniq[n])) {
					uniq = false;
					break;
				}
			}
			if (uniq) {
				interPointsUniq.Add(interPoints[i]);
				interIndexesUniq.Add(interIndexes[i]);
			}
		}

		//create splitLine with intersection points added
		List<Vector3> splitLineWithIntersectionPoints = new List<Vector3>();

		for (int i=0; i< splitLine.Count; i++) {
			splitLineWithIntersectionPoints.Add(splitLine[i]);
			List<Vector3> pointsWithCurrentIndex = new List<Vector3>();
			for (int n=0; n < interIndexesUniq.Count; n++) {
				if (interIndexesUniq[n] == i) pointsWithCurrentIndex.Add(interPointsUniq[n]);
			}
			pointsWithCurrentIndex = SortVector3ListByDistanceToPoint(pointsWithCurrentIndex, splitLine[i]);
			for (int n=0; n < pointsWithCurrentIndex.Count; n++) {
				splitLineWithIntersectionPoints.Add(pointsWithCurrentIndex[n]);
			}
		}
		//remove duplicated points from splitLine with intersection
		List<Vector3> splitLineWithIntersectionPointsUniq = new List<Vector3>();

		for (int i=0; i< splitLineWithIntersectionPoints.Count; i++) {
			bool uniq = true;

			for (int n=0; n < splitLineWithIntersectionPointsUniq.Count; n++) {
				if (IsPointsEqual3D(splitLineWithIntersectionPoints[i], splitLineWithIntersectionPointsUniq[n])) {
					uniq = false;
					break;
				}
			}
			if (uniq) splitLineWithIntersectionPointsUniq.Add(splitLineWithIntersectionPoints[i]);
		}

		//create shapes
		List<Vector3> pointsLeftToProcede = new List<Vector3>();
		for (int i=0; i < splitLineWithIntersectionPointsUniq.Count; i++) {
			pointsLeftToProcede.Add(splitLineWithIntersectionPointsUniq[i]);
		}

		for (int k=0; k < splitLineWithIntersectionPointsUniq.Count; k++) {
			int indexesToRemove = 0;
			List<Vector3> resultShape = new List<Vector3>();
			for (int i=0; i < pointsLeftToProcede.Count; i++) {
				indexesToRemove = i;
				resultShape.Add(pointsLeftToProcede[i]);


				if (i != 0) {
					bool pointMatched = false;
					for (int n=0; n < interPointsUniq.Count; n++) {
						if (IsPointsEqual3D(pointsLeftToProcede[i], interPointsUniq[n])) {
							pointMatched = true;
							break;
						}
					}
					if (pointMatched) break;
				}

			}

			if (resultShape.Count > 1) result.Add(resultShape);

			for (int i=0; i< indexesToRemove; i++) {
				pointsLeftToProcede.RemoveAt(0);
			}
		}

		result = FilterResultSplitLines(result, shape);


		//debug
		/*

		for (int i=0; i < interPointsUniq.Count; i++) {
			GameObject debugObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			debugObj.transform.position = interPointsUniq[i] + new Vector3(0, 1.0f, 0);
		}


		for (int i=0; i < result.Count; i++) {
			DebugShape(result[i], new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), 1) );
		}
		*/


		return result;
	}

	static List<List<Vector3>> FilterResultSplitLines( List<List<Vector3>> splitLines, List<Vector3> shape) {
		List<List<Vector3>> result = new List<List<Vector3>>();

		Vector2[] shapePoly = new Vector2[shape.Count];
		for (int i=0; i < shape.Count; i++) {
			shapePoly[i] = new Vector2(shape[i].x, shape[i].z);
		}

		for (int i=0; i < splitLines.Count; i++) {
			bool match = true;

			// for 2 point splitLines, test not both points belong one segment
			if (match) {
				if (splitLines[i].Count == 2) {
					Vector2 splitLineStart = new Vector2(splitLines[i][0].x, splitLines[i][0].z);
					Vector2 splitLineEnd = new Vector2(splitLines[i][1].x, splitLines[i][1].z);

					for (int n=0; n < shape.Count; n++) {
						Vector2 segmentStart = new Vector2(shape[n].x, shape[n].z);
						Vector2 segmentEnd = new Vector2();
						
						if (n == (shape.Count - 1)) segmentEnd = new Vector2(shape[0].x, shape[0].z);
						else segmentEnd = new Vector2(shape[n+1].x, shape[n+1].z);
						
						if (IsPointOnSegment(segmentStart, segmentEnd, splitLineStart) && IsPointOnSegment(segmentStart, segmentEnd, splitLineEnd)) {
							match = false;
							break;
						}
						
					}
				}
			}

			//for 2 point splitLines, test splitline middle not outside shape and not belong any segment
			if (match) {
				if (splitLines[i].Count == 2) {
					Vector2 middlePoint = new Vector2( (splitLines[i][0].x + splitLines[i][1].x) * 0.5f, (splitLines[i][0].z + splitLines[i][1].z) * 0.5f);

					if (!IsPointInPolygon(shapePoly.Length, shapePoly, middlePoint.x, middlePoint.y, true)) {
						match = false;
					}
					else {
						for (int n=0; n < shape.Count; n++) {
							Vector2 segmentStart = new Vector2(shape[n].x, shape[n].z);
							Vector2 segmentEnd = new Vector2();
							
							if (n == (shape.Count - 1)) segmentEnd = new Vector2(shape[0].x, shape[0].z);
							else segmentEnd = new Vector2(shape[n+1].x, shape[n+1].z);
							
							if (IsPointOnSegment(segmentStart, segmentEnd, middlePoint)) {
								match = false;
								break;
							}
						}
					}

				}
			}


			//for more > 2 points splitLines, test is there points not belong any segment
			if (match) {
				if (splitLines[i].Count > 2) {
					match = false;

					for (int m=0; m < splitLines[i].Count; m++) {
						Vector2 splitLinePoint = new Vector2(splitLines[i][m].x, splitLines[i][m].z);

						bool belongSegment = false;
						for (int n=0; n < shape.Count; n++) {
							Vector2 segmentStart = new Vector2(shape[n].x, shape[n].z);
							Vector2 segmentEnd = new Vector2();
						
							if (n == (shape.Count - 1)) segmentEnd = new Vector2(shape[0].x, shape[0].z);
							else segmentEnd = new Vector2(shape[n+1].x, shape[n+1].z);
						
							if (IsPointOnSegment(segmentStart, segmentEnd, splitLinePoint)) {
								belongSegment = true;
								break;
							}
						}
						if (!belongSegment) {
							match = true;
							break;
						}
					}
				}
			}

			// test no middle points outside polygon
			if (match) {
				for (int n=0; n < splitLines[i].Count; n++) {
					if ( (n != 0) && (n != (splitLines[i].Count - 1)) ) {
						if (!IsPointInPolygon(shapePoly.Length, shapePoly, splitLines[i][n].x, splitLines[i][n].z, true)) {
							match = false;
							break;
						}	
					}
				}
			}

			// test both start and end belongs shape
			if (match) {

				Vector2 splitLineStart = new Vector2(splitLines[i][0].x, splitLines[i][0].z);
				Vector2 splitLineEnd = new Vector2(splitLines[i][splitLines[i].Count-1].x, splitLines[i][splitLines[i].Count-1].z);

				bool startPointOnSegment = false;
				bool endPointOnSegment = false;
				for (int n=0; n < shape.Count; n++) {
					Vector2 segmentStart = new Vector2(shape[n].x, shape[n].z);
					Vector2 segmentEnd = new Vector2();

					if (n == (shape.Count - 1)) segmentEnd = new Vector2(shape[0].x, shape[0].z);
					else segmentEnd = new Vector2(shape[n+1].x, shape[n+1].z);

					if (IsPointOnSegment(segmentStart, segmentEnd, splitLineStart)) startPointOnSegment = true;
					if (IsPointOnSegment(segmentStart, segmentEnd, splitLineEnd)) endPointOnSegment = true;

					if (startPointOnSegment && endPointOnSegment) break;
				}

				if ( (startPointOnSegment == false) || (endPointOnSegment == false) ) match = false;
			}

			//test splitShape have no self intersections
			if (match) { 
				if (TestPolyLineSelfIntersections(splitLines[i])) match = false;
			}


			if (match) result.Add(splitLines[i]);
		}
		//Debug.Log(result.Count);

		return result;
	}

	static bool TestPolyLineSelfIntersections(List<Vector3> polyLine) {
		bool result = false;

		for (int i=0; i< polyLine.Count-1; i++) {
			Vector2 currentPointA = new Vector2(polyLine[i].x, polyLine[i].z);
			Vector2 currentPointB = new Vector2(polyLine[i+1].x, polyLine[i+1].z);

			for (int s=0; s < polyLine.Count-1; s++) {
				Vector2 otherPointA = new Vector2(polyLine[s].x, polyLine[s].z);
				Vector2 otherPointB = new Vector2(polyLine[s+1].x, polyLine[s+1].z);

				if ((currentPointA != otherPointA) && (currentPointA != otherPointB ) && (currentPointB != otherPointA)  && (currentPointB != otherPointB)) {
					bool intersection = TestLinesIntersection(currentPointA, currentPointB, otherPointA, otherPointB);
					if (intersection) intersection = TestSegmentIntersection(currentPointA, currentPointB, otherPointA, otherPointB);
					if (intersection) {
						result = true;
						return result;
					}
				}
			}
		}

		return result;
	}


	static bool IsPointOnSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point) {

		Vector2 v1 = segmentEnd - segmentStart;
		Vector2 v2 = point - segmentStart;
		Vector2 v3 = segmentEnd - point;

		float difference = v1.magnitude - (v2.magnitude + v3.magnitude);

		if (Mathf.Abs(difference) < 0.01f) return true; 
		else return false;

	}


	static List<Vector3> SortVector3ListByDistanceToPoint(List<Vector3> listToSort, Vector3 point) {
		List<Vector3> result = new List<Vector3>();

		List<Vector3> list = new List<Vector3>();
		for (int i=0; i < listToSort.Count; i++) {
			list.Add(listToSort[i]);
		}
		for (int k=0; k< listToSort.Count; k++) {

			float minDistance = Mathf.Infinity;
			int minDistanceIndex = 0;
			for (int i=0; i< list.Count; i++) {
				float distance = Vector3.Distance(list[i], point);
				if ( distance < minDistance) {
					minDistance = distance;
					minDistanceIndex = i;
				}
			}
			result.Add(list[minDistanceIndex]);
			list.RemoveAt(minDistanceIndex);
		}

		return result;
	}

	static bool CheckAllPointsInsideShapeIsConnected(Vector2[] poly, List<Vector3> splitLine) {
		bool result = true;
		int stage = 0;

		for (int i=0; i<splitLine.Count; i++) {
			if (stage == 0) {
				if (IsPointInPolygon(poly.Length, poly, splitLine[i].x, splitLine[i].z, true)) {
					stage = 1;
				}
			}
			else {
				if (stage == 1) {
					if (!IsPointInPolygon(poly.Length, poly, splitLine[i].x, splitLine[i].z, true)) {
						stage = 2;
					}
				}
				else {
					if (stage == 2) {
						if (IsPointInPolygon(poly.Length, poly, splitLine[i].x, splitLine[i].z, true)) {
							result = false;
							break;
						}

					}
				}
			}
		}

		return result;
	}



	static bool SplitPolyLassoObject(PolyLassoObject pObj, List<Vector3> splitLine) {
		bool result = true;

		if (splitLine.Count < 2) return false;


		List<Vector3> shape = PolyLassoObjectToWorldShape(pObj);

		int splitStartIndex = 0;
		int splitEndIndex = 0;

		splitStartIndex = GetPointMatchShapePointIndex(shape, splitLine[0]);
		if (splitStartIndex < 0) {
			splitStartIndex = GetPointOnShapeSegmentIndex(shape, splitLine[0]);
			if (splitStartIndex < 0) return false;
		}

		splitEndIndex = GetPointMatchShapePointIndex(shape, splitLine[splitLine.Count-1]);
		if (splitEndIndex < 0) {
			splitEndIndex = GetPointOnShapeSegmentIndex(shape, splitLine[splitLine.Count-1]);
			if (splitEndIndex < 0) return false;
		}


		//create shapes

		List<Vector3> resultShapeA = new List<Vector3>();
		List<Vector3> resultShapeB = new List<Vector3>();

		int state = 0;

		int indexA = 0;
		int indexB = 0;

		if (splitEndIndex < splitStartIndex) {
			splitLine.Reverse();
			indexA = splitEndIndex;
			indexB = splitStartIndex;
		}
		else {
			indexA = splitStartIndex;
			indexB = splitEndIndex;
		}

		for (int i=0; i < shape.Count; i++) {
			if ((state == 0) || (state == 2)) resultShapeA.Add (shape[i]);
			if (state == 1) resultShapeB.Add (shape[i]);

			if ( i == indexA) {
				state++;
				for (int n=0; n < splitLine.Count; n++) {
					resultShapeA.Add(new Vector3(splitLine[n].x, shape[0].y, splitLine[n].z));
				}
			}
			if (i == indexB) {
				state++;
				splitLine.Reverse();
				for (int n=0; n < splitLine.Count; n++) {
					resultShapeB.Add(new Vector3(splitLine[n].x, shape[0].y, splitLine[n].z));
				}
			}
		}

		//merge duplicates and make clockwise

		resultShapeA = MergeShapeDuplicatePoints(resultShapeA);
		resultShapeB = MergeShapeDuplicatePoints(resultShapeB);

		if (!CheckIfShapeClockwise(resultShapeA.ToArray())) resultShapeA.Reverse();
		if (!CheckIfShapeClockwise(resultShapeB.ToArray())) resultShapeB.Reverse();


		//set size 
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
		  
		resultShapeA = PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, resultShapeA);
		resultShapeB = PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, resultShapeB);  


		//build shapes

		Object[] newSelection = new Object[Selection.objects.Length + 2];
		for (int s = 0; s < Selection.objects.Length; s++) {
			newSelection[s] = Selection.objects[s];
		}

		GameObject trim0 = PolygonLassoBuildObject(null, false, resultShapeA, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
		if (trim0 != null) {
			//set size
			trim0.transform.parent = null;
			trim0.transform.localScale = objLocalScale;
			trim0.transform.localPosition = objLocalPosition;
			trim0.transform.localRotation = objLocalRotation;
			trim0.transform.parent = surforgeSettings.root.transform;

			result = true;
			newSelection[newSelection.Length-2] = trim0;
		}

		GameObject trim1 = PolygonLassoBuildObject(null, false, resultShapeB, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
		if (trim1 != null) {
			//set size
			trim1.transform.parent = null;
			trim1.transform.localScale = objLocalScale;
			trim1.transform.localPosition = objLocalPosition;
			trim1.transform.localRotation = objLocalRotation;
			trim1.transform.parent = surforgeSettings.root.transform;

			result = true;
			newSelection[newSelection.Length-1] = trim1;
		}

		pObj.transform.localScale = objLocalScale;
		pObj.transform.localPosition = objLocalPosition;
		pObj.transform.localRotation = objLocalRotation;
		pObj.transform.parent = pObjParent;


		Selection.objects = newSelection;

		if (surforgeSettings.seamless) {
			RemoveSeamlessInstances(pObj);
			pObj.deleting = true;
		}
		
		//snap to objects
		if (surforgeSettings.polyLassoObjects == null) {
			surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
		}
		else {
			surforgeSettings.polyLassoObjects.Remove(pObj);
		}	
		
		Undo.DestroyObjectImmediate(pObj.gameObject);

		return result;
	}

	static int GetPointOnShapeSegmentIndex(List<Vector3> shape, Vector3 point) {
		int result = -1;
		Vector2 point2D = new Vector2(point.x, point.z);

		for (int i=0; i < shape.Count; i++) {
			Vector2 segmentStart = new Vector2(shape[i].x, shape[i].z);
			Vector2 segmentEnd = new Vector2();
			if (i == (shape.Count-1)) segmentEnd = new Vector2(shape[0].x, shape[0].z);
			else segmentEnd = new Vector2(shape[i+1].x, shape[i+1].z);

			if (IsPointOnSegment(segmentStart, segmentEnd, point2D)) {
				result = i;
				break;
			}
		}

		return result;
	}

	static int GetPointMatchShapePointIndex(List<Vector3> shape, Vector3 point) {
		int result = -1;
		Vector2 point2D = new Vector2(point.x, point.z);

		for (int i=0; i < shape.Count; i++) {
			if (IsPointsEqual(new Vector2(shape[i].x, shape[i].z), point2D)) {
				result = i;
				break;
			}
		}

		return result;
	}

	static bool IsPointsEqual(Vector2 a, Vector2 b) {
		float threshold = 0.00001f; 

		if (Vector2.Distance(a, b) > threshold) return false;
		else return true;
	}

	static bool IsPointsEqual3D(Vector3 a, Vector3 b) {
		float threshold = 0.00001f; 
		
		if (Vector3.Distance(a, b) > threshold) return false;
		else return true;
	}
	


	static List<Vector3> MergeShapeDuplicatePoints(List<Vector3> shape) {
		List<Vector3> result = new List<Vector3>();
		for (int i=0; i < shape.Count; i++) {
			bool isPointUniq = true;
			for (int d=0; d < result.Count; d++) {
				if (IsPointsEqual3D(shape[i], shape[d])) { 
					isPointUniq = false;
					break;
				}
			}
			if (isPointUniq) result.Add(shape[i]);
		}
		return result;
	}


	public static bool TestSegmentIntersection(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
		Vector2 lineIntersection = LineIntersectionPoint(ps1, pe1, ps2, pe2);

		if ( (  ((Mathf.Min(ps1.x, pe1.x) < lineIntersection.x) || (Mathf.Approximately(Mathf.Min(ps1.x, pe1.x),lineIntersection.x)) )   &&   ((lineIntersection.x < Mathf.Max (ps1.x, pe1.x)) || Mathf.Approximately(lineIntersection.x, Mathf.Max (ps1.x, pe1.x)) )  ) &&
		     (  ((Mathf.Min(ps1.y, pe1.y) < lineIntersection.y) || (Mathf.Approximately(Mathf.Min(ps1.y, pe1.y),lineIntersection.y)) )   &&   ((lineIntersection.y < Mathf.Max (ps1.y, pe1.y)) || Mathf.Approximately(lineIntersection.y, Mathf.Max (ps1.y, pe1.y)) )  ) &&
			 (  ((Mathf.Min(ps2.x, pe2.x) < lineIntersection.x) || (Mathf.Approximately(Mathf.Min(ps2.x, pe2.x),lineIntersection.x)) )   &&   ((lineIntersection.x < Mathf.Max (ps2.x, pe2.x)) || Mathf.Approximately(lineIntersection.x, Mathf.Max (ps2.x, pe2.x)) )  ) &&
			 (  ((Mathf.Min(ps2.y, pe2.y) < lineIntersection.y) || (Mathf.Approximately(Mathf.Min(ps2.y, pe2.y),lineIntersection.y)) )   &&   ((lineIntersection.y < Mathf.Max (ps2.y, pe2.y)) || Mathf.Approximately(lineIntersection.y, Mathf.Max (ps2.y, pe2.y)) )  )    ) {
			return true;
		}
		else return false; 
	}

	static void DebugShape(List<Vector3> points, Color color) {
		GameObject lineRenderObj = new GameObject();
		LineRenderer lr = (LineRenderer)lineRenderObj.AddComponent<LineRenderer>();
		Material material = new Material( (Shader)AssetDatabase.LoadAssetAtPath("Assets/Surforge/GPURender/Surforge0.shader", typeof(Shader)));
		material.color = color;
		lr.sharedMaterial = material;
		lr.SetWidth(0.2f, 0.2f);
		lr.SetVertexCount(points.Count);
		for (int j =0; j < points.Count; j++) {
			lr.SetPosition(j, new Vector3(points[j].x, 1.0f, points[j].z));
		}
	}

	public static void MovePlaceToolUp() {
		LogAction("Move up", "+", "");
		surforgeSettings.placeToolVerticalOffset = surforgeSettings.placeToolVerticalOffset + 0.05f;
	}

	public static void MovePlaceToolDown() {
		LogAction("Move down", "-", "");
		surforgeSettings.placeToolVerticalOffset = surforgeSettings.placeToolVerticalOffset - 0.05f;
	}



	public static void MoveObjectsUp() {
		LogAction("Move up", "+", "");

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) { 
			Undo.RecordObject (gameObjects[i].transform, "Object Local Position");
			gameObjects[i].transform.localPosition = new Vector3(gameObjects[i].transform.localPosition.x, gameObjects[i].transform.localPosition.y + 0.05f, gameObjects[i].transform.localPosition.z);
		}

	}

	public static void MoveObjectsDown() {
		LogAction("Move down", "-", "");

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) { 
			Undo.RecordObject (gameObjects[i].transform, "Object Local Position");
			gameObjects[i].transform.localPosition = new Vector3(gameObjects[i].transform.localPosition.x, gameObjects[i].transform.localPosition.y - 0.05f, gameObjects[i].transform.localPosition.z);
		}  
	}


	public static void StepPolyLassoProfileFeaturesScaleIncrease() {
		//Debug.Log ("Profile scale increase");

		Object[] newSelection = new Object[Selection.objects.Length];
		
		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				LogAction("Profile scale increase", " ] ", "");
				
				newSelection[i] = ChangePolyLassoProfileFeaturesScale(polyLassoObject, true, false, 0);
			}
			else {
				Surforge.LogAction("Scale increase", " ] ", "");
				
				gameObjects[i].transform.localScale *= 1.05f;
				newSelection[i] = gameObjects[i];
			}
		}
		
		Selection.objects = newSelection;
	}

	public static void StepPolyLassoProfileFeaturesScaleDecrease() {
		//Debug.Log ("Profile scale decrease");

		Object[] newSelection = new Object[Selection.objects.Length];
		
		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				LogAction("Profile scale decrease", " [ ", "");
				
				newSelection[i] = ChangePolyLassoProfileFeaturesScale(polyLassoObject, false, false, 0);
			}
			else {
				Surforge.LogAction("Scale decrease", " [ ", "");
				
				gameObjects[i].transform.localScale *= 0.95f;
				newSelection[i] = gameObjects[i];
			}
		}
		
		Selection.objects = newSelection;
	}


	public static Object ChangePolyLassoProfileFeaturesScale(PolyLassoObject pObj, bool isIncreasing, bool isAbs, float absRate) {
		float multIncrease = 1.1f;
		float multDecrease = 0.90909f;
		if (!isIncreasing) {
			float tmp = multIncrease;
			multIncrease = multDecrease;
			multDecrease = tmp;
		}

		if (pObj.gameObject.transform.parent != surforgeSettings.root.transform) {
			pObj.gameObject.transform.parent = surforgeSettings.root.transform;
		}
		Vector3 originalLocalPosition = pObj.transform.localPosition;
		pObj.transform.localPosition = new Vector3(0, pObj.transform.localPosition.y, 0);


		GameObject originalTransformForCeltic = new GameObject();
		originalTransformForCeltic.transform.parent = pObj.transform.parent;
		originalTransformForCeltic.transform.localScale = pObj.transform.localScale;
		originalTransformForCeltic.transform.localPosition = pObj.transform.localPosition;
		originalTransformForCeltic.transform.localRotation = pObj.transform.localRotation;
		
		Object result = null;

		Transform pObjParent = pObj.gameObject.transform.parent; 
		
		pObj.gameObject.transform.parent = surforgeSettings.root.transform; 
		
		List<Vector3> shape = PolyLassoObjectToWorldShape(pObj); 



		//set size  
		pObj.gameObject.transform.parent = null;  
		Vector3 objLocalScale = pObj.gameObject.transform.localScale;
		Vector3 objLocalScaleScaled = pObj.gameObject.transform.localScale * multIncrease;
		if (isAbs) {
			objLocalScaleScaled = Vector3.one * absRate;
		}

		Vector3 objLocalPosition = pObj.gameObject.transform.localPosition;
		Quaternion objLocalRotation = pObj.gameObject.transform.localRotation;
		
		GameObject relativeTransforms = new GameObject();
		if (!isAbs) {
			relativeTransforms.transform.localScale = relativeTransforms.transform.localScale * multDecrease;
		}
		else {
			relativeTransforms.transform.localScale = Vector3.one * (float)(1.0f / absRate);
		}

		relativeTransforms.transform.parent = pObj.transform;

		
		pObj.transform.localScale = Vector3.one;
		pObj.transform.localPosition = Vector3.zero;
		pObj.transform.localRotation = Quaternion.identity;
		
		shape = PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, shape);  
		
		
		GameObject resultGameObject = PolygonLassoBuildObject(originalTransformForCeltic, false, shape, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
		
		if (resultGameObject) {
			resultGameObject.transform.parent = null;
			resultGameObject.transform.localScale = objLocalScaleScaled;
			resultGameObject.transform.localPosition = objLocalPosition;
			resultGameObject.transform.localRotation = objLocalRotation;
			resultGameObject.transform.parent = surforgeSettings.root.transform;

			resultGameObject.transform.localPosition = originalLocalPosition;
		}

		pObj.transform.localScale = objLocalScale;
		pObj.transform.localPosition = originalLocalPosition;
		pObj.transform.localRotation = objLocalRotation;
		pObj.transform.parent = pObjParent;
		
		result = (Object)resultGameObject;
		
		//snap to objects
		if (surforgeSettings.polyLassoObjects == null) {
			surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
		}
		else {
			surforgeSettings.polyLassoObjects.Remove(pObj);
		}
		
		if (surforgeSettings.seamless) {
			RemoveSeamlessInstances(pObj);
			pObj.deleting = true;
		}
		Undo.DestroyObjectImmediate(pObj.gameObject);
		
		return result;
	}



	public static void OffsetPolyLassoObjectsOut() {
		Object[] newSelection = new Object[Selection.objects.Length];

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				LogAction("Expand", "Shift + ] ", "");

				newSelection[i] = OffsetPolyLassoObject(polyLassoObject, 0.0625f);
			}
			else {
				Surforge.LogAction("Scale increase", " ] ", "");

				gameObjects[i].transform.localScale *= 1.05f;
				newSelection[i] = gameObjects[i];
			}
		}

		Selection.objects = newSelection;
	}


	public static void OffsetPolyLassoObjectsIn() {
		Object[] newSelection = new Object[Selection.objects.Length];

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				LogAction("Shrink", "Shift + [ ", "");

				newSelection[i] = OffsetPolyLassoObject(polyLassoObject, -0.0625f);
			}
			else {
				Surforge.LogAction("Scale decrease", " [ ", "");

				gameObjects[i].transform.localScale *= 0.95f;
				newSelection[i] = gameObjects[i];
			}
		}

		Selection.objects = newSelection;
	}

	static Object OffsetPolyLassoObject(PolyLassoObject pObj, float offset) {
		GameObject originalTransformForCeltic = new GameObject();
		originalTransformForCeltic.transform.parent = pObj.transform.parent;
		originalTransformForCeltic.transform.localScale = pObj.transform.localScale;
		originalTransformForCeltic.transform.localPosition = pObj.transform.localPosition;
		originalTransformForCeltic.transform.localRotation = pObj.transform.localRotation;

		Object result = null;

		pObj.gameObject.transform.parent = surforgeSettings.root.transform; 

		List<Vector3> shape = PolyLassoObjectToWorldShape(pObj);

		Vector2[] vertices2D = new Vector2[shape.Count];
		for (int i=0; i<vertices2D.Length; i++) {
			vertices2D[i] = new Vector2(shape[i].x, shape[i].z);
		}


		Vector3[] pointsB = Points2DTo3D( CreateShapeOutline(vertices2D, offset, pObj.isOpen), shape[0].y);

		shape = new List<Vector3>();
		for (int n=0; n < pointsB.Length; n++) {
			shape.Add(pointsB[n]);
		}

		//set size 
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
		
		shape = PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, shape);  


		GameObject resultGameObject = PolygonLassoBuildObject(originalTransformForCeltic, false, shape, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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

		if (resultGameObject) {
			resultGameObject.transform.parent = null;
			resultGameObject.transform.localScale = objLocalScale;
			resultGameObject.transform.localPosition = objLocalPosition;
			resultGameObject.transform.localRotation = objLocalRotation;
			resultGameObject.transform.parent = surforgeSettings.root.transform;
		}

		pObj.transform.localScale = objLocalScale;
		pObj.transform.localPosition = objLocalPosition;
		pObj.transform.localRotation = objLocalRotation;
		pObj.transform.parent = pObjParent;

		result = (Object)resultGameObject;

		//snap to objects
		if (surforgeSettings.polyLassoObjects == null) {
			surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
		}
		else {
			surforgeSettings.polyLassoObjects.Remove(pObj);
		}

		if (surforgeSettings.seamless) {
			RemoveSeamlessInstances(pObj);
			pObj.deleting = true;
		}
		Undo.DestroyObjectImmediate(pObj.gameObject);

		return result;
	}
	
	
	
	
	//--decals-----
	

	public static void AddDecalToPolyLassoObjects() {
		LogAction("Add decal", "", "");
		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				AddDecalToPolyLassoObject(polyLassoObject, surforgeSettings.decalSets.decalSets[surforgeSettings.activeDecal]);
			}
		}

	}

	public static void RemoveLastDecal() {
		LogAction("Remove last decal", "", "");
		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				if (polyLassoObject.decalSets.Count > 0) {
					RemoveLastDecalFromPolyLassoObject(polyLassoObject);
				}
			}
		}
		
	}


	public static void RemoveLastDecalFromPolyLassoObject(PolyLassoObject pObj) {
		pObj.decalSets.RemoveAt(pObj.decalSets.Count - 1);

		pObj.inheritMatGroup.RemoveAt(pObj.inheritMatGroup.Count - 1);
		pObj.scatterOnShapeVerts.RemoveAt(pObj.scatterOnShapeVerts.Count - 1);
		pObj.trim.RemoveAt(pObj.trim.Count - 1);
		pObj.perpTrim.RemoveAt(pObj.perpTrim.Count - 1);
		pObj.fitDecals.RemoveAt(pObj.fitDecals.Count - 1);
		pObj.trimOffset.RemoveAt(pObj.trimOffset.Count - 1);
		pObj.decalOffset.RemoveAt(pObj.decalOffset.Count - 1);
		pObj.decalOffsetRandom.RemoveAt(pObj.decalOffsetRandom.Count - 1);
		pObj.decalGap.RemoveAt(pObj.decalGap.Count - 1);
		pObj.decalGapRandom.RemoveAt(pObj.decalGapRandom.Count - 1);
		pObj.decalSize.RemoveAt(pObj.decalSize.Count - 1);
		pObj.decalSizeRandom.RemoveAt(pObj.decalSizeRandom.Count - 1);
		pObj.decalRotation.RemoveAt(pObj.decalRotation.Count - 1);
		pObj.decalRotationRandom.RemoveAt(pObj.decalRotationRandom.Count - 1);

		RebuildDecals(pObj);
	}


	static void AddDecalToPolyLassoObject(PolyLassoObject pObj, DecalSet decalSet) {
		pObj.decalSets.Add(decalSet);

		pObj.inheritMatGroup.Add (decalSet.inheritMatGroup);
		pObj.scatterOnShapeVerts.Add (decalSet.scatterOnShapeVerts);
		pObj.trim.Add (decalSet.trim);
		pObj.perpTrim.Add (decalSet.perpTrim);
		pObj.fitDecals.Add (decalSet.fitDecals);
		pObj.trimOffset.Add(decalSet.trimOffset);
		pObj.decalOffset.Add(decalSet.decalOffset);
		pObj.decalOffsetRandom.Add(decalSet.decalOffsetRandom);
		pObj.decalGap.Add(decalSet.decalGap);
		pObj.decalGapRandom.Add(decalSet.decalGapRandom);
		pObj.decalSize.Add(decalSet.decalSize);
		pObj.decalSizeRandom.Add(decalSet.decalSizeRandom);
		pObj.decalRotation.Add(decalSet.decalRotation);
		pObj.decalRotationRandom.Add(decalSet.decalRotationRandom);

		RebuildDecals(pObj);
	}


	static void RebuildDecals(PolyLassoObject pObj) {
		
		RemoveOldDecals(pObj);

		if (pObj.decalSets.Count > 0) {

			for (int i=0; i< pObj.decalSets.Count; i++) {
				float maxHeight = GetPloyLassoObjectMaxHeight(pObj.heights);
				ScatterDecals(pObj, pObj.shape, maxHeight, pObj.bevelAmount, pObj.bevelSteps, pObj.decalSets[i], 
				              pObj.inheritMatGroup[i],
				              pObj.scatterOnShapeVerts[i],
				              pObj.trim[i],
				              pObj.perpTrim[i],
				              pObj.fitDecals[i],
				              pObj.trimOffset[i],
				              pObj.decalOffset[i], 
				              pObj.decalOffsetRandom[i],
				              pObj.decalGap[i], 
				              pObj.decalGapRandom[i], 
				              pObj.decalSize[i], 
				              pObj.decalSizeRandom[i], 
				              pObj.decalRotation[i], 
				              pObj.decalRotationRandom[i]);
			}
		}
	}

	static float GetPloyLassoObjectMaxHeight(float[] heights) {
		float result = Mathf.NegativeInfinity;
		for (int i=0; i< heights.Length; i++) {
			if (heights[i] > result)  result = heights[i];
		}
		return result;
	}

	static void ScatterDecals(PolyLassoObject pObj, List<Vector3> shape, float maxHeight, float bevelAmount, int bevelSteps, DecalSet decalSet, 
	                          bool inheritMatGroup,
	                          bool scatterOnShapeVerts, 
	                          bool trim,
	                          bool perpTrim,
	                          bool fitDecals,
	                          float trimOffset,
	                          float decalOffset, 
	                          float decalOffsetRandom, 
	                          float decalGap, 
	                          float decalGapRandom, 
	                          float decalSize, 
	                          float decalSizeRandom, 
			                  float decalRotation, 
	                          float decalRotationRandom) {
	
		Vector2[] vertices2D = new Vector2 [shape.Count];
		
		for (int i=0; i< shape.Count; i++) {
			Vector2 vertex2d = new Vector2(shape[i].x, shape[i].z);
			vertices2D[i] = vertex2d;
		}
		
		Vector2[] vertsA = CreateShapeOutline(vertices2D, decalOffset, pObj.isOpen);

		if (scatterOnShapeVerts) {
			Vector3[] decalScatterShapeOnVerts = new Vector3[vertsA.Length];
			for (int v=0; v< vertsA.Length; v++) {
				decalScatterShapeOnVerts[v] = new Vector3(vertsA[v].x, maxHeight + 0.1f, vertsA[v].y);
			}
			ScatterDecalsOnVerts(pObj, decalScatterShapeOnVerts, trim, perpTrim, decalSet, decalSize,  decalSizeRandom, 
			                     decalRotation, decalRotationRandom, inheritMatGroup);
		}
		else {
			Vector2[] vertices2DBeveled = MultiBevel(vertsA, bevelAmount, bevelSteps, pObj.isOpen);

			Vector3[] decalScatterShape = new Vector3[vertices2DBeveled.Length];

			if (pObj.deformedBorder != null) {
				if (pObj.deformedBorder.Length > 0) {
					decalScatterShape = new Vector3[pObj.deformedBorder.Length];
					for (int s=0; s< pObj.deformedBorder.Length; s++) {
						decalScatterShape[s] = new Vector3(pObj.deformedBorder[s].x, maxHeight + 0.1f, pObj.deformedBorder[s].y);
					}
				}
			}

			else {
				for (int s=0; s< vertices2DBeveled.Length; s++) {
					decalScatterShape[s] = new Vector3(vertices2DBeveled[s].x, maxHeight + 0.1f, vertices2DBeveled[s].y);
				}
			}
		
			ScatterDecalsOnShape(pObj, decalScatterShape, trim, perpTrim, decalSet, decalGap, decalGapRandom,  decalSize,  decalSizeRandom, 
			                     decalRotation, decalRotationRandom, inheritMatGroup);
		}

	}

	static void ScatterDecalsOnVerts(PolyLassoObject pObj, Vector3[] decalScatterShapeOnVerts, bool trim, bool perpTrim, DecalSet decalSet, 
	                                 float decalSize, float decalSizeRandom, 
	                                 float decalRotation, float decalRotationRandom, bool inheritMatGroup) {

		Material iheritedMat = null;
		Material iheritedFloaterMat = null;
		if (inheritMatGroup) {
			Renderer renderer = (Renderer)pObj.GetComponent<Renderer>();
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[0]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[0]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[0]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[1]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[1]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[1]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[2]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[2]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[2]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[3]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[3]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[3]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[4]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[4]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[4]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[5]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[5]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[5]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[6]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[6]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[6]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[7]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[7]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[7]);
			}
		}

		for (int i = 0; i < decalScatterShapeOnVerts.Length; i++) {
			Vector3 scatterPoint = new Vector3();
			scatterPoint = decalScatterShapeOnVerts[i];

			float r = decalRotation + GetShapeVertexAngle(decalScatterShapeOnVerts, i, false);
			
			InstantiateDecals(pObj, scatterPoint, trim, perpTrim, decalSet, 
			                  decalSize, decalSizeRandom, r, decalRotationRandom, iheritedMat, iheritedFloaterMat);
		}
	}
			                  
			                  
	static void ScatterDecalsOnShape(PolyLassoObject pObj, Vector3[] decalScatterShape, bool trim, bool perpTrim, DecalSet decalSet, float decalGap, 
	                                 float decalGapRandom, float decalSize, float decalSizeRandom, 
	                                 float decalRotation, float decalRotationRandom, bool inheritMatGroup) {

		Material iheritedMat = null;
		Material iheritedFloaterMat = null;
		if (inheritMatGroup) {
			Renderer renderer = (Renderer)pObj.GetComponent<Renderer>();
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[0]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[0]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[0]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[1]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[1]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[1]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[2]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[2]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[2]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[3]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[3]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[3]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[4]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[4]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[4]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[5]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[5]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[5]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[6]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[6]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[6]);
			}
			if (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[7]) {
				iheritedMat = new Material(surforgeSettings.materialGroups[7]);
				iheritedFloaterMat = new Material(surforgeSettings.floaterMaterialGroups[7]);
			}
		}

		if (decalGap <= 0) decalGap = 0.01f;

		List<Vector3> newScatterPoints = new List<Vector3>();

		float extraLength = 0;
		for (int s=0; s< decalScatterShape.Length; s++) {
			Vector2 v1 = new Vector2(decalScatterShape[s].x, decalScatterShape[s].z);
			Vector2 v2;
			if (s == (decalScatterShape.Length-1)) v2 = new Vector2(decalScatterShape[0].x, decalScatterShape[0].z);
			else v2 = new Vector2(decalScatterShape[s+1].x, decalScatterShape[s+1].z);

			Vector2 vLine = v2 - v1;
			float remainLength = 0;
	
			newScatterPoints.AddRange(SplitLineToSegments(new Vector2(decalScatterShape[s].x, decalScatterShape[s].z), vLine, decalGap,
			                                              decalGapRandom, ref remainLength, extraLength, decalScatterShape[0].y));
			extraLength = remainLength;
			
		}


		Vector3[] scatterPoints = newScatterPoints.ToArray();

		if (scatterPoints.Length > 1) {
			for (int i = 0; i < scatterPoints.Length; i++) {
				Vector3 scatterPoint = new Vector3();
				scatterPoint = scatterPoints[i];

				float r = decalRotation + GetShapeVertexAngle(scatterPoints, i, false);

				InstantiateDecals(pObj, scatterPoint, trim, perpTrim, decalSet, decalSize, 
				                  decalSizeRandom, r, decalRotationRandom, iheritedMat, iheritedFloaterMat);
			}
		}
	}

	static List<Vector3> SplitLineToSegments(Vector2 startPoint, Vector2 vLine, float decalGap, float decalGapRandom, ref float remainLength,
	                                         float extraLength, float height) {

		List<Vector3> result = new List<Vector3>();
		float availableLength = vLine.magnitude;
		Vector2 currentPointOnLine = startPoint;

		for (int i=0; i< 1000; i++) {

			float gap = decalGap + Mathf.Pow(Random.Range( Mathf.Abs(decalGapRandom) * -1, Mathf.Abs(decalGapRandom)), 4) - extraLength;
			if (gap <= 0) gap = 0.01f;
			if (availableLength > gap) {
				currentPointOnLine = currentPointOnLine + vLine.normalized * gap;
				Vector2 usedLineSpace = currentPointOnLine - startPoint;
				if (usedLineSpace.magnitude < vLine.magnitude) {
					result.Add (new Vector3(currentPointOnLine.x, height, currentPointOnLine.y));
					availableLength = availableLength - gap;
					extraLength = 0;
				}
				else {
					remainLength = availableLength + extraLength;
					break;
				}
			}
			else {
				remainLength = availableLength + extraLength;
				break;
			}

		}
		return result;
	}


	static void InstantiateDecals(PolyLassoObject pObj, Vector3 pos, bool trim, bool perpTrim, DecalSet decalSet, float decalSize, float decalSizeRandom, 
	                              float decalRotation, float decalRotationRandom, Material iheritedMat, Material iheritedFloaterMat) {

		Vector3 scatterPoint = new Vector3(pos.x + pObj.gameObject.transform.position.x, 
		                                   pos.y + pObj.gameObject.transform.position.y, 
		                                   pos.z + pObj.gameObject.transform.position.z);
		
		GameObject newDecal = (GameObject)Instantiate(decalSet.decals[Random.Range(0, decalSet.decals.Length)], scatterPoint, Quaternion.identity); 
		float modifiedRotation = decalRotation + Random.Range( Mathf.Abs(decalRotationRandom) * -1, Mathf.Abs(decalRotationRandom));
		newDecal.transform.Rotate(Vector3.up, modifiedRotation);

		float modifiedSize = decalSize + Mathf.Pow(Random.Range( Mathf.Abs(decalSizeRandom) * -1, Mathf.Abs(decalSizeRandom)), 2);
		newDecal.transform.localScale = newDecal.transform.localScale * modifiedSize;

		if (iheritedMat != null) {
			Renderer renderer = (Renderer)newDecal.GetComponent<Renderer>();
			if ((renderer.sharedMaterial.shader == surforgeSettings.materialGroups[0]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[1]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[2]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[3]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[4]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[5]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[6]) ||
			    (renderer.sharedMaterial.shader == surforgeSettings.materialGroups[7])) {
				renderer.sharedMaterial = iheritedMat;
			}
			else {
				renderer.sharedMaterial = iheritedFloaterMat;
			}
		}
			

		Vector2[] shape2d = Points3DTo2D(pObj.shape.ToArray());;

		if (pObj.deformedBorder != null) {
			if (pObj.deformedBorder.Length > 0) {
				shape2d = pObj.deformedBorder;

				for (int s = 0; s < shape2d.Length; s++) {
					shape2d[s] = shape2d[s] + new Vector2(pObj.transform.position.x, pObj.transform.position.z);
				}
			}
		}
		else {
			for (int s = 0; s < shape2d.Length; s++) {
				shape2d[s] = shape2d[s] + new Vector2(pObj.transform.position.x, pObj.transform.position.z);
			}

			float maxOutline = Mathf.NegativeInfinity;
			for (int i=0; i < pObj.offsets.Length; i++) {
				if (pObj.offsets[i] > maxOutline) maxOutline = pObj.offsets[i];
			}

			shape2d = CreateShapeOutline(shape2d, -0.01f, pObj.isOpen); 
			shape2d = MultiBevel(shape2d, pObj.bevelAmount, pObj.bevelSteps, pObj.isOpen);
		}

		if (trim) {
			if (newDecal.layer == 0) {
				TrimDecal(newDecal, shape2d, decalRotation, modifiedSize, perpTrim);
			}
		}
		
		newDecal.transform.parent = pObj.transform;
		pObj.decals.Add(newDecal);

	}


	static void TrimDecal(GameObject decal, Vector2[] shape2d, float rotation, float size, bool perpTrim) {
		MeshFilter meshFilter = (MeshFilter)decal.GetComponent<MeshFilter>();
		Mesh oldMesh = meshFilter.sharedMesh;

		Mesh msh = new Mesh();
		msh.vertices = oldMesh.vertices;
		msh.triangles = oldMesh.triangles;

		Vector3[] worldspaceOldVerts = new Vector3[oldMesh.vertices.Length];
		for (int k=0; k < oldMesh.vertices.Length; k++) {
			worldspaceOldVerts[k] = decal.transform.TransformPoint(oldMesh.vertices[k]);
		}

		Vector3[] movedVerts = new Vector3[msh.vertices.Length];

		Vector2 decalOffsetVector = FoundDecalOffsetVector(rotation);
		Vector2 refPoint1 = FoundRefPoint(rotation, decal, 2.0f);
		Vector2 refPoint2 = FoundRefPoint(rotation, decal, -2.0f);

		for (int t=0; t < msh.vertices.Length; t++) {
			movedVerts[t] = TrimDecalMovePoint(worldspaceOldVerts[t], shape2d, decal, rotation, perpTrim, 
			                                   decalOffsetVector, refPoint1, refPoint2); 
		}

		msh.vertices = movedVerts;
		msh.RecalculateNormals();
		msh.RecalculateBounds();

		meshFilter.mesh = msh;

		MeshCollider meshCollider = (MeshCollider)decal.GetComponent<MeshCollider>();
		if (meshCollider != null) {
			meshCollider.sharedMesh = msh;
		}

		decal.transform.position = Vector3.zero;
		decal.transform.rotation = Quaternion.identity;
		decal.transform.localScale = Vector3.one;
	}

	static Vector2 FoundDecalOffsetVector(float rotation) {
		Quaternion decalOffsetRotation = Quaternion.identity;
		decalOffsetRotation.eulerAngles = new Vector3(0, rotation, 0);
		Vector3 decalOffsetVector = decalOffsetRotation * new Vector3(0,0,1);
		return new Vector2(decalOffsetVector.x, decalOffsetVector.z) * -100.0f;
	}

	static Vector2 FoundRefPoint(float rotation, GameObject decal, float mult) {
		Quaternion refRotation = Quaternion.identity;
		refRotation.eulerAngles = new Vector3(0, rotation + 90.0f, 0);
		Vector3 vRef = refRotation * new Vector3(0, 0, 1);
		
		Vector3 refPoint = vRef * mult + decal.transform.position;
		return new Vector2(refPoint.x, refPoint.z);
	}


	static Vector3 TrimDecalMovePoint(Vector3 oldPos, Vector2[] shape2d, GameObject decal, float rotation, bool perpTrim,
	                                  Vector2 decalOffsetVector, Vector2 refPoint1, Vector2 refPoint2) {
		Vector3 result;

		if (IsPointInPolygon(shape2d.Length, shape2d, oldPos.x, oldPos.z, true)) {
			result = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
		}
		else {

			Vector2 perpPoint = new Vector2(decal.transform.position.x, decal.transform.position.z);
			Vector2 oldPos2D = new Vector2(oldPos.x, oldPos.z);
			if (perpTrim) {
				perpPoint = PerpendicularPointToSegment(refPoint1,  refPoint2, oldPos2D);
				perpPoint = perpPoint + decalOffsetVector;
			}

			Vector2 intersectionPoint = new Vector2();
			bool foundIntersection = false;
			float closestDistance = Mathf.Infinity;

			for (int i=0; i< shape2d.Length; i++) {
				Vector2 p1 = new Vector2(shape2d[i].x, shape2d[i].y);;
				Vector2 p2;
				if (i == (shape2d.Length-1)) {
					p2 = new Vector2(shape2d[0].x, shape2d[0].y);
				}
				else {
					p2 = new Vector2(shape2d[i+1].x, shape2d[i+1].y);
				}

				bool intersection = TestLinesIntersection(p1, p2, perpPoint, oldPos2D);
				if (intersection) intersection = TestSegmentIntersection(p1, p2, perpPoint, oldPos2D);
				if (intersection) {

					Vector2 newIntersectionPoint = LineIntersectionPoint(p1, p2, perpPoint, oldPos2D);
					foundIntersection = true;

					float distance = Vector2.Distance(newIntersectionPoint, oldPos2D);
					if (distance < closestDistance) {
						intersectionPoint = newIntersectionPoint;
						closestDistance = distance;
					}
				}

			}

			if (foundIntersection) {
				result = new Vector3(intersectionPoint.x, decal.transform.position.y, intersectionPoint.y);
			}
			else result = new Vector3 (oldPos.x, oldPos.y, oldPos.z);

		}


		return result;
	}
		

	public static Vector2 PerpendicularPointToSegment(Vector2 segmentPointA, Vector2 segmentPointB, Vector3 point) {
		float k = ((segmentPointB.y-segmentPointA.y) * (point.x-segmentPointA.x) - (segmentPointB.x-segmentPointA.x) * (point.y-segmentPointA.y)) 
			/ ( Mathf.Pow((segmentPointB.y-segmentPointA.y), 2) + Mathf.Pow((segmentPointB.x-segmentPointA.x), 2) );
		return new Vector2(point.x - k * (segmentPointB.y-segmentPointA.y), point.y + k * (segmentPointB.x-segmentPointA.x));
	}


	static void RemoveOldDecals(PolyLassoObject pObj) {
		if (pObj.decals != null) {
			for (int d = 0; d < pObj.decals.Count; d++) {
				DestroyImmediate(pObj.decals[d]);
			}
		}
		pObj.decals = new List<GameObject>();
	}

	static void RemoveOldDetails(PolyLassoObject pObj) {
		if (pObj.details != null) {
			for (int d = 0; d < pObj.details.Count; d++) {
				DestroyImmediate(pObj.details[d]);
			}
		}
		pObj.details = new List<GameObject>();
	}
	

	static float GetShapeVertexAngle(Vector3[] shape, int index, bool isOpen) {
		Vector2 pointMiddle = new Vector2(shape[index].x, shape[index].z);
		Vector2 point1;
		Vector2 point2;
		
		if (index == 0) {
			if (isOpen) {
				point1 = new Vector2(shape[index+1].x, shape[index+1].z);
				point2 = new Vector2(shape[0].x, shape[0].z);
				pointMiddle = GetSegmentMiddle(point1, point2);
			}
			else {
				point1 = new Vector2(shape[index+1].x, shape[index+1].z);
				point2 = new Vector2(shape[shape.Length-1].x, shape[shape.Length-1].z);
			}
		}
		else {
			if (index == (shape.Length-1)) {
				if (isOpen) {
					point1 = new Vector2(shape[index].x, shape[index].z);
					point2 = new Vector2(shape[index-1].x, shape[index-1].z);
					pointMiddle = GetSegmentMiddle(point1, point2);
				}
				else {
					point1 = new Vector2(shape[0].x, shape[0].z);
					point2 = new Vector2(shape[index-1].x, shape[index-1].z);
				}
			}
			else {
				point1 = new Vector2(shape[index+1].x, shape[index+1].z);
				point2 = new Vector2(shape[index-1].x, shape[index-1].z);
			}
		}

		Vector2 v1 = point1 - pointMiddle;
		Vector2 v2 = point2 - pointMiddle;

		Quaternion angleTestRot1 = Quaternion.LookRotation(new Vector3(v2.x, 0, v2.y));
		float angle1 = angleTestRot1.eulerAngles.y + 90.0f;

		Quaternion angleTestRot2 = Quaternion.LookRotation(new Vector3(v1.x, 0, v1.y));
		float angle2 = angleTestRot2.eulerAngles.y - 90.0f;

		angle1 = Mathf.Round(angle1 * 100) * 0.01f;
		angle1 = ConvertAngleTo360Form(angle1);

		angle2 = Mathf.Round(angle2 * 100) * 0.01f;
		angle2 = ConvertAngleTo360Form(angle2);

		return GetAngleAverage(angle1, angle2);
	}


	static float ConvertAngleTo360Form(float angle) {
		float result = angle;
		if (angle > 360) result = angle - 360;
		if (angle <= 0) result = angle + 360;
		return result;
	}

	static float GetAngleAverage(float a, float b) {
		float diff = ( ( a - b + 180 + 360 ) % 360 ) - 180;
		float angle = (360 + b + ( diff / 2.0f ) ) % 360;
		return angle;
	}





	//----------- noise deformer---------------------

	public static void AddNoiseToPolyLassoObjects() {
		LogAction("Add noise", "", "");

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				AddNoiseToPolyLassoObject(polyLassoObject, surforgeSettings.noisePresets.noisePresets[surforgeSettings.activeNoisePreset]);
			}
		}
	}

	public static void RemoveNoiseFromPolyLassoObjects() {
		LogAction("Remove noise", "", "");

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject polyLassoObject = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (polyLassoObject != null) {
				RemoveNoiseToPolyLassoObject(polyLassoObject);
			}
		}
	}

	static void RemoveNoiseToPolyLassoObject(PolyLassoObject pObj) {
		pObj.noise = false;
		pObj.deformedBorder = null;

		RebuildPolyLassoObject(null, pObj, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
	}


	static void RebuildPolyLassoObjectSameSettings(PolyLassoObject pObj) {
		
		RebuildPolyLassoObject(null, pObj, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
	}


	static void AddNoiseToPolyLassoObject(PolyLassoObject pObj, NoisePreset noisePreset) {
		pObj.noise = true;
		pObj.shapeSubdiv = noisePreset.shapeSubdiv;
		if (pObj.shapeSubdiv < 0.1f) pObj.shapeSubdiv = 0.1f;
		pObj.noise1Amount = noisePreset.noise1Amount;
		pObj.noise1Frequency = noisePreset.noise1Frequency;
		pObj.noise2Amount = noisePreset.noise2Amount;
		pObj.noise2Frequency = noisePreset.noise2Frequency;

		RebuildPolyLassoObject(null, pObj, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
	}


	static Vector2 GetOnCircleVectorsSegment(float sectorAngle, int rotationsScount, Vector2 v, Vector2 cornerCircleOffsetVector) {
		Vector2 result = cornerCircleOffsetVector;
		for (int i = 0; i < rotationsScount; i++) {
			result = RotateOnCircleVectorsSegmentOneSector(sectorAngle, v, result);
		}
		return result;

	}

	static Vector2 RotateOnCircleVectorsSegmentOneSector(float resultAngle, Vector2 v, Vector2 cornerCircleOffsetVector) {
		Vector2 pointB = v + cornerCircleOffsetVector;
		float angleB = (180.0f - (resultAngle + 90.0f));
		float side = Mathf.Sin(DegreesToRadians(resultAngle)) / Mathf.Sin (DegreesToRadians(angleB));
		Vector2 vectorC = new Vector2(cornerCircleOffsetVector.y, -cornerCircleOffsetVector.x) * -1.0f;
		Vector2 pointC = pointB + vectorC.normalized * side;
		Vector2 result = (pointC - v).normalized;
		return result;
	}

	static bool TestCurrentEdgeLongEnough(Vector2[] shape, int index, float minLength) {
		bool result = false;
		Vector2 v1 = shape[index];
		Vector2 v2;

		if (index == (shape.Length-1)) {
			v2 = shape[0];
		}
		else {
			v2 = shape[index + 1];
		}

		if (Vector2.Distance(v1, v2) > minLength) result = true;
		return result;
	}

	static bool TestNextEdgeLongEnough(Vector2[] shape, int index, float minLength) {
		bool result = false;
		Vector2 v1;
		Vector2 v2;
		
		if (index == (shape.Length-1)) {
			v1 = shape[0];
			v2 = shape[1];
		}
		else {
			if (index == (shape.Length-2)) {
				v1 = shape[index + 1];
				v2 = shape[0];
			}
			else {
				v1 = shape[index + 1];
				v2 = shape[index + 2];
			}
		}
		
		if (Vector2.Distance(v1, v2) > minLength) result = true;
		return result;
	}

	static bool TestPrevEdgeLongEnough(Vector2[] shape, int index, float minLength) {
		bool result = false;
		Vector2 v1 = shape[index];
		Vector2 v2;
		
		if (index == 0) {
			v2 = shape[shape.Length - 1];
		}
		else {
			v2 = shape[index - 1];
		}
		
		if (Vector2.Distance(v1, v2) > minLength) result = true;
		return result;
	}

	static float[] lengthOffsetedShape; 
	static float[] heightOffsetedShape; 

	static bool[] cornerCirclesOffsetMask;
	static Vector2[]  cornerCirclesOffsetVectors;
	static float[] cornerCirclesOffsets;

	static Vector2[]  placeOnCircleVectors;
	static Vector2[]  cornerPoints;
	static int[] startAndEndOfCirclesMask;

	static float[] circleCenterOffset;



	static List<Vector2> circleCentersMetConditions;

	static bool TestIfCircleCenterFarEnoughFromOthers(Vector2 center, Vector2[] shape, float cornerCircle, float minDistBetweenCircleCenters, float minDistFromCircleToShapeEdge) {
		bool result = true;

		if (IsPointInPolygon(shape.Length, shape, center.x, center.y, false)) {
			return false;
		}
		else {
			for (int i=0; i<12; i++) {
				float angle = i * 30.0f;
				Vector2 pos = PlaceOnCircle(center, cornerCircle * 0.1f + minDistFromCircleToShapeEdge, angle);
				if (IsPointInPolygon(shape.Length, shape, pos.x, pos.y, false)) {
					return false;
				}
			}
		}

		if (circleCentersMetConditions.Count == 0) return result;

		for (int i=0; i < circleCentersMetConditions.Count; i++) {
			if (Vector2.Distance(circleCentersMetConditions[i], center) < minDistBetweenCircleCenters) {
				result = false;
				break;
			}
		}

		return result;
	}

	static Vector2 PlaceOnCircle(Vector2 center, float radius, float angle) {
		Vector2 pos;
		pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
		pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);

		return pos;
	} 


	static Vector2[] SubdivideAndCreateLengthOffsetedShapeEdgewise(Vector2[] shape, bool isOpen, int[] subdivideValues, bool edgeWiseOffset, float[] lengthOffsets, float[] heightOffsets, int repeatSize, float offsetMinEdge, PolyLassoCorner corner, int offsetsLength, int iteration, float[] childProfileVerticalOffsets, float[] childProfileDepthOffsets, int[] childProfileHorisontalOffsets, int[] childProfileMatGroups, PolyLassoRelativeShape[] childProfileShapes, PolyLassoObject obj, bool createChildShapes) { 

		//prepare repeated and not repeated length offsets
		float[] lengthOffsetsWithoutRepeat;
		float[] repeatedLengthOffsets;
		if ( ((lengthOffsets.Length - repeatSize) > 0) && edgeWiseOffset) {
			lengthOffsetsWithoutRepeat = new float[lengthOffsets.Length - repeatSize];
			repeatedLengthOffsets = new float[repeatSize];
			for (int a=0; a< lengthOffsets.Length; a++) {
				if (a < lengthOffsetsWithoutRepeat.Length) lengthOffsetsWithoutRepeat[a] = lengthOffsets[a];
				else {
					repeatedLengthOffsets[a - lengthOffsetsWithoutRepeat.Length] = lengthOffsets[a];
				}
			}
		}
		else {
			lengthOffsetsWithoutRepeat = lengthOffsets;
			repeatedLengthOffsets = new float[0];
		}

		//prepare repeated and not repeated height offsets
		float[] heightOffsetsWithoutRepeat;
		float[] repeatedHeightOffsets;
		if ( ((heightOffsets.Length - repeatSize) > 0) && edgeWiseOffset) {
			heightOffsetsWithoutRepeat = new float[heightOffsets.Length - repeatSize];
			repeatedHeightOffsets = new float[repeatSize];
			for (int a=0; a< heightOffsets.Length; a++) {
				if (a < heightOffsetsWithoutRepeat.Length) heightOffsetsWithoutRepeat[a] = heightOffsets[a];
				else {
					repeatedHeightOffsets[a - heightOffsetsWithoutRepeat.Length] = heightOffsets[a];
				}
			}
		}
		else {
			heightOffsetsWithoutRepeat = heightOffsets;
			repeatedHeightOffsets = new float[0];
		}



		//prepare corners data

		int cornerCircle = 0;
		float circleToSideDistance = 0; 
		float circleMinEdge = 0; 
		float minDistBetweenCircleCenters = 0; 
		float minDistFromCircleToShapeEdge = 0;


		if (corner != null) {
			int[] cornerCircleChecked = new int[offsetsLength];
			if (corner.cornerCircle == null) {
				for (int i=0; i < offsetsLength; i++) {
					cornerCircleChecked[i] = 0;
				}
			}
			else {
				if (corner.cornerCircle.Length == 0) {
					for (int i=0; i < offsetsLength; i++) {
						cornerCircleChecked[i] = 0;
					}
				}
				else {
					for (int i=0; i < offsetsLength; i++) {
						if (i < corner.cornerCircle.Length) {
							cornerCircleChecked[i] = corner.cornerCircle[i];
						}
						else cornerCircleChecked[i] = corner.cornerCircle[corner.cornerCircle.Length - 1];
					}
				}
			}

			cornerCircle = cornerCircleChecked[iteration];
			circleToSideDistance = corner.circleToSideDistance; 
			circleMinEdge = corner.circleMinEdge; 
			minDistBetweenCircleCenters = corner.minDistBetweenCircleCenters; 
			minDistFromCircleToShapeEdge = corner.minDistFromCircleToShapeEdge;
		}



		List<Vector2> result = new List<Vector2>();
		
		List<float> lengthOffsetedShapeList = new List<float>();
		List<float> heightOffsetedShapeList = new List<float>();
		
		List<bool> cornerCirclesOffsetMaskList = new List<bool>();
		List<Vector2> cornerCirclesOffsetVectorsList = new List<Vector2>();
		List<float> cornerCirclesOffsetsList = new List<float>();
		List<Vector2> placeOnCircleVectorsList = new List<Vector2>();
		List<Vector2> cornerPointsList = new List<Vector2>();
		List<int> startAndEndOfCirclesMaskList = new List<int>();
		List<float> circleCenterOffsetList = new List<float>();

		circleCentersMetConditions = new List<Vector2>();

		bool isCircleCenterFarEnoughFirst = false;

		int shapeLength = shape.Length;
		if (isOpen) shapeLength = shapeLength - 1;

		for (int i=0; i < shapeLength; i++) {
			result.Add (shape[i]);
			
			Vector2 v1 = shape[i];
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = shape[0];
			else v2 = shape[i+1];
			Vector2 vLine = v2 - v1;
			
			Vector2[] newVerts = SplitLineToUniformSegments(v1, vLine, subdivideValues[i]);
			
			float[] lengthOffsetedShapeSegment = new float[newVerts.Length +1];
			float[] heightOffsetedShapeSegment = new float[newVerts.Length +1];
			
			bool[] cornerCirclesOffsetMaskSegment = new bool[newVerts.Length +1];
			int[] startAndEndOfCirclesMaskSegment = new int[newVerts.Length +1];
			Vector2[] cornerCirclesOffsetVectorsSegment = new Vector2[newVerts.Length + 1]; 
			float[] cornerCirclesOffsetsSegment = new float[newVerts.Length + 1];
			Vector2[] placeOnCircleVectorsSegment = new Vector2[newVerts.Length + 1]; 
			Vector2[] cornerPointsSegment = new Vector2[newVerts.Length + 1]; 
			float[] circleCenterOffsetSegment = new float[newVerts.Length + 1];
			
			//---------------
			Vector2 v0;
			if (i == 0) v0 = shape[shape.Length-1];
			else v0 = shape[i-1];
			
			Vector2 v3;
			if (i == (shape.Length-1)) {
				v3 = shape[1];
			}
			else {
				if (i == (shape.Length-2)) {
					v3 = shape[0];
				}
				else {
					v3 = shape[i+2];
				}
			}
			
			float angle = GetShapeCornerAngleAtIndex(shape, i);
			float angleNext = 0;
			if (i == (shape.Length-1)) {
				angleNext = GetShapeCornerAngleAtIndex(shape, 0);
			}
			else {
				angleNext = GetShapeCornerAngleAtIndex(shape, i+1);
			}
			
			
			Vector2 cornerCircleOffsetVector = ((v2 - v1).normalized + (v0 - v1).normalized).normalized;
			if (angle == 180.0f) cornerCircleOffsetVector = new Vector2((v2 - v1).normalized.y, -(v2 - v1).normalized.x);
			Vector2 offsetedPoint = v1 + cornerCircleOffsetVector * 0.01f;
			if (IsPointInPolygon(shape.Length, shape, offsetedPoint.x, offsetedPoint.y, false)) {
				cornerCircleOffsetVector = cornerCircleOffsetVector * -1;
			}
			
			Vector2 cornerCircleOffsetVectorNext = ((v3 - v2).normalized + (v1 - v2).normalized).normalized;
			if (angleNext == 180.0f) cornerCircleOffsetVectorNext = new Vector2((v3 - v2).normalized.y, - (v3 - v2).normalized.x);
			Vector2 offsetedPointNext = v2 + cornerCircleOffsetVectorNext * 0.01f;
			if (IsPointInPolygon(shape.Length, shape, offsetedPointNext.x, offsetedPointNext.y, false)) {
				cornerCircleOffsetVectorNext = cornerCircleOffsetVectorNext * -1;
			}


			//----
			float angleC = 180.0f - (angle * 0.5f + 90.0f);
			float lengthN = (float)(cornerCircle) * Mathf.Tan(DegreesToRadians(angleC));
			float lengthL = lengthN / Mathf.Sin(DegreesToRadians(angleC));
			lengthL = Mathf.Floor(lengthL);
			if (angle == 180.0f) {
				lengthN = cornerCircle;
				lengthL = cornerCircle;
			}
			
			float angleCnext = 180.0f - (angleNext * 0.5f + 90.0f);
			float lengthNnext = (float)(cornerCircle) * Mathf.Tan (DegreesToRadians(angleCnext));
			float lengthLnext = lengthNnext / Mathf.Sin(DegreesToRadians(angleCnext));
			lengthLnext = Mathf.Floor(lengthLnext);
			if (angleNext == 180.0f) {
				lengthNnext = cornerCircle;
				lengthLnext = cornerCircle;
			}
			//-----------

			float sectorAngle =     (180.0f     * 0.5f) / (float)(lengthL);
			float sectorAngleNext = (180.0f     * 0.5f) / (float)(lengthLnext);


			float resultCircleCenterOffset = circleToSideDistance / Mathf.Sin(DegreesToRadians((angle * 0.5f)));
			float resultCircleCenterOffsetNext = circleToSideDistance / Mathf.Sin(DegreesToRadians((angleNext * 0.5f)));
			
			
			for (int s=0; s < lengthOffsetedShapeSegment.Length; s++) { 
				if (s < lengthOffsetsWithoutRepeat.Length) {
					lengthOffsetedShapeSegment[s] = lengthOffsetsWithoutRepeat[s];
				}
				else {
					lengthOffsetedShapeSegment[s] = lengthOffsetsWithoutRepeat[lengthOffsetsWithoutRepeat.Length - 1];
				}
			}

			for (int s=0; s < heightOffsetedShapeSegment.Length; s++) {
				if (s < heightOffsetsWithoutRepeat.Length) {
					heightOffsetedShapeSegment[s] = heightOffsetsWithoutRepeat[s];
				}
				else {
					heightOffsetedShapeSegment[s] = heightOffsetsWithoutRepeat[heightOffsetsWithoutRepeat.Length - 1];
				}
			}


			bool isCurrentEdgeLongEnough = TestCurrentEdgeLongEnough(shape, i, circleMinEdge);
			bool isNextEdgeLongEnough = TestNextEdgeLongEnough(shape, i, circleMinEdge);
			bool isPrevEdgeLongEnough = TestPrevEdgeLongEnough(shape, i, circleMinEdge);


			
			for (int s = 0; s < newVerts.Length + 1; s++) {
				if ( s < newVerts.Length) {
					if (s <= lengthL) {
						cornerCirclesOffsetMaskSegment[s] = true;
						cornerCirclesOffsetVectorsSegment[s] = cornerCircleOffsetVector;
						cornerCirclesOffsetsSegment[s] = cornerCircle * 0.1f;
						cornerPointsSegment[s] = v1 + cornerCircleOffsetVector.normalized * lengthN * 0.1f;
						
						placeOnCircleVectorsSegment[s] = GetOnCircleVectorsSegment(sectorAngle, s, v1, cornerCircleOffsetVector);
						circleCenterOffsetSegment[s] = resultCircleCenterOffset - lengthN * 0.1f;
					}
				}
				
				else {
					cornerCirclesOffsetMaskSegment[s] = true;
					cornerCirclesOffsetVectorsSegment[s] = cornerCircleOffsetVector;
					cornerCirclesOffsetsSegment[s] = cornerCircle * 0.1f;
					cornerPointsSegment[s] = v1 + cornerCircleOffsetVector.normalized * lengthN * 0.1f;
					
					placeOnCircleVectorsSegment[s] = GetOnCircleVectorsSegment(sectorAngle, s, v1, cornerCircleOffsetVector);
					circleCenterOffsetSegment[s] = resultCircleCenterOffset - lengthN * 0.1f;
				}
				
				if (s == lengthL ) {
					startAndEndOfCirclesMaskSegment[s] = -1;
				}
				
			}
			
			
			int counter = 1;
			for (int s = newVerts.Length - 1 + 1; s >= 0; s--) {
				if (s < newVerts.Length) {
					if (s >= (newVerts.Length - lengthLnext +1)) {
						cornerCirclesOffsetMaskSegment[s] = true;
						cornerCirclesOffsetVectorsSegment[s] = cornerCircleOffsetVectorNext;
						cornerCirclesOffsetsSegment[s] = cornerCircle * 0.1f;
						cornerPointsSegment[s] = v2 + cornerCircleOffsetVectorNext.normalized * lengthNnext * 0.1f;
						
						placeOnCircleVectorsSegment[s] = GetOnCircleVectorsSegment(-sectorAngleNext, counter, v2, cornerCircleOffsetVectorNext);
						circleCenterOffsetSegment[s] = resultCircleCenterOffsetNext - lengthNnext * 0.1f;
					}
				}
				
				else {
					cornerCirclesOffsetMaskSegment[s] = true;
					cornerCirclesOffsetVectorsSegment[s] = cornerCircleOffsetVectorNext; 
					cornerCirclesOffsetsSegment[s] = cornerCircle * 0.1f;
					cornerPointsSegment[s] = v2 + cornerCircleOffsetVectorNext.normalized * lengthNnext * 0.1f;
					
					placeOnCircleVectorsSegment[s] = GetOnCircleVectorsSegment(-sectorAngleNext, counter, v2, cornerCircleOffsetVectorNext);
					circleCenterOffsetSegment[s] = resultCircleCenterOffsetNext - lengthNnext * 0.1f;
				}
				
				if (counter == lengthLnext ) {
					startAndEndOfCirclesMaskSegment[s] = 1;
				}
				
				counter++;
			}


			//-----remove edge circles not meet conditions
			bool isCircleCenterFarEnough = false;
			bool isCircleCenterFarEnoughNext = false;

			if (isCurrentEdgeLongEnough && isPrevEdgeLongEnough) {
				Vector2 centerVertex = cornerPointsSegment[0] + cornerCirclesOffsetVectorsSegment[0].normalized * circleCenterOffsetSegment[0];
				if (TestIfCircleCenterFarEnoughFromOthers(centerVertex, shape, (float)cornerCircle, minDistBetweenCircleCenters, minDistFromCircleToShapeEdge)) {
					circleCentersMetConditions.Add(centerVertex); 
					isCircleCenterFarEnough = true;
				}
			}
			if (isCurrentEdgeLongEnough && isNextEdgeLongEnough) {
				Vector2 centerVertexNext = cornerPointsSegment[newVerts.Length - 1 + 1] + cornerCirclesOffsetVectorsSegment[newVerts.Length - 1 + 1].normalized * circleCenterOffsetSegment[newVerts.Length - 1 + 1];
				if (TestIfCircleCenterFarEnoughFromOthers(centerVertexNext, shape, (float)cornerCircle, minDistBetweenCircleCenters, minDistFromCircleToShapeEdge)) {
					isCircleCenterFarEnoughNext = true;
				}
			}

			if (i == 0) {
				isCircleCenterFarEnoughFirst = isCircleCenterFarEnough;
			}
			if (i == (shape.Length - 1)) {
				isCircleCenterFarEnoughNext = isCircleCenterFarEnoughFirst;
			}

			for (int s=0; s <= lengthL; s++) {
				if (s < newVerts.Length) {
					if ( !(isCurrentEdgeLongEnough && isPrevEdgeLongEnough && isCircleCenterFarEnough)) cornerCirclesOffsetMaskSegment[s] = false;
				}
			}
			for (int s = newVerts.Length - 1 + 1; s >= (newVerts.Length - lengthLnext +1); s--) {
				if (s >= 0) {
					if ( !(isCurrentEdgeLongEnough && isNextEdgeLongEnough && isCircleCenterFarEnoughNext)) cornerCirclesOffsetMaskSegment[s] = false;
				}
			}
			//-----

			
			if (cornerCircle == 0) {
				for (int s=0; s< cornerCirclesOffsetMaskSegment.Length; s++) {
					cornerCirclesOffsetMaskSegment[s] = false;
				}
			}
			
			
			for (int s=0; s < Mathf.Floor(lengthOffsetedShapeSegment.Length * 0.5f); s++) {
				lengthOffsetedShapeSegment[lengthOffsetedShapeSegment.Length - 1 -s] = lengthOffsetedShapeSegment[s+1];
			}

			for (int s=0; s < Mathf.Floor(heightOffsetedShapeSegment.Length * 0.5f); s++) {
				heightOffsetedShapeSegment[heightOffsetedShapeSegment.Length - 1 -s] = heightOffsetedShapeSegment[s+1];
			}


			//---------add repeated lengthOffsets--------
			if ((repeatSize > 0) && (repeatedLengthOffsets.Length > 0)) {

				int repeatedOffsetsSpace = lengthOffsetedShapeSegment.Length - (lengthOffsetsWithoutRepeat.Length * 2);
				if (repeatedOffsetsSpace >= repeatedLengthOffsets.Length) {
					int fullRepeats = repeatedOffsetsSpace / repeatedLengthOffsets.Length;
					int repeatRemains = repeatedOffsetsSpace % repeatedLengthOffsets.Length;
					int repeatRemainHalf = repeatRemains / 2;

					int repeatedOffsetCounter = 0;
					int repeatsCounter = 0;
					for (int s = lengthOffsetsWithoutRepeat.Length + repeatRemainHalf; s < lengthOffsetedShapeSegment.Length - lengthOffsetsWithoutRepeat.Length; s++) {
						lengthOffsetedShapeSegment[s] = repeatedLengthOffsets[repeatedOffsetCounter];

						repeatedOffsetCounter++;
						if (repeatedOffsetCounter >= repeatedLengthOffsets.Length) {
							repeatedOffsetCounter = 0;
							repeatsCounter++;
						}
						if (repeatsCounter >= fullRepeats) break;
					}
				}
			
			}
			//-------add repeated heightOffsets--------
			if ((repeatSize > 0) && (repeatedHeightOffsets.Length > 0)) {
				int repeatedOffsetsSpace = heightOffsetedShapeSegment.Length - (heightOffsetsWithoutRepeat.Length * 2);
				if (repeatedOffsetsSpace >= repeatedHeightOffsets.Length) {
					int fullRepeats = repeatedOffsetsSpace / repeatedHeightOffsets.Length;
					int repeatRemains = repeatedOffsetsSpace % repeatedHeightOffsets.Length;
					int repeatRemainHalf = repeatRemains / 2;
					
					int repeatedOffsetCounter = 0;
					int repeatsCounter = 0;
					for (int s = heightOffsetsWithoutRepeat.Length + repeatRemainHalf; s < heightOffsetedShapeSegment.Length - heightOffsetsWithoutRepeat.Length; s++) {

						heightOffsetedShapeSegment[s] = repeatedHeightOffsets[repeatedOffsetCounter];
						
						repeatedOffsetCounter++;
						if (repeatedOffsetCounter >= repeatedHeightOffsets.Length) {
							repeatedOffsetCounter = 0;
							repeatsCounter++;
						}
						if (repeatsCounter >= fullRepeats) break;
					}
				}
			}

			//------------------

			
			if ( Vector2.Distance(v1, v2) < offsetMinEdge) {
				lengthOffsetedShapeSegment = new float[subdivideValues[i]];
				for (int s=0; s < lengthOffsetedShapeSegment.Length; s++) {
					lengthOffsetedShapeSegment[s] = lengthOffsets[0];
				}

				heightOffsetedShapeSegment = new float[subdivideValues[i]];
				for (int s=0; s < heightOffsetedShapeSegment.Length; s++) {
					heightOffsetedShapeSegment[s] = heightOffsets[0];
				}
			}

			for (int s=0; s < lengthOffsetedShapeSegment.Length; s++) {
				lengthOffsetedShapeList.Add(lengthOffsetedShapeSegment[s]);
			}

			for (int s=0; s < heightOffsetedShapeSegment.Length; s++) {
				heightOffsetedShapeList.Add(heightOffsetedShapeSegment[s]);
			}

			
			cornerCirclesOffsetMaskList.AddRange(cornerCirclesOffsetMaskSegment);
			cornerCirclesOffsetVectorsList.AddRange (cornerCirclesOffsetVectorsSegment);
			cornerCirclesOffsetsList.AddRange (cornerCirclesOffsetsSegment);
			placeOnCircleVectorsList.AddRange(placeOnCircleVectorsSegment);
			cornerPointsList.AddRange(cornerPointsSegment);
			startAndEndOfCirclesMaskList.AddRange(startAndEndOfCirclesMaskSegment);
			circleCenterOffsetList.AddRange(circleCenterOffsetSegment);
			
			result.AddRange(newVerts);


			//---child shapes---
			if (createChildShapes) {
				if (childProfileShapes != null) {
					if (childProfileShapes.Length > 0) {
						if ( (newVerts.Length > 3) && ( Vector2.Distance(v1, v2) >= offsetMinEdge) ) {

							float halfEdgeLength = Mathf.Floor((newVerts.Length + 1) * 0.5f);
							Vector2 parentShapeEdgeDirection =  (newVerts[1] - newVerts[0]).normalized;
							Vector2 parentShapeEdgePerp = new Vector2(parentShapeEdgeDirection.y, -parentShapeEdgeDirection.x);

							for (int f=0; f< childProfileShapes.Length; f++) {

								List<Vector2> halfOfRelativeShape = new List<Vector2>();
								bool childShapeHalfsConnected = false;
								for (int q = 0; q < childProfileShapes[f].relativeShape.Length; q++) {
									if (childProfileShapes[f].relativeShape[q].y < halfEdgeLength) {
										halfOfRelativeShape.Add(childProfileShapes[f].relativeShape[q]);

									}
									else {
										childShapeHalfsConnected = true;
									}
								}

								List<Vector2> halfOfChildShapeWorldSpace = new List<Vector2>();
								for (int q = 0; q < halfOfRelativeShape.Count; q++) {
									Vector2 childPoint = newVerts[(int)(halfOfRelativeShape[q].y)] + parentShapeEdgePerp * halfOfRelativeShape[q].x + parentShapeEdgePerp * childProfileDepthOffsets[f] * -1.0f;
									halfOfChildShapeWorldSpace.Add(childPoint);
								}
				  				
								if (!childShapeHalfsConnected) {
									BuildChildShape(null, obj, halfOfChildShapeWorldSpace, childProfileVerticalOffsets[f], childProfileMatGroups[f], childProfileShapes[f].relativeShapeProfilePreset);
								}

								Vector2 edgeCenter = new Vector2 ((v1.x + v2.x) * 0.5f, (v1.y + v2.y) * 0.5f);
								Vector2 halfEdge = v1 - edgeCenter;
								Vector2 symmPointB = edgeCenter + new Vector2(halfEdge.y, -halfEdge.x);
								List<Vector2> symmChildShapeWorldSpace = CreateSymmShapeAboutLine(halfOfChildShapeWorldSpace, edgeCenter, symmPointB);

								if (!childShapeHalfsConnected) {
									BuildChildShape(null, obj, symmChildShapeWorldSpace, childProfileVerticalOffsets[f], childProfileMatGroups[f], childProfileShapes[f].relativeShapeProfilePreset);
								}

								if (childShapeHalfsConnected) {
									symmChildShapeWorldSpace.Reverse();
									halfOfChildShapeWorldSpace.AddRange(symmChildShapeWorldSpace);
									BuildChildShape(null, obj, halfOfChildShapeWorldSpace, childProfileVerticalOffsets[f], childProfileMatGroups[f], childProfileShapes[f].relativeShapeProfilePreset);
								}


							}

						}
					}
				}
			}


		}
		
		lengthOffsetedShape = lengthOffsetedShapeList.ToArray();
		heightOffsetedShape = heightOffsetedShapeList.ToArray();
		
		cornerCirclesOffsetMask = cornerCirclesOffsetMaskList.ToArray();
		cornerCirclesOffsetVectors = cornerCirclesOffsetVectorsList.ToArray();
		cornerCirclesOffsets = cornerCirclesOffsetsList.ToArray();
		placeOnCircleVectors = placeOnCircleVectorsList.ToArray();
		cornerPoints = cornerPointsList.ToArray();
		startAndEndOfCirclesMask = startAndEndOfCirclesMaskList.ToArray(); 
		circleCenterOffset = circleCenterOffsetList.ToArray();
		
		return result.ToArray();	
	}


	static void CreateNotEdgeWiseOffset(float[] lengthOffsets, float[] heightOffsets) {
		int lengthOffsetedShapeLength = lengthOffsetedShape.Length;
		int heightOssfetedShapeLength = heightOffsetedShape.Length;

		lengthOffsetedShape = new float[lengthOffsetedShapeLength];
		heightOffsetedShape = new float[heightOssfetedShapeLength];

		int lengthCounter = 0;
		for (int i=0; i < lengthOffsetedShape.Length; i++) {
			lengthOffsetedShape[i] = lengthOffsets[lengthCounter];
			lengthCounter++;
			if (lengthCounter >= lengthOffsets.Length) lengthCounter = 0;
		}
		int heightCounter = 0;
		for (int i=0; i < heightOffsetedShape.Length; i++) {
			heightOffsetedShape[i] = heightOffsets[heightCounter];
			heightCounter++;
			if (heightCounter >= heightOffsets.Length) heightCounter = 0;
		}
	}


	static void CreateLengthWiseOffset(float[] lengthOffsets, float[] heightOffsets) {
		int lengthOffsetedShapeLength = lengthOffsetedShape.Length;
		lengthOffsetedShape = new float[lengthOffsetedShapeLength];

		float[] lengthOffsetsWithoutRepeat = lengthOffsets;


		for (int s=0; s < lengthOffsetedShapeLength; s++) { 
			if (s < lengthOffsetsWithoutRepeat.Length) {
				lengthOffsetedShape[s] = lengthOffsetsWithoutRepeat[s];
			}
			else {
				lengthOffsetedShape[s] = lengthOffsetsWithoutRepeat[lengthOffsetsWithoutRepeat.Length - 1];
			}
		}

		for (int s=0; s < Mathf.Floor(lengthOffsetedShapeLength * 0.5f); s++) {
			lengthOffsetedShape[lengthOffsetedShape.Length - 1 -s] = lengthOffsetedShape[s+1];
		}

	}


	static List<Vector2> CreateSymmShapeAboutLine(List<Vector2> shape, Vector2 pointA, Vector2 pointB) {
		List<Vector2> result = new List<Vector2>();

		for (int i=0; i < shape.Count; i++) {
			
			Vector2 perpPoint = PerpendicularPointToSegment( pointA, pointB,  shape[i] );
			Vector2 perp = shape[i] - perpPoint;
			perp = perp * -1.0f;
			result.Add(shape[i] + perp + perp);
		}
		return result;
	}

	static void BuildChildShape(GameObject originalTransformForCeltic, PolyLassoObject obj, List<Vector2> childShape, float childProfileVerticalOffset, int childProfileMatGroup, PolyLassoProfile profile) {
		List<Vector3> actualChildShape = new List<Vector3>();

		for (int i=0; i < childShape.Count; i++) {
			//TODO: relative to parent scale and rotation
			Vector3 transformedPoint = new Vector3(childShape[i].x + obj.gameObject.transform.localPosition.x, obj.gameObject.transform.localPosition.y + childProfileVerticalOffset, childShape[i].y + obj.gameObject.transform.localPosition.z);
			transformedPoint = RotatePointAroundPivot(transformedPoint, obj.gameObject.transform.position, obj.gameObject.transform.localRotation.eulerAngles, obj.gameObject.transform.localScale);
			actualChildShape.Add(transformedPoint);
		}

		Vector3[] pointsToCheck = actualChildShape.ToArray();
		if (pointsToCheck.Length < 3) return;
		if (!CheckIfShapeClockwise(pointsToCheck)) actualChildShape.Reverse();

		GameObject newObj = PolygonLassoBuildObject(originalTransformForCeltic, true, actualChildShape, profile.bevelAmount, profile.bevelSteps, profile.offsets, profile.heights,
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
		                                            	   childProfileMatGroup,
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

		newObj.transform.parent = obj.gameObject.transform;
		Renderer renderer = (Renderer)newObj.GetComponent<Renderer>();
		PolyLassoObject pObj = (PolyLassoObject)newObj.GetComponent<PolyLassoObject>();

		Material mat;

		if ((pObj.isFloater) && (childProfileMatGroup < 8)) {
			mat = new Material( surforgeSettings.floaterMaterialGroups[childProfileMatGroup]);
			renderer.sharedMaterial = mat;
		}
		else {
			mat = new Material( surforgeSettings.materialGroups[childProfileMatGroup]);
			renderer.sharedMaterial = mat;
		}

		obj.details.Add(newObj);

	}




	static Vector2[] ReversVector2Array(Vector2[] array) {
		Vector2[] result = new Vector2[array.Length];
		for (int r = 0; r < result.Length; r++) {
			result[r] = array[array.Length - 1 -r];
		}
		return result;
	}


	static Vector2[] SubdivideShape(Vector2[] shape, int[] subdivideValues, bool isOpen) {
		List<Vector2> result = new List<Vector2>();

		int shapeLength = shape.Length;
		if (isOpen) shapeLength = shapeLength - 1;

		for (int i=0; i < shapeLength; i++) {
			result.Add (shape[i]);
			Vector2 v1 = shape[i];
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = shape[0];
			else v2 = shape[i+1];
			Vector2 vLine = v2 - v1;

			Vector2[] newVerts = SplitLineToUniformSegments(v1, vLine, subdivideValues[i]);
			result.AddRange(newVerts);
		}

		return result.ToArray();
	}

	static Vector2[] SplitLineToUniformSegments(Vector2 startPoint, Vector2 vLine, int segmentsCount) {
		List<Vector2> result = new List<Vector2>();
		float segmentLength = vLine.magnitude / (float)segmentsCount;
		Vector2 currentPointOnLine = startPoint;

		for (int i=0; i< segmentsCount-1; i++) {
			currentPointOnLine = currentPointOnLine + vLine.normalized * segmentLength;
			result.Add (currentPointOnLine);
		}

		return result.ToArray();
	}


	static int[] GetShapeSubdivideValues(Vector2[] shape, float minEdge) {
		int[] result = new int[shape.Length];

		for (int i=0; i < shape.Length; i++) {
			Vector2 v1 = shape[i];
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = shape[0];
			else v2 = shape[i+1];

			Vector2 line = v2 - v1;
			int value = Mathf.FloorToInt(line.magnitude / minEdge);

			result[i] = value;
		}

		return result;
	}
	

	static Vector3[] GetNoiseDeformerDerivates(Vector2[] shape, Vector3 mod, Vector2 noise1Amount, float noise1Frequency,
	                                           Vector2 noise2Amount, float noise2Frequency) {
		Vector3[] result = new Vector3[shape.Length];
	
		for (int i=0; i < shape.Length; i++) {
			Vector3 point = new Vector3(shape[i].x, 0, shape[i].y) + mod;
			float frequency = noise1Frequency; //0.2f;
			int octaves = 8;
			float lacunarity = 4.0f;
			float persistence = 0.1f;
			
			NoiseMethod method = Noise.methods[1][2];
			NoiseSample sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);

			float frequency2 = noise2Frequency; //1.0f;
			int octaves2 = 8;
			float lacunarity2 = 1.0f;
			float persistence2 = 0.05f;
			
			NoiseSample sample2 = Noise.Sum(method, point, frequency2, octaves2, lacunarity2, persistence2);

			
			result[i] = new Vector3(sample.derivative.x * noise1Amount.x + sample2.derivative.x * noise2Amount.x,
			                        0,
			                        sample.derivative.z * noise1Amount.y + sample2.derivative.z * noise2Amount.y);
		}
		return result;
	}


	static bool TestShapeIntersections(Vector2[] shape) {
		bool result = false;

		for (int i=0; i < shape.Length; i++) {
			Vector2 currentPointA = shape[i];
			Vector2 currentPointB;
			if (i == (shape.Length - 1)) {
				currentPointB = shape[0];

			}
			else {
				currentPointB = shape[i+1];

			}

			for (int m=0; m< shape.Length; m++) {
				Vector2 otherPointA;
				Vector2 otherPointB;

				otherPointA = shape[m];
				if (m == (shape.Length - 1)) {
					otherPointB = shape[0];
				}
				else {
					otherPointB = shape[m+1];
				}
				if ((currentPointA != otherPointA) && (currentPointA != otherPointB ) && (currentPointB != otherPointA)  && (currentPointB != otherPointB)) {

					bool intersection = TestLinesIntersection(currentPointA, currentPointB, otherPointA, otherPointB);
					if (intersection) intersection = TestSegmentIntersection(currentPointA, currentPointB, otherPointA, otherPointB);
					if (intersection) {
						result = true;
						return result;
					}
				}
			}


		}
		return result;
	}

	static bool FixShapeIntersections(Vector2[] offsetedShape, Vector2[] sourceShape) {
		bool fixMade = false;

		for (int i=0; i < sourceShape.Length; i++) {
			Vector2 sourcePointA = sourceShape[i];
			Vector2 sourcePointB;
			Vector2 offsetedPointA = offsetedShape[i];
			Vector2 offsetedPointB;

	
			if (i == (sourceShape.Length - 1)) {
				sourcePointB = sourceShape[0];
				offsetedPointB = offsetedShape[0];
			}
				else {
				sourcePointB = sourceShape[i+1];
				offsetedPointB = offsetedShape[i+1];
			}
				
			bool intersection = TestLinesIntersection(sourcePointA, offsetedPointA, sourcePointB, offsetedPointB);
			if (intersection) intersection = TestSegmentIntersection(sourcePointA, offsetedPointA, sourcePointB, offsetedPointB);
			if (intersection) {
				fixMade = true;

				Vector2 v1 = offsetedPointA - sourcePointA;
				Vector2 v2 = offsetedPointB - sourcePointB;

				Vector2 middle = (v1.normalized + v2.normalized); 

				Vector2 newOffsetedPointA = sourcePointA + middle;
				Vector2 newOffsetedPointB = sourcePointB + middle;

	
				offsetedShape[i] = newOffsetedPointA;

				if (i == (sourceShape.Length - 1)) {
					offsetedShape[0] = newOffsetedPointB;
				}
				else {
					offsetedShape[i+1] = newOffsetedPointB;
				}
					
					
				return fixMade;
			}

		}
		return fixMade;
	}

	

	static Vector2[] ApplyNoiseDeformer(Vector2[] shape, Vector3[] deformerDerivates) {
		
		Vector2[] result = new Vector2[shape.Length];
	
		for (int i=0; i < shape.Length; i++) {
			
			Vector2 newPosition = shape[i] + new Vector2(deformerDerivates[i].x, deformerDerivates[i].z);
			result[i] = newPosition;
		}
		
		return result;
	}


	public static void StoreMaterialGroups() {
		surforgeSettings.storedShapes = new List<List<Vector3>>();
		surforgeSettings.storedGroups = new List<int>();

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				GameObject obj = gameObjects[i];
				if (obj) {
					StoreMaterialGroup(obj);
				}
			}
		}
		
		else {
			Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
			for (int i=0; i < transforms.Length; i++) {
				GameObject obj = transforms[i].gameObject;
				if (obj) {
					StoreMaterialGroup(obj);
				}
			}
		}
	}

	static void StoreMaterialGroup(GameObject obj) {
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
			if (materialGroup != -1) {

				PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();
				if (pObj) {
					surforgeSettings.storedShapes.Add(pObj.shape);
					surforgeSettings.storedGroups.Add(materialGroup);
				}
			}
		}

	}


	public static void LoadMaterialGroups() {
		if(surforgeSettings.storedShapes == null) return;
		

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				GameObject obj = gameObjects[i];
				if (obj) {
					LoadMaterialGroup(obj);
				}
			}
		}
		
		else {
			Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
			for (int i=0; i < transforms.Length; i++) {
				GameObject obj = transforms[i].gameObject;
				if (obj) {
					LoadMaterialGroup(obj);
				}
			}
		}
	}

	static void LoadMaterialGroup(GameObject obj) {
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
			if (materialGroup != -1) {
				
				PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();
				if (pObj) {
					FindAndSetStoredMaterialGroup(pObj);
				}
			}
		}
		
	}

	static void FindAndSetStoredMaterialGroup(PolyLassoObject pObj) {
		float centerX = 0;
		float centerZ = 0;

		for (int i=0; i< pObj.shape.Count; i++) {
			centerX = centerX + pObj.shape[i].x;
			centerZ = centerZ + pObj.shape[i].z;
		}
		centerX = centerX / (float)pObj.shape.Count;
		centerZ = centerZ / (float)pObj.shape.Count;

		for (int i=0; i< surforgeSettings.storedShapes.Count; i++) {
			Vector2[] poly = new Vector2[surforgeSettings.storedShapes[i].Count];
			for (int s=0; s< poly.Length; s++) {
				poly[s] = new Vector2(surforgeSettings.storedShapes[i][s].x, surforgeSettings.storedShapes[i][s].z);
			}

			if (IsPointInPolygon(poly.Length, poly, centerX, centerZ, true)) {
				AssignMaterialGroup(pObj.gameObject, surforgeSettings.storedGroups[i]);
				break;
			}
		}
	}


	public static void RandomHeights() {
		LogAction("Set random vertical offset", "", "");

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				SetRandomVerticalOffsetPolyLassoObject(gameObjects[i]);
			}
		}
		else {
			foreach (Transform child in surforgeSettings.root.transform) {
				GameObject obj = child.gameObject;
				if (obj) {
					SetRandomVerticalOffsetPolyLassoObject(obj);
				}
			}
		}
	}

	static void SetVerticalOffsetPolyLassoObject(GameObject obj, float localPositionY) {
		Undo.RegisterFullObjectHierarchyUndo(obj, "set vertical offset"); 
		PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();
		if (pObj) {
			pObj.gameObject.transform.localPosition = new Vector3(pObj.gameObject.transform.localPosition.x, localPositionY, pObj.gameObject.transform.localPosition.z);
		}
	}

	static void SetRandomVerticalOffsetPolyLassoObject(GameObject obj) {
		int random = Random.Range(0, (int)5);
		float offset = 0;
		if (random == 0) offset = 0;
		if (random == 1) offset = -0.05f;
		if (random == 2) offset = 0.05f;
		if (random == 3) offset = -0.1f;
		if (random == 4) offset = 0.1f;
		
		SetVerticalOffsetPolyLassoObject(obj, obj.transform.localPosition.y + offset);
	}


	public static void SimilarHeights() {
		LogAction("Set same vertical offset to similar", "", "");

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				GameObject obj = gameObjects[i];
				if (obj) {
					SetSimilarHeight(obj);
				}
			}
		}
		
		else {
			Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
			for (int i=0; i < transforms.Length; i++) {
				GameObject obj = transforms[i].gameObject;
				if (obj) {
					SetSimilarHeight(obj);
				}
			}
		}
	}

	static void SetSimilarHeight(GameObject obj) {
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
			if (materialGroup >= 0) {
				List<GameObject> similarObjs = GetSimilarObjects(obj, surforgeSettings.root);
				for (int s=0; s < similarObjs.Count; s++) {
					SetVerticalOffsetPolyLassoObject(similarObjs[s], obj.transform.localPosition.y);
				}
			}
		}
	}


	public static void ShiftGroups() {
		LogAction("Shift Material Masks", "", "");

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				Renderer renderer = (Renderer)gameObjects[i].GetComponent<Renderer>();
				if (renderer) {
					int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
					if (materialGroup >= 0) {
						ShiftMaterialGroup(gameObjects[i], materialGroup);
					}
				}
			}
		}
		else {
			foreach (Transform child in surforgeSettings.root.transform) {
				GameObject obj = child.gameObject;
				if (obj) {
					Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
					if (renderer) {
						int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
						if (materialGroup >= 0) {
							ShiftMaterialGroup(obj, materialGroup);
						}
					}
				}
			}
		}
	}


	public static void RandomGroups() {
		LogAction("Set random Material Masks", "", "");

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				Renderer renderer = (Renderer)gameObjects[i].GetComponent<Renderer>();
				if (renderer) {
					int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
					if (materialGroup >= 0) {
						AssignMaterialGroup(gameObjects[i], Random.Range(0, (int)8));
					}
				}
			}
		}
		else {
			foreach (Transform child in surforgeSettings.root.transform) {
				GameObject obj = child.gameObject;
				if (obj) {
					Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
					if (renderer) {
						int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
						if (materialGroup >= 0) {
							AssignMaterialGroup(obj, Random.Range(0, (int)8));
						}
					}
				}
			}
		}
	}


	public static void SelectAllInRoot() {
		List<GameObject> newSelectionList = GetAllGameObjectsInRootWithRenderer(surforgeSettings.root);
		
		Object[] newSelection = newSelectionList.ToArray();
		Selection.objects = newSelection;
	}


	public static void SelectSameGroup() {
		LogAction("Select same mask", "Shift + M", "");

		GameObject[] gameObjects = new GameObject[0];
		
		List<Object> newSelectionList = new List<Object>();
		
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				List<GameObject> similarObjs = GetSameGroupObjects(gameObjects[i], surforgeSettings.root);
				for (int s=0; s < similarObjs.Count; s++) {
					newSelectionList.Add(similarObjs[s]);
				}
			}
		}
		
		Object[] newSelection = newSelectionList.ToArray();
		Selection.objects = newSelection;
	}


	public static void SelectSimilar() {
		LogAction("Select similar", "Shift + S", "");

		GameObject[] gameObjects = new GameObject[0];

		List<Object> newSelectionList = new List<Object>();

		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				List<GameObject> similarObjs = GetSimilarObjects(gameObjects[i], surforgeSettings.root);
				for (int s=0; s < similarObjs.Count; s++) {
					newSelectionList.Add(similarObjs[s]);
				}
			}
		}

		Object[] newSelection = newSelectionList.ToArray();
		Selection.objects = newSelection;
	}

	public static void SimilarGroups() {
		LogAction("Set similar Material Masks", "", "");

		GameObject[] gameObjects = new GameObject[0];
		if (Selection.gameObjects.Length > 0) {
			gameObjects = Selection.gameObjects;
			for (int i=0; i< gameObjects.Length; i++) {
				GameObject obj = gameObjects[i];
				if (obj) {
					SetSimilarGroup(obj);
				}
			}
		}

		else {
			Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
			for (int i=0; i < transforms.Length; i++) {
				GameObject obj = transforms[i].gameObject;
				if (obj) {
					SetSimilarGroup(obj);
				}
			}
		}
	}

	static void SetSimilarGroup(GameObject obj) {
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
			if (materialGroup >= 0) {
				List<GameObject> similarObjs = GetSimilarObjects(obj, surforgeSettings.root);
				for (int s=0; s < similarObjs.Count; s++) {
					AssignMaterialGroup(similarObjs[s], materialGroup);
				}
			}
		}
	}
	
	static int GetMeterialGroupNum(Shader shader) {
		int result = -1;

		for (int i=0; i< surforgeSettings.materialGroups.Length; i++) {
			if (surforgeSettings.materialGroups[i] == shader) {
				result = i;
				return result;
			}
		}
		for (int i=0; i< surforgeSettings.floaterMaterialGroups.Length; i++) {
			if (surforgeSettings.floaterMaterialGroups[i] == shader) {
				result = i;
				return result;
			}
		}

		return result;
	}

	static List<GameObject> GetAllGameObjectsInRootWithRenderer(GameObject root) {
		List<GameObject> result = new List<GameObject>();
		Transform[] searchList = root.GetComponentsInChildren<Transform>();

		for (int i=0; i < searchList.Length; i++) {
			GameObject searchedObj = (GameObject)searchList[i].gameObject;
			if (searchedObj) {
				Renderer rendererSearched = (Renderer)searchedObj.GetComponent<Renderer>();
				if (rendererSearched) {
					result.Add(searchList[i].gameObject);
				}
			}
		}

		return result;
	}

	static List<GameObject> GetSameGroupObjects(GameObject obj, GameObject root) {
		List<GameObject> result = new List<GameObject>();
		Transform[] searchList = root.GetComponentsInChildren<Transform>();

		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			int materialGroup = GetMeterialGroupNum(renderer.sharedMaterial.shader);
		
			for (int i=0; i < searchList.Length; i++) {
				GameObject searchedObj = (GameObject)searchList[i].gameObject;
				if (searchedObj) {

					Renderer rendererSearched = (Renderer)searchedObj.GetComponent<Renderer>();
					if (rendererSearched) {
						int materialGroupSearched  = GetMeterialGroupNum(rendererSearched.sharedMaterial.shader);

						if (materialGroup == materialGroupSearched)  {
							result.Add(searchList[i].gameObject);
						}

					}
				}
			}

		}
		
		return result;
	}

	static List<GameObject> GetSimilarObjects(GameObject obj, GameObject root) {
		List<GameObject> result = new List<GameObject>();
		Transform[] searchList = root.GetComponentsInChildren<Transform>();

		PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();

		if (pObj != null) {
			for (int i=0; i < searchList.Length; i++) {
				PolyLassoObject pObjSearch = (PolyLassoObject)searchList[i].gameObject.GetComponent<PolyLassoObject>();
				if (pObjSearch != null) {
					if (Mathf.Abs(GetPolyLassoObjectShapeLength(pObj) - GetPolyLassoObjectShapeLength(pObjSearch)) < 0.01f)  {
						result.Add(searchList[i].gameObject);
					}
				}
			}
		}
		else {
			MeshFilter meshFilter = (MeshFilter)obj.GetComponent<MeshFilter>();
			if (meshFilter) {
				for (int i=0; i < searchList.Length; i++) {
					MeshFilter meshFilterSearch = (MeshFilter)searchList[i].gameObject.GetComponent<MeshFilter>();
					if (meshFilterSearch) {
						if (meshFilter.sharedMesh == meshFilterSearch.sharedMesh) {
							Vector3 scaleA = new Vector3(Mathf.Abs(obj.transform.localScale.x), Mathf.Abs(obj.transform.localScale.y), Mathf.Abs(obj.transform.localScale.z));
							Vector3 scaleB = new Vector3(Mathf.Abs(searchList[i].gameObject.transform.localScale.x), Mathf.Abs(searchList[i].gameObject.transform.localScale.y), Mathf.Abs(searchList[i].gameObject.transform.localScale.z));
							if (scaleA == scaleB) {
								result.Add(searchList[i].gameObject);
							}
						}
					}
				}
			}
		}

		return result;
	}

	public static float GetPolyLassoObjectShapeLength(PolyLassoObject pObj) {
		float result = 0;

		if (pObj.shape.Count < 2) return result;

		for (int i=0; i < pObj.shape.Count; i++) {
			Vector3 pointA = pObj.shape[i];
			Vector3 pointB = new Vector3();

			if (i == (pObj.shape.Count-1)) pointB = pObj.shape[0];
			else pointB = pObj.shape[i+1];

			result = result + Vector3.Distance(pointA, pointB);
		}

		return result;
	}

	static void ShiftMaterialGroup(GameObject obj, int oldGroup) {
		int newGroup = 0;

		if (oldGroup == 8) newGroup = 9;
		else {
			if (oldGroup == 9) newGroup = 8;
			else {
				if (oldGroup == 7) newGroup = 0;
				else {
					newGroup = oldGroup + 1;
				}
			}
		}

		if (Random.Range(0, 1.0f) > 0.5f) AssignMaterialGroup(obj, newGroup);
	}

	static void AssignMaterialGroup(GameObject obj, int groupNumber) {
		Undo.RegisterFullObjectHierarchyUndo(obj, "set material mask");
		PolyLassoObject pObj = (PolyLassoObject)obj.GetComponent<PolyLassoObject>();
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer != null) {
			Material mat = (Material)renderer.sharedMaterial;
			if (mat != null) {
				if (pObj != null) {
					if ((pObj.isFloater) && (groupNumber < 8)) {
						mat = new Material( surforgeSettings.floaterMaterialGroups[groupNumber]);
						renderer.sharedMaterial = mat;
					}
					else {
						mat = new Material( surforgeSettings.materialGroups[groupNumber]);
						renderer.sharedMaterial = mat;
						
						if (pObj.cutoff) {
							renderer.sharedMaterial.SetTexture("_MainTex", pObj.cutoff);
							renderer.sharedMaterial.SetTextureScale("_MainTex", pObj.cutoffTiling);
						}
						if (pObj.bumpMap) {
							renderer.sharedMaterial.SetTexture("_BumpMap", pObj.bumpMap);
							renderer.sharedMaterial.SetTextureScale("_BumpMap", pObj.bumpMapTiling);
							renderer.sharedMaterial.SetFloat("_BumpIntensity", pObj.bumpMapIntensity);
						}
						if (pObj.aoMap) {
							renderer.sharedMaterial.SetTexture("_AO", pObj.aoMap);
							if (pObj.bumpMap) renderer.sharedMaterial.SetTextureScale("_AO", pObj.bumpMapTiling);
							renderer.sharedMaterial.SetFloat("_AOIntensity", pObj.aoMapIntensity);
						}
					}
					if(surforgeSettings.seamless) {
						if (pObj.gameObject.transform.parent == surforgeSettings.root.transform) { 
							if (pObj.seamlessInstances != null) {
								if (pObj.seamlessInstances.Length == 8) {
									RebuildSeamlessInstances(pObj);
								}
							}
						}
					}
				}
				else {
					bool isFloater = false;
					for (int i=0; i < surforgeSettings.floaterMaterialGroups.Length; i++) {
						if (renderer.sharedMaterial.shader == surforgeSettings.floaterMaterialGroups[i]) {
							isFloater = true;
							break;
						}
					}
					if ((isFloater) && (groupNumber < 8)) {
						mat = new Material( surforgeSettings.floaterMaterialGroups[groupNumber]);
						renderer.sharedMaterial = mat;
					}
					else {
						mat = new Material( surforgeSettings.materialGroups[groupNumber]);
						renderer.sharedMaterial = mat;
					}

					PlaceMesh placeMeshObj = (PlaceMesh)obj.GetComponent<PlaceMesh>();
					if (placeMeshObj != null) {
						float offsetX = 0;
						float offsetZ = 0;
						if (placeMeshObj.randomUvOffset) {
							offsetX	= Random.Range(-1.0f, 1.0f);
							offsetZ	= Random.Range(-1.0f, 1.0f);
						}

						if (placeMeshObj.bumpMap) {
							renderer.sharedMaterial.SetTexture("_BumpMap", placeMeshObj.bumpMap);
							renderer.sharedMaterial.SetTextureScale("_BumpMap", placeMeshObj.bumpMapTiling);
							renderer.sharedMaterial.SetFloat("_BumpIntensity", placeMeshObj.bumpMapIntensity);
							renderer.sharedMaterial.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetZ));
						}
						if (placeMeshObj.aoMap) {
							renderer.sharedMaterial.SetTexture("_AO", placeMeshObj.aoMap);
							renderer.sharedMaterial.SetTextureScale("_AO", placeMeshObj.bumpMapTiling);
							renderer.sharedMaterial.SetFloat("_AOIntensity", placeMeshObj.aoMapIntensity);
							renderer.sharedMaterial.SetTextureOffset("_AO", new Vector2(offsetX, offsetZ));
						}

						if(surforgeSettings.seamless) {
							if (placeMeshObj.gameObject.transform.parent == surforgeSettings.root.transform) { 
								if (placeMeshObj.seamlessInstances != null) {
									if (placeMeshObj.seamlessInstances.Length == 8) {
										RebuildSeamlessInstancesPlaceMesh(placeMeshObj);
									}
								}
							}
						}

					}



				}
			}
		}
		if (pObj != null) {
			pObj.materialID = groupNumber;
		}
	}

	public static void AssignMaterialGroupToSelection(int groupNumber) {
		if (Event.current.shift) {
			Surforge.LogAction("Assign Material Mask " + (groupNumber+1).ToString() + " to similar", "Shift + " + (groupNumber+1).ToString(), "");
			if (groupNumber == 8) {
				Surforge.LogAction("Assign Emission Mask 1 to similar", "Shift + 9", "");
			}
			if (groupNumber == 9) {
				Surforge.LogAction("Assign Emission Mask 2 to similar", "Shift + 0", "");
			}
		}
		else {
			Surforge.LogAction("Assign Material Mask " + (groupNumber+1).ToString(), (groupNumber+1).ToString(), "");
			if (groupNumber == 8) {
				Surforge.LogAction("Assign Emission Mask 1", "9", "");
			}
			if (groupNumber == 9) {
				Surforge.LogAction("Assign Emission Mask 2", "0", "");
			}
		}

		GameObject[] gameObjects = Selection.gameObjects;
		for (int i=0; i< gameObjects.Length; i++) {
			AssignMaterialGroup(gameObjects[i], groupNumber);
		}
	}





	//--- seamless-----
	public static void ActivateSeamlessMode() {
		LogAction("Seamless: on", "Alt + S", "");

		surforgeSettings.seamless = true;

		ResetAllSeamlessInstances();
		UpdatePolyLassoObjectsSnapping();
	}

	public static void ResetAllSeamlessInstances() {
		ClearGlobalSeamlessInstancesList();

		foreach (Transform child in surforgeSettings.root.transform) {
			PolyLassoObject pObj = (PolyLassoObject)child.gameObject.GetComponent<PolyLassoObject>();
			if (pObj != null) {
				RebuildSeamlessInstances(pObj);
			}
			else {
				PlaceMesh placeMeshObj = (PlaceMesh)child.gameObject.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) RebuildSeamlessInstancesPlaceMesh(placeMeshObj);
			}
		}
	}

	public static void DeactivateSeamlessMode() {
		LogAction("Seamless: off", "Alt + S", "");

		surforgeSettings.seamless = false;

		foreach (Transform child in surforgeSettings.root.transform) {
			PolyLassoObject pObj = (PolyLassoObject)child.gameObject.GetComponent<PolyLassoObject>();
			if (pObj != null) {
				RemoveSeamlessInstances(pObj);
			}
			else {
				PlaceMesh placeMeshObj = (PlaceMesh)child.gameObject.GetComponent<PlaceMesh>();
				if (placeMeshObj != null) RemoveSeamlessInstancesPlaceMesh(placeMeshObj);
			}
		}
		ClearGlobalSeamlessInstancesList();
		UpdatePolyLassoObjectsSnapping();
	}


	static void UpdatePolyLassoObjectsSnapping() {
		for (int i=0; i< surforgeSettings.polyLassoObjects.Count; i++) {
			surforgeSettings.polyLassoObjects[i].SetPolyLassoObjectShapePointPairs(surforgeSettings.polyLassoObjects[i]);
		}
	}


	static void RebuildSeamlessInstances(PolyLassoObject obj) {
		RemoveSeamlessInstances(obj);

		obj.seamlessInstances = new GameObject[8];
		CreateSeamlessInstances(obj);
	}

	static void RebuildSeamlessInstancesPlaceMesh(PlaceMesh obj) {
		RemoveSeamlessInstancesPlaceMesh(obj);

		obj.seamlessInstances = new GameObject[8];
		CreateSeamlessInstancesPlaceMesh(obj);
	}

	public static void RemoveSeamlessInstances(PolyLassoObject obj) {
		if (obj) {
			if (obj.seamlessInstances != null) {
				for (int i=0; i<obj.seamlessInstances.Length; i++) {
					if (obj.seamlessInstances[i] != null) {
						if (obj.seamlessInstances[i].gameObject != null) DestroyImmediate(obj.seamlessInstances[i].gameObject);
					}
				}
			}
			obj.seamlessInstances = null;
		}
	}

	static void RemoveSeamlessInstancesPlaceMesh(PlaceMesh obj) {
		if (obj) {
			if (obj.seamlessInstances != null) {
				for (int i=0; i<obj.seamlessInstances.Length; i++) {
					if (obj.seamlessInstances[i] != null) {
						if (obj.seamlessInstances[i].gameObject != null) DestroyImmediate(obj.seamlessInstances[i].gameObject);
					}
				}
			}
			obj.seamlessInstances = null;
		}
	}

	static void CreateSeamlessInstancesPlaceMesh(PlaceMesh obj) {
		float offsetZ = Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * surforgeSettings.root.transform.localScale.z;
		float offsetX = Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * surforgeSettings.root.transform.localScale.x;

		for (int i=0; i< 8; i++) {
			GameObject inst = (GameObject)Instantiate(obj.gameObject);
			DestroyImmediate(inst.GetComponent<PlaceMesh>());
			UpdateSeamlessInstanceTransform(inst, i, offsetX, offsetZ);

			//seamless highlight
			ApplySeamlessHighlight(inst);
			foreach(Transform child in inst.transform) {
				ApplySeamlessHighlight(child.gameObject);
			}

			obj.seamlessInstances[i] = inst;
			if (surforgeSettings.seamlessInstances != null) {
				surforgeSettings.seamlessInstances.Add(inst);
			}
			else {
				surforgeSettings.seamlessInstances = new List<GameObject>();
			}
		}
	}


	static void ApplySeamlessHighlight(GameObject obj) {
		Renderer renderer = (Renderer)obj.GetComponent<Renderer>();
		if (renderer) {
			Material[] newMaterials = new Material[2];
			newMaterials[0] = renderer.sharedMaterial;
			newMaterials[1] = (Material)Instantiate(surforgeSettings.seamlessHighlight);
			renderer.sharedMaterials = newMaterials;
		}
	}


	static void CreateSeamlessInstances(PolyLassoObject obj) {

		float offsetZ = Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * surforgeSettings.root.transform.localScale.z;
		float offsetX = Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * surforgeSettings.root.transform.localScale.x;

		obj.deleting = true; //for instances not report deleting PolyLassoObject
		for (int i=0; i< 8; i++) {
			GameObject inst = (GameObject)Instantiate(obj.gameObject);
			DestroyImmediate(inst.GetComponent<PolyLassoObject>());
			UpdateSeamlessInstanceTransform(inst, i, offsetX, offsetZ);

			//seamless highlight
			ApplySeamlessHighlight(inst);
			foreach(Transform child in inst.transform) {
				ApplySeamlessHighlight(child.gameObject);
			}

			obj.seamlessInstances[i] = inst;
			if (surforgeSettings.seamlessInstances != null) {
				surforgeSettings.seamlessInstances.Add(inst);
			}
			else {
				surforgeSettings.seamlessInstances = new List<GameObject>();
			}
		}
		obj.deleting = false;

	}

	public static void UpdateSeamlessInstanceTransform(GameObject inst, int i, float offsetX, float offsetZ) {
		if (i == 0) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z); 
		if (i == 1) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z); 
		
		if (i == 2) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 3) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ); 
		
		if (i == 4) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 5) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ);
		
		if (i == 6) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 7) inst.transform.position = new Vector3(inst.transform.localPosition.x * surforgeSettings.root.transform.localScale.x, inst.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, inst.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ);
		
		inst.transform.localScale = new Vector3(inst.transform.localScale.x * surforgeSettings.root.transform.localScale.x,
		                                        inst.transform.localScale.y * surforgeSettings.root.transform.localScale.y,
		                                        inst.transform.localScale.z * surforgeSettings.root.transform.localScale.z);

	}
	

	static void ClearGlobalSeamlessInstancesList() {
		if (surforgeSettings.seamlessInstances != null) {
			for (int i=0; i<surforgeSettings.seamlessInstances.Count; i++) {
				if (surforgeSettings.seamlessInstances[i] != null) DestroyImmediate(surforgeSettings.seamlessInstances[i].gameObject);
			}
		}
		surforgeSettings.seamlessInstances = new List<GameObject>();
	}


	//----- Poly Lasso Undo-----
	static void OnUndoRedo() {
		if (surforgeSettings) {
			if (surforgeSettings.root != null) {

				CheckObjectNeedRebuildToFixUnityUndoBug();
				RemoveNullPolyLassoObjectsFromList();
				if(surforgeSettings.seamless) ResetAllSeamlessInstances();

			}
		}

		//check for remove old surforgeSettings
		SurforgeSettings[] sceneSurforgeSettings = (SurforgeSettings[])GameObject.FindObjectsOfType(typeof(SurforgeSettings)); 
		for (int i=0; i< sceneSurforgeSettings.Length; i++) {
			if (sceneSurforgeSettings[i] != surforgeSettings) DestroyImmediate(sceneSurforgeSettings[i].gameObject);
		}

		SurforgeRoot[] sceneSurforgeRootObjects = (SurforgeRoot[])GameObject.FindObjectsOfType(typeof(SurforgeRoot));
		for (int i=0; i< sceneSurforgeRootObjects.Length; i++) {
			if (surforgeSettings == null) {
				DestroyImmediate(sceneSurforgeRootObjects[i].gameObject);
			}
			else {
				if (surforgeSettings.root != null) {
					if (sceneSurforgeRootObjects[i].gameObject != surforgeSettings.root) DestroyImmediate(sceneSurforgeRootObjects[i].gameObject);
				}
				else {
					DestroyImmediate(sceneSurforgeRootObjects[i].gameObject);
				}
			}
		}

		PolyLassoObject[] scenePolyLassoObjects = (PolyLassoObject[])GameObject.FindObjectsOfType(typeof(PolyLassoObject));
		for (int i=0; i< scenePolyLassoObjects.Length; i++) {
			if (scenePolyLassoObjects[i].transform.parent == null) DestroyImmediate(scenePolyLassoObjects[i].gameObject);
		}


		SurforgeTexturePreview[] sceneSurforgeTexturePreviewObjects = (SurforgeTexturePreview[])GameObject.FindObjectsOfType(typeof(SurforgeTexturePreview));
		for (int i=0; i < sceneSurforgeTexturePreviewObjects.Length; i++) {
			if (surforgeSettings == null) {
				DestroyImmediate(sceneSurforgeTexturePreviewObjects[i].gameObject);
			}
			else {
				if (surforgeSettings.extentTexturePreview != null) {
					if (surforgeSettings.extentTexturePreview != sceneSurforgeTexturePreviewObjects[i]) DestroyImmediate(sceneSurforgeTexturePreviewObjects[i].gameObject);
				}
				else {
					DestroyImmediate(sceneSurforgeTexturePreviewObjects[i].gameObject);
				}
			}
		}

	}

	static void RemoveNullPolyLassoObjectsFromList() {
		List<PolyLassoObject> tmp = new List<PolyLassoObject>();
		for (int i=0; i< surforgeSettings.polyLassoObjects.Count; i++) {
			tmp.Add (surforgeSettings.polyLassoObjects[i]);
		}
		surforgeSettings.polyLassoObjects.Clear();
		for (int i=0; i< tmp.Count; i++) {
			if (tmp[i] != null) {
				surforgeSettings.polyLassoObjects.Add (tmp[i]);
			}
		}
	}
	
	public static void RegisterUndoPolyLassoCreatedObject(PolyLassoObject obj) {
		Undo.RegisterCreatedObjectUndo(obj.gameObject, "Create object");
	}

	public static void RegisterUndoPlaceMeshObject(GameObject obj) {
		Undo.RegisterCreatedObjectUndo(obj.gameObject, "Create object");
	}

	static void CheckObjectNeedRebuildToFixUnityUndoBug() {
		if (surforgeSettings.polyLassoObjects == null) {
			surforgeSettings.polyLassoObjects = new List<PolyLassoObject>();
		}

		foreach (Transform child in surforgeSettings.root.transform) {
			PolyLassoObject pObj = child.gameObject.GetComponent<PolyLassoObject>();
			if (pObj != null) {

				//snap to objects
				if (!surforgeSettings.polyLassoObjects.Contains(pObj)) {
					surforgeSettings.polyLassoObjects.Add(pObj);
				}

				Renderer renderer = (Renderer)pObj.gameObject.GetComponent<Renderer>();
				if (renderer != null) {
					Material mat = (Material)renderer.sharedMaterial;
					if (mat == null) {

						RebuildPolyLassoObject(null, pObj, pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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

						renderer = (Renderer)pObj.gameObject.GetComponent<Renderer>();
						if (renderer != null) {
							mat = (Material)renderer.sharedMaterial;
							if (mat == null) {
								mat = new Material( surforgeSettings.materialGroups[pObj.materialID]);
								renderer.material = mat; 
							}
						}
					}
				}

			}

		}
	}





	public static void CopyMaterialToChosenID(Material sourceMaterial, int sourceGroupNum, Material destMaterial, int destGroupNum) {
		destMaterial.SetColor("_Tint" + destGroupNum, sourceMaterial.GetColor ("_Tint" + sourceGroupNum));
		destMaterial.SetColor("_SpecularTint" + destGroupNum, sourceMaterial.GetColor("_SpecularTint" + sourceGroupNum));
		
		destMaterial.SetFloat("_SpecularIntensity" + destGroupNum, sourceMaterial.GetFloat("_SpecularIntensity" + sourceGroupNum));
		destMaterial.SetFloat("_" + destGroupNum + "SpecularContrast", sourceMaterial.GetFloat("_" + sourceGroupNum + "SpecularContrast"));
		destMaterial.SetFloat("_" + destGroupNum + "SpecularBrightness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "SpecularBrightness"));
		
		destMaterial.SetFloat("_Glossiness" + destGroupNum, sourceMaterial.GetFloat ("_Glossiness" + sourceGroupNum));
		destMaterial.SetFloat("_GlossinessIntensity" + destGroupNum, sourceMaterial.GetFloat ("_GlossinessIntensity" + sourceGroupNum));
		destMaterial.SetFloat("_" + destGroupNum + "GlossinessContrast", sourceMaterial.GetFloat ("_" + sourceGroupNum + "GlossinessContrast"));
		destMaterial.SetFloat("_" + destGroupNum + "GlossinessBrightness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "GlossinessBrightness"));
		
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Intensity", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Intensity"));
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Intensity", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Intensity"));
		destMaterial.SetVector("_" + destGroupNum + "WornEdgesNoiseMix", sourceMaterial.GetVector("_" + sourceGroupNum + "WornEdgesNoiseMix"));
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesAmount", sourceMaterial.GetFloat ("_" + sourceGroupNum + "WornEdgesAmount"));
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesOpacity", sourceMaterial.GetFloat ("_" + sourceGroupNum + "WornEdgesOpacity"));
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesContrast", sourceMaterial.GetFloat ("_" + sourceGroupNum + "WornEdgesContrast"));
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesBorder", sourceMaterial.GetFloat ("_" + sourceGroupNum + "WornEdgesBorder"));
		destMaterial.SetColor("_" + destGroupNum + "WornEdgesBorderTint", sourceMaterial.GetColor ("_" + sourceGroupNum + "WornEdgesBorderTint"));
		destMaterial.SetColor("_" + destGroupNum + "UnderlyingDiffuseTint", sourceMaterial.GetColor ("_" + sourceGroupNum + "UnderlyingDiffuseTint"));
		destMaterial.SetColor("_" + destGroupNum + "UnderlyingSpecularTint", sourceMaterial.GetColor ("_" + sourceGroupNum + "UnderlyingSpecularTint"));
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingDiffuse", sourceMaterial.GetFloat ("_" + sourceGroupNum + "UnderlyingDiffuse"));
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingSpecular", sourceMaterial.GetFloat ("_" + sourceGroupNum + "UnderlyingSpecular"));
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingGlossiness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "UnderlyingGlossiness"));
		
		destMaterial.SetFloat("_NormalsStrength" + destGroupNum, sourceMaterial.GetFloat ("_NormalsStrength" + sourceGroupNum));
		destMaterial.SetFloat("_BumpMapStrength" + destGroupNum, sourceMaterial.GetFloat ("_BumpMapStrength" + sourceGroupNum));

		destMaterial.SetFloat("_OcclusionMapStrength" + destGroupNum, sourceMaterial.GetFloat ("_OcclusionMapStrength" + sourceGroupNum));
		
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Specular", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Specular"));
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Glossiness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Glossiness"));
		destMaterial.SetColor("_" + destGroupNum + "Paint1Color", sourceMaterial.GetColor ("_" + sourceGroupNum + "Paint1Color"));
		destMaterial.SetTexture("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTexture("_" + sourceGroupNum + "Paint1MaskTex"));
		destMaterial.SetTextureScale("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTextureScale("_" + sourceGroupNum + "Paint1MaskTex"));
		destMaterial.SetTextureOffset("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTextureOffset("_" + sourceGroupNum + "Paint1MaskTex"));
		destMaterial.SetVector("_" + destGroupNum + "Paint1NoiseMix", sourceMaterial.GetVector ("_" + sourceGroupNum + "Paint1NoiseMix"));
		
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Specular", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Specular"));
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Glossiness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Glossiness"));
		destMaterial.SetColor("_" + destGroupNum + "Paint2Color", sourceMaterial.GetColor ("_" + sourceGroupNum + "Paint2Color"));
		destMaterial.SetTexture("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTexture ("_" + sourceGroupNum + "Paint2MaskTex"));
		destMaterial.SetTextureScale("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTextureScale ("_" + sourceGroupNum + "Paint2MaskTex"));
		destMaterial.SetTextureOffset("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTextureOffset ("_" + sourceGroupNum + "Paint2MaskTex"));
		destMaterial.SetVector("_" + destGroupNum + "Paint2NoiseMix", sourceMaterial.GetVector ("_" + sourceGroupNum + "Paint2NoiseMix"));
		
		
		destMaterial.SetTexture("_Texture" + destGroupNum, sourceMaterial.GetTexture ("_Texture" + sourceGroupNum));
		destMaterial.SetTextureScale("_Texture" + destGroupNum, sourceMaterial.GetTextureScale ("_Texture" + sourceGroupNum));
		destMaterial.SetTextureOffset("_Texture" + destGroupNum, sourceMaterial.GetTextureOffset ("_Texture" + sourceGroupNum));

		destMaterial.SetTexture("_BumpMap" + destGroupNum, sourceMaterial.GetTexture ("_BumpMap" + sourceGroupNum));
		destMaterial.SetTextureScale("_BumpMap" + destGroupNum, sourceMaterial.GetTextureScale ("_BumpMap" + sourceGroupNum));
		destMaterial.SetTextureOffset("_BumpMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_BumpMap" + sourceGroupNum));

		destMaterial.SetTexture("_OcclusionMap" + destGroupNum, sourceMaterial.GetTexture ("_OcclusionMap" + sourceGroupNum));

		destMaterial.SetTexture("_SpecularMap" + destGroupNum, sourceMaterial.GetTexture ("_SpecularMap" + sourceGroupNum)); 
		destMaterial.SetTextureScale("_SpecularMap" + destGroupNum, sourceMaterial.GetTextureScale ("_SpecularMap" + sourceGroupNum));
		destMaterial.SetTextureOffset("_SpecularMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_SpecularMap" + sourceGroupNum));

		destMaterial.SetFloat("_UseSpecularMap" + destGroupNum, sourceMaterial.GetFloat ("_UseSpecularMap" + sourceGroupNum)); 
		destMaterial.SetFloat("_GlossinessFromAlpha" + destGroupNum, sourceMaterial.GetFloat ("_GlossinessFromAlpha" + sourceGroupNum)); 

		destMaterial.SetTexture("_EmissionMap" + destGroupNum, sourceMaterial.GetTexture ("_EmissionMap" + sourceGroupNum)); 
		destMaterial.SetTextureScale("_EmissionMap" + destGroupNum, sourceMaterial.GetTextureScale ("_EmissionMap" + sourceGroupNum));
		destMaterial.SetTextureOffset("_EmissionMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_EmissionMap" + sourceGroupNum));
		destMaterial.SetColor("_EmissionMapTint" + destGroupNum, sourceMaterial.GetColor ("_EmissionMapTint" + sourceGroupNum));
		destMaterial.SetFloat("_EmissionMapIntensity" + destGroupNum, sourceMaterial.GetFloat ("_EmissionMapIntensity" + sourceGroupNum));

		destMaterial.SetFloat("_" + destGroupNum + "GlobalTransparency", sourceMaterial.GetFloat ("_" + sourceGroupNum + "GlobalTransparency"));
		destMaterial.SetFloat("_" + destGroupNum + "AlbedoTransparency", sourceMaterial.GetFloat ("_" + sourceGroupNum + "AlbedoTransparency"));
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Transparency", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Transparency"));
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Transparency", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Transparency"));

		destMaterial.SetFloat("_" + destGroupNum + "MaterialRotation", sourceMaterial.GetFloat ("_" + sourceGroupNum + "MaterialRotation"));
	}


	public static void CopyStandardSpecularMaterialToChosenID(Material sourceMaterial, int sourceGroupNum, Material destMaterial, int destGroupNum) {
		destMaterial.SetColor("_Tint" + destGroupNum, sourceMaterial.GetColor ("_Color"));

		if (sourceMaterial.GetTexture ("_SpecGlossMap") == null) {
			destMaterial.SetColor("_SpecularTint" + destGroupNum, sourceMaterial.GetColor("_SpecColor"));
		}
		
		destMaterial.SetFloat("_SpecularIntensity" + destGroupNum, 1.0f);
		destMaterial.SetFloat("_" + destGroupNum + "SpecularContrast", 0);
		destMaterial.SetFloat("_" + destGroupNum + "SpecularBrightness", 0.5f);
		
		destMaterial.SetFloat("_Glossiness" + destGroupNum, sourceMaterial.GetFloat ("_Glossiness"));
		destMaterial.SetFloat("_GlossinessIntensity" + destGroupNum, 1.0f);
		destMaterial.SetFloat("_" + destGroupNum + "GlossinessContrast", 0);
		destMaterial.SetFloat("_" + destGroupNum + "GlossinessBrightness", 0.5f);
		
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Intensity", 0);
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Intensity", 0);
		destMaterial.SetVector("_" + destGroupNum + "WornEdgesNoiseMix", new Vector4(0, 0, 0, 1));
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesAmount", 0);
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesOpacity", 0);
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesContrast", 1.0f);
		destMaterial.SetFloat("_" + destGroupNum + "WornEdgesBorder", 0);
		destMaterial.SetColor("_" + destGroupNum + "WornEdgesBorderTint", new Color(0, 0, 0, 1));
		destMaterial.SetColor("_" + destGroupNum + "UnderlyingDiffuseTint", new Color(0, 0, 0, 1));
		destMaterial.SetColor("_" + destGroupNum + "UnderlyingSpecularTint", new Color(1, 1, 1, 1));
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingDiffuse", 1.0f);
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingSpecular", 1.0f);
		destMaterial.SetFloat("_" + destGroupNum + "UnderlyingGlossiness", 1.0f);
		
		//destMaterial.SetFloat("_NormalsStrength" + destGroupNum, sourceMaterial.GetFloat ("_NormalsStrength" + sourceGroupNum));
		destMaterial.SetFloat("_BumpMapStrength" + destGroupNum, sourceMaterial.GetFloat("_BumpScale"));
		
		destMaterial.SetFloat("_OcclusionMapStrength" + destGroupNum, 1.0f);
		
		//destMaterial.SetFloat("_" + destGroupNum + "Paint1Specular", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Specular"));
		//destMaterial.SetFloat("_" + destGroupNum + "Paint1Glossiness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint1Glossiness"));
		//destMaterial.SetColor("_" + destGroupNum + "Paint1Color", sourceMaterial.GetColor ("_" + sourceGroupNum + "Paint1Color"));
		//destMaterial.SetTexture("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTexture("_" + sourceGroupNum + "Paint1MaskTex"));
		//destMaterial.SetTextureScale("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTextureScale("_" + sourceGroupNum + "Paint1MaskTex"));
		//destMaterial.SetTextureOffset("_" + destGroupNum + "Paint1MaskTex", sourceMaterial.GetTextureOffset("_" + sourceGroupNum + "Paint1MaskTex"));
		//destMaterial.SetVector("_" + destGroupNum + "Paint1NoiseMix", sourceMaterial.GetVector ("_" + sourceGroupNum + "Paint1NoiseMix"));
		
		//destMaterial.SetFloat("_" + destGroupNum + "Paint2Specular", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Specular"));
		//destMaterial.SetFloat("_" + destGroupNum + "Paint2Glossiness", sourceMaterial.GetFloat ("_" + sourceGroupNum + "Paint2Glossiness"));
		//destMaterial.SetColor("_" + destGroupNum + "Paint2Color", sourceMaterial.GetColor ("_" + sourceGroupNum + "Paint2Color"));
		//destMaterial.SetTexture("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTexture ("_" + sourceGroupNum + "Paint2MaskTex"));
		//destMaterial.SetTextureScale("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTextureScale ("_" + sourceGroupNum + "Paint2MaskTex"));
		//destMaterial.SetTextureOffset("_" + destGroupNum + "Paint2MaskTex", sourceMaterial.GetTextureOffset ("_" + sourceGroupNum + "Paint2MaskTex"));
		//destMaterial.SetVector("_" + destGroupNum + "Paint2NoiseMix", sourceMaterial.GetVector ("_" + sourceGroupNum + "Paint2NoiseMix"));
		
		
		destMaterial.SetTexture("_Texture" + destGroupNum, sourceMaterial.GetTexture ("_MainTex"));
		destMaterial.SetTextureScale("_Texture" + destGroupNum, sourceMaterial.GetTextureScale ("_MainTex"));
		destMaterial.SetTextureOffset("_Texture" + destGroupNum, sourceMaterial.GetTextureOffset ("_MainTex"));
		
		destMaterial.SetTexture("_BumpMap" + destGroupNum, sourceMaterial.GetTexture ("_BumpMap"));
		destMaterial.SetTextureScale("_BumpMap" + destGroupNum, sourceMaterial.GetTextureScale ("_MainTex"));
		destMaterial.SetTextureOffset("_BumpMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_MainTex"));
		
		destMaterial.SetTexture("_OcclusionMap" + destGroupNum, sourceMaterial.GetTexture ("_OcclusionMap"));
		
		destMaterial.SetTexture("_SpecularMap" + destGroupNum, sourceMaterial.GetTexture ("_SpecGlossMap")); 
		destMaterial.SetTextureScale("_SpecularMap" + destGroupNum, sourceMaterial.GetTextureScale ("_MainTex"));
		destMaterial.SetTextureOffset("_SpecularMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_MainTex"));

		if (sourceMaterial.GetTexture ("_SpecGlossMap") != null) {
			destMaterial.SetFloat("_UseSpecularMap" + destGroupNum, 1.0f); 
			destMaterial.SetFloat("_GlossinessFromAlpha" + destGroupNum, 1.0f);  
		}
		
		destMaterial.SetTexture("_EmissionMap" + destGroupNum, sourceMaterial.GetTexture ("_EmissionMap")); 
		destMaterial.SetTextureScale("_EmissionMap" + destGroupNum, sourceMaterial.GetTextureScale ("_MainTex"));
		destMaterial.SetTextureOffset("_EmissionMap" + destGroupNum, sourceMaterial.GetTextureOffset ("_MainTex"));

		Color emissionColor = sourceMaterial.GetColor ("_EmissionColor");
		emissionColor = new Color(Mathf.Clamp01(emissionColor.r), Mathf.Clamp01(emissionColor.g), Mathf.Clamp01(emissionColor.b), 1);
		destMaterial.SetColor("_EmissionMapTint" + destGroupNum, emissionColor);
		//destMaterial.SetFloat("_EmissionMapIntensity" + destGroupNum, sourceMaterial.GetFloat ("_EmissionMapIntensity" + sourceGroupNum));
		
		destMaterial.SetFloat("_" + destGroupNum + "GlobalTransparency", 0);
		destMaterial.SetFloat("_" + destGroupNum + "AlbedoTransparency", 0);
		destMaterial.SetFloat("_" + destGroupNum + "Paint1Transparency", 0);
		destMaterial.SetFloat("_" + destGroupNum + "Paint2Transparency", 0);

		destMaterial.SetFloat("_" + destGroupNum + "MaterialRotation", 0);
	}





	//shatter

	public static void ShatterPolyLassoObjects() {
		LogAction("Shatter", "", "");

		SurforgeShatter shatter = Surforge.surforgeSettings.shatterPresets.shatterPresets[Surforge.surforgeSettings.activeShatterPreset];

		GameObject[] gameObjects = Selection.gameObjects;

		List<Object> resultSelection = new List<Object>();

		List<GameObject> objectsForDelete = new List<GameObject>();

		for (int i=0; i< gameObjects.Length; i++) {
			PolyLassoObject pObj = (PolyLassoObject)gameObjects[i].GetComponent<PolyLassoObject>();
			if (pObj != null) {

				Selection.objects = new Object[1] {gameObjects[i]};

				List<List<Vector3>> shatterShapes = GetShatterShapes(pObj); 
				shatterShapes = FilterDelaunayResultShapes(shatterShapes);

				//delaunay shatter
				if (shatter.delaunay) {

					//remove noise before shatter to optimize speed
					bool wasNoise = pObj.noise;
					pObj.noise = false;

					objectsForDelete.Add(gameObjects[i]);

					//PolyLassoObjectToWorldShape 
					List<Vector3> delaunayBordersSplitter = PolyLassoObjectToWorldShape(pObj);
					delaunayBordersSplitter.Add(delaunayBordersSplitter[0]);

	
					for (int s=0; s < shatterShapes.Count; s++) {
						if (shatterShapes[s].Count >= 3) {
							Vector2[] poly = new Vector2[shatterShapes[s].Count];
							for (int t=0; t< poly.Length; t++) {
								poly[t] = new Vector2(shatterShapes[s][t].x, shatterShapes[s][t].z);
							}

							List<Vector3> delaunaySecondSplitter = new List<Vector3>();
							for (int t=0; t<delaunayBordersSplitter.Count; t++) {
								delaunaySecondSplitter.Add(delaunayBordersSplitter[t]);
							}

							//set size 
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
							
							shatterShapes[s] = PolyLassoObjectToWorldShapeGameObjectAndShape(relativeTransforms, shatterShapes[s]);

							GameObject delaunayResult = PolygonLassoBuildObject(null, false, shatterShapes[s], pObj.bevelAmount, pObj.bevelSteps, pObj.offsets, pObj.heights,
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
							if (delaunayResult != null) {
								//set size
								delaunayResult.transform.parent = null;
								delaunayResult.transform.localScale = objLocalScale;
								delaunayResult.transform.localPosition = objLocalPosition;
								delaunayResult.transform.localRotation = objLocalRotation;
								delaunayResult.transform.parent = surforgeSettings.root.transform;

								pObj.transform.localScale = objLocalScale;
								pObj.transform.localPosition = objLocalPosition;
								pObj.transform.localRotation = objLocalRotation;
								pObj.transform.parent = pObjParent;

								Selection.objects = new Object[1] {delaunayResult};

								Vector3[] pointsToCheck = delaunayBordersSplitter.ToArray();			
								if (CheckIfShapeClockwise(pointsToCheck)) delaunayBordersSplitter.Reverse();

								SplitSelectedPolyLassoObjects(delaunayBordersSplitter); 

								for (int t=0; t<delaunayBordersSplitter.Count; t++) {
									delaunaySecondSplitter = CyclePolyLassoShape(delaunaySecondSplitter);
									SplitSelectedPolyLassoObjects(delaunaySecondSplitter); 
								}

								RemoveDelaunayShatterObjectsOutsideShape(PolyLassoObjectToWorldShape(pObj));

								//set noise settings back
								for (int h=0; h<Selection.gameObjects.Length; h++) {
									PolyLassoObject pObjResult = (PolyLassoObject)Selection.gameObjects[h].GetComponent<PolyLassoObject>();
									if (pObjResult) {
										pObjResult.noise = wasNoise;
										RebuildPolyLassoObjectSameSettings(pObjResult);
									}
								}


							}
						}

					}
				}

				//simple shatter
				else {
					for (int s=0; s < shatterShapes.Count; s++) {
					 
						Vector3[] pointsToCheck = shatterShapes[s].ToArray();
						if (Surforge.CheckIfShapeClockwise(pointsToCheck)) shatterShapes[s].Reverse();
						Surforge.SplitSelectedPolyLassoObjects(shatterShapes[s]);
					}
				}

				for (int s=0; s< Selection.gameObjects.Length; s++) {
					resultSelection.Add(Selection.gameObjects[s]);
				}
			}
		}

		Selection.objects = resultSelection.ToArray();

		for (int i=0; i<objectsForDelete.Count; i++) { 
			if (objectsForDelete[i]) {
				if (surforgeSettings.seamless) {
					PolyLassoObject pObjToDelete = (PolyLassoObject)objectsForDelete[i].GetComponent<PolyLassoObject>();
					if (pObjToDelete) {
						RemoveSeamlessInstances(pObjToDelete);
						pObjToDelete.deleting = true;
					}
				}
				Undo.DestroyObjectImmediate(objectsForDelete[i].gameObject);
			}
		}

	}

	static List<Vector3> CyclePolyLassoShape(List<Vector3> shape) {
		List<Vector3> result = new List<Vector3>();
		result.Add(shape[shape.Count-1]);

		for (int i=0; i<shape.Count-1; i++) {
			result.Add(shape[i]);
		}

		return result;
	}


	static void RemoveDelaunayShatterObjectsOutsideShape(List<Vector3> shape) {
		Vector2[] poly = new Vector2[shape.Count];
		for (int i=0; i<shape.Count; i++) {
			poly[i] = new Vector2(shape[i].x, shape[i].z);
		}

		List<GameObject> objectsForDelete = new List<GameObject>();

		for (int i=0; i<Selection.gameObjects.Length; i++) {
			PolyLassoObject pObj = (PolyLassoObject)Selection.gameObjects[i].GetComponent<PolyLassoObject>();
			if (pObj) {
				List<Vector3> testShape = PolyLassoObjectToWorldShape(pObj);
				bool outside = false;
				for (int s=0; s< testShape.Count; s++) {
					if (IsPointInPolygon(poly.Length, poly, testShape[s].x, testShape[s].z, false)) {
						if (!IsPointOnShape(poly, new Vector2(testShape[s].x, testShape[s].z))) {
							outside = true;
							break;
						}
					}
				}
				if (outside) {
					objectsForDelete.Add(pObj.gameObject);
				}
			}
		}

		for (int i=0; i<objectsForDelete.Count; i++) {
			if (objectsForDelete[i]) {
				if(surforgeSettings.seamless) {
					PolyLassoObject pObjToDelete = (PolyLassoObject)objectsForDelete[i].GetComponent<PolyLassoObject>();
					if (pObjToDelete) {
						RemoveSeamlessInstances(pObjToDelete);
						pObjToDelete.deleting = true;
					}
				}
				DestroyImmediate(objectsForDelete[i].gameObject);
			}
		}
	}

	static bool IsPointOnShape(Vector2[] poly, Vector2 point) {
		bool result = false;

		for (int i=0; i<poly.Length; i++) {
			Vector2 segmentStart = poly[i];
			Vector2 segmentEnd = new Vector2();

			if (i == (poly.Length - 1)) segmentEnd = poly[0];
			else segmentEnd = poly[i+1];

			if (IsPointOnSegment(segmentStart, segmentEnd, point)) {
				result = true;
				break;
			}
		}

		return result;
	}

	static List<List<Vector3>> GetShatterShapes(PolyLassoObject pObj) {
		List<Vector3> shape = PolyLassoObjectToWorldShape(pObj);
		List<List<Vector3>> result = new List<List<Vector3>>();
		SurforgeShatter shatter = Surforge.surforgeSettings.shatterPresets.shatterPresets[Surforge.surforgeSettings.activeShatterPreset];

		if (shatter.optimalLine) {
			result = ShatterOptimalLine(shape);
			return result;
		}
		else {
			if (shatter.delaunay) {
				result = ShatterDelaunay(shape,false);

				return result;
			}
		}


		return result;
	}



	// shatter Delaunay
	static List<Vector3> CreateDelaunayPointGrid(List<Vector3> shape, SurforgeShatter shatterPreset) {
		List<Vector3> result = new List<Vector3>();

		//get center vertex and bounds
		float centerX = 0;
		float centerZ = 0;
		
		float minX = Mathf.Infinity;
		float maxX = Mathf.NegativeInfinity;
		
		float minZ = Mathf.Infinity;
		float maxZ = Mathf.NegativeInfinity;
		
		for (int i=0; i< shape.Count; i++) {
			centerX = centerX + shape[i].x;
			centerZ = centerZ + shape[i].z;
			
			if (shape[i].x < minX) minX = shape[i].x;
			if (shape[i].x > maxX) maxX = shape[i].x;
			
			if (shape[i].z < minZ) minZ = shape[i].z;
			if (shape[i].z > maxZ) maxZ = shape[i].z;
		}
		centerX = centerX / (float)shape.Count;
		centerZ = centerZ / (float)shape.Count;

		//create bounds
		result.Add(new Vector3(minX - 100.0f + Random.Range(-25.0f, 25.0f), shape[0].y, minZ -100.0f + Random.Range(-25.0f, 25.0f)));
		result.Add(new Vector3(maxX + 100.0f + Random.Range(-25.0f, 25.0f), shape[0].y, minZ -100.0f + Random.Range(-25.0f, 25.0f)));
		result.Add(new Vector3(minX - 100.0f + Random.Range(-25.0f, 25.0f), shape[0].y, maxZ + 100.0f + Random.Range(-25.0f, 25.0f)));
		result.Add(new Vector3(maxX + 100.0f + Random.Range(-25.0f, 25.0f), shape[0].y, maxZ + 100.0f + Random.Range(-25.0f, 25.0f)));

		//create point grid

		float gridStepX = shatterPreset.gridStepX;
		if (gridStepX == 0) gridStepX = 1;
		
		float gridStepZ = shatterPreset.gridStepZ;
		if (gridStepZ == 0) gridStepZ = 1;

		for(float i = centerX; i < maxX; i = i + gridStepX) {
			for(float s = centerZ; s < maxZ; s = s + gridStepZ) {
				result.Add(new Vector3(i, shape[0].y, s));
			}
		}
		for(float i = centerX; i > minX; i = i - gridStepX) {
			for(float s = centerZ; s > minZ; s = s - gridStepZ) {
				if (! ((i == centerX) && (s == centerZ))) {
					result.Add(new Vector3(i, shape[0].y, s));
				}
			}
		}

		for(float i = centerX + gridStepX; i < maxX; i = i + gridStepX) {
			for(float s = centerZ - gridStepZ; s > minZ; s = s - gridStepZ) {
				result.Add(new Vector3(i, shape[0].y, s));
			}
		}
		for(float i = centerX - gridStepX; i > minX; i = i - gridStepX) {
			for(float s = centerZ + gridStepZ; s < maxZ; s = s + gridStepZ) {
				result.Add(new Vector3(i, shape[0].y, s));
			}
		}

		//randomize point grid
		for (int i=0; i< result.Count; i++) {
			result[i] = result[i] + new Vector3( Random.Range(-shatterPreset.gridStepRandomOffset, shatterPreset.gridStepRandomOffset), 
			                                    shape[0].y, 
			                                    Random.Range(-shatterPreset.gridStepRandomOffset, shatterPreset.gridStepRandomOffset));
		}

		//second step
		if (shatterPreset.secondStep) {
			List<Vector3> secondStepPoints = new List<Vector3>();

			for (int i=0; i< result.Count; i++) {
				for (int s=0; s< Random.Range(shatterPreset.secondStepVertsMin, shatterPreset.secondStepVertsMax); s++) {
					secondStepPoints.Add(result[i] + new Vector3( Random.Range(-shatterPreset.secondStepRandomOffset, shatterPreset.secondStepRandomOffset), 
					                                             shape[0].y, 
					                                             Random.Range(-shatterPreset.secondStepRandomOffset, shatterPreset.secondStepRandomOffset)));
				}
			}

			for (int i=0; i< secondStepPoints.Count; i++) {
				result.Add(secondStepPoints[i]);
			}

			// third step
			if (shatterPreset.thirdStep) {
				List<Vector3> thirdStepPoints = new List<Vector3>();
				
				for (int i=0; i< result.Count; i++) {
					for (int s=0; s< Random.Range(shatterPreset.thirdStepVertsMin, shatterPreset.thirdStepVertsMax); s++) {
						thirdStepPoints.Add(result[i] + new Vector3( Random.Range(-shatterPreset.thirdStepRandomOffset, shatterPreset.thirdStepRandomOffset), 
						                                             shape[0].y, 
						                                            Random.Range(-shatterPreset.thirdStepRandomOffset, shatterPreset.thirdStepRandomOffset)));
					}
				}
				
				for (int i=0; i< thirdStepPoints.Count; i++) {
					result.Add(thirdStepPoints[i]);
				}
				
			}

		}

		//add shape points
		for (int i=0; i< shape.Count; i++) {
			result.Add(shape[i]);
		}
		
		
		//shuffle
		ShuffleVector3List(result);


		/*
		// apply point minimal distance treshold
		float pointMinDistance = shatterPreset.pointMinDistance;
		if (pointMinDistance < 0.1f) pointMinDistance = 0.1f;

		List<Vector3> resultFiltered = new List<Vector3>();

		for (int i=0; i< result.Count; i++) {
			bool remove = false;
			for (int s=0; s< result.Count; s++) {
				if (i != s) {
					if (Vector3.Distance (result[i], result[s]) < pointMinDistance) {
						remove = true;
						break;
					}
				}
			}
			if (!remove) {
				resultFiltered.Add(result[i]);
			}
		}


		return resultFiltered;
		*/
		return result;
	}

	static void ShuffleVector3List(List<Vector3> listToShuffle) {
		for (int i = listToShuffle.Count - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			Vector3 tmp = listToShuffle[i];
			listToShuffle[i] = listToShuffle[r];
			listToShuffle[r] = tmp;
		}
	}


	static List<List<Vector3>> ShatterDelaunay(List<Vector3> shape, bool triangulate) {
		Vector2[] shapePoly = new Vector2[shape.Count];
		for (int i=0; i < shape.Count; i++) {
			shapePoly[i] = new Vector2(shape[i].x, shape[i].z);
		}

		SurforgeShatter shatterPreset = surforgeSettings.shatterPresets.shatterPresets[surforgeSettings.activeShatterPreset];

		List<List<Vector3>> result = new List<List<Vector3>>();

		VoronoiMesh<Vertex2, Cell2, VoronoiEdge<Vertex2, Cell2>> voronoiMesh;

		Vertex2[] vertices = new Vertex2[shape.Count];


		if (triangulate) {
			for (int i=0; i< vertices.Length; i++) {
				vertices[i] = new Vertex2(shape[i].x, shape[i].z);
			}
			
			voronoiMesh = VoronoiMesh.Create<Vertex2, Cell2>(vertices);

			foreach (var vertex in voronoiMesh.Vertices) {
				List<Vector3> splitLine = new List<Vector3>();

				for (int i=0; i < vertex.Vertices.Length; i++) {
					splitLine.Add(new Vector3((float)vertex.Vertices[i].x, shape[0].y, (float)vertex.Vertices[i].y));
				}

				result.Add(splitLine); 
			}
		}

		else {

			//create point grid
			List<Vector3> newVerts = CreateDelaunayPointGrid(shape, shatterPreset);

			vertices = new Vertex2[newVerts.Count];

			for (int i=0; i< vertices.Length; i++) {
				vertices[i] = new Vertex2(newVerts[i].x, newVerts[i].z);
			}

			voronoiMesh = VoronoiMesh.Create<Vertex2, Cell2>(vertices);

			// foreach voronoi cell
			foreach(Cell2 v in voronoiMesh.Vertices) {

				//debug
				/*
				GameObject debugObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				debugObj.name = "center";
				debugObj.transform.position = new Vector3(v.Circumcenter.x, 0, v.Circumcenter.y) + new Vector3(0, 1.0f, 0);
				*/

				for (int i=0; i < v.Vertices.Length; i++) {
					List<Cell2> suroundingCells = FindVoronoiCellsAroundVertex(v.Vertices[i], voronoiMesh.Vertices);
					suroundingCells = SortVoronoiCells(v.Vertices[i], suroundingCells);

					//result shape from sorted points
					List<Vector3> splitLine = new List<Vector3>();
					for (int s=0; s< suroundingCells.Count; s++) {
						splitLine.Add(new Vector3(suroundingCells[s].Circumcenter.x, shape[0].y, suroundingCells[s].Circumcenter.y));
					}
					result.Add(splitLine);

				}

			}

			//debug
			/*
			for (int i=0; i < result.Count; i++) {
				DebugShape(result[i], new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), 1) ); 
			}
			*/

		}

		return result;
	}

	static List<List<Vector3>> FilterDelaunayResultShapes(List<List<Vector3>> delaunayShapes) {
		List<List<Vector3>> result = new List<List<Vector3>>();

		if (delaunayShapes.Count == 0) return result;

		//remove equal shapes
		List<List<Vector3>> tmp = new List<List<Vector3>>();
		for (int i=1; i< delaunayShapes.Count; i++) {
			tmp.Add(delaunayShapes[i]);
		}
		result.Add(delaunayShapes[0]);

		for (int s=0; s<tmp.Count; s++) {
			bool match = false;
			for (int t=0; t<result.Count; t++) {
				if (AreShapesEqual(tmp[s], result[t])) {
					match = true;
					break;
				}
			}
			if (!match) {
				result.Add(tmp[s]);
			}
		}
		// weld points that are to close 
		float weldTreshold = 0.1f;
		for (int i=0; i< result.Count; i++) {
			int timesToTryWeld = result[i].Count;
			for (int s=0; s< timesToTryWeld; s++) {
				if (!WeldShapeClosePoints(result[i], weldTreshold)) break;
			}
		}


		//check for clockwise
		for (int i=0; i< result.Count; i++) {
			Vector3[] pointsToCheck = result[i].ToArray();
			if (!CheckIfShapeClockwise(pointsToCheck)) result[i].Reverse();
		}

		return result;
	}

	static bool WeldShapeClosePoints(List<Vector3> shape, float weldTreshold) {
		if (shape.Count <= 3) return false;


		int pointToRemoveIndex = 0;
		bool removePoint = false;
		for (int i=0; i< shape.Count; i++) {
			Vector3 pointA = shape[i];
			Vector3 pointB = new Vector3();

			if (i == (shape.Count-1)) pointB = shape[0];
			else pointB = shape[i+1];

			if (Vector3.Distance(pointA, pointB) < weldTreshold) {
				removePoint = true;
				pointToRemoveIndex = i;
				break;
			}
		}

		if (removePoint) {
			shape.RemoveAt(pointToRemoveIndex);
			return true;
		}
		else return false;

	}


	static bool AreShapesEqual(List<Vector3> shapeA, List<Vector3> shapeB) {
		bool result = true;

		if (shapeA.Count != shapeB.Count) return false;

		for (int i=0; i < shapeA.Count; i++) {
			bool pointEqual = false;
			for (int s=0; s < shapeB.Count; s++) {
				if (IsPointsEqual3D(shapeA[i], shapeB[s])) {
					pointEqual = true;
					break;
				}
			}
			if (!pointEqual) {
				result = false;
				break;
			}
		}
	

		return result;
	}


	static List<Cell2> FindVoronoiCellsAroundVertex(Vertex2 centerVertex, IEnumerable<Cell2> allVerts) {
		List<Cell2> suroundingCells = new List<Cell2>();
		foreach (var s in allVerts) {
			bool match = false;
			foreach (var z in s.Vertices) {
				if (z == centerVertex) {
					match = true;
					break;
				}
			}
			if (match) suroundingCells.Add(s);
		}
		return suroundingCells;
	}

	static List<Cell2> SortVoronoiCells(Vertex2 centerVertex, List<Cell2> cells) {
		List<Cell2> sortedCells = new List<Cell2>();

		for (int i=0; i< cells.Count; i++) {

			List<Cell2> unsortedCells = new List<Cell2>();
			for (int u=0; u< cells.Count; u++) {
				if (u != i) unsortedCells.Add(cells[u]);
			}

			sortedCells = new List<Cell2>();
			sortedCells.Add(cells[i]);

			for (int t=0; t < cells.Count-1; t++) {
				int matchIndex = 0;
				bool match = false;
				for (int s=0; s< unsortedCells.Count; s++) {
					if (AreCellsNeighbours(sortedCells[sortedCells.Count-1], unsortedCells[s])) {
						matchIndex = s;
						match = true;
						break;
					}
				}
				if (match) {
					sortedCells.Add(unsortedCells[matchIndex]);
					unsortedCells.RemoveAt(matchIndex);
				}
			}
			if (sortedCells.Count == cells.Count) break;

		}


		return sortedCells;
	}

	static bool AreCellsNeighbours(Cell2 cellA, Cell2 cellB) {
		int matches = 0;
		for (int i=0; i< cellA.Vertices.Length; i++) {
			for (int s=0; s< cellB.Vertices.Length; s++) {
				if (cellA.Vertices[i] == cellB.Vertices[s]) {
					matches++;
					break;
				}
			}
			if (matches > 1) break;
		}

		if (matches > 1) return true;
		else return false;
	}





	// shatter optimal line
	static List<List<Vector3>> ShatterOptimalLine(List<Vector3> shape) {
		List<List<Vector3>> result = new List<List<Vector3>>();

		//if > 4, use delaunay triangulate
		if (shape.Count > 4) { 
			return ShatterDelaunay(shape, true);
		}
		else {
			result.Add(GetShatterOptimalLine(shape));
		}

		return result;
	}

	static List<Vector3> GetShatterOptimalLine(List<Vector3> shape) {
		List<Vector3> result = new List<Vector3>();

		int segmentIndexA = 0;
		int segmentIndexB = 0;


		segmentIndexA = FindLongestShapeSegmentIndex(shape); 

		List<int> sortedShapeIndexes = SortShapeIndexesBySegmentLength(shape);

		if (shape.Count < 4) {
			for (int i=0; i< sortedShapeIndexes.Count; i++) {
				segmentIndexB = sortedShapeIndexes[i];
				if ((segmentIndexA != segmentIndexB)) {
					result = CreateShatterSplitLine(shape, segmentIndexA, segmentIndexB);
					break;
				}
			}
		}
		if (shape.Count == 4) {
			for (int i=0; i< sortedShapeIndexes.Count; i++) {
				segmentIndexB = sortedShapeIndexes[i];
				if ((segmentIndexA != segmentIndexB)) {
					if ( (segmentIndexA != (segmentIndexB + 1)) && (segmentIndexA != (segmentIndexB - 1)) ) {
						if ( ((segmentIndexA == 0) && (segmentIndexB == (shape.Count-1))) || ((segmentIndexB == 0) && (segmentIndexA == (shape.Count-1))) ) {
						}
						else {
							result = CreateShatterSplitLine(shape, segmentIndexA, segmentIndexB);
							break;
						}
					}
				}
			}
		}


		return result;
	}

	static bool TestSplitShapeIntersectShapeOtherThanConnectedSegments(List<Vector3> shape, List<Vector3> splitLine, int segmentIndexA, int segmentIndexB) {
		bool result = false;

		for (int i=0; i< shape.Count; i++) {
			if ((i != segmentIndexA) && (i != segmentIndexB)) {
				Vector2 shapeSegmentStart = new Vector2(shape[i].x, shape[i].z);
				Vector2 shapeSegmentEnd = new Vector2();
				if (i == (shape.Count-1)) shapeSegmentEnd = new Vector2(shape[0].x, shape[0].z);
				else shapeSegmentEnd = new Vector2(shape[i+1].x, shape[i+1].z);

				for (int s=0; s< splitLine.Count -1; s++) {
					Vector2 splitSegmentStart = new Vector2(splitLine[s].x, splitLine[s].z);
					Vector2 splitSegmentEnd = new Vector2(splitLine[s+1].x, splitLine[s+1].z);

					if (TestLinesIntersection(shapeSegmentStart, shapeSegmentEnd, splitSegmentStart, splitSegmentEnd)) {
						if (TestSegmentIntersection(shapeSegmentStart, shapeSegmentEnd, splitSegmentStart, splitSegmentEnd)) {
							return true;
						}
					}
				}
			}
		}

		return result;
	}


	static List<Vector3> CreateShatterSplitLine (List<Vector3> shape, int segmentIndexA, int segmentIndexB) {
		List<Vector3> result = new List<Vector3>();

		Vector2 segmentAstart = new Vector2 (shape[segmentIndexA].x, shape[segmentIndexA].z);
		Vector2 segmentBstart = new Vector2 (shape[segmentIndexB].x, shape[segmentIndexB].z);
		
		Vector2 segmentAend = new Vector2();
		Vector2 segmentBend = new Vector2();
		
		if (segmentIndexA == (shape.Count - 1)) segmentAend = new Vector2 (shape[0].x, shape[0].z);
		else segmentAend = new Vector2 (shape[segmentIndexA+1].x, shape[segmentIndexA+1].z);
		
		if (segmentIndexB == (shape.Count - 1)) segmentBend = new Vector2 (shape[0].x, shape[0].z);
		else segmentBend = new Vector2 (shape[segmentIndexB+1].x, shape[segmentIndexB+1].z);
		
		Vector2 resultA = RandomPointOnSegment(segmentAstart, segmentAend);
		Vector2 resultB = RandomPointOnSegment(segmentBstart, segmentBend);
		
		result.Add(new Vector3(resultA.x, shape[0].y, resultA.y));
		result.Add(new Vector3(resultB.x, shape[0].y, resultB.y));

		return result;
	}

	static List<int> SortShapeIndexesBySegmentLength(List<Vector3> shape) {
		List<int> result = new List<int>();
		List<int> tmp = new List<int>();

		for (int i=0; i< shape.Count; i++) {
			tmp.Add(i);
		}

		for (int i=0; i< shape.Count; i++) {
			float maxDistance =0;
			int resultIndex = 0;
			int indexToRemove = 0;

			for (int s=0; s< tmp.Count; s++) {
				Vector3 a = shape[tmp[s]];
				Vector3 b = new Vector3();

				if (tmp[s] == (shape.Count-1)) b = shape[0];
				else b = shape[tmp[s]+1];

				float distance = Vector3.Distance (a, b);
				if (distance > maxDistance) {
					maxDistance = distance;
					resultIndex = tmp[s];
					indexToRemove = s;
				}
			}
			result.Add(resultIndex);
			if (tmp.Count > 0) {
				tmp.RemoveAt(indexToRemove);
			}
			if (tmp.Count == 0) break;
		}

		return result;
	}


	static int FindLongestShapeSegmentIndex(List<Vector3> shape) {
		int result = 0;

		float maxDistance = 0;
		for (int i=0; i< shape.Count; i++) {
			Vector3 a = shape[i];
			Vector3 b = new Vector3();

			if (i == (shape.Count-1)) b = shape[0];
			else b = shape[i+1];

			float distance = Vector3.Distance (a, b);
			if (distance > maxDistance) {
				maxDistance = distance;
				result = i;
			}
		}

		return result;
	}


	static Vector2 RandomPointOnSegment(Vector2 segmentStart, Vector2 segmentEnd) {
		Vector2 result = new Vector2();

		Vector2 v = (segmentEnd - segmentStart) * (0.5f + Random.Range(-0.2f, 0.2f));
		result = segmentStart + v;

		return result; 
	}  


	public static void LogAction(string action, string hotkey, string secondHotkey) {
		if (surforgeSettings != null) {
			surforgeSettings.lastActionText = action;
			surforgeSettings.lastActionHotkey = hotkey;
			surforgeSettings.lastActionSecondHotkey = secondHotkey;
			surforgeSettings.lastActionTimer = Time.realtimeSinceStartup + 4.0f; 
		}
	} 


	public static float GetShapeLength2D(List<Vector3> shape) {
		float result = 0;
		
		for (int i=0; i < shape.Count; i++) {
			Vector2 pointA = new Vector2(shape[i].x, shape[i].z);
			Vector2 pointB = new Vector2();
			
			if (i == (shape.Count-1)) pointB = new Vector2(shape[0].x, shape[0].z);
			else pointB = new Vector2(shape[i+1].x, shape[i+1].z);
			
			result = result + Vector2.Distance(pointA, pointB);
		}
		
		return result;
	}

	public static Vector3 GetWarpedPoint(float targetDistFromStart, Vector3[] shape, float height) {
		Vector3 result = new Vector3();

		float skippedDistance = 0;
		for (int i=0; i < shape.Length; i++) {
			Vector2 pointA = new Vector2(shape[i].x, shape[i].z);
			Vector2 pointB = new Vector2();
			
			if (i == (shape.Length-1)) pointB = new Vector2(shape[0].x, shape[0].z);
			else pointB = new Vector2(shape[i+1].x, shape[i+1].z);

			float segmentLength = Vector2.Distance(pointA, pointB);


			if (skippedDistance + segmentLength >= targetDistFromStart) {
				float distFromSegmentStart = targetDistFromStart - skippedDistance;
				float percentOfSegment = distFromSegmentStart / segmentLength;

				Vector2 offsetVector = (pointB - pointA) * percentOfSegment;
				Vector2 pointOnSegment = pointA + offsetVector;

				result = new Vector3(pointOnSegment.x, height, pointOnSegment.y);

				break;
			}


			skippedDistance = skippedDistance + segmentLength;
		}

		return result;
	}


	public static int[] GetShapeSubdivideValuesVector3(Vector3[] shape, float minEdge) {
		int[] result = new int[shape.Length];
		
		for (int i=0; i < shape.Length; i++) {
			Vector2 v1 = new Vector2(shape[i].x, shape[i].z);
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = new Vector2(shape[0].x, shape[0].z);
			else v2 = new Vector2(shape[i+1].x, shape[i+1].z);
			
			Vector2 line = v2 - v1;
			int value = Mathf.FloorToInt(line.magnitude / minEdge);
			
			result[i] = value;
		}
		
		return result;
	}
	
	public static Vector3[] SubdivideShapeVector3(Vector3[] shape, int[] subdivideValues, bool isOpen) {
		List<Vector3> result = new List<Vector3>();
		
		int shapeLength = shape.Length;
		if (isOpen) shapeLength = shapeLength - 1;
		
		for (int i=0; i < shapeLength; i++) {
			result.Add (shape[i]);
			Vector2 v1 = new Vector2(shape[i].x, shape[i].z);
			Vector2 v2;
			if (i == (shape.Length-1)) v2 = new Vector2(shape[0].x, shape[0].z);
			else v2 = new Vector2(shape[i+1].x, shape[i+1].z);
			Vector2 vLine = v2 - v1;
			
			Vector2[] newVerts = SplitLineToUniformSegments(v1, vLine, subdivideValues[i]);
			for (int s=0; s < newVerts.Length; s++) {
				result.Add(new Vector3(newVerts[s].x, shape[0].y, newVerts[s].y));
			}
		}
		
		return result.ToArray();
	}


	public static Vector2 GetPolyLassoObjectShapeCenter(PolyLassoObject pObj) {
		List<Vector3> worldShape = PolyLassoObjectToWorldShape(pObj);
		float xTotal = 0;
		float zTotal = 0;

		for (int i=0; i< worldShape.Count; i++) {
			xTotal = xTotal + worldShape[i].x;
			zTotal = zTotal + worldShape[i].z;
		}
		Vector2 result = new Vector2(xTotal / (float)worldShape.Count, zTotal / (float)worldShape.Count );
		
		return result; 
	}


	public static List<Vector3> FitShapeStartPoint(List<Vector3> shape) {
		Vector2 lineA = new Vector2(surforgeSettings.warpShapeCenterLinePoint.x, surforgeSettings.warpShapeCenterLinePoint.z);
		Vector2 lineB = new Vector2(surforgeSettings.warpShape[0].x, surforgeSettings.warpShape[0].z);

		Vector2 lineSegmentA = lineB + (lineA - lineB).normalized * 50.0f;
		Vector2 lineSegmentB = lineB + (lineA - lineB).normalized * 50.0f * -1.0f;

		List<Vector2> intersectionPoints = new List<Vector2>();
		List<int> intersectionIndexes = new List<int>();

		for (int i=0; i< shape.Count; i++) {
			Vector2 pointA = new Vector2(shape[i].x, shape[i].z);
			Vector2 pointB = new Vector2();
			if (i == (shape.Count - 1 )) pointB = new Vector2(shape[0].x, shape[0].z);
			else pointB = new Vector2(shape[i+1].x, shape[i+1].z);

			if (TestLinesIntersection(pointA, pointB, lineSegmentA, lineSegmentB)) {
				if (TestSegmentIntersection(pointA, pointB, lineSegmentA, lineSegmentB)) {
					intersectionPoints.Add(LineIntersectionPoint(pointA, pointB, lineSegmentA, lineSegmentB));
					intersectionIndexes.Add(i);
				}
			}
		}

		float minDist = Mathf.Infinity;
		Vector2 closestIntersection = intersectionPoints[0];
		int closestIntersectionIndex = intersectionIndexes[0];
		for (int i=0; i< intersectionPoints.Count; i++) {
			float dist = Vector2.Distance(intersectionPoints[i], lineB);
			if (dist < minDist) {
				minDist = dist;
				closestIntersection = intersectionPoints[i];
				closestIntersectionIndex = intersectionIndexes[i];
			}
		}

		//debug
		/*
		Vector3 newPoint = new Vector3(closestIntersection.x, shape[0].y, closestIntersection.y);
		GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
		test.transform.position = newPoint;
		test.transform.localScale = test.transform.localScale * 0.1f;
		*/

		List<Vector3> result = new List<Vector3>();
		for (int i=0; i<shape.Count; i++) {

			if (i == closestIntersectionIndex) {
				Vector3 newPoint = new Vector3(closestIntersection.x, shape[0].y, closestIntersection.y);
				result.Add(newPoint);
			}
			else {
				result.Add(shape[i]);
			}
		}

		for (int i=0; i<closestIntersectionIndex; i++) {
			Vector3 movedVertex = result[0];
			result.RemoveAt(0);
			result.Add(movedVertex);
		}

		return result;
	}

	public static void SetWarpShapeCenterLinePoint() {
		if (surforgeSettings != null) {
			if (surforgeSettings.warpShape != null) {
				if (surforgeSettings.warpShape.Count > 2) {
					Vector2 pointMiddle = new Vector2(surforgeSettings.warpShape[0].x, surforgeSettings.warpShape[0].z);
					Vector2 pointA = new Vector2(surforgeSettings.warpShape[1].x, surforgeSettings.warpShape[1].z);
					Vector2 pointB = new Vector2(surforgeSettings.warpShape[surforgeSettings.warpShape.Count-1].x, surforgeSettings.warpShape[surforgeSettings.warpShape.Count-1].z);

					Vector2 vectorA = pointA - pointMiddle;
					Vector2 vectorB = pointB - pointMiddle;
					vectorA = new Vector2(-vectorA.y, vectorA.x);
					vectorB = new Vector2(-vectorB.y, vectorB.x) * -1.0f;

					Vector2 vPointA = pointMiddle + vectorA.normalized;
					Vector2 vPointB = pointMiddle + vectorB.normalized;
					Vector2 resultPoint = new Vector2((vPointA.x + vPointB.x) * 0.5f, (vPointA.y + vPointB.y) * 0.5f);
					Vector2 resultVector = resultPoint - pointMiddle;
					resultPoint = pointMiddle + resultVector.normalized * 1.5f;
					surforgeSettings.warpShapeCenterLinePoint = new Vector3(resultPoint.x, surforgeSettings.warpShape[0].y, resultPoint.y);
				}
			}
		}
	}





	//---clipper---- 
	/*
	//[MenuItem("Window/" + "ClipTest")]
	public static void ClipTest() {
		if (Selection.gameObjects.Length == 2) {
			PolyLassoObject pObjA = (PolyLassoObject)Selection.gameObjects[0].GetComponent<PolyLassoObject>();
			PolyLassoObject pObjB = (PolyLassoObject)Selection.gameObjects[1].GetComponent<PolyLassoObject>();

			if ((pObjA != null) && (pObjB != null)) {
				List<IntPoint> pathA = new List<IntPoint>();
				List<IntPoint> pathB = new List<IntPoint>();
				for (int i=0; i < pObjA.shape.Count; i++) {
					IntPoint p = Vector3toIntPoint(pObjA.shape[i]);
					pathA.Add(p);
				}
				for (int i=0; i < pObjB.shape.Count; i++) {
					IntPoint p = Vector3toIntPoint(pObjB.shape[i]);
					pathB.Add(p);
				}

				Clipper clipper = new Clipper(Clipper.ioStrictlySimple | Clipper.ioPreserveCollinear);
				clipper.AddPath(pathA, PolyType.ptSubject, true);
				clipper.AddPath(pathB, PolyType.ptClip, true);

				List<List<IntPoint>> solution = new List<List<IntPoint>>();
				clipper.Execute(ClipType.ctUnion, solution);

				for(int i=0; i< solution.Count; i++) {
					List<Vector3> solutionShape = new List<Vector3>();
					for (int s=0; s< solution[i].Count; s++) {
						Vector3 point = IntPointToVector(solution[i][s], pObjA.shape[0].y);
						solutionShape.Add(point);
					}
					surforgeSettings.warpShape = solutionShape;
				}
			}



		}
	}


	//[MenuItem("Window/" + "ClipOffset")]
	public static void ClipOffset() {
		if (Selection.gameObjects.Length == 1) {
			PolyLassoObject pObj = (PolyLassoObject)Selection.gameObjects[0].GetComponent<PolyLassoObject>();
			
			if (pObj != null) {
				Vector3[] shape = ClipperOffsetShape(pObj.shape.ToArray(), -5.0f);

				List<Vector3> shapeList = new List<Vector3>();
				for (int i=0; i<shape.Length; i++) {
					shapeList.Add(shape[i]);
				}

				surforgeSettings.warpShape = shapeList;
			}
			
			
			
		}
	}


	public static Vector3[] ClipperOffsetShape(Vector3[] shape, float offset) {
		Vector3[] result = null;
		if (shape.Length > 0) {
			float height = shape[0].y;
			List<List<IntPoint>> solution = new List<List<IntPoint>>();
			ClipperOffset co =  new ClipperOffset();
			co.AddPath(ShapeToClipperPath(shape), JoinType.jtSquare, EndType.etClosedPolygon);
			co.Execute(ref solution, (double)(offset * 100000));
	

			if (solution.Count > 0) {
				int hightshLengthIndex = 0;
				int hightstLength = 0;

				for (int i=0; i< solution.Count; i++) {
					if (solution[i].Count > hightstLength) {
						hightshLengthIndex = i;
						hightstLength = solution[i].Count;
					}
				}

				result = ClipperPathToShape(solution[hightshLengthIndex], height).ToArray();
			}

			
		}
		return result;
	}


	static List<IntPoint> ShapeToClipperPath(Vector3[] shape) {
		List<IntPoint> result = new List<IntPoint>();
		for (int i=0; i < shape.Length; i++) {
			IntPoint p = Vector3toIntPoint(shape[i]);
			result.Add(p);
		}
		return result;
	}

	static List<Vector3> ClipperPathToShape(List<IntPoint> path, float height) {
		List<Vector3> result = new List<Vector3>();
		for (int i=0; i < path.Count; i++) {
			Vector3 v = IntPointToVector(path[i], height);
			result.Add(v);
		}
		return result;
	}


	static Vector3 IntPointToVector(IntPoint point, float height) {
		return new Vector3(point.X * 0.00001f, height, point.Y * 0.00001f);
	}

	static IntPoint Vector3toIntPoint(Vector3 vector) {
		return new IntPoint((int)(vector.x * 100000), (int)(vector.z * 100000));
	}
	*/








}

