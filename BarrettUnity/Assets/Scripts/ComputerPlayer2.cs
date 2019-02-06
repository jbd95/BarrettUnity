using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer2 : MonoBehaviour
{

    GameManager manager;
    GameManager.ProjectileInfo TrackedProjectile = null;
    bool IsMoving = false;
    public bool IsActive = false;
    public float Velocity = 0.3f;
    public GameObject Bucket;
    float InitialVelocity;
    Vector3 PredictedPosition;

    // Use this for initialization
    void Start()
    {
        InitialVelocity = Velocity;
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        if (!IsActive)
        {
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (manager.controller.Arrow.IsVisible && !IsMoving)
        {
            StopAllCoroutines();
            //StartCoroutine(MoveToHome());
            TeleportHome();
            IsMoving = true;
            return;
        }

        if (IsMoving && TrackedProjectile == null && !(manager.controller.Arrow.IsVisible))
        {
            IsMoving = false;
        }

        if (TrackedProjectile == null)
        {
            TrackedProjectile = manager.GetVisibleProjectile();
            if (TrackedProjectile == null)
            {
                return;
            }
        }

        if (TrackedProjectile.IsCaught == GameManager.Caught.Undetermined)
        {
            if (!IsMoving)
            {
                PredictedPosition = ChooseOptimalPathtoProjectile(TrackedProjectile);
                StartCoroutine(MoveToPosition(PredictedPosition, Velocity));
                IsMoving = true;
            }
            else
            {
                if (PredictedPosition != ChooseOptimalPathtoProjectile(TrackedProjectile))
                {
                    PredictedPosition = ChooseOptimalPathtoProjectile(TrackedProjectile);
                    StopAllCoroutines();
                    StartCoroutine(MoveToPosition(PredictedPosition, Velocity));
                    IsMoving = true;
                }
            }
            //EstimateVelocity();
        }
        else
        {
            TrackedProjectile = null;
            IsMoving = false;
            StopAllCoroutines();
        }
        UpdateVelocity();
    }


    public void SetVelocity(string speed)
    {
        switch (speed)
        {
            case "Fast":
                Velocity = 0.3f;
                break;
            case "Medium":
                Velocity = 0.1f;
                break;
            case "Slow":
                Velocity = 0.05f;
                break;
        }
    }

    private void UpdateVelocity()
    {
        Velocity = InitialVelocity * Mathf.Exp(-0.25f * (0.01f) * (manager.GameTime.ElapsedMilliseconds / 1000));
        //Velocity = Mathf.Pow(Velocity, (-0.25f * (manager.GameTime.ElapsedMilliseconds / 1000)));
        //Debug.Log(Velocity);
    }

    // Update is called once per frame
    /*void FixedUpdate()
    {
        if(manager.controller.Arrow.IsVisible() && !IsMoving)
        {
            StopAllCoroutines();
        }

        if(TrackedProjectile == null)
        {
            TrackedProjectile = manager.GetVisibleProjectile();
        }

        if (manager.controller.Arrow.IsVisible && !Moving)
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
    }*/

    public IEnumerator MoveToHome()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(MoveToPosition(manager.controller.GetBoxCoordinatesInWorldFrame(manager.GridCenter.x, manager.GridCenter.y), Velocity));
    }

    public void TeleportHome()
    {
        transform.position = manager.controller.GetBoxCoordinatesInWorldFrame(manager.GridCenter.x, manager.GridCenter.y);
    }

    public IEnumerator MoveToPosition(Vector3 target, float velocity)
    {
        float time_alotted = 0;
        Vector3 start_position = transform.position;
        float distance = Vector3.Distance(target, transform.position);
        float time_needed = distance / velocity;
        while (time_alotted < 1)
        {
            time_alotted += Time.deltaTime / time_needed;
            transform.position = Vector3.Lerp(start_position, target, time_alotted);
            yield return null;
        }
    }

    /*public bool WillHitProjectile(GameManager.ProjectileInfo info)
    {

    }*/

    public float CalculateTimeForArrival(float totaldistance)
    {
        return totaldistance / Velocity;
    }
    public float CalculateTimeForArrival(Vector3 PlayerPosition, Vector3 EndPosition)
    {
        return CalculateTimeForArrival(Vector3.Distance(PlayerPosition, EndPosition));
    }
    public float CalculateTimeForArrival(Vector3 PlayerPosition, Pair EndPosition)
    {
        Vector3 end_location = manager.controller.UIContainer.transform.TransformPoint(manager.controller.GetBoxCoordinatesRelativeToUIContainer(EndPosition.x, EndPosition.y));
        end_location = manager.mainCamera.ScreenToWorldPoint(end_location);
        return CalculateTimeForArrival(PlayerPosition, end_location);
    }

    private float CalculateTimeForXArrival(Vector3 PlayerPosition, Vector3 EndPosition)
    {
        return CalculateTimeForArrival(Mathf.Abs(PlayerPosition.z - EndPosition.z));
    }
    private float CalculateTimeForXArrival(float xstart, float xend)
    {
        return CalculateTimeForArrival(Mathf.Abs(xend - xstart));
    }

    public Vector3 ChooseOptimalPathtoProjectile(GameManager.ProjectileInfo info)
    {
        Vector3 ProjectilePosition = info.Projectile.transform.position;
        if (transform.position.y < ProjectilePosition.y)
        {
            if (info.Projectile.Position.x == manager.controller.BucketBoxPosition(transform.position).x)
            {
                return info.Projectile.transform.position;
            }
        }

        float timeNeeded = CalculateTimeForArrival(transform.position, info.Projectile.transform.position);
        Vector3 newlocation = PredictProjectileLocationInTime(info, timeNeeded);
        float newTimeNeeded = CalculateTimeForArrival(transform.position, newlocation);
        if (newTimeNeeded < timeNeeded)
        {
            newlocation.y -= (Projectile.velocity * Mathf.Abs(timeNeeded - newTimeNeeded));
        }
        

        if (manager.controller.BucketBoxPosition(newlocation) == new Pair(-1, -1))
        {
            if ((int)CalculateTimeForXArrival(transform.position, info.Projectile.transform.position) > (int)ProjectileLifetime(info))
            {
                //Debug.Log("Unable to Reach Projectile");
                //return manager.controller.GetBoxCoordinatesInWorldFrame(manager.GridCenter.x, manager.GridCenter.y);
            }
            return new Vector3(0, transform.position.y, info.Projectile.transform.position.z);
        }

        return newlocation;
    }

    public Vector3 PredictProjectileLocationInTime(GameManager.ProjectileInfo info, float additionaltime)
    {
        //additionaltime -= Mathf.Abs(CalculateTimeForArrival(Mathf.Abs(transform.position.y - info.Projectile.transform.position.y)) / 3f);
        if (info.IsCaught == GameManager.Caught.Undetermined)
        {
            Vector3 predicted_position = info.Projectile.transform.position;
            predicted_position.y += (Projectile.velocity * additionaltime);
            return predicted_position;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
    private float ProjectileLifetime(GameManager.ProjectileInfo info)
    {
        Vector3 transformedDestination = manager.controller.UIContainer.transform.TransformPoint(new Vector3(manager.controller.xStartValue, manager.controller.yStartValue, 0));
        float ydistance = Mathf.Abs(info.Projectile.transform.position.y - manager.mainCamera.ScreenToWorldPoint(new Vector3(transformedDestination.x, transformedDestination.y, 1.1f)).y);
        return ydistance / -Projectile.velocity;
    }

    private float ProjectileLifetime(float y, float projectileVelocity)
    {
        Vector3 transformedDestination = manager.controller.UIContainer.transform.TransformPoint(new Vector3(manager.controller.xStartValue, manager.controller.yStartValue, 0));
        float ydistance = Mathf.Abs(y - manager.mainCamera.ScreenToWorldPoint(new Vector3(transformedDestination.x, transformedDestination.y, 1.1f)).y);
        return ydistance / -projectileVelocity;
    }

    public Grid<bool> GeneratePerformanceTable(float projectileVelocity)
    {
        Grid<bool> returnable = new Grid<bool>(manager.controller.xIntervalCount, manager.controller.yIntervalCount, false, true);
        for(int x = 0; x < manager.controller.xIntervalCount; x++)
        {
            for(int y = 0; y < manager.controller.yIntervalCount; y++)
            {
                returnable[x, y] = CanReachCoordinates(x, y, projectileVelocity);
            }
        }
        return returnable;
    }
    private bool CanReachCoordinates(int x, int y, float projectileVelocity)
    {
        float availableTime = ProjectileLifetime(manager.controller.GetBoxCoordinatesInWorldFrame(x, y).y, projectileVelocity);
        if ((int)CalculateTimeForXArrival(manager.controller.GetBoxCoordinatesInWorldFrame(manager.GridCenter.x, manager.GridCenter.y).z, manager.controller.GetBoxCoordinatesInWorldFrame(x, y).z) > (int)availableTime)
        {
            return false;
        }
        return true;
    }
}
