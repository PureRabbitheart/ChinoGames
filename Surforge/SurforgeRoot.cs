#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode]
public class SurforgeRoot : MonoBehaviour {

	[HideInInspector]
	public SurforgeSettings surforgeSettings;
		
	bool skipDestroy;

	void OnDestroy(){
		if (surforgeSettings != null) {
			if (!skipDestroy) {
				surforgeSettings.SkipDestroyRoot();
				DestroyImmediate(surforgeSettings.gameObject);
			}
		}
	}

	public void SkipDestroy() {
		skipDestroy = true;
	}

}
#endif