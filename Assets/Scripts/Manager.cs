using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Main manager class, responsible for the paint layers and brushes.
 */
public class Manager : MonoBehaviour {
    /**
     * The tag to apply to all boundary edge colliders
     */
    private string BOUNDARY_LAYER_TAG = "Boundary";

    /**
     * The prefab for making new layers
     */
    public GameObject paintBoardLayerPrefab;

    /**
     * The container living in the canvas element to hold
     * the paint board layers
     */
    public GameObject paintBoardContainer;

    /**
     * Reference to the paint board container rect transform
     */
    private RectTransform paintBoardTransform;

    /**
     * Reference to the instructions game object
     */
    public GameObject instructions;

    /**
     * The list holding all the layers
     */
    private List<GameObject> layers = new List<GameObject>();

    /**
     * The currently selected paint board layer
     */
    private RenderTexture currentPaintBoard;

    /**
     * Reference to the current brush
     */
    private IBrush currentBrush;

    /**
     * Setup the bounding edges, first layer, and the initial brush.
     */
    void Start() {
        paintBoardTransform = (RectTransform)paintBoardContainer.transform;

        // Position the edge boundaries at 0,0 in the world space
        // to use them for calculations in the paint board
        float xMin = 0;
        float xMax = xMin + paintBoardTransform.rect.width;
        float yMin = 0;
        float yMax = yMin + paintBoardTransform.rect.height;

        // Setup the bounding box
        GameObject boundingEdges = new GameObject() {
            name = "Bounding edges",
            layer = LayerMask.NameToLayer(BOUNDARY_LAYER_TAG)
        };

        EdgeCollider2D leftEdgeCol = boundingEdges.AddComponent<EdgeCollider2D>();
        leftEdgeCol.SetPoints(new List<Vector2> { new Vector2(xMin, yMin), new Vector2(xMin, yMax) });

        EdgeCollider2D rightEdgeCol = boundingEdges.AddComponent<EdgeCollider2D>();
        rightEdgeCol.SetPoints(new List<Vector2> { new Vector2(xMax, yMin), new Vector2(xMax, yMax) });
        
        EdgeCollider2D topEdgeCol = boundingEdges.AddComponent<EdgeCollider2D>();
        topEdgeCol.SetPoints(new List<Vector2> { new Vector2(xMin, yMax), new Vector2(xMax, yMax) });
        
        EdgeCollider2D bottomEdgeCol = boundingEdges.AddComponent<EdgeCollider2D>();
        bottomEdgeCol.SetPoints(new List<Vector2> { new Vector2(xMin, yMin), new Vector2(xMax, yMin) });

        // Clean up any test components
        foreach (Transform childTransform in paintBoardContainer.transform) {
            Destroy(childTransform.gameObject);
        }

        // Add the first layer
        currentPaintBoard = (RenderTexture)AddPaintLayer().GetComponent<RawImage>().texture;

        // Remove instructions so they aren't always there
        Invoke("RemoveInstructions", 10);
    }

    /**
     * Add a new paint layer
     * @returns the new layer GameObject
     */
    private GameObject AddPaintLayer() {
        GameObject layer = Instantiate(paintBoardLayerPrefab, paintBoardContainer.transform);
        layer.GetComponent<RawImage>().texture = new RenderTexture(
            (int)paintBoardTransform.rect.width, (int)paintBoardTransform.rect.height, 24
        );
        layers.Add(layer);
        return layer;
    }

    /**
     * Wipe the currently selected paint layer
     */
    private void ResetPaintLayer() {
        Utils.ClearOutRenderTexture(currentPaintBoard);
    }

    /**
     * Destroy the on screen instructions.
     */
    private void RemoveInstructions() {
        Destroy(instructions);
    }

    /**
     * Select a new paint brush. Cleans up the old brush and
     * initializes the new one.
     * @param brush
     */
    private void SelectBrush(IBrush brush) {
        if (currentBrush != null) {
            currentBrush.CleanUp();
        }

        currentBrush = brush;
        currentBrush.Init(
            currentPaintBoard,
            paintBoardTransform,
            LayerMask.GetMask(BOUNDARY_LAYER_TAG)
        );
    }

    /**
     * Hook up the brush class with the FixedUpdate lifecycle event
     */
    void FixedUpdate() {
        if (currentBrush == null) {
            // Setup the initial brush
            SelectBrush(new BouncingBallBrush());
        }
        currentBrush.FixedUpdate();
    }

    /**
     * Listen for mouse interactions
     */
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ResetPaintLayer();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            currentBrush.UpdateInputStartPosition(new Vector3(445, 384, 0));
            currentBrush.OnInputStart();
            currentBrush.UpdateInputCurrentPosition(new Vector3(468, 355, 0));
            currentBrush.UpdateInputCurrentPosition(new Vector3(487, 340, 0));
            currentBrush.OnInputMove();
        }

        if (Input.GetKeyUp(KeyCode.R)) {
            currentBrush.OnInputEnd();
        }

        // Tell the brush the mouse was clicked
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector2? pos = getMousePosOnPaintBoard();
            if (pos != null) {
                currentBrush.UpdateInputStartPosition((Vector2) pos);
                currentBrush.OnInputStart();
            }

        // Tell the brush the mouse was moved
        } else if (Input.GetKey(KeyCode.Mouse0)) {
            Vector2? pos = getMousePosOnPaintBoard();
            if (pos != null) {
                currentBrush.UpdateInputCurrentPosition((Vector2) pos);
                currentBrush.OnInputMove();
            }

        // Tell the brush the mouse was released
        } else if (Input.GetKeyUp(KeyCode.Mouse0)) {
            currentBrush.OnInputEnd();
        }
    }

    /**
     * Gets the mouse position over the paint board, where
     * 0,0 if the bottom left, or null if not over it.
     * @returns position over the paint board, or null
     */
    private Vector2? getMousePosOnPaintBoard() {
        if (!RectTransformUtility.RectangleContainsScreenPoint(paintBoardTransform, Input.mousePosition)) {
            return null;
        }

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(paintBoardTransform, Input.mousePosition, null, out localPoint)) {
            return localPoint + new Vector2(paintBoardTransform.rect.width / 2f, paintBoardTransform.rect.height / 2f);
        }

        return null;
    }
}
