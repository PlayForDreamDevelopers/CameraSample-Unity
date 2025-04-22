using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct FisheyeUndistortJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<byte> srcRgb; // 原始RGB
    [WriteOnly] public NativeArray<byte> dstRgb; // 反畸变后RGB
    [ReadOnly] public NativeArray<float> mapX;
    [ReadOnly] public NativeArray<float> mapY;
    public int width;
    public int height;
    public int channels; // 通道数（通常3）

    public void Execute(int index)
    {
        int x = index % width;
        int y = index / width;
        int idx = y * width + x;

        double srcX = mapX[idx];
        double srcY = mapY[idx];

        int x0 = Mathf.FloorToInt((float) srcX);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt((float) srcY);
        int y1 = y0 + 1;
        float dx = (float) srcX - x0;
        float dy = (float) srcY - y0;

        int dstIdx = idx * channels;
        for (int c = 0; c < channels; ++c)
        {
            float v00 = (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                ? srcRgb[(y0 * width + x0) * channels + c]
                : 0;
            float v10 = (x1 >= 0 && x1 < width && y0 >= 0 && y0 < height)
                ? srcRgb[(y0 * width + x1) * channels + c]
                : 0;
            float v01 = (x0 >= 0 && x0 < width && y1 >= 0 && y1 < height)
                ? srcRgb[(y1 * width + x0) * channels + c]
                : 0;
            float v11 = (x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                ? srcRgb[(y1 * width + x1) * channels + c]
                : 0;
            float v0 = Mathf.Lerp(v00, v10, dx);
            float v1 = Mathf.Lerp(v01, v11, dx);
            float value = Mathf.Lerp(v0, v1, dy);
            dstRgb[dstIdx + c] = (byte) Mathf.Clamp(Mathf.RoundToInt(value), 0, 255);
        }
    }
}