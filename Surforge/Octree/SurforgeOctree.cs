using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeOctree : SurforgeOctreeNode {


	public void SetHalfSizeByMaxDepth(int maxLevel) {
		//int size = 1;
		//for (int i = 0; i < maxLevel; i++) {
		//	size = size * 2;
		//}
		//halfSize = Mathf.RoundToInt(size * 0.5f);
		//Debug.Log(halfSize);

		halfSize = 512;
	}


	public override void Split() {
		if (!hasChildren)  {
			children = new SurforgeOctreeNode[8];
			for (int i = 0; i<8; i++) {
				SurforgeOctreeNode child = (SurforgeOctreeNode)ScriptableObject.CreateInstance(typeof(SurforgeOctreeNode));  
				children[i] = child;
				child.level = 1;

				// Compute new bounding box for this child
				children[i].center = center + boundsOffsetTable[i] * halfSize * 0.5f;
				children[i].halfSize = (int)(halfSize * 0.5f);
			}
			hasChildren = true;
		}
	}



	public override SurforgeOctreeNode CreateBranchWithMorton(System.UInt64 morton, int maxLevel) {

		System.UInt64 leveledMorton = morton >> (maxLevel * 3);
		System.UInt64 i = leveledMorton & mask;
		return children[i].CreateBranchWithMorton(morton, maxLevel);
	}



	public override SurforgeOctreeNode FindChildWithMorton(System.UInt64 morton, int maxLevel) {

		System.UInt64 leveledMorton = morton >> (maxLevel * 3);
		System.UInt64 i = leveledMorton & mask;
		return children[i].FindChildWithMorton(morton, maxLevel);
	}
			


	



}
