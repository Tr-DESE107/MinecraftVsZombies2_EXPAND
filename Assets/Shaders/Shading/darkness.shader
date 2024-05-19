Shader "MinecraftVSZombies2/DarknessShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_LightTex("Light Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_LightBorderThresold("Light Border Thresold", Range(0, 1)) = 0.5
		_LightBorderStrength("Light Border Strength", Range(0, 1)) = 0.5
		_LightBorderWidth("Light Border Width", Float) = 0.5
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
				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;
				fixed _LightBorderThresold;
				fixed _LightBorderStrength;
				float _LightBorderWidth;

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


				fixed4 frag(v2f_darkness IN) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
					fixed4 light = tex2D(_LightTex, IN.texcoord);

					c.rgb = 1 - (1 - c.rgb) * (1 - light.rgb);
					c.rgb = lerp(1, c.rgb, c.a);
					return c;
				}
				ENDCG
			}
		}
}
