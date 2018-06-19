using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTitleManager : MonoBehaviour
{
    [SerializeField]
    private float fMaxTime;
    [SerializeField]
    private TitleManager p_TitleManager;
    [SerializeField]
    private GameObject TeleportParticle;

    private float fNowTime;


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            fNowTime += Time.deltaTime;
            if (fNowTime > fMaxTime)
            {
                p_TitleManager.isTransition = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            fNowTime = 0;
            TeleportParticle.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            TeleportParticle.SetActive(false);
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
