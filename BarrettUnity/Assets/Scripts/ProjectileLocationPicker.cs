using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLocationPicker : MonoBehaviour {

    public GameController controller;

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            controller.SetProjectilePosition(Input.mousePosition);   
        }
    }
}
