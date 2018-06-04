using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTransit : MonoBehaviour
{
    Transit p_Transit;
    // Use this for initialization
    void Start()
    {
        p_Transit = GameObject.Find("RightHandAnchor").GetComponent<Transit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (p_Transit != null)
        {
            p_Transit.tStartPos = transform;
        }
    }
}
