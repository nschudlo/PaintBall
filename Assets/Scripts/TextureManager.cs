using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    private const int CHUNK_SIZE = 500;

    private SpriteRenderer[,] chunks;
    private float bgWidth;
    private float bgHeight;
    private int rows;
    private int cols;
    private HashSet<Texture2D> dirty = new HashSet<Texture2D>();

    public void init(float bgWidth, float bgHeight, float horizontal, float vertical)
    {
        rows = (int)Mathf.Ceil(bgWidth/CHUNK_SIZE);
        cols = (int)Mathf.Ceil(bgHeight/CHUNK_SIZE);

        float chunkSize = (float)CHUNK_SIZE / Utility.PIXELS_PER_UNIT;
        float chunkOffset = chunkSize / 2f;

        // Generate the background sprite chunks
        chunks = new SpriteRenderer[rows,cols];
        for(int rowIdx=0; rowIdx < rows; rowIdx++) {
            for(int colIdx=0; colIdx < cols; colIdx++) {
                // Create the sprite
                GameObject chunk = new GameObject(rowIdx + ", " + colIdx);
                chunk.transform.SetParent(transform);
                
                // Set the position 
                Vector2 pos = new Vector2(
                    (rowIdx * chunkSize) - horizontal + chunkOffset,
                    (colIdx * chunkSize) - vertical + chunkOffset
                );
                chunk.transform.position = pos;

                // Setup the renderer
                SpriteRenderer renderer = chunk.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = 10;
                renderer.sprite = Utility.createSprite(
                    CHUNK_SIZE, CHUNK_SIZE, Color.clear
                );
                renderer.sprite.texture.filterMode = FilterMode.Point;
                chunks[rowIdx,colIdx] = renderer;
            }
        }
    }

    /**
     * Sets a pixel to a color treating the background as
     * a whole. Finds the correct chunk and updates the 
     * correct pixel in that chunk.
     * @param x
     * @param y
     * @param color
     */
    public void SetPixel(int x, int y, Color color) {
        Texture2D tex = chunks[x/CHUNK_SIZE, y/CHUNK_SIZE].sprite.texture;
        tex.SetPixel(x%CHUNK_SIZE, y%CHUNK_SIZE, color);
        dirty.Add(tex);
    }

    /**
     * Apply the textures for all dirty chunks.
     */
    public void Apply() {
        foreach(Texture2D tex in dirty) {
            tex.Apply();
        }
        dirty.Clear();
    }

    /**
     * Reset all the chunks.
     */
    public void ResetPaintLayer() {
        foreach(SpriteRenderer chunk in chunks) {
            Utility.setTextureColor(chunk.sprite.texture, Color.clear);
        }
        dirty.Clear();
    }
}
