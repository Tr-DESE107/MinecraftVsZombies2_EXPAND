Shader "MinecraftVSZombies2/Entity/Shadow"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
			"PreviewType" = "Plane"
        }

        Blend DstColor Zero, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex ShadowVertex
            #pragma fragment ShadowFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ HSV_TINT
            #pragma multi_compile _ CIRCLE_TILED

            #include "hlsl/entity_shadow.hlsl" 
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
