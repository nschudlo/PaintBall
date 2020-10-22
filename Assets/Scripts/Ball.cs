using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private SpriteRenderer ballRenderer;
    private Rigidbody2D ballRigidBody;
    private TextureManager textureManager;

    private float bgWidth;
    private float bgHeight;

    // Queue of colors to cycle through
    private Queue<Color> colorsQueue = new Queue<Color>(new []{ 
        Color.blue, Color.green, Color.yellow, Color.red, Color.magenta
    });

    // The number of balls to be drawn per second
    private const float BALLS_PER_SECOND = 200f;

    // Position on the most recent draw
    private Vector2 previousDrawPos;

    void Awake()
    {
        SpriteRenderer background = GameObject.FindGameObjectWithTag("Background")
            .GetComponent<SpriteRenderer>();
        bgWidth = background.sprite.texture.width;
        bgHeight = background.sprite.texture.height;

        textureManager = GameObject.FindObjectOfType<TextureManager>();
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
        DrawDots(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Draw the dots to the contact position. This
        // makes sure dots are drawn right up to the wall.
        DrawDots(collision.contacts[0].point);   
    }

    /**
     * Draw dots to fill in the positions from the most
     * recently drawn dot to the targetPos.
     * @param targetPos
     */
    private void DrawDots(Vector3 targetPos) {
        float steps = Time.deltaTime * BALLS_PER_SECOND;
        for (float t=0; t < 1; t += (1f/steps)) {
            Vector2 pos = Vector2.Lerp(previousDrawPos, targetPos, t);
            DrawDot(pos);
        }

        textureManager.Apply();
        previousDrawPos = targetPos;
    }

    /**
     * Draw a dot at a position
     * @param pos
     */
    private void DrawDot(Vector2 pos) {
        Color color = colorsQueue.Dequeue();

        // Get the ball position on the paint layer
        Vector2 ballPos = Camera.main.WorldToScreenPoint(pos);
        int ballX = (int)(ballPos.x * bgWidth / Camera.main.pixelWidth);
        int ballY = (int)(ballPos.y * bgHeight / Camera.main.pixelHeight);

        Texture2D ballTexture = ballRenderer.sprite.texture;
        int startX = ballX - (ballTexture.width / 2);
        int startY = ballY - (ballTexture.height / 2);
        for(int x=0; x<ballTexture.width; x++) {
            for(int y=0; y<ballTexture.height; y++) {
                int bgX = startX + x;
                int bgY = startY + y;
                // Don't draw outside of background texture
                if(bgX < 0 || bgY < 0 || bgX >= bgWidth || bgY >= bgHeight) {
                    continue;
                }
                
                // Ignore clear pixels
                if (ballTexture.GetPixel(x, y).a == 0) {
                    continue;
                }

                textureManager.SetPixel(bgX, bgY, color);
            }
        }
        colorsQueue.Enqueue(color);
    }
}
