#include "UnityCG.cginc"
#if LIT
#include "lighting.hlsl"
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
    float4 color : COLOR;
    float2 uv : TEXCOORD0;

    #if BURN_ON
    float2 noiseUV : TEXCOORD1;
    #endif

    #if LIT
    float2 lightUV : TEXCOORD2;
    #endif

    UNITY_VERTEX_OUTPUT_STEREO
};
sampler2D _MainTex;
float4 _MainTex_ST;
float4 _Color;
half4 _ColorOffset;
float3 _HSVOffset;

v2f_entity EntityVert(appdata_entity v)
{
    v2f_entity o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.color = v.color;
                            
#if BURN_ON
    o.noiseUV = TRANSFORM_TEX(v.uv, _BurnNoise);
#endif      
    
#if LIT
    o.lightUV = GetLightUV(v.vertex);
#endif
    
    return o;
}

half4 EntityFrag(v2f_entity i) : SV_Target
{
    half4 col = tex2D(_MainTex, i.uv);
    col = Tint(col, _Color);
    col = Tint(col, i.color);
#if HSV_TINT
    col = ModifyHSV(col, _HSVOffset);
#endif
                
    col.rgb = col.rgb + _ColorOffset.rgb;
                            
#if CIRCLE_TILED
    CircleTile(col, i.uv);
#endif
    
#if LIT
    col = ApplyLight(col, i.lightUV);
#endif

#if BURN_ON
    col = Burn(col, i.noiseUV, i.uv);
#endif
    clip(col.a - 0.01);
    
    return col;
}