Shader "Hidden/GalaxyGridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = v.uv;
                // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return output;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float3 mix(float x, float y, float t){
				return x * (1.0 - t) + y * t;
			}

            float3 drawGrid(float2 pixelCoords, float3 colour, float3 lineColour, float cellSpacing, float lineWidth, float pixelSize) {
			    float2 cellPosition = abs(frac(pixelCoords / float2(cellSpacing,cellSpacing)) - 0.5);
			    float distToEdge = (0.5 - max(cellPosition.x, cellPosition.y)) * cellSpacing;
			   
				
			    colour = distToEdge < lineWidth ? lineColour : colour;

			    return colour;
			}

			float3 drawGraphBackground(float2 pixelCoords, float scale, float3 col){

				float pixelSize = 1.0 / scale;

				
				float3 colour = drawGrid(pixelCoords, col, float3(0.5,0.5,0.5), scale, 0.2, pixelSize);

				return colour;
			}




            float4 frag (v2f i) : SV_Target{
                float4 col = tex2D(_MainTex, i.uv);

                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * viewLength;
                
                if(rayDir.y >= 0) return col;

                float t = - rayPos.y / rayDir.y;
                if(t < 0)  return col;
                if(t > depth) return col;

                float3 pos = rayPos + rayDir * t;
                float2 dis = (rayPos - pos).xz;

                if(max(abs(dis.x), abs(dis.y)) > 500) return col;


                float3 gridColor = drawGraphBackground(pos.xz, 20.0, col.rgb);

                return float4(gridColor , 1.0);
            }
            ENDCG
        }
    }
}
