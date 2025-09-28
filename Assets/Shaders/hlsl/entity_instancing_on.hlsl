#include "UnityCG.cginc"

#include "level.hlsl"
#include "burn.hlsl"
#include "hsv.hlsl"
#include "grayscale.hlsl"

#if LIT
#include "lighting.hlsl"
#endif

#if DEPTH_TEST
#include "depth_test.hlsl"
#endif
        
#if CIRCLE_TILED
#include "circle_tile.hlsl"
#endif

struct appdata_entity
{
    float4 vertex : POSITION;
    float4 color : COLOR0;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f_entity
{
    float4 vertex : SV_POSITION;
    float4 color : COLOR0;
    float2 uv : TEXCOORD0;
    float4 world : TEXCOORD1;
    float2 levelUV : TEXCOORD2;
    float2 noiseUV : TEXCOORD3;
    UNITY_VERTEX_INPUT_INSTANCE_ID 
};

sampler2D _MainTex;
float4 _MainTex_ST;
UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
UNITY_DEFINE_INSTANCED_PROP(half4, _ColorOffset)
UNITY_DEFINE_INSTANCED_PROP(float3, _HSVOffset)
UNITY_DEFINE_INSTANCED_PROP(int, _Grayscale)
UNITY_DEFINE_INSTANCED_PROP(half3, _GrayscaleFactor)
UNITY_INSTANCING_BUFFER_END(Props)

sampler2D _BurnNoise;
float4 _BurnNoise_ST;
UNITY_INSTANCING_BUFFER_START(Burn_Props)
UNITY_DEFINE_INSTANCED_PROP(half, _BurnValue)
UNITY_DEFINE_INSTANCED_PROP(half4, _BurnEdgeColor)
UNITY_DEFINE_INSTANCED_PROP(half4, _BurnFireColor)
UNITY_DEFINE_INSTANCED_PROP(float, _BurnEdgeThreshold)
UNITY_INSTANCING_BUFFER_END(Burn_Props)
burn_struct GetBurnParameters(v2f_entity i)
{
    burn_struct o;
    o.noise = _BurnNoise;
    o.value = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnValue);
    o.edgeThresold = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnEdgeThreshold);
    o.edgeColor = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnEdgeColor);
    o.fireColor = UNITY_ACCESS_INSTANCED_PROP(Burn_Props, _BurnFireColor);
    o.uv = float4(i.uv, i.noiseUV);
    return o;
}

v2f_entity EntityVert(appdata_entity v)
{
    v2f_entity o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    
    o.levelUV = GetLevelUV(v.vertex);
    o.world = mul(unity_ObjectToWorld, v.vertex);
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color;
    o.noiseUV = TRANSFORM_TEX(v.uv, _BurnNoise);
    return o;
}

half4 EntityFrag(v2f_entity i) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);
#if DEPTH_TEST
    ClipDepth(i.world, i.levelUV);
#endif
    
    half4 col = tex2D(_MainTex, i.uv);
    col *= i.color;
    if (UNITY_ACCESS_INSTANCED_PROP(Props, _Grayscale) > 0)
    {
        col = Grayscale(col, UNITY_ACCESS_INSTANCED_PROP(Props, _GrayscaleFactor));
    }
    col *= UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
    col = ModifyHSV(col, UNITY_ACCESS_INSTANCED_PROP(Props, _HSVOffset));
                
    col.rgb = col.rgb + UNITY_ACCESS_INSTANCED_PROP(Props, _ColorOffset).rgb;
                            
#if CIRCLE_TILED
    CircleTile(col, i.uv);
#endif
    
    burn_struct burnParams = GetBurnParameters(i);
    col = Burn(col, burnParams);
    
#if LIT
    col = ApplyLight(col, i.levelUV);
#endif
    
    clip(col.a - 0.01);
    
    return col;
}