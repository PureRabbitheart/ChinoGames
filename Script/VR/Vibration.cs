using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// touchコントローラーのバイブレーションの処理
/// </summary>
/// /// <remarks>
/// 担当：鬼頭亮
/// </remarks>
public class Vibration : MonoBehaviour
{

    /// <summary>
    /// 音楽でバイブレーションのパターンを指定できる　Nullでもよい
    /// </summary>
    public AudioClip audioClip;

    /// <summary>
    /// オキュラスタッチの情報を入れる変数
    /// </summary>
    OVRHapticsClip hapticsClip;


    /// <summary>
    /// バイブレーションの大元となる処理
    /// </summary>
    void Update()
    {

        byte[] samples = new byte[8];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = 255;
        }
        hapticsClip = new OVRHapticsClip(samples, samples.Length);

    }



    /// <summary>
    /// 右のコントローラーのバイブ関数
    /// </summary>
    public void R_VIBRATION(byte vibration_power)
    {
        OVRHaptics.RightChannel.Mix(hapticsClip);//右のコントローラーをバイブする

        byte[] samples = new byte[8];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = vibration_power;//byte型なので1-255の間
        }
        hapticsClip = new OVRHapticsClip(samples, samples.Length);
    }



    /// <summary>
    /// 左のコントローラーのバイブ関数
    /// </summary>
    public void L_VIBRATION(byte vibration_power)
    {
        OVRHaptics.LeftChannel.Mix(hapticsClip); //左のコントローラーをバイブする

        byte[] samples = new byte[8];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = vibration_power;//byte型なので1-255の間
        }
        hapticsClip = new OVRHapticsClip(samples, samples.Length);
    }
}
