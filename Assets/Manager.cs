using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject boundingBoxPrefab;
    public GameObject ballPrefab;

    private Queue<GameObject> balls = new Queue<GameObject>();
    private const float PIXELS_PER_UNIT = 100f;

    /**
     * Setup the bounding box.
     */
    void Start()
    {
        float width = Screen.width;
        float height = Screen.height;
        float halfWidth = width / 2f / PIXELS_PER_UNIT;
        GameObject leftBox = Instantiate(boundingBoxPrefab, new Vector3(-halfWidth, 0, 0), Quaternion.identity);
        leftBox.transform.localScale = new Vector3(0.1f, (float)(height / PIXELS_PER_UNIT), 1);

        GameObject rightBox = Instantiate(boundingBoxPrefab, new Vector3(halfWidth, 0, 0), Quaternion.identity);
        rightBox.transform.localScale = new Vector3(0.1f, (float)(height / PIXELS_PER_UNIT), 1);

        float halfHeight = height / 2f / PIXELS_PER_UNIT;
        GameObject topBox = Instantiate(boundingBoxPrefab, new Vector3(0, halfHeight, 0), Quaternion.identity);
        topBox.transform.localScale = new Vector3((float)(width / PIXELS_PER_UNIT), 0.1f, 1);

        GameObject bottomBox = Instantiate(boundingBoxPrefab, new Vector3(0, -halfHeight, 0), Quaternion.identity);
        bottomBox.transform.localScale = new Vector3((float)(width / PIXELS_PER_UNIT), 0.1f, 1);

        // Setup the texture and sprite for the background
        SpriteRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        Texture2D bgTexture = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight);
        Sprite bgSprite = Sprite.Create(
            bgTexture, 
            new Rect(0.0f, 0.0f, bgTexture.width, bgTexture.height), 
            new Vector2(0.5f, 0.5f),
            PIXELS_PER_UNIT
        );
        bgRenderer.sprite = bgSprite;
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
