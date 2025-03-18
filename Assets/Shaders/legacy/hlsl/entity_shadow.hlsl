#include "lighting.hlsl"
            
            
struct Attributes
{
    float4 vertex : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 vertex : SV_POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    half2 lightUV : TEXCOORD1;

    #if defined(DEBUG_DISPLAY)
    float3  positionWS  : TEXCOORD2;
    #endif

    UNITY_VERTEX_OUTPUT_STEREO
};
            
sampler2D _MainTex;
half4 _MainTex_ST;
float4 _Color;
half _LightFactor;
half4 _RendererColor;

Varyings ShadowVertex(Attributes v)
{
    Varyings o = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.vertex = UnityObjectToClipPos(v.vertex);
    o.lightUV = GetLightUV(v.vertex);

    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color * _Color * _RendererColor;
    return o;
}

half4 ShadowFragment(Varyings i) : SV_Target
{
    half4 main = tex2D(_MainTex, i.uv) * i.color;
    
    half4 lightValue = GetSpotLight(i.lightUV);
    half alpha = main.a;
    half r = lerp(1, lerp(main.r, 1, lightValue.r * _LightFactor), alpha);
    half g = lerp(1, lerp(main.g, 1, lightValue.g * _LightFactor), alpha);
    half b = lerp(1, lerp(main.b, 1, lightValue.b * _LightFactor), alpha);
    return half4(r, g, b, alpha);
}