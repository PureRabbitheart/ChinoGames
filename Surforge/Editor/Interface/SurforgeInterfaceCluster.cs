using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SurforgeCluster))]
public class SurforgeInterfaceCluster : Editor {

	float buttonSize = 0.4f;
	SurforgeCluster cluster;

	void OnEnable() {
		cluster = (SurforgeCluster)target;
		CheckRemovedVoxels();
	}


	void OnSceneGUI () {
		//Tools.current = Tool.None;


		for (int i=0; i < cluster.voxels.Count; i++) {
			if (cluster.voxels[i] != null) {

				SurforgeRig rig = (SurforgeRig)cluster.voxels[i].GetComponent<SurforgeRig>();
				if (rig.blockType == 0) {
					Handles.color = new Color(0, 0, 0, 1);
				}
				if (rig.blockType == 2) {
					Handles.color = Color.blue + Color.cyan;
				}
				if (rig.blockType == 3) {
					Handles.color = new Color(1, 1, 0, 1);
				}
				if (rig.blockType == 4) {
					Handles.color = new Color(1, 1, 1, 1);
				}
				if (rig.blockType == 5) {
					Handles.color = new Color(1, 0.5f, 1, 1);
				}
				if (rig.blockType == 6) {
					Handles.color = new Color(1, 1, 1, 0.3f);
				}
				if (rig.blockType == 7) {
					Handles.color = new Color(0.5f, 1, 1, 1);
				}
				if (rig.blockType == 10) {
					Handles.color = new Color(0.5f, 0.5f, 1, 1);
				}

				DrawVoxelSelectButtons(i, rig);

			}
		}


	}

	void DrawVoxelSelectButtons(int index, SurforgeRig rig) {
		bool nextButton = false;
		if (rig.blockType == 2) {
			Quaternion arrowRotation = Quaternion.identity;
			arrowRotation.eulerAngles = new Vector3(cluster.voxels[index].transform.localEulerAngles.x,
			                                        cluster.voxels[index].transform.localEulerAngles.y,
			                                        cluster.voxels[index].transform.localEulerAngles.z);
			arrowRotation = arrowRotation * Quaternion.LookRotation(Vector3.right); 
			nextButton = Handles.Button(cluster.voxels[index].transform.position, arrowRotation, buttonSize, buttonSize, Handles.ConeCap);
		}
		
		else {
			if (rig.transform.position == Vector3.zero) {
				Handles.color = Color.red + Color.yellow;
				Quaternion originArrowRotation = Quaternion.identity;
				originArrowRotation = originArrowRotation * Quaternion.LookRotation(Vector3.right); 
				nextButton = Handles.Button(cluster.voxels[index].transform.position, originArrowRotation, buttonSize, buttonSize, Handles.ConeCap);
			}
			else {
				nextButton = Handles.Button(cluster.voxels[index].transform.position, Quaternion.identity, buttonSize, buttonSize, Handles.CubeCap);
			}
		}
		
		if (nextButton) {
			SelectVoxel( cluster.voxels[index]);
		}
	}

	void SelectVoxel(Transform v) {
		Selection.activeTransform = v;
	}


	void CheckRemovedVoxels() {
		List<Transform> existingVoxels = new List<Transform>();
		
		foreach (Transform v in cluster.voxels) {
			if (v != null) existingVoxels.Add(v);
		}
		
		cluster.voxels.Clear();
		
		foreach (Transform v in existingVoxels) {
			cluster.voxels.Add(v);
		}
		
		existingVoxels.Clear();
	}




	/*
	void RerollClusterCheck(SurforgeCluster cluster) {
		if ((Event.current.type == EventType.KeyDown)&&(Event.current.keyCode == KeyCode.Keypad7)) {
		//	if (cluster != null) Extent.RerollCluster(cluster);
		}
	}
	*/




	


}
