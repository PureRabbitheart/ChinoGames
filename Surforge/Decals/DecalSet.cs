using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class DecalSet : MonoBehaviour {

	public Texture2D icon;

	public GameObject[] decals;

	public bool inheritMatGroup;
	public bool scatterOnShapeVerts;
	public bool trim;
	public bool perpTrim;
	public bool fitDecals;
	public float trimOffset;
	public float decalOffset;
	public float decalOffsetRandom;
	public float decalGap;
	public float decalGapRandom;
	public float decalSize;
	public float decalSizeRandom;
	public float decalRotation;
	public float decalRotationRandom;
}
