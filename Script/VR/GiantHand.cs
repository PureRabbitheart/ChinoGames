using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantHand : MonoBehaviour
{
    [SerializeField]
    private Transform HeadMountPoint;//HeadMountの位置
    [SerializeField]
    private Transform Hand;//TouchControllerの位置
    [SerializeField]
    float scale = 3.0f;
    [SerializeField]
    private Vector3 vRote;
    void Start()
    {

    }

    void Update()
    {
        //  HMDから手までの距離を算出する
        Vector3 abst = HeadMountPoint.position - Hand.position;

        //  差分の距離に割り合いを掛ける
        abst *= scale;
        Quaternion qua = Quaternion.Euler(vRote);
        transform.rotation = Hand.rotation * qua;
        transform.position = HeadMountPoint.position - abst;
    }
}