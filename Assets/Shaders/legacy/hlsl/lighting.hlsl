#include "UnityCG.cginc"

sampler2D _LightMapSpot;
sampler2D _LightMapGlobal;
float4 _LightMapST;

float2 GetLightUV(float4 vertex)
{
    // 将模型空间坐标转换到世界空间
    float2 worldPos = mul(unity_ObjectToWorld, vertex);
    worldPos.y += 2.1;
    return (worldPos.xy + _LightMapST.zw) / _LightMapST.xy;
}
half4 GetGlobalLight(float2 lightUV)
{
    return tex2D(_LightMapGlobal, lightUV);
}
half4 GetSpotLight(float2 lightUV)
{
    return tex2D(_LightMapSpot, lightUV);
}
half4 GetLight(float2 lightUV)
{
    return GetGlobalLight(lightUV) + GetSpotLight(lightUV);
}
half4 ApplyLight(half4 col, float2 lightUV)
{
    half4 light = GetLight(lightUV);
    col.rgb *= saturate(light.rgb * light.a);
    return col;
}