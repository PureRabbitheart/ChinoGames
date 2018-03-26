using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [Tooltip("敵の動くスピードを入力してね")]
    [SerializeField, Range(0, 1)]
    public float fSpeed;
    [Tooltip("回転スピードを入力してね")]
    [SerializeField, Range(0, 1)]
    private float fRotateSpeed;
    [Tooltip("体力")]
    [SerializeField, Range(0, 100)]
    public float HP;


    private Rigidbody p_Rigidbody;//Rigidbody用の変数
    private Transform target;
    private bool isArea;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")//finishのtagに当たったら
        {
            isArea = true;//動けない範囲になったら
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Finish")//finishのtagに当たったら
        {
            isArea = false;//動けない範囲になったら
        }
    }

    // Use this for initialization
    void Start()
    {
        p_Rigidbody = GetComponent<Rigidbody>();// Rigidbodyの情報を入手
        target = GameObject.Find("Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (isArea == false)//動ける範囲なら
        {
            Move();
            Rotation();
        }

        if (HP <= 0)
        {
            gameObject.transform.DetachChildren();
            Destroy(gameObject);
        }
    }

    void Damage(float Damage)
    {
        HP -= Damage;
    }

    public void Move()
    {
        p_Rigidbody.position += transform.forward * fSpeed;//worldの青軸を使って敵を動かす
    }

    void Rotation()
    {
        p_Rigidbody.rotation = Quaternion.Slerp(p_Rigidbody.rotation, Quaternion.LookRotation(target.position - p_Rigidbody.position), fRotateSpeed);//ターゲットの方向を向く

    }

}
