using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{

    [SerializeField]
    private EnemyManager p_EnemyManager;

    private Animator p_Animator;

    // Use this for initialization
    void Start()
    {
        p_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (p_EnemyManager.Mode)
        {
            case EnemyManager.eMode.Wander://徘徊モード
                p_Animator.SetBool("isWalk", true);
                break;
            case EnemyManager.eMode.Attack://戦闘モード
                p_Animator.SetBool("isWalk", false);
                p_Animator.SetBool("isAttack", true);
                break;
            case EnemyManager.eMode.Vigilance://警戒モード
                p_Animator.SetBool("isWarning", true);
                break;
            case EnemyManager.eMode.Pursuit://追跡モード
                p_Animator.SetBool("isWalk", true);
                break;
            case EnemyManager.eMode.Aid://援護モード
                p_Animator.SetBool("isWalk", true);

                break;
        }
    }
}
