Shader "GruffyShaders/ScrollUV" 
 {
     Properties 
     {
         _MainTint ("Diffuse Tint", Color) = (1,1,1,1)
         //set scroll direction properties  
          _ScrollXSpeed("X Scroll Speed", Range(0,10)) = 2
          _ScrollYSpeed("Y Scroll Speed", Range(0,10)) = 2
         _MainTex ("Base (RGB)", 2D) = "white" {}
         
     }
     SubShader 
     {
         Tags { "RenderType"="Transparent" }
         LOD 200
         
		 Pass {
         CGPROGRAM
         #pragma surface surf Lambert
         fixed4 _MainTint;
         fixed _ScrollXSpeed;
         fixed _ScrollYSpeed;
         sampler2D _MainTex;
 
 
         struct Input 
         {
             float2 uv_MainTex;
 
         };
 
         void surf (Input IN, inout SurfaceOutput o) 
         {
             //create cache to store UV prior to passing  to the texture 2d function
             fixed2 scrolledUV = IN.uv_MainTex;
             //create cache to store eperate x & y components so the uvs can be scaled over time (in built time function)
             fixed xScrollValue = _ScrollXSpeed * _Time;
             fixed yScrollValue = _ScrollYSpeed * _Time;    
             
             //apply final UV offset
             scrolledUV += fixed2(xScrollValue, yScrollValue);
 
             
             //apply the textures and tint
             half4 c = tex2D (_MainTex, scrolledUV);
         
             o.Albedo = c.rgb * _MainTint;
             o.Alpha = c.a;
         }
         ENDCG
		 }
     } 
     FallBack "Diffuse"
 }