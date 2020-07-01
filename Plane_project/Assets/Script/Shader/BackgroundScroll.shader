Shader "Custom/BackgroundScroll"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_MaskValue ("Mask Value", Range(0,1)) = 0.5
		_MaskColor ("Mask Color", Color) = (0,0,0,1)
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
	sampler2D _MaskTex;
	float _MaskValue;

	float4 _MaskColor;
	//float _AlphaSplitEnabled;

	fixed4 SampleSpriteTexture(float2 uv)
	{
        float2 offset = float2(_MainOffsetX,_MainOffsetY);
        float2 mainUV = (uv + offset) * float2(_MainScaleX,_MainScaleY);
        
		fixed4 color = tex2D(_MainTex, mainUV);

// #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
// 		if (_AlphaSplitEnabled)
// 			color.a = tex2D(_AlphaTex, mainUV).r;
// #endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		return color * _Color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

		if(_MaskValue != 0)
		{
			float4 mask = tex2D(_MaskTex, IN.texcoord);
			float alpha = mask.a * (1 - 1/255.0);

			float weight = step(_MaskValue, alpha);

			c.rgb = lerp(c.rgb, lerp(_MaskColor.rgb, c.rgb, weight), _MaskColor.a);
		}

		c.rgb *= c.a;
		return c;
	}
		ENDCG
	}
	}
}