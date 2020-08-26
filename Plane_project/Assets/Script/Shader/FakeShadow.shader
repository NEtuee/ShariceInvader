Shader "Custom/FakeShadow"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
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
        float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
        float2 texcoord : TEXCOORD0;
	};

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
        OUT.texcoord = IN.texcoord;

		return OUT;
	}

	sampler2D _MainTex;

    uniform fixed4 _Color;


	fixed4 frag(v2f IN) : SV_Target
	{
		fixed4 c = _Color;
        
        // if(IN.texcoord.x < 0.05)
        // {
        //     c.a *= (1 - (IN.texcoord.x / 0.05)) * 2;
        // }

        // c.a *= (1 - IN.texcoord.x) * 2;


		return c;
	}
		ENDCG
	}
	}
}