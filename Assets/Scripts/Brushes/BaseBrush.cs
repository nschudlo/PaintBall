using UnityEngine;

/// <summary>
/// The base brush class
/// </summary>
public abstract class BaseBrush : IBrush {
    /// <summary>
    /// Reference to the render texture being painted on
    /// </summary>
    protected RenderTexture paintBoardRT;

    /// <summary>
    /// The start position of the user input
    /// </summary>
    protected Vector3 inputStartPosition;

    /// <summary>
    /// The current position of the input
    /// </summary>
    protected Vector3 currentInputPosition;

    /// <summary>
    /// The previous input position
    /// </summary>
    protected Vector3 previousInputPosition;

    /// <inheritdoc/>
    public virtual void Init(RenderTexture paintBoardRT) {
        this.paintBoardRT = paintBoardRT;
    }

    /// <inheritdoc/>
    public virtual void FixedUpdate() {
    }

    /// <inheritdoc/>
    public virtual void OnInputStart(Vector3 position) {
        inputStartPosition = position;
        currentInputPosition = position;
        previousInputPosition = position;
    }

    /// <inheritdoc/>
    public virtual void OnInputMove(Vector3 position) {
        previousInputPosition = currentInputPosition;
        currentInputPosition = position;
    }

    /// <inheritdoc/>
    public virtual void OnInputEnd(Vector3 position) {
    }
}
