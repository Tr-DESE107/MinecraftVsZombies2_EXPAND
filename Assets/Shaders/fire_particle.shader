Shader "MinecraftVSZombies2/Fire"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "" {}
		_EdgeColor("Edge Color", Color) = (1,1,1,1)
		_LocalRect("Local Rect", Vector) = (0, 0, 1, 1)
		_FireTime("Fire Time", Float) = 0
		_LifeTime("Life Time", Range(0, 1)) = 0
		_ClipThresold("Clip Thresold", Range(0, 1)) = 0.5
    }

	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma target 3.0

	sampler2D _MainTex;
	float _FireTime;
	float4 _MainTex_ST;
	fixed4 _EdgeColor;
	sampler2D _NoiseTex;
	float4 _NoiseTex_ST;
	float4 _LocalRect;
	fixed _LifeTime;
	fixed _ClipThresold;

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
		fixed lifetime : TEXCOORD2;
	};

	v2f vert(a2v v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.noise_uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);
		o.color = v.color;
		o.lifetime = v.texcoord.z + _LifeTime;
		return o;
	}


	fixed4 frag(v2f i) :SV_Target
	{
		// 获取原本贴图颜色
		fixed4 col = tex2D(_MainTex, i.uv);
		col.rgba *= i.color.rgba;
		clip(col.a - 0.1);
		
		// 获取对应位置的噪声颜色。
		float2 noise_uv = i.noise_uv - _FireTime * _NoiseTex_ST.xy;
		fixed noise = tex2D(_NoiseTex, noise_uv).r;
		
		// 根据噪声的明度决定是否裁去像素。
		fixed2 localUV = (i.uv - _LocalRect.xy) / (_LocalRect.zw - _LocalRect.xy);
		float clipValue = noise + _ClipThresold - localUV.x - i.lifetime;
		clip(clipValue);

		col = lerp(_EdgeColor, col, clamp(clipValue * 3, 0, 1));
		col.a *= lerp(0, 1, i.lifetime * 4);
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
