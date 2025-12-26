Shader "Celestial/Earth"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(8,256)) = 64
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            fixed4 _SpecularColor;
            float  _Gloss;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);   // directional light
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Diffuse (Lambert)
                float diff = saturate(dot(N, L));
                fixed3 diffuse = _Color.rgb * _LightColor0.rgb * diff;

                // Blinn-Phong Specular
                float3 H = normalize(L + V);
                float spec = pow(saturate(dot(N, H)), _Gloss);
                fixed3 specular = _SpecularColor.rgb * _LightColor0.rgb * spec;

                fixed3 color = diffuse + specular;
                return fixed4(color, _Color.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    }