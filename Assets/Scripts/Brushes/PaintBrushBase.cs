using UnityEngine;

/**
 * The base painting brush. Draws a circle texture along the input path of the user.
 * To create a different brush, extend this class and modify the following values:
 *   STAMP_RESOURCE - the texture to stamp
 *   DRAW_DISTANCE - the pixels in between the stamps
 *   DEFAULT_SIZE - the default size of the stamp
 */
public class PaintBrushBase : BrushBase {
    /**
     * The default size of the circle
     */
    protected int size = 15;

    /**
     * The frequency to stamp the texture
     */
    protected int drawDistance = 1;

    /**
     * Rotation angle to apply to the stamp
     */
    protected float rotation = 0;

    /**
     * The texture to stamp along the path
     */
    protected Texture2D texture;

    /**
     * The previous location that was drawn to
     */
    private Vector2 previousDrawLocation;

    /**
     * Initialize the brush resouces
     */
    public override void OnInit() {
        if (texture == null) {
            texture = Resources.Load<Texture2D>("Textures/Circle");
        }
    }

    /**
     * Draw a single shape to the screen
     */
    protected override void OnInputStart() {
        StampTextureToRenderTexture(texture, currentInputPosition, size, size, Color.white, rotation);
        previousDrawLocation = currentInputPosition;
    }

    /**
     * Paint to the screen
     */
    protected override void OnInputMove() {
        if (!inputStarted) {
            return;
        }

        Vector2 direction = (Vector2)currentInputPosition - previousDrawLocation;
        float distanceRemaining = direction.magnitude;
        while (distanceRemaining > drawDistance) {
            Vector2 pos = previousDrawLocation + (direction.normalized * drawDistance);
            StampTextureToRenderTexture(texture, pos, size, size, Color.white, rotation);
            previousDrawLocation = pos;
            distanceRemaining -= drawDistance;
        }
    }
}
