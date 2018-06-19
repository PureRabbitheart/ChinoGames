using System.Collections;
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

    private bool isGrab;


    void OnTriggerEnter(Collider other)
    {
        if (isGrab == true)
        {
            if (other.GetComponent<ExploderObject>())
            {
                other.GetComponent<ExploderObject>().ExplodeObject(other.gameObject);
            }

            if (other.tag == "MainCamera")
            {
                other.transform.root.GetComponent<EnemyManager>().Damage(fPunchPower);
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
        if (Input.GetAxis(buttonName) > 0.8f)
        {
            isGrab = true;
            PunchFx.SetActive(true);
        }
        else
        {
            PunchFx.SetActive(false);
            isGrab = false;
        }
    }

}
