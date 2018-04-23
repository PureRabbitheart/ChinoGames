using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;//Editor拡張で使うためのusing
#endif

public class FixedManager : MonoBehaviour
{

    enum _type
    {
        PerpetratedMissile,//遊撃ミサイル
        FixedMissile,//まっすぐに固定ミサイル打つやつ
        Fire,//火炎放射
        Gatling,//ガトリング
        Laser,//レーザー
    }

    [SerializeField]
    _type eEnemyType;//敵のタイプ

    [SerializeField]
    private GameObject gTank;//タンク本体
    [SerializeField]
    private GameObject gBattery;//砲撃部分
    [SerializeField]
    private ParticleSystem PsysFire;//炎
    [SerializeField]
    private ParticleSystem PsysSmoke;//煙
    [SerializeField]
    private float fRotateSpeed;//回転スピード
    [SerializeField]
    private float fRadius;//索敵の半径
    [SerializeField]
    private float fBatteSpeed;//砲台部分の回転スピード

    private GunStatus p_GunStatus;//銃のScript参照
    private Vector3 tmpTarget;
    private Rigidbody p_RigiTank;
    private Rigidbody p_RigiBattery;

    private float fSearchTime = 0.0f;

    void Start()
    {
        switch (eEnemyType)
        {
            case _type.PerpetratedMissile:
                LockMissileInit();
                break;
            case _type.FixedMissile:
                FixedMissileInit();
                break;
            case _type.Fire:
                FireInit();
                break;
            case _type.Gatling:
                GatlingInit();
                break;
            case _type.Laser:
                break;
        }
    }

    void Update()
    {
        switch (eEnemyType)
        {
            case _type.PerpetratedMissile:
                LockMissileUpdate();
                break;
            case _type.FixedMissile:
                FixedMissileUpdate();
                break;
            case _type.Fire:
                FireUpdate();
                break;
            case _type.Gatling:
                GatlingUpdate();
                break;
            case _type.Laser:
                break;

        }
    }


    void GatlingInit()//ガトリングの初期化
    {
        p_RigiTank = gTank.GetComponent<Rigidbody>();
        p_GunStatus = GetComponent<GunStatus>();
    }

    void GatlingUpdate()//ガトリングのUpdate
    {
        TankAim(FieldSearch());
    }

    void TankAim(Vector3 vTarget)//特定の方向に向く
    {
        if (vTarget == new Vector3(0, 0, 0))
        {
            p_GunStatus.isEnemyShot = false;
        }
        else
        {
            p_GunStatus.isEnemyShot = true;
            p_RigiTank.rotation = Quaternion.Slerp(p_RigiTank.rotation, Quaternion.LookRotation(vTarget - p_RigiTank.position), fRotateSpeed);//ターゲットの方向を向く
            GatlingRote();
        }
    }

    void GatlingRote()//ガトリングの回転処理
    {
        gBattery.transform.Rotate(new Vector3(0, 0, fBatteSpeed));
    }

    void FixedMissileInit()//固定ミサイルの初期化
    {
        p_RigiTank = gTank.GetComponent<Rigidbody>();
        p_RigiBattery = gBattery.GetComponent<Rigidbody>();
        p_GunStatus = GetComponent<GunStatus>();
        p_GunStatus.isEnemyShot = true;

    }

    void FixedMissileUpdate()//固定ミサイルのUpdate
    {
        fSearchTime += Time.deltaTime;
        float fTime = 10;
        if (fSearchTime > fTime)
        {
            tmpTarget = FieldSearch();
            if (fSearchTime > fTime + 5)
            {
                fSearchTime = 0.0f;
            }
        }
        Rotation(tmpTarget);
    }

    Vector3 FieldSearch()//範囲にいるプレイヤーを探す
    {
        Collider[] Hit;
        Hit = Physics.OverlapSphere(transform.position, fRadius);//半径の大きさ分サーチする

        for (int i = 0; i < Hit.Length; i++)//Colliderがあるオブジェクト分回す
        {
            if (Hit[i].tag == "Player")//Playerのタグがあるやつを戻り値で返す
            {
                return Hit[i].transform.position;
            }
        }
        return new Vector3(0, 0, 0);
    }

    void Rotation(Vector3 vTarget)//固定ミサイルの回転
    {

        TankRot(vTarget);
        BatteryRot(vTarget);

    }

    void TankRot(Vector3 tTarget)//本体の回転処理
    {
        Vector3 vTank = new Vector3(p_RigiTank.position.x, p_RigiTank.position.y, p_RigiTank.position.z);
        Vector3 vTarget = new Vector3(tTarget.x, p_RigiTank.position.y, tTarget.z);
        p_RigiTank.rotation = Quaternion.Slerp(p_RigiTank.rotation, Quaternion.LookRotation(vTarget - vTank), fRotateSpeed);//ターゲットの方向を向く
    }

    void BatteryRot(Vector3 tTarget)//砲台の回転処理
    {
        Vector3 vTank = new Vector3(p_RigiBattery.position.x, p_RigiBattery.position.y, p_RigiBattery.position.z);
        Vector3 vTarget = new Vector3(p_RigiBattery.position.x, tTarget.y, p_RigiBattery.position.z);
        p_RigiBattery.rotation = Quaternion.Slerp(p_RigiBattery.rotation, Quaternion.LookRotation(tTarget - p_RigiBattery.position), fRotateSpeed);//ターゲットの方向を向く
    }

    void FireInit()//火炎放射の初期化
    {
        p_RigiTank = gTank.GetComponent<Rigidbody>();
        p_GunStatus = GetComponent<GunStatus>();

    }

    void FireUpdate()//火炎放射のアップデート
    {
        FireShot();
        TankAim(FieldSearch());

    }

    void FireShot()//炎を出す
    {
        PsysFire.Play();
        PsysSmoke.Play();
    }

    void LockMissileInit()//ロックオンミサイルの初期化
    {
        p_RigiTank = gTank.GetComponent<Rigidbody>();
        p_RigiBattery = gBattery.GetComponent<Rigidbody>();
        p_GunStatus = GetComponent<GunStatus>();
        p_GunStatus.isEnemyShot = true;
    }

    void LockMissileUpdate()//ロックオンミサイルのアップデート
    {
        fSearchTime += Time.deltaTime;
        float fTime = 10;
        if (fSearchTime > fTime)
        {
            tmpTarget = FieldSearch();
            if (fSearchTime > fTime + 5)
            {
                fSearchTime = 0.0f;
            }
        }
        Rotation(tmpTarget);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FixedManager))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FixedManager p_FixedManager = target as FixedManager;

            p_FixedManager.eEnemyType = (_type)EditorGUILayout.EnumPopup("敵の攻撃タイプ", p_FixedManager.eEnemyType);


            if (p_FixedManager.eEnemyType == _type.FixedMissile)//固定ミサイルなら
            {
                p_FixedManager.gTank = EditorGUILayout.ObjectField("横回転するTank部分", p_FixedManager.gTank, typeof(GameObject), true) as GameObject;
                p_FixedManager.gBattery = EditorGUILayout.ObjectField("縦回転する砲台部分", p_FixedManager.gBattery, typeof(GameObject), true) as GameObject;
                EditorGUILayout.LabelField("回転スピード");
                p_FixedManager.fRotateSpeed = EditorGUILayout.Slider(p_FixedManager.fRotateSpeed, 0.0f, 1.0f);
                EditorGUILayout.LabelField("Searchの半径");
                p_FixedManager.fRadius = EditorGUILayout.Slider(p_FixedManager.fRadius, 0.0f, 500.0f);

            }
            else if (p_FixedManager.eEnemyType == _type.Gatling)
            {
                p_FixedManager.gTank = EditorGUILayout.ObjectField("本体部分", p_FixedManager.gTank, typeof(GameObject), true) as GameObject;
                p_FixedManager.gBattery = EditorGUILayout.ObjectField("砲撃部分", p_FixedManager.gBattery, typeof(GameObject), true) as GameObject;
                EditorGUILayout.LabelField("本体の回転スピード");
                p_FixedManager.fRotateSpeed = EditorGUILayout.Slider(p_FixedManager.fRotateSpeed, 0.0f, 1.0f);
                EditorGUILayout.LabelField("砲台部分の回転スピード");
                p_FixedManager.fBatteSpeed = EditorGUILayout.Slider(p_FixedManager.fBatteSpeed, 0.0f, 100.0f);
                EditorGUILayout.LabelField("Searchの半径");
                p_FixedManager.fRadius = EditorGUILayout.Slider(p_FixedManager.fRadius, 0.0f, 500.0f);
            }
            else if(p_FixedManager.eEnemyType == _type.Fire)
            {
                p_FixedManager.gTank = EditorGUILayout.ObjectField("本体部分", p_FixedManager.gTank, typeof(GameObject), true) as GameObject;
                p_FixedManager.gBattery = EditorGUILayout.ObjectField("砲撃部分", p_FixedManager.gBattery, typeof(GameObject), true) as GameObject;
                p_FixedManager.PsysFire = EditorGUILayout.ObjectField("炎のエフェクト", p_FixedManager.PsysFire, typeof(ParticleSystem), true) as ParticleSystem;
                p_FixedManager.PsysSmoke = EditorGUILayout.ObjectField("煙のエフェクト", p_FixedManager.PsysSmoke, typeof(ParticleSystem), true) as ParticleSystem;

                EditorGUILayout.LabelField("本体の回転スピード");
                p_FixedManager.fRotateSpeed = EditorGUILayout.Slider(p_FixedManager.fRotateSpeed, 0.0f, 1.0f);
                EditorGUILayout.LabelField("Searchの半径");
                p_FixedManager.fRadius = EditorGUILayout.Slider(p_FixedManager.fRadius, 0.0f, 500.0f);
            }
            else if (p_FixedManager.eEnemyType == _type.PerpetratedMissile)
            {
                p_FixedManager.gTank = EditorGUILayout.ObjectField("横回転するTank部分", p_FixedManager.gTank, typeof(GameObject), true) as GameObject;
                p_FixedManager.gBattery = EditorGUILayout.ObjectField("縦回転する砲台部分", p_FixedManager.gBattery, typeof(GameObject), true) as GameObject;
                EditorGUILayout.LabelField("回転スピード");
                p_FixedManager.fRotateSpeed = EditorGUILayout.Slider(p_FixedManager.fRotateSpeed, 0.0f, 1.0f);
                EditorGUILayout.LabelField("Searchの半径");
                p_FixedManager.fRadius = EditorGUILayout.Slider(p_FixedManager.fRadius, 0.0f, 500.0f);

            }
        }
    }

#endif
}
