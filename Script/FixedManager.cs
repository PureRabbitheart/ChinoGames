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
        Laser,
    }

    [SerializeField]
    _type eEnemyType;//敵のタイプ

    [SerializeField]
    private GameObject gTank;//タンク本体
    [SerializeField]
    private GameObject gBattery;//砲撃部分
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
                break;
            case _type.FixedMissile:
                FixedMissileInit();
                break;
            case _type.Fire:
                break;
            case _type.Gatling:
                GatlingInit();
                break;
            case _type.Laser:
                break;
            default:
                break;
        }
    }

    void Update()
    {
        switch (eEnemyType)
        {
            case _type.PerpetratedMissile:
                break;
            case _type.FixedMissile:
                FixedMissileUpdate();
                break;
            case _type.Fire:
                break;
            case _type.Gatling:
                GatlingUpdate();
                break;
            case _type.Laser:
                break;
            default:
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
            //if (p_Gun.isEnemy == false)
            //{
            //    p_Gun.strInput[0] = EditorGUILayout.TextField("左のコントローラーの名前", p_Gun.strInput[0]);
            //    p_Gun.strTagName[0] = EditorGUILayout.TextField("左手で持ったときのTag名", p_Gun.strTagName[0]);

            //    p_Gun.strInput[1] = EditorGUILayout.TextField("右のコントローラーの名前", p_Gun.strInput[1]);
            //    p_Gun.strTagName[1] = EditorGUILayout.TextField("右手で持ったときのTag名", p_Gun.strTagName[1]);

            //}
            //else
            //{
            //    p_Gun.isEnemyShot = EditorGUILayout.Toggle("敵用のトリガー", p_Gun.isEnemyShot);

            //}

            //p_Gun.fFireRate = EditorGUILayout.FloatField("弾の間隔", p_Gun.fFireRate);
            //EditorGUILayout.LabelField("拡散度");
            //p_Gun.fSpread = EditorGUILayout.Slider(p_Gun.fSpread, 0.0f, 1.0f);
            //p_Gun.fRange = EditorGUILayout.FloatField("射程", p_Gun.fRange);
            //p_Gun.fDamage = EditorGUILayout.FloatField("ダメージ", p_Gun.fDamage);
            //p_Gun.fBulletSpeed = EditorGUILayout.FloatField("弾の速度", p_Gun.fBulletSpeed);
            //p_Gun.eShotType = (ShotType)EditorGUILayout.EnumPopup("銃の種類", p_Gun.eShotType);
            //p_Gun.fFireRate = EditorGUILayout.FloatField("リロードにかかる時間", p_Gun.fFireRate);
            //p_Gun.clipSize = EditorGUILayout.IntField("最大装填数", p_Gun.clipSize);
            //p_Gun.ammoMax = EditorGUILayout.IntField("弾の最大所持数", p_Gun.ammoMax);
            //p_Gun.ammo = EditorGUILayout.IntField("現在の装填数", p_Gun.ammo);
            //p_Gun.ammoHave = EditorGUILayout.IntField("現在の弾の所持数", p_Gun.ammoHave);
            //p_Gun.ammoUsep = EditorGUILayout.IntField("一発あたりの消費弾数", p_Gun.ammoUsep);
            //p_Gun.shotPerRound = EditorGUILayout.IntField("発射弾数", p_Gun.shotPerRound);
            //p_Gun.Bullet = EditorGUILayout.ObjectField("発射する弾", p_Gun.Bullet, typeof(GameObject), true) as GameObject;
            //p_Gun.isHitScan = EditorGUILayout.Toggle("ヒットスキャンにするか", p_Gun.isHitScan);
            //p_Gun.tMuzzle = EditorGUILayout.ObjectField("発射位置を指定", p_Gun.tMuzzle, typeof(Transform), true) as Transform;
            //p_Gun.tStartRay = EditorGUILayout.ObjectField("Rayの開始位置", p_Gun.tStartRay, typeof(Transform), true) as Transform;

            //p_Gun.isShell = EditorGUILayout.Toggle("薬莢を出すか", p_Gun.isShell);
            //if (p_Gun.isShell == true)
            //{
            //    p_Gun.Shell = EditorGUILayout.ObjectField("薬莢", p_Gun.Shell, typeof(GameObject), true) as GameObject;
            //    p_Gun.tShellOuter = EditorGUILayout.ObjectField("薬莢の排出口", p_Gun.tShellOuter, typeof(Transform), true) as Transform;

            //}

            //p_Gun.MuzzleFX = EditorGUILayout.ObjectField("発射エフェクト", p_Gun.MuzzleFX, typeof(GameObject), true) as GameObject;
            //p_Gun.isShotSE = EditorGUILayout.Toggle("弾の発射音を鳴らすか", p_Gun.isShotSE);
            //if (p_Gun.isShotSE == true)
            //{
            //    p_Gun.ShotSE = EditorGUILayout.ObjectField("発射時の音", p_Gun.ShotSE, typeof(AudioClip), true) as AudioClip;
            //    p_Gun.ShotEndSE = EditorGUILayout.ObjectField("弾切れの音", p_Gun.ShotEndSE, typeof(AudioClip), true) as AudioClip;

            //}

        }
    }

#endif
}
