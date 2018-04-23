using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    enum _type
    {
        Door1,
        Door2,
        Door3,
        Other,//その他
    }
    _type eDoorType;

    private Animator p_Animator;

    void Awake()
    {
        DoorJudgment();
    }
    void Start()
    {
        p_Animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        switch (eDoorType)
        {
            case _type.Door1:
                p_Animator.SetBool("①",true);
                break;
            case _type.Door2:
                p_Animator.SetBool("②", true);
                break;
            case _type.Door3:
                p_Animator.SetBool("③", true);
                Debug.Log(eDoorType);
                break;
            case _type.Other:
                break;
            default:
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Ope");

        switch (eDoorType)
        {
            case _type.Door1:
                p_Animator.SetBool("①", false);
                break;
            case _type.Door2:
                p_Animator.SetBool("②", false);
                break;
            case _type.Door3:
                p_Animator.SetBool("③", false);
                break;
            case _type.Other:
                break;
            default:
                break;
        }
    }


    void DoorJudgment()//どのドアを使っているか判定
    {
        if (gameObject.transform.Find("1").gameObject.activeSelf)
        {
            eDoorType = _type.Door1;
        }
        else if (gameObject.transform.Find("2").gameObject.activeSelf)
        {
            eDoorType = _type.Door2;
        }
        else if (gameObject.transform.Find("3").gameObject.activeSelf)
        {
            eDoorType = _type.Door3;
        }
        else
        {
            eDoorType = _type.Other;
        }
    }
}
