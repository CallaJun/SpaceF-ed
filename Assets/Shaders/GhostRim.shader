Shader "Custom/Ghost Rim"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
	}
	
	Subshader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend One OneMinusSrcColor
		Cull Off
		Lighting Off
		ZWrite Off
		
		Pass
		{
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				struct v2f
				{
					float4 pos		: SV_POSITION;
					float2 nDotV	: TEXCOORD0;
				};
				
				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.nDotV = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal).xy;
					return o;
				}
				
				uniform float4 _Color;
				
				float4 frag (v2f i) : SV_TARGET
				{
					float intensity = clamp(length(i.nDotV) - 0.7, 0.05, 1);
					float4 mc = float4(intensity, intensity, intensity, 1);
					
					return _Color * mc * 2.0;
				}
			ENDCG
		}
	}
}