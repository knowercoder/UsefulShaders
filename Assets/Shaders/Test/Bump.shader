Shader "Custom/Bump" 
{ 
    Properties 
    { 
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
        CBUFFER_END
        ENDHLSL

        Pass {
            Name "Example"
            Tags { "LightMode"="UniversalForward" }
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct a2v{
                float4 positionOS   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
            };

            struct v2f{
                float4 positionCS   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 t2w0     : TEXCOORD1;
                float4 t2w1     : TEXCOORD2;
                float4 t2w2     : TEXCOORD3;
            };

            sampler2D _MainTex;


            v2f vert(a2v IN) {
                v2f OUT;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normal_inputs = GetVertexNormalInputs(IN.normal, IN.tangent);
                OUT.positionCS = positionInputs.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.t2w0 = float4(normal_inputs.tangentWS.x, normal_inputs.bitangentWS.x, normal_inputs.normalWS.x, positionInputs.positionWS.x);
                OUT.t2w1 = float4(normal_inputs.tangentWS.y, normal_inputs.bitangentWS.y, normal_inputs.normalWS.y, positionInputs.positionWS.y);
                OUT.t2w2 = float4(normal_inputs.tangentWS.z, normal_inputs.bitangentWS.z, normal_inputs.normalWS.z, positionInputs.positionWS.z);
                return OUT;
            }

            half4 frag(v2f IN) : SV_Target {
                float3 posWS = float3(IN.t2w0.w, IN.t2w1.w, IN.t2w2.w);
                float3x3 t2wMatrix = float3x3(IN.t2w0.xyz, IN.t2w1.xyz, IN.t2w2.xyz);
                Light light = GetMainLight();
                half3 lightDirWS = normalize(light.direction);
                half3 viewDirWs = normalize(_WorldSpaceCameraPos.xyz - posWS);
                half3 halfDirWs = normalize(lightDirWS + viewDirWs);

                half4 baseMap = tex2D(_MainTex, IN.uv);
                half3 finalColor = baseMap.rgb;

                
                return half4(finalColor, 1);
            }
            
            ENDHLSL
        }
    }
}