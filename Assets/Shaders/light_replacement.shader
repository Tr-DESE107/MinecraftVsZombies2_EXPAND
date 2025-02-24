Shader "MinecraftVSZombies2/LightReplacementShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		[Header(Burn)]
		[Toggle(BURN_ON)]
		_Burn("Can Burn", Int) = 0
		_BurnNoise("Noise Tex", 2D) = "white"{}
		_BurnValue("Value", Range(0, 1)) = 0
		_BurnEdgeThreshold("Edge Threshold", Float) = 0.2
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Light"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		UsePass "MinecraftVSZombies2/LightShader/Main"
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

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#include "cg/entity.cginc"
			// 使用multi_compile_local而非shader_feature_local，以使这个标记被打进包
			#pragma multi_compile_local _ BURN_ON
			#pragma vertex EntityVert
			#pragma fragment fragLighting
			ENDCG
		}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Entity3D"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        Pass {
		    Name "ZWrite"

            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #include "cg/entity_model.cginc"
            #pragma vertex vert
            #pragma fragment fragMask
            ENDCG
        }
        Pass {
		    Name "Lighting"

            ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            CGPROGRAM
            #include "cg/entity_model.cginc"
            #pragma vertex vert
            #pragma fragment fragLighting
            ENDCG
        }
	}
	SubShader
	{
		Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="EntityMask"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend Off
        ColorMask 0
		UsePass "MinecraftVSZombies2/Entity/Mask/Main"
	}
}
