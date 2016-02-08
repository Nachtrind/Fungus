Shader "Fungus/SelfGlowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_GlowColor ("Glow Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "black" {}
		_GlowTex ("GlowTex (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert noforwardadd halfasview approxview nolightmap


		sampler2D _MainTex;
		sampler2D _GlowTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GlowTex;
		};

		fixed4 _Color;
		fixed4 _GlowColor;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 g = tex2D (_GlowTex, IN.uv_GlowTex);
			o.Albedo = c.rgb * lerp(_Color.rgb, _GlowColor.rgb, g.a);
			o.Emission = c.rgb * g.a * _GlowColor.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
