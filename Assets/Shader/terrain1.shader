Shader "ForTerrain/Shader1"{

	Properties{
		_Color ("Color", color)	= (1.0,1.0,1.0,1.0)
	}
	//d s
	SubShader{
		Pass{
			CGPROGRAM


			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;

			struct vertexInput{
				float4 vertex: POSITION;
			};

			struct vertexOutput{
				float4 pos: SV_POSITION;
			};


			//vertex

			vertexOutput vert(vertexInput v){
				vertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			//fragment

			float4 frag(vertexOutput i): COLOR{
				return _Color;
			}

			ENDCG
		}
	}
//	Fallback "Diffuse"
}