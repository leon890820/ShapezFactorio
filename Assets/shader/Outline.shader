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

			//�Ĥ@?pass�A�U??�u�k?�V�~�첾���w�Z�áA�u?�X�y?��??��
			Pass
			{
				//�簣�����A�u��V�I���A����y?pass�O���`��Vpass���ҫ���e
				Cull Front
				//�`�װ����ާ@�A????��?�ȶV�j�A�`��????pass��V������??�o������j���`�׭�
				//�Y�Z�ì����?�A��e���Q���`��V��pass��?�A����y?pass�O���`��Vpass���ҫ���e
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
				//�b���^��?�U�A�C???�u�k?�첾�A?���y??�y����j?�p���z???
				//v.vertex.xyz += v.normal * _OutlineLength;
				o.pos = UnityObjectToClipPos(v.vertex);
				//?�k?��V??��?��?�A?���U???���v��?����?
				float3 normalView = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				//??��?�k?xy��???���v��?�Az�`�פ�??����]�O?�q�קK�����_?������??�첾
				//����y?pass�O���`��Vpass���ҫ���e
				float2 offset = TransformViewToProjection(normalView.xy);
				//��?�b��v��??��??�u�k?�첾�ާ@
				o.pos.xy += offset * _OutlineLength;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//??Pass����?�X�y??��
				return _OutlineColor;
			}

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

			//�ĤG?pass�Q��Blinn-Phong?��ҫ����`��V
			Pass
			{
				CGPROGRAM

				#include "Lighting.cginc"

				fixed4 _Diffuse;
				sampler2D _MainTex;
				//�ϥΤFTRANSFROM_TEX���N�ݭn�w?XXX_ST
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
					//�q?TRANSFORM_TEX?��?�z��?�A�D�n?�z�FOffset�MTiling����?,�q??���P�_o.uv = v.texcoord.xy;
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
