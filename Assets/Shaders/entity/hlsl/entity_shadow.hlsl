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

    UNITY_VERTEX_OUTPUT_STEREO
};
            
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
half4 _MainTex_ST;
float4 _Color;
half4 _RendererColor;

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
            
Varyings ShadowVertex(Attributes v)
{
    Varyings o = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(v.positionOS);
#if defined(DEBUG_DISPLAY)
    o.positionWS = TransformObjectToWorld(v.positionOS);
#endif

    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
    o.color = v.color * _Color * _RendererColor;
    return o;
}

#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

half4 ShadowFragment(Varyings i) : SV_Target
{
    half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
    
    SurfaceData2D surfaceData;
    InputData2D inputData;

    InitializeSurfaceData(half3(1.0, 1.0, 1.0), main.a, half4(1,1,1,1), surfaceData);
    InitializeInputData(i.uv, i.lightingUV, inputData);
    
    half4 lightValue = CombinedShapeLightShared(surfaceData, inputData);
    half alpha = main.a;
    half r = lerp(1, lerp(main.r, 1, lightValue.r), alpha);
    half g = lerp(1, lerp(main.g, 1, lightValue.g), alpha);
    half b = lerp(1, lerp(main.b, 1, lightValue.b), alpha);
    return half4(r, g, b, alpha);
}