using System;
using System.Buffers;
using System.Runtime.InteropServices;
using UnityEngine;
using YVR.Core;
using YVR.Enterprise.Camera;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class VSTControl : MonoBehaviour
    {
        public static ArrayPool<byte> arrayPool;
        
        private Texture[] m_Image = new Texture[2];
        
        private VSTCameraFrequencyType m_FrequencyType = VSTCameraFrequencyType.VSTFrequency8Hz;
        private VSTCameraResolutionType m_ResolutionType = VSTCameraResolutionType.VSTResolution660_616;
        private VSTCameraFormatType m_FormatType = VSTCameraFormatType.VSTCameraFmtNv21;
        private VSTCameraSourceType m_SourceType = VSTCameraSourceType.VSTCameraBothEyes;

        private void Awake()
        {
            YVRManager.instance.hmdManager.SetPassthrough(true);
            arrayPool = ArrayPool<byte>.Shared;
            SetVSTCameraFrequency();
            SetVSTCameraResolution();
            SetVSTCameraFormat();
            SetVSTCameraOutputSource();
        }
        public void SetVSTCameraFrequency() { YVRVSTCameraPlugin.SetVSTCameraFrequency(m_FrequencyType); }
        private void SetVSTCameraResolution() { YVRVSTCameraPlugin.SetVSTCameraResolution(m_ResolutionType); }

        private void SetVSTCameraFormat() { YVRVSTCameraPlugin.SetVSTCameraFormat(m_FormatType); }
        private void SetVSTCameraOutputSource()
        {
            Debug.Log($"sss set source is {m_SourceType}");
            YVRVSTCameraPlugin.SetVSTCameraOutputSource(m_SourceType);
        }
        
        
        public void OpenVSTCamera() { YVRVSTCameraPlugin.OpenVSTCamera(); }
        
        public void CloseVSTCamera() { YVRVSTCameraPlugin.CloseVSTCamera(); }
        
        public Texture[] AcquireVSTCameraFrame()
        {
            byte[][] frameBytes = new byte[2][];
            
            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);
            for (int i = 0; i < frameData.cameraFrameItem.data.Length; i++)
            {
                if (frameData.cameraFrameItem.data[i] == IntPtr.Zero) continue;
                byte[] data = arrayPool.Rent(frameData.cameraFrameItem.dataSize);
                Marshal.Copy(frameData.cameraFrameItem.data[i], data, 0, frameData.cameraFrameItem.dataSize);
                frameBytes[i] = data;
            }

            if (frameData.cameraFrameItem.data[0] != IntPtr.Zero)
            {
                if (m_Image[0] != null) Destroy(m_Image[0]);

                Texture2D texture2DLeft = LoadNV21Image(frameBytes[0], frameData.cameraFrameItem.width,
                                                        frameData.cameraFrameItem.height);
                m_Image[0] = texture2DLeft;
            }

            if (frameData.cameraFrameItem.data[1] != IntPtr.Zero)
            {
                if (m_Image[1] != null) Destroy(m_Image[1]);

                Texture2D texture2DRight = LoadNV21Image(frameBytes[1], frameData.cameraFrameItem.width,
                                                         frameData.cameraFrameItem.height);
                m_Image[1] = texture2DRight;
            }
            arrayPool.Return(frameBytes[0]);
            arrayPool.Return(frameBytes[1]);
            
            return m_Image;
        }
        private Texture2D LoadNV21Image(byte[] nv21Data, int width, int height)
        {
            byte[] rgbData = null;
            ConvertNV21ToRGB(ref rgbData, nv21Data, width, height);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.LoadRawTextureData(rgbData);
            texture.Apply();
            return texture;
        }

        private static void ConvertNV21ToRGB(ref byte[] rgbData, byte[] nv21Data, int width, int height)
        {
            int frameSize = width * height;

            if (rgbData == null || frameSize != rgbData.Length)
            {
                rgbData = new byte[frameSize * 3];
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int yIndex = y * width + x;
                    int uvIndex = frameSize + (y / 2) * width + (x & ~1);

                    int yData = nv21Data[yIndex] & 0xff;
                    int vData = nv21Data[uvIndex] & 0xff;
                    int uData = nv21Data[uvIndex + 1] & 0xff;

                    yData = yData < 16 ? 16 : yData;

                    int r = (int) (1.164f * (yData - 16) + 1.596f * (vData - 128));
                    int g = (int) (1.164f * (yData - 16) - 0.813f * (vData - 128) - 0.391f * (uData - 128));
                    int b = (int) (1.164f * (yData - 16) + 2.018f * (uData - 128));

                    r = r < 0 ? 0 : (r > 255 ? 255 : r);
                    g = g < 0 ? 0 : (g > 255 ? 255 : g);
                    b = b < 0 ? 0 : (b > 255 ? 255 : b);

                    int rgbIndex = yIndex * 3;
                    rgbData[rgbIndex] = (byte) r;
                    rgbData[rgbIndex + 1] = (byte) g;
                    rgbData[rgbIndex + 2] = (byte) b;
                }
            }
        }
    }
}