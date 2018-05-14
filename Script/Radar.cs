using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{

    [SerializeField]
    private GameObject Camera;

    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private float fDistance;
    [SerializeField]
    private Image[] TargetImage;
    [SerializeField]
    private List<string> EnemyTag;
    [SerializeField]
    private GameObject canvas;

    private Animator[] p_Animator;
    private RaycastHit hit;
    private bool isBadActive;
    private int ActiveMarker;
    private Vector3 baseScale;

    void Awake()
    {
        p_Animator = new Animator[TargetImage.Length];
    }

    void Start()
    {
        baseScale = TargetImage[0].transform.parent.localScale / MarkerScale(canvas.transform.position);
        for (int i = 0; i < TargetImage.Length; i++)
        {
            p_Animator[i] = TargetImage[i].GetComponent<Animator>();
        }
    }

    void Update()
    {
        List<GameObject> tmpObj = SearchTarget();
        ActiveMarker = tmpObj.Count;
        if (ActiveMarker != 0)
        {
            isBadActive = true;
            for (int i = 0; i < TargetImage.Length; i++)
            {
                if (i < tmpObj.Count)
                {
                    TargetMarker(tmpObj[i], i);
                }
            }
        }
        else
        {
            for (int i = 0; i < TargetImage.Length; i++)
            {
                p_Animator[i].SetBool("Set", false);
                StartCoroutine(DelayMethod(0.2f, i));
            }
        }

        for (int i = ActiveMarker; i < TargetImage.Length; i++)
        {
            p_Animator[i].SetBool("Set", false);
            StartCoroutine(DelayMethod(0.2f, i));

        }
    }


    void TargetMarker(GameObject target, int num)
    {

        float distance = Vector3.Distance(target.transform.position, Camera.transform.position);
        Vector3 rot = target.transform.position - Camera.transform.position;

        Ray ray = new Ray(Camera.transform.position, rot);
        Debug.DrawRay(ray.origin, ray.direction * fDistance, Color.red);

        if (Physics.Raycast(ray, out hit, fDistance, mask))
        {
            p_Animator[num].SetBool("Set", true);
            TargetImage[num].enabled = true;
            Debug.DrawLine(hit.point, Camera.transform.position);
            TargetImage[num].rectTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            TargetImage[num].transform.parent.localScale = baseScale * MarkerScale(hit.transform.position);
            //スケールもここで変える
        }
        else
        {
            p_Animator[num].SetBool("Set", false);
            // TargetImage[num].enabled = false;

            StartCoroutine(DelayMethod(0.2f, num));
        }
    }


    float MarkerScale(Vector3 target)
    {
        return (target - Camera.transform.position).magnitude;
    }

    private IEnumerator DelayMethod(float waitTime, int num)
    {
        yield return new WaitForSeconds(waitTime);
        TargetImage[num].enabled = false;
    }


    List<GameObject> SearchTarget()
    {
        float TmpDistance = 0;           //距離用一時変数
        List<GameObject> target = new List<GameObject>();

        for (int i = 0; i < EnemyTag.Count; i++)
        {
            foreach (GameObject obs in GameObject.FindGameObjectsWithTag(EnemyTag[0]))
            {

                TmpDistance = Vector3.Distance(obs.transform.position, transform.position);

                if (fDistance > TmpDistance)//比べて近いのがあったら座標をいれる
                {
                    target.Add(obs); //ターゲット情報を入れる
                }

            }
        }
        return target;
    }
}
//public class Radar : MonoBehaviour
//{

//    [SerializeField]
//    private GameObject Camera;

//    [SerializeField]
//    private LayerMask mask;
//    [SerializeField]
//    private float fDistance;
//    [SerializeField]
//    private Image[] TargetImage;
//    [SerializeField]
//    private List<string> EnemyTag;


//    private Animator[] p_Animator;
//    private RaycastHit hit;
//    private bool isBadActive;
//    private int ActiveMarker;

//    void Awake()
//    {
//        p_Animator = new Animator[TargetImage.Length];
//    }

//    void Start()
//    {
//        for (int i = 0; i < TargetImage.Length; i++)
//        {
//            p_Animator[i] = TargetImage[i].GetComponent<Animator>();
//        }
//    }

//    void Update()
//    {
//        List<GameObject> tmpObj = SearchTarget();
//        ActiveMarker = tmpObj.Count;
//        if (ActiveMarker != 0)
//        {
//            isBadActive = true;
//            for (int i = 0; i < TargetImage.Length; i++)
//            {
//                if (i < tmpObj.Count)
//                {

//                    TargetMarker(tmpObj[i], i);

//                }
//            }
//        }
//        else
//        {
//            for (int i = 0; i < TargetImage.Length; i++)
//            {
//                p_Animator[i].SetBool("Set", false);
//                StartCoroutine(DelayMethod(0.2f, i));
//            }
//        }

//        for (int i = ActiveMarker; i < TargetImage.Length; i++)
//        {
//            p_Animator[i].SetBool("Set", false);
//            StartCoroutine(DelayMethod(0.2f, i));

//        }
//    }


//    void TargetMarker(GameObject target, int num)
//    {

//        float distance = Vector3.Distance(target.transform.position, Camera.transform.position);

//        Vector3 rot = target.transform.position - Camera.transform.position;

//        Ray ray = new Ray(Camera.transform.position, rot);
//        Debug.DrawRay(ray.origin, ray.direction * fDistance, Color.red);
//        if (Physics.Raycast(ray, out hit, 1.0f, mask))
//        {
//            p_Animator[num].SetBool("Set", true);
//            TargetImage[num].enabled = true;
//            Debug.DrawLine(hit.point, Camera.transform.position);

//            TargetImage[num].rectTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
//        }
//        else
//        {
//            p_Animator[num].SetBool("Set", false);
//            // TargetImage[num].enabled = false;

//            StartCoroutine(DelayMethod(0.2f, num));
//        }
//    }

//    private IEnumerator DelayMethod(float waitTime, int num)
//    {
//        yield return new WaitForSeconds(waitTime);
//        TargetImage[num].enabled = false;
//    }


//    List<GameObject> SearchTarget()
//    {
//        float TmpDistance = 0;           //距離用一時変数
//        List<GameObject> target = new List<GameObject>();

//        for (int i = 0; i < EnemyTag.Count; i++)
//        {
//            foreach (GameObject obs in GameObject.FindGameObjectsWithTag(EnemyTag[0]))
//            {

//                TmpDistance = Vector3.Distance(obs.transform.position, transform.position);

//                if (fDistance > TmpDistance)//比べて近いのがあったら座標をいれる
//                {
//                    target.Add(obs); //ターゲット情報を入れる
//                }

//            }
//        }
//        return target;
//    }
//}
