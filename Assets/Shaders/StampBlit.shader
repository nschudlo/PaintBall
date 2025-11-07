Shader "Custom/StampBlitShader"
{
    Properties
    {
        _MainTex ("Previous Drawing", 2D) = "white" {}
        _StampTex ("Circle Texture", 2D) = "white" {}
        _StampPositionUV ("Stamp Center (UV)", Vector) = (0,0,0,0)
        _StampSizePixels ("Stamp Size (Pixels)", Float) = 50.0
    }
    
    SubShader
    {
        // ... (Tags and Pass setup) ...

        Pass
        {
            // Set up Alpha Blending (Important for stamping/drawing!)
            Blend SrcAlpha OneMinusSrcAlpha 
            Cull Off ZWrite Off ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _StampTex;
            float4 _StampPositionUV;
            float _StampSizePixels;
            float4 _MainTex_TexelSize; // Unity built-in for texture size

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Get the existing drawing color (from tempRT)
                fixed4 existingColor = tex2D(_MainTex, i.uv);

                // 2. Determine if the current pixel (i.uv) is inside the stamp area

                // Convert UV position to pixel position
                float2 pixelPos = i.uv * _ScreenParams.xy; 
                
                // Convert Stamp UV center to pixel position
                float2 stampCenter = _StampPositionUV.xy * _ScreenParams.xy;

                // Calculate the distance from the current pixel to the stamp center
                float dist = distance(pixelPos, stampCenter);

                // If the pixel is outside the circle's radius, return the existing color
                if (dist > _StampSizePixels / 2.0)
                {
                    return existingColor;
                }

                // 3. If inside, calculate the UV for the small circle texture
                
                // Get the offset from the center of the stamp
                float2 offset = pixelPos - stampCenter;
                
                // Normalize the offset to the size of the stamp (from -0.5 to 0.5)
                float2 normalizedOffset = offset / (_StampSizePixels / 2.0);
                
                // Convert to the actual UV range of the stamp texture (0 to 1)
                float2 stampUV = normalizedOffset * 0.5 + 0.5;

                // 4. Sample the circle and blend
                fixed4 stampColor = tex2D(_StampTex, stampUV);

                // Blend the stamp color with the existing color
                // The Blend mode in the Pass { ... } block handles the final output.
                // We ensure the final fragment color includes the alpha from the stamp.
                return lerp(existingColor, stampColor, stampColor.a);
            }
            ENDCG
        }
    }
}