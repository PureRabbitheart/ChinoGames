using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeVoxelizedCluster : ScriptableObject {

	public GameObject model;
	public string type;
	public bool ivy;
	public bool freeform;
	public bool fixedRotation;
	public bool rotationFlip;
	public bool crosslink;
	public bool border;
	public bool corner;
	public bool center;

	public int priority;
	public int length;
	
	public SurforgeBlock[,,] clusterGrid;
	
	public SurforgeBlock[][,,] clusterAllRotations;

	public SurforgeVoxelizedCluster[] voxelizedBevelClusters;

	public int group;

	public bool mismatchPattern;

	public MonoBehaviour clusterPrefab;

}
