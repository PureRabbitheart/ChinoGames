using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRote : MonoBehaviour
{

    [SerializeField]
    private float fRoteSpeed;
    private Rigidbody p_Rigidbody;
    private Transform Camera;


    // Use this for initialization
    void Start()
    {
        Camera = GameObject.Find("MainCamera").transform;
           p_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera != null)
        {
            Rotation();
        }
    }

    void Rotation()
    {
        p_Rigidbody.rotation = Quaternion.Slerp(p_Rigidbody.rotation, Quaternion.LookRotation(Camera.position - p_Rigidbody.position), fRoteSpeed);//
    }
}
