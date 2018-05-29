using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IKManager : MonoBehaviour
{
    public string Name;

    void Start()
    {

    }

    void Update()
    {
        transform.position = GameObject.Find(Name).transform.position;
        transform.rotation = GameObject.Find(Name).transform.rotation;
    }
}

