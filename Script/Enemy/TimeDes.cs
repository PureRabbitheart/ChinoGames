using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDes : MonoBehaviour {

    public float fEndTime;
    private float fNowTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        fNowTime += Time.deltaTime;
        if(fEndTime < fNowTime)
        {
            Destroy(gameObject);
        }
	}
}
