Shader "Custom/FogPlane"
{
	Properties
	{		
		_Color("Color", COLOR) = (1,1,1,1)
		_EdgeBlend("Edge Blend", float) = 0.0		
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent-10" }
		LOD 100

		Pass
		{			
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;				
				float4 projPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;			
			float _EdgeBlend;
			uniform sampler2D _CameraDepthTexture;


			v2f vert(appdata_base v)
			{
				v2f o;		
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);			

				o.projPos = ComputeScreenPos(o.vertex);
				return o;
			}

			#define DEBUG_VAL(x) return fixed4(x, x, x, 1);

			fixed4 frag(v2f i) : SV_Target
			{
				half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
				depth = LinearEyeDepth(depth);
				half edge = 1 - exp( - _EdgeBlend * (depth - i.projPos.w));

				fixed4 col = _Color;				
				col.a = clamp(edge, 0, 1) * col.a;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}