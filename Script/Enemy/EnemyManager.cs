using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{

    public bool isAIEnemy;//AIか乗っ取っているか

    private float fAttackTime;//攻撃後のクールタイム
    private float fNowTime;//経過時間
    private bool[] isTarget;//ターゲット
    private bool isAttack;//攻撃したか
    private NavMeshAgent agent;
    private List<GameObject> lTarget = new List<GameObject>();//触れたやつ

    [SerializeField, Range(0, 100)]
    private float HP;//体力
    [SerializeField]
    private float fTimeOut;//色を変えるスキン
    [SerializeField]
    private GameObject SkinMeshModel;//色を変えるスキン
    [SerializeField]
    private Transform[] tTarget;//目的場所

    enum mode
    {
        Wander,//徘徊モード
        Attack,//戦闘モード
        Vigilance,//警戒モード(※移動停止ｗ)
        Pursuit,//追跡モード
    }

    [SerializeField]
    private mode Mode;

    void OnTriggerEnter(Collider other)//触れたら
    {
        if (isAIEnemy == true)
        {
            if (other.tag == "Sonar" && SkinMeshModel != null)
            {
                foreach (Transform child in SkinMeshModel.transform)
                {
                    if (child.GetComponent<Renderer>())
                    {
                        child.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 1, 0));
                    }
                }
            }
        }
        else
        {

        }

    }


    void OnTriggerExit(Collider other)//離れたら
    {
        if (isAIEnemy == true)
        {
            if (other.tag == "Sonar" && SkinMeshModel != null)
            {
                foreach (Transform child in SkinMeshModel.transform)
                {
                    if (child.GetComponent<Renderer>())
                    {
                        child.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0, 0, 0));
                    }
                }
            }
        }
        else
        {

        }
    }


    void Start()
    {
        if (GetComponent<NavMeshAgent>() != null && tTarget.Length > 0)//NavMeshがあれば
        {
            isTarget = new bool[tTarget.Length];//ターゲット分フラグの配列を大きくする
            int nextTarget = Random.Range(0, tTarget.Length);//次に行く場所をランダムで決める
            isTarget[nextTarget] = true;//フラグをON
            agent = GetComponent<NavMeshAgent>();
        }

        SearchingBehavior searching = GetComponentInChildren<SearchingBehavior>();
        searching.onFound += OnFound;
        searching.onLost += OnLost;

    }


    void Update()
    {
        TakeOverControl();//乗り移っているかAI制御か判断する
        EnemyControl();//敵の制御
        HPController();//死亡判定
    }

    void TakeOverControl()//乗っ取られているか
    {
        if (GetComponentInChildren<OVRCameraRig>())
        {
            isAIEnemy = false;
        }
        else
        {
            isAIEnemy = true;
        }
    }

    void EnemyControl()//敵の行動アルゴリズム
    {
        if (isAIEnemy == true)
        {
            switch (Mode)
            {
                case mode.Wander://徘徊
                    WanderingAround();
                    break;
                case mode.Attack://攻撃
                    Attacking();
                    break;
                case mode.Vigilance://警戒
                    WatchOut();
                    break;
                case mode.Pursuit://追跡
                    Chase();
                    break;
            }
        }
    }

    private void OnFound(GameObject foundObj)//探索中怪しいものを見つけたら
    {
        lTarget.Add(foundObj);

        if (lTarget.Count > 0)
        {
            Mode = mode.Vigilance;//警戒モードに移行
            Debug.Log(transform.name + "警戒モードに移行");
        }
    }

    private void OnLost(GameObject lostObj)//見つけたものを見失ったら
    {
        lTarget.Remove(lostObj);

        if (lTarget.Count == 0)
        {
            //Mode = mode.Wander;
        }
    }

    void Damage(float Damage)//ダメージ処理
    {
        HP -= Damage;
    }

    void HPController()//HPの制御
    {
        if (HP <= 0)//体力がなくなったら
        {
            //gameObject.transform.DetachChildren();//親子関係を消して
            Destroy(transform.root.gameObject);//消す
        }

    }

    void WanderingAround()//徘徊モード
    {
        if (GetComponent<NavMeshAgent>() != null && tTarget.Length > 0)
        {
            GetComponent<NavMeshAgent>().enabled = true;

            float fDis = 0.0f;//距離
            for (int i = 0; i < tTarget.Length; i++)
            {
                if (isTarget[i] == true)
                {
                    fDis = Vector3.Distance(transform.position, tTarget[i].position);//目的地までの距離を設定
                    agent.SetDestination(tTarget[i].position);//目的地まで突っ走る
                    break;
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
            Mode = mode.Vigilance;//警戒モードに移行
            Debug.Log(transform.name + "警戒モード移行");
            return;
        }


        float fDis = Vector3.Distance(transform.position, Target.transform.position);//目的地までの距離を設定

        if (GetComponent<NavMeshAgent>() != null && fDis > 3.0f)
        {
            agent.SetDestination(Target.transform.position);//プレイヤーのところまでイクゥゥゥゥゥウウウウ
        }
        else if (fDis < 3.0f)
        {
            Mode = mode.Attack;//戦闘態勢に移行
            fAttackTime = 0.0f;
            isAttack = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            Debug.Log(transform.name + "戦闘態勢に移行");
        }


    }

    void WatchOut()//警戒モード
    {
        fNowTime += Time.deltaTime;//警戒モードになってからの経過時間
        if (fNowTime > fTimeOut)//警戒時間がある一定になったら
        {
            Mode = mode.Wander;//徘徊モードに移行
            Debug.Log(transform.name + "徘徊モードに移行");
        }

        for (int i = 0; i < lTarget.Count; i++)//索敵内にプレイヤーがいるかチェック
        {
            if (lTarget[i].tag == "Player")//いたら
            {
                Mode = mode.Pursuit;//追跡モードに移行
                break;
            }
        }

    }

    void Attacking()
    {
        if (isAttack == true)
        {
            Debug.Log("攻撃した");
            isAttack = false;
        }

        fAttackTime += Time.deltaTime;
        if (fAttackTime > 5.0f)
        {
            TargetInit();
            agent.updatePosition = true;
            agent.updateRotation = true;
            Mode = mode.Pursuit;//追跡モードに移行する
        }

    }

    void TargetInit()
    {
        for (int i = 0; i < tTarget.Length; i++)
        {
            if (isTarget[i] == true)
            {
                isTarget[i] = false;//今までの目的地を消す
                int nextTarget = Random.Range(0, tTarget.Length);//ランダム
                isTarget[nextTarget] = true;//次の目的地を設定
                break;
            }
        }
    }

}
