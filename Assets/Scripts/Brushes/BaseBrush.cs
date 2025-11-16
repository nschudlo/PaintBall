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

    /// <inheritdoc/>
    public virtual void Init(RenderTexture paintBoardRT, RectTransform paintBoardTransform, LayerMask boundaryMask) {
        _paintBoardRT = paintBoardRT;
        _paintBoardTransform = paintBoardTransform;
        _boundaryMask = boundaryMask;
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
}
