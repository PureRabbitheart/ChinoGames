using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.PostProcessing;


public class PostProccessingHandler : MonoBehaviour
{

    [SerializeField]
    PostProcessingProfile profile;

    // Use this for initialization
    void Start()
    {
        //  チェックのON/OFFっぽい
        profile.motionBlur.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
