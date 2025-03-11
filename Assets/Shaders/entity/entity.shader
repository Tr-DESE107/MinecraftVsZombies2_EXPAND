Shader "MinecraftVSZombies2/Entity/Entity"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

		[Header(Color Offset)]
		[Space(10)]
		_ColorOffset("Color Offset", Color) = (0,0,0,0)

		[Header(Burn)]
		[Toggle(BURN_ON)]
		_Burn("Can Burn", Int) = 0
		_BurnNoise("Noise Tex", 2D) = "white"{}
		_BurnValue("Value", Range(0, 1)) = 0
		_BurnEdgeColor("Edge Color", Color) = (0,0.5,0.5,1)
		_BurnEdgeThreshold("Edge Threshold", Float) = 0.2
		_BurnFireColor("Fire Color", Color) = (0,1,1,1)
        
		[Header(Circle Tiled)]
		[Toggle(CIRCLE_TILED)]
		_CircleTiled("Circle Tiled", Int) = 0
		_CircleFill("Fill", Range(0, 1)) = 1
		[KeywordEnum(Right, Down, Left, Up)]
		_CircleStart("Start", Range(0, 3)) = 0
		[Toggle]
		_CircleClockwise("Clockwise", Int) = 0
		
		[Toggle(HSV_TINT)]
		_HSVTint("HSVTint", Int) = 0

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
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

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
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

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

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

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ HSV_TINT
            #pragma multi_compile _ CIRCLE_TILED

            #include "hlsl/entity_universal_forward.hlsl"
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
