Shader "MinecraftVSZombies2/Legacy/Entity"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}

		[Header(Color)]
		[Space(10)]
        _Color("Tint", Color) = (1,1,1,1)
		_ColorOffset("Color Offset", Color) = (0,0,0,0)
		[Toggle]
		_Grayscale("Grayscale", Int) = 0
		_GrayscaleFactor("Grayscale Factor", Vector) = (0.598,1.174,0.224,0)

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
		
		[Header(HSV)]
		[Toggle(HSV_TINT)]
		_HSVTint("HSVTint", Int) = 0
		_HSVOffset("HSV Offset", Vector) = (0,0,0,0)

		[Header(Lighting)]
		[Toggle(LIT)]
		_LIT("Lit", Int) = 1
		[HideInInspector] _LightDisabled("Light Disabled", Int) = 0
		[Toggle] _BackgroundLit("Lit by Background", Int) = 0
		[Toggle] _SpotLit("Lit by Spot", Int) = 1
        _LightMapSpot("Light Map Spot", 2D) = "black" {}
        _LightMapST ("Light Map ST", Vector) = (14, 10.2, 0, 0)

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        
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
			"PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Main"

            HLSLPROGRAM
            #include "../hlsl/entity.hlsl"
            #pragma vertex EntityVert
            #pragma fragment EntityFrag
            #pragma multi_compile_instancing
            #pragma multi_compile _ BURN_ON
            #pragma multi_compile _ HSV_TINT
            #pragma multi_compile _ CIRCLE_TILED
            #pragma multi_compile _ LIT
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
