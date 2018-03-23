using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRWarp : MonoBehaviour
{

    private LineRenderer laser;
    private RaycastHit hit;



    [SerializeField]
    GameObject targetMarker;
    [SerializeField]
    float vertexCount = 25;
    [SerializeField]
    float initialVelocity = 1;
    List<Vector3> vertexes = new List<Vector3>();
    static readonly float Gravity = 9.81F;


    void Start()
    {
        laser = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        SetTarget();
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            transform.root.position = targetMarker.transform.position;
        }
        //if (Physics.Raycast(transform.position, -transform.right, out hit))//コライダーにあたっていたら
        //{
        //    //laser.SetPosition(0, transform.position);//始点
        //    //laser.SetPosition(1, hit.point);//終点
        //    if (OVRInput.GetDown(OVRInput.RawButton.A))
        //    {
        //        transform.root.position = vertexes[vertexes.Count - 1];
        //    }
        //}
        //else//触れていなかったら
        //{
        //    laser.enabled = false;
        //    //laser.SetPosition(0, transform.position);//始点
        //    //laser.SetPosition(1, -transform.right * 100);//終点
        //}

    }


    public void SetTarget()
    {

        laser.enabled = true;
        //コントローラの向いている角度(x軸回転)をラジアン角へ
        var angleFacing = -Mathf.Deg2Rad * transform.eulerAngles.z;
        var h = transform.position.y+5.0f;//触れた場所のY座標
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
            var forward = new Vector3(-transform.right.x, 0, -transform.right.z);
            var vertex = transform.position + forward * x + Vector3.up * y;
            vertexes.Add(vertex);
        }
        //ターゲットマーカーを頂点の最終地点へ
        targetMarker.transform.position = vertexes[vertexes.Count - 1];
        //LineRendererの頂点の設置
        laser.positionCount = vertexes.Count;
        laser.SetPositions(vertexes.ToArray());
        //リストの初期化
        vertexes.Clear();
    }
    static float square(float num)
    {
        return Mathf.Pow(num, 2);
    }

}

