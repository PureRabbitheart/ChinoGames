using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[SelectionBase]
public class SurforgeCluster : MonoBehaviour {

	public GameObject model;
	public string type;
	public bool freeform;
	public bool ivy;
	public bool fixedRotation;
	public bool rotationFlip;
	public bool neverGlueTo;
	public bool crosslink;
	public bool border;
	public bool corner;
	public bool center;

	public int priority;
	public int length;

	public List<Transform> voxels = new List<Transform>();
		
	public int generation;

	public List<SurforgeCluster> bevelClusters;
	


}
