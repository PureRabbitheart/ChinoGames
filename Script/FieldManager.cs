using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    [SerializeField]
    private GameManager p_GameManager;

    public enum AREA
    {
        A,
        B,
        C,
        D,
    }

    public AREA eArea;

    private bool isAreaTime;
    void OnTriggerEnter(Collider other)
    {
        EnemyManager p_EnemyManager = other.transform.root.GetComponent<EnemyManager>();
        if (p_EnemyManager != null && p_EnemyManager.LTarget.Count == 0)
        {
            switch (eArea)
            {
                case AREA.A:
                    for (int i = 0; i < p_GameManager.A_PosList.Count; i++)
                    {
                        p_EnemyManager.LTarget.Add(p_GameManager.A_PosList[i].transform);
                    }
                    break;
                case AREA.B:
                    for (int i = 0; i < p_GameManager.A_PosList.Count; i++)
                    {
                        p_EnemyManager.LTarget.Add(p_GameManager.B_PosList[i].transform);
                    }
                    break;
                case AREA.C:
                    for (int i = 0; i < p_GameManager.A_PosList.Count; i++)
                    {
                        p_EnemyManager.LTarget.Add(p_GameManager.C_PosList[i].transform);
                    }
                    break;
                case AREA.D:
                    for (int i = 0; i < p_GameManager.A_PosList.Count; i++)
                    {
                        p_EnemyManager.LTarget.Add(p_GameManager.D_PosList[i].transform);
                    }
                    break;

            }

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            isAreaTime = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isAreaTime = false;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (isAreaTime == true)
        {
            p_GameManager.AreaUpdate(eArea.ToString());
        }

    }
}
