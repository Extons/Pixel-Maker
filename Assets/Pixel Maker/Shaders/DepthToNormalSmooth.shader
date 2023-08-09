Shader "Custom/DepthToNormalSmooth"{
    SubShader{
        Tags { "RenderType" = "Opaque" }
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraDepthNormalsTexture;
            sampler2D _CameraDepthTexture;


            struct vertexOutput {
                float4 position_in_clip_space : SV_POSITION;
                float2 depth : TEXCOORD0;
                float4 scrPos : TEXCOORD1;
                float4 position_in_world_space : TEXCOORD2;
            };

            vertexOutput vert(appdata_full v) {
                vertexOutput output;
                output.position_in_clip_space = UnityObjectToClipPos(v.vertex);
                //use the helper function instead of MATRIX_MVP as per documentation
                //mul(UNITY_MATRIX_MVP,float4(v.vertex,1.0));

                output.position_in_world_space = mul(unity_ObjectToWorld,v.vertex);
                //recall computeScreenPos does not divide by w, but because this is not an image effect,
                //diving by w here will WARP things unless we are completely parallel to viewing camera

                output.scrPos = ComputeScreenPos(output.position_in_clip_space);

                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                float3 normalValues;
                float depthValue;
                //extract depth value and normal values

                //recall computeScreenPos does not divide by w, so if you want proper screenUV for use in tex2D divide
                //xy by w
                //https://forum.unity3d.com/threads/what-does-the-function-computescreenpos-in-unitycg-cginc-do.294470/

                float2 screenUV = input.scrPos.xy / input.scrPos.w;
                //DecodeDepthNormal(tex2Dproj(_CameraDepthNormalsTexture, input.scrPos), depthValue, normalValues);
                //Alternatively:


                    DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture,screenUV), depthValue, normalValues);

                    //TODO: CONVERT TO WORLD NORMAL?               
                    float4 worldNormalValues = mul(unity_ObjectToWorld,mul(UNITY_MATRIX_IT_MV, float4(normalValues,1.))); //THIS ISNT RIGHT :(

                   float d = pow(depthValue,2);
                   //return float4(normalValues.x,normalValues.y,normalValues.z,d);   

                   return half4(GammaToLinearSpace(normalValues.xyz * 0.5 + 0.5), d);
               }

            ENDCG
           }

    }
        fallback "Diffuse"
}