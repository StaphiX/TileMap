using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    static bool bInitialised = false;
    static Vector2 lastTouchPos = Vector2.zero;
    static Vector2 lastDragTouchPos = Vector2.zero;
    static Vector2 lastPostition = Vector2.zero;
    static Vector2 currentTouchPos = Vector2.zero;
    static Vector2 currentDragTouchPos = Vector2.zero;
    static Vector2 currentPosition = Vector2.zero;
    static Vector2 deltaPosition = Vector2.zero;
    static Vector2 deltaTouchPos = Vector2.zero;
    static Vector2 deltaDragTouchPos = Vector2.zero;
    static int heldCount = 0;
    static int dragHeldCount = 0;

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

        if (Input.GetMouseButton(1))
        {
            ++dragHeldCount;
            currentDragTouchPos = currentPosition;
        }
        else
        {
            dragHeldCount = 0;
            currentDragTouchPos = -Vector2.one;
            deltaDragTouchPos = Vector2.zero;
        }


        if (bInitialised)
        {
            deltaPosition = currentPosition - lastPostition;
            if (heldCount > 1)
                deltaTouchPos = currentTouchPos - lastTouchPos;
            if (dragHeldCount > 1)
                deltaDragTouchPos = currentDragTouchPos - lastDragTouchPos;
        }

        lastTouchPos = currentTouchPos;
        lastDragTouchPos = currentDragTouchPos;
        lastPostition = currentPosition;
    }

    public static Vector2 GetDrag()
    {
        return deltaDragTouchPos;
    }

    public static bool IsDragging(float fThreshold = 1.0f)
    {
        if(Mathf.Abs(deltaDragTouchPos.sqrMagnitude) > fThreshold)
        {
            return true;
        }

        return false;
        
    }


}
