using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDevice : MonoBehaviour
{

    [SerializeField]
    private GameObject[] gPort = new GameObject[2];

    void Awake()
    {

        for (int i = 0; i < gPort.Length; i++)//Warpの数分回す
        {
            if (!gPort[i].GetComponent<CapsuleCollider>())//Colliderがついていなかったら
            {
                gPort[i].AddComponent<CapsuleCollider>();
            }
            if (!gPort[i].GetComponent<Rigidbody>())//Rigidbodyがついていなかったら
            {
                gPort[i].AddComponent<Rigidbody>();
            }
            if (!gPort[i].GetComponent<ChildrenTransferDevice>())
            {
                gPort[i].AddComponent<ChildrenTransferDevice>();
            }

        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }


    public Vector3 NextPos(string name)
    {
        if (gPort[0].name == name)
        {
            return gPort[1].transform.position;
        }
        else if (gPort[1].name == name)
        {
            return gPort[0].transform.position;
        }
        return new Vector3(0, 0, 0);

    }
}
