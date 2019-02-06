using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : MonoBehaviour
{


    public GameObject User;
    public bool IsActive;
    public float SmoothDistance = 5;
    GameManager manager;
    public Speed CurrentSpeed = Speed.Medium;
    GameManager.ProjectileInfo CurrentProjectile;
    Vector3 PredictedPosition;
    bool Moving = false;
    public VelocityTest test;

    // Use this for initialization
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(manager.controller.Arrow.IsVisible && !Moving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToHome());
            Moving = true;
            return;
        }
        if (CurrentProjectile == null)
        {
            CurrentProjectile = manager.GetVisibleProjectile();
        }
        else
        {
            if (CurrentProjectile.IsCaught == GameManager.Caught.Undetermined)
            {
                if (!Moving)
                {
                    PredictedPosition = ChooseOptimalPathtoProjectile(CurrentProjectile);
                    //StartCoroutine(MoveToPosition(PredictedPosition, CurrentSpeed));
                    StartCoroutine(test.MoveToPosition(PredictedPosition, 1));
                    Moving = true;
                }
                else
                {
                    if (PredictedPosition != ChooseOptimalPathtoProjectile(CurrentProjectile))
                    {
                        PredictedPosition = ChooseOptimalPathtoProjectile(CurrentProjectile);
                        StopCoroutine(test.MoveToPosition(PredictedPosition, 1));
                        StartCoroutine(test.MoveToPosition(PredictedPosition, 1));
                        //StopCoroutine("MoveToPosition");
                        //StartCoroutine(MoveToPosition(PredictedPosition, CurrentSpeed));
                    }
                }
            }
            else
            {
                CurrentProjectile = null;
                Moving = false;
                StopAllCoroutines();
            }
        }
    }

    /* IEnumerator MoveToProjectile(GameManager.ProjectileInfo info)
     {
         while(info.IsCaught == GameManager.Caught.Undetermined)
         {

         }
     }*/

    public IEnumerator MoveToPosition(Vector3 position, Speed speed)
    {
        //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        Vector3 PlayerPosition = manager.controller.UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(User.transform.position));
        Vector3 EndingPosition = manager.controller.UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(position));

        float xdistance = PlayerPosition.x - EndingPosition.x;
        float ydistance = PlayerPosition.y - EndingPosition.y;
        float distance = Mathf.Sqrt(Mathf.Pow(xdistance, 2) + Mathf.Pow(ydistance, 2));

        int intervalcount = Mathf.RoundToInt(distance / SmoothDistance);

        /*Debug.Log("Time for Arrival: " + CalculateTimeForArrival(distance, speed));
        Debug.Log("Interval Count: " + intervalcount);
        Debug.Log("Distance: " + distance);*/

        for (int i = 0; i < intervalcount; i++)
        {
            if(PredictedPosition != position)
            {
                break;
            }
            //watch.Start();
            xdistance = (EndingPosition.x - PlayerPosition.x) / (intervalcount - i);
            ydistance = (EndingPosition.y - PlayerPosition.y) / (intervalcount - i);

            PlayerPosition.x += xdistance;
            PlayerPosition.y += ydistance;

            Vector3 new_position = manager.controller.UIContainer.transform.TransformPoint(new Vector3(PlayerPosition.x, PlayerPosition.y, 1.1f));
            new_position = manager.mainCamera.ScreenToWorldPoint(new_position);
            User.transform.position = new_position;

            yield return new WaitForSecondsRealtime((float)((int)speed) / 1000);
            /* watch.Stop();
             Debug.Log(watch.ElapsedMilliseconds);
             watch.Reset();*/
        }

    }

    public IEnumerator MoveToPair(Pair position, Speed speed)
    {
        //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        Vector3 PlayerPosition = manager.controller.UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(User.transform.position));
        Vector3 EndingPosition = manager.controller.GetBoxCoordinatesRelativeToUIContainer(position.x, position.y);

        float xdistance = PlayerPosition.x - EndingPosition.x;
        float ydistance = PlayerPosition.y - EndingPosition.y;
        float distance = Mathf.Sqrt(Mathf.Pow(xdistance, 2) + Mathf.Pow(ydistance, 2));

        int intervalcount = Mathf.RoundToInt(distance / SmoothDistance);

        /*Debug.Log("Time for Arrival: " + CalculateTimeForArrival(distance, speed));
        Debug.Log("Interval Count: " + intervalcount);
        Debug.Log("Distance: " + distance);*/

        for (int i = 0; i < intervalcount; i++)
        {
            //watch.Start();
            xdistance = (EndingPosition.x - PlayerPosition.x) / (intervalcount - i);
            ydistance = (EndingPosition.y - PlayerPosition.y) / (intervalcount - i);

            PlayerPosition.x += xdistance;
            PlayerPosition.y += ydistance;

            Vector3 new_position = manager.controller.UIContainer.transform.TransformPoint(new Vector3(PlayerPosition.x, PlayerPosition.y, 1.1f));
            new_position = manager.mainCamera.ScreenToWorldPoint(new_position);
            User.transform.position = new_position;

            yield return new WaitForSecondsRealtime((float)((int)speed) / 1000);
            /* watch.Stop();
             Debug.Log(watch.ElapsedMilliseconds);
             watch.Reset();*/
        }

    }

    public IEnumerator MoveToHome()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(MoveToPair(manager.GridCenter, CurrentSpeed));
    }

    public float CalculateTimeForArrival(float totaldistance, Speed speed)
    {
        switch (speed)
        {
            case Speed.Fast:
                return totaldistance / (SmoothDistance / 0.0165f);
            case Speed.Medium:
                return totaldistance / (SmoothDistance / 0.0171f);
            case Speed.Slow:
                return totaldistance / (SmoothDistance / 0.0325f);
            default:
                return totaldistance / (SmoothDistance / 0.0171f);
        }
    }
    public float CalculateTimeForArrival(Vector3 PlayerPosition, Vector3 EndPosition, Speed speed)
    {
        float xdistance = PlayerPosition.x - EndPosition.x;
        float ydistance = PlayerPosition.y - EndPosition.y;
        float distance = Mathf.Sqrt(Mathf.Pow(xdistance, 2) + Mathf.Pow(ydistance, 2));
        return CalculateTimeForArrival(distance, speed);
    }
    public float CalculateTimeForArrival(Vector3 PlayerPosition, Pair EndPosition, Speed speed)
    {
        return CalculateTimeForArrival(PlayerPosition, manager.controller.GetBoxCoordinatesRelativeToUIContainer(EndPosition.x, EndPosition.y), speed);
    }

    public Vector3 ChooseOptimalPathtoProjectile(GameManager.ProjectileInfo info)
    {
        Vector3 PlayerPosition = manager.controller.UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(User.transform.position));
        Vector3 ProjectilePosition = manager.controller.UIContainer.transform.InverseTransformPoint(manager.mainCamera.WorldToScreenPoint(info.Projectile.transform.position));
        if (PlayerPosition.y < ProjectilePosition.y)
        {
            if(info.Projectile.Position.x == manager.controller.BucketBoxPosition(User.transform.position).x)
            {
                return info.Projectile.transform.position;
            }
        }

        float timeNeeded = CalculateTimeForArrival(PlayerPosition, info.Projectile.Position, CurrentSpeed);

        return PredictProjectileLocationInTime(info, timeNeeded);
    }

    public Vector3 PredictProjectileLocationInTime(GameManager.ProjectileInfo info, float additionaltime)
    {
        if (info.IsCaught == GameManager.Caught.Undetermined)
        {
            
            float deltay = info.Projectile.rigidBody.velocity.y * additionaltime;

            Vector3 predicted_position = info.Projectile.transform.position;
            predicted_position.y += deltay;

            return predicted_position;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public void ChangeSpeed(int new_speed)
    {
        switch(new_speed)
        {
            case 0:
                CurrentSpeed = Speed.Slow;
                break;
            case 1:
                CurrentSpeed = Speed.Medium;
                break;
            case 2:
                CurrentSpeed = Speed.Fast;
                break;
        }
    }

    public enum Speed
    {
        Slow = 20,
        Medium = 15,
        Fast = 1
    }

}
