float3 RGB2HSV(half3 rgb)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    float h = abs(q.z + (q.w - q.y) / (6.0 * d + e));
    float s = d / (q.x + e);
    float v = q.x;
    return float3(h, s, v);
}
half3 HSV2RGB(float3 hsv)
{
    float h = hsv.x;
    float s = hsv.y;
    float v = hsv.z;
    float3 modular = fmod(h * 6.0 + float3(0.0, 4.0, 2.0), 6.0);
    float3 absolute = abs(modular - 3.0);
    float3 rgb = clamp(absolute - 1.0, 0, 1);
    return v * lerp(float3(1, 1, 1), rgb, s);
}
half4 ModifyHSVNormalized(half4 color, float3 offset)
{
    float3 hsv = RGB2HSV(color.rgb);
    float hue = frac(hsv.x + offset.x);
    float sat = clamp(hsv.y + offset.y, 0, 1);
    float value = clamp(hsv.z + offset.z, 0, 1);
    float3 tintedHsv = float3(hue, sat, value);
    return half4(HSV2RGB(tintedHsv), color.a);
}
half4 ModifyHSV(half4 color, float3 offset)
{
    float3 normalized = float3(offset.x / 360.0, offset.y / 100.0, offset.z / 100.0);
    return ModifyHSVNormalized(color, normalized);
}