Shader "MinecraftVSZombies2/Entity/Reverse"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

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
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "EntityPassLight"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend OneMinusDstColor OneMinusSrcAlpha
			ZWrite Off
			Cull Off

			CGPROGRAM
			#include "cg/entity.cginc"
			#pragma shader_feature_local _ BURN_ON
			#pragma vertex EntityVert
			#pragma fragment frag

			fixed4 frag(v2fEntity i) :SV_Target
			{
				fixed4 col = EntityFrag(i);
				col.rgb *= col.a;
				return col;
			}
			ENDCG
		}
	}
	FallBack "MinecraftVSZombies2/Entity/Entity"
}
