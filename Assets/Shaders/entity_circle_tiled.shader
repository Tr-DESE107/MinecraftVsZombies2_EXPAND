Shader "MinecraftVSZombies2/Entity/CircleTiled"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		[Header(Color Offset)]
		[Space(10)]
		_ColorOffset("Color Offset", Color) = (0,0,0,0)
		
		[Header(Circle)]
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
			"CanUseSpriteAtlas" = "False"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, One One
			ZWrite Off
			Cull Off

			CGPROGRAM
			#include "cg/entity.cginc"
			#include "cg/circle_tile.cginc"
			#pragma shader_feature_local _ BURN_ON
			#pragma shader_feature_local _ COLORED_ON
			#pragma vertex EntityVert
			#pragma fragment frag
			fixed4 frag(v2f i) :SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
				col = col + _ColorOffset;
				col = CircleTile(col, i.uv);
				#if BURN_ON
				col = Burn(col, i);
				#endif
				clip(col.a - 0.15);

				return col;
			}

			ENDCG
		}
	}
	FallBack "MinecraftVSZombies2/Entity/Entity"
}
