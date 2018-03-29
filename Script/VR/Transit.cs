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
    private GameObject Sphere;//半径
    [SerializeField]
    private float fRadius = 1.0f;//半径
    [SerializeField]
    private GameObject Camera;
    [SerializeField]
    private int MaxCount;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private Animator p_Animator;
    [SerializeField]
    private Sonar p_Sonar;

    void Start()
    {
        laser = this.GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        Sphere.transform.position = transform.position;

        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            laser.enabled = true;
            Collider[] HitEnemy = HitJudge();//触れているオブジェクトをすべて返す
            RayHit(HitEnemy);//範囲内にいて更にレイで触れているオブジェクトを処理
        }
        else
        {
            laser.enabled = false;
            NowCount = 0;
            Sphere.transform.localScale = new Vector3(0, 0, 0);
        }
        if (OVRInput.GetUp(OVRInput.RawButton.A) && Soul != null)
        {
            if (p_Animator != null)
            {
                p_Animator.SetBool("isMove", true);
            }
            Invoke("ChangeSouls", 0.2f);//もし触れているなら魂を変える
        }
    }

    //触れているオブジェクトの判定
    Collider[] HitJudge()
    {
        Collider[] Hit;
        if (NowCount < MaxCount)//最大サイズ以下なら
        {
            Hit = Physics.OverlapSphere(transform.position, fRadius * NowCount);
            Sphere.transform.localScale = new Vector3(fRadius * NowCount, fRadius * NowCount, fRadius * NowCount);

            NowCount++;
        }
        else//最大のサイズになったら
        {
            Hit = Physics.OverlapSphere(transform.position, fRadius * MaxCount);
            Sphere.transform.localScale = new Vector3(fRadius * MaxCount, fRadius * MaxCount, fRadius * MaxCount);

        }
        return Hit;
    }

    //魂をうつす処理
    void ChangeSouls()
    {
        transform.root.Find("Model").gameObject.SetActive(true);//取り付いていた敵のモデルを出す
        transform.root.Find("VRModel").gameObject.SetActive(false);//取り付いていた自分用のモデルを消す
        hit.transform.gameObject.SetActive(false);//敵を消す
        hit.transform.root.Find("VRModel").gameObject.SetActive(true);//自分用の敵を出す
        Camera.transform.parent = Soul.transform.root;//親子関係の移動
        Camera.transform.position = new Vector3(Soul.transform.root.position.x, Soul.transform.root.position.y + 0.6f, Soul.transform.root.position.z);//カメラの移動
        if(p_Sonar != null)
        {
            p_Sonar.isPlay = true;
        }
        if (p_Animator != null)
        {
            p_Animator.SetBool("isMove", false);
        }
    }

    //右手から出るRayの処理
    void RayHit(Collider[] hitEnemy)
    {
        Ray ray = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(ray, out hit, 40.0f, mask))//手からレイを飛ばす
        {
            for (int i = 0; i < hitEnemy.Length; i++)//円に触れているオブジェクト分
            {
                if (hitEnemy[i].transform == hit.transform)//円に触れているかつLineRendererに触れていたら
                {
                    laser.SetPosition(0, transform.position);//始点
                    laser.SetPosition(1, hit.point);//終点
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
