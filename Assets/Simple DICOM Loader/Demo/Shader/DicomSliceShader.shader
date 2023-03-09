Shader "Custom/DicomSliceShader" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

	Category{

		Lighting Off
		ZWrite On
		Cull Off

		SubShader{
			Pass{
				SetTexture[_MainTex]{
					constantColor[_Color]
					Combine texture * constant, texture * constant
				}
			}
		}
	}
}
