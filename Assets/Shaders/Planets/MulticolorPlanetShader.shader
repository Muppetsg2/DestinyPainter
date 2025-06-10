Shader "Custom/MulticolorPlanetShader"
{
    Properties
    {
        [HideInInspector][NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
        _SegmentCount ("Segment Count", Int) = 1
    }

    SubShader
    {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
        }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_SEGMENTS 10

            sampler2D _MainTex;
            float4 _Colors[MAX_SEGMENTS];
            float4 _SecondaryColors[MAX_SEGMENTS];
            float _Angles[MAX_SEGMENTS + 1];
            int _SegmentCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 pos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = v.uv * 2.0 - 1.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.pos;
                float r = length(pos);
                float angle = atan2(pos.y, pos.x);
                angle = degrees(angle);
                if (angle < 0) angle += 360;

                float2 texUV = i.uv;
                float4 texColor = tex2D(_MainTex, texUV).rgba;

                for (int idx = 0; idx < _SegmentCount; ++idx)
                {
                    if (angle >= _Angles[idx] && angle < _Angles[idx + 1])
                    {
                        float t = smoothstep(0.0, 1.0, r);
                        float3 color = lerp(_SecondaryColors[idx], _Colors[idx], t);
                        return float4(color * texColor.rgb * texColor.a, texColor.a);
                    }
                }

                return float4(0,0,0,texColor.a);
            }
            ENDCG
        }
    }
}
