using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTutorial : MonoBehaviour
{
    [SerializeField]
    private TutorialManager p_TutorialManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            p_TutorialManager.OpenDoor();
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
