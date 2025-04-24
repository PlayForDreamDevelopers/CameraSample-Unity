using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace YVR.Enterprise.Camera.Samples.Utilities
{
    [BurstCompile]
    public struct RotateRGB90CWJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> srcRgb;
        [WriteOnly] public NativeArray<byte> dstRgb;
        public int srcWidth;
        public int srcHeight;

        public void Execute(int index)
        {
            int dstWidth = srcHeight;

            int dstX = index % dstWidth;
            int dstY = index / dstWidth;
            
            int mirrorDstX = dstWidth - 1 - dstX;

            int srcX = dstY;
            int srcY = srcHeight - 1 - mirrorDstX;

            int srcIdx = (srcY * srcWidth + srcX) * 3;
            int dstIdx = (dstY * dstWidth + dstX) * 3;

            dstRgb[dstIdx] = srcRgb[srcIdx];
            dstRgb[dstIdx + 1] = srcRgb[srcIdx + 1];
            dstRgb[dstIdx + 2] = srcRgb[srcIdx + 2];
        }
    }
}