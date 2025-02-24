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
            "RenderType"="Entity3D" 
        }
        Pass {
		    Name "ZWrite"

            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #include "cg/entity_model.cginc"
            #pragma vertex vert
            #pragma fragment fragMask
            ENDCG
        }
        Pass {
		    Name "Main"

            Tags { "LightMode" = "ForwardBase" }
            ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            CGPROGRAM
            #include "cg/entity_model.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack "Diffuse"
}
