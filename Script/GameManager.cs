using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset[] EnemyFile = new TextAsset[4];//敵の出てくる情報が入っているデータ
    [SerializeField]
    private TextAsset PatternFile;//敵の出てくる情報が入っているデータ
    [SerializeField]
    private List<string[]> A_ManagerList = new List<string[]>();//Aの場所の敵情報を管理するリスト
    [SerializeField]
    private List<string[]> B_ManagerList = new List<string[]>();//Bの場所の敵情報を管理するリスト
    [SerializeField]
    private List<string[]> C_ManagerList = new List<string[]>();//Cの場所の敵情報を管理するリスト
    [SerializeField]
    private List<string[]> D_ManagerList = new List<string[]>();//Dの場所の敵情報を管理するリスト

    [SerializeField]
    private List<GameObject> EnemyList = new List<GameObject>();//敵の情報を管理するリスト
    [SerializeField]
    private List<GameObject> A_PosList = new List<GameObject>();//敵の出現パターンリスト
    [SerializeField]
    private List<GameObject> B_PosList = new List<GameObject>();//敵の出現パターンリスト
    [SerializeField]
    private List<GameObject> C_PosList = new List<GameObject>();//敵の出現パターンリスト
    [SerializeField]
    private List<GameObject> D_PosList = new List<GameObject>();//敵の出現パターンリスト

    private List<string[]> PatternList = new List<string[]>();//敵の出現パターンリスト
    private int[] CreateCount = { 0, 0, 0, 0 };
    private float fNowTime = 0.0f;

    void Awake()
    {
        A_ManagerList = ResourceLoad(EnemyFile[0].name);
        PatternList = ResourceLoad(PatternFile.name);
    }

    void Start()
    {

    }

    void Update()
    {
        EnemyCreate(0);
        Debug.Log(CreateCount[0]);
    }

    void EnemyCreate(int count)//敵の生成プログラム
    {
        fNowTime += Time.deltaTime;

        if (fNowTime > float.Parse(A_ManagerList[CreateCount[count]][0]))//時間になったら
        {

            int PatternNum = int.Parse(A_ManagerList[CreateCount[count]][1]);//どのパターンか調べる
            for (int i = 1; i < PatternList[PatternNum][0].Length; i++)//そのパターンがどのくらいの敵を出すのか調べる
            {
                int EnemyNum = int.Parse(PatternList[PatternNum][i]);//どの敵を出すかを識別
                Instantiate(EnemyList[EnemyNum], transform.position, Quaternion.identity);//生成
            }
            CreateCount[count]++;

        }
    }

    List<string[]> ResourceLoad(string FileName)//データの読み込み
    {
        List<string[]> tmpFile = new List<string[]>();
        TextAsset CSVFile = Resources.Load(FileName) as TextAsset; /* Resouces/CSV下のCSV読み込み */
        StringReader reader = new StringReader(CSVFile.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            tmpFile.Add(line.Split(',')); // リストに入れる
        }
        return tmpFile;
    }

    void ResourceSave(List<string[]> file, string fileName)//データ出力
    {
        StreamWriter p_StreamWriter = new StreamWriter(Application.dataPath + "/Resources/" + fileName + ".csv", false, Encoding.UTF8);//第一引数path、第二引数追記するか最初からやるか
                                                                                                                                       // データ出力
        for (int i = 0; i < file.Count; i++)
        {
            string tmpstring = string.Join(",", file[i]);
            p_StreamWriter.WriteLine(tmpstring);

        }
        p_StreamWriter.Flush();
        p_StreamWriter.Close();
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class CharacterEditor : Editor
    {
        bool[] isList = { false, false, false, false, false, false };

        GameManager p_GameManager = null;

        public override void OnInspectorGUI()
        {
            if (p_GameManager == null)
            {
                p_GameManager = target as GameManager;
                //ResetList(ref p_GameManager.EnemyList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eTYPE)).Length);
                //ResetList(ref p_GameManager.A_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eA_POS)).Length);
                //ResetList(ref p_GameManager.B_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eB_POS)).Length);
                //ResetList(ref p_GameManager.C_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eC_POS)).Length);
                //ResetList(ref p_GameManager.D_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eD_POS)).Length);
            }

            p_GameManager.EnemyFile[0] = EditorGUILayout.ObjectField("Aの場所のレベルデータ", p_GameManager.EnemyFile[0], typeof(TextAsset), true) as TextAsset;
            p_GameManager.EnemyFile[1] = EditorGUILayout.ObjectField("Bの場所のレベルデータ", p_GameManager.EnemyFile[1], typeof(TextAsset), true) as TextAsset;
            p_GameManager.EnemyFile[2] = EditorGUILayout.ObjectField("Cの場所のレベルデータ", p_GameManager.EnemyFile[2], typeof(TextAsset), true) as TextAsset;
            p_GameManager.EnemyFile[3] = EditorGUILayout.ObjectField("Dの場所のレベルデータ", p_GameManager.EnemyFile[3], typeof(TextAsset), true) as TextAsset;

            p_GameManager.PatternFile = EditorGUILayout.ObjectField("敵のパターンデータ", p_GameManager.PatternFile, typeof(TextAsset), true) as TextAsset;

            if (isList[0] = EditorGUILayout.Foldout(isList[0], "敵のリスト"))
            {
                // Enemyリスト表示
                for (int i = 0; i < System.Enum.GetNames(typeof(SpawnResourcesSetting.eTYPE)).Length; i++)//enumの数分回す
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var strVal = System.Enum.GetName(typeof(SpawnResourcesSetting.eTYPE), i);
                        EditorGUILayout.LabelField(strVal);
                        p_GameManager.EnemyList[i] = EditorGUILayout.ObjectField(p_GameManager.EnemyList[i], typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (isList[1] = EditorGUILayout.Foldout(isList[1], "Aの出現場所"))
            {
                // Enemyリスト表示
                for (int i = 0; i < System.Enum.GetNames(typeof(SpawnResourcesSetting.eA_POS)).Length; i++)//enumの数分回す
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var strVal = System.Enum.GetName(typeof(SpawnResourcesSetting.eA_POS), i);
                        EditorGUILayout.LabelField(strVal);
                        p_GameManager.A_PosList[i] = EditorGUILayout.ObjectField(p_GameManager.A_PosList[i], typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            if (isList[2] = EditorGUILayout.Foldout(isList[2], "Bの出現場所"))
            {
                // Enemyリスト表示
                for (int i = 0; i < System.Enum.GetNames(typeof(SpawnResourcesSetting.eB_POS)).Length; i++)//enumの数分回す
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var strVal = System.Enum.GetName(typeof(SpawnResourcesSetting.eB_POS), i);
                        EditorGUILayout.LabelField(strVal);
                        p_GameManager.B_PosList[i] = EditorGUILayout.ObjectField(p_GameManager.B_PosList[i], typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            if (isList[3] = EditorGUILayout.Foldout(isList[3], "Cの出現場所"))
            {
                // Enemyリスト表示
                for (int i = 0; i < System.Enum.GetNames(typeof(SpawnResourcesSetting.eC_POS)).Length; i++)//enumの数分回す
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var strVal = System.Enum.GetName(typeof(SpawnResourcesSetting.eC_POS), i);
                        EditorGUILayout.LabelField(strVal);
                        p_GameManager.C_PosList[i] = EditorGUILayout.ObjectField(p_GameManager.C_PosList[i], typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (isList[4] = EditorGUILayout.Foldout(isList[4], "Dの出現場所"))
            {
                // Enemyリスト表示
                for (int i = 0; i < System.Enum.GetNames(typeof(SpawnResourcesSetting.eD_POS)).Length; i++)//enumの数分回す
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        var strVal = System.Enum.GetName(typeof(SpawnResourcesSetting.eD_POS), i);
                        EditorGUILayout.LabelField(strVal);
                        p_GameManager.D_PosList[i] = EditorGUILayout.ObjectField(p_GameManager.D_PosList[i], typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        void ResetList(ref List<GameObject> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(null);
            }
        }
    }

#endif
}
