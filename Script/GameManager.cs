﻿using System.Collections;
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

    public List<GameObject> A_PosList = new List<GameObject>();//Aの場所リスト
    public List<GameObject> B_PosList = new List<GameObject>();//Bの場所リスト
    public List<GameObject> C_PosList = new List<GameObject>();//Cの場所リスト
    public List<GameObject> D_PosList = new List<GameObject>();//Dの場所リスト
    public bool[] isAreaClear = { false, false, false, false };

    private List<string[]> PatternList = new List<string[]>();//敵の出現パターンリスト
    [SerializeField]
    private int[] CreateCount = { 0, 0, 0, 0 };
    [SerializeField]
    private float[] fNowTime = { 0.0f, 0.0f, 0.0f, 0.0f };

    private GameObject PlayerClear;

    void Awake()
    {
        A_ManagerList = ResourceLoad(EnemyFile[0].name);
        PatternList = ResourceLoad(PatternFile.name);
    }

    void Start()
    {
        isAreaClear = PlayerPrefsX.GetBoolArray("RoomData");
        PlayerClear = GameObject.Find("Clear");
        PlayerClear.SetActive(false);

    }


    public void AreaUpdate(string posName)
    {
        switch (posName)
        {
            case "A":
                EnemyCreate(A_ManagerList, A_PosList, 0);
                break;
            case "B":
                EnemyCreate(B_ManagerList, B_PosList, 1);
                break;
            case "C":
                EnemyCreate(C_ManagerList, C_PosList, 2);
                break;
            case "D":
                EnemyCreate(D_ManagerList, D_PosList, 3);
                break;
        }

    }

    public void AreaReset(int count)
    {
        CreateCount[count] = 0;
        fNowTime[count] = 0.0f;
    }


    void EnemyCreate(List<string[]> managerList, List<GameObject> posList, int count)//敵の生成プログラム  引数…ManagerList,position,count
    {
        fNowTime[count] += Time.deltaTime;
        if (managerList.Count > CreateCount[count] && fNowTime[count] > float.Parse(managerList[CreateCount[count]][0]))//時間になったら
        {

            int PatternNum = int.Parse(managerList[CreateCount[count]][1]);//どのパターンか調べる

            for (int i = 1; i < PatternList[PatternNum].Length; i++)//そのパターンがどのくらいの敵を出すのか調べる
            {
                int EnemyNum = int.Parse(PatternList[PatternNum][i]);//どの敵を出すかを識別

                Instantiate(EnemyList[EnemyNum], posList[int.Parse(managerList[PatternNum][2])].transform.position, Quaternion.identity);//生成
            }
            CreateCount[count]++;

        }
        else if (managerList.Count <= CreateCount[count])//最後まで出し終わったら
        {
            GameObject[] tagobjs = GameObject.FindGameObjectsWithTag("EnemyModel");
            int EnemyCount = 0;
            foreach (GameObject obj in tagobjs)
            {
                EnemyCount++;
            }
            if(EnemyCount == 0)
            {
                RoomClear(count);
            }
            Debug.Log(EnemyCount);
        }
    }


    public void RoomClear(int count)
    {
        isAreaClear[count] = true;
        PlayerPrefsX.SetBoolArray("RoomData", isAreaClear);
        PlayerClear.SetActive(true);
        PlayerClear.GetComponent<Animator>().SetTrigger("Clear");
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
                ListAdd(ref p_GameManager.EnemyList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eTYPE)).Length);
                ListAdd(ref p_GameManager.A_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eA_POS)).Length);
                ListAdd(ref p_GameManager.B_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eB_POS)).Length);
                ListAdd(ref p_GameManager.C_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eC_POS)).Length);
                ListAdd(ref p_GameManager.D_PosList, System.Enum.GetNames(typeof(SpawnResourcesSetting.eD_POS)).Length);
            }

            p_GameManager.fNowTime[0] = EditorGUILayout.FloatField("高さ", p_GameManager.fNowTime[0]);

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


            p_GameManager.isAreaClear[0] = EditorGUILayout.Toggle("A", p_GameManager.isAreaClear[0]);
            p_GameManager.isAreaClear[1] = EditorGUILayout.Toggle("B", p_GameManager.isAreaClear[1]);
            p_GameManager.isAreaClear[2] = EditorGUILayout.Toggle("C", p_GameManager.isAreaClear[2]);
            p_GameManager.isAreaClear[3] = EditorGUILayout.Toggle("D", p_GameManager.isAreaClear[3]);

        }



        void ResetList(ref List<GameObject> list, int count)
        {
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(null);
            }
        }

        void ListAdd(ref List<GameObject> list, int count)
        {
            for (int i = list.Count; i < count; i++)
            {
                list.Add(null);
            }
        }
    }

#endif
}
