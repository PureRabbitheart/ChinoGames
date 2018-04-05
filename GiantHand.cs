using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHand : MonoBehaviour
{

    [SerializeField]
    private Transform Hand;//TouchControllerの位置
    [SerializeField]
    private float Double;//倍率

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Hand.transform.rotation;
        transform.localPosition = Hand.localPosition * Double;

    }
}
