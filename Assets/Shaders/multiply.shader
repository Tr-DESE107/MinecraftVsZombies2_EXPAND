Shader "MinecraftVSZombies2/MultiplyShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"Lighting2D" = "Solid"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend DstColor Zero
			ZWrite Off
			Cull Off

			CGPROGRAM
			#include "UnitySprites.cginc"
			#pragma vertex SpriteVert
			#pragma fragment SpriteFrag 
			ENDCG
		}
	}
	FallBack "Sprites/Default"
}
