Shader "MinecraftVSZombies2/DarknessShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_LightTex("Light Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_BlurRadius("Blur Radius", Int) = 0
		_BlurSize("Blur Size", Float) = 0
		_Color("Tint", Color) = (1,1,1,1)
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend DstColor Zero

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnitySprites.cginc"

				sampler2D _LightTex;
				half4 _LightTex_TexelSize;
				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;
				int _BlurRadius;
				fixed _BlurSize;

				struct a2v_darkness
				{
					float4 vertex : POSITION0;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f_darkness
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float2 noise_uv : TEXCOORD1;
					fixed4 color : COLOR;
				};


				v2f_darkness vert(a2v_darkness IN)
				{
					v2f_darkness OUT;

					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.noise_uv = TRANSFORM_TEX(IN.vertex, _NoiseTex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;

					return OUT;
				}

				fixed4 blur(sampler2D tex, half4 texelSize, fixed2 uv)
				{
					int blurSize = _BlurSize;
					int radius = _BlurRadius;
					int grids = (radius * 2 + 1) * (radius * 2 + 1);
					fixed4 col = 0;
					for (int x = -radius; x <= radius; x++)
					{
						for (int y = -radius; y <= radius; y++)
						{
							float weight = 1.0 / grids;
							fixed2 offset = fixed2(x * texelSize.x * blurSize, y * texelSize.y * blurSize);
							col += tex2D(tex, uv + offset) * weight;
						}
					}
					return col;
				}

				fixed4 frag(v2f_darkness IN) : SV_Target
				{
					fixed4 dark = tex2D(_MainTex, IN.texcoord) * IN.color;
					fixed4 light = blur(_LightTex, _LightTex_TexelSize, IN.texcoord);



					dark.rgb = 1 - (1 - dark.rgb) * (1 - light.rgb);
					dark.rgb = lerp(1, dark.rgb, dark.a);
					return dark;
				}
				ENDCG
			}
		}
}
