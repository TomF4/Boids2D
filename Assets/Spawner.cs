using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int numberOfBoids = 5;
    public static ArrayList boids = new ArrayList();

    public Color colour = new Vector4(0, 0, 0, 1);
    public float spawnRadius = 10;

    GameObject boid;
    public GizmoType showSpawnRegion;

    public enum GizmoType { Never, SelectedOnly, Always }

    // Start is called before the first frame update
    void Start()
    {
        boid = Resources.Load<GameObject>("boid");
        intialiseBoids();
    }

    void intialiseBoids()
    {
        Vector2 topRight = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        float x;
        float y;
        for (int i = 0; i < numberOfBoids; i++)
        {
            x = Random.Range(-Camera.main.ScreenToWorldPoint(topRight).x, Camera.main.ScreenToWorldPoint(topRight).x);
            y = Random.Range(-Camera.main.ScreenToWorldPoint(topRight).y, Camera.main.ScreenToWorldPoint(topRight).y);

            GameObject newBoid = boid;
            newBoid.transform.position = new Vector3(x,y, -1);
            newBoid.GetComponent<SpriteRenderer>().color = colour;
            newBoid = Instantiate(newBoid);

            boids.Add(newBoid);
        }
    }
}
