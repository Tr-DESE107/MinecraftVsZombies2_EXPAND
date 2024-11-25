#include "UnityCG.cginc"
#pragma target 3.0


struct a2v
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    fixed4 color : COLOR;
};
struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
#if BURN_ON
    float2 noise_uv : TEXCOORD1;
#endif
    fixed4 color : COLOR;
};

sampler2D _MainTex;
float4 _MainTex_ST;
half4 _MainTex_TexelSize;
fixed4 _Color;
fixed4 _ColorOffset;


#if BURN_ON
sampler2D _BurnNoise;
float4 _BurnNoise_ST;

fixed _BurnValue;
fixed4 _BurnEdgeColor;
fixed4 _BurnFireColor;
float _BurnEdgeThreshold;

fixed4 Burn(fixed4 col, v2f i)
{
    if (_BurnValue)
    {
        float2 noise_uv = i.noise_uv;
        fixed noise = tex2D(_BurnNoise, noise_uv).r - _BurnValue;

        clip(noise);
        if (noise <= _BurnEdgeThreshold + i.uv.y)
        {
            col.rgb = lerp(_BurnEdgeColor.rgb, col.rgb, saturate(noise / _BurnEdgeThreshold));
        }

        if (noise <= _BurnEdgeThreshold * 0.023)
        {
            col.rgb = _BurnFireColor.rgb;
        }
    }
    return col;
}
#endif

v2f EntityVert(a2v v)
{
    v2f o;

    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
#if BURN_ON
    o.noise_uv = TRANSFORM_TEX(v.texcoord, _BurnNoise);
#endif
    o.color = v.color;
    return o;
}

fixed4 EntityFrag(v2f i) :SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
    
    col.rgb = _ColorOffset.rgb + col.rgb;
    //col.rgb = _ColorOffset.rgb * _ColorOffset.a + col.rgb * (1 - _ColorOffset.a);

    #if BURN_ON
    col = Burn(col, i);
    #endif
    //clip(col.a - 0.15);

    return col;
}