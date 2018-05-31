using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public class PolyLassoProfile : MonoBehaviour {

	//
	//----Poly Lasso Profile preset format----
	//
	public Texture icon; //profile icon
	public Texture iconLite; //profile icon for lite guiskin
	
	public float bevelAmount; //the amount of bevel for simple (not isAdaptive) shapes
	public int bevelSteps; //bevel steps. Beware of values more than 4
	public float[] offsets; //horisontal offsets. Can be negative, but beware high values. -0.1f / +0.5f commonly used
	public float[] heights; //vertical offsets. The length must be equal to horisontal offsets
	public Color iconColor = Color.white; //icon color
	public bool isFloater; // true = frofile is floater. No Ambient Occlusion outside this object (renders in separate pass)

	public bool isTube; //true = profile is tube (spawn as tube-shaped curve). false = profile is loft (spawn as panel)
	public bool isOpen; //true = curve is open (not closed shape)
	public float thickness; //tube thickness
	public Vector2[] section; //points of tube section

	public bool isAdaptive; //if true, shape has adaptive subdivision and can spawn aditional detail on edges and corners (with settings below)
	public float adaptiveStep; //size of adaptive step (now only 0.1f supported)
	
	public float[] lengthOffsets0; //the edges detailing offset information, horisontal
	public float[] lengthOffsets1;
	public float[] lengthOffsets2;

	public float[] heightOffsets0; //the edges detailing offset information, vertical (not active now)
	public float[] heightOffsets1;
	public float[] heightOffsets2;
	
	public int repeatSize; //size of offset array (count from the array end) to repeat. Repeated part starts from edge center.

	public int[] lengthOffsetOrder; //the edges detailing horisontal sets order (to use one multiple times, etc)
	public int[] heightOffsetOrder; //the edges detailing vertical sets order (to use one multiple times, etc) (not active now)

	public bool edgeWiseOffset; //true = spawn details symmetricaly between start/end of the edge. false = spawn detail in repeat cycle between start/end of whole shape.
	public bool lengthWiseOffset; //true = spawn details symmetricaly between start/end of the shape. Overrides edgeWiseOffset
	public float offsetMinEdge; //minimum edge to spawn edgewise detail.

	public PolyLassoCorner corner; // poly lasso corner profile, optional

	public float[] childProfileVerticalOffsets; //vertical offsets for child profiles relative to parent Y position
	public float[] childProfileDepthOffsets; //how child profiles deepen / bumpout relative to parent poly lasso shape
	public int[] childProfileHorisontalOffsets; //offset of child profile from corner in adaptive subdivision units
	public int[] childProfileMatGroups; //numbers of material group attached to child 
	public PolyLassoRelativeShape[] childProfileShapes; //child poly lasso profiles, arranged on edge. x for offset, y for adaptive step number (rounded down)

	public PolyLassoProfile[] followerProfiles; //follower profiles copy parent profile shape with given offset
	public float[] followerProfileVerticalOffsets; //vertical offsets for follower profiles relative to parent Y position
	public float[] followerProfileDepthOffsets; //how follower profiles deepen / bumpout relative to parent poly lasso shape
	public int[] followerProfileMatGroups; //numbers of material group attached to follower 

	public Texture2D cutoff; //cutoff seamless texture for making grills and stripes
	public Vector2 cutoffTiling; //cutoff tiling settings

	public Texture2D bumpMap; //bumpMap seamless texture for making rocks, etc
	public float bumpMapIntensity = 1; //bumpMap intensity 0 to 2
	public Vector2 bumpMapTiling; //bump and ao maps tiling settings
	public Texture2D aoMap; //aoMap seamless texture
	public float aoMapIntensity; //aoMap intensity -1 to 1. Usually you whant to use something like 1 to 1.1 to get more worn edges

	public bool randomUvOffset; //randomize profile texture UV offset

	public int stoneType; //0 no stone effect, 1 convex hull, 2 convex hull + subdivide + 3d noise, 3 gemA effect, 4 gemB effect 

	public bool allowIntersections; //allow intersections for closed shapes. Open shapes allow intersections by default
	public bool overlapIntersections; //overlap intersections (celtic ornament) (for tubes only)
	public float overlapAmount; //how far up and down intersection overlapping point can move (for tubes only)
	public bool usedForOverlapping; //other shape can overlap this
	public bool overlapStartInvert; //first overlap go up instead of down
	public bool curveUVs; // create UVs along curve, not planar

}
