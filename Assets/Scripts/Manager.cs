using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
    /**
     * Reference to the prefab used to build the bounding box
     * around the paint board container
     */
    public GameObject boundingBoxPrefab;


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
     * Reference to the instructions game object
     */
    public GameObject instructions;

    /**
     * The thickness to set the bounding boxes to
     */
    private const float BOUNDING_BOX_THICKNESS = 0.1f;

    private float screenAspect;
    private float horizontal;
    private float vertical;
    private float bgWidth;
    private float bgHeight;

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

    void Start() {
        screenAspect = (float)Screen.width / (float)Screen.height;
        horizontal = Camera.main.orthographicSize * screenAspect;
        vertical = Camera.main.orthographicSize;

        // Setup the bounding box
        float xPos = horizontal + (BOUNDING_BOX_THICKNESS / 2f);
        GameObject leftBox = Instantiate(boundingBoxPrefab, new Vector3(-xPos, 0, 0), Quaternion.identity);
        leftBox.transform.localScale = new Vector3(BOUNDING_BOX_THICKNESS, vertical * 2, 1);

        GameObject rightBox = Instantiate(boundingBoxPrefab, new Vector3(xPos, 0, 0), Quaternion.identity);
        rightBox.transform.localScale = new Vector3(BOUNDING_BOX_THICKNESS, vertical * 2, 1);

        float yPos = vertical + (BOUNDING_BOX_THICKNESS / 2f);
        GameObject topBox = Instantiate(boundingBoxPrefab, new Vector3(0, yPos, 0), Quaternion.identity);
        topBox.transform.localScale = new Vector3(horizontal * 2, BOUNDING_BOX_THICKNESS, 1);

        GameObject bottomBox = Instantiate(boundingBoxPrefab, new Vector3(0, -yPos, 0), Quaternion.identity);
        bottomBox.transform.localScale = new Vector3(horizontal * 2, BOUNDING_BOX_THICKNESS, 1);

        bgWidth = horizontal * 2 * Utils.PIXELS_PER_UNIT;
        bgHeight = vertical * 2 * Utils.PIXELS_PER_UNIT;

        // Setup the static background sprite
        GameObject.FindGameObjectWithTag("Background")
            .GetComponent<SpriteRenderer>()
            .sprite = Utils.createSprite(
                (int)bgWidth, (int)bgHeight, Color.black
            );

        // Clean up any test components
        foreach (Transform childTransform in paintBoardContainer.transform) {
            Destroy(childTransform.gameObject);
        }

        // Add the first layer
        AddPaintLayer();
        currentPaintBoard = (RenderTexture)layers[0].GetComponent<RawImage>().texture;

        // Remove instructions so they aren't always there
        Invoke("RemoveInstructions", 10);

        // Setup the initial brush
        SelectBrush(new BouncingBallBrush());
    }

    /**
     * Add a new paint layer
     * @returns the new layer GameObject
     */
    private GameObject AddPaintLayer() {
        GameObject layer = Instantiate(paintBoardLayerPrefab, paintBoardContainer.transform);
        layer.GetComponent<RawImage>().texture = new RenderTexture(
            (int)bgWidth, (int)bgHeight, 24
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
        // TODO destroy old brush

        currentBrush = brush;
        currentBrush.Init(currentPaintBoard);
    }

    /**
     * Hook up the brush class with the FixedUpdate lifecycle event
     */
    void FixedUpdate() {
        currentBrush.FixedUpdate();
    }

    /**
     * Listen for mouse presses to add or remove balls
     */
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ResetPaintLayer();
        }

        // Take note of the mouse starting position
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            currentBrush.OnInputStart(Input.mousePosition);

        // Start the ball if the mouse is down
        } else if (Input.GetKey(KeyCode.Mouse0)) {
            currentBrush.OnInputMove(Input.mousePosition);

        // Stop the ball on mouse up
        } else if (Input.GetKeyUp(KeyCode.Mouse0)) {
            currentBrush.OnInputEnd(Input.mousePosition);
        }
    }
}
