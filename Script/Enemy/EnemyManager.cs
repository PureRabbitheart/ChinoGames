using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [Tooltip("敵の動くスピードを入力してね")]
    [SerializeField, Range(0, 1)]
    private float fSpeed;
    [Tooltip("回転スピードを入力してね")]
    [SerializeField, Range(0, 1)]
    private float fRotateSpeed;
    [Tooltip("体力")]
    [SerializeField, Range(0, 100)]
    private float HP;
    [SerializeField]
    private GameObject SkinMeshModel;

    private Rigidbody p_Rigidbody;//Rigidbody用の変数
    private Transform target;
    private bool isArea;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sonar")
        {
            foreach (Transform child in SkinMeshModel.transform)
            {
                if (child.GetComponent<Renderer>())
                {
                    child.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 1, 0));
                }
            }
        }
        else if (other.gameObject.tag == "Finish")//finishのtagに当たったら
        {
            isArea = true;//動けない範囲になったら
        }
    }


    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Sonar")
        {
            foreach (Transform child in SkinMeshModel.transform)
            {
                if (child.GetComponent<Renderer>())
                {
                    child.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 0, 0));
                }
            }
        }
        else if (other.gameObject.tag == "Finish")//finishのtagに当たったら
        {
            isArea = false;//動けない範囲になったら
        }
    }


    void Start()
    {
        p_Rigidbody = GetComponent<Rigidbody>();// Rigidbodyの情報を入手
        target = GameObject.Find("Camera").transform;
    }


    void Update()
    {

        if (isArea == false)//動ける範囲なら
        {
            if (fSpeed > 0)
            {
                Move();
            }
            if (fRotateSpeed > 0)
            {
                Rotation();
            }
        }

        if (HP <= 0)//体力がなくなったら
        {
            gameObject.transform.DetachChildren();//親子関係を消して
            Destroy(gameObject);//消す
        }
    }

    void Damage(float Damage)//ダメージ処理
    {
        HP -= Damage;
    }

    public void Move()//移動の処理
    {
        p_Rigidbody.position += transform.forward * fSpeed;//worldの青軸を使って敵を動かす
    }

    void Rotation()//回転の処理
    {
        p_Rigidbody.rotation = Quaternion.Slerp(p_Rigidbody.rotation, Quaternion.LookRotation(target.position - p_Rigidbody.position), fRotateSpeed);//ターゲットの方向を向く

    }

}
