using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{

    public bool isAIEnemy;//AIか乗っ取っているか
    [SerializeField, Range(0, 100)]
    private float HP;//体力
    [SerializeField]
    private GameObject SkinMeshModel;//色を変えるスキン
    [SerializeField]
    private Transform[] tTarget;//目的場所

    private NavMeshAgent agent;
    private bool[] isTarget;//ターゲット

    private List<GameObject> lTarget = new List<GameObject>();//触れたやつ


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
        if (GetComponent<NavMeshAgent>() != null && tTarget.Length > 0 && isAIEnemy == true)
        {
            navMesh();
        }
        HPController();//死亡判定
    }

    void TakeOverControl()
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


    private void OnFound(GameObject foundObj)//探索中怪しいものを見つけたら
    {
        lTarget.Add(foundObj);
        Debug.Log(foundObj.name);
    }

    private void OnLost(GameObject lostObj)//見つけたものを見失ったら
    {
        lTarget.Remove(lostObj);

        if (lTarget.Count == 0)
        {
        }
    }

    void Damage(float Damage)//ダメージ処理
    {
        HP -= Damage;
    }

    void HPController()
    {
        if (HP <= 0)//体力がなくなったら
        {
            //gameObject.transform.DetachChildren();//親子関係を消して
            Destroy(transform.root.gameObject);//消す
        }

    }

    void navMesh()//NavMeshの制御
    {
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

        for (int i = 0; i < tTarget.Length; i++)
        {
            if (isTarget[i] == true && fDis < 5f)//半径が5F以下になったら
            {
                isTarget[i] = false;//今までの目的地を消す
                int nextTarget = Random.Range(0, tTarget.Length);//ランダム
                isTarget[nextTarget] = true;//次の目的地を設定
                break;
            }
        }
    }

}
