Shader "ForTerrain/Shader2"{

	Properties{
		_Color ("Color", color)	= (1.0,1.0,1.0,1.0)
	}

	SubShader{
		Pass{
			CGPROGRAM


			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;

			//unity defined var
			uniform float4 _LightColor0;



			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
			};

			struct vertexOutput{
				float4 pos: SV_POSITION;
				float4 col: COLOR;
			};


			//vertex

			vertexOutput vert(vertexInput v){
				vertexOutput o;
				float3 normalDirection = mul(float4(v.normal,0.0),_World2Object).xyz;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.col = float4(v.normal, 1.0);
				return o;
			}

			//fragment

			float4 frag(vertexOutput i): COLOR{
				return i.col;
			}

			ENDCG
		}
	}
//	Fallback "Diffuse"
}