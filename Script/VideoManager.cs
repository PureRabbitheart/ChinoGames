using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

    public bool isReset;
    private float fNowTime;
    private bool isInit;

    void Start()
    {
        isInit = false;
        GetComponent<VideoPlayer>().Play();
    }

    void Update()
    {
        if (isInit == false)
        {
            isInit = true;
            fNowTime = PlayerPrefs.GetFloat("VideoTime");
            GetComponent<VideoPlayer>().time = PlayerPrefs.GetFloat("VideoTime");
        }


        fNowTime += Time.deltaTime;

        if (isReset == true)
        {
            fNowTime = 0;
            isReset = false;
            PlayerPrefs.SetFloat("VideoTime", 0f);
            GetComponent<VideoPlayer>().time = PlayerPrefs.GetFloat("VideoTime");
            GetComponent<VideoPlayer>().Play();
        }
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("VideoTime", fNowTime);

    }
}
