using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
   


    public enum eMODE
    {
        Wander,//徘徊モード
        Attack,//戦闘モード
        Vigilance,//警戒モード(※移動停止ｗ)
        Pursuit,//追跡モード
        Aid,//援護モード
    }
    public enum eATTACK_TYPE
    {
        OneGun,//1つの銃だけ
        DoubleGun,//２つの銃だけ
        Sword,//剣
    }
    public enum eMODEL_TYPE
    {
        Boss,
        Drone,
        FourLeg,
        BigRobo,
        Dust,
        DustMK2,
        Gorilla,
        Gundam,
    }

    public bool isAIEnemy;//AIか乗っ取っているか
    public float fActiveTime;
    public eMODE Mode;
    public eATTACK_TYPE AttackType;
    public eMODEL_TYPE ModelType;
    public List<Transform> LTarget;//目的場所

    private Vector3 AidTarget;//援護を呼ばれた場所
    private float fAttackTime;//攻撃後のクールタイム
    private float fNowTime;//経過時間
    private float fStartTime;//乗りうつり始めた時間

    private bool[] isTarget;//ターゲット
    private bool isAttack;//攻撃したか
    private bool isAfter = false;//乗り移ってからもとに戻ったとき
    private bool isLeftRight;//警戒時の左右を見る
    private bool isInit;//初期化フラグ
    private NavMeshAgent agent;
    private List<GameObject> lTarget = new List<GameObject>();//触れたやつ
    private TimeGage p_TimeGage;

    [SerializeField, Range(0, 100)]
    private float EnemyHP;//体力
    [SerializeField, Range(0, 100)]
    private float PlayerHP;//体力
    [SerializeField]
    private float fMaxActiveTime;//乗りうつって居られる時間

    [SerializeField]
    private float fTimeOut;//警戒モードが切れる時間
    [SerializeField]
    private float fRadius;//半径
    [SerializeField]
    private Animator p_Animator;
    [SerializeField]
    private GunStatus[] p_GunStatus = new GunStatus[3];

    void OnTriggerEnter(Collider other)//触れたら
    {
        if (isAIEnemy == true)
        {

        }
        else
        {

        }

    }


    void OnTriggerExit(Collider other)//離れたら
    {
        if (isAIEnemy == true)
        {

        }
        else
        {

        }
    }


    void Start()
    {
        Invoke("Init", 0.4f);
    }

    void Init()
    {
        p_TimeGage = GameObject.Find("TimeGage").GetComponent<TimeGage>();

        if (GetComponent<NavMeshAgent>() != null && LTarget.Count > 0)//NavMeshがあれば
        {
            isTarget = new bool[LTarget.Count];//ターゲット分フラグの配列を大きくする
            int nextTarget = Random.Range(0, LTarget.Count);//次に行く場所をランダムで決める
            isTarget[nextTarget] = true;//フラグをON
            agent = GetComponent<NavMeshAgent>();
        }
        TakeOverControl();

        if (isAIEnemy == true)
        {
            SearchingBehavior searching = GetComponentInChildren<SearchingBehavior>();
            searching.onFound += OnFound;
            searching.onLost += OnLost;
        }
        isInit = true;
    }


    void Update()
    {
        if (isInit == true)
        {
            TakeOverControl();//乗り移っているかAI制御か判断する
            EnemyControl();//敵の制御
            HPController();//死亡判定
        }

    }

    void TakeOverControl()//乗っ取られているか
    {
        if (GetComponentInChildren<OVRCameraRig>())
        {
            isAfter = true;//乗っ取りました
            isAIEnemy = false;//乗り移っている
            if (agent != null)
            {
                agent.SetDestination(transform.position);
            }
        }
        else
        {
            if (isAfter == true)
            {
                EnemyHP = 0.0f;
                isAfter = false;
            }
            isAIEnemy = true;//AI   
        }
    }

    void EnemyControl()//敵の行動アルゴリズム
    {
        if (isAIEnemy == true)
        {
            switch (Mode)
            {
                case eMODE.Wander://徘徊
                    WanderingAround();
                    break;
                case eMODE.Attack://攻撃
                    Attacking();
                    break;
                case eMODE.Vigilance://警戒
                    WatchOut();
                    break;
                case eMODE.Pursuit://追跡
                    Chase();
                    break;
                case eMODE.Aid://援護
                    Assistance();
                    break;

            }
            AnimatorUpdate();//アニメーション判定
        }
    }

    private void OnFound(GameObject foundObj)//探索中怪しいものを見つけたら
    {
        lTarget.Add(foundObj);

        if (/*lTarget.Count > 0*/foundObj.tag == "Player")
        {
            fNowTime = 0.0f;
            Mode = eMODE.Vigilance;//警戒モードに移行
            Debug.Log("HIt");
        }
    }

    private void OnLost(GameObject lostObj)//見つけたものを見失ったら
    {
        lTarget.Remove(lostObj);

        if (lTarget.Count == 0)
        {
            //Mode = eMODE.Wander;
        }
    }

    void Damage(float Damage)//ダメージ処理
    {
        EnemyHP -= Damage;
        agent.SetDestination(transform.position);//そこの場所で一旦止まる
        p_Animator.SetBool("isDamage", true);
    }

    void HPController()//HPの制御
    {


        if (isAIEnemy == true && EnemyHP <= 0)//体力がなくなったら
        {
            Destroy(transform.root.gameObject);//消す
        }
        else if (TimeActive(isAIEnemy) || isAIEnemy == false && PlayerHP <= 0)
        {
            Debug.Log("死にました");
        }

    }

    bool TimeActive(bool palyer)//乗りうつって居られる時間の計算
    {
        if (palyer == false)
        {
            fActiveTime += Time.deltaTime;
            p_TimeGage.Now = fMaxActiveTime - fActiveTime;
            p_TimeGage.Max = fMaxActiveTime;
            if (fActiveTime > fMaxActiveTime)
            {
                return true;
            }
        }
        return false;
    }


    void WanderingAround()//徘徊モード
    {
        if (GetComponent<NavMeshAgent>() != null && LTarget.Count > 0)
        {
            GetComponent<NavMeshAgent>().enabled = true;

            float fDis = 0.0f;//距離
            for (int i = 0; i < LTarget.Count; i++)
            {
                if (LTarget[i] != null)
                {
                    if (isTarget[i] == true)
                    {
                        fDis = Vector3.Distance(transform.position, LTarget[i].position);//目的地までの距離を設定
                        agent.SetDestination(LTarget[i].position);//目的地まで突っ走る
                        break;
                    }
                }

            }


            if (fDis < 5f)//半径が5F以下になったら
            {
                TargetInit();
            }

        }
        else if (GetComponent<NavMeshAgent>() != null)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    void Chase()//追跡
    {
        GameObject Target = null;//目的のやつ

        for (int i = 0; i < lTarget.Count; i++)//索敵内にプレイヤーがいるかチェック
        {
            if (lTarget[i].tag == "Player")//いたら
            {
                Target = lTarget[i];//プレイヤーの情報を共有する
            }
        }

        if (Target == null)//範囲内にプレイヤーがいなければ
        {
            fNowTime = 0.0f;//経過時間をリセット
            Mode = eMODE.Vigilance;//警戒モードに移行

            return;
        }


        float fDis = Vector3.Distance(transform.position, Target.transform.position);//目的地までの距離を設定

        if (GetComponent<NavMeshAgent>() != null && fDis > 3.0f)
        {
            agent.SetDestination(Target.transform.position);//プレイヤーのところまでイクゥゥゥゥゥウウウウ
        }
        else if (fDis < 15.0f)
        {
            fAttackTime = 0.0f;
            isAttack = true;
            agent.SetDestination(transform.position);
            Mode = eMODE.Attack;//戦闘態勢に移行
        }


    }

    void WatchOut()//警戒モード
    {
        fNowTime += Time.deltaTime;//警戒モードになってからの経過時間
        agent.SetDestination(transform.position);//そこの場所で一旦止まる



        if (fNowTime > fTimeOut)//警戒時間がある一定になったら
        {
            Mode = eMODE.Wander;//徘徊モードに移行
        }

        for (int i = 0; i < lTarget.Count; i++)//索敵内にプレイヤーがいるかチェック
        {
            if (lTarget[i] != null && lTarget[i].tag == "Player")//いたら
            {
                Mode = eMODE.Pursuit;//追跡モードに移行
                break;
            }
        }


        //if (count > 150)
        //{
        //    switch (isLeftRight)
        //    {
        //        case true:
        //            isLeftRight = false;
        //            break;
        //        case false:
        //            isLeftRight = true;
        //            break;
        //    }
        //    count = 0;
        //}
        //switch (isLeftRight)
        //{
        //    case true:
        //        transform.Rotate(new Vector3(0, 1, 0));
        //        count++;
        //        break;
        //    case false:
        //        transform.Rotate(new Vector3(0, -1, 0));
        //        count++;
        //        break;
        //}
    }

    void Attacking()//攻撃モード
    {
        //if (isAttack == true)
        //{
        //    Debug.Log("攻撃した");
        //    p_Animator.SetBool("isAttack", true);
        //    CollHelp();//援護を呼ぶ
        //    isAttack = false;
        //}

        //fAttackTime += Time.deltaTime;
        //if (fAttackTime > 2.0f)
        //{
        //    TargetInit();
        //    Mode = eMODE.Pursuit;//追跡モードに移行する
        //}
        Debug.Log("戦闘モード");
    }



    void TargetInit()//次の目的地を選ぶ　
    {
        for (int i = 0; i < LTarget.Count; i++)
        {
            if (isTarget[i] == true)
            {
                isTarget[i] = false;//今までの目的地を消す
                int nextTarget = Random.Range(0, LTarget.Count);//ランダム
                isTarget[nextTarget] = true;//次の目的地を設定
                break;
            }
        }


    }

    void CollHelp()//援護を呼ぶ
    {
        Collider[] HitHelp = Physics.OverlapSphere(transform.position, fRadius);//範囲内にいる仲間
        for (int i = 0; i < HitHelp.Length; i++)
        {
            EnemyManager enemy = HitHelp[i].GetComponentInParent<EnemyManager>();

            if (enemy != null)
            {
                if (enemy.Mode == eMODE.Wander)
                {
                    enemy.GetAidPos(transform.position);
                    enemy.Mode = eMODE.Aid;//援護モードに移行
                    Debug.Log("援護を呼んだ");
                }
            }
        }
    }

    void Assistance()//援護に向かう
    {
        if (AidTarget != new Vector3(0, 0, 0))
        {
            float fDis = Vector3.Distance(transform.position, AidTarget);//目的地までの距離を設定
            if (fDis > 10.0f)
            {
                agent.SetDestination(AidTarget);//プレイヤーのところまでイクゥゥゥゥゥウウウウ
            }
            else
            {
                p_Animator.SetBool("isRun", false);
                AidTarget = new Vector3(0, 0, 0);
                fNowTime = 0.0f;
                Mode = eMODE.Vigilance;//警戒モード移行
            }
        }

    }

    public void GetAidPos(Vector3 pos)//援護する場所を伝える
    {
        AidTarget = pos;
    }

    void AnimatorUpdate()//アニメーションの切り替え
    {
        switch (Mode)
        {
            case EnemyManager.eMODE.Wander://徘徊モード
                WanderAnim(ModelType);
                break;
            case EnemyManager.eMODE.Attack://戦闘モード
                AttackAnim(ModelType);
                break;
            case EnemyManager.eMODE.Vigilance://警戒モード
                VigilanceAnim(ModelType);
                break;
            case EnemyManager.eMODE.Pursuit://追跡モード
                PursuitAnim(ModelType);
                break;
            case EnemyManager.eMODE.Aid://援護モード
                AidAnim(ModelType);
                break;
        }
    }

    void WanderAnim(eMODEL_TYPE type)//キャラごとのアニメーションの切り替え　徘徊
    {
        switch (type)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.BigRobo:
                p_Animator.SetBool("isWarning", false);
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.FourLeg:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }

    }

    void AttackAnim(eMODEL_TYPE type)//キャラごとのアニメーションの切り替え　攻撃
    {
        switch (type)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isAttack", true);
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isAttack", true);
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.BigRobo:
                BigRoboAttack(true);
                break;
            case eMODEL_TYPE.FourLeg:
                p_Animator.SetBool("isAttack", true);
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isAttack", true);
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }
    }

    void VigilanceAnim(eMODEL_TYPE type)//キャラごとのアニメーションの切り替え　警戒
    {
        switch (type)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.BigRobo:
                p_Animator.SetBool("isWarning", true);
                break;
            case eMODEL_TYPE.FourLeg:
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isWarning", true);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }
    }

    void PursuitAnim(eMODEL_TYPE type)//キャラごとのアニメーションの切り替え　追跡
    {
        switch (type)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.BigRobo:
                p_Animator.SetBool("isWarning", false);
                p_Animator.SetBool("isWalk", true);
                break;
            case eMODEL_TYPE.FourLeg:
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isWalk", false);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }
    }

    void AidAnim(eMODEL_TYPE type)//キャラごとのアニメーションの切り替え　援護
    {
        switch (type)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isRun", true);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isRun", true);
                break;
            case eMODEL_TYPE.BigRobo:
                p_Animator.SetBool("isWarning", false);
                p_Animator.SetBool("isRun", true);
                break;
            case eMODEL_TYPE.FourLeg:
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isWarning", false);
                p_Animator.SetBool("isRun", true);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }
    }

    void BigRoboAttack(bool isAnim)
    {

        if (isAttack == true || isAnim == false)
        {
            p_Animator.SetBool("isWalk", false);
            p_Animator.SetBool("isWarning", false);
            p_Animator.SetBool("isAttack", false);
            p_Animator.SetBool("isAttackRoket", false);
            p_Animator.SetBool("isAttackShot", false);
            p_Animator.SetBool("isAttackRote", false);


            switch (Random.Range(0, 2))
            {
                case 0:
                    p_Animator.SetBool("isAttackShot", isAnim);
                    break;
                case 1:
                    p_Animator.SetBool("isAttackRoket", isAnim);
                    break;
            }
        }
        isAttack = false;


    }

    public void OneGunShot()//敵の玉を打つ処理
    {
        p_GunStatus[0].isEnemyShot = true;
    }
    public void TwoGunShot()//敵の玉を打つ処理
    {
        p_GunStatus[1].isEnemyShot = true;
    }

    public void TripleGunShot()//敵の玉を打つ処理
    {
        p_GunStatus[0].isEnemyShot = true;
        p_GunStatus[1].isEnemyShot = true;
        p_GunStatus[2].isEnemyShot = true;

    }

    public void AttackEnd()//攻撃終了の合図
    {
        switch (ModelType)
        {
            case eMODEL_TYPE.Boss:
                p_Animator.SetBool("isAttack", false);
                break;
            case eMODEL_TYPE.Drone:
                p_Animator.SetBool("isAttack", false);
                break;
            case eMODEL_TYPE.BigRobo:
                BigRoboAttack(false);
                break;
            case eMODEL_TYPE.FourLeg:
                break;
            case eMODEL_TYPE.DustMK2:
                p_Animator.SetBool("isAttack", false);
                break;
            case eMODEL_TYPE.Gundam:
                break;
        }
        TargetInit();
        Mode = eMODE.Pursuit;//追跡モードに移行する
    }
    public void HelpCall()//仲間を呼ぶ
    {
        CollHelp();//援護を呼ぶ
    }
    public void DamageEnd()//仲間を呼ぶ
    {
        p_Animator.SetBool("isDamage", false);
        p_Animator.SetBool("isWalk", false);
        p_Animator.SetBool("isRun", false);
        p_Animator.SetBool("isAttack", false);
        p_Animator.SetBool("isWarning", false);
        p_Animator.SetBool("isAttackShot", false);
        p_Animator.SetBool("isAttackRoket", false);

        TargetInit();
        Mode = eMODE.Pursuit;//追跡モードに移行する
    }

}
