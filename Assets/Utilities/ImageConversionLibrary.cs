using System;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace YVR.Enterprise.Camera.Samples
{
    public class ImageConversionLibrary
    {
        [BurstCompile]
        public static void ConvertNV21ToRGB(ref byte[] rgbData, byte[] nv21Data, int width, int height)
        {
            int frameSize = width * height;
    
            if (rgbData == null || rgbData.Length != frameSize * 3)
                rgbData = new byte[frameSize * 3];

            
            const float yScale = 1.164f;
            const float vRCoeff = 1.596f;
            const float uGCoeff = -0.391f;
            const float vGCoeff = -0.813f;
            const float uBCoeff = 2.018f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    
                    int yIndex = mad(y, width, x);
            
                    
                    int uvY = y >> 1; 
                    int uvX = x & ~1; 
                    int uvStride = (width + 1) & ~1; 
                    int uvIndex = frameSize + mad(uvY, uvStride, uvX);

                    
                    int yVal = clamp(nv21Data[yIndex], 16, 235);    
                    int vVal = nv21Data[uvIndex];                   
                    int uVal = nv21Data[uvIndex + 1];           
                    
                    float3 yuv = float3(
                                        yVal - 16,  
                                        uVal - 128, 
                                        vVal - 128  
                                       );

                    
                    float3 rgb = float3(
                                        mad(yScale, yuv.x, vRCoeff * yuv.z),
                                        mad(yScale, yuv.x, mad(uGCoeff, yuv.y, vGCoeff * yuv.z)),
                                        mad(yScale, yuv.x, uBCoeff * yuv.y)
                                       );
                    
                    rgb = clamp(rgb, 0f, 255f);
                    
                    int rgbIndex = yIndex * 3;
                    rgbData[rgbIndex]     = (byte)rgb.x; 
                    rgbData[rgbIndex + 1] = (byte)rgb.y; 
                    rgbData[rgbIndex + 2] = (byte)rgb.z; 
                }
            }
        }
        [BurstCompile]
        public static byte[] ConvertNV21ToYUV420P(byte[] nv21Data, int width, int height)
        {
            int ySize = width * height;
            int uvSize = ySize / 4;
    
            byte[] yuv420p = new byte[ySize + 2 * uvSize]; 
    
            Buffer.BlockCopy(nv21Data, 0, yuv420p, 0, ySize);
    
            int srcOffset = ySize;
            int uOffset = ySize;
            int vOffset = ySize + uvSize; 

            for (int i = 0; i < uvSize; i++)
            {
                int srcIndex = math.mad(2, i, 0); 
                yuv420p[vOffset + i] = nv21Data[srcOffset + srcIndex];
        
                srcIndex = math.mad(2, i, 1);      
                yuv420p[uOffset + i] = nv21Data[srcOffset + srcIndex];
            }

            return yuv420p;
        }
        
        public static Texture2D LoadNV21Image(byte[] nv21Data, int width, int height)
        {
            byte[] rgbData = null;
            ImageConversionLibrary.ConvertNV21ToRGB(ref rgbData, nv21Data, width, height);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.LoadRawTextureData(rgbData);
            texture.Apply();
            return texture;
        }
    }
}

