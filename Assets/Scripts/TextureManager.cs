using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    private const int CHUNK_SIZE = 200;

    private SpriteRenderer paintRenderer;

    private SpriteRenderer[,] chunks;
    private float bgWidth;
    private float bgHeight;
    private int rows;
    private int cols;

    void Start()
    {
        paintRenderer = GameObject.FindGameObjectWithTag("Paint")
            .GetComponent<SpriteRenderer>();
    }

    public void init(float bgWidth, float bgHeight, float horizontal, float vertical)
    {
        Camera mainCam = Camera.main;

        rows = (int)Mathf.Ceil(bgWidth/CHUNK_SIZE);
        cols = (int)Mathf.Ceil(bgHeight/CHUNK_SIZE);
        float chunkOffset = ((float)CHUNK_SIZE/2f/Utility.PIXELS_PER_UNIT);

        // Generate the background sprite chunks
        chunks = new SpriteRenderer[rows,cols];
        for(int rowIdx=0; rowIdx < rows; rowIdx++) {
            for(int colIdx=0; colIdx < cols; colIdx++) {
                GameObject chunk = new GameObject(rowIdx + ", " + colIdx);
                SpriteRenderer renderer = chunk.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = 10;

                Vector2 pos = new Vector2(
                    Mathf.Lerp(-horizontal, horizontal, (float)rowIdx/rows) + chunkOffset,
                    Mathf.Lerp(-vertical, vertical, (float)colIdx/cols) + chunkOffset
                );
                chunk.transform.position = pos;
                chunk.transform.SetParent(transform);
                renderer.sprite = Utility.createSprite(
                    CHUNK_SIZE, CHUNK_SIZE, Color.white
                );
                chunks[rowIdx,colIdx] = renderer;
            }
        }
    }
}
