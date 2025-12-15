Shader "Custom/InfiniteGridTransparentOffsetAA"
{
    Properties
    {
        _LineColor("Line Color", Color) = (1,1,1,1)
        _GridSize("Grid Size", Float) = 1
        _LineWidth("Line Width", Float) = 0.02
        _Offset("Grid Offset (XYZ)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _LineColor;
            float  _GridSize;
            float  _LineWidth;
            float4 _Offset;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xyz + _Offset.xyz;
                o.pos = mul(UNITY_MATRIX_VP, world);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // grid coordinate in cell units
                float2 coord = i.worldPos.xz / _GridSize;

                // distance to nearest grid center line in each axis (0..0.5)
                float2 cell = abs(frac(coord) - 0.5);

                // distance to closest line (either X or Z line)
                float distToLine = min(cell.x, cell.y);

                // screen-space derivative for anti-aliasing
                float aa = fwidth(distToLine) * 1.5;

                // anti-aliased line mask
                float lineMask = 1.0 - smoothstep(_LineWidth - aa, _LineWidth + aa, distToLine);
                lineMask = saturate(lineMask);

                // optional: fade out very dense grid far away
                // float density = max(fwidth(coord.x), fwidth(coord.y));
                // float densityFade = saturate(1.0 / (density * 2.0));
                // lineMask *= densityFade;

                return float4(_LineColor.rgb, lineMask * _LineColor.a);
            }
            ENDCG
        }
    }
}