using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;//Editor拡張で使うためのusing
#endif


public class GunStatus : Gun
{
    [SerializeField]
    private string[] strInput = new string[2];//コントローラーのボタンの名前
    [SerializeField]
    private string[] strTagName = new string[2];// タグの名前管理
    private int LeftOrRight;// 0と1で左手か右手を決める
    [SerializeField]
    private bool isEnemy;// 敵に使うか自分に使うか
    public bool isEnemyShot;// 敵に使うか自分に使うか

    void Update()// 弾を打つ処理
    {
        if (isEnemy == false)//自分で使うなら
        {
            Hand_Judge();//手の判定を行う
            Recast();//ﾘｷｬｽﾄ
            if (!(gameObject.tag == "Untagged"))//武器にtagが入っていたら
            {
                OVRShot(strInput[LeftOrRight]);//その手に合ったトリガー名を引数で送る

            }
            Reload();
        }
        else//敵なら
        {
            Shot(isEnemyShot);//自動で弾を撃つ
            isEnemyShot = false;
        }
    }


    void Hand_Judge()// 手の判定
    {
        for (int i = 0; i < strTagName.Length; i++)//配列分ループ
        {
            if (gameObject.tag == strTagName[i])//どの手で持っているかの判定
            {
                LeftOrRight = i;//0が左手　1が右手
            }
        }
    }


    void Reload()// リロードの処理
    {

        if (LeftOrRight == 0)//左手なら
        {
            Reload(OVRInput.GetDown(OVRInput.RawButton.LThumbstick));//アナログスティックを押し込んだらリロード
        }
        else//右手なら
        {
            Reload(OVRInput.GetDown(OVRInput.RawButton.RThumbstick));//アナログスティックを押し込んだらリロード
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GunStatus))]
    public class CharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GunStatus p_Gun = target as GunStatus;

            p_Gun.isEnemy = EditorGUILayout.Toggle("敵用ですか？", p_Gun.isEnemy);

            if (p_Gun.isEnemy == false)
            {
                p_Gun.strInput[0] = EditorGUILayout.TextField("左のコントローラーの名前", p_Gun.strInput[0]);
                p_Gun.strTagName[0] = EditorGUILayout.TextField("左手で持ったときのTag名", p_Gun.strTagName[0]);

                p_Gun.strInput[1] = EditorGUILayout.TextField("右のコントローラーの名前", p_Gun.strInput[1]);
                p_Gun.strTagName[1] = EditorGUILayout.TextField("右手で持ったときのTag名", p_Gun.strTagName[1]);

            }
            else
            {
                p_Gun.isEnemyShot = EditorGUILayout.Toggle("敵用のトリガー", p_Gun.isEnemyShot);

            }

            p_Gun.fFireRate = EditorGUILayout.FloatField("弾の間隔", p_Gun.fFireRate);
            EditorGUILayout.LabelField("拡散度");
            p_Gun.fSpread = EditorGUILayout.Slider(p_Gun.fSpread, 0.0f, 1.0f);
            p_Gun.fRange = EditorGUILayout.FloatField("射程", p_Gun.fRange);
            p_Gun.fDamage = EditorGUILayout.FloatField("ダメージ", p_Gun.fDamage);
            p_Gun.fBulletSpeed = EditorGUILayout.FloatField("弾の速度", p_Gun.fBulletSpeed);
            p_Gun.eShotType = (ShotType)EditorGUILayout.EnumPopup("銃の種類", p_Gun.eShotType);
            p_Gun.fReloadTime = EditorGUILayout.FloatField("リロードにかかる時間", p_Gun.fReloadTime);
            p_Gun.clipSize = EditorGUILayout.IntField("最大装填数", p_Gun.clipSize);
            p_Gun.ammoMax = EditorGUILayout.IntField("弾の最大所持数", p_Gun.ammoMax);
            p_Gun.ammo = EditorGUILayout.IntField("現在の装填数", p_Gun.ammo);
            p_Gun.ammoHave = EditorGUILayout.IntField("現在の弾の所持数", p_Gun.ammoHave);
            p_Gun.ammoUsep = EditorGUILayout.IntField("一発あたりの消費弾数", p_Gun.ammoUsep);
            p_Gun.shotPerRound = EditorGUILayout.IntField("発射弾数", p_Gun.shotPerRound);
            p_Gun.Bullet = EditorGUILayout.ObjectField("発射する弾", p_Gun.Bullet, typeof(GameObject), true) as GameObject;
            p_Gun.isHitScan = EditorGUILayout.Toggle("ヒットスキャンにするか", p_Gun.isHitScan);
            p_Gun.tMuzzle = EditorGUILayout.ObjectField("発射位置を指定", p_Gun.tMuzzle, typeof(Transform), true) as Transform;
            p_Gun.tStartRay = EditorGUILayout.ObjectField("Rayの開始位置", p_Gun.tStartRay, typeof(Transform), true) as Transform;

            p_Gun.isShell = EditorGUILayout.Toggle("薬莢を出すか", p_Gun.isShell);
            if (p_Gun.isShell == true)
            {
                p_Gun.Shell = EditorGUILayout.ObjectField("薬莢", p_Gun.Shell, typeof(GameObject), true) as GameObject;
                p_Gun.tShellOuter = EditorGUILayout.ObjectField("薬莢の排出口", p_Gun.tShellOuter, typeof(Transform), true) as Transform;

            }

            p_Gun.MuzzleFX = EditorGUILayout.ObjectField("発射エフェクト", p_Gun.MuzzleFX, typeof(GameObject), true) as GameObject;
            p_Gun.isShotSE = EditorGUILayout.Toggle("弾の発射音を鳴らすか", p_Gun.isShotSE);
            if (p_Gun.isShotSE == true)
            {
                p_Gun.ShotSE = EditorGUILayout.ObjectField("発射時の音", p_Gun.ShotSE, typeof(AudioClip), true) as AudioClip;
                p_Gun.ShotEndSE = EditorGUILayout.ObjectField("弾切れの音", p_Gun.ShotEndSE, typeof(AudioClip), true) as AudioClip;

            }

        }
    }

#endif
}
