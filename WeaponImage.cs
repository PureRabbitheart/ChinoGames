using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponImage : MonoBehaviour
{

    private EnemyManager p_EnemyManager;
    [SerializeField]
    private Image WeaponUI;
    [SerializeField]
    private Sprite Dust;
    [SerializeField]
    private Sprite Gorilla;
    [SerializeField]
    private Sprite Drone;
    [SerializeField]
    private Sprite Boss;
    [SerializeField]
    private Sprite DustMK2;
    [SerializeField]
    private Sprite BigRobo;
    [SerializeField]
    private Sprite FourLeg;
    void Start()
    {

    }

    void Update()
    {
        p_EnemyManager = transform.root.GetComponent<EnemyManager>();
        switch (p_EnemyManager.ModelType)
        {
            case EnemyManager.eMODEL_TYPE.Drone:
                WeaponUI.sprite = Drone;
                break;
            case EnemyManager.eMODEL_TYPE.Dust:
                WeaponUI.sprite = Dust;
                break;
            case EnemyManager.eMODEL_TYPE.Gorilla:
                WeaponUI.sprite = Gorilla;
                break;
            case EnemyManager.eMODEL_TYPE.DustMK2:
                break;
            case EnemyManager.eMODEL_TYPE.BigRobo:
                break;
            case EnemyManager.eMODEL_TYPE.Boss:
                break;
            case EnemyManager.eMODEL_TYPE.FourLeg:
                break;
            case EnemyManager.eMODEL_TYPE.Gundam:
                break;
        }

    }
}
