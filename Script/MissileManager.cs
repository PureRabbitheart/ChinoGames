using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Explosion;

    void OnTriggerEnter(Collider other)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject.transform.root);
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
