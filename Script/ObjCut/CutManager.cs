using UnityEngine;
using System.Collections;

public class CutManager : MonoBehaviour
{

    public Material capMaterial;
    public GameObject Effect;
    public float fTimeEnd;
    private float fNowTime;
    private bool isCut = true;
    GameObject[] CutObj = new GameObject[2];

    void OnTriggerEnter(Collider other)
    {
        if (isCut == true)
        {
            if (other.tag == "CutObject")
            {
                CutObjcet(other);//モデルのカット処理
                isCut = false;
            }
            else if (other.tag == "EnemyHead")
            {
                CutObjcet(other);//モデルのカット処理
                if (other.tag == "EnemyHead")
                {
                    other.transform.root.gameObject.SendMessage("Damage", 100);
                }
                isCut = false;
                GameObject Fx = Instantiate(Effect, other.transform.position, Quaternion.identity);
                Destroy(Fx, 1f);

            }
        }
    }

    void CutMesh()
    {
        if (CutObj[0] != null && CutObj[1] != null)
        {
            CutObj[0].AddComponent<MeshCollider>();
            MeshCollider LeftObj = CutObj[0].GetComponent<MeshCollider>();//コンポーネントする
            LeftObj.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
            LeftObj.convex = true;//使えるようにする
            CutObj[0].tag = "CutObject";
            CutObj[0].GetComponent<Rigidbody>().useGravity = true;

            if (!CutObj[1].GetComponent<Rigidbody>())//Rigidbodyがなければ
            {
                CutObj[1].AddComponent<Rigidbody>();//コンポーネントする
                CutObj[1].tag = "CutObject";
            }

            if (!CutObj[1].GetComponent<MeshCollider>())
            {
                MeshCollider RightObj = CutObj[1].AddComponent<MeshCollider>();//コンポーネントする
                RightObj.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
                RightObj.convex = true;//使えるようにする
            }

            Destroy(CutObj[0], 5f);
            Destroy(CutObj[1], 5f);
        }

    }

    // Use this for initialization
    void Start()
    {


    }

    void Update()
    {
        if (isCut == false)
        {
            fNowTime += Time.deltaTime;
            if (fNowTime > fTimeEnd)
            {
                isCut = true;
            }
        }
        else
        {
            fNowTime = 0.0f;
        }


    }


    void CutObjcet(Collider other)
    {
        GameObject.Find("OVRCameraRig").transform.GetComponent<Vibration>().R_VIBRATION(255);
        GameObject.Find("OVRCameraRig").transform.GetComponent<Vibration>().L_VIBRATION(255);

        GameObject victim = other.gameObject;//触れたオブジェクトの情報を入れる
        GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);//触れた角度場所から計算してカッティングする
        CutObj = pieces;


        if (pieces[0].GetComponent<BoxCollider>())//BoxColliderならコンポーネントを切る
        {
            Destroy(pieces[0].GetComponent<BoxCollider>());
        }
        else if (pieces[0].GetComponent<MeshCollider>())//MeshColliderならコンポーネントを切る
        {
            Destroy(pieces[0].GetComponent<MeshCollider>());
        }
        else if (pieces[0].GetComponent<CapsuleCollider>())//MeshColliderならコンポーネントを切る
        {
            Destroy(pieces[0].GetComponent<CapsuleCollider>());
        }
        else if (pieces[0].GetComponent<SphereCollider>())//MeshColliderならコンポーネントを切る
        {
            Destroy(pieces[0].GetComponent<SphereCollider>());
        }

        if (!pieces[0].GetComponent<Rigidbody>())
        {
            pieces[0].AddComponent<Rigidbody>();
        }

        Invoke("CutMesh", 0.05f);
    }
}
