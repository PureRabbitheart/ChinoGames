using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaterialManager : MonoBehaviour
{
    public bool isWireFrame;
    [SerializeField]
    private GameObject ModelParent;
    [SerializeField]
    private Material mNormal;
    [SerializeField]
    private Material mFrame;
    [SerializeField]
    private GameObject Light;

    private float fMateAnim;
    private bool isStart;
    [SerializeField]
    private List<GameObject> RenderMeshList = new List<GameObject>();

    void Awake()
    {

        GetChildren(ModelParent, ref RenderMeshList);
    }

    // Use this for initialization
    void Start()
    {
        fMateAnim = 0.2f;
        isStart = true;
        Light.SetActive(false);




        for (int i = 0; i < RenderMeshList.Count; i++)
        {
            RenderMeshList[i].GetComponent<Renderer>().material.SetFloat("_CutOff", fMateAnim);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart == true)
        {
            StartMaterialAnim();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.M) || isWireFrame == true)
            {
                for (int i = 0; i < RenderMeshList.Count; i++)
                {
                    FrameUpdate(RenderMeshList[i], true);
                    RenderMeshList[i].GetComponent<Renderer>().material = mFrame;
                }
                Light.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.M) || isWireFrame == false)
            {
                for (int i = 0; i < RenderMeshList.Count; i++)
                {
                    FrameUpdate(RenderMeshList[i], false);
                    RenderMeshList[i].GetComponent<Renderer>().material = mNormal;

                }
                Light.SetActive(false);

            }
        }

    }

    void StartMaterialAnim()
    {
        if (fMateAnim >= 0.0f)
        {
            for (int i = 0; i < RenderMeshList.Count; i++)
            {
                RenderMeshList[i].GetComponent<Renderer>().material.SetFloat("_CutOff", fMateAnim);
            }
            fMateAnim -= 0.002f;
        }
        else
        {
            isStart = false;
            for (int i = 0; i < RenderMeshList.Count; i++)
            {
                RenderMeshList[i].GetComponent<Renderer>().material = mNormal;
            }
        }

    }//燃えて出てくるアニメーション

    void FrameUpdate(GameObject obj, bool isFrame)
    {
        if (obj.GetComponent<SkinnedMeshRenderer>() != null)//スキンメッシュなら
        {
            SkinFrame(obj, isFrame);
        }
        else if (obj.GetComponent<MeshFilter>() != null)//メッシュなら
        {
            Frame(obj, isFrame);
        }
    }


    void Frame(GameObject obj, bool isFrame)
    {
        if (isFrame == true)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);
        }
        else
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Triangles, 0);
        }

    }

    void SkinFrame(GameObject obj, bool isFrame)
    {
        if (isFrame == true)
        {
            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.SetIndices(smr.sharedMesh.GetIndices(0), MeshTopology.LineStrip, 0);
        }
        else
        {
            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh.SetIndices(smr.sharedMesh.GetIndices(0), MeshTopology.Triangles, 0);
        }
    }

    void GetChildren(GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            if (ob.gameObject.GetComponent<Renderer>())
            {
                allChildren.Add(ob.gameObject);
            }
            GetChildren(ob.gameObject, ref allChildren);
        }
    }
}
