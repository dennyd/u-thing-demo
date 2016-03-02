Shader "Custom/AP" {
	Properties {
		_HeightMap2D ("HeightMap texture", 2D) = "black"{}
		_T1("texture 1", 2D) = "white" {}				
		_T2("texture 2", 2D) = "white" {}				
		_T3("texture 3", 2D) = "white" {}				
		_T4("texture 4", 2D) = "white" {}				


		// snow
		// rock
		// grass
		// water


	}
	SubShader {
		Tags {
			"RenderType"="Opaque"
			"IgnoreProjector"="True"
		}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert decal:add

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _HeightMap2D;
		sampler2D _T1;
		sampler2D _T2;
		sampler2D _T3;
		sampler2D _T4;



		struct Input {
			float2 uv_HeightMap2D;
			float2 uv_T1;
			float2 uv_T2;
			float2 uv_T3;
			float2 uv_T4;
		};


		void surf (Input IN, inout SurfaceOutput o) {

			fixed3 height = tex2D (_HeightMap2D, IN.uv_HeightMap2D);
			fixed3 t1 = tex2D (_T1, IN.uv_T1);
			fixed3 t2 = tex2D (_T2, IN.uv_T2);
			fixed3 t3 = tex2D (_T3, IN.uv_T3);



			fixed3 col = (height+t1+t2+t3)/2;

			o.Albedo = col;
		}

		ENDCG
	}
//	FallBack "Diffuse"
}
