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
    private readonly int DEFAULT_SIZE = 15;

    /**
     * The name of the texture resource to stamp along the input path
     */
    protected readonly string STAMP_RESOURCE = "Textures/Circle";

    /**
     * The frequency to stamp the texture
     */
    protected readonly int DRAW_DISTANCE = 1;

    /**
     * The texture to stamp along the path
     */
    private Texture2D stampTexture;

    /**
     * The previous location that was drawn to
     */
    private Vector2 previousDrawLocation;

    /**
     * Initialize the brush resouces
     */
    public override void OnInit() {
        stampTexture = Resources.Load<Texture2D>(STAMP_RESOURCE);
    }

    /**
     * Draw a single shape to the screen
     */
    protected override void OnInputStart() {
        StampTextureToRenderTexture(stampTexture, currentInputPosition, DEFAULT_SIZE, DEFAULT_SIZE, Color.white);
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
        while (distanceRemaining > DRAW_DISTANCE) {
            Vector2 pos = previousDrawLocation + (direction.normalized * DRAW_DISTANCE);
            StampTextureToRenderTexture(stampTexture, pos, DEFAULT_SIZE, DEFAULT_SIZE, Color.white);
            previousDrawLocation = pos;
            distanceRemaining -= DRAW_DISTANCE;
        }
    }
}
