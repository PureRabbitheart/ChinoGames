using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenTransferDevice : MonoBehaviour
{

    TransferDevice p_TransferDevice;

    private float fNowTime;


    void OnTriggerEnter(Collider other)
    {
        fNowTime = 0.0f;
        p_TransferDevice.TeleportationAnim();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            fNowTime += Time.deltaTime;
            if (fNowTime > 3.2f)
            {
                other.transform.root.position = p_TransferDevice.NextPos(transform.name);
                fNowTime = -5.0f;
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        p_TransferDevice.TeleportationBadAnim();
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