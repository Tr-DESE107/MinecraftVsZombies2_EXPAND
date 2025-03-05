Shader "MinecraftVSZombies2/Entity/Model"
{
    Properties
    {
        // 基础属性
        _MainTex ("Diffuse", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalScale ("Normal Scale", Range(-2,2)) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        #if BURN_ON
        #include "hlsl/burn.hlsl"
        #endif

        #if CIRCLE_TILED
        #include "hlsl/circle_tiled.hlsl"
        #endif

        #include "hlsl/tint.hlsl"
        ENDHLSL
        
        Pass {
		    Name "ZWrite"

            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
            };
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _Color;
            float4 _MainTex_ST;


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 采样基础贴图
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                
                clip(baseColor.a - 0.001);
                return baseColor;
            }
            ENDHLSL
        }
        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ HSV_TINT
            #pragma multi_compile _ CIRCLE_TILED

            #include "hlsl/entity_universal2D.hlsl" 
            ENDHLSL
        }
        Pass
        {
            Name "NormalsRendering"
            Tags { "LightMode" = "NormalsRendering"}

            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            #include "hlsl/entity_normals_rendering.hlsl" 
            ENDHLSL
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ CIRCLE_TILED

            #include "hlsl/entity_universal_forward.hlsl"
            ENDHLSL
        }
    }
}