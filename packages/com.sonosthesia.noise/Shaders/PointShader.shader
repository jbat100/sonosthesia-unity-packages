Shader "Graph/Point Surface" {
	SubShader {
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface)
		{
			surface.Albedo = half3(1, 1, 1);
			surface.Smoothness = 0.5;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
