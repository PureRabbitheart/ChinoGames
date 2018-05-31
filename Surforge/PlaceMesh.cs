#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class PlaceMesh : MonoBehaviour {

	public Texture icon;
	public Texture iconLite;
	public bool isText;  //if true, right button randomize text instead of flip, and symmetry not flip this asset

	[HideInInspector]
	public GameObject[] seamlessInstances;

	[HideInInspector]
	public SurforgeSettings surforgeSettings; 

	public Texture2D bumpMap; //bumpMap seamless texture for making rocks, etc
	public float bumpMapIntensity = 1; //bumpMap intensity 0 to 2
	public Vector2 bumpMapTiling; //bump and ao maps tiling settings
	public Texture2D aoMap; //aoMap seamless texture
	public float aoMapIntensity; //aoMap intensity -1 to 1. Usually you whant to use something like 1 to 1.1 to get more worn edges
	
	public bool randomUvOffset; //randomize profile texture UV offset

	public PlaceMesh[] shuffleArray; //if any, shuffle with right button
	public bool shuffleScale; //if true, also shuffle scale
	public bool shuffleRotation; //if true, also shuffle rotation and scale x/z flip


	[SerializeField] int instanceID = 0;
	void Awake(){
		if (instanceID != GetInstanceID()) {
			if (instanceID == 0){
				instanceID = GetInstanceID();
			}
			else {
				instanceID = GetInstanceID();
				if (instanceID < 0){
					if (surforgeSettings != null) {
						if (!surforgeSettings.isPlaceToolActive) {
							surforgeSettings.seamlessNeedUpdate = true;
						}
					}
				}
			}
		}
	}


	public void Update() {
		if (surforgeSettings) {
			if (surforgeSettings.seamless) {
				if (this.seamlessInstances != null) {
					if (this.seamlessInstances.Length == 8) {
						float offsetZ = Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * surforgeSettings.root.transform.localScale.z;
						float offsetX = Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * surforgeSettings.root.transform.localScale.x;
						for (int i=0; i<8; i++) {
							if (this.seamlessInstances[i]) {
								UpdateSeamlessInstanceTransformRealtime(this.seamlessInstances[i], i, offsetX, offsetZ, this.gameObject);
							}
						}
					}
				}
			}
		}
	}

	void UpdateSeamlessInstanceTransformRealtime(GameObject inst, int i, float offsetX, float offsetZ, GameObject source) {
		if (i == 0) inst.transform.position = new Vector3(source.transform.position.x + offsetX, source.transform.position.y, source.transform.position.z); 
		if (i == 1) inst.transform.position = new Vector3(source.transform.position.x - offsetX, source.transform.position.y, source.transform.position.z); 
		
		if (i == 2) inst.transform.position = new Vector3(source.transform.position.x + offsetX, source.transform.position.y, source.transform.position.z + offsetZ); 
		if (i == 3) inst.transform.position = new Vector3(source.transform.position.x + offsetX, source.transform.position.y, source.transform.position.z - offsetZ); 
		
		if (i == 4) inst.transform.position = new Vector3(source.transform.position.x - offsetX, source.transform.position.y, source.transform.position.z + offsetZ); 
		if (i == 5) inst.transform.position = new Vector3(source.transform.position.x - offsetX, source.transform.position.y, source.transform.position.z - offsetZ);
		
		if (i == 6) inst.transform.position = new Vector3(source.transform.position.x, source.transform.position.y, source.transform.position.z  + offsetZ); 
		if (i == 7) inst.transform.position = new Vector3(source.transform.position.x, source.transform.position.y, source.transform.position.z - offsetZ);

		if (source.transform.parent == surforgeSettings.root.transform) {
			inst.transform.localScale = new Vector3(source.transform.localScale.x * surforgeSettings.root.transform.localScale.x,
			                                        source.transform.localScale.y * surforgeSettings.root.transform.localScale.y,
			                                        source.transform.localScale.z * surforgeSettings.root.transform.localScale.z);
		}
		else {
			inst.transform.localScale = new Vector3(source.transform.localScale.x,
			                                        source.transform.localScale.y,
			                                        source.transform.localScale.z);
		}
		
		inst.transform.rotation = source.transform.rotation;
	}


	void OnDestroy(){
		if (surforgeSettings != null) {
			if (!surforgeSettings.isPlaceToolActive) {
				surforgeSettings.seamlessNeedUpdate = true;
			}
		}
	}


}
#endif