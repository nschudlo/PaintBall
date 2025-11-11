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
     * The ball prefab
     * // TODO move this to a paint brush class
     */
    public GameObject ballPrefab;

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
     * Reference to the current ball on screen
     * // TODO move this to a paint brush class
     */
    private GameObject ball;

    /**
     * The thickness to set the bounding boxes to
     */
    private const float BOUNDING_BOX_THICKNESS = 0.1f;

    /**
     * The distance the mouse needs to move away from the
     * starting position to trigger the ball to start
     */
    private const float DISTANCE_TO_MOVE = 50;

    /**
     * A modifier to apply to the speed to dampen the speed
     * being applied to the ball
     */
    private const float FORCE_SPEED_RATIO = 1000;

    /**
     * The position the mouse was on mouse down
     */
    private Vector2 mouseStartPos;

    /**
     * The position the mouse was on the last update
     */
    private Vector2 mousePrevPos;

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
    }

    private void AddPaintLayer() {
        GameObject layer = Instantiate(paintBoardLayerPrefab, paintBoardContainer.transform);
        layer.GetComponent<RawImage>().texture = new RenderTexture(
            (int)bgWidth, (int)bgHeight, 24
        );
        layers.Add(layer);
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
     * Listen for mouse presses to add or remove balls.
     */
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ResetPaintLayer();
        }

        // Take note of the mouse starting position
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            mouseStartPos = Input.mousePosition;
            mousePrevPos = mouseStartPos;

        // Start the ball if the mouse is down
        } else if (Input.GetKey(KeyCode.Mouse0)) {
            if (ball == null) {
                Vector2 mouseCurrPos = Input.mousePosition;
                // Check if mouse has moved outside the desired bounds
                float distance = Mathf.Abs(Vector2.Distance(mouseStartPos, mouseCurrPos));

                if (distance > DISTANCE_TO_MOVE) {
                    Vector2 startPos = Camera.main.ScreenToWorldPoint(mouseStartPos);
                    // Create the ball
                    ball = Instantiate(
                        ballPrefab,
                        startPos, 
                        Quaternion.identity
                    );

                    // The speed in pixels the mouse is moving as it crosses the threshold
                    float speed = Mathf.Abs(Vector2.Distance(mousePrevPos, mouseCurrPos)) / Time.deltaTime;

                    // Initialize the ball with an intial velocity
                    Vector2 ballVelocity = (mouseCurrPos - mouseStartPos) * (speed / Utils.PIXELS_PER_UNIT) * (1 / FORCE_SPEED_RATIO);
                    Texture2D ballTexture = Resources.Load<Texture2D>("Ball");
                    ball.GetComponent<Ball>().Init(ballVelocity, currentPaintBoard, ballTexture);
                }
                mousePrevPos = mouseCurrPos;
            }

        // Stop the ball on mouse up
        } else if (Input.GetKeyUp(KeyCode.Mouse0)) {
            mouseStartPos = Vector2.zero;
            Destroy(ball);
            ball = null;
        }
    }
}
