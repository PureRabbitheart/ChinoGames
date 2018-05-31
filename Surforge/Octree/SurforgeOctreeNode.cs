using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurforgeOctreeNode : ScriptableObject {
	

	//octree data
	public System.UInt64 mask = 7;
	
	public SerializedSurforgeBlock block;

	public SurforgeOctreeNode[] children;
	public bool hasChildren;
	public int level;

	public Vector3 center;
	public int halfSize;

	public Vector3[] boundsOffsetTable = new Vector3[8]
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(+0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, +0.5f, -0.5f),
		new Vector3(+0.5f, +0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, +0.5f),
		new Vector3(+0.5f, -0.5f, +0.5f),
		new Vector3(-0.5f, +0.5f, +0.5f),
		new Vector3(+0.5f, +0.5f, +0.5f)
	};


	public virtual void Split() {
		if (!hasChildren)  {
			children = new SurforgeOctreeNode[8];
			for (int i = 0; i<8; i++) {
				SurforgeOctreeNode child = (SurforgeOctreeNode)ScriptableObject.CreateInstance(typeof(SurforgeOctreeNode));
				children[i] = child;
				child.level = level + 1;

				// Compute new bounding box for this child
				children[i].center = center + boundsOffsetTable[i] * halfSize * 0.5f;
				children[i].halfSize = (int)(halfSize * 0.5f);
			}
			hasChildren = true;
		}
	}

	public virtual SurforgeOctreeNode CreateBranchWithMorton(System.UInt64 morton, int maxLevel) {
		if (maxLevel >= level) {
			System.UInt64 leveledMorton = morton >> ((maxLevel - level) * 3 );
			System.UInt64 i = leveledMorton & mask;
			Split ();
			return children[i].CreateBranchWithMorton(morton, maxLevel);
		}
		else return this;
	}



	public virtual SurforgeOctreeNode FindChildWithMorton(System.UInt64 morton, int maxLevel) {
		if (level > maxLevel) return this;
		if (!hasChildren) return null;
		System.UInt64 leveledMorton = morton >> ((maxLevel - level) * 3 );
		System.UInt64 i = leveledMorton & mask;
		return children[i].FindChildWithMorton(morton, maxLevel);
	}




	public void DebugAllMortons(int maxLevel) {
		if (children != null)  {
			for (int i = 0; i<8; i++) {
				children[i].DebugAllMortons(maxLevel);
				Debug.Log ("array [i]: " + i + "  level: " + level);
			}
		}
		else {
			if (level >= maxLevel) Debug.Log ("  no children,  " +  " level:  " + level);
		}
	}



	public virtual void DrawNodeGizmos() {
		if (hasChildren) {
		
			for (int i=0; i <  children.Length; i++) {

				Gizmos.color = Color.green * 0.1f * level;
				if (children[i].block != null) {
					//if (!children[i].block.isActive) Gizmos.color = Color.white;

					if (children[i].block.blockType == 0 ) Gizmos.color = new Color(0, 0, 0, 1);
					if (children[i].block.blockType == 2 ) Gizmos.color = new Color(0, 1, 0, 1);
					if (children[i].block.blockType == 3 ) Gizmos.color = new Color(1, 1, 0, 1);
					if (children[i].block.blockType == 4 ) Gizmos.color = new Color(1, 1, 1, 1);
					if (children[i].block.blockType == 5 ) Gizmos.color = new Color(1, 0.5f, 1, 1);
					if (children[i].block.blockType == 6 ) Gizmos.color = new Color(1, 1, 1, 0.3f);
					if (children[i].block.blockType == 7 ) Gizmos.color = new Color(0.5f, 1, 1, 1);
					if (children[i].block.blockType == 10 ) Gizmos.color = new Color(0.5f, 0.5f, 1, 1);

					if (!children[i].block.isActive) Gizmos.DrawWireCube(children[i].center, new Vector3(children[i].halfSize-0.7f, children[i].halfSize-0.7f, children[i].halfSize-0.7f));
					else Gizmos.DrawWireCube(children[i].center, new Vector3(children[i].halfSize-0.2f, children[i].halfSize-0.2f, children[i].halfSize-0.2f));
				}
				//Gizmos.DrawWireCube(children[i].center, new Vector3(children[i].halfSize, children[i].halfSize, children[i].halfSize));

			}
		}
	}
	

	public void SearchForLinksInNodeChildren(List<SurforgeLink> links) {
		if (block != null) {
			if(block.blockType == 2) {
				if (!block.isActive) {
					if (links.Count <= 10) {
						SurforgeLink link = (SurforgeLink)ScriptableObject.CreateInstance(typeof(SurforgeLink));
						link.x = Mathf.RoundToInt(center.x);
						link.y = Mathf.RoundToInt(center.y);
						link.z = Mathf.RoundToInt(center.z);
						link.direction = block.rotation;
						link.node =  this;
						links.Add(link);
					}
				}
			}
		}
		if (hasChildren) {
			for (int i = 0; i<8; i++) {
				children[i].SearchForLinksInNodeChildren(links);
			}
		}
	}

	public void SearchForCoversInNodeChildren(List<SurforgeLink> links) {
		if (block != null) {
			if(block.blockType == 4) {
				if (!block.isActive) {
					if (links.Count <= 10) {
						SurforgeLink link = (SurforgeLink)ScriptableObject.CreateInstance(typeof(SurforgeLink));
						link.x = Mathf.RoundToInt(center.x);
						link.y = Mathf.RoundToInt(center.y);
						link.z = Mathf.RoundToInt(center.z);
						link.direction = block.rotation;
						link.node =  this;
						links.Add(link);
					}
				}
			}
		}
		if (hasChildren) {
			for (int i = 0; i<8; i++) {
				children[i].SearchForCoversInNodeChildren(links);
			}
		}
	}


	public virtual SurforgeOctreeNode GetNodeByPointFromRoot(Vector3 point, int maxSearchLevel) {
		if( (hasChildren) && (level < maxSearchLevel) ) {
			float shortestDistance = 10000;
			int closestChildNum = 0;
			for (int i = 0; i<8; i++) {
				float d = Vector3.Distance(point, children[i].center);
				if (d <  shortestDistance)  {
					shortestDistance = d;
					closestChildNum = i;
				}
			}
			return children[closestChildNum].GetNodeByPointFromRoot(point, maxSearchLevel);
		}
		if (level == maxSearchLevel) return this;
		else return null;
	}
	
	public bool CheckBounds(Vector3 nodeMin, Vector3 nodeMax, Vector3 min, Vector3 max) {
		if ((nodeMax.x >= max.x)&&(nodeMax.y >= max.y)&&(nodeMax.z >= max.z) &&
		    (nodeMin.x <= min.x)&&(nodeMin.y <= min.y)&&(nodeMin.z <= min.z)) {
			return true;
		}
		else return false;
	}



	public bool CheckPointInNode(Vector3 nodeMin, Vector3 nodeMax, Vector3 point) {
		if ((nodeMax.x >= point.x)&&(nodeMax.y >= point.y)&&(nodeMax.z >= point.z) &&
		    (nodeMin.x <= point.x)&&(nodeMin.y <= point.y)&&(nodeMin.z <= point.z)) {
			return true;
		}
		else return false;
	}






	public void SearchForNeighbors(List<SurforgeOctreeNode> nodes, SurforgeOctree octree) {
		List<Vector3> neighbourPositions = new List<Vector3>();
		
		Vector3 nX = new Vector3(center.x + halfSize, center.y, center.z); 
		neighbourPositions.Add (nX);
		Vector3 nZ = new Vector3(center.x, center.y, center.z + halfSize); 
		neighbourPositions.Add (nZ);
		Vector3 nXm = new Vector3(center.x - halfSize, center.y, center.z); 
		neighbourPositions.Add (nXm);
		Vector3 nZm = new Vector3(center.x, center.y, center.z - halfSize); 
		neighbourPositions.Add (nZm);
		Vector3 nXZ = new Vector3(center.x + halfSize, center.y, center.z + halfSize); 
		neighbourPositions.Add (nXZ);
		Vector3 nXZm = new Vector3(center.x + halfSize, center.y, center.z - halfSize); 
		neighbourPositions.Add (nXZm);
		Vector3 nXmZ = new Vector3(center.x - halfSize, center.y, center.z + halfSize); 
		neighbourPositions.Add (nXmZ);
		Vector3 nXmZm = new Vector3(center.x - halfSize, center.y, center.z - halfSize); 
		neighbourPositions.Add (nXmZm);
		
		Vector3 nXY = new Vector3(center.x + halfSize, center.y+ halfSize, center.z); 
		neighbourPositions.Add (nXY);
		Vector3 nZY = new Vector3(center.x, center.y+ halfSize, center.z + halfSize); 
		neighbourPositions.Add (nZY);
		Vector3 nXmY = new Vector3(center.x - halfSize, center.y+ halfSize, center.z); 
		neighbourPositions.Add (nXmY);
		Vector3 nZmY = new Vector3(center.x, center.y+ halfSize, center.z - halfSize); 
		neighbourPositions.Add (nZmY);
		Vector3 nXZY = new Vector3(center.x + halfSize, center.y+ halfSize, center.z + halfSize); 
		neighbourPositions.Add (nXZY);
		Vector3 nXZmY = new Vector3(center.x + halfSize, center.y+ halfSize, center.z - halfSize); 
		neighbourPositions.Add (nXZmY);
		Vector3 nXmZY = new Vector3(center.x - halfSize, center.y+ halfSize, center.z + halfSize); 
		neighbourPositions.Add (nXmZY);
		Vector3 nXmZmY = new Vector3(center.x - halfSize, center.y+ halfSize, center.z - halfSize); 
		neighbourPositions.Add (nXmZmY);
		Vector3 nY = new Vector3(center.x, center.y + halfSize, center.z); 
		neighbourPositions.Add (nY);
		
		Vector3 nXYm = new Vector3(center.x + halfSize, center.y- halfSize, center.z); 
		neighbourPositions.Add (nXYm);
		Vector3 nZYm = new Vector3(center.x, center.y- halfSize, center.z + halfSize); 
		neighbourPositions.Add (nZYm);
		Vector3 nXmYm = new Vector3(center.x - halfSize, center.y- halfSize, center.z); 
		neighbourPositions.Add (nXmYm);
		Vector3 nZmYm = new Vector3(center.x, center.y- halfSize, center.z - halfSize); 
		neighbourPositions.Add (nZmYm);
		Vector3 nXZYm = new Vector3(center.x + halfSize, center.y- halfSize, center.z + halfSize); 
		neighbourPositions.Add (nXZYm);
		Vector3 nXZmYm = new Vector3(center.x + halfSize, center.y- halfSize, center.z - halfSize); 
		neighbourPositions.Add (nXZmYm);
		Vector3 nXmZYm = new Vector3(center.x - halfSize, center.y- halfSize, center.z + halfSize); 
		neighbourPositions.Add (nXmZYm);
		Vector3 nXmZmYm = new Vector3(center.x - halfSize, center.y- halfSize, center.z - halfSize); 
		neighbourPositions.Add (nXmZmYm);
		Vector3 nYm = new Vector3(center.x, center.y - halfSize, center.z); 
		neighbourPositions.Add (nYm);
		
		foreach (Vector3 neighbourPos in neighbourPositions) {
			SurforgeOctreeNode n = octree.GetNodeByPointFromRoot(neighbourPos, level);
			if (n != null) nodes.Add(n);
		}
	}


}
