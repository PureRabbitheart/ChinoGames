using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRoomChange : MonoBehaviour
{

    [SerializeField]
    private RoomChange p_RoomChange;

    public void ActivateEnd()
    {
        GetComponent<Animator>().SetBool("isActivate", false);
        p_RoomChange.SceneChange();
    }

    public void BadActivateEnd()
    {
        GetComponent<Animator>().SetBool("isBadActivate", false);
    }

}
