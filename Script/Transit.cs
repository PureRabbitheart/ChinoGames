using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transit : MonoBehaviour
{

    LineRenderer laser;
    RaycastHit hit;
    public GameObject obj;
    public GameObject Camera;
    public LayerMask mask;
    // Use this for initialization
    void Start()
    {
        laser = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit) && hit.transform.tag == obj.transform.tag)
        {
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, hit.point);
            hit.collider.GetComponent<MeshRenderer>().material.color = Color.red;
            if(OVRInput.GetDown(OVRInput.RawButton.A))
            {
                Camera.SetActive(false);
                hit.transform.Find("Camera").gameObject.SetActive(true);
            }

        }
        else
        {
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, transform.forward * 100);
            obj.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
