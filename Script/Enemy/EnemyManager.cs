using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public float HP;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (HP < 0)
        {
            Destroy(gameObject);
        }
    }

    void Damage(float Damage)
    {
        HP -= Damage;
    }
}
