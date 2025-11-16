using UnityEngine;

/// <summary>
/// The base brush class
/// </summary>
public abstract class BaseBrush : IBrush {
    /// <summary>
    /// Reference to the render texture being painted on
    /// </summary>
    protected RenderTexture paintBoardRT {
        get { return _paintBoardRT; }
    }
    private RenderTexture _paintBoardRT;

    /// <summary>
    /// Reference to the paint board rect transform
    /// </summary>
    protected RectTransform paintBoardTransform {
        get { return _paintBoardTransform; }
    }
    private RectTransform _paintBoardTransform;

    /// <summary>
    /// Reference to a layer mask to filter for the boundary edge colliders
    /// </summary>
    protected LayerMask boundaryMask {
        get { return _boundaryMask; }
    }
    private LayerMask _boundaryMask;

    /// <summary>
    /// The start position of the user input
    /// </summary>
    protected Vector3 inputStartPosition {
        get { return _inputStartPosition; }
    }
    private Vector3 _inputStartPosition;

    /// <summary>
    /// The current position of the input
    /// </summary>
    protected Vector3 currentInputPosition {
        get { return _currentInputPosition; }
    }
    private Vector3 _currentInputPosition;

    /// <summary>
    /// The previous input position
    /// </summary>
    protected Vector3 previousInputPosition {
        get { return _previousInputPosition; }
    }
    private Vector3 _previousInputPosition;

    /**
     * The material with the shader for stamping textures
     */
    private Material _stampMaterial;

    /// <inheritdoc/>
    public virtual void Init(RenderTexture paintBoardRT, RectTransform paintBoardTransform, LayerMask boundaryMask) {
        _paintBoardRT = paintBoardRT;
        _paintBoardTransform = paintBoardTransform;
        _boundaryMask = boundaryMask;

        _stampMaterial = Resources.Load<Material>("Shaders/StampBlit");
    }

    /// <inheritdoc/>
    public virtual void CleanUp() {
    }

    /// <inheritdoc/>
    public virtual void FixedUpdate() {
    }

    /// <inheritdoc/>
    public virtual void OnInputStart() {
    }

    /// <inheritdoc/>
    public virtual void UpdateInputStartPosition(Vector3 position) {
        _inputStartPosition = position;
        _currentInputPosition = position;
        _previousInputPosition = position;
    }

    /// <inheritdoc/>
    public virtual void OnInputMove() {
    }

    /// <inheritdoc/>
    public virtual void UpdateInputCurrentPosition(Vector3 position) {
        _previousInputPosition = currentInputPosition;
        _currentInputPosition = position;
    }

    /// <inheritdoc/>
    public virtual void OnInputEnd() {
    }

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
        _stampMaterial.SetTexture("_StampTex", stampTexture);

        // Position must be converted to UV space (0 to 1)
        Vector2 positionUV = new Vector2(
            positionPixels.x / _paintBoardRT.width,
            positionPixels.y / _paintBoardRT.height
        );
        _stampMaterial.SetVector("_StampPositionUV", positionUV);

        // Pass the size of the circle for the shader to use
        _stampMaterial.SetFloat("_StampWidthPixels", stampWidth);
        _stampMaterial.SetFloat("_StampHeightPixels", stampHeight);

        _stampMaterial.SetColor("_StampColor", color);

        // Use the custom material/shader to stamp the circle onto the buffer
        RenderTexture tempRT = RenderTexture.GetTemporary(_paintBoardRT.width, _paintBoardRT.height, 0, _paintBoardRT.format);
        Graphics.Blit(_paintBoardRT, tempRT);
        Graphics.Blit(tempRT, _paintBoardRT, _stampMaterial);

        RenderTexture.ReleaseTemporary(tempRT);
    }
}
