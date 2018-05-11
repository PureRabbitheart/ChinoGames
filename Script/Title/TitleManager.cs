using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    [SerializeField]
    private bool isTransition;//Scene遷移のフラグ

    [SerializeField]
    private SceneObject nextScene;

    [SerializeField]
    private Animator p_Animator;

    public bool isGetOn;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isGetOn == true)
        {
            p_Animator.SetBool("Action", true);
            isGetOn = false;
        }

        if (isTransition == true)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
