using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private SpriteRenderer ballRenderer;
    private Rigidbody2D ballRigidBody;
    private SpriteRenderer paintRenderer;

    // Queue of color pixels to cycle through   
    private Queue<Color[]> colorPixels;
    private Color[] COLORS = {
        Color.blue, Color.red, Color.green, Color.yellow, Color.magenta
    };
    private const int DOT_WIDTH = 20;
    private const float LERP_STEPS = 5;

    // Position on the most recent draw
    private Vector2 previousDrawPos;

    void Awake()
    {
        paintRenderer = GameObject.FindGameObjectWithTag("Paint").GetComponent<SpriteRenderer>();
        ballRenderer = GetComponent<SpriteRenderer>();
        ballRigidBody = GetComponent<Rigidbody2D>();

        previousDrawPos = transform.position;

        // Initialize the color pixel arrays
        colorPixels = new Queue<Color[]>();
        foreach(Color color in COLORS) {
            Color[] pixels = new Color[DOT_WIDTH*DOT_WIDTH];
            for(int idx=0; idx < DOT_WIDTH*DOT_WIDTH; idx++) {
                pixels[idx] = color;
            }
            colorPixels.Enqueue(pixels);
        }
    }

    /**
     * Initialize the ball.
     * @param force
     */
    public void Init(Vector2 force) {
        ballRigidBody.AddForce(force);
    }

    private void Update() {
        Vector2 currentPos = transform.position;

        // Draw another dot if enough distance has 
        if(Mathf.Abs(Vector2.Distance(previousDrawPos, currentPos)) > 0.1) {
            for(float x=0; x < 1; x += (1f/LERP_STEPS)) {
                Vector2 pos = Vector2.Lerp(previousDrawPos, transform.position, x);
                DrawDot(pos);
            }
            previousDrawPos = transform.position;
        }
    }

    /**
     * Draw a dot at a position
     * @param pos
     */
    private void DrawDot(Vector2 pos) {                    
        Texture2D bgTexture = paintRenderer.sprite.texture;

        // Get the ball position on the paint layer
        Vector2 ballPos = Camera.main.WorldToScreenPoint(pos);
        int ballX = (int)(ballPos.x * bgTexture.width / Camera.main.pixelWidth);
        int ballY = (int)(ballPos.y * bgTexture.height / Camera.main.pixelHeight);

        // Draw a dot at that position
        Color[] pixels = colorPixels.Dequeue();
        bgTexture.SetPixels(
            ballX - (DOT_WIDTH/2), 
            ballY - (DOT_WIDTH/2), 
            DOT_WIDTH, 
            DOT_WIDTH, 
            pixels
        );
        colorPixels.Enqueue(pixels);

        bgTexture.Apply();           
    }
}
