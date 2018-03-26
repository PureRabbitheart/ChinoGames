using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRWarp : MonoBehaviour
{

    private LineRenderer laser;
    private RaycastHit hit;
    private List<Vector3> vArrow = new List<Vector3>();

    [SerializeField]
    private GameObject targetMarker;
    [SerializeField]
    private float vertexCount = 30;
    [SerializeField]
    private float initialVelocity = 10;
    [SerializeField]
    private float Gravity = 9.81F;
   
    void Start()
    {
        laser = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);//アナログスティックの入力
        if (stickR.x != 0 || stickR.y != 0)//入力があったら
        {
            SetTarget();//放物線の計算と描画
            laser.enabled = true;
            float stickAngle = Mathf.Atan2(stickR.y, stickR.x);
            Quaternion setQuat = new Quaternion();
            setQuat.eulerAngles = new Vector3(0, -Mathf.Rad2Deg * stickAngle + transform.rotation.eulerAngles.y, 0);
            targetMarker.transform.rotation = setQuat;
            //Vector3 direction = new Vector3(stickR.x , transform.rotation.eulerAngles.y, stickR.y  );
            //targetMarker.transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            laser.enabled = false;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.A))//Aボタンを押したら
        {
            transform.root.position = targetMarker.transform.position;
            transform.root.rotation = targetMarker.transform.rotation;
        }
    }


    public void SetTarget()
    {

        laser.enabled = true;
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
            Debug.DrawRay(ray.origin, ray.direction, Color.black, 1.0f);
            if (Physics.Raycast(ray, out hit, dis + 1.0f))//コライダーにあたっていたら
            {
                targetMarker.transform.position = hit.point;//ターゲットマーカーを頂点の最終地点へ
                break;
            }
            else
            {
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

