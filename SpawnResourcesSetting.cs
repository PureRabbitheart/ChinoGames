using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            Debug.Log(this.PatternName);
        }
    }

    public eWORLD eRoom;
    public eWORLD eTmpRoom;
}


