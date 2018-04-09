using UnityEngine;
using System.Collections.Generic;

public class SearchingBehavior : MonoBehaviour
{
    public event System.Action<GameObject> onFound = (obj) =>
    {
    };
    public event System.Action<GameObject> onLost = (obj) =>
    {
    };

    [SerializeField, Range(0.0f, 360.0f)]
    private float fSearchAngle = 0.0f;
    private float fSearchCosTheta = 0.0f;

    private SphereCollider p_SphereCollider;
    private List<FoundObj> lFoundList = new List<FoundObj>();






    private void Awake()
    {
        p_SphereCollider = GetComponent<SphereCollider>();
        ApplyAngle();
    }

    private void OnDisable()//非アクティブになったら呼ばれる
    {
        lFoundList.Clear();//消す
    }

    private void OnValidate()//値が変更されたら呼ばれる
    {
        ApplyAngle();
    }


    private void Update()
    {
        UpdateFoundObj();
    }


    private void ApplyAngle()//角度の適用
    {
        float searchRad = fSearchAngle * 0.5f * Mathf.Deg2Rad;
        fSearchCosTheta = Mathf.Cos(searchRad);
    }

    public float SearchAngle//角度を渡す
    {
        get { return fSearchAngle; }
    }

    public float Radius//半径を渡す
    {

        get
        {
            if (p_SphereCollider == null)
            {
                p_SphereCollider = GetComponent<SphereCollider>();
            }
            return p_SphereCollider != null ? p_SphereCollider.radius : 0.0f;
        }
    }

    private void UpdateFoundObj()
    {
        foreach (FoundObj foundData in lFoundList)//リスト分回す
        {
            GameObject targetObject = foundData.Object;
            if (targetObject == null)
            {
                continue;
            }

            bool isFound = CheckFound(targetObject);
            foundData.Update(isFound);

            if (foundData.IsFound())
            {
                onFound(targetObject);
            }
            else if (foundData.IsLost())
            {
                onLost(targetObject);
            }
        }
    }

    private bool CheckFound(GameObject target)//チェックする
    {
        Vector3 vtargetPos = target.transform.position;
        Vector3 vpos = transform.position;

        Vector3 pos_XZ = Vector3.Scale(vpos, new Vector3(1.0f, 0.0f, 1.0f));
        Vector3 targetpos_XZ = Vector3.Scale(vtargetPos, new Vector3(1.0f, 0.0f, 1.0f));

        Vector3 toTargetFlatDir = (targetpos_XZ - pos_XZ).normalized;
        Vector3 forward = transform.forward;
        if (!WithinAngle(forward, toTargetFlatDir, fSearchCosTheta))
        {
            return false;
        }

        Vector3 TargetDir = (vtargetPos - vpos).normalized;

        if (!HitRay(vpos, TargetDir, target))
        {
            return false;
        }

        return true;
    }

    private bool WithinAngle(Vector3 i_forwardDir, Vector3 i_toTargetDir, float i_cosTheta)
    {
        // 方向ベクトルが無い場合は、同位置にあるものだと判断する。
        if (i_toTargetDir.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }

        float dot = Vector3.Dot(i_forwardDir, i_toTargetDir);
        return dot >= i_cosTheta;
    }

    private bool HitRay(Vector3 fromPosition, Vector3 TargetDir, GameObject target)
    {
        // 方向ベクトルが無い場合は、同位置にあるものだと判断する。
        if (TargetDir.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }

        RaycastHit onHitRay;
        if (!Physics.Raycast(fromPosition, TargetDir, out onHitRay, Radius))
        {
            return false;
        }

        if (onHitRay.transform.gameObject != target)
        {
            return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject enterObject = other.gameObject;

        // 念のため多重登録されないようにする。
        if (lFoundList.Find(value => value.Object == enterObject) == null)
        {
            lFoundList.Add(new FoundObj(enterObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject exitObject = other.gameObject;

        var foundData = lFoundList.Find(value => value.Object == exitObject);
        if (foundData == null)
        {
            return;
        }

        if (foundData.IsCurrentFound())
        {
            onLost(foundData.Object);
        }

        lFoundList.Remove(foundData);
    }


    private class FoundObj
    {
        public FoundObj(GameObject Obj)
        {
            Object = Obj;
        }

        public GameObject Object;
        private bool isCurrentFound = false;
        private bool isPrevFound = false;



        public Vector3 Position
        {
            get { return Object != null ? Object.transform.position : Vector3.zero; }
        }

        public void Update(bool isFound)
        {
            isPrevFound = isCurrentFound;
            isCurrentFound = isFound;
        }

        public bool IsFound()
        {
            return isCurrentFound && !isPrevFound;
        }

        public bool IsLost()
        {
            return !isCurrentFound && isPrevFound;
        }

        public bool IsCurrentFound()
        {
            return isCurrentFound;
        }
    }

}