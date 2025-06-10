Shader "Custom/SpriteCircleWave"
{
    Properties
    {
        [HideInInspector][NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Base Radius", Range(0, 1)) = 0.4
        _Amplitude ("Wave Amplitude", Range(0, 0.2)) = 0.05
        _Frequency ("Wave Frequency", Float) = 10.0
        _Speed ("Wave Speed", Float) = 2.0
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            float _Radius;
            float _Amplitude;
            float _Frequency;
            float _Speed;
            float4 _Center;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // przelicz do wspó³rzêdnych biegunowych
                float2 center = _Center.xy;
                float2 delta = uv - center;

                float angle = atan2(delta.y, delta.x); // [-pi, pi]
                float radius = length(delta);

                // oblicz zmienn¹ granicê z fal¹
                float wave = sin(angle * _Frequency + _Time.y * _Speed);
                float distortedRadius = _Radius + wave * _Amplitude;

                // jeœli poza zakresem — przezroczysty
                if (radius > distortedRadius)
                    return float4(0, 0, 0, 0);

                // wewn¹trz — kolor
                fixed4 texCol = tex2D(_MainTex, uv);
                return texCol * _Color;
            }
            ENDCG
        }
    }
}