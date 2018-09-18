// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Black Out" {
	Properties {
		_Magnitude("Magnitude", Range(0, 1)) = 1
		_MainTex("Main Texture", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};
			struct v2f {
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
			};
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			sampler2D _MainTex;
			float _Magnitude;
			float4 frag(v2f i) : SV_TARGET {
				float4 color = tex2D(_MainTex, i.uv) * saturate(1 - _Magnitude);
				return color;
			}
			ENDCG
		}

		
	}
}
