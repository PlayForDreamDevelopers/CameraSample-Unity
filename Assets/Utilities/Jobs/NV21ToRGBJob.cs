using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace YVR.Enterprise.Camera.Samples.Utilities
{
    [BurstCompile]
    public struct NV21ToRGBJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> nv21Data;
        [WriteOnly] public NativeArray<byte> rgbData;
        public int width;
        public int height;

        public void Execute(int index)
        {
            int frameSize = width * height;
            int y = index / width;
            int x = index % width;
            int yIndex = y * width + x;

            int uvY = y >> 1;
            int uvX = x & ~1;
            int uvStride = (width + 1) & ~1;
            int uvIndex = frameSize + uvY * uvStride + uvX;

            float yVal = math.clamp(nv21Data[yIndex], 16, 235) - 16f;
            float vVal = nv21Data[uvIndex] - 128f;
            float uVal = nv21Data[uvIndex + 1] - 128f;

            float r = 1.164f * yVal + 1.596f * vVal;
            float g = 1.164f * yVal - 0.391f * uVal - 0.813f * vVal;
            float b = 1.164f * yVal + 2.018f * uVal;

            r = math.clamp(r, 0f, 255f);
            g = math.clamp(g, 0f, 255f);
            b = math.clamp(b, 0f, 255f);

            int rgbIndex = yIndex * 3;
            rgbData[rgbIndex] = (byte) r;
            rgbData[rgbIndex + 1] = (byte) g;
            rgbData[rgbIndex + 2] = (byte) b;
        }
    }
}