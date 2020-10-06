Shader "Custom/TestShader"
{
    Properties
    {
        _Color("Main Color",Color) = (1,1,1,1)
        _BoostColor("Boost Color",Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
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

            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float4(v.uv.xy,0,0);

                return o;
            }

            float nrand(float2 uv,float seed)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * seed);
            }

            fixed4 _Color;
            fixed4 _BoostColor;
            float _random;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = _Color;
                // if (nrand(i.vertex,_random) > .8 - (i.uv.x * i.uv.x * i.uv.x))
                //     return float4(0, 0, 0, 0);

                float distY = abs(i.uv.y - .5) * 2;

                if(distY < .5)
                {
                    //if (nrand(i.vertex,_random) > .5 - (i.uv.x * i.uv.x * i.uv.x))
                    if(i.uv.x < .15)
                    {
                        col = _BoostColor;
                        col.w = 1 - i.uv.x / .2;
                        return col;
                    }

                    return float4(0, 0, 0, 0);
                }

                if(i.uv.x > 0.4)
                {
                    col.w = 1 - ((i.uv.x - 0.4) / 0.6);
                }

                return col;
            }

            ENDCG

        }
    }
}