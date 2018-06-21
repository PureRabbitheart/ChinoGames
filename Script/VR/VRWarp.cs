using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRWarp : MonoBehaviour
{
    
    private LineRenderer laser;
    private RaycastHit hit;
    private List<Vector3> vArrow = new List<Vector3>();
    private bool isMove;//移動中か
    private float Gravity = 9.81f;//重力
    private Animator p_Animator;

    [SerializeField]
    private LineRenderer p_LineRenderer;
    [SerializeField]
    private GameObject targetMarker;
    [SerializeField]
    private float vertexCount = 30;
    [SerializeField]
    private float fHeight;//ヘッドマウントの高さ
    [SerializeField]
    private float initialVelocity = 10;//長さ距離
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private LayerMask KeepOut;
    [SerializeField]
    private Material MoveMaterial;
    [SerializeField]
    private Material OutMaterial;

    void Start()
    {
        laser = this.GetComponent<LineRenderer>();
        p_Animator = GameObject.Find("WarpAnim").GetComponent<Animator>();
    }


    void Update()
    {

        Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);//アナログスティックの入力
        if (stickR.x > 0.5f || stickR.x < -0.5f || stickR.y > 0.5f || stickR.y < -0.5f)//入力があったら
        {

            SetTarget();//放物線の計算と描画
            laser.enabled = true;//放物線を出す
            targetMarker.SetActive(true);//マーカーを出す
            float stickAngle = Mathf.Atan2(stickR.y, stickR.x);//アナログスティックの座標を角度にする
            Quaternion setQuat = new Quaternion();
            setQuat.eulerAngles = new Vector3(0, -Mathf.Rad2Deg * stickAngle + transform.rotation.eulerAngles.y, 0);//Controllerの向きとアナログスティックの傾きを合わせる
            targetMarker.transform.rotation = setQuat;//代入
        }
        else
        {
            if (isMove == true && stickR.x <= 0.03f && stickR.x >= -0.03f && stickR.y <= 0.03f && stickR.y >= -0.03f)//アナログスティックをいじっていなくて移動になったら
            {

                if (p_LineRenderer != null)
                {
                    p_LineRenderer.enabled = true;
                }
                isMove = false;//移動をストップ
                transform.root.position = new Vector3(targetMarker.transform.position.x, targetMarker.transform.position.y + fHeight, targetMarker.transform.position.z);//座標を代入

                Quaternion setQuat = new Quaternion();
                setQuat.eulerAngles = new Vector3(0, targetMarker.transform.eulerAngles.y, 0);//Controllerの向きとアナログスティックの傾きを合わせる
                transform.root.rotation = setQuat;//回転を代入

                if (p_Animator != null)
                {
                    p_Animator.SetBool("isMove", true);
                }
                Invoke("FadeReset", 0.2f);//もし触れているなら魂を変える

                Invoke("OFF", 5.0f);
            }
            laser.enabled = false;//放物線を消す
            targetMarker.SetActive(false);//マーカーを消す
        }


    }

    void FadeReset()
    {
        if (p_Animator != null)
        {
            p_Animator.SetBool("isMove", false);
        }
    }

    void OFF()
    {
        if (p_LineRenderer != null)
        {
            p_LineRenderer.enabled = false;
        }
    }

    public void SetTarget()
    {

        //コントローラの向いている角度(x軸回転)をラジアン角へ
        var angleFacing = -Mathf.Deg2Rad * transform.eulerAngles.z;
        var h = transform.position.y + 5.0f;//触れた場所のY座標
        var v0 = initialVelocity;
        var sin = Mathf.Sin(angleFacing);
        var cos = Mathf.Cos(angleFacing);
        var g = Gravity;

        //地面に到達する時間の式 :
        //  t = (v0 * sinθ) / g + √ (v0^2 * sinθ^2) / g^2 + 2 * h / g
        var arrivalTime = (v0 * sin) / g + Mathf.Sqrt((square(v0) * square(sin)) / square(g) + (2F * h) / g);

        for (var i = 0; i < vertexCount; i++)
        {
            //delta時間あたりのワールド座標(ラインレンダラーの節)
            var delta = i * arrivalTime / vertexCount;
            var x = v0 * cos * delta;
            var y = v0 * sin * delta - 0.5F * g * square(delta);
            Vector3 forward = new Vector3(-transform.right.x, 0, -transform.right.z);
            Vector3 vertex = transform.position + forward * x + Vector3.up * y;
            vArrow.Add(vertex);
        }


        for (int i = 1; i < vertexCount; i++)
        {
            Vector3 vec3 = vArrow[i] - vArrow[i - 1];
            Ray ray = new Ray(vArrow[i - 1], vec3);
            float dis = Vector3.Distance(vArrow[i], vArrow[i - 1]);
            //Debug.DrawRay(ray.origin, ray.direction, Color.black, 1.0f);
            if (Physics.Raycast(ray, out hit, dis + 1.0f, KeepOut))
            {
                laser.material = OutMaterial;
                isMove = false;//移動中

                break;
            }
            else if (Physics.Raycast(ray, out hit, dis + 1.0f, mask))//コライダーにあたっていたら
            {
                targetMarker.transform.position = hit.point;//ターゲットマーカーを頂点の最終地点へ
                laser.material = MoveMaterial;
                isMove = true;//移動中
                break;
            }
            else
            {
                isMove = false;//移動中
                laser.material = OutMaterial;
                //targetMarker.transform.position = hit.point;//ターゲットマーカーを頂点の最終地点へ
                targetMarker.transform.position = vArrow[vArrow.Count - 1];//ターゲットマーカーを頂点の最終地点へ
            }
        }


        //LineRendererの頂点の設置
        laser.positionCount = vArrow.Count;
        laser.SetPositions(vArrow.ToArray());
        vArrow.Clear();        //リストの初期化

    }
    static float square(float num)
    {
        return Mathf.Pow(num, 2);
    }

}

