Shader "Custom/Floor"
{
    Properties
    {
        _Diffuse("Diffuse Color", Color) = (1, 1, 1, 1)
        _Specular("Specular Color", Color) = (1, 1, 1, 1)
        _Gloss("Gloss", Range(8.0, 256.0)) = 32.0
        _MainTex("Albedo (RGB)", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "LightMode" = "ForwardBase" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                // 需要將物件的頂點由物件空間轉至世界空間
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float4 _MainTex_ST;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 ambient = tex2D(_MainTex , i.uv).rgb * UNITY_LIGHTMODEL_AMBIENT;
                //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT;

                fixed3 worldNormalDir = normalize(i.normal);

                fixed3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);

                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormalDir, worldLightDir));


                
                // ---Blinn-Phong的作法結束---

                // ---Phong的作法---
                // fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormalDir));
                //
                // fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                //
                // fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);        
                // 
                // ---Phong的作法結束---

                return fixed4(ambient + diffuse , 1.0);
            }
            ENDCG
        }
    }

        Fallback "Specular"
}