using UnityEngine;
using System.IO;

[System.Serializable]
public class UCMRTPParams
{
    public double fx, fy, cx, cy;
    public double[] distCoeffs; // 长度16
}

public class UCMRTPUndistortionLUTGenerator : MonoBehaviour
{
    public int width = 2640;
    public int height = 2464;
    public UCMRTPParams param;

    public string mapXFile = "map_x.bin";
    public string mapYFile = "map_y.bin";

    [ContextMenu("Generate Undistort LUT")]
    public void GenerateLUT()
    {
        float[] mapX = new float[width * height];
        float[] mapY = new float[width * height];

        double fx = param.fx;
        double fy = param.fy;
        double cx = param.cx;
        double cy = param.cy;
        double alpha = param.distCoeffs[0];

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                double xn = (x - cx) / fx;
                double yn = (y - cy) / fy;
                var unDist = new Double2(xn, yn);

                int idx = y * width + x;

                Double2 distorted = SolveDistortUCMRTP(unDist, param);

                double xDist = distorted.x, yDist = distorted.y;
                double w = 1.0;
                double rho2 = xDist * xDist + yDist * yDist + w * w;
                double rho = System.Math.Sqrt(rho2);
                double norm = alpha * rho + (1 - alpha) * w;
                double mx = xDist / norm;
                double my = yDist / norm;

                // 转回原图像素坐标
                double srcX = mx * fx + cx;
                double srcY = my * fy + cy;

                mapX[idx] = (float) srcX;
                mapY[idx] = (float) srcY;
            }
        }

        // 保存为二进制文件
        using (var fs = new FileStream(mapXFile, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
            foreach (var v in mapX)
                bw.Write(v);

        using (var fs = new FileStream(mapYFile, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
            foreach (var v in mapY)
                bw.Write(v);

        Debug.Log($"LUT saved: {mapXFile}, {mapYFile}");
    }

    public struct Double2
    {
        public double x, y;

        public Double2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Double2 operator +(Double2 a, Double2 b) => new(a.x + b.x, a.y + b.y);
        public static Double2 operator -(Double2 a, Double2 b) => new(a.x - b.x, a.y - b.y);
        public double SqrMagnitude() => x * x + y * y;
    }

    private static Double2 DistortUCMRTP(Double2 mxy, UCMRTPParams p)
    {
        double k1 = p.distCoeffs[1];
        double k2 = p.distCoeffs[2];
        double k3 = p.distCoeffs[3];
        double k4 = p.distCoeffs[4];
        double k5 = p.distCoeffs[5];
        double k6 = p.distCoeffs[6];
        double p1 = p.distCoeffs[7];
        double p2 = p.distCoeffs[8];
        double s1 = p.distCoeffs[9];
        double s2 = p.distCoeffs[10];
        double s3 = p.distCoeffs[11];
        double s4 = p.distCoeffs[12];
        double x = mxy.x, y = mxy.y;
        double mx2 = x * x;
        double my2 = y * y;
        double xy = x * y;
        double rho2 = mx2 + my2;
        double rho4 = rho2 * rho2;
        double rho6 = rho4 * rho2;
        double rho8 = rho4 * rho4;
        double radDist = 1.0 + k1 * rho2 + k2 * rho2 * rho2 + k3 * rho6 + k4 * rho4 * rho4 + k5 * rho8 * rho2 +
                         k6 * rho8 * rho4;
        double dU = x * radDist + 2.0 * p1 * xy + p2 * (rho2 + 2.0 * mx2) + s1 * rho2 + s3 * rho4;
        double dV = y * radDist + 2.0 * p2 * xy + p1 * (rho2 + 2.0 * my2) + s2 * rho2 + s4 * rho4;
        return new Double2(dU, dV);
    }

    private static Double2 SolveDistortUCMRTP(Double2 target, UCMRTPParams p)
    {
        Double2 mxy = target;
        for (int i = 0; i < 16; ++i)
        {
            Double2 estimate = DistortUCMRTP(mxy, p);
            var err = new Double2(target.x - estimate.x, target.y - estimate.y);
            mxy = new Double2(mxy.x + err.x, mxy.y + err.y);
            if (err.SqrMagnitude() < 1e-10)
                break;
        }

        return mxy;
    }
}