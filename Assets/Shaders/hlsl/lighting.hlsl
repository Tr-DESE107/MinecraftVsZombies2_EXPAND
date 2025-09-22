#include "UnityCG.cginc"

sampler2D _LightMapSpot;
float4 _LightMapST;
int _LightStarted;
float4 _LightGlobal;
float4 _LightBackground;
int _BackgroundLit;
int _SpotLit;
int _HDRDisabled;


float2 GetLightUV(float4 vertex)
{
    // 将模型空间坐标转换到世界空间
    float2 worldPos = mul(unity_ObjectToWorld, vertex);
    worldPos.y += 2.1;
    return (worldPos.xy + _LightMapST.zw) / _LightMapST.xy;
}
float4 ToLinear(float4 col)
{
    return pow(col, 2.2);
}
float4 ToGamma(float4 col)
{
    return pow(col, 1.0 / 2.2);
}

float4 GetGlobalLight()
{
    return _LightGlobal;
}
float4 GetBackgroundLight()
{
    return _LightBackground;
}
float4 GetSpotLight(float2 lightUV)
{
    return tex2D(_LightMapSpot, lightUV);
}
float4 GetLight(float2 lightUV)
{
    float4 light = GetGlobalLight();
    if (_BackgroundLit)
    {
        float4 background = GetBackgroundLight();
        light *= background;
    }
    if (_SpotLit)
    {
        float4 spot = GetSpotLight(lightUV);
        light += spot;
    }
    if (_HDRDisabled)
    {
        light = saturate(light);
    }
    return light;
}


#if defined(INSTANCING_ON)
UNITY_INSTANCING_BUFFER_START(Light_Props)
UNITY_DEFINE_INSTANCED_PROP(int, _LightDisabled)
UNITY_INSTANCING_BUFFER_END(Light_Props)

bool LightDisabled()
{
    return UNITY_ACCESS_INSTANCED_PROP(Light_Props, _LightDisabled) > 0;
}
#else
int _LightDisabled;
bool LightDisabled()
{
    return _LightDisabled > 0;
}
#endif

float4 ApplyLight(float4 col, float2 lightUV)
{
    if (!LightDisabled() && _LightStarted)
    {
        float4 light = GetLight(lightUV);
        float4 colLin = ToLinear(col);
        float4 lightLin = ToLinear(light);
        colLin.rgb *= lightLin.rgb * saturate(lightLin.a);
        colLin.rgb = saturate(colLin.rgb);
        col = ToGamma(colLin);
    }
    return col;
}