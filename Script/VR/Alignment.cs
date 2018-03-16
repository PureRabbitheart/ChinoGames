using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;//Editor拡張で使うためのusing
#endif

public class Alignment : MonoBehaviour
{
    [SerializeField]
    private GameObject LHand;//左手
    [SerializeField]
    private GameObject RHand;//右手

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LHand.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/LeftHandAnchor").gameObject.transform.position;
        LHand.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/LeftHandAnchor").gameObject.transform.rotation;
        RHand.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/RightHandAnchor").gameObject.transform.position;
        RHand.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/RightHandAnchor").gameObject.transform.rotation;

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Alignment))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Alignment p_Alignment = target as Alignment;
            p_Alignment.LHand = EditorGUILayout.ObjectField("左手", p_Alignment.LHand, typeof(GameObject), true) as GameObject;
            p_Alignment.RHand = EditorGUILayout.ObjectField("右手", p_Alignment.RHand, typeof(GameObject), true) as GameObject;

        }
    }

#endif
}
