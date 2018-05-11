using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenTransferDevice : MonoBehaviour
{

    TransferDevice p_TransferDevice;

    private float fNowTime;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("イリュージョン");
        fNowTime = 0.0f;
    }

    void OnTriggerStay(Collider other)
    {
        fNowTime += Time.deltaTime;
        if (fNowTime > 5)
        {
            other.transform.root.position = p_TransferDevice.NextPos(transform.name);
            fNowTime = 0.0f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        fNowTime = 0.0f;
    }

    void Awake()
    {
        p_TransferDevice = transform.parent.GetComponent<TransferDevice>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
