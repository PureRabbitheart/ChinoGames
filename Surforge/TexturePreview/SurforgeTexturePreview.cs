#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class SurforgeTexturePreview : MonoBehaviour {

	[HideInInspector]
	public SurforgeSettings surforgeSettings;

	public Transform cameraHolder;

	public SurforgeComposer composer;

	public Camera previewCamera;
	public RenderTexture previewRenderTexture;

	public Transform previewCameraFocus;

	public GameObject mapOnScreenshotPrefab;

	public Shader wireframeShader;

	public GameObject floorPrefab;


	bool skipDestroy;

	void OnDestroy(){
		if (surforgeSettings != null) {
			if (!skipDestroy) {
				surforgeSettings.SkipDestroyTexturePreview();
				DestroyImmediate(surforgeSettings.gameObject);
			}
		}
	}

	public void SkipDestroy() {
		skipDestroy = true;
	}
	
}
#endif