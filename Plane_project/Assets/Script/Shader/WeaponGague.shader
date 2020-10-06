Shader "Custom/WeaponGague"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 1
		_Progress("Progress", Range(0.0,1.0)) = 0.0
        _TopLayerSize("TopLayerSize", Range(0.0,1.0)) = 0.0
        _TopLayerColor("TopLayerColor",Color) = (1,1,1,1)
        _RestColor("RestColor",Color) = (1,1,1,1)
		[MaterialToggle] _Inverse("Inverse", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
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

	uniform float _Progress;
    uniform float _TopLayerSize;
	uniform float _Inverse;
    uniform fixed4 _TopLayerColor;
    uniform fixed4 _RestColor;

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;

	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		if (_AlphaSplitEnabled)
			color.a = tex2D(_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
		if(_Inverse == 0)
		{
			if((_Progress - IN.texcoord.x) < _TopLayerSize)
        	{
        	    c.rgb = _TopLayerColor.rgb;
        	}
        	else
        	{
        	    c *= _RestColor;
        	    c.rgb *= c.a;
        	}

			c.a *= IN.texcoord.x < _Progress;
		}
		else
		{
			if((1 - IN.texcoord.x) < _TopLayerSize)
        	{
        	    c.rgb = _TopLayerColor.rgb;
        	}
        	else
        	{
        	    c.rgb = _RestColor.rgb * c.a;
        	    c.a = _RestColor.a * c.a;
        	}

			c.a *= IN.texcoord.x > _Progress;
		}
		
		
		return c;;
	}
		ENDCG
	}
	}
}