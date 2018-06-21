using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChange : MonoBehaviour
{

    [SerializeField]
    private SceneObject nextScene;
    [SerializeField]
    private Animator p_Animator;
    [SerializeField]
    private bool isEvacuation;
    [SerializeField]
    private bool isStartAnim;

    private float startTime;
    private bool isStart;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isEvacuation == true)
        {
            p_Animator.SetBool("isActivate", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isEvacuation == false && isStart == true)
        {
            isEvacuation = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        if(isStartAnim == true)
        {
            p_Animator.SetBool("isBadActivate", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime < 5.0f)
        {
            startTime += Time.deltaTime;
        }
        else
        {
            isStart = true;
        }
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(nextScene);
    }
}
