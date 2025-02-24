sampler2D _MainTex;
float4 _MainTex_ST;
half4 _MainTex_TexelSize;
fixed4 _Color;
fixed4 _ColorOffset;
fixed _Emission;
int _HSVTint;

#if BURN_ON
sampler2D _BurnNoise;
float4 _BurnNoise_ST;

fixed _BurnValue;
fixed4 _BurnEdgeColor;
fixed4 _BurnFireColor;
float _BurnEdgeThreshold;
#endif

struct a2vEntity
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    fixed4 color : COLOR;
};
struct v2fEntity
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
#if BURN_ON
    float2 noise_uv : TEXCOORD1;
#endif
    fixed4 color : COLOR;
};
