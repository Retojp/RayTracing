using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera _cam;
    private Vector3 prevMouse;
    public GameObject _stick,ball;
    private GameObject obj;
    public bool control;
    // Start is called before the first frame update
    void Start()
    {
        prevMouse = Input.mousePosition;
        _cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (control)
        {
            _cam.transform.Rotate((prevMouse.y - Input.mousePosition.y) * 0.5f, 0, 0);
            _stick.transform.Rotate(0, -(prevMouse.x - Input.mousePosition.x) * 0.5f, 0);
            prevMouse = Input.mousePosition;
            obj = _stick;
            if (Input.GetKey(KeyCode.Q))
            {
                obj.transform.Rotate(0, -1, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                obj.transform.Rotate(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                obj.transform.Translate(-0.1f, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                obj.transform.Translate(0.1f, 0, 0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                obj.transform.Translate(0, 0, 0.1f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                obj.transform.Translate(0, 0, -0.1f);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                obj.transform.Translate(0, 0.1f, 0);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                obj.transform.Translate(0, -0.1f, 0);
            }
        }
        else
        {
            obj = ball;
            prevMouse = Input.mousePosition;
            obj.transform.rotation = _stick.transform.rotation;
            if (Input.GetKey(KeyCode.Q))
            {
                obj.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                obj.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                obj.transform.Translate(-0.1f, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                obj.transform.Translate(0.1f, 0, 0);
            }
            if (Input.GetKey(KeyCode.W))
            {
                obj.transform.Translate(0, 0, 0.1f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                obj.transform.Translate(0, 0, -0.1f);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                obj.transform.Translate(0, 0.1f, 0);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                obj.transform.Translate(0, -0.1f, 0);
            }
        }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
            control = !control;
            }

    }
}
