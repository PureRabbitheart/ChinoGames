
using UnityEditor;
using UnityEngine;

class WebWindow : EditorWindow
{

    WebViewHook webView;
    string url = "https://google.com";

    [MenuItem("Web/WebWindow")]
    static void Load()
    {
        WebWindow window = GetWindow<WebWindow>();
        window.Show();
    }

    void OnEnable()
    {
        if (!webView)
        {
            webView = CreateInstance<WebViewHook>();
        }
    }

    public void OnBecameInvisible()
    {
        if (webView)
        {
            webView.Detach();
        }
    }

    void OnDestroy()
    {
        DestroyImmediate(webView);
    }

    void OnGUI()
    {
        if (webView.Hook(this))
            webView.LoadURL(url);

        if (GUI.Button(new Rect(0, 0, 25, 20), "←"))
            webView.Back();
        if (GUI.Button(new Rect(25, 0, 25, 20), "〒"))
        {
            url = "https://google.com";
            webView.LoadURL(url);

        }
        if (GUI.Button(new Rect(50, 0, 25, 20), "→"))
            webView.Forward();

        GUI.SetNextControlName("urlfield");
        url = GUI.TextField(new Rect(75, 0, (position.width / 3) * 2, 20), url);
        var ev = Event.current;

        if (GUI.Button(new Rect(position.width - 75, 0, 75, 20), "Youtube"))
        {
            url = "https://www.youtube.com/";
            webView.LoadURL(url);

        }

        if (ev.isKey && GUI.GetNameOfFocusedControl().Equals("urlfield"))
            if (ev.keyCode == KeyCode.Return)
            {
                webView.LoadURL(url);
                GUIUtility.keyboardControl = 0;
                webView.SetApplicationFocus(true);
                ev.Use();
            }


        if (ev.type == EventType.Repaint)
        {
            webView.OnGUI(new Rect(0, 20, position.width, position.height - 20));
        }
    }
}
