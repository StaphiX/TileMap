using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    List<UIScreen> screen = new List<UIScreen>();

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddScreen(UIScreen screen)
    {
        this.screen.Add(screen);
    }
}
