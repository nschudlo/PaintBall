
using UnityEngine;

/// <summary>
/// An interface to define the properties of a brush.
/// </summary>
public interface IBrush {
    /// <summary>
    /// Initialization function
    /// </summary>
    /// <param name="paintBoardRT">The render texture to paint on</param>
    void Init(RenderTexture paintBoardRT);

    /// <summary>
    /// Run updates at a fixed interval.
    /// </summary>
    void FixedUpdate();

    /// <summary>
    /// What to do with this brush when the user starts an input
    /// </summary>
    /// <param name="position">The position of the input</param>
    void OnInputStart(Vector3 position);

    /// <summary>
    /// What to do with this brush when the user moves after starting an input
    /// </summary>
    /// <param name="position">The new position input position</param>
    void OnInputMove(Vector3 position);

    /// <summary>
    /// What to do with this brush when the user ends an input
    /// </summary>
    /// <param name="position">The position the input ended</param>
    void OnInputEnd(Vector3 position);
}
