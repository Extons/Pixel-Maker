Shader "Custom/Pixelated16BitWithOutline" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _PaletteTex("Palette", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness("Outline Thickness", Range(0.001, 0.1)) = 0.005
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _PaletteTex;
                float4 _MainTex_ST;
                float4 _OutlineColor;
                float _OutlineThickness;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 texel = tex2D(_MainTex, i.uv);
                    texel = floor(texel.rgba * 15.99) / 15.0; // Round to nearest 16-bit color
                    fixed4 palette = tex2D(_PaletteTex, texel.xy);
                    fixed4 outlineColor = _OutlineColor;
                    float2 pixelSize = 1.0 / _ScreenParams.xy;

                    // Calculate outline
                    float outline = 1.0 - tex2D(_MainTex, i.uv).a;
                    float4 outlineTexel = outlineColor * outline;

                    // Apply outline thickness
                    float2 offset = _OutlineThickness * pixelSize;
                    float4 sum = tex2D(_MainTex, i.uv + float2(-offset.x, -offset.y)) * outlineTexel;
                    sum += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)) * outlineTexel;
                    sum += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y)) * outlineTexel;
                    sum += tex2D(_MainTex, i.uv + float2(offset.x, offset.y)) * outlineTexel;
                    return palette + sum;
                }
                ENDCG
            }
        }
}