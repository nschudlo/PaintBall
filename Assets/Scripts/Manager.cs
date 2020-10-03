using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject boundingBoxPrefab;
    public GameObject ballPrefab;

    private Queue<GameObject> balls = new Queue<GameObject>();

    void Start()
    {
        float width = Screen.width;
        float height = Screen.height;
        float halfWidth = width / 2f / Utility.PIXELS_PER_UNIT;

        // Setup the bounding box
        GameObject leftBox = Instantiate(boundingBoxPrefab, new Vector3(-halfWidth, 0, 0), Quaternion.identity);
        leftBox.transform.localScale = new Vector3(0.1f, (float)(height / Utility.PIXELS_PER_UNIT), 1);

        GameObject rightBox = Instantiate(boundingBoxPrefab, new Vector3(halfWidth, 0, 0), Quaternion.identity);
        rightBox.transform.localScale = new Vector3(0.1f, (float)(height / Utility.PIXELS_PER_UNIT), 1);

        float halfHeight = height / 2f / Utility.PIXELS_PER_UNIT;
        GameObject topBox = Instantiate(boundingBoxPrefab, new Vector3(0, halfHeight, 0), Quaternion.identity);
        topBox.transform.localScale = new Vector3((float)(width / Utility.PIXELS_PER_UNIT), 0.1f, 1);

        GameObject bottomBox = Instantiate(boundingBoxPrefab, new Vector3(0, -halfHeight, 0), Quaternion.identity);
        bottomBox.transform.localScale = new Vector3((float)(width / Utility.PIXELS_PER_UNIT), 0.1f, 1);

        // Setup the paintable background sprite
        GameObject.FindGameObjectWithTag("Paint")
            .GetComponent<SpriteRenderer>()
            .sprite = Utility.createSprite(
                Camera.main.pixelWidth, Camera.main.pixelHeight, Color.clear
            );

        // Setup the static background sprite
        GameObject.FindGameObjectWithTag("Background")
            .GetComponent<SpriteRenderer>()
            .sprite = Utility.createSprite(
                Camera.main.pixelWidth, Camera.main.pixelHeight, Color.black
            );        
    }

    /**
     * Listen for mouse presses to add or remove balls.
     */
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            GameObject newBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            balls.Enqueue(newBall);
        } else if(Input.GetKeyDown(KeyCode.Mouse1)) {
            if(balls.Count > 0) {
                Destroy(balls.Dequeue());
            }
        }
    }
}
