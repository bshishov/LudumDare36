// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Billboard" {
	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Texture", 2D) = "white" {}
		_TransparentCutout("Cutout Color", Range(0.0,50.0)) = 1.0
		_BackgroundColor("BG Color", Color) = (0.0,0.0,0.0,1.0)
		_SrcBlend("Src Blend", Int) = 0
		_DstBlend("Dst Blend", Int) = 0
	}
	SubShader
	{		
		Tags{ "Queue"="Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"	"PreviewType"="Plane"}
		LOD 100

		Pass
		{
			//ZWrite On
			Cull Off			
			//ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One OneMinusSrcAlpha
			//Blend [_SrcBlend] [_DstBlend]

			CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag			

#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;			
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _BackgroundColor;
			float _TransparentCutout;


			v2f vert(appdata input)
			{
				v2f output;
				
				float scaleX = length(mul(unity_ObjectToWorld, float4(1.0, 0.0, 0.0, 0.0)));
				float scaleY = length(mul(unity_ObjectToWorld, float4(0.0, 1.0, 0.0, 0.0)));
				output.vertex = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					+ float4(input.vertex.x * scaleX, input.vertex.y * scaleY, 0, 0));

				output.uv = input.uv;
				return output;
			}

			float4 frag(v2f input) : COLOR
			{
				return tex2D(_MainTex, float2(_MainTex_ST.xy * input.uv.xy + _MainTex_ST.zw)) * _Color;				
			}
			ENDCG
		}
	}
}
