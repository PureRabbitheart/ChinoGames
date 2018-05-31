using UnityEngine;
using UnityEditor;
using System.Collections;

public class SurforgeAssetPostprocessor : AssetPostprocessor {


	void OnPostprocessModel(GameObject obj) {

		if (Surforge.surforgeSettings != null) {
			Surforge.surforgeSettings.modelNeedUpdate = true;
			//Debug.Log ("OnPostprocessModel: " +  obj); 
		}


	}

}