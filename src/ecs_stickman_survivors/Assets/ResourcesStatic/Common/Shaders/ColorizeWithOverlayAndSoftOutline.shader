Shader "Custom/ColorizeWithOverlayAndSoftOutline"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Intensity ("Color Intensity", Range(0,1)) = 0.5
        _OverlayColor ("Overlay Color", Color) = (1,1,1,1)
        _OverlayIntensity ("Overlay Intensity", Range(0,1)) = 0.0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0,25)) = 1.0 // Adjusted the range to 1-10
        _OutlineSmoothness ("Outline Smootheness", Range(1, 10)) = 2.0 // Exposed parameter for outline smootheness
        _FlipX ("Flip X", Float) = 0 // 0 for no flip, 1 for flip on X-axis
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
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
            half4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineSize;
            float _OutlineSmoothness; // Exposed parameter for outline smoothness
            float _FlipX;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                if (_FlipX > 0.5) {
                    o.uv.x = 1.0 - o.uv.x;
                }

                if (_MainTex_ST.z < 0) {
                    o.uv.x = 1.0 - o.uv.x;
                }

                o.uv = TRANSFORM_TEX(o.uv, _MainTex);
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                fixed4 outlineColor = _OutlineColor;
                float2 texSize = float2(1.0 / _MainTex_TexelSize.x, 1.0 / _MainTex_TexelSize.y) * _OutlineSize * 0.00001; // Scale texSize by _OutlineSize

                // Sample the texture multiple times around the current pixel
                float alpha = 0.0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0) continue; // Skip the center pixel
                        alpha += tex2D(_MainTex, i.uv + float2(x, y) * texSize).a;
                    }
                }

                // Calculate the average alpha value of the surrounding pixels
                alpha /= 8.0; // 8 samples were taken

                // Calculate the center alpha value
                float centerAlpha = tex2D(_MainTex, i.uv).a;

                // Apply a smooth falloff effect to the outline using _OutlineSmoothness
                float outlineAlpha = lerp(centerAlpha, alpha, _OutlineSmoothness);

                // Only draw outline if center alpha is less than the outline alpha
                if (centerAlpha < outlineAlpha)
                {
                    return outlineColor * outlineAlpha;
                }
                else
                {
                    return tex2D(_MainTex, i.uv);
                }
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
            float4 _MainTex_ST;
            float4 _Color;
            float _Intensity;
            float4 _OverlayColor;
            float _OverlayIntensity;
            float _FlipX;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                if (_FlipX > 0.5) {
                    o.uv.x = 1.0 - o.uv.x;
                }

                if (_MainTex_ST.z < 0) {
                    o.uv.x = 1.0 - o.uv.x;
                }

                o.uv = TRANSFORM_TEX(o.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Apply the base colorization
                fixed4 colorized = lerp(tex, _Color, _Intensity * tex.a);

                // Apply the overlay color considering sprite's alpha
                fixed4 overlay = lerp(colorized, _OverlayColor, _OverlayIntensity * tex.a);

                return overlay;
            }
            ENDCG
        }
    } 
    Fallback "Sprites/Default"
}
