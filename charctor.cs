using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charctor : MonoBehaviour
{
    public float fSpeed;
    [SerializeField]
    private Transform tCamera;

    private bool isMode = true;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        KeyBoard();
        Pad();
        //CameraZoom();
    }

    void Pad()
    {

        float LY = Input.GetAxis("Vertical") / 2;
        float LX = Input.GetAxis("Horizontal") / 2;

        if (LY != 0 || LX != 0)
        {
            transform.position += new Vector3(LX, 0, -LY);
        }

        float RY = Input.GetAxis("Vertical2") / 2;
        float RX = Input.GetAxis("Horizontal2") / 2;

        if (RY != 0 || RX != 0)
        {
            transform.Rotate(-RY, RX, 0);
        }

        if (Input.GetKey(KeyCode.JoystickButton7))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            isMode = !isMode;
        }
        float RT = Input.GetAxis("RT");
        float LT = Input.GetAxis("LT");
        if(isMode == true)
        {
            if (LT != 0)
            {
                transform.position += new Vector3(0, LT, 0);
            }

            if (RT != 0)
            {
                transform.position += new Vector3(0, -RT, 0);
            }
        }
        else
        {
            tCamera.GetComponent<Camera>().fieldOfView += LT*50;
        }
   
    }

    void KeyBoard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, fSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -fSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(fSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-fSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += new Vector3(0, fSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += new Vector3(0, -fSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.T))
        {
            transform.Rotate(50 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.G))
        {
            transform.Rotate(-50 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.H))
        {
            transform.Rotate(0, -50 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0, 50 * Time.deltaTime, 0);
        }
    }
}
