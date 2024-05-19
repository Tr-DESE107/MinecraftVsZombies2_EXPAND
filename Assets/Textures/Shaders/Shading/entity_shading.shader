Shader "MinecraftVSZombies2/EntityShadowShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"RenderType" = "Light"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend OneMinusDstColor One

		Pass
		{
		
			CGPROGRAM
			#include "../cg/darkness.cginc"
			#pragma vertex vert
			#pragma fragment frag_light
			ENDCG
		}
	}
		
	SubShader
	{
		Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"RenderType" = "LightMask"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{

			CGPROGRAM
			#include "../cg/darkness.cginc"
			#pragma vertex vert
			#pragma fragment frag_light
			ENDCG
		}
	}
}
