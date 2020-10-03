using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject boundingBoxPrefab;
    public GameObject ballPrefab;

    private GameObject ball;
    private const float DISTANCE_TO_MOVE = 50;
    private const float MAX_SPEED = 4000;
    private const float FORCE_SPEED_RATIO = 200;

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

    Vector2 mouseStartPos;
    Vector2 mousePrevPos;

    /**
     * Listen for mouse presses to add or remove balls.
     */
    void Update()
    {
        // Take note of the starting position
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            mouseStartPos = Input.mousePosition;
            mousePrevPos = mouseStartPos;
            
        // Start the ball if the mouse is down
        } else if(Input.GetKey(KeyCode.Mouse0)) {
            if(ball == null) { 
                Vector2 mouseCurrPos = Input.mousePosition;
                // Check if mouse has moved outside the desired bounds
                float distance = Mathf.Abs(Vector2.Distance(mouseStartPos, mouseCurrPos));
                
                if(distance > DISTANCE_TO_MOVE) {
                    Vector2 startPos = Camera.main.ScreenToWorldPoint(mouseStartPos);
                    // Create the ball
                    ball = Instantiate(
                        ballPrefab, 
                        startPos, 
                        Quaternion.identity
                    );
                    
                    // Limit the speed
                    float speed = Mathf.Abs(Vector2.Distance(mousePrevPos, mouseCurrPos))/Time.deltaTime;
                    speed = Mathf.Clamp(speed, 0, MAX_SPEED);
                    Debug.Log(speed);

                    // Add the force to the ball
                    Vector2 force = (mouseCurrPos - mouseStartPos) * (speed / FORCE_SPEED_RATIO);
                    ball.GetComponent<Ball>().Init(force);
                }
                mousePrevPos = mouseCurrPos;
            }

        // Stop the ball on mouse up
        } else if(Input.GetKeyUp(KeyCode.Mouse0)) {
            mouseStartPos = Vector2.zero;
            Destroy(ball);
            ball = null;
        }
    }
}
