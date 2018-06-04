using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarCollider : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sonar"&&transform.tag == "MainCamera")
        {     
            other.transform.root.GetComponent<EnemyMaterialManager>().isWireFrame = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Sonar" && transform.tag == "MainCamera")
        {
            other.transform.root.GetComponent<EnemyMaterialManager>().isWireFrame = false;
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
