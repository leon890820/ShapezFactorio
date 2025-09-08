Shader "Hidden/Ocean"
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float3 colA;
            float3 colB;
            float depthMutiplier;
            float alphaMutiplier;

            float2 raySphereDst(float3 sphereCenter, float sphereRadius, float3 rayOrigin, float3 rayDir){
                float3 offset = rayOrigin - sphereCenter;
                float a = dot(rayDir, rayDir); // 可省略（若 rayDir 已經 normalize 為 1）
                float b = 2.0 * dot(offset, rayDir);
                float c = dot(offset, offset) - sphereRadius * sphereRadius;

                float discriminant = b * b - 4.0 * a * c;

                if (discriminant < 0)
                {
                    // Ray missed the sphere
                    return float2(0, 0);
                }

                float sqrtD = sqrt(discriminant);
                float t0 = (-b - sqrtD) / (2.0 * a);
                float t1 = (-b + sqrtD) / (2.0 * a);

                // 距離要排序，保證 t0 是前方相交點
                float entry = min(t0, t1);
                float exit  = max(t0, t1);

                float dstToSphere = max(0, entry);
                float dstInsideSphere = max(0, exit - dstToSphere);

                return float2(dstToSphere, dstInsideSphere);
            }

            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
                // Adapted from: http://jcgt.org/published/0007/03/04/
                float3 t0 = (boundsMin - rayOrigin) * invRaydir;
                float3 t1 = (boundsMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);
                
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));


                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }

            float3 calcOceanColor(float3 originCol ,float oceanDepth){
                float opticalDepth01 = 1 - exp(-oceanDepth * depthMutiplier);
                float alpha = 1 - exp(-oceanDepth * alphaMutiplier);
                float3 oceanCol = lerp(colA, colB, opticalDepth01);

                return lerp(originCol, oceanCol , alpha);    
            }



            float4 frag (v2f i) : SV_Target
            {

                // just invert the colors
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * viewLength;

                float2 sphereInfo = raySphereDst(float3(0,0,0), 1, rayPos, rayDir);
                
                float4 col = tex2D(_MainTex, i.uv);

                if(sphereInfo.y > 0 && depth > sphereInfo.x){
                    float3 oceanColor = calcOceanColor(col.rgb , depth - sphereInfo.x);
                    return float4(oceanColor,1.0);    
                }
                
                return col;
            }
            ENDCG
        }
    }
}
