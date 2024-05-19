#include "gas.cginc"

struct appdata
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	half2 texcoord : TEXCOORD0;
};

struct v2f_darkness
{
	float4 vertex   : SV_POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

fixed4 _DarknessColor;
fixed _LevelObjectDiffuse;
fixed4 _Color;

v2f_darkness vert(appdata IN)
{
	v2f_darkness OUT;

	OUT.vertex = UnityObjectToClipPos(IN.vertex);
	OUT.texcoord = IN.texcoord;
	OUT.color = IN.color;

	return OUT;
}


fixed4 frag_darkness(v2f_darkness i) : SV_Target
{
	fixed4 c = tex2D(_MainTex, i.texcoord) * _Color;
	c.a *= i.color.a;
	c.rgb = lerp(1, _DarknessColor.rgb + _LevelObjectDiffuse, c.a);
	c.a = 1;
	return c;
}

fixed4 frag_light(v2f_darkness i) : SV_Target
{
	fixed4 c = tex2D(_MainTex, i.texcoord) * i.color * _Color;
	c.rgb *= c.a;
	return c;
}

fixed4 frag_gas_darkness(v2f_gas i) : SV_TARGET
{
	fixed4 c = SampleGas(_MainTex, i.uv, _NoiseTex, i.noise_uv, i.thickness) * i.color * _Color;
	c.rgb = lerp(1, _DarknessColor.rgb + _LevelObjectDiffuse, c.a);
	c.a = 1;
	return c;
}