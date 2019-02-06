using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityTest : MonoBehaviour {

    float time;
    Vector3 start_position;
    Vector3 end_position;
    float time_needed;
    

    void Start()
    {

    }

    public IEnumerator MoveToPosition(Vector3 target, float velocity)
    {
        start_position = transform.position;
        var distance = Vector3.Distance(target, transform.position);
        time_needed = distance / velocity;
        while(time < 1)
        {
            time += Time.deltaTime / time_needed;
            transform.position = Vector3.Lerp(start_position, target, time);
            yield return null;
        }
    }
}
