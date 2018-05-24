using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeGage : MonoBehaviour {
    
    //時間
    [SerializeField]
    private float Max;
    [SerializeField]
    private float Now;

   
    TextMeshProUGUI Text;
    public Image Circle;


    // Use this for initialization
    void Start () {
        Text = GetComponent<TextMeshProUGUI>();
    }
	
	// Update is called once per frame
	void Update () {
        

        Circle.fillAmount = Now / Max;
        Text.SetText(Now.ToString());


    }
}
