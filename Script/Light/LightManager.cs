using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] LightObj;//ライトのObject選択
    [SerializeField]
    private Color NextLight;//新しくする色

    public bool isLight;//切り替わるポイント

    private Light p_Light;
    private LensFlare p_LensFlare;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLight == true)
        {
            for (int i = 0; i < LightObj.Length; i++)
            {
                p_Light = LightObj[i].GetComponent<Light>();
                p_LensFlare = LightObj[i].GetComponent<LensFlare>();
                if (p_Light != null)
                {
                    p_Light.color = NextLight;
                }
                if (p_LensFlare != null)
                {
                    p_LensFlare.color = NextLight;
                }
            }
            isLight = false;
        }
    }



}
