using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    public bool isFrame;

    [SerializeField]
    private Transform[] tTarget;

    NavMeshAgent agent;
    private bool[] isTarget;
    private float fMove;
    private int nextTarget;



    void Start()
    {
        if (GetComponent<NavMeshAgent>() != null)
        {
            isTarget = new bool[tTarget.Length];
            nextTarget = Random.Range(0, tTarget.Length);
            isTarget[nextTarget] = true;
            agent = GetComponent<NavMeshAgent>();
        }


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
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMesh();
        }
    }


    void navMesh()
    {
        for (int i = 0; i < tTarget.Length; i++)
        {
            if (isTarget[i] == true)
            {
                fMove = Vector3.Distance(transform.position, tTarget[i].position);
                agent.SetDestination(tTarget[i].position);
            }
        }

        if (isTarget[nextTarget] == true && fMove < 5f)
        {
            for (int i = 0; i < tTarget.Length; i++)
            {
                isTarget[i] = false;
            }
            nextTarget = Random.Range(0, tTarget.Length);
            isTarget[nextTarget] = true;
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
}
