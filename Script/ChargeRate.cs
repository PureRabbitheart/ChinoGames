using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChargeRate : MonoBehaviour
{

    [SerializeField]
    private Transit p_Transit;
    [SerializeField]
    private TextMeshProUGUI Text;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        int ConvertNowTime =(int)(p_Transit.ChargeRate * 100);
        Text.SetText(ConvertNowTime.ToString() + "%");
    }
}
