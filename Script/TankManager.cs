using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Tank;
    [SerializeField]
    private GameObject Battery;
    [SerializeField, Range(0, 1)]
    private float fRotateSpeed;
    [SerializeField, Range(0, 200)]
    private float fRadius = 10.0f;
    [SerializeField]
    private GunStatus p_GunStatus;

    private Vector3 tmpTarget;
    private Rigidbody p_RigiTank;
    private Rigidbody p_RigiBattery;

    private float fShotTime = 0.0f;
    private float fSearchTime = 0.0f;

    void Start()
    {
        p_RigiTank = Tank.GetComponent<Rigidbody>();
        p_RigiBattery = Battery.GetComponent<Rigidbody>();
    }

    void Update()
    {
        fShotTime += Time.deltaTime;
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

        if (fShotTime > 5)
        {
            p_GunStatus.isEnemyShot = true;
            fShotTime = 0.0f;
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

    void Rotation(Vector3 vTarget)
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
}
