#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class PolyLassoObject : MonoBehaviour {

	public SurforgeSettings surforgeSettings;
	public bool deleting;
	public bool isChild;

	public List<Vector3P> shapePointPairs;

	public List<Vector3> shape;
	public float bevelAmount;
	public int bevelSteps;
	public float[] offsets;
	public float[] heights;

	//decals
	public List<DecalSet> decalSets = new List<DecalSet>();

	public List<bool> inheritMatGroup = new List<bool>();
	public List<bool> scatterOnShapeVerts = new List<bool>();
	public List<bool> trim = new List<bool>();
	public List<bool> perpTrim = new List<bool>();
	public List<bool> fitDecals = new List<bool>();

	public List<float> trimOffset = new List<float>();
	public List<float> decalOffset = new List<float>();
	public List<float> decalOffsetRandom = new List<float>();
	public List<float> decalGap = new List<float>();
	public List<float> decalGapRandom = new List<float>();
	public List<float> decalSize = new List<float>();
	public List<float> decalSizeRandom = new List<float>();
	public List<float> decalRotation = new List<float>();
	public List<float> decalRotationRandom = new List<float>();

	public List<GameObject> decals;

	public bool noise;
	public float shapeSubdiv;
	
	public Vector2 noise1Amount;
	public float noise1Frequency;
	
	public Vector2 noise2Amount;
	public float noise2Frequency;

	public int materialID;

	public Vector2[] deformedBorder;

	public GameObject[] seamlessInstances;

	public bool isFloater;

	public bool isTube;
	public bool isOpen;
	public float thickness;
	public Vector2[] section; 

	public bool isAdaptive;
	public float adaptiveStep;
	public float[] lengthOffsets0;
	public float[] lengthOffsets1;
	public float[] lengthOffsets2;
	
	public float[] heightOffsets0;
	public float[] heightOffsets1;
	public float[] heightOffsets2;
	
	public int repeatSize;
	
	public int[] lengthOffsetOrder;
	public int[] heightOffsetOrder;

	public bool edgeWiseOffset;
	public bool lengthWiseOffset;
	public float offsetMinEdge;

	public PolyLassoCorner corner;

	public List<GameObject> details;

	public float[] childProfileVerticalOffsets; 
	public float[] childProfileDepthOffsets;
	public int[] childProfileHorisontalOffsets;
	public int[] childProfileMatGroups;
	public PolyLassoRelativeShape[] childProfileShapes;

	public PolyLassoProfile[] followerProfiles; 
	public float[] followerProfileVerticalOffsets; 
	public float[] followerProfileDepthOffsets; 
	public int[] followerProfileMatGroups; 

	public Texture2D cutoff; 
	public Vector2 cutoffTiling; 

	public Texture2D bumpMap; 
	public float bumpMapIntensity; 
	public Vector2 bumpMapTiling; 
	public Texture2D aoMap;
	public float aoMapIntensity;

	public bool randomUvOffset;

	public int stoneType;

	public bool allowIntersections; 
	public bool overlapIntersections; 
	public float overlapAmount; 

	public float[] overlapAmounts;
	public Vector3[] overlapIntersectedShape;

	public bool usedForOverlapping; 
	public bool overlapStartInvert; 
	public bool curveUVs; 

	Quaternion oldRotation;
	Vector3 oldPosition;
	Vector3 oldScale;

	Vector3 oldRootScale;


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
						if (surforgeSettings.polyLassoObjects != null) {
							if (!surforgeSettings.polyLassoObjects.Contains(this)) {
								surforgeSettings.polyLassoObjects.Add(this);
							}
						}

						if (!surforgeSettings.isPolygonLassoActive) {
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

			if ((oldPosition != this.transform.position) || (oldRotation != this.transform.rotation) || (oldScale != this.transform.localScale)
			    || (surforgeSettings.root.transform.localScale != oldRootScale) ) {
				//Debug.Log ("transformChanged!");

				oldPosition = this.transform.position;
				oldRotation = this.transform.rotation;
				oldScale = this.transform.localScale;
				oldRootScale = surforgeSettings.root.transform.localScale;

				SetPolyLassoObjectShapePointPairs(this);
			}

		}
	}

	void OnDestroy(){
		if (surforgeSettings) {
			if (!deleting) surforgeSettings.seamlessNeedUpdate = true;
			surforgeSettings.polyLassoObjects.Remove(this);
		}
	}

	void UpdateSeamlessInstanceTransformRealtime(GameObject inst, int i, float offsetX, float offsetZ, GameObject source) {
		if (i == 0) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z); 
		if (i == 1) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z); 
		
		if (i == 2) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 3) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x + offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ); 
		
		if (i == 4) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 5) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x - offsetX, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ);
		
		if (i == 6) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z + offsetZ); 
		if (i == 7) inst.transform.position = new Vector3(source.transform.localPosition.x * surforgeSettings.root.transform.localScale.x, source.transform.localPosition.y * surforgeSettings.root.transform.localScale.y, source.transform.localPosition.z * surforgeSettings.root.transform.localScale.z - offsetZ);
		
		inst.transform.localScale = new Vector3(source.transform.localScale.x * surforgeSettings.root.transform.localScale.x,
		                                        source.transform.localScale.y * surforgeSettings.root.transform.localScale.y,
		                                        source.transform.localScale.z * surforgeSettings.root.transform.localScale.z);

		inst.transform.rotation = source.transform.localRotation;
	}



	public void SetPolyLassoObjectShapePointPairs(PolyLassoObject pObj) {
		pObj.shapePointPairs = new List<Vector3P>();	
		List<Vector3> shape = PolyLassoObjectToWorldShape(pObj);

		float offsetZ = Mathf.Abs(surforgeSettings.textureBorders.minZ - surforgeSettings.textureBorders.maxZ) * surforgeSettings.root.transform.localScale.z;
		float offsetX = Mathf.Abs(surforgeSettings.textureBorders.minX - surforgeSettings.textureBorders.maxX) * surforgeSettings.root.transform.localScale.x;

		
		for (int i=0; i< shape.Count; i++) {
			Vector3P pair = new Vector3P();
			if (i < (shape.Count -1) ) {
				pair.a = shape[i];
				pair.b = shape[i+1];
			}
			else {
				pair.a = shape[i];
				pair.b = shape[0];
			}
			pObj.shapePointPairs.Add(pair);

			if (surforgeSettings.seamless) {
				Vector3P pairSeamless1 = new Vector3P();
				pairSeamless1.a = new Vector3(pair.a.x + offsetX, pair.a.y, pair.a.z);
				pairSeamless1.b = new Vector3(pair.b.x + offsetX, pair.b.y, pair.b.z);
				pObj.shapePointPairs.Add(pairSeamless1);

				Vector3P pairSeamless2 = new Vector3P();
				pairSeamless2.a = new Vector3(pair.a.x - offsetX, pair.a.y, pair.a.z);
				pairSeamless2.b = new Vector3(pair.b.x - offsetX, pair.b.y, pair.b.z);
				pObj.shapePointPairs.Add(pairSeamless2);

				Vector3P pairSeamless3 = new Vector3P();
				pairSeamless3.a = new Vector3(pair.a.x, pair.a.y, pair.a.z + offsetZ);
				pairSeamless3.b = new Vector3(pair.b.x, pair.b.y, pair.b.z + offsetZ);
				pObj.shapePointPairs.Add(pairSeamless3);

				Vector3P pairSeamless4 = new Vector3P();
				pairSeamless4.a = new Vector3(pair.a.x, pair.a.y, pair.a.z - offsetZ);
				pairSeamless4.b = new Vector3(pair.b.x, pair.b.y, pair.b.z - offsetZ);
				pObj.shapePointPairs.Add(pairSeamless4);

				Vector3P pairSeamless5 = new Vector3P();
				pairSeamless5.a = new Vector3(pair.a.x + offsetX, pair.a.y, pair.a.z + offsetZ);
				pairSeamless5.b = new Vector3(pair.b.x + offsetX, pair.b.y, pair.b.z + offsetZ);
				pObj.shapePointPairs.Add(pairSeamless5);

				Vector3P pairSeamless6 = new Vector3P();
				pairSeamless6.a = new Vector3(pair.a.x + offsetX, pair.a.y, pair.a.z - offsetZ);
				pairSeamless6.b = new Vector3(pair.b.x + offsetX, pair.b.y, pair.b.z - offsetZ);
				pObj.shapePointPairs.Add(pairSeamless6);

				Vector3P pairSeamless7 = new Vector3P();
				pairSeamless7.a = new Vector3(pair.a.x - offsetX, pair.a.y, pair.a.z + offsetZ);
				pairSeamless7.b = new Vector3(pair.b.x - offsetX, pair.b.y, pair.b.z + offsetZ);
				pObj.shapePointPairs.Add(pairSeamless7);

				Vector3P pairSeamless8 = new Vector3P();
				pairSeamless8.a = new Vector3(pair.a.x - offsetX, pair.a.y, pair.a.z - offsetZ);
				pairSeamless8.b = new Vector3(pair.b.x - offsetX, pair.b.y, pair.b.z - offsetZ);
				pObj.shapePointPairs.Add(pairSeamless8);
			}
		}
	}

	List<Vector3> PolyLassoObjectToWorldShape(PolyLassoObject polyLassoObject) {
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

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles, Vector3 localScale) {
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = new Vector3(dir.x * localScale.x, dir.y * localScale.y, dir.z * localScale.z);
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	
}
#endif