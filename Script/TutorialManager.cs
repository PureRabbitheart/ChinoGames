using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    [SerializeField]
    private Animator p_DoorAnim;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            p_DoorAnim.SetTrigger("Open");
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
}
