using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  //Sceneを変える為のusing



public class SceneChange : MonoBehaviour
{

    [Tooltip("遷移したいScene名を入れてね")]
    public string Scene;


    void Update()
    {

       if(Input.GetKeyDown(KeyCode.Space)|| OVRInput.GetDown(OVRInput.RawButton.X))
        {
            SceneManager.LoadScene(Scene);//遷移したいScene名を引数に入れて遷移する
        }

    }
}
