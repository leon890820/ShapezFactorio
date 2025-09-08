Shader "Custom/Outline"
{
	Properties{
		_MainTex("Texture", 2D) = "white"{}
		_Diffuse("DiffuseColor", Color) = (1,1,1,1)
		_Specular("SpecularColor",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8,256)) = 32
		_OutlineColor("_OutlineColor", Color) = (1,0,0,1)
		_OutlineLength("_OutlineLength", Range(0,1)) = 0.0
	}

		SubShader
		{

			//第一?pass，各??沿法?向外位移指定距离，只?出描?的??色
			Pass
			{
				//剔除正面，只渲染背面，防止描?pass与正常渲染pass的模型交叉
				Cull Front
				//深度偏移操作，????的?值越大，深度????pass渲染的片元??得比原先更大的深度值
				//即距离相机更?，更容易被正常渲染的pass覆?，防止描?pass与正常渲染pass的模型交叉
				Offset 20,20
			//Zwrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			fixed4 _OutlineColor;
			float _OutlineLength;

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				//在物体空?下，每???沿法?位移，?种描??造成近大?小的透???
				//v.vertex.xyz += v.normal * _OutlineLength;
				o.pos = UnityObjectToClipPos(v.vertex);
				//?法?方向??到?空?，?接下???到投影空?做准?
				float3 normalView = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				//??空?法?xy坐???到投影空?，z深度不??的原因是?量避免垂直于?平面的??位移
				//防止描?pass与正常渲染pass的模型交叉
				float2 offset = TransformViewToProjection(normalView.xy);
				//最?在投影空??行??沿法?位移操作
				o.pos.xy += offset * _OutlineLength;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//??Pass直接?出描??色
				return _OutlineColor;
			}

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

			//第二?pass利用Blinn-Phong?色模型正常渲染
			Pass
			{
				CGPROGRAM

				#include "Lighting.cginc"

				fixed4 _Diffuse;
				sampler2D _MainTex;
				//使用了TRANSFROM_TEX宏就需要定?XXX_ST
				float4 _MainTex_ST;
				fixed4 _Specular;
				float _Gloss;

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 worldNormal : TEXCOORD0;
					float2 uv : TEXCOORD1;
					float3 worldPos : TEXCOORD2;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul((float3x3)unity_ObjectToWorld, v.vertex);
					//通?TRANSFORM_TEX?化?理坐?，主要?理了Offset和Tiling的改?,默??等同于o.uv = v.texcoord.xy;
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
					fixed3 halfDir = normalize(viewDir + worldLightDir);
					fixed3 specular = _Specular * pow(saturate(dot(halfDir, worldNormal)), _Gloss);
					fixed3 diffuse = _LightColor0.xyz * _Diffuse * saturate(dot(worldNormal, worldLightDir));
					fixed4 color = tex2D(_MainTex, i.uv);
					color.rgb = color.rgb * diffuse + ambient;
					return color;
				}
				#pragma vertex vert
				#pragma fragment frag	

				ENDCG
			}
		}
			FallBack "Diffuse"
}
