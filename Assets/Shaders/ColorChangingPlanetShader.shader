Shader "Custom/ColorChangingPlanetShader"
{
    Properties
    {
        [HideInInspector][NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        _InnerColor ("Inner Color", Color) = (1, 1, 1, 1)
        _OuterColor ("Outer Color", Color) = (0, 0, 0, 1)
        _BlendPoint ("Blend Point (0-1)", Range(0,1)) = 0.5
        _UseInnerColorForBorder ("Use Inner Color For Border", Float) = 1
        _BorderWidth ("Border Width", Range(0.01, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _InnerColor;
            float4 _OuterColor;
            float _BlendPoint;
            float _UseInnerColorForBorder;
            float _BorderWidth;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv;
                float r = length(pos);

                if (r > 1.0 - _BorderWidth)
                {
                    return _UseInnerColorForBorder > 0.5 ? _InnerColor : _OuterColor;
                }

                float t;
                if (_BlendPoint > 0.5)
                {
                    t = smoothstep((_BlendPoint * 2 - 1), 1, r);
                }
                else
                {
                    t = smoothstep(0.0, _BlendPoint * 2, r);
                }
                return lerp(_InnerColor, _OuterColor, t);
            }
            ENDCG
        }
    }
}