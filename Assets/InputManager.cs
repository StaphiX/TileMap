using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    static bool bInitialised = false;
    static Vector2 lastTouchPos = Vector2.zero;
    static Vector2 lastPostition = Vector2.zero;
    static Vector2 currentTouchPos = Vector2.zero;
    static Vector2 currentPosition = Vector2.zero;
    static Vector2 deltaPosition = Vector2.zero;
    static Vector2 deltaTouchPos = Vector2.zero;
    static int heldCount = 0;

    void Awake()
    {
        CaptureInput();
        bInitialised = true;
    }

    void Update()
    {
        CaptureInput();
    }

    void CaptureInput()
    {
        currentPosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            ++heldCount;
            currentTouchPos = currentPosition;
        }
        else
        {
            heldCount = 0;
            currentTouchPos = -Vector2.one;
            deltaTouchPos = Vector2.zero;
        }

        if (bInitialised)
        {
            deltaPosition = currentPosition - lastPostition;
            if (heldCount > 1)
            {
                deltaTouchPos = currentTouchPos - lastTouchPos;
                //Debug.Log(deltaTouchPos);
            }
        }

        lastTouchPos = currentTouchPos;
        lastPostition = currentPosition;
    }

    public static Vector2 GetDrag()
    {
        return deltaTouchPos;
    }

    public static bool IsDragging(float fThreshold = 1.0f)
    {
        if(Mathf.Abs(deltaTouchPos.sqrMagnitude) > fThreshold)
        {
            return true;
        }

        return false;
        
    }


}
