Shader "Custom/SpriteShaderWithNormalMap"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Albedo ("Albedo Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _UV ("Custom UV", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _Albedo;
        sampler2D _NormalMap;
        sampler2D _UV;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_UV;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed2 uv = tex2D(_UV, IN.uv_UV).rg;

            // Sample the main texture
            fixed4 c = tex2D(_Albedo, uv);

            // Sample the normal map and convert it from [0, 1] to [-1, 1] range
            fixed3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex) * 2.0 - 1.0);

            o.Albedo = c.rgb;
            o.Normal = normal;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
