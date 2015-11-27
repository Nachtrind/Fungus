Shader "Fungus/GroundShader"
{
	Properties
	{
		_MainTex("Schleim (nicht zuweisen)", 2D) = "white" {}
		_StructureTex("Struktur (zuweisen)", 2D) = "white" {}
		_Ground("Boden (zuweisen)", 2D) = "black" {}
		_Speed("Geschwindigkeit", Range(0,0.1)) = 0.05
		_Strength("Stärke", Range(0, 0.05)) = 0.05
		_Color("Tint", color) = (1,1,1,1)
		_Spread("Blur", Range(0.001, 0.1)) = 0.005
		_CutOff("Schärfe", Range(0.01, 0.5)) = 0.05
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Standard addshadow
		#pragma target 2.0

		sampler2D _MainTex;
		sampler2D _StructureTex;
		sampler2D _Ground;
		fixed4 _Color;
		fixed _Speed;
		fixed _Strength;
		half _Spread;
		half _CutOff;

		struct Input
		{
			float2 uv_StructureTex;
			float2 uv_Ground;
			float4 screenPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed speed = frac(_Speed*_Time.g);
			fixed3 displacer = tex2D(_StructureTex, IN.uv_StructureTex+speed);
			fixed gray = dot(displacer.rgb, fixed3(0.3, 0.59, 0.11));
			fixed2 disp = fixed2(gray, gray);
			fixed3 mask = fixed3(0, 0, 0);
			for (fixed x = -1;x <= 1;x++) 
			{
				for (fixed y = -1;y <= 1;y++) 
				{
					mask += tex2D(_MainTex, lerp(fixed2(IN.screenPos.x+(x*_Spread), IN.screenPos.y+(y*_Spread)), disp, _Strength));
				}
			}
			mask /= 9;
			fixed slimesharpness = step(_CutOff, mask.g);
			fixed3 slime = tex2D(_StructureTex, lerp(IN.uv_StructureTex, disp, _Strength*5));
			fixed3 ground = tex2D(_Ground, IN.uv_Ground);
			o.Albedo = lerp(slime*_Color*2, ground, 1-slimesharpness);
		}
		ENDCG
	}
	FallBack "Legacy/Transparent/Diffuse"
}
