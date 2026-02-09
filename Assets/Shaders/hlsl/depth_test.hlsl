#include "UnityCG.cginc"

int _DepthTest;
sampler2D _DepthMap;
float _DepthOffset;


float4 GetBackgroundDepthColor(float2 depthUV)
{
    return tex2D(_DepthMap, depthUV);
}
float4 GetBackgroundDepthColorLod(float4 depthUV)
{
    return tex2Dlod(_DepthMap, depthUV);
}
float SampleDepthByColor(float4 world, float4 color)
{
    if (color.a > 0.9)
    {
        float d = color.r * 255 + round(color.g * 5) * 255;
        float backgroundDepth = d / 100 - 2.1 + _DepthOffset;
        return world.z - backgroundDepth;
    }
    return 0;
}
float SampleDepth(float4 world, float2 depthUV)
{
    if (_DepthTest < 1)
        return 0;
    float4 color = GetBackgroundDepthColor(depthUV);
    return SampleDepthByColor(world, color);
}
float SampleDepthParticle(float4 world, float4 depthUV)
{
    if (_DepthTest < 1)
        return 0;
    float4 color = GetBackgroundDepthColorLod(depthUV);
    return SampleDepthByColor(world, color);
}
void ClipDepth(float4 world, float2 depthUV)
{
    float depth = SampleDepth(world, depthUV);
    clip(0.01 - depth);
}