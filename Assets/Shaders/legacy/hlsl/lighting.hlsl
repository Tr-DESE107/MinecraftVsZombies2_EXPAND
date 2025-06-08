#include "UnityCG.cginc"

sampler2D _LightMapSpot;
float4 _LightMapST;
int _LightStarted;
half4 _LightGlobal;
half4 _LightBackground;
int _BackgroundLit;


float2 GetLightUV(float4 vertex)
{
    // 将模型空间坐标转换到世界空间
    float2 worldPos = mul(unity_ObjectToWorld, vertex);
    worldPos.y += 2.1;
    return (worldPos.xy + _LightMapST.zw) / _LightMapST.xy;
}
half4 ToLinear(half4 col)
{
    return pow(col, 2.2);
}
half4 ToGamma(half4 col)
{
    return pow(col, 1.0 / 2.2);
}

half4 GetGlobalLight()
{
    return _LightGlobal;
}
half4 GetBackgroundLight()
{
    return _LightBackground;
}
half4 GetSpotLight(float2 lightUV)
{
    return tex2D(_LightMapSpot, lightUV);
}
half4 GetLight(float2 lightUV)
{
    half4 global = GetGlobalLight();
    half4 spot = GetSpotLight(lightUV);
    if (_BackgroundLit)
    {
        half4 background = GetBackgroundLight();
        return spot + global * background;
    }
    else
    {
        return spot + global;
    }
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

half4 ApplyLight(half4 col, float2 lightUV)
{
    if (!LightDisabled() && _LightStarted)
    {
        half4 light = GetLight(lightUV);
        half4 colLin = ToLinear(col);
        half4 lightLin = ToLinear(light);
        colLin.rgb *= lightLin.rgb * saturate(lightLin.a);
        colLin.rgb = saturate(colLin.rgb);
        col = ToGamma(colLin);
    }
    return col;
}