// OculusFinger Ver.0.21 - zlib License
// (C) NISHIDA Ryota, ship of EYLN http://dev.eyln.com

using UnityEngine;
using System.Collections.Generic;

public class OculusFinger : MonoBehaviour
{
    [TooltipAttribute("Awake時に現在のFingerTypeにあわせて自動設定を行うか")]
    public bool isAwakeAutoSetup = true;
    [TooltipAttribute("Oculus Touchによるタッチ入力を有効にするか")]
    public bool isEnableTouchControl = true;

    public enum FingerType
    {
        Custom,     // 独自設定
        Auto,       // Awake時に指の名前から自動的設定
        L_Thumb,    // 左手親指
        L_Index,    // 左手人差し指
        L_Middle,   // 左手中指
        L_Ring,     // 左手薬指
        L_Pinky,    // 左手小指
        R_Thumb,    // 右手親指
        R_Index,    // 右手人差し指
        R_Middle,   // 右手中指
        R_Ring,     // 右手薬指
        R_Pinky,    // 右手小指
    }

    [TooltipAttribute("指のタイプ")]
    public FingerType fingerType = FingerType.Auto;

    [HeaderAttribute("Oculus Touch Settings")]

    [TooltipAttribute("触れると指を半分曲げる近接センサーボタン類")]
    public List<OVRInput.RawTouch> touchButtonPool;
    [TooltipAttribute("握ると指を曲げるトリガー（touchButtonPool指定時は半分～最後まで）")]
    public OVRInput.RawAxis1D trigger = OVRInput.RawAxis1D.None;
    [Range(0, 0.99f), TooltipAttribute("トリガーを使うとき、指を曲げ始める開始位置")]
    public float triggerStart = 0.0f;
    [TooltipAttribute("触れていてなおかつトリガーを少し曲げたときに指を半分曲げる近接センサーボタン")]
    public OVRInput.RawTouch relatedTouchButton;

    [HeaderAttribute("Joint Settings")]

    [SerializeField, TooltipAttribute("根本から指先につながっていく関節を設定（未指定のときAwakeで自動設定）")]
    private List<Transform> jointPool;
    [SerializeField, TooltipAttribute("各関節が根本の角度に対してどの割合で曲がるかを設定（未指定のときAwakeで自動設定）")]
    private List<float> jointLevelPool;
    private List<Quaternion> jointBaseRotPool; // 各関節の初期姿勢（Awake時に自動記憶）

    [HeaderAttribute("Finger Angle")]
    [TooltipAttribute("指関節の回転軸")]
    public Vector3 axis = Vector3.up;
    [TooltipAttribute("現在の回転角度（実行時に動的に変わる）")]
    public float angle = 0.0f;
    [TooltipAttribute("最大の回転角度")]
    public float maxAngle = 90.0f;
    [Range(0, 1), TooltipAttribute("指を開くときの補間の速さ")]
    public float openLerpLevel = 0.3f;
    [Range(0, 1), TooltipAttribute("指を閉じるときの補間の速さ")]
    public float closeLerpLevel = 0.15f;
    [TooltipAttribute("親指か（親指を立てられるようにするか）")]
    public bool isThumb = false;
    private bool isThumbsUp = false; // 親指を立てているか

    void Awake()
    {
        if (isAwakeAutoSetup) { AutoSetup(); }

        jointBaseRotPool = new List<Quaternion>();
        foreach (Transform joint in jointPool)
        {
            jointBaseRotPool.Add(joint.transform.localRotation);
        }
    }

    // 現在のFingerTypeにあわせて各種設定を自動設定（コンポーネントのメニューからも実行可能）
    [ContextMenu("Automatic Setup")]
    void AutoSetup()
    {
        AutoSetupJoint();
        SetupFingerType(fingerType);
    }

    // 指の根本にOculusFingerコンポーネントをつけたとして、その子供の最初にあるものを
    // 順に関節として、末端の前まで自動設定する（手動設定済みの場合はそのまま）
    // あわせて、関節を曲げる量も個別に設定可能にする
    void AutoSetupJoint()
    {
        if (jointPool.Count <= 0)
        {
            Transform t = transform;
            while (t && t.childCount > 0)
            {
                jointPool.Add(t);
                jointLevelPool.Add(1.0f);
                t = t.GetChild(0);
            }
        }
    }

    // GameObjectの名前が指の名前のとき指タイプとボタンや回転軸を自動設定
    void AutoSetupFingerType()
    {
        FingerType type = FingerType.Custom;
        string name = transform.name.ToLower();
        string[] typeNames = { "thumb", "index", "middle", "ring", "pinky" };
        int typeIndex = (int)FingerType.L_Thumb;
        if (name.IndexOf("right") >= 0) { typeIndex = (int)FingerType.R_Thumb; }

        for (int i = 0; i < typeNames.Length; i++)
        {
            if (name.IndexOf(typeNames[i]) >= 0)
            {
                type = (FingerType)(typeIndex + i);
                break;
            }
        }
        SetupFingerType(type);
    }

    void Update()
    {
        if (!isEnableTouchControl) return;

        // Oculus Touchの近接センサー付きボタンを指定していて、触れていたら指を半分曲げる
        float touchLevel = 0.0f;
        foreach (OVRInput.RawTouch touch in touchButtonPool)
        {
            if (OVRInput.Get(touch)) { touchLevel = 0.5f; break; }
        }

        // 近接センサーを指定していて、なおかつ（この指の）トリガーを少しでもひいていたら指を半分曲げる
        bool isRelatedTouch = OVRInput.Get(relatedTouchButton) && OVRInput.Get(trigger) > 0.1f;
        if (isRelatedTouch) touchLevel = 0.5f;

        // 近接センサーに触れてるか、近接センサーを指定していないとき、トリガーで曲げる
        if (touchLevel > 0.0f || touchButtonPool.Count <= 0)
        {
            float triggerLevel = (OVRInput.Get(trigger) - triggerStart) / (1 - triggerStart);
            triggerLevel = Mathf.Clamp01(triggerLevel);
            if (touchLevel > 0.0f) { triggerLevel *= 0.5f; } // 近接センサーを使っているときは残りをトリガーで曲げる
            touchLevel += triggerLevel;
        }

        // 親指のとき、（中指、小指などの）指定のトリガーを引いていて、（親指の）近接センサーから指が離れていたら親指を立てるモードにする
        isThumbsUp = (isThumb && touchLevel <= 0.05f && OVRInput.Get(trigger) > triggerStart);
        if (isThumbsUp) { touchLevel = -0.5f; }

        // 指を補間量にあわせて補間
        float lerpLevel = (touchLevel <= 0.0f) ? openLerpLevel : closeLerpLevel;
        if (touchLevel > 0.0f && touchButtonPool.Count > 0 && OVRInput.Get(trigger) < 0.1f) { lerpLevel *= 0.5f; } // ちょっと近接センサーに触れただけのときはゆっくり補間する
        angle = Mathf.Lerp(angle, maxAngle * touchLevel, lerpLevel);
    }

    void LateUpdate()
    {
        if (jointPool.Count != jointBaseRotPool.Count && jointPool.Count != jointBaseRotPool.Count)
        {
            Debug.LogError("jointData Error.");
            return;
        }

        // 指の角度にあわせて各関節の姿勢を決定
        for (int i = 0; i < jointPool.Count; i++)
        {
            Transform joint = jointPool[i];
            float jointLevel = jointLevelPool[i];
            if (isThumbsUp) { jointLevel = 0.5f - i * 0.1f; }
            float rot = angle * jointLevel;
            Quaternion jointBaseRot = jointBaseRotPool[i];
            joint.localRotation = jointBaseRot * Quaternion.AngleAxis(angle * jointLevel, axis);
        }
    }

    // 指タイプにあわせてボタンや回転軸を自動設定
    // （キャラクターによって調整が必要な場合は下記内容を書き換えるか、Inspector上で手動設定する）
    void SetupFingerType(FingerType type)
    {
        this.fingerType = type;
        if (type == FingerType.Custom) return;
        if (type == FingerType.Auto) { AutoSetupFingerType(); return; }

        touchButtonPool.Clear();
        relatedTouchButton = OVRInput.RawTouch.None;
        triggerStart = 0.0f;
        axis = new Vector3(0, 1, 0);
        isThumb = false;

        switch (fingerType)
        {
            case FingerType.L_Thumb:
                touchButtonPool.Add(OVRInput.RawTouch.LThumbstick);
                touchButtonPool.Add(OVRInput.RawTouch.LThumbRest);
                touchButtonPool.Add(OVRInput.RawTouch.X);
                touchButtonPool.Add(OVRInput.RawTouch.Y);
                trigger = OVRInput.RawAxis1D.LHandTrigger;
                axis = new Vector3(-0.1f, 0.8f, -0.4f);
                isThumb = true;
                break;
            case FingerType.L_Index:
                trigger = OVRInput.RawAxis1D.LIndexTrigger;
                triggerStart = 0.5f;
                relatedTouchButton = OVRInput.RawTouch.LIndexTrigger;
                axis = new Vector3(0.1f, 1, 0);
                break;
            case FingerType.L_Middle:
                trigger = OVRInput.RawAxis1D.LHandTrigger;
                relatedTouchButton = OVRInput.RawTouch.LIndexTrigger;
                triggerStart = 0.95f;
                break;
            case FingerType.L_Ring:
                trigger = OVRInput.RawAxis1D.LHandTrigger;
                triggerStart = 0.1f;
                break;
            case FingerType.L_Pinky:
                trigger = OVRInput.RawAxis1D.LHandTrigger;
                axis = new Vector3(0, 1, -0.1f);
                break;

            case FingerType.R_Thumb:
                touchButtonPool.Add(OVRInput.RawTouch.RThumbstick);
                touchButtonPool.Add(OVRInput.RawTouch.RThumbRest);
                touchButtonPool.Add(OVRInput.RawTouch.A);
                touchButtonPool.Add(OVRInput.RawTouch.B);
                trigger = OVRInput.RawAxis1D.RHandTrigger;
                axis = new Vector3(0.1f, 0.5f, 0.4f);
                isThumb = true;
                break;
            case FingerType.R_Index:
                trigger = OVRInput.RawAxis1D.RIndexTrigger;
                triggerStart = 0.1f;
                relatedTouchButton = OVRInput.RawTouch.RIndexTrigger;
                axis = new Vector3(-0.1f, 1, 0);
                break;
            case FingerType.R_Middle:
                trigger = OVRInput.RawAxis1D.RHandTrigger;
                relatedTouchButton = OVRInput.RawTouch.RIndexTrigger;
                triggerStart = 0.95f;
                break;
            case FingerType.R_Ring:
                trigger = OVRInput.RawAxis1D.RHandTrigger;
                triggerStart = 0.1f;
                break;
            case FingerType.R_Pinky:
                trigger = OVRInput.RawAxis1D.RHandTrigger;
                axis = new Vector3(0, 1, 0.1f);
                break;
        }

        if (jointLevelPool.Count >= 3)
        {
            float[,] levels = {
                { 0.05f, 0.5f, 0.9f },  // 親指
                { 0.9f, 1.0f, 1.2f },   // 人差し指
                { 1.0f, 0.8f, 1.6f },   // 中指
                { 1.0f, 0.7f, 1.6f },   // 薬指
                { 1.0f, 0.7f, 1.6f }    // 小指
            };

            int fi = (int)fingerType;
            int levelType = (fi >= (int)FingerType.R_Thumb) ? fi - (int)FingerType.R_Thumb : fi - (int)FingerType.L_Thumb;
            jointLevelPool[0] = levels[levelType, 0];
            jointLevelPool[1] = levels[levelType, 1];
            jointLevelPool[2] = levels[levelType, 2];

        }
    }
}