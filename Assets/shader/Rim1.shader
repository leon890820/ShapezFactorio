Shader "edge_rim_ver4" {
	Properties{
	  _Color("Color Tint", Color) = (1, 1, 1, 1)
	  _MainTex("Main Tex", 2D) = "white" {}
	  _rim_control("control", 2D) = "white" {}
	  _RimColor("RimColor", Color) = (1, 1, 1, 1)
	  _EdgeScale("Edge Scale", Float) = 1.0
	  _RimScale("Rim Scale", Float) = 0.0
	  _speed("speed", Float) = 1.0
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _rim_control;
			float4 _MainTex_ST;
			fixed4 _Specular;
			fixed4  _RimColor;
			float _Gloss;
			float _EdgeScale;
			float _RimScale;
			float _speed;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;


				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				//Ãä½t¥ú­pºâ
				half rim = 1.0 - saturate(dot(normalize(viewDir), i.worldNormal));
				fixed3 emission = _RimColor.rgb * pow(rim, _EdgeScale);
				fixed rim_curve = tex2D(_rim_control, fixed2(i.uv.x,i.uv.y + _Time.y * _speed * 0.1)).r;
				emission *= rim_curve;

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));


				return fixed4(ambient + diffuse + emission * _RimScale, 1.0);
			}

			ENDCG
		}
	  }
	  FallBack "Specular"
}