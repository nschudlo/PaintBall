using UnityEngine;

/**
 * Handles drawing the trail of balls behind the ball object
 */
public class Ball : MonoBehaviour {
    /**
     * The number of balls to be drawn per second
     */
    private const float BALLS_PER_SECOND = 200f;

    /**
     * The minimum time to wait before calling draw
     * again. This doesn't prevent drawing dots when
     * the ball collides with the walls.
     */
    private const float TIME_BETWEEN_DRAWS = 0.05f;

    /**
     * Reference to the ball renderer
     */
    private SpriteRenderer ballRenderer;

    /**
     * Reference to the physics ball object
     */
    private Rigidbody2D ballRigidBody;

    /**
     * Reference to the sprite renderer for the background
     */
    private SpriteRenderer paintRenderer;

    /**
     * List of colors to cycle through
     */
    private readonly Color[] COLOURS = {
        Color.blue, Color.green, Color.yellow, Color.red, Color.magenta
    };
    private int colourIdx = 0;

    /**
     * Position on the most recent draw
     */
    private Vector2 previousDrawPos;

    /**
     * Time that has elapsed since the most recent draw event
     */
    float elapsed = 0f;

    /**
     * Handle getting references to needed components.
     */
    void Awake() {
        paintRenderer = GameObject.FindGameObjectWithTag("Paint")
            .GetComponent<SpriteRenderer>();
        ballRenderer = GetComponent<SpriteRenderer>();
        ballRigidBody = GetComponent<Rigidbody2D>();

        previousDrawPos = transform.position;
    }

    /**
     * Initialize the ball movement.
     * @param force
     */
    public void Init(Vector2 force) {
        ballRigidBody.AddForce(force);
    }

    /**
     * Check if enough time has passed to draw the
     * next batch of dots.
     */
    private void FixedUpdate() {
        elapsed += Time.deltaTime;
        if (elapsed >= TIME_BETWEEN_DRAWS) {
            DrawDots(transform.position);
        }
    }

    /**
     * Handles the case where the ball hits the edge, so the
     * trail of balls needs to be drawn early.
     */
    private void OnCollisionEnter2D(Collision2D collision) {
        elapsed += Time.deltaTime;
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
        float steps = elapsed * BALLS_PER_SECOND;
        for (float t = 0; t < 1; t += 1f / steps) {
            Vector2 pos = Vector2.Lerp(previousDrawPos, targetPos, t);
            DrawDot(paintRenderer.sprite.texture, pos);
        }

        paintRenderer.sprite.texture.Apply();
        previousDrawPos = targetPos;
        elapsed = 0f;
    }

    /**
     * Draw a dot at a position
     * @param bgTexture
     * @param pos
     */
    private void DrawDot(Texture2D bgTexture, Vector2 pos) {
        Color color = COLOURS[colourIdx++];
        colourIdx = colourIdx % COLOURS.Length;

        // Get the ball position on the paint layer
        Vector2 ballPos = Camera.main.WorldToScreenPoint(pos);
        int ballX = (int)(ballPos.x * bgTexture.width / Camera.main.pixelWidth);
        int ballY = (int)(ballPos.y * bgTexture.height / Camera.main.pixelHeight);

        Texture2D ballTexture = ballRenderer.sprite.texture;
        int startX = ballX - (ballTexture.width / 2);
        int startY = ballY - (ballTexture.height / 2);
        for (int x = 0; x < ballTexture.width; x++) {
            for (int y = 0; y < ballTexture.height; y++) {
                int bgX = startX + x;
                int bgY = startY + y;
                // Don't draw outside of background texture
                if (bgX < 0 || bgY < 0 || bgX >= bgTexture.width || bgY >= bgTexture.height) {
                    continue;
                }

                // Ignore clear pixels
                if (ballTexture.GetPixel(x, y).a == 0) {
                    continue;
                }

                bgTexture.SetPixel(bgX, bgY, color);
            }
        }
    }
}
