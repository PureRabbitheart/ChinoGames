using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;//VR関係の処理を行うためのusing
#if UNITY_EDITOR 
using UnityEditor;//Editor拡張で使うためのusing
#endif

public class CameraChange : MonoBehaviour
{


    [SerializeField]
    private GameObject VRCamera;//VRカメラ

    [SerializeField]
    private GameObject Camera;  //カメラの種類を保存



    void Awake()
    {

        Camera.SetActive(false);//初期化
        VRCamera.SetActive(false);//初期化

        if (XRDevice.isPresent == true)//Oculusが接続されていたら
        {
            VRCamera.SetActive(true);//Oculusのカメラをアクティブにする
        }
        else//接続されていなかったら
        {
            Camera.SetActive(true);//メインカメラをアクティブにする
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraChange))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CameraChange p_CameraChange = target as CameraChange;

            p_CameraChange.VRCamera = EditorGUILayout.ObjectField("VRカメラ", p_CameraChange.VRCamera, typeof(GameObject), true) as GameObject;
            p_CameraChange.Camera = EditorGUILayout.ObjectField("Unityカメラ", p_CameraChange.Camera, typeof(GameObject), true) as GameObject;

        }
    }

#endif
}
