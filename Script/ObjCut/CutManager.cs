using UnityEngine;
using System.Collections;

public class CutManager : MonoBehaviour
{

    public Material capMaterial;
    public float fTimeEnd;
    private float fNowTime;
    private bool isCut = true;

    void OnTriggerEnter(Collider other)
    {
        if (isCut == true)
        {
            if (other.tag == "EnemyHead")
            {
                GameObject victim = other.gameObject;//触れたオブジェクトの情報を入れる
                GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);//触れた角度場所から計算してカッティングする
                if (!pieces[0].GetComponent<BoxCollider>())//BoxColliderならコンポーネントを切る
                {
                    Destroy(pieces[0].GetComponent<BoxCollider>());
                }
                else if(!pieces[0].GetComponent<MeshCollider>())//MeshColliderならコンポーネントを切る
                {
                    Destroy(pieces[0].GetComponent<MeshCollider>());
                }
                else if (!pieces[0].GetComponent<CapsuleCollider>())//MeshColliderならコンポーネントを切る
                {
                    Destroy(pieces[0].GetComponent<CapsuleCollider>());
                }
                else if (!pieces[0].GetComponent<SphereCollider>())//MeshColliderならコンポーネントを切る
                {
                    Destroy(pieces[0].GetComponent<SphereCollider>());
                }

                if (!pieces[0].GetComponent<MeshCollider>())
                {
                    pieces[0].AddComponent<MeshCollider>();//元々あったやつにMeshColliderをコンポーネントする
                    pieces[0].GetComponent<MeshCollider>().convex = true;//使えるようにする

                }

                if (!pieces[1].GetComponent<Rigidbody>())//Rigidbodyがなければ
                {
                    pieces[1].AddComponent<Rigidbody>();//コンポーネントする
                       pieces[1].tag = "EnemyHead";
                }
                if(!pieces[1].GetComponent<MeshCollider>())
                {
                    pieces[1].AddComponent<MeshCollider>();//コンポーネントする
                    pieces[1].GetComponent<MeshCollider>().convex = true;//使えるようにする

                }
                Debug.Log(pieces.Length);
                isCut = false;
            }
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
    void Cut()
    {

    }

    /*void OnDrawGizmosSelected() {

		Gizmos.color = Color.green;

		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position + transform.up * 0.5f + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + transform.forward * 5.0f);

		Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
		Gizmos.DrawLine(transform.position,  transform.position + -transform.up * 0.5f);

	}*/

}
