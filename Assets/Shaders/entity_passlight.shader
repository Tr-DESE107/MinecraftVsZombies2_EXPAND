Shader "MinecraftVSZombies2/Entity/PassLight"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		[Header(Color Offset)]
		[Space(10)]
		_ColorOffset("Color Offset", Color) = (0,0,0,0)

		[Header(Burn)]
		[Toggle(BURN_ON)]
		_Burn("Can Burn", Int) = 0
		_BurnNoise("Noise Tex", 2D) = "white"{}
		_BurnValue("Value", Range(0, 1)) = 0
		_BurnEdgeColor("Edge Color", Color) = (0,0.5,0.5,1)
		_BurnEdgeThreshold("Edge Threshold", Float) = 0.2
		_BurnFireColor("Fire Color", Color) = (0,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"Lighting2D" = "Pass"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		UsePass "MinecraftVSZombies2/Entity/Entity/Main"
	}
	FallBack "MinecraftVSZombies2/Entity/Entity"
}
