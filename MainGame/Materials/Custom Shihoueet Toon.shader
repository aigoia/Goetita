Shader "Custom/Silhouette Toon"
{
      Properties
          {
              _LightingRamp ("Lighting Ramp", 2D) = "white" {}
              _Color ("Color", Color) = (0.7058823,0.7058823,0.7058823,1)
              _Diffuse ("Diffuse", 2D) = "white" {}
              [HideInInspector]_CutOut ("CutOut", Range(0, 1)) = 0
              _NormalMap ("Normal Map", 2D) = "bump" {}
              _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.6
              _Gloss ("Gloss", Range(0, 1)) = 0.5
              _GlossIntensity ("Gloss Intensity", Range(0, 1)) = 1
              _OcclussionMap ("Occlussion Map", 2D) = "white" {}
              _OcclussionIntensity ("Occlussion Intensity", Range(0, 1)) = 0
              _EmissionMap ("Emission Map", 2D) = "white" {}
              [HDR]_HDREmissionColor ("HDR Emission Color", Color) = (0,0,0,1)
          
              _SilhouetteColor ("Silhouette Color", Color) = (1, 0, 0, 0.5)
      
              [Space]
//              _SpaceColor ("Color", Color) = (1,1,1,1)
              _MainTex ("Albedo (RGB)", 2D) = "white" {}
          }
          SubShader
          {
//              Tags { "RenderType"="Opaque" }
              
              /****************************************************************
              *                            Pass 1
              *****************************************************************
              * - 메인 패스
              * - 스텐실 버퍼에 Ref 2 기록
              *****************************************************************/
              ZWrite On
      
              Stencil
              {
                  Ref 2
                  Pass Replace // Stencil, Z Test 모두 성공한 부분에 2 기록
              }
              
              Name "FORWARD"
              Tags {
                  "LightMode"="ForwardBase"
              }
              
           Pass {   
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #define UNITY_PASS_FORWARDBASE
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "Lighting.cginc"
                #pragma multi_compile_fwdbase_fullshadows
                #pragma only_renderers d3d9 d3d11 glcore gles 
                #pragma target 3.0
                uniform float4 _Color;
                uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
                uniform float _Gloss;
                uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
                uniform float _NormalIntensity;
                uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
                uniform float4 _HDREmissionColor;
                uniform float _GlossIntensity;
                uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
                uniform float _OcclussionIntensity;
                uniform sampler2D _LightingRamp; uniform float4 _LightingRamp_ST;
                struct VertexInput {
                  float4 vertex : POSITION;
                  float3 normal : NORMAL;
                  float4 tangent : TANGENT;
                  float2 texcoord0 : TEXCOORD0;
                };
                struct VertexOutput {
                  float4 pos : SV_POSITION;
                  float2 uv0 : TEXCOORD0;
                  float4 posWorld : TEXCOORD1;
                  float3 normalDir : TEXCOORD2;
                  float3 tangentDir : TEXCOORD3;
                  float3 bitangentDir : TEXCOORD4;
                  LIGHTING_COORDS(5,6)
                };
                VertexOutput vert (VertexInput v) {
                  VertexOutput o = (VertexOutput)0;
                  o.uv0 = v.texcoord0;
                  o.normalDir = UnityObjectToWorldNormal(v.normal);
                  o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                  o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                  o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                  float3 lightColor = _LightColor0.rgb;
                  o.pos = UnityObjectToClipPos( v.vertex );
                  TRANSFER_VERTEX_TO_FRAGMENT(o)
                  return o;
                }
                float4 frag(VertexOutput i) : COLOR {
                  i.normalDir = normalize(i.normalDir);
                  float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                  float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                  float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                  float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                  float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                  float3 lightColor = _LightColor0.rgb;
                ////// Lighting:
                  float attenuation = LIGHT_ATTENUATION(i);
                ////// Emissive:
                  float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                  float node_6355 = max(0,dot(lightDirection,normalDirection));
                  float2 node_8043 = float2(node_6355,0.0);
                  float4 _LightingRamp_var = tex2D(_LightingRamp,TRANSFORM_TEX(node_8043, _LightingRamp));
                  float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                  float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                  float3 node_1525 = ((1.0*((_LightingRamp_var.rgb*node_3674)+saturate((smoothstep( 0.0, 0.025, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity))))+0.0);
                  float node_9675 = 1.0;
                  float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                  float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                  float3 emissive = ((_EmissionMap_var.rgb*_HDREmissionColor.rgb)+(node_1525*node_7893*UNITY_LIGHTMODEL_AMBIENT.rgb));
                  float3 finalColor = emissive + (node_1525*node_7893*(_LightColor0.rgb*attenuation));
                  return fixed4(finalColor,1);
                }
                ENDCG
             }
             
             Pass{
              Name "FORWARD_DELTA"
                  Tags {
                      "LightMode"="ForwardAdd"
                  }
                  Blend One One
                  
                  
                  CGPROGRAM
                  #pragma vertex vert
                  #pragma fragment frag
                  #define UNITY_PASS_FORWARDADD
                  #include "UnityCG.cginc"
                  #include "AutoLight.cginc"
                  #include "Lighting.cginc"
                  #pragma multi_compile_fwdadd_fullshadows
                  #pragma only_renderers d3d9 d3d11 glcore gles 
                  #pragma target 3.0
                  uniform float4 _Color;
                  uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
                  uniform float _Gloss;
                  uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
                  uniform float _NormalIntensity;
                  uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
                  uniform float4 _HDREmissionColor;
                  uniform float _GlossIntensity;
                  uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
                  uniform float _OcclussionIntensity;
                  uniform sampler2D _LightingRamp; uniform float4 _LightingRamp_ST;
                  struct VertexInput {
                      float4 vertex : POSITION;
                      float3 normal : NORMAL;
                      float4 tangent : TANGENT;
                      float2 texcoord0 : TEXCOORD0;
                  };
                  struct VertexOutput {
                      float4 pos : SV_POSITION;
                      float2 uv0 : TEXCOORD0;
                      float4 posWorld : TEXCOORD1;
                      float3 normalDir : TEXCOORD2;
                      float3 tangentDir : TEXCOORD3;
                      float3 bitangentDir : TEXCOORD4;
                      LIGHTING_COORDS(5,6)
                  };
                  VertexOutput vert (VertexInput v) {
                      VertexOutput o = (VertexOutput)0;
                      o.uv0 = v.texcoord0;
                      o.normalDir = UnityObjectToWorldNormal(v.normal);
                      o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                      o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                      o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                      float3 lightColor = _LightColor0.rgb;
                      o.pos = UnityObjectToClipPos( v.vertex );
                      TRANSFER_VERTEX_TO_FRAGMENT(o)
                      return o;
                  }
                  float4 frag(VertexOutput i) : COLOR {
                      i.normalDir = normalize(i.normalDir);
                      float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                      float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                      float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                      float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                      float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                      float3 lightColor = _LightColor0.rgb;
      ////// Lighting:
                      float attenuation = LIGHT_ATTENUATION(i);
                      float node_6355 = max(0,dot(lightDirection,normalDirection));
                      float2 node_8043 = float2(node_6355,0.0);
                      float4 _LightingRamp_var = tex2D(_LightingRamp,TRANSFORM_TEX(node_8043, _LightingRamp));
                      float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                      float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                      float3 node_1525 = ((1.0*((_LightingRamp_var.rgb*node_3674)+saturate((smoothstep( 0.0, 0.025, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity))))+0.0);
                      float node_9675 = 1.0;
                      float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                      float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                      float3 finalColor = (node_1525*node_7893*(_LightColor0.rgb*attenuation));
                      return fixed4(finalColor * 1,0);
                  }
                  ENDCG
              }
      
//              CGPROGRAM
//              #pragma surface surf Lambert
//              #pragma target 3.0
//              
//              fixed4 _Color;
//              sampler2D _MainTex;
//      
//              struct Input
//              {
//                  float2 uv_MainTex;
//              };
//      
//              void surf (Input IN, inout SurfaceOutput o)
//              {
//                  fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//                  o.Albedo = c.rgb;
//                  o.Alpha = c.a;
//              }
//              ENDCG
      
              /****************************************************************
              *                            Pass 2
              *****************************************************************
              * - Zwrite off
              * - ZTest Greater : 다른 물체에 가려진 부분에 단색 실루엣 렌더링
              * - Stencil NotEqual : 다른 실루엣이 그려진 부분에 덮어쓰지 않기
              *****************************************************************/
              ZWrite Off
              ZTest Greater // 가려진 부분에 항상 그린다
      
              Stencil
              {
                  Ref 2
                  Comp NotEqual // 패스 1에서 렌더링 성공한 부분에는 그리지 않도록 한다
              }
               
              
              CGPROGRAM
              #pragma surface surf nolight alpha:fade noforwardadd nolightmap noambient novertexlights noshadow
              
              struct Input { float4 color:COLOR; };
              float4 _SilhouetteColor;
              
              void surf (Input IN, inout SurfaceOutput o)
              {
                  o.Emission = _SilhouetteColor.rgb;
                  o.Alpha = _SilhouetteColor.a;
              }
              float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
              {
                  return float4(s.Emission, s.Alpha);
              }
              ENDCG
              
          }
          FallBack "Diffuse"
          CustomEditor "ShaderForgeMaterialInspector"
}
