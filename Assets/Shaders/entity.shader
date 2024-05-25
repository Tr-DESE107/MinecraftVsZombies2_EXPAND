Shader "MinecraftVSZombies2/EntityShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		[Header(Color Offset)]
		[Space(10)]
		_ColorOffset("Color Offset", Color) = (0,0,0,0)
		
		/*[KeywordEnum(Disabled, Never, Less, Equal, LessEqual, Greater, NotEqual, GreaterEqual, Always)]
		_ZTest("ZTest", Range(1, 8)) = 8
		
		[KeywordEnum(Disabled, Never, Less, Equal, LessEqual, Greater, NotEqual, GreaterEqual, Always)]
		_StencilComp("Stencil Comparison", Range(1, 8)) = 8
		_Stencil("Stencil ID", Range(0, 255)) = 0
		[KeywordEnum(Keep, Zero, Replace, IncrSat, DecrSat, Invert, IncrWarp, DecrWarp)]_StencilPassOp("Stencil Pass Operation", Range(0, 7)) = 0
		[KeywordEnum(Keep, Zero, Replace, IncrSat, DecrSat, Invert, IncrWarp, DecrWarp)]_StencilFailOp("Stencil Fail Operation", Range(0, 7)) = 0
		_StencilWriteMask("Stencil Write Mask", Range(0, 255)) = 255
		_StencilReadMask("Stencil Read Mask", Range(0, 255)) = 255
		_ColorMask("Color Mask", Range(0, 15)) = 15*/

		[Header(Circle)]
		[Toggle(CIRCLETILED_ON)]
		_CircleTiled("Circle Tiled", Int) = 0
		_CircleFill("Fill", Range(0, 1)) = 1
		[KeywordEnum(Right, Down, Left, Up)]
		_CircleStart("Start", Range(0, 3)) = 0
		[Toggle]
		_CircleClockwise("Clockwise", Int) = 0

		[Header(Burn)]
		[Toggle(BURN_ON)]
		_Burn("Can Burn", Int) = 0
		_BurnNoise("Noise Tex", 2D) = "white"{}
		_BurnValue("Value", Range(0, 1)) = 0
		_BurnEdgeColor("Edge Color", Color) = (0,0.5,0.5,1)
		_BurnEdgeThreshold("Edge Threshold", Float) = 0.2
		_BurnFireColor("Fire Color", Color) = (0,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Entity"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, One One
			ZWrite Off
			Cull Off
			/*Stencil {
				Ref 0
				Comp GEqual
				Pass Keep
				Fail Keep
			}*/

			CGPROGRAM
			#include "cg/entity.cginc"
			#pragma shader_feature_local _ BURN_ON
			#pragma shader_feature_local _ CIRCLETILED_ON
			#pragma shader_feature_local _ COLORED_ON
			#pragma vertex EntityVert
			#pragma fragment EntityFrag
			ENDCG
		}
	}
	FallBack "Sprites/Default"
}
