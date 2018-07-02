﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public bool isFrame;

    [SerializeField]
    private GameObject Boss;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag =="Player")
        {
            Boss.SetActive(true);
            Boss.GetComponent<Animator>().SetBool("PVAnim", true);
        }
    }


    void Start()
    {



    }

    void Update()
    {
        if (GetComponent<SkinnedMeshRenderer>() != null)
        {
            SkinFrame();
        }
        else if (GetComponent<MeshFilter>() != null)
        {
            Frame();
        }
    }




    void Frame()
    {
        if (isFrame == true)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);
        }
        else
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Triangles, 0);
        }

    }

    void SkinFrame()
    {
        if (isFrame == true)
        {
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.SetIndices(smr.sharedMesh.GetIndices(0), MeshTopology.LineStrip, 0);
        }
        else
        {
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.SetIndices(smr.sharedMesh.GetIndices(0), MeshTopology.Triangles, 0);
        }
    }

    public void Hit()
    {

    }
    public void FootR()
    {

    }
    public void FootL()
    {

    }
}
