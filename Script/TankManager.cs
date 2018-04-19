using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Tank;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Tank.transform.Rotate(new Vector3(0, 0, 5));
    }
}
