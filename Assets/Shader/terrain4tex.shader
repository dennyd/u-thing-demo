Shader "ForTerrain/terrain4tex" {
	Properties {
		_HeightMap2D ("HeightMap texture", 2D) = "red"{}
		_Snow2D ("Snow texture", 2D) = "white"{}
		_Grass2D("Grass texture", 2D) = "green" {}
		_Rock2D("Rock texture", 2D) = "brown" {}
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
//
//			// Albedo comes from a texture tinted by color
//			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
//			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;

			float4 height = tex2D(_HeightMap2D, IN.uv_HeightMap2D);

			float4 snow = tex2D(_Snow2D, IN.uv_Snow2D);
			float4 rock = tex2D(_Rock2D, IN.uv_Rock2D);
			float4 grass = tex2D(_Grass2D, IN.uv_Grass2D);
			float4 water = tex2D(_Water2D, IN.uv_Water2D);

			float scale = (height.x+height.y+height.z)/3;
			float rate = scale/(255);
			float4 lrp = snow;
			if(rate>75){
				lrp =  lerp(snow,rock,rate);			
			}else if(rate>50){
				lrp =  lerp(rock,grass,rate);
			}else if(rate>25){
				lrp =  lerp(grass,rock,rate);
			}
			else if(rate>0){
				lrp =  lerp(grass,water,rate);
			}
//			float4 t2 =  lerp(grass,water,height);

//			o.Albedo = lerp(rock,grass,height);
			o.Albedo = lrp;
		}
		ENDCG
	}
//	FallBack "Diffuse"
}
