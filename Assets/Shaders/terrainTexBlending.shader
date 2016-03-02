Shader "4Terrain/TexBlending" {
	Properties {
		_HeightMap2D ("HeightMap texture", 2D) = "red"{}
		_Rock2D("Rock texture", 2D) = "brown" {}
		_Grass2D("Grass texture", 2D) = "green" {}



		_Snow2D ("Snow texture", 2D) = "white"{}
		_Water2D ("Water texture", 2D) = "blue" {}

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _HeightMap2D;
		sampler2D _Snow2D;
		sampler2D _Grass2D;
		sampler2D _Rock2D;
		sampler2D _Water2D;

		struct Input {
			float2 uv_HeightMap2D;
			float2 uv_Snow2D;
			float2 uv_Grass2D;
			float2 uv_Rock2D;
			float2 uv_Water2D;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float4 height = tex2D(_HeightMap2D, IN.uv_HeightMap2D);
			float4 rock = tex2D(_Rock2D, IN.uv_Rock2D);
			float4 grass = tex2D(_Grass2D, IN.uv_Grass2D);

			float4 snow = tex2D(_Snow2D, IN.uv_Snow2D);
			float4 water = tex2D(_Water2D, IN.uv_Water2D);


			float3 lrp;

			float rate = height.rgb; 
			if (rate<0.5){
				lrp = snow.rgb * (1-rate) + rock.rgb*(rate);
			}else{
				lrp = rock.rgb * (1-rate) + grass.rgb*(rate);
			}



			o.Albedo = lrp;
		}
		ENDCG
	}
//	FallBack "Diffuse"
}
