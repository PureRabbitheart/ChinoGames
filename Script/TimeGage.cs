using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeGage : MonoBehaviour
{

    public float Max;//最大値
    public float Now;//現在の値
    [SerializeField]
    private TextMeshProUGUI Text;
    [SerializeField]
    private Image Circle;//残りの％


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Circle.fillAmount = Now / Max;
        int ConvertNowTime =(int) Now;
        Text.SetText(ConvertNowTime.ToString());
    }
}
