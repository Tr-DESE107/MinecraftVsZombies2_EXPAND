Shader "MinecraftVSZombies2/Entity/Model3D"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", Range(0,1)) = 0
		_Cutoff("Cutoff", Range(0,1)) = 0

		[Header(Color Offset)]
		[Space(10)]
		_ColorOffset("Color Offset", Color) = (0,0,0,0)
        
    }
    SubShader
    {
        Tags 
        { 
			"Queue" = "Transparent"
            "RenderType"="Transparent" 
        }
        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #include "Lighting.cginc"

        float4 _Color;
        fixed4 _ColorOffset;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed _Emission;
        fixed _Cutoff;

        struct a2v {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float3 texcoord : TEXCOORD0;
        };
        struct v2f {
            float4 pos : SV_POSITION;
            float3 worldNormal : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
            float2 uv : TEXCOORD2;
        };

        v2f vert(a2v v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            return o;
        }
        fixed4 frag(v2f i) : SV_Target
        {
            fixed3 worldNormal = normalize(i.worldNormal);
            fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            fixed4 color = tex2D(_MainTex, i.uv);
            color *= _Color;
            clip (color.a - _Cutoff);
            color.rgb = _ColorOffset.rgb + color.rgb;
            fixed3 albedo = color.rgb;
            fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
            fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
            fixed3 emission = albedo * _Emission;
            return fixed4(emission + ambient + diffuse, color.a);
        }
        ENDCG
        Pass {
            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragMask
            
            fixed4 fragMask(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                color *= _Color;
                clip (color.a - 1);
                return color;
            }
            ENDCG
        }
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack "Diffuse"
}
