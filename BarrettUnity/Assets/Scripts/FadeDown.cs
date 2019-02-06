using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeDown : MonoBehaviour {

    Vector3 endPos;
	void Start () {
        transform.gameObject.SetActive(false);
        endPos = transform.position;
        //float height = transform.position 
	}
	
}
