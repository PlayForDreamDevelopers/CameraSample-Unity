using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace YVR.Enterprise.Camera.Samples.Utilities
{
    [BurstCompile]
    public static class ImageConversionLibrary
    {
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