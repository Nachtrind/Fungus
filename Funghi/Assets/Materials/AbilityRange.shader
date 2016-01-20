Shader "Fungus/AbilityRange" {
	Properties{
		[HideInInspector]
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[HideInInspector]
		_ScaleTex("Scale", 2D) = "Black" {}
		_Color("Center Tint", Color) = (1,1,1,1)
		_HealthMarginSize("Health Margin size", Range(0, 1)) = 0.1
		_ColorMargin("Health Margin Tint", Color) = (1,1,1,1)
		[HideInInspector]
		_HPTex("HP tex", 2D) = "white" {}
		_HP("Health percentage (normalized)", Range(0,1)) = 0
		_ColorHP("Health Lost Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue"="Transparent+1" }
			LOD 200
			cull off
			zwrite off
			ztest off
			Lighting off
			Fog{ Color(0,0,0,0) }
			blend One One
			offset -2,0

		CGPROGRAM
		#pragma surface surf Unlit nofog approxview nolightmap halfasview noforwardadd

		fixed4 _Color;
		sampler2D _MainTex;
		fixed _HealthMarginSize;
		sampler2D _ScaleTex;
		fixed4 _ColorMargin;
		sampler2D _HPTex;
		fixed4 _ColorHP;
		fixed _HP;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			half4 c;
			c.rgb = s.Albedo;
			c.a = 1;
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 hpBase = tex2D(_ScaleTex, IN.uv_MainTex);
			fixed4 hpBlend = tex2D(_HPTex, IN.uv_MainTex);
			fixed3 ring = (1 - step(hpBase, _HealthMarginSize)).rgb;
			fixed3 hp = step(hpBlend.rgb, _HP).rgb;
			fixed3 margin = clamp(tex - ring, 0, 1);
			ring = lerp(margin*_ColorHP*_ColorHP.a, margin*_ColorMargin*_ColorMargin.a, hp);
			o.Albedo = (tex - margin)*_Color.rgb*_Color.a+ ring;
		}
		ENDCG
		}
		FallBack "Legacy/Additive"
}
