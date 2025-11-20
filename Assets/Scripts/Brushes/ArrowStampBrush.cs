using UnityEngine;

/**
 * Draws a trail of four arrow symbols with random rotations.
 */
public class ArrowStampBrush : PaintBrushBase {
    /**
     * Initialize the brush values
     */
    public ArrowStampBrush() {
        size = 30;
        drawDistance = 50;
        texture = Resources.Load<Texture2D>("Textures/Arrows-Four");
        rotation = Random.Range(0, 360);
    }

    /**
     * Randomly rotate the brush
     */
    protected override void OnInputMove() {
        rotation = Random.Range(0, 360);
        base.OnInputMove();
    }
}
