using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCamera : MonoBehaviour
{

    [SerializeField]
    private Transform Camera;
    private Vector3 offset;         //プレイヤーとカメラ間のオフセット距離を格納する Public 変数

    // Use this for initialization
    void Start()
    {
        if (Camera != null)
        {
            offset = transform.position - Camera.transform.position;
        }
        UnityEngine.XR.XRSettings.showDeviceView = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera != null)
        {

            transform.position = Camera.transform.position + offset;
        }
    }
}
