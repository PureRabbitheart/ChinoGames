using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDevice : MonoBehaviour
{

    [SerializeField]
    private GameObject[] gPort = new GameObject[2];
    [SerializeField]
    private Transform[] tWarpPoint = new Transform[2];
    [SerializeField]
    private Animator[] p_Animator = new Animator[2];

    void Awake()
    {

        for (int i = 0; i < gPort.Length; i++)//Warpの数分回す
        {
            if (!gPort[i].GetComponent<CapsuleCollider>())//Colliderがついていなかったら
            {
                gPort[i].AddComponent<CapsuleCollider>();
            }
            gPort[i].GetComponent<CapsuleCollider>().isTrigger = true;


            if (!gPort[i].GetComponent<Rigidbody>())//Rigidbodyがついていなかったら
            {
                gPort[i].AddComponent<Rigidbody>();
            }
            gPort[i].GetComponent<Rigidbody>().useGravity = false;

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
        Debug.DrawLine(gPort[0].transform.position, gPort[1].transform.position, Color.white);
    }


    public Vector3 NextPos(string name)
    {
        if (gPort[0].name == name)
        {
            return tWarpPoint[1].position;
        }
        else if (gPort[1].name == name)
        {
            return tWarpPoint[0].position;
        }
        return new Vector3(0, 0, 0);

    }

    public void TeleportationAnim()
    {
        p_Animator[0].SetBool("isActivate", true);
        p_Animator[1].SetBool("isActivate", true);
    }

    public void TeleportationBadAnim()
    {
        p_Animator[0].SetBool("isActivate", false);
        p_Animator[1].SetBool("isActivate", false);
    }
}