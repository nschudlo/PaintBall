using UnityEngine;

/**
 * An enum of the possible current state of this brush
 */
enum EBallState {
    /**
     * The user isn't clicking yet
     */
    NotStarted,
    /**
     * The user has started clicking, but
     * the brush hasn't started yet
     */
    InputStarted,
    /**
     * The user is currently clicking, and the
     * brush has started
     */
    Started
}

/**
 * A bouncing brush ball, leaving a colourful trail where it travels
 */
public class BouncingBallBrush : BaseBrush {
    /**
     * The number of balls to be drawn per interval
     */
    private const int STEPS_PER_INTERVAL = 4;

    /**
     * The distance the mouse needs to move away from the
     * starting position to trigger the ball to start
     */
    private const float DISTANCE_TO_MOVE = 50;

    /**
     * A modifier to apply to the speed to dampen the speed
     * being applied to the ball
     */
    private const float FORCE_SPEED_RATIO = 5000;

    /**
     * The size to stamp the ball
     */
    private const int DRAW_SIZE = 28;

    /**
     * The texture to stamp along the path
     */
    private Texture2D stampTexture;

    /**
     * The material with the shader for stamping textures
     */
    private Material stampMaterial;

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
     * The current velocity the ball is moving
     */
    private Vector2 velocity;

    /**
     * The distance to separate the balls being drawn
     */
    private float distancePerStep;

    /**
     * The starting position of the current line segment
     */
    private Vector2 startPos;

    /**
     * The hit info for next boundary collision
     */
    private RaycastHit2D hitInfo;

    /**
     * The current state of the bouncing ball
     */
    private EBallState currentState;

    /**
     * Initialize the ball movement.
     * @param initialVelocity - the initial velocity to give the ball
     * @param paintboardRT - the paint board render texture to draw to
     * @param stampTexture - the texture to stamp along the path
     */
    public override void Init(RenderTexture paintBoardRT, RectTransform paintBoardTransform) {
        base.Init(paintBoardRT, paintBoardTransform);
        stampMaterial = Resources.Load<Material>("Shaders/StampBlit");
        stampTexture = Resources.Load<Texture2D>("Textures/Ball");
    }

    /**
     * Take note of the starting position, and wait until the user
     * moves a certain distance before starting the brush.
     * @param position
     */
    public override void OnInputStart() {
        currentState = EBallState.InputStarted;
    }

    /**
     * Check if the ball should start moving on screen and start it. If it's already
     * moving do nothing.
     * @param position
     */
    public override void OnInputMove() {
        if (currentState != EBallState.InputStarted) {
            return;
        }

        // Check if input has moved outside the desired bounds
        float distance = Mathf.Abs(Vector2.Distance(inputStartPosition, currentInputPosition));
        if (distance < DISTANCE_TO_MOVE) {
            return;
        }

        // The speed in pixels the mouse is moving as it crosses the threshold
        float inputSpeed = Mathf.Abs(Vector2.Distance(previousInputPosition, currentInputPosition)) / Time.deltaTime;

        // Initialize the ball with an intial velocity
        velocity = (currentInputPosition - inputStartPosition) * (inputSpeed / FORCE_SPEED_RATIO);
        
        previousDrawPos = inputStartPosition;
        distancePerStep = velocity.magnitude / STEPS_PER_INTERVAL;
        UpdateVelocityInfo(inputStartPosition);

        currentState = EBallState.Started;
    }

    /**
     * Called when the user lets go of their input.
     * @param position
     */
    public override void OnInputEnd() {
        currentState = EBallState.NotStarted;
    }

    /**
     * Update the ball's velocity information, including calculating the next
     * point the ball will hit a boundary.
     * @param startPos
     */
    private void UpdateVelocityInfo(Vector2 startPos) {
        this.startPos = startPos;

        Collider2D prevCollider = null;
        if (currentState == EBallState.Started) {
            // Determine the new reflected velocity
            velocity = Vector2.Reflect(velocity, hitInfo.normal);

            // Disable the previous collider so the next hit can be
            // calculated without interference
            prevCollider = hitInfo.collider;
            prevCollider.enabled = false;
        }

        hitInfo = Physics2D.Raycast(startPos, velocity);

        if (prevCollider) {
            prevCollider.enabled = true;
        }
    }

    /**
     * Draw the next batch of dots.
     */
    public override void FixedUpdate() {
        if (currentState == EBallState.Started) {
            DrawDots();
        }
    }

    /**
     * Draw dots to fill in the positions from the most
     * recently drawn dot to the targetPos.
     */
    private void DrawDots() {
        Color color = COLOURS[colourIdx++];
        colourIdx = colourIdx % COLOURS.Length;

        for (int stepIdx = 0; stepIdx < STEPS_PER_INTERVAL; stepIdx++) {
            Vector2 pos = previousDrawPos + (velocity.normalized * distancePerStep);

            // Check if the new pos goes beyond the boundary
            if (!Utils.IsPointBetweenAB(startPos, hitInfo.point, pos)) {
                pos = hitInfo.point;

                // Update the velocity with the new starting position and angle
                UpdateVelocityInfo(pos);
            }

            DrawDot(pos, color);
            previousDrawPos = pos;
        }
    }

    /**
     * Draw a dot at a position
     * @param pos
     * @param color
     */
    private void DrawDot(Vector2 pos, Color color) {
        // Get the position on the paint board layer
        Vector2 rtPos = new Vector2(
            (int)(pos.x * paintBoardRT.width / paintBoardTransform.rect.width),
            (int)(pos.y * paintBoardRT.height / paintBoardTransform.rect.height)
        );
        StampTextureToRenderTexture(paintBoardRT, stampTexture, rtPos, stampMaterial, DRAW_SIZE, DRAW_SIZE, color);
    }

    /**
     * Stamps a texture onto a render texture at a given x, y coordinate, where 0,0
     * is the bottom right corner of the render texture
     * @param renderTexture - the target render texture
     * @param stampTexture - the texture to stamp
     * @param positionPixels - the position to place the stamp
     * @param stampMat - the material used to stamp
     * @param stampWidth - the width to stamp the stampTexture
     * @param stampHeight - the height to stamp the stampTexture
     * @param color - color to apply to the stamp
     */
    public void StampTextureToRenderTexture(RenderTexture renderTexture, Texture2D stampTexture, Vector2 positionPixels, Material stampMat, int stampWidth, int stampHeight, Color color) {
        stampMat.SetTexture("_StampTex", stampTexture);

        // Position must be converted to UV space (0 to 1)
        Vector2 positionUV = new Vector2(
            positionPixels.x / renderTexture.width,
            positionPixels.y / renderTexture.height
        );
        stampMat.SetVector("_StampPositionUV", positionUV);

        // Pass the size of the circle for the shader to use
        stampMat.SetFloat("_StampWidthPixels", stampWidth);
        stampMat.SetFloat("_StampHeightPixels", stampHeight);

        stampMat.SetColor("_StampColor", color);

        // Use the custom material/shader to stamp the circle onto the buffer
        RenderTexture tempRT = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, renderTexture.format);
        Graphics.Blit(renderTexture, tempRT);
        Graphics.Blit(tempRT, renderTexture, stampMat);

        RenderTexture.ReleaseTemporary(tempRT);
    }
}
