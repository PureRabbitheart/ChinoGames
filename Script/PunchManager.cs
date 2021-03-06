﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exploder;

public class PunchManager : MonoBehaviour
{
    [SerializeField]
    private float fPunchPower;
    [SerializeField]
    enum BUTTON
    {
        RTrigger,
        LTrigger,
    }
    [SerializeField]
    private BUTTON eButtonType;
    [SerializeField]
    private GameObject PunchFx;
    [SerializeField]
    private GameObject PunchHitFx;
    private bool isGrab;


    void OnTriggerEnter(Collider other)
    {
        if (isGrab == true)
        {
            if (other.GetComponent<ExploderObject>())
            {
                other.GetComponent<ExploderObject>().ExplodeObject(other.gameObject);
            }

            if (other.tag == "EnemyModel")
            {
                other.transform.root.GetComponent<EnemyManager>().Damage(fPunchPower);
                GameObject HitFx = Instantiate(PunchHitFx, transform.position, Quaternion.identity);
                Destroy(HitFx, 1.0f);
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
        GrabPunch(eButtonType.ToString());
    }


    void GrabPunch(string buttonName)
    {
        if (Input.GetAxis(buttonName) > 0.5f)
        {
            isGrab = true;
            if (PunchFx.activeSelf == false)
            {
                PunchFx.SetActive(true);
            }
        }
        else
        {
            if (PunchFx.activeSelf == true)
            {
                PunchFx.SetActive(false);
            }
            isGrab = false;
        }
    }

}
