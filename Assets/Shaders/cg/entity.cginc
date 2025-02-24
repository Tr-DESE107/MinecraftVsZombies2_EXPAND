#include "UnityCG.cginc"
#include "entity_common.cginc"
#pragma target 3.0


#if BURN_ON
fixed4 Burn(fixed4 col, v2fEntity i)
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

// ×ÅÉ«¡£
float3 RGB2HSV(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
float3 HSV2RGB(float3 c)
{
    float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6) - 3.0) - 1.0, 0, 1);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * lerp(float3(1, 1, 1), rgb, c.y);
}
fixed4 Tint(fixed4 color, fixed4 tint)
{
    if (_HSVTint > 0)
    {
        float3 hsv = RGB2HSV(color.rgb);
        float3 tintHsv = RGB2HSV(tint.rgb);
        float t = hsv.y * hsv.z;
        float3 tintedHsv = float3(hsv.x + tintHsv.x, hsv.y * tintHsv.y, hsv.z * tintHsv.z);
        return fixed4(HSV2RGB(tintedHsv), color.a);

    }
    return color * tint;
}


v2fEntity EntityVert(a2vEntity v)
{
    v2fEntity o;

    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
#if BURN_ON
    o.noise_uv = TRANSFORM_TEX(v.texcoord, _BurnNoise);
#endif
    o.color = v.color;
    return o;
}

fixed4 FragColor(fixed4 col, fixed4 tint) : SV_Target
{
    col = Tint(col, tint);
    col = Tint(col, _Color);
    
    col.rgb = _ColorOffset.rgb + col.rgb;

    return col;
}
fixed4 EntityFrag(v2fEntity i) :SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    col = FragColor(col, i.color);
#if BURN_ON
    col = Burn(col, i);
#endif
    return col;
}
fixed4 fragLighting(v2fEntity i) : SV_Target
{
    fixed4 c = EntityFrag(i);
    c.rgb = lerp(0, c.rgb, _Emission);
    return c;
}