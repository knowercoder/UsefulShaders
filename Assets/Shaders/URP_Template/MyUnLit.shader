Shader "Custom/MyUnLit"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _Tint("Tint", Color) = (1, 1, 1, 1)
    }
    
    SubShader
    {        
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {            
            HLSLPROGRAM
            
            #pragma vertex vert            
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"             

            float4 _Tint;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

           
            struct Attributes
            {              
                float4 positionOS   : POSITION;   
                float2 uv : TEXCOORD0;              
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD0;  
            };

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;               
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);     
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);           
                return OUT;
            }
                     
            half4 frag(Attributes IN) : SV_Target
            {   
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Tint;
                return color;
            }
            ENDHLSL
        }
    }
}
