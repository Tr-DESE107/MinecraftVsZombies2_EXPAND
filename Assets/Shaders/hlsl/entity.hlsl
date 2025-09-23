#include "UnityCG.cginc"

#include "level.hlsl"

#if LIT
#include "lighting.hlsl"
#endif

#if DEPTH_TEST
#include "depth_test.hlsl"
#endif
        
#if BURN_ON
#include "burn.hlsl"
#endif

#if CIRCLE_TILED
#include "circle_tile.hlsl"
#endif
        
#if HSV_TINT
#include "hsv.hlsl"
#endif
        
half4 Tint(half4 color, half4 tint)
{
    return color * tint;
}

half4 Grayscale(half4 color, half3 factor)
{
    //获取灰度值
    half _texGray = dot(color.rgb, factor);
    //将灰度值分别赋予rgb三色通道
    return half4(_texGray, _texGray, _texGray, color.a);
}

struct appdata_entity
{
    float4 vertex : POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f_entity
{
    float4 vertex : SV_POSITION;
    float4 world : TEXCOORD1;
    float2 levelUV : TEXCOORD2;
    float4 color : COLOR;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID 

    #if BURN_ON
    float2 noiseUV : TEXCOORD3;
    #endif
};
sampler2D _MainTex;
float4 _MainTex_ST;
#if defined(INSTANCING_ON)
UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
UNITY_DEFINE_INSTANCED_PROP(half4, _ColorOffset)
UNITY_DEFINE_INSTANCED_PROP(float3, _HSVOffset)
UNITY_DEFINE_INSTANCED_PROP(int, _Grayscale)
UNITY_DEFINE_INSTANCED_PROP(half3, _GrayscaleFactor)
UNITY_INSTANCING_BUFFER_END(Props)
#else
float4 _Color;
half4 _ColorOffset;
float3 _HSVOffset;
int _Grayscale;
half3 _GrayscaleFactor;
#endif

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
                            
#if BURN_ON
    o.noiseUV = TRANSFORM_TEX(v.uv, _BurnNoise);
#endif
    
    return o;
}


#if defined(INSTANCING_ON)
half4 EntityFrag(v2f_entity i) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);
    
    half4 col = tex2D(_MainTex, i.uv);
    col = Tint(col, UNITY_ACCESS_INSTANCED_PROP(Props, _Color));
    col = Tint(col, i.color);
#if HSV_TINT
    col = ModifyHSV(col, UNITY_ACCESS_INSTANCED_PROP(Props, _HSVOffset));
#endif
    if (UNITY_ACCESS_INSTANCED_PROP(Props, _Grayscale) > 0)
    {
        col = Grayscale(col, UNITY_ACCESS_INSTANCED_PROP(Props, _GrayscaleFactor));
    }
                
    col.rgb = col.rgb + UNITY_ACCESS_INSTANCED_PROP(Props, _ColorOffset).rgb;
                            
#if CIRCLE_TILED
    CircleTile(col, i.uv);
#endif
    
#if LIT
    col = ApplyLight(col, i.levelUV);
#endif

#if BURN_ON
    col = Burn(col, i.noiseUV, i.uv);
#endif
    clip(col.a - 0.01);
    
    return col;
}
#else
half4 EntityFrag(v2f_entity i) : SV_Target
{
#if DEPTH_TEST
    ClipDepth(i.world, i.levelUV);
#endif
    
    half4 col = tex2D(_MainTex, i.uv);
    col = Tint(col, i.color);
    if (_Grayscale > 0)
    {
        col = Grayscale(col, _GrayscaleFactor);
    }
    col = Tint(col, _Color);
#if HSV_TINT
    col = ModifyHSV(col, _HSVOffset);
#endif
                
    col.rgb = col.rgb + _ColorOffset.rgb;
                            
#if CIRCLE_TILED
    CircleTile(col, i.uv);
#endif
    
#if LIT
    col = ApplyLight(col, i.levelUV);
#endif
    

#if BURN_ON
    col = Burn(col, i.noiseUV, i.uv);
#endif
    clip(col.a - 0.01);
    
    return col;
}
#endif
