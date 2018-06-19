using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVAnimator : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 0.5F;
    [SerializeField]
    private Renderer rend;
    [SerializeField]
    private bool LR;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        float offset = -(Time.time * scrollSpeed);
        if(LR == true)
        {
            rend.materials[0].SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
        else
        {
            rend.materials[1].SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }

}
