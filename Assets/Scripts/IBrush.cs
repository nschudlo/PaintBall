
using UnityEngine;

/// <summary>
/// An interface to define the properties of a brush.
/// </summary>
public interface IBrush {
    /// <summary>
    /// Initialization function
    /// </summary>
    /// <param name="paintBoardRT">The render texture to paint on</param>
    /// <param name="paintBoardTransform">The rect transform of the paint board</param>
    /// <param name="boundaryMask">A layer mask for the boundary edge colliders</param>
    void Init(RenderTexture paintBoardRT, RectTransform paintBoardTransform, LayerMask boundaryMask);

    /// <summary>
    /// Clean up the brush
    /// </summary>
    void CleanUp();

    /// <summary>
    /// Run updates at a fixed interval.
    /// </summary>
    void FixedUpdate();

    /// <summary>
    /// Update the start input position. Called by the manager when the an
    /// input starts on the paint board. 
    /// </summary>
    /// <param name="position">The input start coordinates, where 0,0 is
    /// the bottom left of the paint board</param>
    void UpdateInputStartPosition(Vector3 position);

    /// <summary>
    /// What to do with this brush when the user starts an input
    /// </summary>
    void OnInputStart();

    /// <summary>
    /// Update the current input position. Called by the manager when an
    /// input is in progress and the pointer moves.
    /// </summary>
    /// <param name="position">The current input coordinates, where 0,0 is the
    /// bottom left of the paint board</param>
    void UpdateInputCurrentPosition(Vector3 position);

    /// <summary>
    /// What to do with this brush when the user moves after starting an input
    /// </summary>
    void OnInputMove();

    /// <summary>
    /// What to do with this brush when the user ends an input
    /// </summary>
    void OnInputEnd();
}
