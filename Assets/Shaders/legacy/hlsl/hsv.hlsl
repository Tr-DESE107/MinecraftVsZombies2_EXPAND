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
half4 ModifyHSV(half4 color, float3 offset)
{
    float3 hsv = RGB2HSV(color.rgb);
    float hue = frac(hsv.x + offset.x / 360.0);
    float3 tintedHsv = float3(hue, hsv.y + offset.y / 100.0, hsv.z + offset.z / 100.0);
    return half4(HSV2RGB(tintedHsv), color.a);
}