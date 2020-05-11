Shader "Unlit/FallOffShader"
{
    Properties
    {
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_Fill ("Fill", Range(0,1)) = 0.0
			_Color("Tint", Color) = (1,1,1,1)
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha  OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _MainTex_ST;
			float _Fill;


            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
            fixed4 col = tex2D(_MainTex, i.texcoord	);
			float falloff = 0.25;
			float fill = lerp(0, 1, _Fill);
			float falloffShift = falloff * (i.texcoord.x * 0.5 + 0.5);

			float alpha = min(1.0, max(0.0, (fill - i.texcoord.x + falloffShift) / falloff));
			float4 black = float4(0.0, 0.0, 0.0, 0.0);

			col *= lerp(black, col, alpha);
            return col;
            }
            ENDCG
        }
    }
}
