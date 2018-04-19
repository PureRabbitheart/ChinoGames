using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Explosion;
    [SerializeField]
    private float fSpeed;

    private float Power;
    private Rigidbody p_Rigidbody;


    void OnCollisionEnter(Collision other)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        p_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()//移動の処理
    {
        p_Rigidbody.position += transform.forward * fSpeed;//worldの青軸を使って敵を動かす
    }

    void BulletPower(float Damage)//銃側から威力を授かる
    {
        Power = Damage;//威力をもらいました
    }
}
