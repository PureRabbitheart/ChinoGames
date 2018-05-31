using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeRigClusters : MonoBehaviour { 

	static GameObject objectToVoxelize;
	static GameObject clusterObj;
	static SurforgeCluster cluster;

	static Vector3 maxBounds;
	static Vector3 minBounds;
	static Vector3 gridOffset;

	static bool[,,] voxelGrid;
	

	//[MenuItem ("Extent/Rig Clusters/Link Voxels")]
	static void LinkVoxels () {
		GameObject parentObject = Selection.activeGameObject.transform.parent.gameObject;
		parentObject.GetComponent<SurforgeCluster>().voxels.Clear();
		GameObject[] selected = Selection.gameObjects;
		foreach (GameObject obj in selected) {
			parentObject.GetComponent<SurforgeCluster>().voxels.Add(obj.transform);
		}
	}

	//[MenuItem ("Extent/Rig Clusters/Rig Model")]
	static void RigModel() {
		objectToVoxelize = Selection.activeGameObject;

		clusterObj = new GameObject();
		clusterObj.name = "new cluster";
		clusterObj.transform.position = objectToVoxelize.transform.position;

		cluster = (SurforgeCluster)clusterObj.AddComponent<SurforgeCluster>();
		cluster.model = objectToVoxelize;
		cluster.gameObject.name = objectToVoxelize.name;
		objectToVoxelize.AddComponent<BoxCollider>();

		VoxelizeMesh ();

		Selection.activeObject = cluster;
	}
	
	
	static void VoxelizeMesh () {

		SetObjectRenderBounds();
		SetVoxGridOffset();
		CreateVoxelGrid();

		Vector3 initialPosition = objectToVoxelize.transform.position;

		objectToVoxelize.transform.position = new Vector3(objectToVoxelize.transform.position.x + gridOffset.x, 
		                                                  objectToVoxelize.transform.position.y + gridOffset.y, 
		                                                  objectToVoxelize.transform.position.z + gridOffset.z ); 

		FillGridWithGameObjectMeshShell(objectToVoxelize, false);

		objectToVoxelize.transform.position = initialPosition;


		CreateVoxelObjects();

		objectToVoxelize.transform.parent = clusterObj.transform;
	
	}

	static void SetObjectRenderBounds() {

		maxBounds = new Vector3( objectToVoxelize.GetComponent<Renderer>().bounds.max.x,
		                         objectToVoxelize.GetComponent<Renderer>().bounds.max.y,
		                         objectToVoxelize.GetComponent<Renderer>().bounds.max.z );

		minBounds = new Vector3( objectToVoxelize.GetComponent<Renderer>().bounds.min.x,
		                         objectToVoxelize.GetComponent<Renderer>().bounds.min.y,
		                         objectToVoxelize.GetComponent<Renderer>().bounds.min.z );
	}



	static void SetVoxGridOffset() {
		gridOffset = new Vector3( CalculateGridOffset(minBounds.x),
		                          CalculateGridOffset(minBounds.y),
		                          CalculateGridOffset(minBounds.z)  );
	}

	static int CalculateGridOffset(float minBound) {
		int result = 0;
		float moved = 0;

		if (minBound < -0.5f) {
			moved = minBound;
			for (int i=0; i < Mathf.Ceil (Mathf.Abs(minBound)) + 2; i++) {
				result = result + 1;
				moved = moved + 1;
				if (moved >= -0.5f) break;
			}
		}

		if (minBound > -0.5f) {
			moved = minBound;
			for (int i=0; i < Mathf.Ceil (Mathf.Abs(minBound)) + 2; i++) {
				moved = moved - 1;
				if (moved >= -0.5f) {
					result = result - 1;
				}
				else {
					break;
				}
			}
		}
		return result;
	}


	static void CreateVoxelGrid() {
		voxelGrid = new bool[(int)(Mathf.Ceil (maxBounds.x + 0.5f) - Mathf.Floor(minBounds.x + 0.5f)), 
		                     (int)(Mathf.Ceil (maxBounds.y + 0.5f) - Mathf.Floor(minBounds.y + 0.5f)), 
		                     (int)(Mathf.Ceil (maxBounds.z + 0.5f) - Mathf.Floor(minBounds.z + 0.5f))  ];
	}

	static void CreateVoxelObjects() {
		for (int x = 0; x < voxelGrid.GetLength(0); x++) {
			for (int y = 0; y < voxelGrid.GetLength(1); y++) {
				for (int z = 0; z < voxelGrid.GetLength(2); z++) {

					if (voxelGrid[x,y,z] == true) {

						/*
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						DestroyImmediate (cube.collider);

						cube.transform.position = new Vector3(x - gridOffset.x, y - gridOffset.y, z - gridOffset.z);
						cube.transform.parent = clusterObj.transform;
						*/


						GameObject voxelObj = new GameObject();
						voxelObj.name = "vox";
						
						voxelObj.transform.position = new Vector3(x - gridOffset.x, y - gridOffset.y, z - gridOffset.z);
						voxelObj.transform.parent = clusterObj.transform;
						voxelObj.AddComponent<SurforgeRig>();
						cluster.voxels.Add(voxelObj.transform);


					}
				
				}
			}
		}
	}


	//// voxels intersection
	/// 
	static float MAX_FLOAT = 9999999999999999.0f;
	static float MIN_FLOAT = -9999999999999999.0f;

	static float side = 0.5f; //for voxel 1 unit size


	static Vector3[] GetAABCCorners(short x, short y, short z)  {
		Vector3 center = new Vector3(x,y,z);
		Vector3[] corners = new Vector3[8];
		corners[0] = new Vector3(center.x + side, center.y - side, center.z + side);
		corners[1] = new Vector3(center.x + side, center.y - side, center.z - side);
		corners[2] = new Vector3(center.x - side, center.y - side, center.z - side);
		corners[3] = new Vector3(center.x - side, center.y - side, center.z + side);
		corners[4] = new Vector3(center.x + side, center.y + side, center.z + side);
		corners[5] = new Vector3(center.x + side, center.y + side, center.z - side);
		corners[6] = new Vector3(center.x - side, center.y + side, center.z - side);
		corners[7] = new Vector3(center.x - side, center.y + side, center.z + side);

		return corners;	
	}

	static bool ProjectionsIntersectOnAxis(Vector3[] solidA, Vector3[] solidB, Vector3 axis) {
		float minSolidA = MinOfProjectionOnAxis(solidA, axis);
		float maxSolidA = MaxOfProjectionOnAxis(solidA, axis);
		float minSolidB = MinOfProjectionOnAxis(solidB, axis);
		float maxSolidB = MaxOfProjectionOnAxis(solidB, axis);
		
		if (minSolidA > maxSolidB) { 
			return false;
		}
		if (maxSolidA < minSolidB) {
			return false;
		}
		
		return true;    
	}

	static float MinOfProjectionOnAxis(Vector3[] solid, Vector3 axis) {
		float min = MAX_FLOAT;
		float dotProd = 0;
		
		for (int i = 0; i < solid.Length; ++i) {
			dotProd = Vector3.Dot(solid[i], axis);
			if (dotProd < min) { 
				min = dotProd; 
			}
		}
		return min;
	}
	
	static float MaxOfProjectionOnAxis(Vector3[] solid, Vector3 axis) {
		float max = MIN_FLOAT; 
		float dotProd = 0;
		
		for (int i = 0; i < solid.Length; ++i) {
			dotProd = Vector3.Dot(solid[i], axis);
			if (dotProd > max) {
				max = dotProd;
			}
		}
		return max;
	}


	static bool TriangleIntersectAABC(Vector3[] triangle, short x, short y, short z) {
		Vector3[] aabcCorners = GetAABCCorners(x, y, z);
		Vector3 triangleEdgeA = triangle[1] - triangle[0];
		Vector3 triangleEdgeB = triangle[2] - triangle[1];
		Vector3 triangleEdgeC = triangle[0] - triangle[2];
		Vector3 triangleNormal = Vector3.Cross(triangleEdgeA, triangleEdgeB);
		Vector3 aabcEdgeA = new Vector3(1, 0, 0);	
		Vector3 aabcEdgeB = new Vector3(0, 1, 0);	
		Vector3 aabcEdgeC = new Vector3(0, 0, 1);	
		
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeA))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeB))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeA, aabcEdgeC))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeA))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeB))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeB, aabcEdgeC))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeA))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeB))) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, Vector3.Cross(triangleEdgeC, aabcEdgeC))) return false;
		
	
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, triangleNormal)) return false;
		
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeA)) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeB)) return false;
		if (!ProjectionsIntersectOnAxis(aabcCorners, triangle, aabcEdgeC)) return false;
		
		return true;	
	}


	static void FillGridWithGameObjectMeshShell(GameObject gameObj, bool storeNormalSum) {
		Mesh gameObjMesh = (Mesh)gameObj.GetComponent<MeshFilter>().sharedMesh;
		Transform gameObjTransf = gameObj.transform;
		Vector3[] triangle = new Vector3[3];	
		Vector3[] meshVertices = gameObjMesh.vertices;
		int[] meshTriangles = gameObjMesh.triangles;
		int meshTrianglesCount = meshTriangles.Length / 3;
		short x = 0;
		short y = 0;
		short z = 0;

		// For each triangle, perform SAT intersection check with the AABCs within the triangle AABB.
		for (int i = 0; i < meshTrianglesCount; ++i) {
			triangle[0] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3]]);
			triangle[1] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3 + 1]]);
			triangle[2] = gameObjTransf.TransformPoint(meshVertices[meshTriangles[i * 3 + 2]]);

			// Find the triangle AABB, select a sub grid.

			short startX = (short)Mathf.Floor((Mathf.Min(new float[] {triangle[0].x, triangle[1].x, triangle[2].x}) - 1) );
			short startY = (short)Mathf.Floor((Mathf.Min(new float[] {triangle[0].y, triangle[1].y, triangle[2].y}) - 1) );
			short startZ = (short)Mathf.Floor((Mathf.Min(new float[] {triangle[0].z, triangle[1].z, triangle[2].z}) - 1) );
			short endX = (short)Mathf.Ceil((Mathf.Max(new float[] {triangle[0].x, triangle[1].x, triangle[2].x}) + 1) );
			short endY = (short)Mathf.Ceil((Mathf.Max(new float[] {triangle[0].y, triangle[1].y, triangle[2].y}) + 1) );
			short endZ = (short)Mathf.Ceil((Mathf.Max(new float[] {triangle[0].z, triangle[1].z, triangle[2].z}) + 1) );

			if (startX < 0) startX = 0;
			if (startY < 0) startY = 0;
			if (startZ < 0) startZ = 0;

			if (endX > voxelGrid.GetLength(0)) endX = (short)voxelGrid.GetLength(0);
			if (endY > voxelGrid.GetLength(1)) endY = (short)voxelGrid.GetLength(1);
			if (endZ > voxelGrid.GetLength(2)) endZ = (short)voxelGrid.GetLength(2);


			for (x = startX; x < endX; x++) {
				for (y = startY; y < endY; y++) {
					for (z = startZ; z < endZ; z++) {
						if (TriangleIntersectAABC(triangle, x, y, z)) {
							voxelGrid[x, y, z] = true;
						}
					}
				}
			}

			
		}	


	}







}
