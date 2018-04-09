using UnityEngine;
using System.Collections.Generic;

public class Finder : MonoBehaviour
{
    [SerializeField]
    private Material mDefaultMaterial;
    [SerializeField]
    private Material mFoundMaterial;

    private Renderer m_renderer;
    private List<GameObject> lTarget = new List<GameObject>();


    private void Awake()
    {
        m_renderer = GetComponentInChildren<Renderer>();

        SearchingBehavior searching = GetComponentInChildren<SearchingBehavior>();
        searching.onFound += OnFound;
        searching.onLost += OnLost;
    }

    private void OnFound(GameObject foundObject)
    {
        lTarget.Add(foundObject);
        m_renderer.material = mDefaultMaterial;
    }

    private void OnLost(GameObject lostObject)
    {
        lTarget.Remove(lostObject);

        if (lTarget.Count == 0)
        {
            m_renderer.material = mFoundMaterial;
        }
    }

}