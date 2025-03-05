struct Attributes
{
    float3 positionOS : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    float4 tangent : TANGENT;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    half4 color : COLOR;
    float2 uv : TEXCOORD0;
    half3 normalWS : TEXCOORD1;
    half3 tangentWS : TEXCOORD2;
    half3 bitangentWS : TEXCOORD3;
    UNITY_VERTEX_OUTPUT_STEREO
};
            
TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);
half4 _NormalMap_ST; // Is this the right way to do this?

Varyings NormalsRenderingVertex(Attributes attributes)
{
    Varyings o = (Varyings) 0;
    UNITY_SETUP_INSTANCE_ID(attributes);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(attributes.positionOS);
    o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
    o.color = attributes.color;
    o.normalWS = -GetViewForwardDir();
    o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
    o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
    return o;
}

#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

half4 NormalsRenderingFragment(Varyings i) : SV_Target
{
    const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
    const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

    return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
}