using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace YVR.Enterprise.Camera.Samples
{
    [BurstCompile]
    public static class ImageConversionLibrary
    {
        [BurstCompile]
        public static void ConvertNV21ToYuv420P(in NativeArray<byte> nv21Data, ref NativeArray<byte> yuv420PData,
                                                int width, int height)
        {
            int ySize = width * height;
            int uvSize = ySize / 4;

            for (int i = 0; i < ySize; i++)
                yuv420PData[i] = nv21Data[i];

            int srcOffset = ySize;
            int uOffset = ySize;
            int vOffset = ySize + uvSize;

            for (int i = 0; i < uvSize; i++)
            {
                int srcIndexV = srcOffset + (i << 1);
                int srcIndexU = srcOffset + (i << 1) + 1;
                yuv420PData[vOffset + i] = nv21Data[srcIndexV]; // V
                yuv420PData[uOffset + i] = nv21Data[srcIndexU]; // U
            }
        }

        public static Texture2D LoadNV21Image(byte[] nv21Data, int width, int height)
        {
            using var nv21Native = new NativeArray<byte>(nv21Data, Allocator.TempJob);
            using var rgbData = new NativeArray<byte>(width * height * 3, Allocator.TempJob);

            var job = new NV21ToRGBJob
            {
                nv21Data = nv21Native,
                rgbData = rgbData,
                width = width,
                height = height
            };
            JobHandle handle = job.Schedule(width * height, 256);
            handle.Complete();

            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.LoadRawTextureData(rgbData);
            texture.Apply();

            return texture;
        }
    }
}