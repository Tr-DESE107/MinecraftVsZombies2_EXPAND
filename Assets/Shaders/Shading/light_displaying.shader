Shader "MinecraftVSZombies2/LightDisplayingShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"Lighting2D" = "Display"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnitySprites.cginc"


			struct appdata_masking
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f_masking
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f_masking vert(appdata_masking IN)
			{
				v2f_masking OUT;

				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}


			fixed4 frag(v2f_masking IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.uv) * _Color * IN.color;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
