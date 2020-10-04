using UnityEngine;

public class Utility 
{

    public const int PIXELS_PER_UNIT = 100;

    /**
     * Create a sprite object with a starting color.
     * @param width
     * @param height
     * @param color
     */
    static public Sprite createSprite(int width, int height, Color color) {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        setTextureColor(texture, color);

        return Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, width, height), 
            new Vector2(0.5f, 0.5f),
            PIXELS_PER_UNIT
        );
    }

    /**
     * Set every pixel of a texture to a solid color.
     * @param texture
     * @param color
     */
    static public void setTextureColor(Texture2D texture, Color color) {
        Color[] pixels = new Color[texture.width * texture.height];
        for(int startColorIdx=0; startColorIdx < pixels.Length; startColorIdx++) {
            pixels[startColorIdx] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }
}
