Shader "Fungus/SlimeShader"
{
	Properties
	{
		_MainTex("Maske (nicht zuweisen)", 2D) = "white" {}
		_StructureTex("Struktur (zuweisen)", 2D) = "white" {}
		_Displacement("Wellen (zuweisen)", 2D) = "gray" {}
		_Speed("Geschwindigkeit", Range(0,0.1)) = 1 
		_Color("Tint", color) = (1,1,1,1)
		_CutOff("Schärfe", Range(0.1,0.9)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 200
		blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert alpha
		#pragma target 2.0

		sampler2D _MainTex;
		sampler2D _StructureTex;
		sampler2D _Displacement;
		fixed4 _Color;
		fixed _CutOff;
		fixed _Speed;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_StructureTex;
			float2 uv_Displacement;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float speed = frac(_Speed*_Time.g);
			fixed4 displacer = tex2D(_Displacement, IN.uv_Displacement+speed);
			float gray = ((displacer.r + displacer.g + displacer.b) / 3);
			float2 disp = float2(gray+0.7, gray+1); //offset correction - TODO: find autocompensation values
			fixed4 mask = tex2D(_MainTex, lerp(IN.uv_MainTex, disp, 0.01));
			fixed4 tex = tex2D(_StructureTex, lerp(IN.uv_StructureTex, disp, 0.05));
			o.Emission = tex.rgb*_Color;
			o.Alpha = saturate(clamp(mask.a-_CutOff,0,1)*10);
		}
		ENDCG
	}
	FallBack "Legacy/Transparent/Diffuse"
}
