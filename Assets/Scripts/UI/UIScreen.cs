using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : UIObject
{
    void Awake()
    {
        InitObject();
    }

    protected virtual void InitObject()
    {

    }

    // Use this for initialization
    void Start ()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
