using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRote : MonoBehaviour
{


    [Tooltip("回転スピードを入力してね")]
    [SerializeField, Range(0, 1)]
    private float fRotateSpeed;

    private Rigidbody p_Rigidbody;//Rigidbody用の変数
    private Transform target;

    void Start()
    {
        p_Rigidbody = GetComponent<Rigidbody>();// Rigidbodyの情報を入手
        target = GameObject.Find("Camera").transform;

    }

    void Update()
    {
        Rotation();
    }

    void Rotation()
    {
       // p_Rigidbody.rotation = Quaternion.Slerp(p_Rigidbody.rotation, Quaternion.LookRotation(target.position - p_Rigidbody.position), fRotateSpeed);//ターゲットの方向を向く
    }
}
