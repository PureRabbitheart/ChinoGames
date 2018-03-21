using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{

    [SerializeField]
    private Vector3 vLHandRote;
    [SerializeField]
    private Vector3 vRHandRote;
    [SerializeField]
    private Vector3 vHeadRote;
    [SerializeField]
    public Vector3 vLookPos;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.Find("Camera/OVRCameraRig").GetComponent<OVRCameraRig>().vLHandRote = vLHandRote;
        transform.parent.Find("Camera/OVRCameraRig").GetComponent<OVRCameraRig>().vRHandRote = vRHandRote;
        transform.parent.Find("Camera/OVRCameraRig").GetComponent<OVRCameraRig>().vHeadRote = vHeadRote;
        transform.parent.Find("Camera/OVRCameraRig").GetComponent<OVRCameraRig>().vLookPos = vLookPos;
    }
}
