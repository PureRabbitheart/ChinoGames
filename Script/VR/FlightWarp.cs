using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightWarp : MonoBehaviour
{
    [SerializeField]
    private Transform tCamera;//centerCameraの座標
    [SerializeField]
    private GameObject Sphere;//当たり判定用のオブジェクト

    private LineRenderer p_LineRenderer;
    private GameObject[] Coll = new GameObject[6];//玉
    private Transform WarpPos;//移動する場所
    private bool isMove;//移動するよ

    private float startTime;//開始時間
    private float fTime = 1.0f;//乗り移るときの移動時間
    private Vector3 vStartPos;
    bool isMovingWarp = false;
    private Vector3 tEndPos;//ゴール

    void Start()
    {
        InitInstance();
        p_LineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);//アナログスティックの入力
        if (stickL.x > 0.5f || stickL.x < -0.5f || stickL.y > 0.5f || stickL.y < -0.5f)//入力があったら
        {
            ColliderSet();
            LaserSet();
        }
        else
        {
            ColliderOut();
            Moving(stickL);
        }
    }

    void Moving(Vector2 stickL)
    {
        if (isMovingWarp == false && WarpPos != null && stickL.x <= 0.03f && stickL.x >= -0.03f && stickL.y <= 0.03f && stickL.y >= -0.03f)//アナログスティックをいじっていなくて移動になったら
        {
            startTime = Time.timeSinceLevelLoad;
            vStartPos = transform.root.position;
            tEndPos = WarpPos.position;
            isMovingWarp = true;
            WarpPos = null;

        }
        else if (isMovingWarp == true)
        {
            Warp();
        }

    }

    void Warp()
    {
        float fnowTime = Time.timeSinceLevelLoad - startTime;//開始時間と今の時間を計算する
        if (fnowTime > fTime)//移動時間よりも経過時間が超えたら
        {
            transform.root.position = tEndPos;
            WarpPos = null;
            isMovingWarp = false;

        }
        else if (transform.root.position == tEndPos)
        {
            transform.root.position = tEndPos;
            WarpPos = null;
            isMovingWarp = false;

        }

        float rate = fnowTime / fTime;//経過時間と移動時間の計算
        transform.root.position = Vector3.Lerp(vStartPos, tEndPos, rate);
    }

    void ColliderOut()//何もしていないとき
    {
        p_LineRenderer.enabled = false;

        for (int i = 0; i < Coll.Length; i++)
        {
            Coll[i].SetActive(false);
        }
    }

    void ColliderSet()//当たり判定の配置
    {
        p_LineRenderer.enabled = true;

        for (int i = 0; i < Coll.Length; i++)
        {
            Coll[i].SetActive(true);
        }

        Coll[0].transform.position = tCamera.position + new Vector3(0, 0, 5.0f);
        Coll[1].transform.position = tCamera.position + new Vector3(0, 0, -5.0f);
        Coll[2].transform.position = tCamera.position + new Vector3(0, 5.0f, 0);
        Coll[3].transform.position = tCamera.position + new Vector3(0, -5.0f, 0);
        Coll[4].transform.position = tCamera.position + new Vector3(5.0f, 0, 0);
        Coll[5].transform.position = tCamera.position + new Vector3(-5.0f, 0, 0);

    }

    void LaserSet()//Ray関係の処理
    {
        Ray ray = new Ray(transform.position, -transform.right);
        Debug.DrawRay(ray.origin, ray.direction * 10.0f, Color.black);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 10.0f))//手からレイを飛ばす
        {
            if (Hit.transform.tag == "FlightWarp")
            {
                WarpPos = Hit.transform;
            }
            else
            {
                WarpPos = null;
            }
        }
        else
        {
            WarpPos = null;
        }
        p_LineRenderer.SetPosition(0, ray.origin);
        p_LineRenderer.SetPosition(1, ray.origin + ray.direction * 40);

    }

    void InitInstance()//当たり判定の球を配列に入れる
    {
        for (int i = 0; i < 6; i++)
        {
            Coll[i] = Instantiate(Sphere, tCamera.position, Quaternion.identity);
        }
    }
}
