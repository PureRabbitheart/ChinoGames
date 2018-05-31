using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SurforgeRig))]
public class SurforgeInterfaceRig : Editor {

	float buttonSize = 0.4f;

	string[] rigOptions = new string[8] {"voxel", "on cover only", "link", "cover", "ivy", "glue", "void", "edges" };
	int rigIndex;
	int oldRigIndex = 999;

	SurforgeRig targetRig;
	Transform v;
	Transform parent;
	SurforgeCluster cluster;

	void OnEnable() {
		Setup ();
		CheckRemovedVoxels();
	}

	void Setup() {
		targetRig = (SurforgeRig)target;
		v = targetRig.gameObject.transform;
		parent = v.transform.parent;
		cluster = (SurforgeCluster)parent.GetComponent<SurforgeCluster>();
	}

	public override void OnInspectorGUI () {
		if (oldRigIndex != rigIndex) SetIndexByBlockType(targetRig.blockType);
		rigIndex = EditorGUILayout.Popup("Block type:", rigIndex, rigOptions);
		if (oldRigIndex != rigIndex) {
			SetBlockTypeByIndex();
			if (rigIndex != 2) targetRig.linkType = "";
		}

		if (rigIndex == 0) { //default voxel
			if (targetRig.transform.position != Vector3.zero) {
				bool setOriginButton = GUILayout.Button("Set Origin");
				if (setOriginButton) SetOrigin();
			}
		}

		if ((rigIndex == 2) || (rigIndex == 0) || (rigIndex == 1))  { //link, voxel, on cover only
			targetRig.linkType = EditorGUILayout.TextField("Link type: ", targetRig.linkType);

			bool rotateY = GUILayout.Button("Rotate Y");
			if (rotateY) RotateVoxelAroundY();
		}


	}


	void SetOrigin() {
		if (cluster.transform.position != Vector3.zero) ClusterToZeroConstantChildrenPosition();
		cluster.transform.position = Vector3.zero - targetRig.transform.position;
		ClusterToZeroConstantChildrenPosition();
	}


	void ClusterToZeroConstantChildrenPosition() {
		foreach (Transform v in cluster.voxels) {
			v.parent =  null;
		}
		cluster.model.transform.parent =  null;

		cluster.transform.position = Vector3.zero;

		foreach (Transform v in cluster.voxels) {
			v.parent =  cluster.transform;
		}
		cluster.model.transform.parent =  cluster.transform;

	}

	void RotateVoxelAroundY() {
		bool rotated = false;

		if ( (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == 0) || 
		     (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == 360) || 
		     (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == -360)  ) {

			if (!rotated) {
				rotated = true;
				targetRig.transform.localEulerAngles = new Vector3 (0, 90.0f, 0);
			}
		}
		if ( (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == 90) || 
		     (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == -270) ) {

			if (!rotated) {
				rotated = true;
				targetRig.transform.localEulerAngles = new Vector3 (0, 180.0f, 0);
			}
		}
		if ( (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == 180) || 
		     (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == -180) ) {

			if (!rotated) {
				rotated = true;
				targetRig.transform.localEulerAngles = new Vector3 (0, -90.0f, 0);
			}
		}
		if ( (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == -90) || 
		     (Mathf.RoundToInt(targetRig.transform.localEulerAngles.y) == 270) ) {

			if (!rotated) {
				rotated = true;
				targetRig.transform.localEulerAngles = new Vector3 (0, 0, 0);
			}
		}
	}


	void DrawVoxelAddHandles() {
		for (int i = 0; i < 6; i++) {
			switch (i) {
			case 0:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(0, 1.0f, 0));
				break;
			case 1:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(0, -1.0f, 0));
				break;
			case 2:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(1.0f, 0, 0));
				break;
			case 3:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(-1.0f, 0, 0));
				break;
			case 4:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(0, 0, 1.0f));
				break;
			case 5:
				CheckAndDrawVoxelAddHandle(targetRig.transform.position + new Vector3(0, 0, -1.0f));
				break;
			}

		}
	}

	void CheckAndDrawVoxelAddHandle(Vector3 handlePos) {
		bool matchPosition = false;
		foreach (Transform v in cluster.voxels) {
			if (v.position == handlePos) {
				matchPosition = true;
				break;
			}
		}
		if (!matchPosition) DrawVoxelAddHandle(handlePos);
	}

	void DrawVoxelAddHandle(Vector3 handlePos) {
		Handles.color = new Color(0, 1, 0, 1);
		bool addVoxelButton = Handles.Button(handlePos, Quaternion.identity, 0.2f, 0.2f, Handles.SphereCap);
		if (addVoxelButton) {
			AddVoxel(handlePos);
		}
	}

	void AddVoxel(Vector3 voxelPos) {
		GameObject voxelObj = new GameObject();
		voxelObj.name = "vox";
		voxelObj.transform.position = voxelPos;
		voxelObj.transform.parent = cluster.transform;

		SurforgeRig newVoxelRig = voxelObj.AddComponent<SurforgeRig>();
		newVoxelRig.blockType = targetRig.blockType;
		newVoxelRig.linkType = targetRig.linkType;
		newVoxelRig.transform.rotation = targetRig.transform.rotation;

		cluster.voxels.Add(voxelObj.transform);
		Selection.activeObject = voxelObj;
	}



	void OnSceneGUI () {
		//Tools.current = Tool.None;

		int activeClusterIndex = 0;

		for (int i=0; i < cluster.voxels.Count; i++) {
			if (cluster.voxels[i] != null) {
			SurforgeRig rig = (SurforgeRig)cluster.voxels[i].GetComponent<SurforgeRig>();

				if (rig == targetRig) {
					activeClusterIndex = i;
				}
		

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

		SurforgeRig activeRig = (SurforgeRig)cluster.voxels[activeClusterIndex].GetComponent<SurforgeRig>();
		DrawCubeLines(activeRig.transform.position);
		DrawVoxelAddHandles();
		
	
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


	void SetBlockTypeByIndex() {
		switch(rigIndex) {
		case 0:
			targetRig.blockType = 0;
			break;
		case 1:
			targetRig.blockType = 10;
			break;
		case 2: 
			targetRig.blockType = 2;
			break;
		case 3: 
			targetRig.blockType = 4;
			break;
		case 5:
			targetRig.blockType = 5;
			break;
		case 4:
			targetRig.blockType = 3;
			break;
		case 6:
			targetRig.blockType = 6;
			break;
		case 7:
			targetRig.blockType = 7;
			break;
		}
	}
	
	void SetIndexByBlockType(int blockTypeNum) {
		switch(blockTypeNum) {
		case 0:
			rigIndex = 0;
			break;
		case 10:
			rigIndex = 1;
			break;
		case 2: 
			rigIndex = 2;
			break;
		case 3:
			rigIndex = 4;
			break;
		case 4:
			rigIndex = 3;
			break;
		case 5:
			rigIndex = 5;
			break;
		case 6:
			rigIndex = 6;
			break;
		case 7:
			rigIndex = 7;
			break;
		}
		oldRigIndex = rigIndex;
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


	void DrawCubeLines(Vector3 voxelPos) {
		Handles.color = new Color(1, 1, 1, 1);
		Handles.DrawLine(voxelPos + new Vector3(0.5f, 0.5f, 0.5f), voxelPos + new Vector3(-0.5f, 0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, 0.5f, -0.5f), voxelPos + new Vector3(-0.5f, 0.5f, -0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, -0.5f, 0.5f), voxelPos + new Vector3(-0.5f, -0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, -0.5f, -0.5f), voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
		Handles.DrawLine(voxelPos + new Vector3(-0.5f, 0.5f, 0.5f), voxelPos + new Vector3(-0.5f, -0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(-0.5f, 0.5f, -0.5f), voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, 0.5f, 0.5f), voxelPos + new Vector3(0.5f, -0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, 0.5f, -0.5f), voxelPos + new Vector3(0.5f, -0.5f, -0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, 0.5f, -0.5f), voxelPos + new Vector3(0.5f, 0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(-0.5f, 0.5f, -0.5f), voxelPos + new Vector3(-0.5f, 0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(0.5f, -0.5f, -0.5f), voxelPos + new Vector3(0.5f, -0.5f, 0.5f));
		Handles.DrawLine(voxelPos + new Vector3(-0.5f, -0.5f, -0.5f), voxelPos + new Vector3(-0.5f, -0.5f, 0.5f));
	}



}
