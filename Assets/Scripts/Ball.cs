using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private SpriteRenderer ballRenderer;
    private Rigidbody2D ballRigidBody;
    private SpriteRenderer paintRenderer;

    // Queue of colors to cycle through
    private Queue<Color> colorsQueue = new Queue<Color>(new []{ 
        Color.blue, Color.red, Color.green, Color.yellow, Color.magenta
    });
    private const float LERP_STEPS = 5;

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

    private void FixedUpdate() {
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
        Color color = colorsQueue.Dequeue();

        // Get the ball position on the paint layer
        Vector2 ballPos = Camera.main.WorldToScreenPoint(pos);
        int ballX = (int)(ballPos.x * bgTexture.width / Camera.main.pixelWidth);
        int ballY = (int)(ballPos.y * bgTexture.height / Camera.main.pixelHeight);

        Texture2D ballTexture = ballRenderer.sprite.texture;
        int startX = ballX - (ballTexture.width / 2);
        int startY = ballY - (ballTexture.height / 2);
        for(int x=0; x<ballTexture.width; x++) {
            for(int y=0; y<ballTexture.height; y++) {
                int bgX = startX + x;
                int bgY = startY + y;
                // Don't draw outside of background texture
                if(bgX < 0 || bgY < 0 || bgX >= bgTexture.width || bgY >= bgTexture.height) {
                    continue;
                }
                
                // Ignore clear pixels
                if (ballTexture.GetPixel(x, y).a == 0) {
                    continue;
                }

                bgTexture.SetPixel(bgX, bgY, color);
            }
        }

        bgTexture.Apply();
        colorsQueue.Enqueue(color);
    }
}
