Shader "MinecraftVSZombies2/Nightmare Darkness"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "" {}
		_Radius("Radius", Range(0, 1)) = 0
		_Edge("Edge", Float) = 3
    }

	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma target 3.0

	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _NoiseTex;
	float4 _NoiseTex_ST;
	fixed _Radius;
	float _Edge;

	struct a2v
	{
		float4 vertex : POSITION;
		float3 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float2 noise_uv : TEXCOORD1;
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
	};

	v2f vert(a2v v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.noise_uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
		o.color = v.color;
		
		return o;
	}


	fixed4 frag(v2f i) :SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
		col.rgba *= i.color.rgba;
		
		float dis = distance(float2(0.5,0.5), i.uv) * 2;
		float diff = (_Radius + _Edge - dis) /_Edge;

		float2 noise1_uv = i.noise_uv;
		float2 noise2_uv = i.noise_uv * 0.4 + 0.1;
		fixed noise1 = tex2D(_NoiseTex, noise1_uv).r;
		fixed noise2 = tex2D(_NoiseTex, noise2_uv).r;
		fixed noise = noise1 + noise2;

		col.a -= max(1-_Radius, noise) * (1 - diff);
		
		return col;
	}

	ENDCG

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			ZWrite Off
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG

		}

	}
	FallBack "Sprites/Default"
}
