using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SettingManager : MonoBehaviour
{

    private List<string[]> SettingList = new List<string[]>();//設定ファイルを読み込んだデータを入れておく


    void Awake()
    {
        SettingLoad();//設定ファイルの読み込み
        Setting();//書き込み
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void SettingLoad()//初めて起動時に行う設定
    {
        TextAsset CSVFile = Resources.Load("Setting") as TextAsset; /* Resouces/CSV下のCSV読み込み */
        StringReader reader = new StringReader(CSVFile.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            SettingList.Add(line.Split(',')); // リストに入れる
        }
    }

    void Setting()//設定の確認
    {
        if (int.Parse(SettingList[0][0]) == 0)//初期起動時だったら
        {
            InitSetting();
            SettingList[0][0] = "1";
        }
    }

    void InitSetting()//初期起動の設定を行う処理
    {
        StreamWriter p_StreamWriter = new StreamWriter(Application.dataPath + "/Resources/Setting.csv", false, Encoding.UTF8);//第一引数path、第二引数追記するか最初からやるか
        p_StreamWriter.WriteLine("1");//初期起動
        p_StreamWriter.WriteLine("1");//全体音量
        p_StreamWriter.WriteLine("1");//BGM
        p_StreamWriter.WriteLine("1");//SE
        p_StreamWriter.Flush();
        p_StreamWriter.Close();
    }

    public void SettingSave()//書き込み
    {
        StreamWriter p_StreamWriter = new StreamWriter(Application.dataPath + "/Resources/Setting.csv", false, Encoding.UTF8);//第一引数path、第二引数追記するか最初からやるか
        for (int i = 0; i < SettingList.Count; i++)
        {
            for (int j = 0; j < SettingList[i].Length; j++)
            {
                p_StreamWriter.WriteLine(SettingList[i][j]);
            }
        }
        p_StreamWriter.Flush();
        p_StreamWriter.Close();
    }

    public float ALL//BGMのSetGet
    {
        set { SettingList[1][0] = value.ToString(); }
        get { return float.Parse(SettingList[1][0]); }
    }

    public float BGM//BGMのSetGet
    {
        set { SettingList[2][0] = value.ToString(); }
        get { return float.Parse(SettingList[2][0]); }
    }

    public float SE//SEのSetGet
    {
        set { SettingList[3][0] = value.ToString(); }
        get { return float.Parse(SettingList[3][0]); }
    }
}
