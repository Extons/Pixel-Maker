Shader "Custom/BitColor" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _PaletteTex("Palette", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 1
        _Saturation("Saturation", Range(0, 2)) = 1
        _Contrast("Constrast", float) = 1
        _BitValue("Bit", Range(1, 255)) = 1
        [Toggle]_Bit("Convert To Bit Texture", float) = 1
        [Toggle]_Palette("Use Palette", float) = 1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _PaletteTex;
                float _Palette;
                float4 _OutlineColor;
                float _OutlineWidth;
                float _Saturation;
                float _Contrast;
                float _BitValue;
                float _Bit;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 worldPos : TEXCOORD1;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 color = tex2D(_MainTex, i.uv);

                    // Apply saturation
                    color.rgb = pow(lerp(color.rgb, saturate(color.rgb * _Saturation), _Saturation), _Contrast);

                    float4 finalColor = color;

                    if (_Bit) {
                        // Convert to 8-bit color palette
                        float3 palette = floor(color.rgb * 255.0 / _BitValue) * _BitValue / 255.0;
                        color.rgb = palette;

                        // Dithering
                        float3 error = color.rgb - palette;
                        if (i.uv.x < 0.5)
                        {
                            if (i.uv.y < 0.5) {
                                color.rgb += error * 0.0625;
                            }
                            else
                            {
                                color.rgb += error * 0.1875;
                            }
                        }
                        else
                        {
                            if (i.uv.y < 0.5)
                            {
                                color.rgb += error * 0.3125;
                            }
                            else
                            {
                                color.rgb += error * 0.4375;
                            }
                        }
                    }

                    if (_Palette) {

                        finalColor = tex2D(_PaletteTex, float2(color.r, 0.5)) * color.a;
                    }
                    else {
                        finalColor = color;
                    }

                    // Outline
                    float4 outline = float4(0, 0, 0, 0);
                    float2 pixelSize = 1.0 / _ScreenParams.xy;
                    float2 dx = float2(_OutlineWidth, 0) * pixelSize;
                    float2 dy = float2(0, _OutlineWidth) * pixelSize;
                    outline += tex2D(_MainTex, i.uv + dx);
                    outline += tex2D(_MainTex, i.uv - dx);
                    outline += tex2D(_MainTex, i.uv + dy);
                    outline += tex2D(_MainTex, i.uv - dy);
                    outline = outline / 4.0;

                    float4 coloredOutline = (lerp(color, 1, step(0.01, outline.a)) - color.a) * _OutlineColor;


                    return  finalColor + coloredOutline;
                }
            ENDCG
            }
        }
}
