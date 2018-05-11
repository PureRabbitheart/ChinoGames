using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class MixerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject gSettingUI;
    [SerializeField]
    private AudioMixerSnapshot GameShot;
    [SerializeField]
    private AudioMixerSnapshot SEShot;
    [SerializeField]
    private AudioMixer p_AudioMixer;
    [SerializeField]
    private SettingManager p_SettingManager;
    [SerializeField]
    private Slider BGMBar;//BGM音量
    [SerializeField]
    private Slider SEBar;//SE音量
    [SerializeField]
    private Slider ALLBar;//全体音量
    private bool isUIActive;


    void Awake()
    {

    }

    void Start()
    {
        InitVolume();
        isUIActive = false;
        gSettingUI.SetActive(isUIActive);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isUIActive == true)
            {
                isUIActive = false;
            }
            else
            {
                isUIActive = true;
            }
            gSettingUI.SetActive(isUIActive);
        }
    }

    void InitVolume()
    {
        p_AudioMixer.SetFloat("MasterVol", p_SettingManager.ALL);
        p_AudioMixer.SetFloat("BGMVol", p_SettingManager.BGM);
        p_AudioMixer.SetFloat("SEVol", p_SettingManager.SE);
        ALLBar.value = p_SettingManager.ALL;
        BGMBar.value = p_SettingManager.BGM;
        SEBar.value = p_SettingManager.SE;

    }

    public void SetMaster(float volume)
    {
        p_AudioMixer.SetFloat("MasterVol", volume);
        p_SettingManager.ALL = volume;
        p_SettingManager.SettingSave();
    }

    public void SetBGM(float volume)
    {
        p_AudioMixer.SetFloat("BGMVol", volume);
        p_SettingManager.BGM = volume;
        p_SettingManager.SettingSave();
    }

    public void SetSE(float volume)
    {
        p_AudioMixer.SetFloat("SEVol", volume);
        p_SettingManager.SE = volume;
        p_SettingManager.SettingSave();
    }
}
