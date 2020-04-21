Shader "Custom/BackgroundScroll"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
//		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_Color ("Main Color", Color) = (1,1,1,1)
        _MainScaleX("ScreenScaleX",Float) = 1
        _MainScaleY("ScreenScaleY",Float) = 1
        _MainOffsetX("MainOffsetX",Float) = 0
        _MainOffsetY("MainOffsetY",Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"

	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
	};

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;
//#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
//#endif

		return OUT;
	}

	uniform float4 _Color;

    uniform float _MainScaleX;
    uniform float _MainScaleY;
    uniform float _MainOffsetX;
    uniform float _MainOffsetY; 

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;

	fixed4 SampleSpriteTexture(float2 uv)
	{
        float2 offset = float2(_MainOffsetX,_MainOffsetY);
        float2 mainUV = (uv + offset) * float2(_MainScaleX,_MainScaleY);
        
		fixed4 color = tex2D(_MainTex, mainUV);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		if (_AlphaSplitEnabled)
			color.a = tex2D(_AlphaTex, mainUV).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		return color * _Color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
		c.rgb *= c.a;
		return c;
	}
		ENDCG
	}
	}
}