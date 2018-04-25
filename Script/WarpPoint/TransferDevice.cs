using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDevice : MonoBehaviour
{

    [SerializeField]
    private GameObject[] gPort = new GameObject[2];

    public struct _warp
    {
        public bool isWarp;
        public string WarpName;
    }

    public _warp eWarpInfo = new _warp();

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
        if (eWarpInfo.isWarp == true)
        {
            for (int i = 0; i < gPort.Length; i++)//Warpの数分回す
            {
                if (gPort[i].name == eWarpInfo.WarpName)
                {

                }
            }
        }
    }
}
