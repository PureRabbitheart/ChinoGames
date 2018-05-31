using UnityEngine;
using System.Collections;

public class SurforgeLink : ScriptableObject {
	
	public int x;
	public int y;
	public int z;
	public byte direction;

	public SurforgeOctreeNode node;

	public bool isTemporaryLink;

	public bool centerX;
	public bool centerZ;
}
