TEXTURE2D(_BurnNoise);
SAMPLER(sampler_BurnNoise);
float4 _BurnNoise_ST;
half _BurnValue;
half4 _BurnEdgeColor;
half4 _BurnFireColor;
float _BurnEdgeThreshold;

half4 Burn(half4 col, float2 noiseUV, float2 uv)
{
    if (_BurnValue)
    {
        half noise = SAMPLE_TEXTURE2D(_BurnNoise, sampler_BurnNoise, noiseUV).r - _BurnValue;

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