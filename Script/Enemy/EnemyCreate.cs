using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreate : MonoBehaviour
{


    [SerializeField]
    private float fLoopTime;//生成する時間 
    [SerializeField]
    private GameObject Enemy;

    private float fNowTime;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fNowTime += Time.deltaTime;
        if (fNowTime > fLoopTime)
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
            fNowTime = 0.0f;
        }
    }
}
