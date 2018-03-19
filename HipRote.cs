using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipRote : MonoBehaviour
{

    public GameObject Head;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       // if (Head.transform.rotation.x > 0.4f || Head.transform.rotation.x < -0.4f)
        {
            transform.rotation = new Quaternion(Head.transform.rotation.x / 2, Head.transform.rotation.y, Head.transform.rotation.z, Head.transform.rotation.w);
        }
       // Debug.Log(Head.transform.rotation.x);
    }
}
