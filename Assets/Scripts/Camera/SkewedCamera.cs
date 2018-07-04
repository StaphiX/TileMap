using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is meant to be attached to your main camera.
// If you want to use it on more than one camera at a time, it will require
// modifcations due to the Camera.on* delegates in OnEnable()/OnDisable().

[ExecuteInEditMode]
public class SkewedCamera : MonoBehaviour
{
    public static int PIXELSPERUNIT = 64;
    public static int PIXELSCALE = 1;

    private void Awake()
    {
        SetupCamera();
    }

    void SetupCamera()
    {
        Camera.main.orthographicSize = ((float)Screen.height / ((float)PIXELSCALE * (float)PIXELSPERUNIT)) * 0.5f;
        //SetObliqueness(0, -1);
    }

    void SetObliqueness(float horizObl, float vertObl)
    {
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        Camera.main.projectionMatrix = Camera.main.projectionMatrix.
    }

    private void OnPreCull()
    {
        SetObliqueness(0.0f, 0.25f);
    }
}
