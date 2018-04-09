using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Transit : MonoBehaviour
{
    private int NowCount = 0;//索敵範囲の半径のカウントを数える
    private float fTime = 0.5f;//乗り移るときの移動時間
    private float startTime;//開始時間
    private bool isMoveAction;//乗り移っている状況
    private bool isAction;
    private Vector3 tEndPos;//ゴール
    private Vector3 startPosition;//開始地点
    private GameObject Soul;//乗り移る先
    private LineRenderer laser;
    private RaycastHit hit;

    [SerializeField]
    private GameObject Sphere;//索敵範囲
    [SerializeField]
    private float fRadius = 1.0f;//半径
    [SerializeField]
    private GameObject Camera;//カメラオブジェクト
    [SerializeField]
    private int MaxCount;//最大半径
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private Image p_Image;
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
                if (p_Image != null)
                {
                    p_Image.color = new Vector4(255, 255, 255, 100);
                }
                isAction = true;
                Invoke("ChangeSouls", 0.2f);//もし触れているなら魂を変える
            }

        }



        if (isMoveAction == true)
        {
            MoveAction();
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


        Camera.transform.parent = Soul.transform.root;//親子関係の移動
        //Camera.transform.position = new Vector3(Soul.transform.root.position.x, Soul.transform.root.position.y + 0.6f, Soul.transform.root.position.z);//カメラの移動

        startTime = Time.timeSinceLevelLoad;
        startPosition = transform.position;
        tEndPos = new Vector3(Soul.transform.root.position.x, Soul.transform.root.position.y + 2f, Soul.transform.root.position.z);
        isMoveAction = true;


    }

    //右手から出るRayの処理
    void RayHit(Collider[] hitEnemy)
    {
        Ray ray = new Ray(transform.position, -transform.right);
        Debug.DrawRay(ray.origin, ray.direction * 40, Color.black);

        if (Physics.Raycast(ray, out hit, 40.0f, mask))//手からレイを飛ばす
        {
            for (int i = 0; i < hitEnemy.Length; i++)//円に触れているオブジェクト分
            {
                if (hitEnemy[i].transform == hit.transform)//円に触れているかつLineRendererに触れていたら
                {
                    laser.SetPosition(0, ray.origin);//始点
                    laser.SetPosition(1, hit.point);//終点
                    Soul = hit.transform.gameObject;
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
        isAction = false;

        Quaternion setQuat = new Quaternion();
        setQuat.eulerAngles = new Vector3(0, hit.transform.eulerAngles.y, 0);//Controllerの向きとアナログスティックの傾きを合わせる
        Camera.transform.rotation = setQuat;//回転を代入

        hit.transform.gameObject.SetActive(false);//敵を消す
        hit.transform.root.Find("VRModel").gameObject.SetActive(true);//自分用の敵を出す

        if (p_Sonar != null)
        {
            p_Sonar.isPlay = true;
        }
        if (p_Image != null)
        {
            p_Image.color = new Vector4(255, 255, 255, 0);
        }
    }
}
