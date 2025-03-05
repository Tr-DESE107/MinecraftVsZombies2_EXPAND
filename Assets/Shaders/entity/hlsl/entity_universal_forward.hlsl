struct Attributes
{
    float3 positionOS : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;

    #if defined(DEBUG_DISPLAY)
    float3  positionWS  : TEXCOORD2;
    #endif

    #if BURN_ON
    float2  noiseUV        : TEXCOORD3;
    #endif

    UNITY_VERTEX_OUTPUT_STEREO
};
           
CBUFFER_START(UnityPerMaterial)
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_ST;
half4 _ColorOffset;
float4 _Color;
half4 _RendererColor;
CBUFFER_END
            
            
Varyings UnlitVertex(Attributes v)
{
    Varyings o = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(v.positionOS);
#if defined(DEBUG_DISPLAY)
    o.positionWS = TransformObjectToWorld(v.positionOS);
#endif
                
#if BURN_ON
    o.noiseUV = TRANSFORM_TEX(v.uv, _BurnNoise);
#endif

    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color * _Color * _RendererColor;
    return o;
}
float4 UnlitFragment(Varyings i) : SV_Target
{
    float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

    col = Tint(col, i.color);
    
    col.rgb = _ColorOffset.rgb + col.rgb;

#if CIRCLE_TILED
    CircleTile(col, i.uv);
#endif

#if BURN_ON
    col = Burn(col, i.noiseUV, i.uv);
#endif

#if defined(DEBUG_DISPLAY)
    SurfaceData2D surfaceData;
    InputData2D inputData;
    half4 debugColor = 0;

    InitializeSurfaceData(col.rgb, col.a, surfaceData);
    InitializeInputData(i.uv, inputData);
    SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

    if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
    {
        return debugColor;
    }
#endif

    return col;
}