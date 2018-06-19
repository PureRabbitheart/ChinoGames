using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;//Editor拡張で使うためのusing
#endif

public class MissileManager : MonoBehaviour
{

    enum _type
    {
        Lock,//一回ロックオン
        Pursue,//追撃
        Straight,//直進
    }

    [SerializeField]
    _type eMissileType;

    [SerializeField]
    private GameObject Explosion;//爆破Effect
    [SerializeField]
    private float fSpeed;//移動スピード
    [SerializeField]
    private List<string> EnemyTag;
    [SerializeField]
    private List<string> DamageTag;

    private float Power;//威力
    private Rigidbody p_Rigidbody;
    private GameObject Target = null;

    void OnCollisionEnter(Collision other)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        for (int i = 0; i < DamageTag.Count; i++)
        {
            if (DamageTag[i] == other.transform.tag)
            {
                other.transform.root.SendMessage("Damage",Power);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        p_Rigidbody = GetComponent<Rigidbody>();

        switch (eMissileType)
        {
            case _type.Pursue:
                break;
            case _type.Lock:
                SearchRote();
                break;
            case _type.Straight:
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (eMissileType)
        {
            case _type.Pursue:
                SearchRote();
                Move();
                break;
            case _type.Lock:
                Move();
                break;
            case _type.Straight:
                Move();
                break;
        }

    }

    public void Move()//移動の処理
    {
        p_Rigidbody.position += transform.forward * fSpeed;//worldの青軸を使って敵を動かす
    }

    void BulletPower(float Damage)//銃側から威力を授かる
    {
        Power = Damage;//威力をもらいました
    }

    void SearchRote()
    {
        Target = SearchTarget();
        if (Target != null)
        {
            transform.LookAt(Target.transform);//敵のほうを向く
        }

    }

    GameObject SearchTarget()
    {
        float TmpDistance = 0;           //距離用一時変数
        float Close_Distance = 0;          //最も近いオブジェクトの距離
        GameObject target = null;

        for (int i = 0; i < EnemyTag.Count; i++)
        {
            foreach (GameObject obs in GameObject.FindGameObjectsWithTag(EnemyTag[i]))
            {

                TmpDistance = Vector3.Distance(obs.transform.position, transform.position);

                if (Close_Distance == 0 || Close_Distance > TmpDistance)//比べて近いのがあったら座標をいれる
                {
                    Close_Distance = TmpDistance;//近い敵の距離
                    target = obs; //ターゲット情報を入れる
                }

            }
        }
        return target;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(MissileManager))]
    public class CharacterEditor : Editor
    {
        bool isSearchTab = true;
        bool isDamageTab = true;

        public override void OnInspectorGUI()
        {
            MissileManager p_MissileManager = target as MissileManager;

            p_MissileManager.eMissileType = (_type)EditorGUILayout.EnumPopup("ミサイルのタイプ", p_MissileManager.eMissileType);
            EditorGUILayout.LabelField("移動スピード");
            p_MissileManager.fSpeed = EditorGUILayout.Slider(p_MissileManager.fSpeed, 0.0f, 10.0f);

            p_MissileManager.Explosion = EditorGUILayout.ObjectField("爆発のエフェクト", p_MissileManager.Explosion, typeof(GameObject), true) as GameObject;

            if (p_MissileManager.eMissileType == _type.Lock || p_MissileManager.eMissileType == _type.Pursue)//遊撃ミサイル
            {

                int i, len = p_MissileManager.EnemyTag.Count;

                // 折りたたみ表示
                if (isSearchTab = EditorGUILayout.Foldout(isSearchTab, "サーチするTag名"))
                {
                    // リスト表示
                    for (i = 0; i < len; ++i)
                    {
                        p_MissileManager.EnemyTag[i] = EditorGUILayout.TextField("タグ名" + i.ToString(), p_MissileManager.EnemyTag[i]);
                    }

                    string tmpName = null;
                    string go = EditorGUILayout.TextField("追加", tmpName);
                    if (go != null)
                        p_MissileManager.EnemyTag.Add(go);
                    if (GUILayout.Button("リセット"))
                    {
                        p_MissileManager.EnemyTag.Clear();
                    }

                }

            }
            else if (p_MissileManager.eMissileType == _type.Straight)//固定ミサイル
            {

            }

            int j, size = p_MissileManager.DamageTag.Count;

            // 折りたたみ表示
            if (isSearchTab = EditorGUILayout.Foldout(isSearchTab, "ダメージを与えるTag名"))
            {
                // リスト表示
                for (j = 0; j < size; ++j)
                {
                    p_MissileManager.DamageTag[j] = EditorGUILayout.TextField("タグ名" + j.ToString(), p_MissileManager.DamageTag[j]);
                }

                string tmpName = null;
                string go = EditorGUILayout.TextField("追加", tmpName);
                if (go != null)
                    p_MissileManager.DamageTag.Add(go);
                if (GUILayout.Button("リセット"))
                {
                    p_MissileManager.DamageTag.Clear();
                }

            }

        }
    }

#endif
}
