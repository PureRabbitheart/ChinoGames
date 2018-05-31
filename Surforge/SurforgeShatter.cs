using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public class SurforgeShatter : MonoBehaviour {

	public Texture icon;

	public bool optimalLine; //random split by optimal line (between not near longest edges)

	public bool delaunay; //split using delaunay triangulation
	public float gridStepX; //point grid step for delaunay
	public float gridStepZ;

	public float gridStepRandomOffset; //random offset of the grid's points

	public bool secondStep; //create more points around grid points
	public int  secondStepVertsMin; //points amount
	public int  secondStepVertsMax;
	public float secondStepRandomOffset; //random offset of the second step points

	public bool thirdStep; //create even more points around second step points
	public int  thirdStepVertsMin; //points amount
	public int  thirdStepVertsMax;
	public float thirdStepRandomOffset; //random offset of the third step points

	public float pointMinDistance; //minimal distance between points



}
