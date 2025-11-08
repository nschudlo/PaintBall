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
     * Reference to the ball renderer
     */
    private SpriteRenderer ballRenderer;

    /**
     * Reference to the physics ball object
     */
    private Rigidbody2D ballRigidBody;

    /**
     * Reference to the paintboard render texture
     */
    private RenderTexture paintboardRT;

    /**
     * The material with the shader for stamping textures
     */
    public Material stampMaterial;

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
     * Handle getting references to needed components.
     */
    void Awake() {
        ballRenderer = GetComponent<SpriteRenderer>();
        ballRigidBody = GetComponent<Rigidbody2D>();

        previousDrawPos = transform.position;
    }

    /**
     * Initialize the ball movement.
     * @param force
     */
    public void Init(Vector2 force, RenderTexture paintboardRT) {
        ballRigidBody.AddForce(force);
        this.paintboardRT = paintboardRT;
    }

    /**
     * Draw the next batch of dots.
     */
    private void FixedUpdate() {
        DrawDots(transform.position);
    }

    /**
     * Draw dots to fill in the positions from the most
     * recently drawn dot to the targetPos.
     * @param targetPos
     */
    private void DrawDots(Vector3 targetPos) {
        Color color = COLOURS[colourIdx++];
        colourIdx = colourIdx % COLOURS.Length;

        float steps = Time.fixedDeltaTime * BALLS_PER_SECOND;
        for (float t = 0; t < 1; t += 1f / steps) {
            Vector2 pos = Vector2.Lerp(previousDrawPos, targetPos, t);
            DrawDot(pos, color);
        }

        previousDrawPos = targetPos;
    }

    /**
     * Draw a dot at a position
     * @param pos
     * @param color
     */
    private void DrawDot(Vector2 pos, Color color) {
        // Get the ball position on the paint layer
        Vector2 ballPos = Camera.main.WorldToScreenPoint(pos);
        int ballX = (int)(ballPos.x * paintboardRT.width / Camera.main.pixelWidth);
        int ballY = (int)(ballPos.y * paintboardRT.height / Camera.main.pixelHeight);

        Texture2D ballTexture = ballRenderer.sprite.texture;
        StampTextureToRenderTexture(paintboardRT, ballTexture, new Vector2(ballX, ballY), stampMaterial, new Vector2(27, 27), color);
    }

    /**
     * Stamps a texture onto a render texture at a given x, y coordinate
     * @param renderTexture - the target render texture
     * @param stampTexture - the texture to stamp
     * @param positionPixels - the position to place the stamp
     * @param stampMat - the material used to stamp
     * @param targetSize - the target size of the texture
     * @param color - color to apply to the stamp
     */
    public void StampTextureToRenderTexture(RenderTexture renderTexture, Texture2D stampTexture, Vector2 positionPixels, Material stampMat, Vector2 targetSize, Color color) {
        stampMat.SetTexture("_StampTex", stampTexture);

        // Position must be converted to UV space (0 to 1)
        Vector2 positionUV = new Vector2(
            positionPixels.x / renderTexture.width,
            positionPixels.y / renderTexture.height
        );
        stampMat.SetVector("_StampPositionUV", positionUV);

        // Pass the size of the circle for the shader to use
        // TODO fix this so the circle is the same size as the physics ball
        stampMat.SetFloat("_StampWidthPixels", targetSize.x);
        stampMat.SetFloat("_StampHeightPixels", targetSize.y);

        stampMat.SetColor("_StampColor", color);

        // Use the custom material/shader to stamp the circle onto the buffer
        RenderTexture tempRT = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, renderTexture.format);
        Graphics.Blit(renderTexture, tempRT);
        Graphics.Blit(tempRT, renderTexture, stampMat);

        RenderTexture.ReleaseTemporary(tempRT);
    }
}
