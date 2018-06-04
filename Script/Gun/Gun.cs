using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class Gun : MonoBehaviour
{
    public float fFireRate;// 弾の間隔
    public float fSpread;// 拡散度
    public float fRange;// //射程 未実装
    public float fDamage;//ダメージ
    public float fBulletSpeed;// 弾の速度
    public enum ShotType// 銃の種類
    {
        Full,
        Semi,
        Burst　//未実装
    }
    public ShotType eShotType;// 銃の種類をいれる変数
    public float fReloadTime;// リロードにかかる時間
    public int clipSize;// 最大装填数
    public int ammoMax;// 弾の最大所持数
    [SerializeField]
    public int ammo;// 現在の装填数
    [SerializeField]
    public int ammoHave;// 現在の弾の所持数
    public int ammoUsep;// 一発あたりの消費弾数
    public int shotPerRound;// 発射弾数
    public GameObject Bullet;// 発射するオブジェクト
    private float fTimeS;// Shotのタイム
    private float fTimeR;// Reloadのタイム
    private bool isShot;// 弾を打っているかのフラグ
    private bool isReload;// リロードをするかのフラグ
    public bool isHitScan;// ヒットスキャンにするか
    private Quaternion initRotation;// 角度初期化する際にでも
    [SerializeField]
    public Transform tMuzzle;// 発射位置を指定
    public Transform tStartRay;// Rayの開始位置
    private int layer;// layerの距離
    public GameObject Shell;// 薬莢
    public Transform tShellOuter;// 薬莢の排出口
    public bool isShell; // 薬きょうを出すか
    private bool isShellPut = false;// 薬きょうを出すときに建てるフラグ
    public GameObject MuzzleFX;// 発射エフェクト                     
    ParticleSystem Psys;// パーティクルシステムの情報を入れる変数
    public bool isShotSE;// 弾の発射音のフラグ

    private bool isSE;// 弾の発射音のフラグ
    private bool isBulletEnd;//球切れ

    public AudioClip ShotSE;//弾を打ったときの音
    public AudioClip ShotEndSE;//打ち終わったときの音


    public void Init()// 変数に情報を入れる処理　初期化
    {
        ammo = clipSize;
        ammoHave = ammoMax;
    }

    public void ChoiceType(string input)// どのタイプの銃にするか
    {
        if (!isReload)
        {
            switch (eShotType)
            {
                case ShotType.Full:
                    Shot(Input.GetButton(input));
                    break;
                case ShotType.Semi:
                    Shot(Input.GetButtonDown(input));
                    break;
                case ShotType.Burst:
                    break;
            }
        }
    }

    public void Shot(bool input)//　ボタンを押したら弾を打つ
    {
        if (isShot)
        {
            fTimeS += Time.deltaTime;
            if (fTimeS > fFireRate)
            {

                isShot = false;
                fTimeS = 0;
            }

        }

        if (input && !isShot)
        {
            ShotElement();
        }
    }


    public void OVRShot(string INPUT)// ボタンを押したら　GetAxis
    {
        if (Input.GetAxis(INPUT) > 0.8f && !isShot == true)
        {
            ShotElement();
            if (INPUT == "RFinger")
            {
                GameObject.Find("OVRCameraRig").transform.GetComponent<Vibration>().R_VIBRATION(255);
            }
            else
            {
                GameObject.Find("OVRCameraRig").transform.GetComponent<Vibration>().L_VIBRATION(255);
            }
        }
    }



    public void Recast()// 弾の打つ間隔
    {
        if (isShot)
        {
            fTimeS += Time.deltaTime;
            if (fTimeS > fFireRate)
            {
                isShot = false;
                fTimeS = 0;
            }

        }

    }

    public void Reload(bool input)// リロードの処理
    {
        if (input)
        {
            if (!(ammo >= clipSize))
            {
                isReload = true;
            }
        }
        if (isReload)
        {

            fTimeR += Time.deltaTime;
            if (fTimeR >= fReloadTime)
            {
                if (ammoHave - (clipSize - ammo) <= 0)
                {
                    ammo += (ammoHave);
                    ammoHave = 0;
                }
                else
                {
                    ammoHave -= (clipSize - ammo);
                    ammo += (clipSize - ammo);
                }
                isReload = false;
                fTimeR = 0f;
            }
        }

    }

    private void ShotElement()// レイキャストでうつ弾の処理
    {
        isShot = true;
        if (ammo > 0)
        {
            for (int i = 0; i < shotPerRound; i++)
            {
                Vector3 diffusionVector;
                float angle_x = Random.Range(-fSpread, fSpread);
                float angle_y = Random.Range(-fSpread, fSpread);
                diffusionVector = new Vector3(angle_x, angle_y, 0);

                Ray ray = new Ray(tStartRay.position, tStartRay.forward + diffusionVector);
                RaycastHit hit;
                if (isHitScan)
                {
                    if (Physics.Raycast(ray, out hit, fRange, layer))
                    {
                        if (hit.transform.tag == "Enemy")
                        {
                            //  hit.collider.SendMessage("Damage", fDamage);
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    GameObject b = Instantiate(Bullet, tMuzzle.position, tMuzzle.rotation);
                    if (Physics.Raycast(ray, out hit, fRange, layer))
                    {
                        b.GetComponent<Rigidbody>().velocity = (hit.point - b.transform.position).normalized * fBulletSpeed;
                        b.SendMessage("BulletPower", fDamage);
                    }
                    else
                    {
                        b.GetComponent<Rigidbody>().velocity = (ray.GetPoint(fRange) - b.transform.position).normalized * fBulletSpeed;
                        b.SendMessage("BulletPower", fDamage);

                    }
                }
                //生成と同時に弾に移動速度を与える
                if (MuzzleFX != null)
                {
                    var fx = Instantiate(MuzzleFX, tMuzzle.position, tMuzzle.rotation) as GameObject;
                    fx.transform.parent = tMuzzle;
                    Destroy(fx, 1.0f);

                }
                isShellPut = true;
                if (isShellPut == true && isShell == true)
                {
                    Instantiate(Shell, tShellOuter.position, tShellOuter.rotation);
                    isShellPut = false;

                }

                isSE = true;
                //Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f, true);
                //transform.rotation = initRotation;
            }
            ammo -= ammoUsep;
        }
        else
        {
            isBulletEnd = true;
        }
        if (isShotSE == true)
        {
            Sound();
        }
    }

    public int GetAmmo()// 残りの残弾
    {
        return ammo;
    }

    public int GetAmmoHave()// 残り持っている残弾数
    {
        return ammoHave;
    }

    void Sound()
    {
        if (isSE == true)
        {
            GetComponent<AudioSource>().PlayOneShot(ShotSE);
            isSE = false;
        }
        else if (isBulletEnd == true)
        {
            GetComponent<AudioSource>().PlayOneShot(ShotEndSE);
            isBulletEnd = false;

        }

    }

}
