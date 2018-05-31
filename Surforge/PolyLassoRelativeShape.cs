using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class PolyLassoRelativeShape : MonoBehaviour {

	public Vector2[] relativeShape; // x for deptfh offset relative to parent edge, y for adaptive step number of perent edge (rounded down)
	public PolyLassoProfile relativeShapeProfilePreset; //child profile presets
}
