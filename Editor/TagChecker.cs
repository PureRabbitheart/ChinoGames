using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

/// <summary>
///  Tagの数を計算する処理
/// </summary>
/// <remarks>
/// 担当：鬼頭亮
/// </remarks>
public class TagChecker : EditorWindow
{
    private string oldTagToSearchFor = "";
    private string tagToSearchFor = "";

    private Vector2 scrollPos;
    private Vector2 resultScrollPos;
    private Vector2 objectsScrollPos;

    private Dictionary<string, Pair<bool, List<GameObject>>> sceneObjectsWithTag;
    private List<GameObject> results;

    private bool searchFlag = true;
    private bool listFlag = false;

    enum ActiveOption
    {
        ALL,
        ACTIVE_ONLY,
        INACTIVE_ONLY
    };
    private ActiveOption oldActiveOption = ActiveOption.ALL;
    private ActiveOption activeOption = ActiveOption.ALL;


    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("GameObject　TagSearch", EditorStyles.boldLabel);

            oldActiveOption = activeOption;
            activeOption = (ActiveOption)EditorGUILayout.EnumPopup("Select Active or Inactive", (System.Enum)activeOption);

            if (GUILayout.Button("Reload") || activeOption != oldActiveOption)
            {
                LoadSceneObjectsWithTag();
                Search();
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


            //タグ名を指定することで絞り込む////////////////////////////////////////////////////////////
            if (searchFlag = EditorGUILayout.Foldout(searchFlag, "Search Tag Name"))
            {
                EditorGUI.indentLevel++;

                oldTagToSearchFor = tagToSearchFor;
                tagToSearchFor = EditorGUILayout.TagField("Tag To Search:", tagToSearchFor);

                if (tagToSearchFor != oldTagToSearchFor)
                    Search();

                if (results != null)
                {
                    EditorGUILayout.LabelField("Scene Objects Found:", results.Count.ToString(), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    foreach (GameObject go in results)
                    {
                        EditorGUILayout.ObjectField(go, typeof(GameObject), false);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////

            EditorGUILayout.Space();

            //すべてのシーン内オブジェクトをタグ別にリストアップ////////////////////////////////////////
            if (listFlag = EditorGUILayout.Foldout(listFlag, "List By Tags"))
            {
                if (sceneObjectsWithTag != null)
                {
                    EditorGUI.indentLevel++;

                    foreach (var item in sceneObjectsWithTag)
                    {

                        if (item.Value.First = EditorGUILayout.Foldout(item.Value.First, item.Key + ":" + item.Value.Second.Count.ToString()))
                        {
                            EditorGUI.indentLevel++;
                            foreach (GameObject go in item.Value.Second)
                            {
                                EditorGUILayout.ObjectField(go, typeof(GameObject), false);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }


    // すべてのオブジェクトを取得しタグ別にリストに格納する////////////////////////////////////////////////////////////
    void LoadSceneObjectsWithTag()
    {

        // DictionaryのKeyはタグ、ValueはPairクラス.Pairはツリー表示非表示フラグとGameObjectリスト.
        if (sceneObjectsWithTag == null)
            sceneObjectsWithTag = new Dictionary<string, Pair<bool, List<GameObject>>>();

        // オブジェクトDictionalyの初期化その１：未登録のタグがあればそのリストを作成
        foreach (string tag in InternalEditorUtility.tags)
        {
            if (!sceneObjectsWithTag.ContainsKey(tag))
            {
                sceneObjectsWithTag.Add(tag, new Pair<bool, List<GameObject>>());
            }
            sceneObjectsWithTag[tag].Second = new List<GameObject>();
        }

        // オブジェクトDictionalyの初期化その２：タグが消去されていればそのリストを削除        
        bool existTag = false;
        foreach (string key in sceneObjectsWithTag.Keys)
        {
            foreach (string tag in InternalEditorUtility.tags)
            {
                if (key == tag)
                {
                    existTag = true;
                    break;
                }
                else
                {
                    existTag = false;
                }
            }

            if (!existTag)
            {
                sceneObjectsWithTag.Remove(key);
            }
        }

        // 全てのオブジェクトを配列で取得し順に処理する
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {

            // アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得
            string path = AssetDatabase.GetAssetOrScenePath(obj);

            // シーン上に存在するオブジェクトかどうか拡張子で判定
            bool isScene = path.Contains(".unity");
            if (isScene && sceneObjectsWithTag.ContainsKey(obj.tag))
            {
                if (activeOption == ActiveOption.ALL || obj.activeInHierarchy == (activeOption == ActiveOption.ACTIVE_ONLY))
                {  // Activeに関するオプションに一致するかチェック
                    sceneObjectsWithTag[obj.tag].Second.Add(obj);      // タグに一致したオブジェクトはリストに追加
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Search()
    {

        if (tagToSearchFor != null && tagToSearchFor != "")
        {
            results = sceneObjectsWithTag[tagToSearchFor].Second;
        }

    }


    // エディターのメニューバーに追加
    [MenuItem("Tools/Tag Search")]
    static void Init()
    {
        TagChecker window = EditorWindow.GetWindow<TagChecker>("Find With Tag");
        window.LoadSceneObjectsWithTag();
        window.ShowPopup();
    }


    //自作Pairクラス
    private class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
}
