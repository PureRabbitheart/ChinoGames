using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grab : MonoBehaviour
{


    private Quaternion lastRootation;// 回転の情報を入れる変数
    private Quaternion currentRoation;// 回転の情報を入れる変数
    private GameObject grabbedObject;// 持った物のObjectの情報を入れる変数
    [SerializeField]
    private bool grabbing;// 握っているかのフラグ
    [SerializeField]
    private LayerMask grabMask;// LayerMaskの情報を入れる変数
    [SerializeField]
    private float grabRadius;// layerの距離
    [SerializeField]
    private OVRInput.Controller controller;// どの手で持つか
    [SerializeField]
    private string buttonName;// どのボタンで持つか
    [SerializeField]
    private string having_TagName;// 持った時のTagを指定

    //private enum Trigger
    //{
    //    LTrigger,
    //    RTrigger
    //}
    //[SerializeField]
    //private Trigger trigger;



    void Start()
    {
        grabbing = false;

    }

    void GrabObject()// 握っているとき
    {

        grabbing = true;
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);
        if (hits.Length > 0)
        {
            int closestHit = 0;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHit].distance) closestHit = i;
            }

            grabbedObject = hits[closestHit].transform.gameObject;
            grabbedObject.transform.parent = transform;                                                             //親離れ　自立する
            grabbedObject.layer = 0;//レイヤーをデフォルトにする
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;                                             //掴んだ瞬間Rigidbodyを消す
            grabbedObject.transform.position = transform.position;                                                  //掴んだオブジェクトに手の座標を入れて動かす
            grabbedObject.transform.rotation = transform.rotation;
            grabbedObject.tag = having_TagName;//タグの名前変更

        }

    }

    void DropObject()// 握っていないとき
    {
        grabbing = false;
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity();
            grabbedObject.layer = 8;//レイヤーをデフォルトにする
            grabbedObject.tag = "Untagged";
            grabbedObject = null;
        }
    }


    Vector3 GetAngularVelocity()// Objectの回転
    {
        Quaternion deltaRotation = currentRoation * Quaternion.Inverse(lastRootation);
        return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
    }

    void Update()// 持つオブジェクトの処理
    {
        //Debug.DrawRay(transform.position, transform.forward,Color.red);
        if (grabbedObject != null)
        {
            lastRootation = currentRoation;
            currentRoation = grabbedObject.transform.rotation;
        }


        if (!grabbing && Input.GetAxis(buttonName) > 0.8f)//何も持っていなかったら
        {
            GrabObject();//持つ

        }
        if (grabbing && Input.GetAxis(buttonName) < 0.8f)//何か持っていたら
        {
            DropObject();//離したら
        }

    }

    //bool JudgeInput()
    //{
    //    switch (trigger)
    //    {
    //        case Trigger.LTrigger:
    //            return OVRInput.GetDown(OVRInput.RawButton.LHandTrigger);
    //        case Trigger.RTrigger:
    //            return OVRInput.GetDown(OVRInput.RawButton.RHandTrigger);

    //    }
    //    return false;
    //}
}
