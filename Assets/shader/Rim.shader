Shader "edge_rim_ver5" {
		Properties{
		  _Color("Color Tint", Color) = (1, 1, 1, 1)
		  _RimColor("RimColor", Color) = (1, 1, 1, 1)
		  _EdgeScale("Edge Scale", Float) = 1.0
		  _RimScale("Rim Scale", Float) = 1.0
		  _des("des", Range(0, 1)) = 1
		}
		SubShader{
			Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

			GrabPass{ "GrabPassTexture" }
			Pass{
				Tags{ "LightMode" = "ForwardBase" }
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "Lighting.cginc"

				fixed4 _Color;
				sampler2D GrabPassTexture;
				float4 _MainTex_ST;
				fixed4  _RimColor;
				float _EdgeScale;
				float _RimScale;
				float _des;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float3 worldNormal : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
					float4 scrPos : TEXCOORD02;
				};

				v2f vert(a2v v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.scrPos = ComputeGrabScreenPos(o.pos);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target{

					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
					fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

					//Ãä½t¥ú­pºâ
					half rim = 1.0 - saturate(dot(normalize(viewDir), i.worldNormal));
					fixed3 emission = _RimColor.rgb * pow(rim, _EdgeScale);

					fixed3 grabpass_color = tex2D(GrabPassTexture, i.scrPos.xy / i.scrPos.w).rgb;

					return fixed4(grabpass_color + emission * _RimScale * _des, 1.0);
					}

				ENDCG
			}
		}
		FallBack "Specular"	
}