#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using kawetofe.randomPrefabPlacer;

namespace kawetofe.randomPrefabPlacer
{



    [CustomEditor(typeof(RandomPrefabPlacer))]
    public class RandomObjectPlacerEditor : Editor
    {
        RandomPrefabPlacer placer;
        bool multiplePlacementObjects = true;
        bool showStandardButtons = true;
       
        
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Random Prefab Placer v1.1");
            GUILayout.Label("for help visit");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("www.kawetofe.com"))
            {
                Application.OpenURL("http://www.kawetofe.com/wordpress/random-prefab-placer/");
            }
            if (GUILayout.Button(" rate it \u2730\u2730\u2730\u2730\u2730"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/random-prefab-placer-104765");
            }
            EditorGUILayout.EndHorizontal();
           
            EditorGUILayout.Space();
          
            placer = (RandomPrefabPlacer)target;

            placer.editMode = EditorGUILayout.Toggle("Draw Mode", placer.editMode);

           

            if (!placer.editMode)
            {
                if(placer.rppBrush != null)
                EditorGUILayout.HelpBox("- > Use 'Control+LMB' to change placement object and center of placement circle \n - > switch to draw mode to draw with the selected prefab brush", MessageType.Info);
               
                EditorGUILayout.BeginHorizontal();
                placer.rppBrush = (RPPBrush)EditorGUILayout.ObjectField(placer.rppBrush, typeof(RPPBrush), true);
                EditorGUILayout.EndHorizontal();
                if (placer.rppBrush != null)
                {
                    showStandardButtons = true;
                   
                    placer.prefabs = new System.Collections.Generic.List<PlacementPrefab>();
                    foreach (PlacementPrefab p in placer.rppBrush.prefabs)
                    {
                        placer.prefabs.Add(p);
                    }

                    DrawDefaultInspector();

                } else
                {
                    EditorGUILayout.HelpBox("Create a RPPBrush with the 'Create' option at the Project View, drag it to the RPPBrush box to edit the variables and draw with it",MessageType.Warning);
                    showStandardButtons = false;
                }

            

            } else if(placer.editMode)
            {
                EditorGUILayout.HelpBox(" - > use left mouse button (LMB) to place objects \n - > use CTRL+LMB to delete objects \n - > use RMB to change placement object",MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                placer.rppBrush = (RPPBrush)EditorGUILayout.ObjectField(placer.rppBrush, typeof(RPPBrush), true);
                EditorGUILayout.EndHorizontal();
               

                EditorGUILayout.BeginHorizontal();
                placer.objectToPlacePrefabs = (Transform)EditorGUILayout.ObjectField(placer.objectToPlacePrefabs, typeof(Transform), true);
                multiplePlacementObjects = EditorGUILayout.Toggle("Draw on multiple objects", multiplePlacementObjects);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                float sliderValue = EditorGUILayout.Slider(new GUIContent("Brush Size"), placer.placementRadius, .1f, 80f);
                placer.placementRadius = sliderValue;                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                int numberOfObjects =(int) EditorGUILayout.Slider(new GUIContent("Number of objects"), placer.objectsToPlace, 1, 500);
                placer.objectsToPlace = numberOfObjects;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                float remAlpha = EditorGUILayout.Slider(new GUIContent("Alpha for removing"), placer.removalAlpha, 1f, 100f);
                placer.removalAlpha = remAlpha;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                
                if (placer.rppBrush != null)
                {
                  
                    placer.prefabs.Clear();
                    foreach(PlacementPrefab p in placer.rppBrush.prefabs)
                    {
                        placer.prefabs.Add(p);
                    }
                   
                } else
                {
                    EditorGUILayout.LabelField("Please insert RPPBrush to start");
                }
               
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            if (showStandardButtons)
            {
                EditorGUILayout.BeginHorizontal();
            
                if (!placer.editMode)
                {

                    if (GUILayout.Button("Place Objects"))
                    {
                        placer.PlaceObjects();
                    }
                }

                if (GUILayout.Button("Remove all placed Objects"))
                {
                    placer.RemovePlacedObjects();
                }

               

                if (GUILayout.Button("Lock placed objects"))
                {
                    if (EditorUtility.DisplayDialog("Make objects permanent?", "This will make all placed objects on this surface permanent.\n The brush tool will no longer have access to them, are you sure you want to do that?", "Yes", "No"))
                    {
                        placer.MakeObjectsPermanent();
                    }
                }
          
            EditorGUILayout.EndHorizontal();
            }


        }

        /// <summary>
        /// Pick active placement object to place prefabs to.
        /// </summary>
        public void PickObjectToPlacePrefabs()
        {
            // Shoot a ray from the mouse position into the world
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;
            // Shoot this ray. check in a distance of 10000.
            if (Physics.Raycast(worldRay, out hitInfo, 10000))
            {
                if (!hitInfo.collider.gameObject.name.Contains("(RPPClone)"))
                {
                    placer.objectToPlacePrefabs = hitInfo.collider.transform;
                    Vector3 worldPoint = hitInfo.point;
                    placer.placementCircleCenter.transform.position = worldPoint;
                }
            }
        }

       

        /// <summary>
        /// OnSceneGUI 
        /// </summary>
        public void OnSceneGUI()
        {
           

            if (placer != null)
            {
                // if circle center GameObject is missing
                if (placer.placementCircleCenter == null || placer.placementCircleCenter == placer.objectToPlacePrefabs)
                {
                    Transform circleCenter = (Transform)Instantiate(new GameObject(), placer.objectToPlacePrefabs.position, Quaternion.identity).transform;
                    circleCenter.name = "RPPCircleCenter";
                    circleCenter.SetParent(placer.transform);
                    placer.placementCircleCenter = circleCenter;
                }

                // if placer is not in brush mode
                if (!placer.editMode)

                {

                    if (Event.current.control)
                    {
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    }
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control)
                    {
                       
                        PickObjectToPlacePrefabs();
                    }
                }

                // if placer is in edit mode
                if (placer.editMode)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                   
                    
                    
                    // Event on mouse move
                    if (Event.current.type == EventType.MouseMove)
                    {
                        if (multiplePlacementObjects)
                        {
                            PickObjectToPlacePrefabs();
                        }

                        // Shoot a ray from the mouse position into the world
                        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit hitInfo;
                        // Shoot this ray. check in a distance of 10000.
                        if (Physics.Raycast(worldRay, out hitInfo, 10000))
                        {
                           
                            if (hitInfo.collider.transform == placer.objectToPlacePrefabs.transform)
                            {

                                Vector3 worldPoint = hitInfo.point;
                                placer.placementCircleCenter.transform.position = worldPoint;


                            }
                        }
                    }

                    // Place Objects on Mouse Button down
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt && !Event.current.control)
                    {
                       
                        placer.PlaceObjects();
                    }
                    // Remove Objects inside Circle
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control)
                    {
                       
                        placer.RemovePlacedObjectsInCircle();
                    }

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                    {
                        PickObjectToPlacePrefabs();
                    }


                  
                    SceneView.RepaintAll();
                } // End if __editMode
            }
        }

        /// <summary>
        /// Menu Item
        /// </summary>
        [MenuItem("GameObject/RandomPrefabPlacer/RPPBrush")]
        public static void InstantiateRPPBrushTool()
        {
            GameObject rppBrush = (GameObject)Resources.Load("kawetofe/randomPrefabPlacer/RPPBrush");
            GameObject cloneObj = Instantiate(rppBrush, Vector3.zero, Quaternion.identity);
            cloneObj.name = cloneObj.name.Replace("(Clone)", "");
        }

    }

}
#endif
