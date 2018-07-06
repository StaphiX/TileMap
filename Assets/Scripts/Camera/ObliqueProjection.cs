using UnityEngine;
using System.Collections;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class ObliqueProjection : MonoBehaviour
{
    public float angle = 45.0f;
    public float zScale = 0.5f;
    public float zOffset = 0.0f;
    public static int PIXELSPERUNIT = 64;
    public static int PIXELSCALE = 1;

    private int screenHeight = Screen.height;
    Camera camera = null;

    private void Awake()
    {

    }

    public void Update()
    {
        if(screenHeight != Screen.height)
            camera.orthographicSize = ((float)Screen.height / ((float)PIXELSCALE * (float)PIXELSPERUNIT)) * 0.5f;
    }

    public void Apply ()
    {
        if (camera == null)
            camera = GetComponent<Camera>();

        camera.orthographic = true;
        var orthoHeight = camera.orthographicSize;
        var orthoWidth = camera.aspect * orthoHeight;
        var m = Matrix4x4.Ortho (-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, camera.nearClipPlane, camera.farClipPlane);
        var s = zScale / orthoHeight;
        m [0, 2] = +s * Mathf.Sin (Mathf.Deg2Rad * -angle);
        m [1, 2] = -s * Mathf.Cos (Mathf.Deg2Rad * -angle);
        m [0, 3] = -zOffset * m [0, 2];
        m [1, 3] = -zOffset * m [1, 2];
        camera.projectionMatrix = m;
    }

    void OnEnable ()
    {
        Apply ();
    }

    void OnDisable ()
    {
        camera.ResetProjectionMatrix ();
    }
}