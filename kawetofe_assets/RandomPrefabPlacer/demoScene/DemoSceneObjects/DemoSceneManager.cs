using UnityEngine;
using UnityEngine.SceneManagement;

namespace kawetofe.randomPrefabPlacer
{
    public class DemoSceneManager : MonoBehaviour
    {
        Scene currentScene;

       void Awake()
        {
            currentScene = SceneManager.GetActiveScene();
        }


        public void NextScene()
        {
            int thisSceneInt = currentScene.buildIndex;
            int nextSceneInt = thisSceneInt + 1;
            if(nextSceneInt > SceneManager.sceneCount)
            {
                nextSceneInt = 0;
            }
            SceneManager.LoadScene(nextSceneInt);
        }

        public void PreviousScene()
        {
            int thisSceneInt = currentScene.buildIndex;
            int nextSceneInt = thisSceneInt - 1;
            if (nextSceneInt <0)
            {
                nextSceneInt = SceneManager.sceneCount -1;
            }
            SceneManager.LoadScene(nextSceneInt);
        }
    }
}