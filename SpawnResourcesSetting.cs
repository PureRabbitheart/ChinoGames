using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class SpawnResourcesSetting : ScriptableObject
{
    public enum eWORLD
    {
        A,
        B,
        C,
        D,
        Pattern,
        Chino,
    }

    public enum eA_POS
    {
        右,
        左,

    }
    public enum eB_POS
    {
        ドア,
        テレポート,
    }
    public enum eC_POS
    {
        鹿島,
        ラッキージャーヴィス,
    }
    public enum eD_POS
    {
        ウォースパイト,
        大和,
    }

    public enum eTYPE
    {
        ボス,
        雑魚,

    }

    public struct PatternInfo
    {
        public string PatternName;
        public List<eTYPE> EnemyList;

        public void stringAdd(string name)
        {
            PatternName = name;
            System.Console.WriteLine(name, PatternName);
        }
    }

    public eWORLD eRoom;
    public eWORLD eTmpRoom;
}



