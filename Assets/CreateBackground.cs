using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBackground : MonoBehaviour
{
    public Color colour = new Vector4(0, 0, 0, 1);

    public GameObject background;


    // Start is called before the first frame update
    void Start()
    {
        background.transform.position = new Vector3(0, 0, 0);
        ResizeBackground();
        background.GetComponent<SpriteRenderer>().color = colour;
        Instantiate(background);

        //intialiseBoids();

    }

    void ResizeBackground()
    {
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        background.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height,1);
    }

    //void intialiseBoids()
    //{
    //    boid = Resources.Load<GameObject>("boid");

    //    Vector2 topRight = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

    //    float x;
    //    float y;
    //    for (int i = 0; i < numberOfBoids; i++)
    //    {
    //        x = Random.Range(-Camera.main.ScreenToWorldPoint(topRight).x, Camera.main.ScreenToWorldPoint(topRight).x);
    //        y = Random.Range(-Camera.main.ScreenToWorldPoint(topRight).y, Camera.main.ScreenToWorldPoint(topRight).y);


    //        GameObject newBoid = boid;
    //        newBoid.transform.position = new Vector3(x, y, -1);
    //        Instantiate(newBoid);
    //        //= Instantiate(boid, new Vector3(x, y, -1), Quaternion.identity);

    //        boids.Add(newBoid);
    //    }
    //}
}