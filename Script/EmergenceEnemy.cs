using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergenceEnemy : MonoBehaviour
{

    public GameObject Enemy;
    [SerializeField]
    private Transform CreatePos;
    [SerializeField]
    private float fCreateTime;

    // Use this for initialization
    void Start()
    {
        Invoke("CreateEnmey", fCreateTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateEnmey()
    {
        Instantiate(Enemy, CreatePos.position, Quaternion.identity);
        Destroy(gameObject, 1.0f);
    }
}
