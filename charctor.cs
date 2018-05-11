using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charctor : MonoBehaviour
{
    public float fSpeed;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, fSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -fSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(fSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-fSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.T))
        {
            transform.Rotate(50 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.G))
        {
            transform.Rotate(-50 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.H))
        {
            transform.Rotate(0, -50 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0, 50 * Time.deltaTime, 0);
        }
    }
}
