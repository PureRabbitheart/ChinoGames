using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transit : MonoBehaviour
{

    private LineRenderer laser;
    private RaycastHit hit;
    //private GameObject HitModel;//自分の本体
    //[SerializeField]
    //private GameObject Camera;

    public int MaxCount;
    private int NowCount = 0;

    void Start()
    {
        laser = this.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {


        //if (OVRInput.GetDown(OVRInput.RawButton.A) && HitModel != null)
        //{
        //    transform.root.Find("Model").gameObject.SetActive(true);
        //    transform.root.Find("VRModel").gameObject.SetActive(false);
        //    hit.transform.gameObject.SetActive(false);
        //    hit.transform.root.Find("VRModel").gameObject.SetActive(true);
        //    Camera.transform.parent = hit.transform.root;
        //    Camera.transform.position = new Vector3(hit.transform.root.position.x, hit.transform.root.position.y + 0.5f, hit.transform.root.position.z);
        //    HitModel = null;

        //}



        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            Collider[] HitEnemy;

            if (NowCount < MaxCount)//最大サイズ以下なら
            {
                HitEnemy = Physics.OverlapSphere(transform.position, 0.1f * NowCount);
                NowCount++;
            }
            else//最大のサイズになったら
            {
                HitEnemy = Physics.OverlapSphere(transform.position, 0.1f * MaxCount);
            }

            if (Physics.Raycast(transform.position, -transform.right, out hit) && hit.transform.tag == "EnemyHead")//手からレイを飛ばす
            {
                for(int i = 0; i< HitEnemy.Length;i++)//円に触れているオブジェクト分
                {
                    if(HitEnemy[i].transform == hit.transform)//円に触れているかつLineRendererに触れていたら
                    {
                        laser.SetPosition(0, transform.position);//始点
                        laser.SetPosition(1, hit.point);//終点
                        Debug.Log("Hit");
                        hit.transform.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
            else
            {
                laser.SetPosition(0, transform.position);
                laser.SetPosition(1, -transform.right * 30);
            }
        }
        else
        {
            NowCount = 0;
        }

    }
}
