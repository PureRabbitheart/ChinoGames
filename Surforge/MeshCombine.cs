using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshCombine {

	public static void CombineMeshes(GameObject obj) {

		GameObject oldMeshClone = null;

		MeshFilter targetFilter = obj.GetComponent<MeshFilter>();
		if (targetFilter == null) targetFilter = obj.AddComponent<MeshFilter>();
		else { 
			//copy self mesh to use in combine
			oldMeshClone = new GameObject();
			oldMeshClone.transform.parent = obj.transform;
			MeshFilter oldMeshCloneFilter = oldMeshClone.AddComponent<MeshFilter>();
			Mesh oldMeshCopy = (Mesh)Mesh.Instantiate(targetFilter.sharedMesh);
			oldMeshCloneFilter.mesh = oldMeshCopy;
			oldMeshClone.AddComponent<MeshRenderer>();
		}

		MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
		if (meshRenderer == null) meshRenderer = obj.AddComponent<MeshRenderer>();

		MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int index = 0;
		
		int matIndex = -1;
		
		for (int i = 0; i < meshFilters.Length; i++)
		{
			if (meshFilters[i].sharedMesh == null) continue;
			if (meshFilters[i].GetComponent<Renderer>().enabled == false)
			{
				continue;
			}
			else if (matIndex == -1)
			{
				matIndex = i;
			}
			if (meshFilters[i].Equals(targetFilter)) continue;
			
			
			combine[index].mesh = meshFilters[i].sharedMesh;
			
			combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].GetComponent<Renderer>().enabled = false;
		}

		if (oldMeshClone != null) oldMeshClone.GetComponent<Renderer>().enabled = false;



		if (targetFilter.sharedMesh == null) targetFilter.sharedMesh = new Mesh(); 
		targetFilter.sharedMesh.CombineMeshes(combine);

		Mesh meshCopy = (Mesh)Mesh.Instantiate(targetFilter.sharedMesh);

		targetFilter.mesh = meshCopy;

		targetFilter.GetComponent<Renderer>().material = meshFilters[matIndex].GetComponent<Renderer>().sharedMaterial;



		
	}
}