using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    
    private float Power;//  弾の威力　銃側からセンドメッセージで来る
    [Tooltip("あたり判定のあるTag名を入れてね")]
    public string[] strTagName;//  敵のtagを入れておく配列
    //[Tooltip("当たった時のエフェクトのPrefabを入れてね")]
    //public GameObject Effect;//  弾と敵に当たった時のEffect
    //[Tooltip("ヘッドショットした時のエフェクトのPrefabを入れてね")]
    //public GameObject headFX;//  ヘッドショットした時のEffect
    //[Tooltip("壁とかに当たった時のエフェクトのPrefabを入れてね")]
    //public GameObject lastFX;//  かべなどに当たった時のエフェクト

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
                   // Instantiate(headFX, transform.position, transform.rotation);
                   // Instantiate(Effect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));//エフェクトを生成

                }
                

            //    else
            //    {
            //        other.transform.root.gameObject.SendMessage("Damage", Power);
            //        Instantiate(Effect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));//エフェクトを生成
            //    }

            //    Destroy(gameObject);//弾を消す
            //}

            //else if (other.tag == "LastEnemy")
            //{
            //    Instantiate(lastFX, transform.position, transform.rotation);
            //    Destroy(gameObject);//弾を消す
            //}
            //else
            //{
            //    if (other.tag == "NAGANO")
            //    {
            //        Instantiate(Effect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));//エフェクトを生成

            //    }
            }

        }
    }

    //  銃側から弾の威力をSendMessageで受け取る
    void BulletPower(float Damage)//銃側から威力を授かる
    {
        Power = Damage;//威力をもらいました
    }
}
