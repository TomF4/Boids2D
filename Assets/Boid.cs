using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private float movespeed = 2.0f;

    //ScreenWrapping
    float leftConstraint = Screen.width;
    float rightConstraint = Screen.width;
    float topConstraint = Screen.height;
    float bottomConstraint = Screen.height;
    float buffer = 0.2f;
    float distanceZ;
    ArrayList boids;
    Color colour = new Color(1, 1, 1, 1);

    Vector3 heading;
    float randomAngle;
    public GizmoType showSpawnRegion;

    public enum GizmoType { Never, SelectedOnly, Always }

    private float viewRadius = 3.5f;
    private float avoidRadius = 1.0f;

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {
        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, viewRadius);
    }

    void Start()
    {
        Camera cam = Camera.main;
        transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        distanceZ = Mathf.Abs(cam.transform.position.z + transform.position.z);

        leftConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        bottomConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
        topConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;

        boids = Spawner.boids;
        Time.timeScale = 3;
    }

    void Update()
    {
        screenWrap();
        Quaternion seperateQ = seperate();
        Quaternion alignmentQ = alignment();
        Quaternion cohesionQ = cohesion();

        Quaternion testQ = Quaternion.Slerp(alignmentQ, cohesionQ, 0.5f);
        Quaternion testA = Quaternion.Slerp(testQ, seperateQ, 0.5f);
        //
        transform.rotation = Quaternion.RotateTowards(transform.rotation, testA, (111.0f) * Time.deltaTime);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, testA, (333.0f) * Time.deltaTime);
        
        transform.position = transform.position + transform.up * movespeed * Time.deltaTime;

        Debug.DrawLine(transform.position, transform.position + transform.up * 1.0f, Color.white, 0.022f, false);
    }

    Quaternion seperate()
    {
        GameObject closestBoid = null;
        double distance = Mathf.Infinity;
        Vector3 offsetToTarget = new Vector3(0,0,0);
        foreach (GameObject boidy in boids)
        {
            offsetToTarget = (boidy.transform.position - transform.position);
            float sqrDst = offsetToTarget.x * offsetToTarget.x + offsetToTarget.y * offsetToTarget.y + offsetToTarget.z * offsetToTarget.z;
         
            if (sqrDst < distance && transform.position != boidy.transform.position && sqrDst < avoidRadius)
            {
                closestBoid = boidy;
                distance = sqrDst;
            }

        }
        if (closestBoid == null)
        {
            return transform.rotation;
        }

        float angle = Mathf.Atan2(closestBoid.transform.position.y - transform.position.y, closestBoid.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, -angle));
    }

    Quaternion alignment()
    {
        ArrayList nearbyBoids = new ArrayList();

        foreach (GameObject boidy in boids)
        {
            Vector3 offsetToTarget = (boidy.transform.position - transform.position);
            float sqrDst = offsetToTarget.x * offsetToTarget.x + offsetToTarget.y * offsetToTarget.y + offsetToTarget.z * offsetToTarget.z;
            if(sqrDst < viewRadius && transform.position != boidy.transform.position)
            {
                nearbyBoids.Add(boidy);
            }
        }

        if (nearbyBoids.Count == 0)
        {
            return transform.rotation;
        }

        Vector3 averageHeading = new Vector3(0, 0, 0);
        float angle = 0;
        foreach (GameObject boidy in nearbyBoids)
        {
            averageHeading += boidy.transform.position + transform.up;
            angle += Mathf.Atan2(boidy.transform.position.y - transform.position.y, boidy.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Debug.DrawLine(transform.position, boidy.transform.position, Color.green, 0.022f, false);
        }
        averageHeading /= nearbyBoids.Count;
        angle = Mathf.Atan2(averageHeading.y - transform.position.y, averageHeading.x - transform.position.x);

        //angle /= nearbyBoids.Count;

        return Quaternion.Euler(new Vector3(0, 0, angle));
    }
    Quaternion cohesion()
    {
        ArrayList nearbyBoids = new ArrayList();

        foreach (GameObject boidy in boids)
        {
            Vector3 offsetToTarget = (boidy.transform.position - transform.position);
            float sqrDst = offsetToTarget.x * offsetToTarget.x + offsetToTarget.y * offsetToTarget.y + offsetToTarget.z * offsetToTarget.z;
            if (sqrDst < viewRadius * viewRadius && transform.position != boidy.transform.position)
            {
                nearbyBoids.Add(boidy);
            }
        }

        if (nearbyBoids.Count == 0)
        {
            return transform.rotation;
        }

        Vector3 averagePosition = new Vector3(0, 0, 0);
        foreach (GameObject boidy in nearbyBoids)
        {
            averagePosition += boidy.transform.position;
            Debug.DrawLine(transform.position, boidy.transform.position, Color.blue, 0.2022f, false);
        }
        averagePosition /= nearbyBoids.Count;
        float angle = Mathf.Atan2(averagePosition.y - transform.position.y, averagePosition.x - transform.position.x) * Mathf.Rad2Deg;
        //angle /= nearbyBoids.Count;
        return Quaternion.Euler(new Vector3(0, 0, angle));
    }   
    
    void screenWrap()
    {
        if (transform.position.x < leftConstraint - buffer)
            transform.position = new Vector3(rightConstraint - 0.10f, transform.position.y, transform.position.z);
        if (transform.position.x > rightConstraint)
            transform.position = new Vector3(leftConstraint, transform.position.y, transform.position.z);
        if (transform.position.y < bottomConstraint - buffer)
            transform.position = new Vector3(transform.position.x, topConstraint + buffer, transform.position.z);
        if (transform.position.y > topConstraint + buffer)
            transform.position = new Vector3(transform.position.x, bottomConstraint - buffer, transform.position.z);
    }
    private Quaternion calcAvg(ArrayList rotationlist)
    {

        float x = 0, y = 0, z = 0, w = 0;
        foreach (Quaternion q in rotationlist)
        {
            x += q.x; y += q.y; z += q.z; w += q.w;
        }
        float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        return new Quaternion(x * k, y * k, z * k, w * k).normalized;
    }
}




//foreach (GameObject boidy in boids)
//{
//    Vector3 offsetToTarget = (boidy.transform.position - transform.position);

//    float sqrDst = offsetToTarget.x * offsetToTarget.x + offsetToTarget.y * offsetToTarget.y + offsetToTarget.z * offsetToTarget.z;
//    //float distance = Vector3.Distance(transform.position, boidy.transform.position);

//    if (sqrDst < viewRadius)
//    {
//        float angle = Mathf.Atan2(offsetToTarget.y, offsetToTarget.x) * Mathf.Rad2Deg;
//        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

//        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * movespeed);
//        // transform.position = transform.position + transform.up * movespeed * Time.deltaTime;
//        Debug.DrawLine(transform.position, boidy.transform.position, Color.magenta, 0.5f, false);
//    }
//    else
//    {
//        //int random = Random.Range(0, 2);
//        //if (random == 1)
//        //{
//        //    randomAngle = Random.Range(0, 360);
//        //    
//        //    Quaternion q = Quaternion.AngleAxis(randomAngle, Vector3.forward);
//        //    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * movespeed);
//        //}
//    }
//}