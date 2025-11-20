Shader "Custom/StampBlitShader"
{
    Properties
    {
        _MainTex ("Previous Drawing", 2D) = "white" {}
        _StampTex ("Stamp Texture", 2D) = "white" {}
        _StampPositionUV ("Stamp Center (UV)", Vector) = (0,0,0,0)
        _StampWidthPixels ("Stamp Width (Pixels)", Float) = 50.0
        _StampHeightPixels ("Stamp Height (Pixels)", Float) = 50.0
        _StampRotationSinCos ("Stamp Rotation (cos, sin)", Vector) = (1,0,0,0)
        _StampColor ("Optional Stamp Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Pass
        {
            // IMPORTANT: Alpha Blending for layering (SrcAlpha Over OneMinusSrcAlpha)
            Blend SrcAlpha OneMinusSrcAlpha 
            Cull Off ZWrite Off ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _StampTex;
            float4 _StampPositionUV;
            float _StampWidthPixels;
            float _StampHeightPixels;
            float4 _StampRotationSinCos;
            float4 _StampColor;

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

                // --- 2. Calculate coordinates in screen/texture space (Pixels) ---
                float2 pixelPos = i.uv * _ScreenParams.xy; 
                float2 stampCenter = _StampPositionUV.xy * _ScreenParams.xy;
                float2 offsetFromCenter = pixelPos - stampCenter;

                // Half dimensions (Needed for normalization)
                float halfWidth = _StampWidthPixels / 2.0;
                float halfHeight = _StampHeightPixels / 2.0;

                // --- 3. Calculate UV for the Stamp Texture (with ROTATION and proper clipping) ---

                // Normalize the offset (results in -1.0 to 1.0)
                float2 normalizedOffset = offsetFromCenter / float2(halfWidth, halfHeight);

                // Retrieve pre-calculated cos and sin from the uniform
                float c = _StampRotationSinCos.x; // cos(angle)
                float s = _StampRotationSinCos.y; // sin(angle)
                
                // Apply 2D Rotation Matrix to the normalized offset.
                float2 rotatedOffset;
                rotatedOffset.x = normalizedOffset.x * c + normalizedOffset.y * s;
                rotatedOffset.y = -normalizedOffset.x * s + normalizedOffset.y * c;
                
                // Bounds Check on the *rotated* coordinates (-1.0 to 1.0). 
                // This is the only check needed to clip the stamp correctly.
                if (abs(rotatedOffset.x) > 1.0 || abs(rotatedOffset.y) > 1.0)
                {
                    return existingColor;
                }

                // Convert rotated offset (-1.0 to 1.0) to actual UV coordinates (0.0 to 1.0)
                float2 stampUV = rotatedOffset * 0.5 + 0.5;

                // 4. Sample the stamp and blend
                fixed4 stampColor = tex2D(_StampTex, stampUV);

                if (stampColor.a == 0) {
                    return existingColor;
                }

                return lerp(existingColor, _StampColor, stampColor.a);
            }
            ENDCG
        }
    }
}