Shader "Custom/ScreenSoftNoise" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

	}
	SubShader {
		//Tags { "RenderType"="Opaque" }
	
	   //설정을 투명 설정으로 바꿔줘야 한다.
		Tags { "RenderType"="Transparent"  "Queue" = "Overlay" }

		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass{}//카메라 화면을 받아오는 부분

		LOD 200

		CGPROGRAM

		#pragma surface surf nolight noambient

		#pragma target 3.0
		sampler2D _MainTex;
		sampler2D _GrabTexture;

		struct Input 
		{
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float2 screenUV = IN.screenPos.rgb / IN.screenPos.a;

			screenUV = float2(screenUV.r,screenUV.g);
			o.Emission = tex2D(_GrabTexture,screenUV + c.r * 0.01 - 0.005);
			o.Alpha = c.a;
		}

		float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
		{
			return float4(0, 0, 0, 1);
		}
		ENDCG
	}
	//FallBack "Diffuse"
	FallBack "Regacy Shaders/Transparent/Diffuse"

}