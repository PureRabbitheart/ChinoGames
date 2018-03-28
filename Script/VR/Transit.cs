using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transit : MonoBehaviour
{
    private GameObject Soul;
    private int NowCount = 0;
    private LineRenderer laser;
    private RaycastHit hit;
    [SerializeField]
    private float fRadius =1.0f;//半径
    [SerializeField]
    private GameObject Camera;
    [SerializeField]
    private int MaxCount;


    void Start()
    {
        laser = this.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            Collider[] HitEnemy = HitJudge();//触れているオブジェクトをすべて返す
            RayHit(HitEnemy);//範囲内にいて更にレイで触れているオブジェクトを処理
        }
        else
        {
            NowCount = 0;
        }

        ChangeSouls();//もし触れているなら魂を変える

    }

    Collider[] HitJudge()
    {
        Collider[] Hit;
        if (NowCount < MaxCount)//最大サイズ以下なら
        {
            Hit = Physics.OverlapSphere(transform.position, fRadius * NowCount);
            NowCount++;
        }
        else//最大のサイズになったら
        {
            Hit = Physics.OverlapSphere(transform.position, fRadius * MaxCount);
        }
        return Hit;
    }


    void ChangeSouls()
    {

        if (OVRInput.GetUp(OVRInput.RawButton.A) && Soul != null)
        {
            transform.root.Find("Model").gameObject.SetActive(true);
            transform.root.Find("VRModel").gameObject.SetActive(false);
            hit.transform.gameObject.SetActive(false);
            hit.transform.root.Find("VRModel").gameObject.SetActive(true);
            Camera.transform.parent = Soul.transform.root;
            Camera.transform.position = new Vector3(Soul.transform.root.position.x, Soul.transform.root.position.y + 0.6f, Soul.transform.root.position.z);
        }
    }

    void RayHit(Collider[] hitEnemy)
    {
        if (Physics.Raycast(transform.position, -transform.right, out hit) && hit.transform.tag == "MainCamera")//手からレイを飛ばす
        {
            for (int i = 0; i < hitEnemy.Length; i++)//円に触れているオブジェクト分
            {
                if (hitEnemy[i].transform == hit.transform)//円に触れているかつLineRendererに触れていたら
                {
                    laser.SetPosition(0, transform.position);//始点
                    laser.SetPosition(1, hit.point);//終点
                    //hit.transform.GetComponent<Renderer>().material.color = Color.red;
                    Soul = hit.transform.gameObject;
                }
            }
        }
        else
        {
            Soul = null;

            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, -transform.right * 30);
        }
    }

}
