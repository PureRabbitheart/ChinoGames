using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class CameraPost : MonoBehaviour {

	public Material mat;

	void OnRenderImage (RenderTexture source, RenderTexture destination){

		Graphics.Blit(source,destination,mat);
	}
}
