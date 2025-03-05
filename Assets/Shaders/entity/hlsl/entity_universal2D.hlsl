#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            
            
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
    half2 lightingUV : TEXCOORD1;

    #if defined(DEBUG_DISPLAY)
    float3  positionWS  : TEXCOORD2;
    #endif

    #if BURN_ON
    float2  noiseUV        : TEXCOORD3;
    #endif

    UNITY_VERTEX_OUTPUT_STEREO
};
            
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
TEXTURE2D(_MaskTex);
SAMPLER(sampler_MaskTex);
CBUFFER_START(UnityPerMaterial)
half4 _MainTex_ST;
float4 _Color;
half4 _ColorOffset;
half4 _RendererColor;
CBUFFER_END

#if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
#endif

#if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
#endif

#if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
#endif

#if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
#endif
            
Varyings CombinedShapeLightVertex(Attributes v)
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
    o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
    o.color = v.color * _Color * _RendererColor;
    return o;
}

#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

half4 CombinedShapeLightFragment(Varyings i) : SV_Target
{
    half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

    main = Tint(main, i.color);
    
    main.rgb = _ColorOffset.rgb + main.rgb;
                
#if CIRCLE_TILED
    CircleTile(main, i.uv);
#endif

#if BURN_ON
    main = Burn(main, i.noiseUV, i.uv);
#endif

    const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
    SurfaceData2D surfaceData;
    InputData2D inputData;

    InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
    InitializeInputData(i.uv, i.lightingUV, inputData);

    return CombinedShapeLightShared(surfaceData, inputData);
}