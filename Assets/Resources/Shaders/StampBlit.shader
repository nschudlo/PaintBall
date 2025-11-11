Shader "Custom/StampBlitShader"
{
    Properties
    {
        _MainTex ("Previous Drawing", 2D) = "white" {}
        _StampTex ("Stamp Texture", 2D) = "white" {}
        _StampPositionUV ("Stamp Center (UV)", Vector) = (0,0,0,0)
        _StampWidthPixels ("Stamp Width (Pixels)", Float) = 50.0
        _StampHeightPixels ("Stamp Height (Pixels)", Float) = 50.0
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

                // Current pixel position on the destination RT
                float2 pixelPos = i.uv * _ScreenParams.xy; 
                
                // Stamp center position on the destination RT
                float2 stampCenter = _StampPositionUV.xy * _ScreenParams.xy;
                
                // Calculate pixel offset from the center of the stamp
                float2 offsetFromCenter = pixelPos - stampCenter;

                // --- 3. Rectangular Bounds Check ---

                // Half dimensions
                float halfWidth = _StampWidthPixels / 2.0;
                float halfHeight = _StampHeightPixels / 2.0;

                // Check if pixel is outside the stamp's rectangular bounds
                if (abs(offsetFromCenter.x) > halfWidth || abs(offsetFromCenter.y) > halfHeight)
                {
                    return existingColor;
                }

                // --- 4. Calculate UV for the Stamp Texture ---

                // Normalize the offset to the stamp's half-dimensions (results in -1.0 to 1.0)
                float2 normalizedOffset = offsetFromCenter / float2(halfWidth, halfHeight);
                
                // Convert normalized offset (-1.0 to 1.0) to actual UV coordinates (0.0 to 1.0)
                float2 stampUV = normalizedOffset * 0.5 + 0.5;

                // 5. Sample the stamp and blend
                fixed4 stampColor = tex2D(_StampTex, stampUV);

                if (stampColor.a == 0) {
                    return existingColor;
                }

                // Blend the stamp color with the existing color based on the stamp's alpha
                return lerp(existingColor, _StampColor, stampColor.a);
            }
            ENDCG
        }
    }
}