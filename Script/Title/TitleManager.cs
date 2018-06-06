using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public bool isTransition;//Scene遷移のフラグ

    [SerializeField]
    private SceneObject nextScene;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTransition == true)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
