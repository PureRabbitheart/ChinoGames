using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transit : MonoBehaviour
{

    private LineRenderer laser;
    private RaycastHit hit;
    private GameObject HitModel;//自分の本体
    [SerializeField]
    private GameObject Camera;


    void Start()
    {
        laser = this.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit) && hit.transform.tag == "MainCamera")
        {
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, hit.point);
            hit.collider.GetComponent<MeshRenderer>().material.color = Color.red;
            HitModel = hit.transform.gameObject;
        }
        else
        {
            HitModel = null;
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, transform.forward * 100);
        }

        if (OVRInput.GetDown(OVRInput.RawButton.A) && HitModel != null)
        {
            transform.root.Find("Model").gameObject.SetActive(true);
            transform.root.Find("VRModel").gameObject.SetActive(false);
            hit.transform.gameObject.SetActive(false);
            hit.transform.root.Find("VRModel").gameObject.SetActive(true);
            Camera.transform.parent = hit.transform.root;
            Camera.transform.position = new Vector3(hit.transform.root.position.x, hit.transform.root.position.y + 0.5f, hit.transform.root.position.z);
            HitModel = null;

        }
    }
}
