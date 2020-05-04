using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensControl : MonoBehaviour
{
    public GameObject sphere1, sphere2;
    public float speed=1;
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(Input.GetKey(KeyCode.UpArrow))
        {
            sphere1.transform.Translate( 0.5f * speed, 0, 0);
            sphere1.transform.localScale += new Vector3(1 * speed, 1 * speed, 1 * speed);
            sphere2.transform.Translate(-0.5f * speed, 0, 0);
            sphere2.transform.localScale += new Vector3(1 * speed, 1 * speed, 1 * speed);
        }
        else if(Input.GetKey(KeyCode.DownArrow) && distance < sphere1.transform.localScale.x / 2 && distance > 0.2f)
        {
            sphere1.transform.Translate(-0.5f * speed, 0, 0);
            sphere1.transform.localScale -= new Vector3(1 * speed, 1 * speed, 1 * speed);
            sphere2.transform.Translate( 0.5f * speed, 0, 0);
            sphere2.transform.localScale -= new Vector3(1 * speed, 1 * speed, 1 * speed);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && distance > 0.2f)
        {
            sphere1.transform.Translate(-0.5f * speed, 0, 0);
            sphere2.transform.Translate(0.5f * speed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && distance < sphere1.transform.localScale.x/2)
        {
            sphere1.transform.Translate(0.5f * speed, 0, 0);
            sphere2.transform.Translate(-0.5f * speed, 0, 0);
        }
    }
    private void Update()
    {
        distance = Mathf.Abs(sphere1.transform.transform.position.x);
    }
}
