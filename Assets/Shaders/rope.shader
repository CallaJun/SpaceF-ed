Shader "Custom/Rope"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	Subshader
	{

		
		Pass
		{
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata {
					float4 pos : POSITION;
					float4 uv : TEXCOORD0;
					fixed4 color : COLOR;
				};
				
				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
				};
				
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.pos);
					o.color = v.color;
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				
				float4 frag (v2f i) : SV_TARGET
				{
					float4 baseColor = tex2D(_MainTex, i.uv);
					baseColor = saturate(baseColor * (1 - i.color.r) + i.color.r * i.color);
					return baseColor;
				}
			ENDCG
		}
	}
}