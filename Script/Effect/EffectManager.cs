using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    [SerializeField]
    private float fDestroyTime;


    [SerializeField]
    private ParticleSystem[] Psys;

    private bool isTime;

    void Start()
    {
        for(int i = 0;i <Psys.Length;i++)
        {
            Psys[i].Play();
        }

        if (fDestroyTime == 0.0f)
        {
            isTime = false;
        }

    }

    void Update()
    {
        if(isTime == true)
        {
            Destroy(gameObject, fDestroyTime);
        }
    }

    
    
}
