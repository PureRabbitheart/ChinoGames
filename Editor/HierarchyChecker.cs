
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// HierachyにコンポーネントActiveチェックボックスを表示するEditorWindow
/// </summary>
public class HierarchyChecker : EditorWindow
{
    private const int WIDTH = 18;
    [SerializeField]
    private string strComponentName = "";
    static private string strComponentName_ = "";

    [MenuItem("Tools/HierarchyChecker")]
    static void Open()
    {
        GetWindow<HierarchyChecker>();
    }

    void OnDestroy()
    {
        strComponentName_ = ""; // リセット
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("指定したコンポーネントのActiveチェックボックスをHierarchyに表示します");
        EditorGUI.BeginChangeCheck();
        strComponentName_ = this.strComponentName;
        this.strComponentName = EditorGUILayout.TextField(strComponentName_);
        if (EditorGUI.EndChangeCheck())
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.RepaintHierarchyWindow();
            };
        }
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        if (!IsOpen()) { return; }

        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) { return; }

        var component = go.GetComponent(strComponentName_);
        if (component == null) { return; }

        EditorGUI.BeginChangeCheck();
        var enabledProperty = component.GetType().GetProperties().FirstOrDefault(p => p.Name == "enabled");
        if (enabledProperty == null) { return; }

        var rect = selectionRect;
        rect.x = rect.xMax - WIDTH;
        rect.width = WIDTH;
        bool enabled = EditorGUI.Toggle(rect, (bool)enabledProperty.GetValue(component, null)); // ON/OFFアクティブボックスを表示
        if (EditorGUI.EndChangeCheck())
        {
            enabledProperty.SetValue(component, enabled, null);
            EditorSceneManager.MarkSceneDirty(go.scene); // シーン変更フラグ ON
        }
    }

    private static bool IsOpen()
    {
        var windows = Resources.FindObjectsOfTypeAll<HierarchyChecker>();
        return windows != null && windows.Length > 0;
    }
}
