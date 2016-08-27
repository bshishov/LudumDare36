Shader "Custom/RigidBodyMaterial" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bump map", 2D) = "bump" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Lighting On
		LOD 200
		
		CGPROGRAM
#pragma surface surf Lambert//ToonRamp
		sampler2D _Ramp;

		/*

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
			inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
		{
#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
#endif
			half d = (dot(s.Normal, lightDir)*0.5 + 0.5) * atten;
			half3 ramp = tex2D(_Ramp, float2(-d,d)).rgb;

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			return c;
		}
		*/
		

#pragma target 3.0


		
		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		float4 _RimColor;
		float _RimPower;
		float4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = c.a;

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	//FallBack "Diffuse"
	Fallback "VertexLit"
}
