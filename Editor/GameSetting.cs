#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Reflection;

public class GameSetting
{

#if UNITY_EDITOR
    [MenuItem("Tools/Reset/SettingReset")]
#endif

    static void SettingReset()
    {
        StreamWriter StreamWriter_;

        StreamWriter_ = new StreamWriter(Application.dataPath + "/Resources/Setting.csv", false);
        StreamWriter_.WriteLine("0");//初期起動したかの判定
        StreamWriter_.WriteLine("1");//全体音量設定
        StreamWriter_.WriteLine("1");//BGM設定
        StreamWriter_.WriteLine("1");//SE設定
        StreamWriter_.Flush();
        StreamWriter_.Close();

        Debug.Log("ゲームの設定を初期化しました");
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Reset/RoomDataReset")]
#endif

    static void RoomDataReset()
    {
        bool[] isRoomData = { false, false, false, false };

        PlayerPrefsX.SetBoolArray("RoomData", isRoomData);

        Debug.Log("RoomDataを初期化しました");
    }
}
