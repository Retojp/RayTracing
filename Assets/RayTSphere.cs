using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTSphere : MonoBehaviour
{
    public int id = 0;
    public int specular = 0;
    private void OnEnable()
    {
        RayTScript.RegisterSphere(this);
    }
    private void OnDisable()
    {
        RayTScript.UnregisterSphere(this);
    }
}