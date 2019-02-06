using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShield : MonoBehaviour
{

    public GameObject anchor;
    GameObject shield;

    void Start()
    {
        //StartCoroutine(FollowProjectile());
        shield = GameObject.FindGameObjectWithTag("Target_Reference");
    }

    void Update()
    {
        transform.position = new Vector3(shield.transform.position.x, shield.transform.position.y, -20);
        anchor.transform.position = new Vector3(shield.transform.position.x, shield.transform.position.y, anchor.transform.position.z);
        transform.LookAt(anchor.transform);
    }
}
