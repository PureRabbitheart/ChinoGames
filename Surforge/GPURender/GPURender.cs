#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class GPURender : MonoBehaviour {

	SurforgeSettings surforgeSettings;

	Vector3 cameraCenter;

	public Camera aoCaptureCameraPrefab;
	public Camera normalsCaptureCameraPrefab;
	public Camera normalsCombineCameraPrefab;
	public Camera edgesCaptureCameraPrefab;
	public Camera masksCaptureCameraPrefab;
	public Camera masks2CaptureCameraPrefab;
	public Camera combineCameraPrefab;
	public Camera emissionCaptureCameraPrefab;
	public Camera glowCaptureCameraPrefab;
	public Camera addGlowCameraPrefab;
	public Camera labelsCaptureCameraPrefab;
	public Camera labelsAlphaCaptureCameraPrefab;
	public Camera heightmapCaptureCameraPrefab;

	public Camera combineLabelsAlphaCameraPrefab;

	public Camera combineNoiseCameraPrefab;

	public Camera floatersMaskCaptureCameraPrefab;
	public Camera ssaoFloatersMaskCaptureCameraPrefab;

	public Camera ssaoCaptureCameraPrefab;


	public Shader aoCapture;
	public Shader aoCaptureFloater;
	public Shader normalmapCapture;
	public Shader masksCapture;
	public Shader emissionCapture;
	public Shader labelsCapture;
	public Shader labelsAlphaCapture;
	public Shader heightmapCapture;

	public Shader floatersMaskCapture;

	public Shader ssaoCapture;
	public Shader ssaoCaptureFloater;


	Camera aoCaptureCamera;
	Camera normalsCaptureCamera;
	Camera normalsCombineCamera;
	Camera edgesCaptureCamera;
	Camera masksCaptureCamera;
	Camera masks2CaptureCamera;
	Camera combineCamera;
	Camera emissionCaptureCamera;
	Camera glowCaptureCamera;
	Camera addGlowCamera;
	Camera labelsCaptureCamera;
	Camera labelsAlphaCaptureCamera;
	Camera heightmapCaptureCamera;

	Camera combineLabelsAlphaCamera;

	Camera combineNoiseCamera;

	Camera floatersMaskCaptureCamera;

	Camera bucketsCollectCamera;
	


	//public GameObject aoLightsGeosphere;
	public GameObject aoLightsPrefab;
	public GameObject aoLightsPrefabDx11;

	public GameObject aoLightsPrefabLinear;
	public GameObject aoLightsPrefabDx11Linear; 

	GameObject aoLights;
	

	public Shader edgesCapturePost;

	int supersamplingMode;


	//----store render settings information---
	UnityEngine.Rendering.AmbientMode usedAmbientMode;
	Color ambientSkyColor;

	Light[] lights;
	bool[] lightsWasEnabled;

	Vector3 rootScale;
	
	int renderingRes;
	bool stopRender = false;

	// -- ssao settings --
	bool ssao;
	int ssaoRes;
	int ssaoSamplesMult = 1;
	int ssaoAA;
	

	void Start() { 

		HidePlaceToolPreviewObjects();

		rootScale = surforgeSettings.root.transform.localScale;
		surforgeSettings.root.transform.localScale = Vector3.one;

		UpdateSeamlessInstances();

		StoreCurrentRenderSettings();


		ssaoRes = renderingRes;
		if (ssaoRes == 2048) ssaoSamplesMult = 2;
		if (ssaoRes == 4096) ssaoSamplesMult = 4;

		ssaoAA = 1;
		if (supersamplingMode == 1) ssaoAA = 2;
		if (supersamplingMode == 2) ssaoAA = 4;


		// using result maps for temporary to save memory
		//--------------------------
		// 	 normalMap            ->  normalMap
		// aoTexture            ->  emissionMap
		// aoFloatersTexture    ->  objectMasks
		// floatersMaskTexture  ->  objectMasks2
		// heightmapTexture     ->  labelsMap
		//   edgesTexture         ->  edgesTexture
		//   aoEdgesDirtDepth     ->  aoEdgesDirtDepth

		// emissionTexture      ->  labelsAlpha
		//   glowTexture          ->  glowTexture

		// emissionMap          ->  emissionMap
		// objectMasks          ->  objectMasks 
		// objectMasks2         ->  objectMasks2
		// labelsMap            ->  labelsMap 
		// labelsAlpha          ->  labelsAlpha


		if (!stopRender) RenderNormalMap();

		if (surforgeSettings.modelBakedNormal) {
			if (!stopRender) CombineNormalMaps();
		}

		if (ssao) {
			if (!stopRender) RenderSSAO();
			if (!stopRender) RenderSSAOFloaters();
			if (!stopRender) RenderSSAOFloatersMask();
		}
		else {
			if (!stopRender) RenderAO();
			if (!stopRender) RenderAOFloaters();
			if (!stopRender) RenderFloatersMask();
		}

		if (!stopRender) RenderHeightmap();
		if (!stopRender) RenderEdges();  

		if (!stopRender) CombineAoEdgesDirtDepth();


		if (!stopRender) RenderEmission(); 
		if (!stopRender) RenderEmissionGlow(); 

		if (!stopRender) AddGlow();

		if (!stopRender) CombineNoise();


		if (!stopRender) RenderObjectMasks(); 
		if (!stopRender) RenderObjectMasks2(); 
		if (!stopRender) RenderLabels();
		if (!stopRender) RenderLabelsAlpha();

		if (!stopRender) CombineLabelsAlpha();


		FinishRender();

		EditorUtility.ClearProgressBar();
	}



	void ProgressBar(string taskName, int taskNumber, int x, int z, bool showBucketProgress) {
		if ( (renderingRes == 1024) && (supersamplingMode == 0) ) return;

		int totalTasks = 14;

		string aa = "AA: 1x";
		if (supersamplingMode == 1) aa = "AA: 2x";
		if (supersamplingMode == 2) aa = "AA: 4x";

		string ao = "";
		if (ssao) ao = "AO: SSAO " + 1000 * ssaoSamplesMult + " samples"; 
		else ao = "AO: Shadows";

		if ((supersamplingMode != 0) && showBucketProgress) {
			int bucketsInRow = 2;
			if (supersamplingMode == 2) bucketsInRow = 4;

			int currentBucket = x * bucketsInRow + (z+1);
			int totalBuckets = 4;
			if (supersamplingMode == 2) totalBuckets = 16;
			taskName = taskName + " " + currentBucket + " of " +  totalBuckets;
		}


		if( EditorUtility.DisplayCancelableProgressBar("Rendering maps, res: " + renderingRes + ", " + aa + ", " + ao,
		                                               "Rendering " + taskName, 
		                                               (1.0f / (float)totalTasks) * taskNumber ) ) {
			stopRender = true;
		}
	}



	void UpdateSeamlessInstances() {
		for (int i=0; i < surforgeSettings.polyLassoObjects.Count; i++) {
			surforgeSettings.polyLassoObjects[i].Update();
		}

		foreach (Transform child in surforgeSettings.root.transform) {
			GameObject obj = child.gameObject;
			if (obj) {
				PlaceMesh placeMesh = (PlaceMesh)obj.GetComponent<PlaceMesh>();
				if (placeMesh) {
					placeMesh.Update();
				}
			}
		}
	}


	void HidePlaceToolPreviewObjects() {
		if (surforgeSettings.placeToolPreview != null) surforgeSettings.placeToolPreview.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmX != null) surforgeSettings.placeToolPreviewSymmX.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmZ != null) surforgeSettings.placeToolPreviewSymmZ.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmXZ != null) surforgeSettings.placeToolPreviewSymmXZ.SetActive(false);
		
		if (surforgeSettings.placeToolPreviewSymmDiagonal != null) surforgeSettings.placeToolPreviewSymmDiagonal.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmDiagonalX != null) surforgeSettings.placeToolPreviewSymmDiagonalX.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmDiagonalZ != null) surforgeSettings.placeToolPreviewSymmDiagonalZ.SetActive(false);
		if (surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) surforgeSettings.placeToolPreviewSymmDiagonalXZ.SetActive(false);	
	}


	void StoreCurrentRenderSettings() {
		usedAmbientMode = RenderSettings.ambientMode;
		ambientSkyColor = RenderSettings.ambientSkyColor;
	}


	void DisableAllLights() {
		lights = FindObjectsOfType(typeof(Light)) as Light[];
		lightsWasEnabled = new bool[lights.Length];
		for(int i=0; i < lights.Length; i++) {
			lightsWasEnabled[i] = lights[i].enabled;
			lights[i].enabled = false;
		}
	}

	void EnableLights() {
		for(int i=0; i < lights.Length; i++) {
			lights[i].enabled = lightsWasEnabled[i];
		}
	}






	void RenderNormalMap() {
		ProgressBar("Normal map", 1, 0, 0, false);
		
		if (supersamplingMode == 0) {
			normalsCaptureCamera = (Camera)Instantiate(normalsCaptureCameraPrefab);
			normalsCaptureCamera.transform.position = cameraCenter;
			SetCameraClipPlanes(normalsCaptureCamera);
			
			normalsCaptureCamera.targetTexture = surforgeSettings.renderTexture;
			
			normalsCaptureCamera.SetReplacementShader(normalmapCapture, "Surforge"); 
			
			normalsCaptureCamera.Render();
		}
		else {
			int bucketsCount = 2;
			if (supersamplingMode == 2) bucketsCount = 4;
			
			for (int x=0; x< bucketsCount; x++) {
				for (int z=0; z< bucketsCount; z++) {
					
					if (stopRender) break;
					
					int bucketX = x;
					int bucketZ = z;
					
					ProgressBar("Normal map", 1, bucketX, bucketZ, true);
					
					if (normalsCaptureCamera != null) {
						normalsCaptureCamera.targetTexture = null;
						RenderTexture.active = null;
						DestroyImmediate(normalsCaptureCamera.gameObject);
					}
					
					normalsCaptureCamera = (Camera)Instantiate(normalsCaptureCameraPrefab);
					
					normalsCaptureCamera.transform.position = cameraCenter;
					
					SetCameraClipPlanes(normalsCaptureCamera);
					
					Matrix4x4 m = GetCameraProjectionMatrixWithPadding(normalsCaptureCamera, x, z, 1);
					normalsCaptureCamera.projectionMatrix = m;
					
					normalsCaptureCamera.targetTexture = surforgeSettings.renderTexture2;
					
					normalsCaptureCamera.SetReplacementShader(normalmapCapture, "Surforge"); 

					
					if (supersamplingMode == 1) {
						CameraPost bucketDownscalePost = (CameraPost)normalsCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						
						bucketDownscalePost.mat.SetFloat("_bucketX", (float)bucketX); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)bucketZ); 
					}
					if (supersamplingMode == 2) {
						CameraPost bucketDownscalePost = (CameraPost)normalsCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						
						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 0)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 0)); 
						
						bucketDownscalePost = (CameraPost)normalsCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						
						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 1)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 1)); 
					}
					
					normalsCaptureCamera.Render();
					
					
					
					bool isFirstBucket = false;
					if ((x == 0) && (z == 0)) isFirstBucket = true;
					
					CollectBuckets(renderingRes, renderingRes, false, isFirstBucket);
				}
				if (stopRender) break;
			}
			
		}
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.normalMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.normalMap.Apply();
		
		normalsCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (normalsCaptureCamera != null) DestroyImmediate(normalsCaptureCamera.gameObject);
		
		if (bucketsCollectCamera != null) DestroyImmediate(bucketsCollectCamera.gameObject);
	}



	void CombineNormalMaps() {
		normalsCombineCamera = (Camera)Instantiate(normalsCombineCameraPrefab);
		normalsCombineCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(normalsCombineCamera);
		
		normalsCombineCamera.targetTexture = surforgeSettings.renderTexture;

		CameraPost normalsCombinePost = (CameraPost)normalsCombineCamera.gameObject.AddComponent<CameraPost>();
		normalsCombinePost.mat = new Material(surforgeSettings.normalsBakedAddPost);
		normalsCombinePost.mat.SetTexture("_BumpMap", surforgeSettings.modelBakedNormal);
		normalsCombinePost.mat.SetTexture("_SurforgeNormal", surforgeSettings.normalMap);
		
		normalsCombineCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.normalMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.normalMap.Apply();
		
		
		normalsCombineCamera.targetTexture = null;
		RenderTexture.active = null;
		if (normalsCombineCamera != null) DestroyImmediate(normalsCombineCamera.gameObject);
	}




	//-----ssao--------

	void RenderSSAO() {
		ProgressBar("AO", 2, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;

		if (supersamplingMode == 0) {

			aoCaptureCamera = (Camera)Instantiate(ssaoCaptureCameraPrefab);
			aoCaptureCamera.transform.position = cameraCenter;

			aoCaptureCamera.depthTextureMode = DepthTextureMode.DepthNormals;

			Matrix4x4 m = GetCameraProjectionMatrixWithPadding(aoCaptureCamera, 0, 0, 1.25f);
			aoCaptureCamera.projectionMatrix = m;

			CameraPost cameraPost = (CameraPost)aoCaptureCamera.GetComponent<CameraPost> ();
			if (cameraPost != null) {
				cameraPost.mat.mainTexture = null;
			}

			SurforgeSSAOSettings ssaoSettings = (SurforgeSSAOSettings)aoCaptureCamera.gameObject.GetComponent<SurforgeSSAOSettings>();
			if (ssaoSettings) {
				ssaoSettings.res = ssaoRes;
				ssaoSettings.samplesMult = ssaoSamplesMult;
				ssaoSettings.matrix = m;
				ssaoSettings.aa = ssaoAA;
			}

			aoCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
			aoCaptureCamera.RenderWithShader(ssaoCapture, "Base"); 
		}

		else {
			int bucketsCount = 2;
			if (supersamplingMode == 2) bucketsCount = 4;

			
			for (int x=0; x< bucketsCount; x++) {
				for (int z=0; z< bucketsCount; z++) {

					if (stopRender) break;

					int bucketX = x;
					int bucketZ = z;

					ProgressBar("AO", 2, bucketX, bucketZ, true);
					
					if (aoCaptureCamera != null) {
						aoCaptureCamera.targetTexture = null;
						RenderTexture.active = null;
						DestroyImmediate(aoCaptureCamera.gameObject);
					}
					
					aoCaptureCamera = (Camera)Instantiate(ssaoCaptureCameraPrefab);
					
					aoCaptureCamera.transform.position = cameraCenter;
					
					aoCaptureCamera.depthTextureMode = DepthTextureMode.DepthNormals;
					
					Matrix4x4 m = GetCameraProjectionMatrixWithPadding(aoCaptureCamera, x, z, 1.25f);
					aoCaptureCamera.projectionMatrix = m;

					CameraPost cameraPost = (CameraPost)aoCaptureCamera.GetComponent<CameraPost> ();
					if (cameraPost != null) {
						cameraPost.mat.mainTexture = null;
					}
					
					SurforgeSSAOSettings ssaoSettings = (SurforgeSSAOSettings)aoCaptureCamera.gameObject.GetComponent<SurforgeSSAOSettings>();
					if (ssaoSettings) {
						ssaoSettings.res = ssaoRes;
						ssaoSettings.samplesMult = ssaoSamplesMult;
						ssaoSettings.matrix = m;
						ssaoSettings.aa = ssaoAA;
					}
									
					aoCaptureCamera.targetTexture = surforgeSettings.renderTexture2;


					if (supersamplingMode == 1) {
						CameraPost bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)bucketX); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)bucketZ); 
					}

			
					if (supersamplingMode == 2) {
						CameraPost bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 0)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 0)); 
						
						bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 1)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 1)); 
					}

					
					aoCaptureCamera.RenderWithShader(ssaoCapture, "Base"); 

					
					bool isFirstBucket = false;
					if ((x == 0) && (z == 0)) isFirstBucket = true;

					CollectBuckets(renderingRes, renderingRes, false, isFirstBucket);
				}
				if (stopRender) break;
			}
			
		}


		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.emissionMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.emissionMap.Apply();
		
		EnableLights();
		RevertRenderSettings();
		
		aoCaptureCamera.targetTexture = null;
		RenderTexture.active = null;

		if (aoCaptureCamera != null) DestroyImmediate(aoCaptureCamera.gameObject);

		if (bucketsCollectCamera != null) DestroyImmediate(bucketsCollectCamera.gameObject);
	}


	void RenderSSAOFloaters() {
		ProgressBar("AO floaters", 3, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;

		if (supersamplingMode == 0) {

			aoCaptureCamera = (Camera)Instantiate(ssaoCaptureCameraPrefab);
			aoCaptureCamera.transform.position = cameraCenter;

			aoCaptureCamera.depthTextureMode = DepthTextureMode.DepthNormals;

			Matrix4x4 m = GetCameraProjectionMatrixWithPadding(aoCaptureCamera, 0, 0, 1.25f);
			aoCaptureCamera.projectionMatrix = m;

			CameraPost cameraPost = (CameraPost)aoCaptureCamera.GetComponent<CameraPost> ();
			if (cameraPost != null) {
				cameraPost.mat.mainTexture = null;
			}
		
			SurforgeSSAOSettings ssaoSettings = (SurforgeSSAOSettings)aoCaptureCamera.gameObject.GetComponent<SurforgeSSAOSettings>();
			if (ssaoSettings) {
				ssaoSettings.res = ssaoRes;
				ssaoSettings.samplesMult = ssaoSamplesMult;
				ssaoSettings.matrix = m;
				ssaoSettings.aa = ssaoAA;
			}

			aoCaptureCamera.targetTexture = surforgeSettings.renderTexture;

			aoCaptureCamera.RenderWithShader(ssaoCaptureFloater, "Floater"); 

		}

		else {
			int bucketsCount = 2;
			if (supersamplingMode == 2) bucketsCount = 4;

			
			for (int x=0; x< bucketsCount; x++) {
				for (int z=0; z< bucketsCount; z++) {

					if (stopRender) break;

					int bucketX = x;
					int bucketZ = z;

					ProgressBar("AO floaters", 3, bucketX, bucketZ, true);
					
					if (aoCaptureCamera != null) {
						aoCaptureCamera.targetTexture = null;
						RenderTexture.active = null;
						DestroyImmediate(aoCaptureCamera.gameObject);
					}
					
					aoCaptureCamera = (Camera)Instantiate(ssaoCaptureCameraPrefab);
					
					aoCaptureCamera.transform.position = cameraCenter;
					
					aoCaptureCamera.depthTextureMode = DepthTextureMode.DepthNormals;
					
					Matrix4x4 m = GetCameraProjectionMatrixWithPadding(aoCaptureCamera, x, z, 1.25f);
					aoCaptureCamera.projectionMatrix = m;

					CameraPost cameraPost = (CameraPost)aoCaptureCamera.GetComponent<CameraPost> ();
					if (cameraPost != null) {
						cameraPost.mat.mainTexture = null;
					}
					
					SurforgeSSAOSettings ssaoSettings = (SurforgeSSAOSettings)aoCaptureCamera.gameObject.GetComponent<SurforgeSSAOSettings>();
					if (ssaoSettings) {
						ssaoSettings.res = ssaoRes;
						ssaoSettings.samplesMult = ssaoSamplesMult;
						ssaoSettings.matrix = m;
						ssaoSettings.aa = ssaoAA;
					}


					aoCaptureCamera.targetTexture = surforgeSettings.renderTexture2;
					
					
					if (supersamplingMode == 1) {
						CameraPost bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)bucketX); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)bucketZ); 
					}
					if (supersamplingMode == 2) {
						CameraPost bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 0)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 0)); 
						
						bucketDownscalePost = (CameraPost)aoCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 1)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 1)); 
					}
					
					aoCaptureCamera.RenderWithShader(ssaoCaptureFloater, "Floater"); 
					
					
					
					bool isFirstBucket = false;
					if ((x == 0) && (z == 0)) isFirstBucket = true;

					CollectBuckets(renderingRes, renderingRes, false, isFirstBucket);
				}
				if (stopRender) break;
			}
			
		}


		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.objectMasks.Apply();

		
		EnableLights();
		RevertRenderSettings();

		aoCaptureCamera.targetTexture = null;
		RenderTexture.active = null;

		if (aoCaptureCamera != null) DestroyImmediate(aoCaptureCamera.gameObject);

		if (bucketsCollectCamera != null) DestroyImmediate(bucketsCollectCamera.gameObject);
	}


	void RenderSSAOFloatersMask() {
		ProgressBar("Floaters mask", 4, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;


		if (supersamplingMode == 0) {
		
			floatersMaskCaptureCamera = (Camera)Instantiate(ssaoFloatersMaskCaptureCameraPrefab);
			floatersMaskCaptureCamera.transform.position = cameraCenter;

			Matrix4x4 m = GetCameraProjectionMatrixWithPadding(floatersMaskCaptureCamera, 0, 0, 1.25f);
			floatersMaskCaptureCamera.projectionMatrix = m;

			CameraPost cameraPost = (CameraPost)floatersMaskCaptureCamera.GetComponent<CameraPost> ();
			if (cameraPost != null) {
				cameraPost.mat.mainTexture = null;
			}

			floatersMaskCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
			floatersMaskCaptureCamera.RenderWithShader(floatersMaskCapture, "Floater"); 

		}

		else {
			int bucketsCount = 2;
			if (supersamplingMode == 2) bucketsCount = 4;
			
			for (int x=0; x< bucketsCount; x++) {
				for (int z=0; z< bucketsCount; z++) {

					if (stopRender) break;

					int bucketX = x;
					int bucketZ = z;

					ProgressBar("Floaters mask", 4, bucketX, bucketZ, true);
					
					if (floatersMaskCaptureCamera != null) {
						floatersMaskCaptureCamera.targetTexture = null;
						RenderTexture.active = null;
						DestroyImmediate(floatersMaskCaptureCamera.gameObject);
					}
					
					floatersMaskCaptureCamera = (Camera)Instantiate(ssaoFloatersMaskCaptureCameraPrefab);
					
					floatersMaskCaptureCamera.transform.position = cameraCenter;

					Matrix4x4 m = GetCameraProjectionMatrixWithPadding(floatersMaskCaptureCamera, x, z, 1.25f);
					floatersMaskCaptureCamera.projectionMatrix = m;

					CameraPost cameraPost = (CameraPost)floatersMaskCaptureCamera.GetComponent<CameraPost> ();
					if (cameraPost != null) {
						cameraPost.mat.mainTexture = null;
					}

					floatersMaskCaptureCamera.targetTexture = surforgeSettings.renderTexture2;
					
					if (supersamplingMode == 1) {
						CameraPost bucketDownscalePost = (CameraPost)floatersMaskCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)bucketX); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)bucketZ); 
					}
					if (supersamplingMode == 2) {
						CameraPost bucketDownscalePost = (CameraPost)floatersMaskCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 0)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 0)); 
						
						bucketDownscalePost = (CameraPost)floatersMaskCaptureCamera.gameObject.AddComponent<CameraPost>();
						bucketDownscalePost.mat = new Material(surforgeSettings.bucketDownscale);
						bucketDownscalePost.mat.mainTexture = null;

						bucketDownscalePost.mat.SetFloat("_bucketX", (float)Get4xSSBucketPos(bucketX, 1)); 
						bucketDownscalePost.mat.SetFloat("_bucketZ", (float)Get4xSSBucketPos(bucketZ, 1)); 
					}
					
					floatersMaskCaptureCamera.RenderWithShader(floatersMaskCapture, "Floater"); 
					
					
					
					bool isFirstBucket = false;
					if ((x == 0) && (z == 0)) isFirstBucket = true;

					CollectBuckets(renderingRes, renderingRes, false, isFirstBucket);
				}
				if (stopRender) break;
			}
			
		}


		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks2.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.objectMasks2.Apply();

		
		EnableLights();
		RevertRenderSettings();

		floatersMaskCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		
		if (floatersMaskCaptureCamera != null) DestroyImmediate(floatersMaskCaptureCamera.gameObject);

		if (bucketsCollectCamera != null) DestroyImmediate(bucketsCollectCamera.gameObject);
	}
	

	//----------------







	
	void RenderAO() {
		ProgressBar("AO", 2, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;

		aoCaptureCamera = (Camera)Instantiate(aoCaptureCameraPrefab);
		aoCaptureCamera.transform.position = cameraCenter;

		SetCameraClipPlanes(aoCaptureCamera);

		InstantiateAOLights();

		aoCaptureCamera.targetTexture = surforgeSettings.renderTexture;

		aoCaptureCamera.RenderWithShader(aoCapture, "Base");  

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.emissionMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);

		surforgeSettings.emissionMap.Apply();


		EnableLights();
		RevertRenderSettings();

		aoCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		
		if (aoLights != null) DestroyImmediate(aoLights);
		if (aoCaptureCamera != null) DestroyImmediate(aoCaptureCamera.gameObject);
	}

	void RenderAOFloaters() {
		ProgressBar("AO floaters", 3, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;
		
		aoCaptureCamera = (Camera)Instantiate(aoCaptureCameraPrefab);
		aoCaptureCamera.transform.position = cameraCenter;

		SetCameraClipPlanes(aoCaptureCamera);
		
		InstantiateAOLights();

		aoCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		aoCaptureCamera.RenderWithShader(aoCaptureFloater, "Floater"); 

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);

		surforgeSettings.objectMasks.Apply();

		EnableLights();
		RevertRenderSettings();

		aoCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		
		if (aoLights != null) DestroyImmediate(aoLights);
		if (aoCaptureCamera != null) DestroyImmediate(aoCaptureCamera.gameObject);
	}


	void RenderFloatersMask() {
		ProgressBar("Floaters mask", 4, 0, 0, false);

		DisableAllLights();
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientSkyColor = Color.black;
		
		floatersMaskCaptureCamera = (Camera)Instantiate(floatersMaskCaptureCameraPrefab);
		floatersMaskCaptureCamera.transform.position = cameraCenter;
		
		SetCameraClipPlanes(floatersMaskCaptureCamera);

		floatersMaskCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		floatersMaskCaptureCamera.RenderWithShader(floatersMaskCapture, "Floater"); 

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks2.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.objectMasks2.Apply();
		
		EnableLights();
		RevertRenderSettings();

		floatersMaskCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		
		if (floatersMaskCaptureCamera != null) DestroyImmediate(floatersMaskCaptureCamera.gameObject);
	}
	




	void RenderHeightmap() {
		ProgressBar("Height map", 5, 0, 0, false);
		
		heightmapCaptureCamera = (Camera)Instantiate(heightmapCaptureCameraPrefab);
		heightmapCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(heightmapCaptureCamera);
		
		heightmapCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		Shader.SetGlobalFloat("_MaxGeometryHeight", GetMaximimHeight());
		
		heightmapCaptureCamera.SetReplacementShader(heightmapCapture, "Surforge"); 
		
		heightmapCaptureCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.labelsMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.labelsMap.Apply();
		
		
		heightmapCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (heightmapCaptureCamera != null) DestroyImmediate(heightmapCaptureCamera.gameObject);
	}




	void RenderEdges() {
		ProgressBar("Edges map", 6, 0, 0, false);
		
		edgesCaptureCamera = (Camera)Instantiate(edgesCaptureCameraPrefab);
		edgesCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(edgesCaptureCamera);
		
		edgesCaptureCamera.targetTexture = surforgeSettings.edgesRenderTexture;
		
		edgesCaptureCamera.SetReplacementShader(normalmapCapture, "Surforge"); 
		
		CameraPost[] cameraPosts = edgesCaptureCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
			if (cameraPosts[i].mat.shader == edgesCapturePost) {
				cameraPosts[i].mat.SetTexture("_NormalMap", surforgeSettings.normalMap); 
			}
		}
		
		edgesCaptureCamera.Render();
		
		RenderTexture.active = surforgeSettings.edgesRenderTexture;
		surforgeSettings.edgesTexture.ReadPixels(new Rect(0, 0, surforgeSettings.edgesRenderTexture.width, surforgeSettings.edgesRenderTexture.height), 0, 0, false);
		surforgeSettings.edgesTexture.Apply();
		
		
		edgesCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (edgesCaptureCamera != null) DestroyImmediate(edgesCaptureCamera.gameObject);
	}




	void CombineAoEdgesDirtDepth() {
		ProgressBar("Combine maps", 7, 0, 0, false);
		
		combineCamera = (Camera)Instantiate(combineCameraPrefab);
		combineCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(combineCamera);
		
		combineCamera.targetTexture = surforgeSettings.renderTexture;
		
		CameraPost combinePost = (CameraPost)combineCamera.gameObject.GetComponent<CameraPost>();
		
		combinePost.mat.SetTexture("_Ao", surforgeSettings.emissionMap);
		combinePost.mat.SetTexture("_AoFloaters", surforgeSettings.objectMasks);
		combinePost.mat.SetTexture("_Edges", surforgeSettings.edgesTexture);
		combinePost.mat.SetTexture("_Depth", surforgeSettings.labelsMap);
		combinePost.mat.SetTexture("_ModelBakedAO", surforgeSettings.modelBakedAO);
		combinePost.mat.SetTexture("_FloatersMask", surforgeSettings.objectMasks2);
		combinePost.mat.SetTexture("_ModelBakedEdgeMap", surforgeSettings.modelBakedEdgeMap);
		
		
		combineCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.aoEdgesDirtDepth.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.aoEdgesDirtDepth.Apply();
		
		combineCamera.targetTexture = null;
		RenderTexture.active = null;
		if (combineCamera != null) DestroyImmediate(combineCamera.gameObject);
	}
	

	void RenderEmission() {
		ProgressBar("Emission map", 8, 0, 0, false);
		
		emissionCaptureCamera = (Camera)Instantiate(emissionCaptureCameraPrefab);
		emissionCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(emissionCaptureCamera);

		CameraPost cameraPost = (CameraPost)emissionCaptureCamera.GetComponent<CameraPost> ();
		if (cameraPost != null) {
			cameraPost.mat.mainTexture = null;
		}

		emissionCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		emissionCaptureCamera.SetReplacementShader(emissionCapture, "SurforgeEmissionId"); 
		emissionCaptureCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.labelsAlpha.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.labelsAlpha.Apply();
		
		emissionCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (emissionCaptureCamera != null) DestroyImmediate(emissionCaptureCamera.gameObject);
	}


	void RenderEmissionGlow() {
		ProgressBar("Emission glow map", 9, 0, 0, false);
		
		glowCaptureCamera = (Camera)Instantiate(glowCaptureCameraPrefab);
		glowCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(glowCaptureCamera);

		CameraPost[] cameraPosts = glowCaptureCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
		}

		glowCaptureCamera.targetTexture = surforgeSettings.glowRenderTexture;
		
		glowCaptureCamera.SetReplacementShader(emissionCapture, "SurforgeEmissionId"); 
		glowCaptureCamera.Render();
		
		RenderTexture.active = surforgeSettings.glowRenderTexture; 
		surforgeSettings.glowTexture.ReadPixels(new Rect(0, 0, surforgeSettings.glowRenderTexture.width, surforgeSettings.glowRenderTexture.height), 0, 0, false);
		surforgeSettings.glowTexture.Apply();
		
		glowCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (glowCaptureCamera != null) DestroyImmediate(glowCaptureCamera.gameObject);
	}



	void AddGlow() {
		ProgressBar("Add emission glow", 10, 0, 0, false);
		
		addGlowCamera = (Camera)Instantiate(addGlowCameraPrefab);
		addGlowCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(addGlowCamera);
		
		addGlowCamera.targetTexture = surforgeSettings.renderTexture;
		
		CameraPost addGlowPost = (CameraPost)addGlowCamera.gameObject.GetComponent<CameraPost>();
		
		addGlowPost.mat.SetTexture("_Emission", surforgeSettings.labelsAlpha);
		addGlowPost.mat.SetTexture("_Glow", surforgeSettings.glowTexture);
		
		addGlowCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.emissionMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.emissionMap.Apply();
		
		
		addGlowCamera.targetTexture = null;
		RenderTexture.active = null;
		if (addGlowCamera != null) DestroyImmediate(addGlowCamera.gameObject);
	}
	

	// new noise
	void CombineNoise() {
		
		combineNoiseCamera = (Camera)Instantiate(combineNoiseCameraPrefab);
		combineNoiseCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(combineNoiseCamera);
		
		combineNoiseCamera.targetTexture = surforgeSettings.renderTexture;
		
		CameraPost combineNoiseCameraPost = (CameraPost)combineNoiseCamera.gameObject.GetComponent<CameraPost>();
		
		combineNoiseCameraPost.mat.SetTexture("_EmissionMap", surforgeSettings.emissionMap);
		combineNoiseCameraPost.mat.SetTexture("_Noise", surforgeSettings.rgbaNoise);
		
		combineNoiseCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.emissionMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.emissionMap.Apply();
		
		
		combineNoiseCamera.targetTexture = null;
		RenderTexture.active = null;
		if (combineNoiseCamera != null) DestroyImmediate(combineNoiseCamera.gameObject); 
	}


	void RenderObjectMasks() {
		ProgressBar("Objects ID maps", 11, 0, 0, false);

		masksCaptureCamera = (Camera)Instantiate(masksCaptureCameraPrefab);
		masksCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(masksCaptureCamera);

		CameraPost[] cameraPosts = masksCaptureCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
		}

		masksCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		masksCaptureCamera.SetReplacementShader(masksCapture, "SurforgeMaterialId"); 
		masksCaptureCamera.Render();

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.objectMasks.Apply();

		masksCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (masksCaptureCamera != null) DestroyImmediate(masksCaptureCamera.gameObject);
	}

	void RenderObjectMasks2() {
		ProgressBar("Objects ID 2 maps", 12, 0, 0, false);

		masks2CaptureCamera = (Camera)Instantiate(masks2CaptureCameraPrefab);
		masks2CaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(masks2CaptureCamera);

		CameraPost[] cameraPosts = masks2CaptureCamera.gameObject.GetComponents<CameraPost>();
		for (int i=0; i < cameraPosts.Length; i++) {
			cameraPosts[i].mat.mainTexture = null;
		}

		masks2CaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		masks2CaptureCamera.SetReplacementShader(masksCapture, "SurforgeMaterialId"); 
		masks2CaptureCamera.Render();

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.objectMasks2.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.objectMasks2.Apply();

		masks2CaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (masks2CaptureCamera != null) DestroyImmediate(masks2CaptureCamera.gameObject); 
	}
	



	void RenderLabels() {
		ProgressBar("Labels", 13, 0, 0, false);
		
		labelsCaptureCamera = (Camera)Instantiate(labelsCaptureCameraPrefab);
		labelsCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(labelsCaptureCamera);

		labelsCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		labelsCaptureCamera.SetReplacementShader(labelsCapture, "SurforgeLabel");  
		labelsCaptureCamera.Render();

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.labelsMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.labelsMap.Apply();


		labelsCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (labelsCaptureCamera != null) DestroyImmediate(labelsCaptureCamera.gameObject);
	}


	void RenderLabelsAlpha() {
		ProgressBar("Labels alpha", 14, 0, 0, false);
		
		labelsAlphaCaptureCamera = (Camera)Instantiate(labelsAlphaCaptureCameraPrefab);
		labelsAlphaCaptureCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(labelsAlphaCaptureCamera);

		labelsAlphaCaptureCamera.targetTexture = surforgeSettings.renderTexture;
		
		labelsAlphaCaptureCamera.SetReplacementShader(labelsAlphaCapture, "SurforgeLabel");  
		labelsAlphaCaptureCamera.Render();

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.labelsAlpha.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.labelsAlpha.Apply();

		labelsAlphaCaptureCamera.targetTexture = null;
		RenderTexture.active = null;
		if (labelsAlphaCaptureCamera != null) DestroyImmediate(labelsAlphaCaptureCamera.gameObject);
	}



	void CombineLabelsAlpha() {
		//ProgressBar("Add emission glow", 10, 0, 0, false);
		
		combineLabelsAlphaCamera = (Camera)Instantiate(combineLabelsAlphaCameraPrefab);
		combineLabelsAlphaCamera.transform.position = cameraCenter;
		SetCameraClipPlanes(combineLabelsAlphaCamera);
		
		combineLabelsAlphaCamera.targetTexture = surforgeSettings.renderTexture;
		
		CameraPost combineLabelsAlphaCameraPost = (CameraPost)combineLabelsAlphaCamera.gameObject.GetComponent<CameraPost>();
		
		combineLabelsAlphaCameraPost.mat.SetTexture("_Labels", surforgeSettings.labelsMap);
		combineLabelsAlphaCameraPost.mat.SetTexture("_LabelsAlpha", surforgeSettings.labelsAlpha);
		
		combineLabelsAlphaCamera.Render();
		
		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.labelsMap.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.labelsMap.Apply();
		
		
		combineLabelsAlphaCamera.targetTexture = null;
		RenderTexture.active = null;
		if (combineLabelsAlphaCamera != null) DestroyImmediate(combineLabelsAlphaCamera.gameObject); 
	}




	float GetMaximimHeight() {
		float result = 0;

		Transform[] transforms = surforgeSettings.root.GetComponentsInChildren<Transform>(); 
		for (int i=0; i < transforms.Length; i++) {
			GameObject obj = transforms[i].gameObject;
			if (obj) {
				Renderer objRenderer = (Renderer)obj.GetComponent<Renderer>();
				if (objRenderer) {
					bool isFloater = false;
					for (int s=0; s < surforgeSettings.floaterMaterialGroups.Length; s++) {
						if (objRenderer.sharedMaterial.shader == surforgeSettings.floaterMaterialGroups[s]) {
							isFloater = true;
							break;
						}
					}
					if (!isFloater) {
						float height = objRenderer.bounds.max.y;
						if (height > result) result = height;
					}
				}
			}
		}
		return result;
	}


	void SetCameraClipPlanes(Camera camera) {
		camera.farClipPlane = cameraCenter.y + 1000.0f;
		camera.nearClipPlane = cameraCenter.y - 300.0f;
	}


	void InstantiateAOLights() {
		if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11) {
			if (PlayerSettings.colorSpace == ColorSpace.Linear) {
				aoLights = (GameObject)Instantiate(aoLightsPrefabDx11Linear);
			}
			else {
				aoLights = (GameObject)Instantiate(aoLightsPrefabDx11);
			}
		}
		else {
			if (PlayerSettings.colorSpace == ColorSpace.Linear) {
				aoLights = (GameObject)Instantiate(aoLightsPrefabLinear);
			}
			else {
				aoLights = (GameObject)Instantiate(aoLightsPrefab);
			}
		}
	}




	void FinishRender() {
		surforgeSettings.root.transform.localScale = rootScale;

		surforgeSettings.texturePreviewUpdated = true;

		ShowPlaceToolReviewObjects();

		DestroyImmediate(this.gameObject);
	}

	void ShowPlaceToolReviewObjects() {
		if (surforgeSettings.placeToolPreview != null) surforgeSettings.placeToolPreview.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmX != null) surforgeSettings.placeToolPreviewSymmX.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmZ != null) surforgeSettings.placeToolPreviewSymmZ.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmXZ != null) surforgeSettings.placeToolPreviewSymmXZ.SetActive(true);
		
		if (surforgeSettings.placeToolPreviewSymmDiagonal != null) surforgeSettings.placeToolPreviewSymmDiagonal.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmDiagonalX != null) surforgeSettings.placeToolPreviewSymmDiagonalX.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmDiagonalZ != null) surforgeSettings.placeToolPreviewSymmDiagonalZ.SetActive(true);
		if (surforgeSettings.placeToolPreviewSymmDiagonalXZ != null) surforgeSettings.placeToolPreviewSymmDiagonalXZ.SetActive(true);	
	}


	void RevertRenderSettings() {
		RenderSettings.ambientMode = usedAmbientMode;
		RenderSettings.ambientSkyColor = ambientSkyColor;
	}
	

	public void SetCameraCenter(Vector3 center) {
		cameraCenter = center;
	}


	public void SetSurforgeSettings(SurforgeSettings settings) {
		surforgeSettings = settings;
	}

	public void SetSupersamplingMode(int ssMode) {
		supersamplingMode = ssMode;
	}
	
	public void SetRenderingRes(int res) {
		renderingRes = res;
	}

	public void SetAoMode(int mode) {
		if (mode == 0) ssao = false;
		else ssao = true;
	}


	void CollectBuckets(int width, int height, bool isARGB32, bool isFirstBucket) {

		if (bucketsCollectCamera != null) DestroyImmediate(bucketsCollectCamera.gameObject);

		GameObject bucketsCollect = new GameObject();
		bucketsCollectCamera = (Camera)bucketsCollect.AddComponent<Camera>();

		bucketsCollectCamera.targetTexture = surforgeSettings.renderTexture;

		CameraPost bucketsCollectPost = (CameraPost)bucketsCollectCamera.gameObject.AddComponent<CameraPost>();
		bucketsCollectPost.mat = new Material(surforgeSettings.bucketsCollect);

		bucketsCollectPost.mat.SetTexture("_Result", surforgeSettings.bucketTexture);
		bucketsCollectPost.mat.SetTexture("_Bucket", surforgeSettings.renderTexture2);

		if (isFirstBucket) bucketsCollectPost.mat.SetFloat("_IsFirstBucket", 1.0f);
		else bucketsCollectPost.mat.SetFloat("_IsFirstBucket", 0);
		
		bucketsCollectCamera.Render();

		RenderTexture.active = surforgeSettings.renderTexture;
		surforgeSettings.bucketTexture.ReadPixels(new Rect(0, 0, surforgeSettings.renderTexture.width, surforgeSettings.renderTexture.height), 0, 0, false);
		surforgeSettings.bucketTexture.Apply();


		bucketsCollectCamera.targetTexture = null;
		RenderTexture.active = null;
	}
	


	int Get4xSSBucketPos(int bucket, int level) {
		int result = 0;

		if (level == 0) {
			if (bucket == 0) result = 0;
			if (bucket == 1) result = 1;
			if (bucket == 2) result = 0;
			if (bucket == 3) result = 1;
		}
		else {
			if (bucket == 0) result = 0;
			if (bucket == 1) result = 0;
			if (bucket == 2) result = 1;
			if (bucket == 3) result = 1;
		}

		return result;
	}



	Matrix4x4 GetCameraProjectionMatrixWithPadding(Camera cam, int bucketX, int bucketZ, float padding) {
		
		float left = 0; 
		float right = 0;
		float top = 0;
		float bottom = 0;
		
		float bucketOffset = 0;

		float centerX = 0;
		float centerZ = 0;

		if (supersamplingMode == 0) {
			bucketOffset = 2.0f * cam.nearClipPlane * Mathf.Tan(1.0f * 0.5f * Mathf.Deg2Rad);
		}

		if (supersamplingMode == 1) {
			bucketOffset = 2.0f * cam.nearClipPlane * Mathf.Tan(1.0f * 0.5f * Mathf.Deg2Rad) * 0.5f;

			centerX = bucketOffset * -1.0f + bucketOffset * bucketX + bucketOffset * 0.5f;
			centerZ = bucketOffset * -1.0f + bucketOffset * bucketZ + bucketOffset * 0.5f;
		}


		if (supersamplingMode == 2) {
			bucketOffset = 2.0f * cam.nearClipPlane * Mathf.Tan(1.0f * 0.5f * Mathf.Deg2Rad) * 0.25f;

			centerX = bucketOffset * -2.0f + bucketOffset * bucketX + bucketOffset * 0.5f;
			centerZ = bucketOffset * -2.0f + bucketOffset * bucketZ + bucketOffset * 0.5f;	
		}

		left = centerX - (bucketOffset * 0.5f) * padding;
		right = centerX + (bucketOffset * 0.5f) * padding;
		top = centerZ + (bucketOffset * 0.5f) * padding;
		bottom = centerZ - (bucketOffset * 0.5f) * padding;

		
		
		float near = cam.nearClipPlane;
		float far = cam.farClipPlane;
		
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;

		return m;
	}




	
	/*
	void InstantiateAOLights() {
		MeshFilter meshFilter = aoLightsGeosphere.GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.sharedMesh;
		
		GameObject aoLights = new GameObject("aoLights");
		
		for (int i=0 ; i< mesh.vertices.Length; i++) {
			GameObject lightObject = new GameObject("aoLight");
			Light light = (Light)lightObject.AddComponent<Light>();
			light.type = LightType.Directional;
			
			Quaternion lightRotation = Quaternion.LookRotation(mesh.vertices[i] - Vector3.zero);
			lightObject.transform.rotation = lightRotation;
			lightObject.transform.parent = aoLights.transform;
			
		}
	}
	*/

}
#endif