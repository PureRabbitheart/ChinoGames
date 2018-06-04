using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatorManager : MonoBehaviour
{

    [SerializeField]
    private EnemyManager p_EnemyManager;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OneGunShot()//敵の玉を打つ処理
    {
        p_EnemyManager. OneGunShot();
    }

    void TwoGunShot()//敵の玉を打つ処理
    {
        p_EnemyManager.TwoGunShot();
    }

    void AttackEnd()//攻撃終了の合図
    {
        p_EnemyManager.AttackEnd();
    }
    void HelpCall()//仲間を呼ぶ
    {
        p_EnemyManager.HelpCall();
    }

    void TripleGunShot()//敵の玉を打つ処理
    {
        p_EnemyManager.TripleGunShot();
    }

    void DamageEnd()//敵の玉を打つ処理
    {
        p_EnemyManager.DamageEnd();
    }
}
