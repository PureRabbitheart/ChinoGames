using UnityEngine;

namespace kawetofe.randomPrefabPlacer
{
    public class SimpleRotation : MonoBehaviour
    {

        public float rotSpeed = 100f;
        public bool doRotate = true;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (doRotate)
                transform.Rotate(new Vector3(0, Time.deltaTime * rotSpeed, 0)); ;
        }
    }
}
