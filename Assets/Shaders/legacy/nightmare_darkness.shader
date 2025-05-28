Shader "MinecraftVSZombies2/Legacy/Nightmare Darkness"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_Radius("Radius", Range(0, 1)) = 0
		_Edge("Edge", Float) = 3
    }

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float4 _MainTex_ST;
			float4 _NoiseTex_ST;
			float _Edge;
			float _Radius;

			struct Attributes
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half4 color : COLOR;
			};

			struct Varyings
			{
				float2 uv : TEXCOORD0;
				float2 noise_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			Varyings vert(Attributes v)
			{
				Varyings o = (Varyings)0;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.noise_uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
				o.color = v.color;
		
				return o;
			}


			half4 frag(Varyings i) :SV_Target
			{
    			half4 col = tex2D(_MainTex, i.uv);
				col.rgba *= i.color.rgba;
		
				float dis = distance(float2(0.5,0.5), i.uv) * 2;
				float diff = (_Radius + _Edge - dis) /_Edge;

				float2 noise1_uv = i.noise_uv;
				float2 noise2_uv = i.noise_uv * 0.4 + 0.1;
				half noise1 = tex2D(_NoiseTex, noise1_uv).r;
				half noise2 = tex2D(_NoiseTex, noise2_uv).r;
				half noise = noise1 + noise2;

				col.a = saturate(col.a - max(1 - _Radius, noise) * (1 - diff));
		
				return col;
			}
			ENDHLSL

		}

	}
	FallBack "Sprites/Default"
}
