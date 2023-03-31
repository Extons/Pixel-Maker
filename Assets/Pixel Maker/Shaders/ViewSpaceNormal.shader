Shader "Custom/View Space Normal" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                };

                sampler2D _MainTex;

                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return half4(i.normal * 0.5 + 0.5, 1.0f);
                }
                ENDCG
            }
    }

        FallBack "Diffuse"
}