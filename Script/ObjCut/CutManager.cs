using UnityEngine;
using System.Collections;

public class CutManager : MonoBehaviour
{

    public Material capMaterial;

    void OnTriggerEnter(Collider other)
    {
        GameObject victim = other.gameObject;

        GameObject[] pieces = BLINDED_AM_ME.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

        if (!pieces[1].GetComponent<Rigidbody>())
        {
            pieces[1].AddComponent<Rigidbody>();
            //pieces[1].AddComponent<MeshCollider>();
            //pieces[1].GetComponent<MeshCollider>().convex = true;
        }

    }

    // Use this for initialization
    void Start()
    {


    }

    void Update()
    {



    }
    void Cut()
    {

    }

    /*void OnDrawGizmosSelected() {

		Gizmos.color = Color.green;

		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position + transform.up * 0.5f + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + transform.forward * 5.0f);

		Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
		Gizmos.DrawLine(transform.position,  transform.position + -transform.up * 0.5f);

	}*/

}
