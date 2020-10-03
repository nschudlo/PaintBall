using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Color[] colors = {
        Color.blue, Color.red, Color.green, Color.yellow, Color.magenta
    };
    private int colorIdx = 0;

    private SpriteRenderer ballRenderer;
    private Rigidbody2D ballRigidBody;
    private SpriteRenderer paintRenderer;

    // Position on the most recent draw
    private Vector2 previousDrawPos;

    void Awake()
    {
        paintRenderer = GameObject.FindGameObjectWithTag("Paint").GetComponent<SpriteRenderer>();
        ballRenderer = GetComponent<SpriteRenderer>();
        ballRigidBody = GetComponent<Rigidbody2D>();

        previousDrawPos = transform.position;
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
        if(Mathf.Abs(Vector2.Distance(previousDrawPos, currentPos)) > 0.1) {
            DrawDot();
        }
    }

    /**
     * Draw a dot at the current position
     */
    private void DrawDot() {
        previousDrawPos = transform.position;
        int index = colorIdx++ % colors.Length;
            
        // Figure out the ball position on the background
        int ballX = (int)((transform.position.x * Utility.PIXELS_PER_UNIT) + (Screen.width / 2));
        int ballY = (int)((transform.position.y * Utility.PIXELS_PER_UNIT) + (Screen.height / 2));
        // Vector2 ballPos = Camera.main.WorldToScreenPoint(transform.position);
        // int ballX = (int)ballPos.x;
        // int ballY = (int)ballPos.y;

        Texture2D bgTexture = paintRenderer.sprite.texture;

        // Draw a dot at that position
        int width = 10;
        for(int x = -width; x < width; x++) {
            for(int y = -width; y < width; y++) {
                bgTexture.SetPixel(
                    ballX + x, 
                    ballY + y,
                    colors[index]
                );
            }
        }
        bgTexture.Apply();           
    }
}
