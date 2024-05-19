
#include "UnityCG.cginc"
#pragma target 3.0

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _EdgeColor;
sampler2D _NoiseTex;
float4 _NoiseTex_ST;
fixed _Thickness;

struct a2v_gas
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2f_gas
{
	float2 uv : TEXCOORD0;
	float2 noise_uv : TEXCOORD1;
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	fixed thickness : TEXCOORD2;
};

v2f_gas vert_gas(a2v_gas v)
{
	v2f_gas o;
	fixed agePercent = v.texcoord.z;
	o.vertex = UnityObjectToClipPos(v.vertex);
	_NoiseTex_ST.z += agePercent * 0.1;

	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	o.color = v.color;
	o.noise_uv = TRANSFORM_TEX(v.texcoord, _NoiseTex);

	fixed thick;
	if (agePercent <= 0.1)
	{
		thick = lerp(0, 1,agePercent * 10);
	}
	else
	{
		thick = lerp(1, 0,(agePercent - 0.1) / 0.9);
	}

	o.thickness = thick + _Thickness;
	return o;
}


fixed4 SampleGas(sampler2D tex, float2 uv, sampler2D noise_tex, float2 noise_uv, float thickness)
{
	fixed4 col = tex2D(tex, uv);

	fixed noise = tex2D(noise_tex, noise_uv).r;
	float clipValue = (noise - (1 - thickness)) * 2;
	clip(clipValue);
	col.rgb = lerp(_EdgeColor.rgb, col.rgb, clamp(clipValue * 0.5 + 0.5,0, 1));
	col.a *= clamp(clipValue, 0, 1);

	return col;
}

fixed4 frag_gas(v2f_gas i) : SV_TARGET
{
	fixed4 col = SampleGas(_MainTex, i.uv, _NoiseTex, i.noise_uv, i.thickness) * i.color;
	clip(col.a - 0.03);

	return col;
}

