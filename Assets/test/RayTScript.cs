using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RayTScript : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    private RenderTexture _target;
    private Camera _camera;
    public Texture SkyboxTexture;
    public Light DirectionalLight;
    public float mov;
    private float height=0;
    public float mov2;
    struct MeshObject
    {
        public Matrix4x4 localToWorldMatrix;
        public int indices_offset;
        public int indices_count;
        public int specular;
    }

    struct SphereObject
    {
        public Vector3 position;
        public float radius;
        public float specular;
    }
    private static List<MeshObject> _meshObjects = new List<MeshObject>();
    private static List<Vector3> _vertices = new List<Vector3>();
    private static List<int> _indices = new List<int>();
    private ComputeBuffer _meshObjectBuffer;
    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _indexBuffer;
    private ComputeBuffer _spheresBuffer;
    public GameObject periscope; 

    private static bool _meshObjectsNeedRebuilding = false;
    private static List<RayTObject> _rayTracingObjects = new List<RayTObject>();
    private static List<RayTSphere> _rayTracingSpheres = new List<RayTSphere>();
    public static void RegisterObject(RayTObject obj)
    {
        _rayTracingObjects.Add(obj);
        _meshObjectsNeedRebuilding = true;
    }
    public static void UnregisterObject(RayTObject obj)
    {
        _rayTracingObjects.Remove(obj);
        _meshObjectsNeedRebuilding = true;
    }

    public static void RegisterSphere(RayTSphere obj)
    {
        _rayTracingSpheres.Add(obj);
    }
    public static void UnregisterSphere(RayTSphere obj)
    {
        _rayTracingSpheres.Remove(obj); 
    }


    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (_target != null)
                _target.Release();
            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }

    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target

        InitRenderTexture();
        // Set the target and dispatch the compute shader
        RayTracingShader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        RayTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        // Blit the result texture to the screen

        Graphics.Blit(_target, destination);
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RebuildMeshObjectBuffers();
        RebuildSpheres();
        SetShaderParameters();
        Render(destination);
    }

    private void SetShaderParameters()
    {
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetFloat("time", Time.frameCount);
        Vector3 l = DirectionalLight.transform.forward;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(l.x, l.y, l.z, DirectionalLight.intensity));

        SetComputeBuffer("_MeshObjects", _meshObjectBuffer);
        SetComputeBuffer("_Vertices", _vertexBuffer);
        SetComputeBuffer("_Indices", _indexBuffer);
        SetComputeBuffer("_Spheres", _spheresBuffer);


        RayTracingShader.SetFloat("_mov2", mov2);
        RayTracingShader.SetFloat("_mov", mov);


    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.O))
        {
            mov++;
        }
        if(Input.GetKey(KeyCode.L))
        {
            mov--;
        }
        if (Input.GetKey(KeyCode.K))
        {
            mov2++;
        }
        if (Input.GetKey(KeyCode.J))
        {
            mov2--;
        }

        if (Input.GetKey(KeyCode.M))
        {
            if (height >= 0)
            {
                height--;
                periscope.transform.Translate(new Vector3(0, 1, 0));
            }
        }
        if (Input.GetKey(KeyCode.N))
        {
            if (height <= 20)
            {
                height++;
                periscope.transform.Translate(new Vector3(0, -1, 0));
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

    }

    private void RebuildMeshObjectBuffers()
    {
        if (!_meshObjectsNeedRebuilding)
        {
            return;
        }
        _meshObjectsNeedRebuilding = true;
        //_currentSample = 0;
        // Clear all lists
        _meshObjects.Clear();
        _vertices.Clear();
        _indices.Clear();
        // Loop over all objects and gather their data
        foreach (RayTObject obj in _rayTracingObjects)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            // Add vertex data
            int firstVertex = _vertices.Count;
            _vertices.AddRange(mesh.vertices);
            // Add index data - if the vertex buffer wasn't empty before, the
            // indices need to be offset
            int firstIndex = _indices.Count;
            var indices = mesh.GetIndices(0);
            _indices.AddRange(indices.Select(index => index + firstVertex));
            // Add the object itself
            _meshObjects.Add(new MeshObject()
            {
                localToWorldMatrix = obj.transform.localToWorldMatrix,
                indices_offset = firstIndex,
                indices_count = indices.Length,
                specular = obj.specular

            });
        }
        CreateComputeBuffer(ref _meshObjectBuffer, _meshObjects, 76);
        CreateComputeBuffer(ref _vertexBuffer, _vertices, 12);
        CreateComputeBuffer(ref _indexBuffer, _indices, 4);
    }

    private void RebuildSpheres()
    {
        List<SphereObject> spheres = new List<SphereObject>();
        foreach(RayTSphere sph in _rayTracingSpheres)
        {
            spheres.Add(new SphereObject()
            {
                position = sph.transform.position,
                radius = sph.transform.lossyScale.x/2,
                specular = sph.specular
            });
        }
        _rayTracingSpheres = _rayTracingSpheres.OrderBy(o => o.id).ToList();
        Debug.Log(_rayTracingSpheres);
        CreateComputeBuffer(ref _spheresBuffer, spheres, 20);
    }
    private static void CreateComputeBuffer<T>(ref ComputeBuffer buffer, List<T> data, int stride)
    where T : struct
    {
        // Do we already have a compute buffer?
        if (buffer != null)
        {
            // If no data or buffer doesn't match the given criteria, release it
            if (data.Count == 0 || buffer.count != data.Count || buffer.stride != stride)
            {
                buffer.Release();
                buffer = null;
            }
        }
        if (data.Count != 0)
        {
            // If the buffer has been released or wasn't there to
            // begin with, create it
            if (buffer == null)
            {
                buffer = new ComputeBuffer(data.Count, stride);
            }
            // Set data on the buffer
            buffer.SetData(data);
        }
    }
    private void SetComputeBuffer(string name, ComputeBuffer buffer)
    {
        if (buffer != null)
        {
            RayTracingShader.SetBuffer(0, name, buffer);
        }
    }
}
