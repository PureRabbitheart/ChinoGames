using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class PolyLassoCorner : MonoBehaviour {

	public int[] cornerCircle; //circle on corners radius (in adaptive step units) for each stage. 0 = no corner circle on this stage
	
	public bool noCutout; //no cut out, only corner detail (board with nails for example) (not active now) 
	public float circleToSideDistance; //distance from circle center to shape edge (not corner)
	public float circleMinEdge; //minimum edge length to spawn corner circles on.
	public float circleMinAngle; //minimum corner angle to spawn corner corcles on (not active now)
	public float circleMaxAngle; //maximum corner angle to spawn corner corcles on (not active now)
	public float minDistBetweenCircleCenters; //minimum distance between circle centers. If circle closer, it wont appear (to prevent overlap)
	public float minDistFromCircleToShapeEdge; //minimum distance from circle to shape edge (to prevent circle draw beyound shape)
	
	public GameObject cornerDetailPrefab; //bolts, rivets etc, spawned on corner points
	public float cornerDetailSize; //size of corner detail object;
	public bool isCornerDetailCornerOriented; //true = details corner oriented. false = details random oriented (screws for example)
	public int cornerDetailHeightNum; //number of element in "heights" array to get height for corner detail
}
