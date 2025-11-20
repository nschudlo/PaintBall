using UnityEngine;

/// <summary>
/// The base brush class
/// </summary>
public abstract class BrushBase : IBrush {
    /// <summary>
    /// Reference to the render texture being painted on
    /// </summary>
    protected RenderTexture paintBoardRT { get; private set; }

    /// <summary>
    /// Reference to the paint board rect transform
    /// </summary>
    protected RectTransform paintBoardTransform { get; private set; }

    /// <summary>
    /// Reference to a layer mask to filter for the boundary edge colliders
    /// </summary>
    protected LayerMask boundaryMask { get; private set; }
    
    /// <summary>
    /// The start position of the user input
    /// </summary>
    protected Vector3 inputStartPosition { get; private set; }

    /// <summary>
    /// The current position of the input
    /// </summary>
    protected Vector3 currentInputPosition { get; private set; }

    /// <summary>
    /// The previous input position
    /// </summary>
    protected Vector3 previousInputPosition { get; private set; }

    /// <summary>
    /// Flag to indicate the 
    /// </summary>
    protected bool inputStarted { get; private set; }
    
    /**
     * The material with the shader for stamping textures
     */
    private Material stampMaterial;

    /// <inheritdoc/>
    public virtual void Init(RenderTexture paintBoardRT, RectTransform paintBoardTransform, LayerMask boundaryMask) {
        this.paintBoardRT = paintBoardRT;
        this.paintBoardTransform = paintBoardTransform;
        this.boundaryMask = boundaryMask;

        stampMaterial = Resources.Load<Material>("Shaders/StampBlit");
        OnInit();
    }

    /// <summary>
    /// Called after the brush has been initialized
    /// </summary>
    public virtual void OnInit() {
    }

    /// <inheritdoc/>
    public virtual void CleanUp() {
    }

    /// <inheritdoc/>
    public virtual void FixedUpdate() {
    }

    /// <inheritdoc/>
    public void StartInput(Vector3 position) {
        inputStartPosition = position;
        currentInputPosition = position;
        previousInputPosition = position;
        inputStarted = true;
        OnInputStart();
    }

    /// <inheritdoc/>
    public void MoveInput(Vector3 position) {
        previousInputPosition = currentInputPosition;
        currentInputPosition = position;
        OnInputMove();
    }

    /// <inheritdoc/>
    public void EndInput() {
        inputStarted = false;
        OnInputEnd();
    }

    /// <summary>
    /// Called when a new input has started
    /// </summary>
    protected virtual void OnInputStart() {}

    /// <summary>
    /// Called when an active input has moved
    /// </summary>
    protected virtual void OnInputMove() {}

    /// <summary>
    /// Called when an input has ended
    /// </summary>
    protected virtual void OnInputEnd() {}

    /**
     * Print a debug log of the current state of the brush input
     */
    public void PrintInputDebug() {
        Debug.Log($"Starting: {inputStartPosition} Previous: {previousInputPosition} Current: {currentInputPosition} TimeDelta: {Time.deltaTime}");
    }

    /**
     * Stamps a texture onto a render texture at a given x, y coordinate, where 0,0
     * is the bottom right corner of the render texture
     * @param stampTexture - the texture to stamp
     * @param positionPixels - the position to place the stamp
     * @param stampWidth - the width to stamp the stampTexture
     * @param stampHeight - the height to stamp the stampTexture
     * @param color - color to apply to the stamp
     */
    public void StampTextureToRenderTexture(Texture2D stampTexture, Vector2 positionPixels, int stampWidth, int stampHeight, Color color) {
        stampMaterial.SetTexture("_StampTex", stampTexture);

        // Position must be converted to UV space (0 to 1)
        Vector2 positionUV = new Vector2(
            positionPixels.x / paintBoardRT.width,
            positionPixels.y / paintBoardRT.height
        );
        stampMaterial.SetVector("_StampPositionUV", positionUV);

        // Pass the size of the circle for the shader to use
        stampMaterial.SetFloat("_StampWidthPixels", stampWidth);
        stampMaterial.SetFloat("_StampHeightPixels", stampHeight);

        stampMaterial.SetColor("_StampColor", color);

        // Use the custom material/shader to stamp the circle onto the buffer
        RenderTexture tempRT = RenderTexture.GetTemporary(paintBoardRT.width, paintBoardRT.height, 0, paintBoardRT.format);
        Graphics.Blit(paintBoardRT, tempRT);
        Graphics.Blit(tempRT, paintBoardRT, stampMaterial);

        RenderTexture.ReleaseTemporary(tempRT);
    }
}
