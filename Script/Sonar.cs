﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public bool isButton;
    public bool isPlay;
    private Renderer[] ObjectRenderers;
    private static readonly Vector4 GarbagePosition = new Vector4(-5000, -5000, -5000, -5000);
    private static int QueueSize = 20;
    private static Queue<Vector4> positionsQueue = new Queue<Vector4>(QueueSize);
    private static Queue<float> intensityQueue = new Queue<float>(QueueSize);
    private static bool NeedToInitQueues = true;
    private delegate void Delegate();
    private static Delegate RingDelegate;

    private void Start()
    {
        ObjectRenderers = GetComponentsInChildren<Renderer>();

        if (NeedToInitQueues)
        {
            NeedToInitQueues = false;
            for (int i = 0; i < QueueSize; i++)
            {
                positionsQueue.Enqueue(GarbagePosition);
                intensityQueue.Enqueue(-5000f);
            }
        }

        RingDelegate += SendSonarData;
    }
    public void StartSonarRing(Vector4 position, float intensity)
    {
        position.w = Time.time;
        positionsQueue.Dequeue();
        positionsQueue.Enqueue(position);

        intensityQueue.Dequeue();
        intensityQueue.Enqueue(intensity);

        RingDelegate();
    }

    private void SendSonarData()
    {
        foreach (Renderer r in ObjectRenderers)
        {
            r.material.SetVectorArray("_hitPts", positionsQueue.ToArray());
            r.material.SetFloatArray("_Intensity", intensityQueue.ToArray());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!isButton)
        {
            StartSonarRing(collision.contacts[0].point, collision.impulse.magnitude / 10.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyMaterialManager p_EMM = other.transform.root.GetComponent<EnemyMaterialManager>();

        if(p_EMM != null)
        {
            p_EMM.isWireFrame = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        EnemyMaterialManager p_EMM = other.transform.root.GetComponent<EnemyMaterialManager>();

        if (p_EMM != null)
        {
            p_EMM.isWireFrame = false;
        }
    }


    void Update()
    {
        //if(isButton)
        //{
        //    if (OVRInput.GetDown(OVRInput.RawButton.A))
        //    {
        //        StartSonarRing(transform.position, 3.0f);
        //    }

        //    if(isPlay == true)
        //    {
        //        isPlay = false;
        //        StartSonarRing(transform.position, 8.0f);

        //    }
        //}
    }

}
