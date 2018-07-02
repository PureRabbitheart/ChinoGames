using UnityEngine;
using System.Collections;

public class MovieSample : MonoBehaviour
{

    [SerializeField, Range(0, 10)]
    float time = 1;
    [SerializeField, Range(0, 10)]
    float resetTime = 1;
    [SerializeField]
    Transform endPosition;

    //[SerializeField]
    //AnimationCurve curve;

    private float startTime;
    private Vector3 startPosition;

    void OnEnable()
    {
        if (time <= 0)
        {
            transform.position = endPosition.position;
            return;
        }

        startTime = Time.timeSinceLevelLoad;
        startPosition = transform.position;
    }

    void Update()
    {
        var diff = Time.timeSinceLevelLoad - startTime;
 
        if (diff > time)
        {
            transform.position = endPosition.position;
        }
        if(diff>resetTime)
        {
            transform.position = startPosition;
            startTime = Time.timeSinceLevelLoad;

        }

        var rate = diff / time;
        //var pos = curve.Evaluate(rate);

        transform.position = Vector3.Lerp(startPosition, endPosition.position, rate);
        //transform.position = Vector3.Lerp (startPosition, endPosition, pos);
    }

    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR

        if (!UnityEditor.EditorApplication.isPlaying || enabled == false)
        {
            startPosition = transform.position;
        }

        UnityEditor.Handles.Label(endPosition.position, endPosition.ToString());
        UnityEditor.Handles.Label(startPosition, startPosition.ToString());
#endif
        Gizmos.DrawSphere(endPosition.position, 0.1f);
        Gizmos.DrawSphere(startPosition, 0.1f);

        Gizmos.DrawLine(startPosition, endPosition.position);
    }
}