using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Color[] colors = {
        Color.blue, Color.red, Color.green, Color.yellow, Color.magenta
    };
    private int colorIdx = 0;

    private SpriteRenderer ballRenderer;
    private Rigidbody2D ballRigidBody;
    private SpriteRenderer paintRenderer;

    void Start()
    {
        paintRenderer = GameObject.FindGameObjectWithTag("Paint").GetComponent<SpriteRenderer>();
        ballRenderer = GetComponent<SpriteRenderer>();
        ballRigidBody = GetComponent<Rigidbody2D>();

        StartCoroutine("MoveAndDraw");
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Restart();
        }
    }

    /**
     * Stop the ball and start it up again.
     */
    public void Restart() {
        StopCoroutine("MoveAndDraw");
        ballRigidBody.velocity = Vector3.zero;
        ballRigidBody.angularVelocity = 0f;
        StartCoroutine("MoveAndDraw");
    }

    /**
     * Add a force to the ball and drag dots periodically.
     */
    IEnumerator MoveAndDraw() {
        float verticalForce = Random.Range(-10, 10);
        float horizontalForce = Random.Range(-10, 10);
        ballRigidBody.AddForce(new Vector2(verticalForce * 50, horizontalForce * 50));

        while(true) {
            int index = colorIdx++ % colors.Length;
            // ballRenderer.color = colors[index];
            
            // Figure out the ball position on the background
            int ballX = (int)((transform.position.x * Utility.PIXELS_PER_UNIT) + (Screen.width / 2));
            int ballY = (int)((transform.position.y * Utility.PIXELS_PER_UNIT) + (Screen.height / 2));

            Texture2D bgTexture = paintRenderer.sprite.texture;

            // Draw a dot at that position
            int width = Random.Range(5,10);
            for(int x = -width; x < width; x++) {
                for(int y = -width; y < width; y++) {
                    bgTexture.SetPixel(
                        ballX + x, 
                        ballY + y,
                        colors[index]
                    );
                }
            }
            bgTexture.Apply();            

            yield return new WaitForSeconds(0.05f);
        }
    }
}
