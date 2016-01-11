Shader "Fungus/Mobile/GroundShader"
{
	Properties
	{
		[HideInInspector]
		_MainTex("MainTex", 2D) = "black"
		[Header(Ground)]
		_Ground("Texture", 2D) = "gray" {}
		[Header(Slime)]
		_StructureTex("Texture", 2D) = "white" {}
		_Color("Tint", color) = (1,1,1,1)
		_PixelBorder("Size", Range(0,0.99)) = 0
		_Blockyness("PixelDivider", Range(1, 100)) = 50
		[Header(Glow)]
		_Spread("Blur", Range(0, 1)) = 0.005
		_CutOff("Spread", Range(0.01, 0.5)) = 0.05
		_GlowColor("Tint", color) = (1,1,1,1)
		[Header(Wobble)]
		_Speed("Speed", Range(0,0.1)) = 0.05
		_Strength("Slime Texture", Range(0.001, 0.05)) = 0.001
		_BorderStrength("Slime Edge", Range(0, 0.01)) = 0
		_GlowWobble("Glow", Range(0, 0.01)) = 0.001
		[Header(Light)]
		_Brightness("Slime self glow", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

		CGPROGRAM
		#pragma surface surf Lambert addshadow

		sampler2D _MainTex;
		uniform float4 _MainTex_TexelSize;
		sampler2D _StructureTex;
		fixed _Blockyness;
		sampler2D _Ground;
		fixed3 _Color;
		fixed3 _GlowColor;
		fixed _Speed;
		half _Spread;
		half _CutOff;
		half _Strength;
		fixed _PixelBorder;
		half _BorderStrength;
		half _GlowWobble;
		fixed _Brightness;

		struct Input
		{
			float2 uv_StructureTex;
			float2 uv_Ground;
			float4 screenPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			half aspect = _ScreenParams.x / _ScreenParams.y;
			IN.screenPos.w += 0.00001;
			half2 screenSpace = IN.screenPos.xy / IN.screenPos.w;
			half movement = frac(_Speed*_Time.g);
			fixed wobbleStrength = _Strength;
			half3 displacer = tex2D(_StructureTex, IN.uv_StructureTex + movement);
			half3 displacer2 = tex2D(_StructureTex, IN.uv_StructureTex - movement);
			half gray = dot(lerp(displacer.rgb, displacer2.rgb, 0.5), fixed3(0.3, 0.59, 0.11));
			half2 disp = fixed2(gray*_ScreenParams.z, gray*_ScreenParams.w);

			half3 slime = tex2D(_MainTex, lerp(screenSpace, disp, _BorderStrength));
			half2 slimeUV = lerp(IN.uv_StructureTex, disp, wobbleStrength); 
			slimeUV = round(slimeUV * _Blockyness) / _Blockyness;
			half3 slimeTexture = tex2D(_StructureTex, slimeUV);
			half3 groundTexture = tex2D(_Ground, IN.uv_Ground);

			half spread = _Spread * _MainTex_TexelSize;
			
			half3 mask = fixed3(0, 0, 0);
			fixed _GlowSteps = 1;
			for (fixed x = -_GlowSteps;x <= _GlowSteps;x++)
			{
				for (fixed y = -_GlowSteps;y <= _GlowSteps;y++)
				{
					fixed2 maskUV = fixed2(screenSpace.x + (x*spread), screenSpace.y + (y*spread*aspect));
					maskUV = lerp(maskUV, disp, _GlowWobble);
					mask += tex2D(_MainTex, maskUV);
				}
			}
			mask /= 9;
			mask.g = lerp(mask.g, 0, _CutOff);
			fixed3 glowSize = clamp(step(1-_PixelBorder, slime.g), 0, 1);
			half3 glow = clamp(mask.g - glowSize.g, 0, 1);
			half3 final = lerp(groundTexture, slimeTexture*_Color * 2, glowSize);
			o.Albedo = final + (glow*_GlowColor * 2)*(1 - _Brightness);
			o.Emission = ((slimeTexture*_Color*glowSize.g) + (glow*_GlowColor * 2))*_Brightness;
		}
		ENDCG
	}
	FallBack "Legacy/Diffuse"
}
