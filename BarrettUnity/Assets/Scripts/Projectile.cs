using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Rigidbody rigidBody;

    public GameManager manager;
    public GameController controller;

    public static float velocity = -0.1f;

    bool active = true;

    public int ID;
    public Pair Position;
    public Pair StartPos;
    public Vector2 InitialPos;

    private void Start() {
        transform.position = new Vector3(0, InitialPos.y, InitialPos.x);
        rigidBody = GetComponent<Rigidbody>();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<GameController>();
        manager.ProjectileLaunched(manager.GameTime.ElapsedMilliseconds, this.gameObject, StartPos);
        Position = StartPos;
        if (active)
        {
            //rigidBody.AddForce(new Vector3(0, acceleration, 0), ForceMode.Acceleration);
            rigidBody.velocity = new Vector3(0, velocity, 0);
        }
    }

    private void FixedUpdate()
    {
        var temp = controller.BucketBoxPosition(transform.position);
        if(!temp.IsValid())
        {
            WasMissed();
        }
        else
        {
            Position = temp;
        }
    }

    /*private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Shield")
        {
            active = false;
            manager.SuccessfulProjectileBlock();
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            active = false;
            manager.UnsuccessfulProjectileBlock();
            GameObject.Destroy(this.gameObject);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "User")
        {
            WasHit();
        }
        else if(other.tag != "Fireball" && other.tag != "Arrow")
        {
            WasMissed();
        }
    }

    private void WasHit()
    {
        active = false;
        Position = controller.BucketBoxPosition(transform.position);
        manager.SuccessfulProjectileBlock(manager.GameTime.ElapsedMilliseconds, this.gameObject);
        GameObject.Destroy(this.gameObject);
    }
    private void WasMissed()
    {
        active = false;
        manager.UnsuccessfulProjectileBlock(manager.GameTime.ElapsedMilliseconds, this.gameObject);
        GameObject.Destroy(this.gameObject);
    }
}
