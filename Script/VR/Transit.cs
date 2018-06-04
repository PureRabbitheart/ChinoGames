using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transit : MonoBehaviour
{
    public Transform tStartPos;

    private int NowCount = 0;//索敵範囲の半径のカウントを数える
    private float fTime = 0.1f;//乗り移るときの移動時間
    private float startTime;//開始時間
    private bool isMoveAction;//乗り移っている状況
    private bool isAction;
    private Vector3 startPosition;//開始地点
    private Vector3 tEndPos;//ゴール
    private GameObject Soul;//乗り移る先
    private LineRenderer laser;
    private RaycastHit hit;


    [SerializeField]
    private GameObject[] System = new GameObject[2];//索敵範囲
    [SerializeField]
    private GameObject Sphere;//索敵範囲
    [SerializeField]
    private float fRadius = 1.0f;//半径
    [SerializeField]
    private GameObject Camera;//カメラオブジェクト
    [SerializeField]
    private int MaxCount;//最大半径
    [SerializeField]
    private LayerMask EnemyMask;
    [SerializeField]
    private LayerMask FloorMask;
    [SerializeField]
    private Animator SystemImageAnim;


    void Start()
    {
        laser = this.GetComponent<LineRenderer>();

        ParticleSystem ps = System[0].GetComponent<ParticleSystem>();
        ps.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {


        Sphere.transform.position = transform.position;
        if (isAction == false)
        {
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

            if (OVRInput.GetUp(OVRInput.RawButton.A) && Soul != null && isMoveAction == false)
            {
                ParticleSystem ps = System[0].GetComponent<ParticleSystem>();
                ps.GetComponent<Renderer>().enabled = true;

                isAction = true;
                Invoke("ChangeSouls", 0.2f);//もし触れているなら魂を変える
            }


        }


        if (isMoveAction == true)
        {
            MoveAction();
        }

        System[0].transform.position = Camera.transform.position;

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



        Camera.transform.parent = Soul.transform.root;//親子関係の移動
                                                      //Camera.transform.position = new Vector3(Soul.transform.root.position.x, Soul.transform.root.position.y + 0.6f, Soul.transform.root.position.z);//カメラの移動

        startTime = Time.timeSinceLevelLoad;
        startPosition = transform.position;

        Vector3 vTmpPos = Soul.transform.position;
        Ray ray = new Ray(Soul.transform.position, new Vector3(0, -1, 0));//下に向かってレイを飛ばす
        Debug.DrawRay(ray.origin, ray.direction * 40, Color.black);
        RaycastHit HitFloor;
        if (Physics.Raycast(ray, out HitFloor, 40.0f, FloorMask))//手からレイを飛ばす
        {
            tEndPos = new Vector3(Soul.transform.position.x, HitFloor.point.y + ((Soul.transform.localScale.y - 1) * 2), Soul.transform.position.z);
        }


        isMoveAction = true;

    }

    //右手から出るRayの処理
    void RayHit(Collider[] hitEnemy)
    {
        //WireFrame(hitEnemy);

        Ray ray = new Ray(tStartPos.position, -tStartPos.right);
        Debug.DrawRay(ray.origin, ray.direction * 40, Color.black);

        if (Physics.Raycast(ray, out hit, 40.0f, EnemyMask))//手からレイを飛ばす
        {
            for (int i = 0; i < hitEnemy.Length; i++)//円に触れているオブジェクト分
            {
                if (hitEnemy[i].transform == hit.transform)//円に触れているかつLineRendererに触れていたら
                {
                    laser.SetPosition(0, ray.origin);//始点
                    laser.SetPosition(1, hit.point);//終点
                    Soul = hit.transform.gameObject;
                    System[1].transform.position = hit.transform.position;

                }
            }
        }
        else
        {
            Soul = null;

            laser.SetPosition(0, ray.origin);
            laser.SetPosition(1, ray.origin + ray.direction * 40);
        }
    }

    void MoveAction()
    {
        float test = Vector3.Distance(transform.position, tEndPos);

        if (test < 10.0f)
        {
            SystemImageAnim.SetBool("isActivate", true);
        }

        float fnowTime = Time.timeSinceLevelLoad - startTime;//開始時間と今の時間を計算する
        if (fnowTime > fTime)//移動時間よりも経過時間が超えたら
        {
            Camera.transform.position = tEndPos;
            Arrival();
        }
        else if (Camera.transform.position == tEndPos)
        {

            Arrival();//最終地点についたら
        }



        float rate = fnowTime / fTime;//経過時間と移動時間の計算
        Camera.transform.position = Vector3.Lerp(startPosition, tEndPos, rate);



    }


    void Arrival()//到着後の処理
    {
        isMoveAction = false;


        Quaternion setQuat = new Quaternion();
        setQuat.eulerAngles = new Vector3(0, hit.transform.eulerAngles.y, 0);//Controllerの向きとアナログスティックの傾きを合わせる
        Camera.transform.rotation = setQuat;//回転を代入

        hit.transform.gameObject.SetActive(false);//敵を消す
        hit.transform.root.Find("VRModel").gameObject.SetActive(true);//自分用の敵を出す



        ParticleSystem ps = System[0].GetComponent<ParticleSystem>();
        ps.GetComponent<Renderer>().enabled = false;

        SystemImageAnim.SetBool("isActivate", false);
        isAction = false;

    }

    void WireFrame(Collider[] hitEnemy)
    {
        for (int i = 0; i < hitEnemy.Length; i++)//円に触れているオブジェクト分
        {
            EnemyMaterialManager p_EMM = hitEnemy[i].transform.root.GetComponent<EnemyMaterialManager>();
            if (p_EMM != null)
            {
                p_EMM.isWireFrame = true;
            }
        }
    }
}