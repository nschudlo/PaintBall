﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject boundingBoxPrefab;
    public GameObject ballPrefab;
    public GameObject instructions;

    private GameObject ball;
    private const float BOUNDING_BOX_WIDTH = 0.1f;
    private const float DISTANCE_TO_MOVE = 50;
    private const float FORCE_SPEED_RATIO = 200;

    private Vector2 mouseStartPos;
    private Vector2 mousePrevPos;

    private float screenAspect;
    private float horizontal;
    private float vertical;
    private float bgWidth;
    private float bgHeight;

    void Start()
    {
        screenAspect = (float)Screen.width / (float)Screen.height;
        horizontal = Camera.main.orthographicSize * screenAspect;
        vertical = Camera.main.orthographicSize;

        // Setup the bounding box
        float xPos = horizontal + (BOUNDING_BOX_WIDTH/2f);
        GameObject leftBox = Instantiate(boundingBoxPrefab, new Vector3(-xPos, 0, 0), Quaternion.identity);
        leftBox.transform.localScale = new Vector3(BOUNDING_BOX_WIDTH, vertical*2, 1);

        GameObject rightBox = Instantiate(boundingBoxPrefab, new Vector3(xPos, 0, 0), Quaternion.identity);
        rightBox.transform.localScale = new Vector3(BOUNDING_BOX_WIDTH, vertical*2, 1);

        float yPos = vertical + (BOUNDING_BOX_WIDTH/2f);
        GameObject topBox = Instantiate(boundingBoxPrefab, new Vector3(0, yPos, 0), Quaternion.identity);
        topBox.transform.localScale = new Vector3(horizontal*2, BOUNDING_BOX_WIDTH, 1);

        GameObject bottomBox = Instantiate(boundingBoxPrefab, new Vector3(0, -yPos, 0), Quaternion.identity);
        bottomBox.transform.localScale = new Vector3(horizontal*2, BOUNDING_BOX_WIDTH, 1);

        bgWidth = horizontal*2*Utility.PIXELS_PER_UNIT;
        bgHeight = vertical*2*Utility.PIXELS_PER_UNIT;

        // Setup the static background sprite
        GameObject.FindGameObjectWithTag("Background")
            .GetComponent<SpriteRenderer>()
            .sprite = Utility.createSprite(
                (int)bgWidth, (int)bgHeight, Color.black
            );

        // Setup the paintable background sprite
        GameObject.FindGameObjectWithTag("Paint")
            .GetComponent<SpriteRenderer>()
            .sprite = Utility.createSprite(
                (int)bgWidth, (int)bgHeight, Color.clear
            );
        
        // Remove instructions so they aren't always there
        Invoke("RemoveInstructions", 10);
    }

    /**
     * Wipe the paint layer
     */
    private void ResetPaintLayer() {
        Utility.setTextureColor(
            GameObject.FindGameObjectWithTag("Paint")
            .GetComponent<SpriteRenderer>().sprite.texture,
            Color.clear
        );
    }

    /**
     * This message will self descruct in 3...2...1...
     */
    private void RemoveInstructions() {
        Destroy(instructions);
    }

    /**
     * Listen for mouse presses to add or remove balls.
     */
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            ResetPaintLayer();
        }

        // Take note of the mouse starting position
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
