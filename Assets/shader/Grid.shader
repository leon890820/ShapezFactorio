Shader "Grid" {
		Properties{
		  _Color("Color Tint", Color) = (1, 1, 1, 1)

		}
		SubShader{
			Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }
			GrabPass{ "GrabPassTexture" }
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"

			fixed4 _Color;


			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				return o;
			}


			float3 mix(float x, float y, float t){
				return x * (1.0 - t) + y * t;
			}


			float3 drawGrid(float2 pixelCoords, float3 colour, float3 lineColour, float cellSpacing, float lineWidth, float pixelSize) {
			    float2 cellPosition = abs(frac(pixelCoords / float2(cellSpacing,cellSpacing)) - 0.5);
			    float distToEdge = (0.5 - max(cellPosition.x, cellPosition.y)) * cellSpacing;
			    //float lines = smoothstep(lineWidth - pixelSize, lineWidth, distToEdge);

				
			    colour = distToEdge < lineWidth ? lineColour : colour;

			    return colour;
			}

			float3 drawGraphBackground(float2 pixelCoords, float scale){

				float pixelSize = 1.0 / scale;
				float2 cellPosition = floor(pixelCoords / float2(1.0,1.0));
				float2 cellID = float2(floor(cellPosition.x), floor(cellPosition.y));
				float cell = fmod(cellID.x + cellID.y, 2.0);
				float2 checkerboard = float3(cell,cell,cell);
				float3 colour = float3(1.0,1.0,1.0);
				colour = mix(colour, checkerboard, 0.05);

				colour = drawGrid(pixelCoords, colour, float3(0.5,0.5,0.5), 1.0, 0.03, pixelSize);

				return colour;
			}

			float4 frag(v2f i) : SV_Target{

				float3 bg = drawGraphBackground(i.worldPos.xz, 1.0);

				return float4(bg,1.0);
				
			}

		ENDCG
		}
	}
			
}