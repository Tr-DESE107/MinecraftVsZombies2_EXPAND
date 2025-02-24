#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "entity_common.cginc"
#pragma target 3.0

fixed _Cutoff;

struct a2vModel
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float3 texcoord : TEXCOORD0;
};
struct v2fModel
{
    float4 pos : SV_POSITION;
    float3 worldNormal : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    float2 uv : TEXCOORD2;
};

v2fModel vert(a2vModel v)
{
    v2fModel o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    return o;
}
fixed4 frag(v2fModel i) : SV_Target
{
    fixed3 worldNormal = normalize(i.worldNormal);
    fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
    fixed4 color = tex2D(_MainTex, i.uv);
    color *= _Color;
    clip(color.a - _Cutoff);
    color.rgb = _ColorOffset.rgb + color.rgb;
    fixed3 albedo = color.rgb;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
    fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
    fixed3 emission = albedo * _Emission;
    return fixed4(emission + ambient + diffuse, color.a);
}
fixed4 fragLighting(v2fModel i) : SV_Target
{
    fixed4 color = tex2D(_MainTex, i.uv);
    color *= _Color;
    clip(color.a - _Cutoff);
    color.rgb = _ColorOffset.rgb + color.rgb;
    fixed3 albedo = color.rgb;
    fixed3 emission = albedo * _Emission;
    return fixed4(emission, color.a);
}
            
fixed4 fragMask(v2fModel i) : SV_Target
{
    fixed4 color = tex2D(_MainTex, i.uv);
    color *= _Color;
    clip(color.a - 1);
    return color;
}