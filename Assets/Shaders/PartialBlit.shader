Shader "Custom/PartialBlit"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" {}
        // These properties will be set from the script
        _DestUVScale ("Destination UV Scale", Vector) = (1, 1, 0, 0)
        _DestUVOffset ("Destination UV Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        // Disable depth and culling for a simple fullscreen blit quad
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            // The destination scale and offset are passed as UV transformations
            float4 _DestUVScale;
            float4 _DestUVOffset;
            // Standard Unity texture scale/offset properties for _MainTex (used by Graphics.Blit internally)
            float4 _MainTex_ST;


            v2f Vert (appdata v)
            {
                v2f o;
                // Graphics.Blit automatically sets up the vertex positions for a fullscreen quad (0,0 to 1,1) in screen space
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // We calculate the UVs for the source texture.
                // UnityCG.cginc provides a MACRO to handle platform differences for the blit quad UVs.
                // We use the input vertex UVs (which go from 0 to 1 across the screen quad)
                // and transform them to match the destination region.
                o.uv = v.uv * _DestUVScale.xy + _DestUVOffset.xy;
                
                return o;
            }

            float4 Frag (v2f i) : SV_Target
            {
                // Sample the source texture using the calculated UVs
                float4 color = tex2D(_MainTex, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
}
