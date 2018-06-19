using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{


    private float Power;//  弾の威力　銃側からセンドメッセージで来る
    [SerializeField]
    private string[] strTagName;//  敵のtagを入れておく配列
    [SerializeField]
    private GameObject HitFX;//  弾と敵に当たった時のEffect
                             //[Tooltip("ヘッドショットした時のエフェクトのPrefabを入れてね")]
                             //public GameObject headFX;//  ヘッドショットした時のEffect
    [SerializeField]
    private GameObject lastFX;//  かべなどに当たった時のエフェクト

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    //  あたり判定に当たったら１フレームのみ通す　当たったやつにダメージを送る　エフェクトを出す
    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < strTagName.Length; i++)//配列分ループ
        {
            if (other.tag == strTagName[i])//当たったObjectが敵なら
            {
                if (other.tag == "EnemyHead")
                {
                    other.transform.root.gameObject.SendMessage("Damage", Power * 100);//敵にセンドメッセージを送る
                    other.tag = "CutObject";
                    Destroy(other.gameObject, 5f);
                    if (!other.gameObject.GetComponent<Rigidbody>())
                    {
                        other.gameObject.AddComponent<Rigidbody>();
                    }
                    // Instantiate(headFX, transform.position, transform.rotation);
                    Instantiate(HitFX, transform.position, Quaternion.identity);//エフェクトを生成

                }
                else if (other.tag == "BackGround")
                {
                    Instantiate(lastFX, transform.position, Quaternion.identity);//エフェクトを生成
                }
                else
                {
                    other.transform.root.SendMessage("Damage", Power);
                    Instantiate(HitFX, transform.position, Quaternion.identity);//エフェクトを生成
                }

                Destroy(gameObject);//弾を消す
            }

        }
    }


    //  銃側から弾の威力をSendMessageで受け取る
    void BulletPower(float Damage)//銃側から威力を授かる
    {
        Power = Damage;//威力をもらいました
    }
}
