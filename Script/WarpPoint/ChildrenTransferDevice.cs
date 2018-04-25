using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenTransferDevice : MonoBehaviour
{

    TransferDevice p_TransferDevice;

    void OnTriggerEnter(Collider other)
    {
        p_TransferDevice.eWarpInfo.isWarp = true;
        p_TransferDevice.eWarpInfo.WarpName = transform.name;
    }

    void OnTriggerExit(Collider other)
    {

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
