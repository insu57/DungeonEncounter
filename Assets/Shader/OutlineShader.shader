Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color",Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width",Range(0.0,0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            
            Blend SrcAlpha OneMinusSrcAlpha
            Offset 5, 5
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            fixed4 _OutlineColor;
            float _OutlineWidth;
            
            v2f vert (appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }

        Pass
        {
            Name "OutlineCombine"
            Cull Back
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include  "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}

