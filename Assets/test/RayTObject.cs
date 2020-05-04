using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RayTObject : MonoBehaviour
{
    public int specular=0;
    private void OnEnable()
    {
        RayTScript.RegisterObject(this);
    }
    private void OnDisable()
    {
        RayTScript.UnregisterObject(this);
    }
}