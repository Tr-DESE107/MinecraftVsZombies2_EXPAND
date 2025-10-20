#include "UnityCG.cginc"

int _DepthTest;
sampler2D _DepthMap;
float _DepthOffset;


float4 GetBackgroundDepthColor(float2 depthUV)
{
    return tex2D(_DepthMap, depthUV);
}
void ClipDepth(float4 world, float2 depthUV)
{
    if (_DepthTest < 1)
        return;
    float4 color = GetBackgroundDepthColor(depthUV);
    if (color.a > 0.9)
    {
        float d = color.r * 255 + round(color.g * 5) * 255;
        float backgroundDepth = d / 100 - 2.1 + _DepthOffset;
        if (world.z > backgroundDepth)
        {
            discard;
        }
    }
}