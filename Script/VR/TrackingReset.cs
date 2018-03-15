using UnityEngine;
using UnityEngine.VR;//VR系の判定をとるためのusing


/// <summary>
/// VRのヘッドマウントディスプレイのトラッキングの処理
/// </summary>
/// /// <remarks>
/// 担当：鬼頭亮
/// </remarks>
public class TrackingReset : MonoBehaviour
{

    /// <summary>
    /// Rキーを押したらトラッキングする
    /// </summary>
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R)) // spaceキーで位置トラッキングをリセットする
        {
            UnityEngine.XR.InputTracking.Recenter();
        }

    }
}