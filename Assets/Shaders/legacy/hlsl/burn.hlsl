sampler2D _BurnNoise;
float4 _BurnNoise_ST;
#if defined(INSTANCING_ON)
UNITY_INSTANCING_BUFFER_START(Burn_Props)
UNITY_DEFINE_INSTANCED_PROP(half, _BurnValue)
UNITY_DEFINE_INSTANCED_PROP(half4, _BurnEdgeColor)
UNITY_DEFINE_INSTANCED_PROP(half4, _BurnFireColor)
UNITY_DEFINE_INSTANCED_PROP(float, _BurnEdgeThreshold)
UNITY_INSTANCING_BUFFER_END(Burn_Props)

half4 Burn(half4 col, float2 noiseUV, float2 uv)
{
    half value = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnValue);
    if (value)
    {
        half noise = tex2D(_BurnNoise, noiseUV).r - value;

        clip(noise);

        half4 edgeColor = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnEdgeColor);
        float edgeThresold = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnEdgeThreshold);
        if (noise <= edgeThresold + uv.y)
        {
            col.rgb = lerp(edgeColor.rgb, col.rgb, saturate(noise / edgeThresold));
        }

        if (noise <= edgeThresold * 0.023)
        {
            half4 fireColor = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnFireColor);
            col.rgb = fireColor.rgb;
        }
    }
    return col;
}
#else
half _BurnValue;
half4 _BurnEdgeColor;
half4 _BurnFireColor;
float _BurnEdgeThreshold;

half4 Burn(half4 col, float2 noiseUV, float2 uv)
{
    if (_BurnValue)
    {
        half noise = tex2D(_BurnNoise, noiseUV).r - _BurnValue;

        clip(noise);
        if (noise <= _BurnEdgeThreshold + uv.y)
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