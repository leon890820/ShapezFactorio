Shader "Lit/Diffuse With Shadows"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2) // put shadows data into TEXCOORD1
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
            };
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // compute shadows data
                TRANSFER_SHADOW(o)
                return o;
            }

            sampler2D _MainTex;

            float3 drawGrid(float2 pixelCoords,
                float3 colour,
                float3 lineColour,
                float  cellSpacing,
                float  lineWidth)
            {
                // 轉成以 cell 為單位的座標
                float2 coord = pixelCoords / cellSpacing;

                // 每個 cell 中心的距離（0..0.5）
                float2 cellPosition = abs(frac(coord) - 0.5);

                // 距離最近一條線的距離（越小越靠近線）
                float distToLine = (0.5 - max(cellPosition.x, cellPosition.y)) * cellSpacing;

                // 依照螢幕空間算出一個像素對應的距離，當作 AA feather
                float aa = fwidth(distToLine) * 1.5;

                // 把硬切換成平滑過渡
                float lineMask = 1.0 - smoothstep(lineWidth - aa, lineWidth + aa, distToLine);
                lineMask = saturate(lineMask);

                // 用 lineMask 混合線條顏色與背景顏色
                return lerp(colour, lineColour, lineMask);
            }

            float3 drawGraphBackground(float2 pixelCoords, float scale)
            {
                // 底色（你原本的 checkerboard 幾乎看不太到，就先簡化）
                float3 colour = float3(1.0, 1.0, 1.0);

                // 如果你還是想要淡淡的棋盤，可以打開這段
                
                float2 cellPosition = floor(pixelCoords);
                float2 cellID = float2(floor(cellPosition.x), floor(cellPosition.y));
                float cell = fmod(cellID.x + cellID.y, 2.0);
                float3 checkerboard = float3(cell, cell, cell);
                colour = lerp(colour, checkerboard, 0.05);
                

                // 畫格線（lineWidth 視覺上看起來舒服的值自己微調）
                colour = drawGrid(pixelCoords, colour, float3(0.5, 0.5, 0.5), 1.0, 0.03);

                return colour;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 用世界座標或你自己的座標當作 pixelCoords
                float3 bg = drawGraphBackground(i.worldPos.xz, 1.0);

                fixed4 col = fixed4(bg, 1.0);

                // 陰影照舊
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = shadow;
                col.rgb *= lighting;

                return col;
            }
            ENDHLSL
        }

        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}