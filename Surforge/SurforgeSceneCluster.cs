using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
[SelectionBase]
public class SurforgeSceneCluster : MonoBehaviour {
	
	public int generation;
	public string type;

	public List<SurforgeLink> links =  new List<SurforgeLink> ();
	public List<SurforgeLink> covers = new List<SurforgeLink> ();

	public SurforgeVoxelizedCluster vCluster = null;

	//public Int3 position;
	public int positionX;
	public int positionY;
	public int positionZ;

	public int variant;

	public int group;

	public int layer;
}
