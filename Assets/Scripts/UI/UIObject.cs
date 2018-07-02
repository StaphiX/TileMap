using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObject : MonoBehaviour {

    UIObject parent = null;
    List<UIObject> child = new List<UIObject>();

    protected Rect screenRect = Rect.zero;

    void Awake()
    {
        InitObject();
    }

    protected virtual void InitObject()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddChild(UIObject obj)
    {
        child.Add(obj);
    }
}
