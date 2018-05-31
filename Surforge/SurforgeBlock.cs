using UnityEngine;
using System.Collections;

public class SurforgeBlock {
	
	public bool isActive;

	//0 = default voxel
	//10 = voxel can be instanced only on cover
	//1 = center
	//2 = link
	//3 = ivy
	//4 = cover
	//5 = glue
	//6 = void
	//7 = scatter edges

	public byte blockType = 0;

	//0 = 0 degrees
	//1 = -90 degrees
	//2 = 180 degrees
	//3 = 270(90) degrees  //TODO: load for not vertical oriented links
	public byte rotation;

	public string linkType;
	
	public bool neverGlueTo;
	

}
