using System.Collections.Generic;
using UnityEngine;

namespace kawetofe.randomPrefabPlacer
{
    [AddComponentMenu("kawetofe_Assets/Random Prefab placer")]
	[SelectionBase]
	public class RandomPrefabPlacer : MonoBehaviour {
        [HideInInspector]
        public RPPBrush rppBrush;
		[Tooltip("Object to place the prefabs on")]	
		public Transform objectToPlacePrefabs = null;
		public List<PlacementPrefab> prefabs = new List<PlacementPrefab> ();
		public Transform placementCircleCenter;
		[Tooltip("Adjust the placement radius")]
		public float placementRadius = 1f;
		public bool showGizmos = true;
        [HideInInspector]
        public bool editMode = false;
		[Tooltip("Define how many objects in total should be randomly placed")]
		[SerializeField]
		public int objectsToPlace = 10;
		[Tooltip("If checked, placement is done in Playmode on Awake()")]
		[SerializeField]
		public bool placeOnAwake = false;
        [SerializeField]
        [HideInInspector]
        public float removalAlpha = 100;

        //public RangeAttribute scaleRange = new RangeAttribute(0.8f,1.2f);



        // Use this for initialization
        void Awake () {
			if (placeOnAwake) {
				PlaceObjects ();
			}
		}
		

        /// <summary>
        /// Places the Random GameObject
        /// </summary>
		public void PlaceObjects(){
			if (objectToPlacePrefabs == null) {
				objectToPlacePrefabs = this.gameObject.transform;
			}
			if (placementCircleCenter == null) {
				placementCircleCenter = transform;
			}
			for (int i = 0; i < objectsToPlace; i++) {
				PlacementPrefab obj = PickRandomObject ();
                if (obj != null)
                {
                    Transform placed = PlaceObjectWithRandomValues(obj);
                    if (placed != null)
                    {
                        placed.SetParent(objectToPlacePrefabs);
                        placed.name = placed.name.Replace("Clone", "RPPClone");
                    }
                }	
				

			}
		}

		/// <summary>
		/// Removes the placed objects with Name "Clone" in it
		/// </summary>
		public void RemovePlacedObjects(){					
			foreach (Transform t in objectToPlacePrefabs.GetComponentsInChildren<Transform> ()) {
				if(t.gameObject.name.Contains ("(RPPClone)"))
					DestroyImmediate (t.gameObject);
			}

		}

        /// <summary>
        /// Removes the placed objects within the displayed circle
        /// </summary>
        public void RemovePlacedObjectsInCircle()
        {
            GameObject[] placedObjects = FindObjectsOfType<GameObject>() as GameObject[];
            foreach(GameObject c in placedObjects)
            {
                if (c.name.Contains("(RPPClone)"))
                {
                    
                    Vector3 diff = placementCircleCenter.position - c.transform.position;
                    float dist = diff.sqrMagnitude;
                    if(dist <= this.placementRadius*this.placementRadius)
                    {
                        if (Random.Range(0f, 100f) < removalAlpha)
                            DestroyImmediate(c);
                    }
                }
            }

        }

        /// <summary>
        /// Picks the random object.
        /// </summary>
        /// <returns>The random object.</returns>
        PlacementPrefab PickRandomObject(){
			if (prefabs.Count > 0) {
					PlacementPrefab picked = null;
					while (picked == null) {
						int randomPrefabInt = Random.Range (0, prefabs.Count);
						float randomPickNumber = Random.Range (0, 1f);
						if(randomPickNumber < prefabs[randomPrefabInt].possibility){
							picked = prefabs [randomPrefabInt];
							Vector3 scale = RandomScale (prefabs[randomPrefabInt]);
							picked.prefab.localScale = scale;
						}
				}
				return picked;
				
			
			} else {
				Debug.Log ("placement Prefabs not set in RandomObjectPlacer of gameObject "+gameObject.name);
				return null;
			}
		}

		/// <summary>
		/// Places the object with random values.
		/// </summary>
		/// <returns>The object with random values.</returns>
		/// <param name="obj">Object.</param>
		Transform PlaceObjectWithRandomValues(PlacementPrefab obj){
			Vector3 pos = Vector3.zero;
			int count = 0;
			while (pos == Vector3.zero && count < 100) {
				pos = RandomPositionOnRegistredSurface ();
				count++;
			}
			Transform newObj = null;
			Quaternion rot = RandomRotation (obj);
			if (pos != Vector3.zero) {
				 newObj = Instantiate (obj.prefab.gameObject, pos, rot).transform;
			} 
			return newObj;
		}

		/// <summary>
		/// Randoms the scale.
		/// </summary>
		/// <returns>The scale.</returns>
		Vector3 RandomScale(PlacementPrefab pprefab){
			float randomScaleFactor = Random.Range (pprefab.scaleMin, pprefab.scaleMax);
			return new Vector3 (randomScaleFactor, randomScaleFactor, randomScaleFactor);
		}


		/// <summary>
		/// Randoms the rotation in y.
		/// </summary>
		/// <returns>The rotation in y.</returns>
		Quaternion RandomRotation(PlacementPrefab pprefab){
            Vector3 rotLock = pprefab.lockedRotations;
            float xrot = 0;
            float yrot = 0;
            float zrot = 0;
            if(rotLock.x == 0)
            {
                xrot = Random.Range(0, 359f);
            }
            if (rotLock.y == 0)
            {
                yrot = Random.Range(0, 359f);
            }
            if (rotLock.z == 0)
            {
                zrot = Random.Range(0, 359f);
            }
            
			Quaternion returnQuaternion = Quaternion.Euler (new Vector3 (xrot, yrot, zrot));
			return returnQuaternion;

		}

		/// <summary>
		/// Randoms the position on registred surface.
		/// </summary>
		/// <returns>The position on registred surface.</returns>
		Vector3 RandomPositionOnRegistredSurface(){
			Vector3 centerPos = placementCircleCenter.position + new Vector3 (0, 10f, 0);
			Vector2 randomCirclePos = Random.insideUnitCircle* placementRadius;
			Vector3 randomPos = centerPos + new Vector3 (randomCirclePos.x, 0 , randomCirclePos.y);
			Vector3 returnPos = Vector3.zero;
			RaycastHit hit;
			if (Physics.Raycast (randomPos, Vector3.down, out hit, centerPos.y + 500f)) {
				if (hit.collider.gameObject == objectToPlacePrefabs.gameObject) {
					returnPos = hit.point;
                    Debug.Log(randomCirclePos.ToString());
				}
			}
			return returnPos;
		}

		void OnDrawGizmosSelected(){
			if (showGizmos) {
				if (placementCircleCenter == null) {
					placementCircleCenter = transform;
				}
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere (placementCircleCenter.position, this.placementRadius);
                Gizmos.color = new Color(255, 0, 0, .3f);
                Gizmos.DrawSphere(placementCircleCenter.position, this.placementRadius/10);
                
			}

		}

        // Make a RPPBrush for the Painter
        public void MakeABrush()
        {
            

            RPPBrush brush = (RPPBrush)ScriptableObjectUtility.CreateAsset<RPPBrush>();
            brush.objectsToPlace = objectsToPlace;
            brush.prefabs = new List<PlacementPrefab>();
            foreach(PlacementPrefab p in this.prefabs)
            {
                brush.prefabs.Add(p);
            }            
            
            
            
        }

       

        public void MakeObjectsPermanent()
        {
            Transform[] components = objectToPlacePrefabs.GetComponentsInChildren<Transform>();
            foreach(Transform t in components)
            {
                if (t.gameObject.name.Contains("(RPPClone)"))
                {
                    t.gameObject.name = t.gameObject.name.Replace("(RPPClone)", "(Clone)");
                }
            }
        }
    }

	/// <summary>
	/// Placement prefab Class.
	/// </summary>

	[System.Serializable]
	public class PlacementPrefab:System.Object{
		[SerializeField]
		[Tooltip("Drag your game prefabs here")]
		public Transform prefab;
		[SerializeField]
		[Tooltip("Set the possibility of appearence 0 - 1")]
		[Range(0,1f)]
		public float possibility = 0.8f;
		[SerializeField]
		[Range(0.1f,10f)]
		public float scaleMin = .8f;
		[SerializeField]
		[Range(0.0f,100f)]
		public float scaleMax = 1.2f;
        [SerializeField]
        [Header("Locked rotations (1=locked, 0=free)")]
        public Vector3 lockedRotations = new Vector3(1, 0, 1);
        private Texture2D thumbnbnail;
	}
}



