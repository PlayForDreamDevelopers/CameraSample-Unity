using System;
using System.Buffers;
using System.Runtime.InteropServices;
using UnityEngine;
using YVR.Enterprise.Camera.Samples.Utilities;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class VSTCameraControl : MonoBehaviour
    {
        public static ArrayPool<byte> arrayPoolByte;
        public static ArrayPool<int> arrayPoolInt;

        private Texture[] m_Image = new Texture[2];

        private VSTCameraFrequencyType m_FrequencyType = VSTCameraFrequencyType.VSTFrequency8Hz;
        private VSTCameraResolutionType m_ResolutionType = VSTCameraResolutionType.VSTResolution660_616;
        private VSTCameraFormatType m_FormatType = VSTCameraFormatType.VSTCameraFmtNv21;
        private VSTCameraSourceType m_SourceType = VSTCameraSourceType.VSTCameraBothEyes;

        private void Awake()
        {
            arrayPoolByte = ArrayPool<byte>.Shared;
            arrayPoolInt = ArrayPool<int>.Shared;
            SetVSTCameraFrequency();
            SetVSTCameraResolution();
            SetVSTCameraFormat();
            SetVSTCameraOutputSource();
        }

        public void SetVSTCameraFrequency() { YVRVSTCameraPlugin.SetVSTCameraFrequency(m_FrequencyType); }
        private void SetVSTCameraResolution() { YVRVSTCameraPlugin.SetVSTCameraResolution(m_ResolutionType); }

        private void SetVSTCameraFormat() { YVRVSTCameraPlugin.SetVSTCameraFormat(m_FormatType); }

        private void SetVSTCameraOutputSource() { YVRVSTCameraPlugin.SetVSTCameraOutputSource(m_SourceType); }


        public void OpenVSTCamera() { YVRVSTCameraPlugin.OpenVSTCamera(); }

        public void CloseVSTCamera() { YVRVSTCameraPlugin.CloseVSTCamera(); }

        public Texture[] AcquireVSTCameraFrame(ref byte[][] frameBytes, ref int[][] frameInfo)
        {
            if (frameBytes == null)
            {
                frameBytes = new byte[2][];
            }
            else if (frameBytes.Length < 2)
            {
                Array.Resize(ref frameBytes, 2);
            }

            if (frameInfo == null)
            {
                frameInfo = new int[2][];
            }
            else if (frameInfo.Length < 2)
            {
                Array.Resize(ref frameInfo, 2);
            }

            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);
            for (int i = 0; i < frameData.cameraFrameItem.data.Length; i++)
            {
                if (frameData.cameraFrameItem.data[i] == IntPtr.Zero) continue;
                byte[] data = arrayPoolByte.Rent(frameData.cameraFrameItem.dataSize);
                Marshal.Copy(frameData.cameraFrameItem.data[i], data, 0, frameData.cameraFrameItem.dataSize);
                frameBytes[i] = data;

                int[] info = arrayPoolInt.Rent(2);
                info[0] = frameData.cameraFrameItem.width;
                info[1] = frameData.cameraFrameItem.height;
                frameInfo[i] = info;
            }

            if (frameData.cameraFrameItem.data[0] != IntPtr.Zero)
            {
                if (m_Image[0] != null) Destroy(m_Image[0]);

                Texture2D texture2DLeft = ImageConversionLibrary.LoadNV21Image(frameBytes[0],
                                                                               frameData.cameraFrameItem.width,
                                                                               frameData.cameraFrameItem.height);
                m_Image[0] = texture2DLeft;
            }

            if (frameData.cameraFrameItem.data[1] != IntPtr.Zero)
            {
                if (m_Image[1] != null) Destroy(m_Image[1]);

                Texture2D texture2DRight = ImageConversionLibrary.LoadNV21Image(frameBytes[1],
                                                                                frameData.cameraFrameItem.width,
                                                                                frameData.cameraFrameItem.height);
                m_Image[1] = texture2DRight;
            }

            return m_Image;
        }
    }
}