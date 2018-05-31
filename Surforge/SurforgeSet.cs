using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public class SurforgeSet : MonoBehaviour {

	public string setName;
	public Texture texture;
	public Texture textureLite;
	public List<SurforgeCluster> clusters;
	public int growMode;
	public bool haveBorder;
	public bool pattern;
}
