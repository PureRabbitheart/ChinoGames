using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;


public class EnemySpawnSetting : EditorWindow
{
    [MenuItem("Tools/EnemySpawnSetting")]
    private static void Create()
    {
        // 生成
        var window = GetWindow<EnemySpawnSetting>("SpawnSetting");
        window.minSize = new Vector2(1280, 640);

    }

    private SpawnResourcesSetting p_SpawnResource;
    private EnemyDetails p_EnemyDetails;
    private Vector2 vScrollPos = Vector2.zero;
    private Vector2 vLeftScroll = Vector2.zero;
    private List<string[]> SettingList = new List<string[]>();//設定ファイルを読み込んだデータを入れておく
    private List<bool> TabChange = new List<bool>();//スポーンの設定を拡大縮小するList
    private List<bool> PatternTab = new List<bool>();//パターンのTabを拡大縮小するList
    private List<SpawnResourcesSetting.PatternInfo> PatternList = new List<SpawnResourcesSetting.PatternInfo>();//パターンの設定を行うためのList
    private List<string> newName = new List<string>();//新しい名前を保存できるようの変数
    public List<string[]> PatternChoiceList = new List<string[]>();//パターンの名前を入れるList
    static Texture2D bannerTex = null;

    private bool isWeb;


    private void OnGUI()
    {

        if (p_SpawnResource == null)
        {
            p_SpawnResource = ScriptableObject.CreateInstance<SpawnResourcesSetting>();
            ListInit();
            SettingList = ResourceLoad(p_SpawnResource.eRoom.ToString());
            TabReset(SettingList.Count);
            PatternChoiceList = ResourceLoad("Pattern");

        }

        if (p_EnemyDetails == null)
        {
            p_EnemyDetails = ScriptableObject.CreateInstance<EnemyDetails>();
        }

        using (new GUILayout.HorizontalScope())
        {
            p_SpawnResource.eRoom = (SpawnResourcesSetting.eWORLD)EditorGUILayout.EnumPopup("設定する項目", p_SpawnResource.eRoom);
            if (p_SpawnResource.eRoom != p_SpawnResource.eTmpRoom)
            {
                if (p_SpawnResource.eTmpRoom == SpawnResourcesSetting.eWORLD.Pattern)//パターンから離れたら
                {
                    PatternSave(PatternList);
                    p_SpawnResource.eTmpRoom = p_SpawnResource.eRoom;
                    PatternList.Clear();
                }
                if (p_SpawnResource.eRoom == SpawnResourcesSetting.eWORLD.Chino)
                {

                }
                else if (p_SpawnResource.eRoom != SpawnResourcesSetting.eWORLD.Pattern)
                {
                    PatternChoiceList.Clear();
                    ResourceSave(SettingList, p_SpawnResource.eTmpRoom.ToString());
                    p_SpawnResource.eTmpRoom = p_SpawnResource.eRoom;
                    ListInit();
                    SettingList = ResourceLoad(p_SpawnResource.eRoom.ToString());
                    PatternChoiceList = ResourceLoad("Pattern");
                    TabReset(SettingList.Count);
                }
                else if (p_SpawnResource.eRoom == SpawnResourcesSetting.eWORLD.Pattern)//パターンを選んだら
                {
                    ResourceSave(SettingList, p_SpawnResource.eTmpRoom.ToString());
                    p_SpawnResource.eTmpRoom = p_SpawnResource.eRoom;
                    ListInit();
                    List<string[]> tempList = ResourceLoad(p_SpawnResource.eRoom.ToString());
                    PatternListConvert(tempList);
                    TabReset(tempList.Count);
                }

            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("ようこそ！　Unityエディタ拡張の沼へ！");





        switch (p_SpawnResource.eRoom)
        {
            case SpawnResourcesSetting.eWORLD.Pattern:
                PatternUpdate();
                break;
            case SpawnResourcesSetting.eWORLD.Chino:
                ChinoUpdate();
                break;
            default:
                SpawnUpdate();
                break;
        }


        if (SettingList[0][0] != "-0")
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("ファイル保存", GUILayout.MinWidth(500.0f), GUILayout.MinHeight(30.0f)))
                {
                    ResourceSave(SettingList, p_SpawnResource.eRoom.ToString());
                }
            }
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("全消し", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
                {
                    SettingList.Clear();
                    PatternList.Clear();
                    newName.Clear();

                    ListInit();
                    TabReset(SettingList.Count);
                    PatternListInit("New");

                }
            }
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

    void TabReset(int count)
    {
        TabChange.Clear();
        for (int i = 0; i < count; i++)
        {
            TabChange.Add(false);
        }
    }

    void ListInit()//リストの初期化
    {
        string[] line = { "-0", "0", "0" };
        SettingList.Add(line);
    }

    void PatternUpdate()//パターンのアップデート
    {
        vScrollPos = EditorGUILayout.BeginScrollView(vScrollPos, GUI.skin.box);
        {
            // スクロール範囲
            for (int i = 0; i < PatternList.Count; i++)
            {
                TabChange[i] = EditorGUILayout.Foldout(TabChange[i], PatternList[i].PatternName);
                if (TabChange[i] == true)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    newName[i] = EditorGUILayout.TextField("新しいパターンネーム", newName[i]);

                    if (GUILayout.Button("パターン名前変更", GUILayout.MaxWidth(100.0f), GUILayout.MaxHeight(30.0f)))
                    {
                        SpawnResourcesSetting.PatternInfo list = new SpawnResourcesSetting.PatternInfo();
                        list.PatternName = newName[i];
                        list.EnemyList = new List<SpawnResourcesSetting.eTYPE>();
                        list.EnemyList = PatternList[i].EnemyList;
                        PatternList[i] = list;
                    }

                    EditorGUILayout.EndHorizontal();


                    for (int j = 0; j < PatternList[i].EnemyList.Count; j++)
                    {
                        PatternList[i].EnemyList[j] = (SpawnResourcesSetting.eTYPE)EditorGUILayout.EnumPopup("出てくる敵", PatternList[i].EnemyList[j]);
                    }
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    if (GUILayout.Button("出す敵を追加", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
                    {
                        PatternList[i].EnemyList.Add(GetType(0));
                    }

                    if (GUILayout.Button("出す敵を最後から１つ消す", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
                    {
                        if (PatternList[i].EnemyList.Count - 1 > 0)
                        {
                            PatternList[i].EnemyList.RemoveAt(PatternList[i].EnemyList.Count - 1);
                        }
                        else
                        {
                            Debug.Log("これ以上消せません");
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        if (GUILayout.Button("保存", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
        {
            PatternSave(PatternList);//パターンファイルの保存
        }

        if (GUILayout.Button("パターン追加", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
        {
            PatternListInit("追加");
            TabChange.Add(false);
            newName.Add("NewName");
        }

        if (GUILayout.Button("パターンの最後から１つ消す", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
        {
            if (PatternList.Count - 1 > 0)
            {
                newName.RemoveAt(PatternList.Count - 1);
                PatternList.RemoveAt(PatternList.Count - 1);
                TabChange.RemoveAt(PatternList.Count - 1);
            }
            else
            {
                Debug.Log("これ以上消せません");
            }
        }

        EditorGUILayout.EndHorizontal();

    }

    void PatternListConvert(List<string[]> list)//パターンファイルを扱いやすいようにコンバートする
    {
        for (int i = 0; i < list.Count; i++)
        {
            SpawnResourcesSetting.PatternInfo tmpInfo = new SpawnResourcesSetting.PatternInfo();//一時保存用

            //tmpInfo.stringAdd(list[i][0]);//一時保存先に名前を入れる
            tmpInfo.PatternName = list[i][0];
            tmpInfo.EnemyList = new List<SpawnResourcesSetting.eTYPE>();//保存用のインスタンス化
            for (int j = 0; j < list[i].Length; j++)
            {
                if (j != 0)//パターンネームだけ無視する
                {
                    tmpInfo.EnemyList.Add(GetType(int.Parse(list[i][j])));
                }
            }
            newName.Add("NewName");
            PatternList.Add(tmpInfo);
        }
    }

    void PatternListInit(string name)//パターンリストの初期化
    {
        SpawnResourcesSetting.PatternInfo tmpInfo = new SpawnResourcesSetting.PatternInfo();//一時保存用
        tmpInfo.EnemyList = new List<SpawnResourcesSetting.eTYPE>();
        tmpInfo.stringAdd(name);
        tmpInfo.EnemyList.Add(GetType(0));
        PatternList.Add(tmpInfo);
        newName.Add("NewName");
    }

    void PatternSave(List<SpawnResourcesSetting.PatternInfo> file)//パターンファイルの保存
    {
        StreamWriter p_StreamWriter = new StreamWriter(Application.dataPath + "/Resources/Pattern.csv", false, Encoding.UTF8);//第一引数path、第二引数追記するか最初からやるか
        List<string> fileContents = new List<string>();

        for (int i = 0; i < file.Count; i++)
        {
            string newFile = file[i].PatternName;
            fileContents.Add(newFile);
            for (int j = 0; j < file[i].EnemyList.Count; j++)
            {
                int num = (int)file[i].EnemyList[j];
                string newString = num.ToString();
                fileContents.Add(newString);
            }
            string[] stringArray = fileContents.ToArray();
            string tmpstring = string.Join(",", stringArray);
            p_StreamWriter.WriteLine(tmpstring);
            fileContents.Clear();
        }




        p_StreamWriter.Flush();
        p_StreamWriter.Close();
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public SpawnResourcesSetting.eTYPE GetType(int id)
    {
        int max = System.Enum.GetNames(typeof(SpawnResourcesSetting.eTYPE)).Length;
        for (int i = 0; i < max; i++)
        {
            if (i == id)
            {
                SpawnResourcesSetting.eTYPE type = (SpawnResourcesSetting.eTYPE)System.Enum.ToObject(typeof(SpawnResourcesSetting.eTYPE), i);
                return type;
            }
        }
        return 0;
    }

    void SpawnUpdate()//スポーンの設定のアップデート
    {

        vScrollPos = EditorGUILayout.BeginScrollView(vScrollPos, GUI.skin.box);
        {
            // スクロール範囲
            for (int i = 0; i < SettingList.Count; ++i)
            {
                float fTime = float.Parse(SettingList[i][0]);
                int M = (int)(fTime / 60);
                float S = fTime % 60;

                TabChange[i] = EditorGUILayout.Foldout(TabChange[i], M + "分" + S  + "秒");
                if (TabChange[i] == true)
                {

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        // ここの範囲は横並び
                        SettingList[i][0] = EditorGUILayout.TextField("タイマー", SettingList[i][0]);

                        if (GUILayout.Button("1秒マイナス"))
                        {
                            TimeFluctuation(ref SettingList[i][0], -1.0f);
                        }
                        if (GUILayout.Button("0.1秒マイナス"))
                        {
                            TimeFluctuation(ref SettingList[i][0], -0.1f);
                        }

                        if (GUILayout.Button("0.1秒プラス"))
                        {
                            TimeFluctuation(ref SettingList[i][0], 0.1f);
                        }
                        if (GUILayout.Button("1秒プラス"))
                        {
                            TimeFluctuation(ref SettingList[i][0], 1.0f);
                        }


                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Screen.width / 2), GUILayout.Height(100.0f));//パターンのListを出す
                        {
                            vLeftScroll = EditorGUILayout.BeginScrollView(vLeftScroll);
                            {
                                SettingList[i][1] = PatternChoice(PatternChoiceList, int.Parse(SettingList[i][1])).ToString();
                            }
                            EditorGUILayout.EndScrollView();
                            if (GUILayout.Button("詳細表示"))
                            {
                                p_EnemyDetails.Create();
                                p_EnemyDetails.ReLoad();
                            }
                        }
                        EditorGUILayout.EndVertical();


                        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Screen.width / 2), GUILayout.Height(100.0f));//パターンのListを出す
                        {
                            int name = 0;
                            switch (p_SpawnResource.eRoom)
                            {
                                case SpawnResourcesSetting.eWORLD.A:
                                    var enumA = (SpawnResourcesSetting.eA_POS)System.Enum.ToObject(typeof(SpawnResourcesSetting.eA_POS), int.Parse(SettingList[i][2]));//intからenumにする
                                    enumA = (SpawnResourcesSetting.eA_POS)EditorGUILayout.EnumPopup("出現ポイント", enumA);//enumにして
                                    name = (int)enumA;
                                    break;
                                case SpawnResourcesSetting.eWORLD.B:
                                    var enumB = (SpawnResourcesSetting.eB_POS)System.Enum.ToObject(typeof(SpawnResourcesSetting.eB_POS), int.Parse(SettingList[i][2]));//intからenumにする
                                    enumB = (SpawnResourcesSetting.eB_POS)EditorGUILayout.EnumPopup("出現ポイント", enumB);//enumにして
                                    name = (int)enumB;
                                    break;
                                case SpawnResourcesSetting.eWORLD.C:
                                    var enumC = (SpawnResourcesSetting.eC_POS)System.Enum.ToObject(typeof(SpawnResourcesSetting.eC_POS), int.Parse(SettingList[i][2]));//intからenumにする
                                    enumC = (SpawnResourcesSetting.eC_POS)EditorGUILayout.EnumPopup("出現ポイント", enumC);//enumにして
                                    name = (int)enumC;
                                    break;
                                case SpawnResourcesSetting.eWORLD.D:
                                    var enumD = (SpawnResourcesSetting.eD_POS)System.Enum.ToObject(typeof(SpawnResourcesSetting.eD_POS), int.Parse(SettingList[i][2]));//intからenumにする
                                    enumD = (SpawnResourcesSetting.eD_POS)EditorGUILayout.EnumPopup("出現ポイント", enumD);//enumにして
                                    name = (int)enumD;
                                    break;
                            }


                            SettingList[i][2] = name.ToString();//enumからstringにする
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            if (GUILayout.Button("ソート", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
            {

                for (int start = 1; start < SettingList.Count; start++)
                {
                    for (int end = SettingList.Count - 1; end >= start; end--)
                    {
                        if (float.Parse(SettingList[end - 1][0]) > float.Parse(SettingList[end][0]))
                        {
                            string[] tmp = SettingList[end - 1];
                            SettingList[end - 1] = SettingList[end];
                            SettingList[end] = tmp;
                        }
                    }
                }

                ResourceSave(SettingList, p_SpawnResource.eTmpRoom.ToString());
                ListInit();
                SettingList = ResourceLoad(p_SpawnResource.eRoom.ToString());

            }

            if (GUILayout.Button("追加", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
            {
                string[] tmp = { "0", "0", "0" };
                SettingList.Add(tmp);
                TabChange.Add(false);
            }

            if (GUILayout.Button("最後から１つ消す", GUILayout.MinWidth(300.0f), GUILayout.MinHeight(30.0f)))
            {
                if (SettingList.Count - 1 > 0)
                {
                    SettingList.RemoveAt(SettingList.Count - 1);
                    TabChange.RemoveAt(SettingList.Count - 1);
                }
                else
                {
                    Debug.Log("これ以上消せません");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

    }

    void TimeFluctuation(ref string Timer, float Difference)
    {
        float value = float.Parse(Timer);
        value += Difference;
        Timer = value.ToString();
    }

    public int PatternChoice(List<string[]> textList, int index)//パターンの選んでいるものを返す
    {
        for (int i = 0; i < textList.Count; ++i)
        {
            bool flg = GUILayout.Toggle(index == i, textList[i][0]);

            if (flg == true)
            {
                index = i;
            }
        }
        return index;
    }

    void ChinoUpdate()
    {
        if (bannerTex == null)
            bannerTex = Resources.Load<Texture2D>("kasima");

        GUILayout.Space(3);
        var rect = GUILayoutUtility.GetRect(0, int.MaxValue, 30, 30);
        EditorGUI.DrawPreviewTexture(rect, bannerTex, null, ScaleMode.ScaleAndCrop);

        if (isWeb == false)
        {
            Web();
            isWeb = true;
        }
    }

    void Web()
    {
        string YoutubeURL = "https://youtu.be/h4tlNlbuIm0";
        string typeName = "UnityEditor.Web.WebViewEditorWindowTabs";
        Type type = Assembly.Load("UnityEditor.dll").GetType(typeName);

        BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        var methodInfo = type.GetMethod("Create", Flags);
        methodInfo = methodInfo.MakeGenericMethod(type);

        methodInfo.Invoke(null, new object[] { "WebView", YoutubeURL, 200, 530, 800, 600 });


    }
}


public class EnemyDetails : EditorWindow
{

    public void Create()
    {
        // 生成
        var window = GetWindow<EnemyDetails>("PatternList");
        window.minSize = new Vector2(1280, 640);

    }


    private List<bool> TabChange = new List<bool>();
    private List<string[]> EnemyList = new List<string[]>();
    private SpawnResourcesSetting p_SpawnResourcesSetting;
    private Vector2 vScrollPos = Vector2.zero;


    private void OnGUI()
    {
        if (p_SpawnResourcesSetting == null)
        {
            p_SpawnResourcesSetting = ScriptableObject.CreateInstance<SpawnResourcesSetting>();

            ReLoad();
        }

        EditorGUILayout.LabelField("敵のパターンリストです");
        vScrollPos = EditorGUILayout.BeginScrollView(vScrollPos, GUI.skin.box);
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                TabChange[i] = EditorGUILayout.Foldout(TabChange[i], EnemyList[i][0]);
                if (TabChange[i] == true)
                {
                    for (int m = 1; m < EnemyList[i].Length; m++)
                    {

                        var enmVal = (SpawnResourcesSetting.eTYPE)System.Enum.ToObject(typeof(SpawnResourcesSetting.eTYPE), int.Parse(EnemyList[i][m]));//intからenumにする
                        var Name = System.Enum.GetName(typeof(SpawnResourcesSetting.eTYPE), enmVal);//enumからstringにする
                        EditorGUILayout.LabelField(Name);
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        //p_EnemySpawnSetting.PatternChoiceList[0];

    }

    public void ReLoad()
    {
        EnemyList.Clear();
        EnemyList = ResourceLoad("Pattern");
        TabReset(EnemyList.Count);
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

    void TabReset(int count)
    {
        TabChange.Clear();
        for (int i = 0; i < count; i++)
        {
            TabChange.Add(false);
        }
        Debug.Log(TabChange.Count);
    }
}