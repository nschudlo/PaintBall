using UnityEngine;

public class Utils {

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
        for (int startColorIdx = 0; startColorIdx < pixels.Length; startColorIdx++) {
            pixels[startColorIdx] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    /**
     * Clear the contents of a RenderTexture
     * @param renderTexture
     */
    static public void ClearOutRenderTexture(RenderTexture renderTexture) {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }

    /**
     * Checks if a point lies between A and B, assuming:
     * - a is a starting point, and point is going towards b
     * - the point is going directly from a to b
     * @param a - the starting point of the segment
     * @param b - the ending point of the segment
     * @param point - the point somewhere on the ray from a to b
     * @returns true if point is between a and b, false otherwise
     */
    static public bool IsPointBetweenAB(Vector2 a, Vector2 b, Vector2 point) {
        Vector3 AB = b - a;
        Vector3 AP = point - a;

        // Calculate the squared distance from A to P (||AP||^2)
        // as a proxy for the distance traveled from A
        float sqrMagAP = AP.sqrMagnitude;

        // Calculate the squared distance from A to B (||AB||^2)
        // s is the total length of the segment being checked
        float sqrMagAB = AB.sqrMagnitude;

        // If ||AP||^2 is less than or equal to ||AB||^2, 
        // P is before or at B. Since we assume P is on the line 
        // and not behind A, this is sufficient.
        return sqrMagAP <= sqrMagAB;
    }
}
