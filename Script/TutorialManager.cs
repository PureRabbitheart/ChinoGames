using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    [SerializeField]
    private Animator p_DoorAnim;

    [SerializeField]
    private GameObject Hand;
    [SerializeField]
    private GameObject OculusModel;
    [SerializeField]
    private LineRenderer p_LineRender;
    [SerializeField]
    private GameObject VRModel;
    [SerializeField]
    private GameObject Cube;


    public void OpenDoor()
    {
        p_DoorAnim.SetTrigger("Open");
        Cube.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] pos = { Hand.transform.position, OculusModel.transform.position };
        p_LineRender.SetPositions(pos);

        if (VRModel.activeSelf == true)
        {
            Cube.SetActive(true);
        }
    }
}
