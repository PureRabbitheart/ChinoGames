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
    [SerializeField]
    private GameObject Head;//頭
    [SerializeField]
    private GameObject Hip;//腰
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (LHand != null)
        {
            LHand.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/LeftHandAnchor").gameObject.transform.position;
            LHand.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/LeftHandAnchor").gameObject.transform.rotation;
        }
        if (RHand != null)
        {
            RHand.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/RightHandAnchor").gameObject.transform.position;
            RHand.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/RightHandAnchor").gameObject.transform.rotation;
        }
        if (Head != null)
        {
            Head.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/CenterEyeAnchor").gameObject.transform.position;
            Head.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/CenterEyeAnchor").gameObject.transform.rotation;
        }
        if (Hip != null)
        {
            Hip.transform.position = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/Hip").gameObject.transform.position;
            Hip.transform.rotation = transform.parent.Find("Camera/OVRCameraRig/TrackingSpace/Hip").gameObject.transform.rotation;
        }

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
            p_Alignment.Head = EditorGUILayout.ObjectField("頭", p_Alignment.Head, typeof(GameObject), true) as GameObject;
            p_Alignment.Hip = EditorGUILayout.ObjectField("腰", p_Alignment.Hip, typeof(GameObject), true) as GameObject;


        }
    }

#endif
}
