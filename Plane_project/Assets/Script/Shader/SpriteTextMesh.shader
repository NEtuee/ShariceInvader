Shader "Custom/SpriteTextMesh"
{
    Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_MainColor("MainColor",Color) = (1,1,1,1)
        _AlignmentX("AlignementX", float) = 0
        _AlignmentY("AlignementY", float) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
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

    float _AlignmentX;
    float _AlignmentY;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
        float4 ver = IN.vertex;
        ver.x += _AlignmentX;
        ver.y += _AlignmentY;

		OUT.vertex = UnityObjectToClipPos(ver);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;

	float _AlphaSplitEnabled;
	fixed4 _MainColor;

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
		c.rgb *= c.a;
		return c * _MainColor;
	}
		ENDCG
	}
	}
}