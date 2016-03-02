Shader "Custom/FP" {
	Properties {
		_HeightMap2D ("HeightMap texture", 2D) = "black"{}

		//snow 
		_T1("texture 1", 2D) = "white" {}
		// rock
		_T2("texture 2", 2D) = "white" {}				
		// grass
		_T3("texture 3", 2D) = "white" {}					


		_T4("texture 4", 2D) = "white" {}	
		// lava			
		_T5("texture 5", 2D) = "white" {}				
		// water
		_T6("texture 6", 2D) = "white" {}				
		// toxic

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

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
		};


		void surf (Input IN, inout SurfaceOutput o) {

			float3 hight = tex2D (_HeightMap2D, IN.uv_HeightMap2D);
			float3 col = hight ;


			float3 t1 = tex2D (_T1, IN.uv_T1);
			float3 t2 = tex2D (_T2, IN.uv_T2);
			float3 t3 = tex2D (_T3, IN.uv_T3);
	
			float rate = hight;
			float r = 1/5.0f;


			if (rate > (4*r) ){

				float minRate = r*4;
				float maxRate = r*5;
				col = t3;
			}
			else if(rate > (3*r)){

				float minRate = r*3;
				float maxRate = r*4;
				col = lerp(t2,t3,(rate-minRate)/(maxRate-minRate));

			}else if(rate > (2*r)){
				float minRate = r*2;
				float maxRate = r*3;
				col = t2;
			}else if(rate > (r)){
				float minRate = r*1;
				float maxRate = r*2;
				col = lerp(t1,t2,(rate-minRate)/(maxRate-minRate));
			}else if(rate > 0.0f){
				float minRate = 0;
				float maxRate = r*1;
				col = t1;
			}


						

			o.Albedo = col;
		}

		ENDCG

		// NEXT 
		// NEXT 
		// NEXT
		// NEXT 
		// NEXT 
		// NEXT



		CGPROGRAM
		#pragma surface surf Lambert decal:add

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _HeightMap2D;
		sampler2D _T4;
		sampler2D _T5;
		sampler2D _T6;

		struct Input {
			float2 uv_HeightMap2D;
			float2 uv_T4;
			float2 uv_T5;
			float2 uv_T6;
		};


		void surf (Input IN, inout SurfaceOutput o) {

			float3 hight = tex2D (_HeightMap2D, IN.uv_HeightMap2D);
			float3 col = hight ;

			float3 t4 = tex2D (_T4, IN.uv_T4);
			float3 t5 = tex2D (_T5, IN.uv_T5);
			float3 t6 = tex2D (_T6, IN.uv_T6);
	
			float rate = hight;
			float r = 1/5.0f;


			if (rate > (4*r) ){
				float minRate = r*4;
				float maxRate = r*5;
//				col = t3;
			}
			else if(rate > (3*r)){
				float minRate = r*3;
				float maxRate = r*4;
//				col = lerp(t2,t3,(rate-minRate)/(maxRate-minRate));

			}else if(rate > (2*r)){
				float minRate = r*2;
				float maxRate = r*3;
//				col += t4/3;
//				o.Albedo = col;
			}else if(rate > (r)){
				float minRate = r*1;
				float maxRate = r*2;
//				col = lerp(t1,t2,(rate-minRate)/(maxRate-minRate));
			}else if(rate > 0.0f){
				float minRate = 0;
				float maxRate = r*1;
//				col = t1;
			}




		}

		ENDCG


	}
	//Dependency "AddPassShader" = "Custom/AP"
//	FallBack "Diffuse"
}
