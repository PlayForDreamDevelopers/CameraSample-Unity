using System;
using System.Buffers;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YVR.Core;
using ZXing;

namespace YVR.Enterprise.Camera.Samples
{
    public class VSTCameraControl : MonoBehaviour
    {
        private static ArrayPool<byte> s_ArrayPool;

        [SerializeField] private RawImage m_LeftImage;
        [SerializeField] private RawImage m_RightImage;
        [SerializeField] private TMP_Dropdown m_FrequencyDropdown;
        [SerializeField] private TMP_Dropdown m_ResolutionDropdown;
        [SerializeField] private TMP_Dropdown m_FormatDropdown;
        [SerializeField] private TMP_Dropdown m_SourceDropdown;
        [SerializeField] private TextMeshProUGUI m_CameraData;
        [SerializeField] private TextMeshProUGUI m_ScanInfo;
        

        private VSTCameraFrequencyType m_FrequencyType;
        private VSTCameraResolutionType m_ResolutionType;
        private VSTCameraFormatType m_FormatType;
        private VSTCameraSourceType m_SourceType;

        private void Start()
        {
            YVRManager.instance.hmdManager.SetPassthrough(true);
            s_ArrayPool = ArrayPool<byte>.Shared;

            m_FrequencyType = (VSTCameraFrequencyType) m_FrequencyDropdown.value;
            SetVSTCameraFrequency();
            m_FrequencyDropdown.onValueChanged.AddListener(value =>
            {
                m_FrequencyType = (VSTCameraFrequencyType) value;
                SetVSTCameraFrequency();
            });

            m_ResolutionType = (VSTCameraResolutionType) m_ResolutionDropdown.value;
            SetVSTCameraResolution();
            m_ResolutionDropdown.onValueChanged.AddListener(value =>
            {
                m_ResolutionType = (VSTCameraResolutionType) value;
                SetVSTCameraResolution();
            });

            m_FormatType = (VSTCameraFormatType) m_FormatDropdown.value;
            SetVSTCameraFormat();
            m_FormatDropdown.onValueChanged.AddListener(value =>
            {
                m_FormatType = (VSTCameraFormatType) value;
                SetVSTCameraFormat();
            });

            m_SourceType = (VSTCameraSourceType) m_SourceDropdown.value;
            SetVSTCameraOutputSource();
            m_SourceDropdown.onValueChanged.AddListener(value =>
            {
                m_SourceType = (VSTCameraSourceType) value;
                SetVSTCameraOutputSource();
            });

            RefreshVSTCameraInfo();
        }

        public void SetVSTCameraFrequency() { YVRVSTCameraPlugin.SetVSTCameraFrequency(m_FrequencyType); }

        public void RefreshVSTCameraInfo()
        {
            VSTCameraFrequencyType freqType = default;
            VSTCameraResolutionType resolution = default;
            VSTCameraFormatType formatType = default;
            VSTCameraSourceType sourceType = default;
            VSTCameraIntrinsicExtrinsicData data = default;

            YVRVSTCameraPlugin.GetVSTCameraFrequency(ref freqType);
            YVRVSTCameraPlugin.GetVSTCameraResolution(ref resolution);
            YVRVSTCameraPlugin.GetVSTCameraFormat(ref formatType);
            YVRVSTCameraPlugin.GetVSTCameraOutputSource(ref sourceType);
            YVRVSTCameraPlugin.GetVSTCameraIntrinsicExtrinsic(YVREyeNumberType.LeftEye, ref data);

            string text = $"Frequency: {freqType}\n" +
                          $"Resolution: {resolution}\n" +
                          $"Format: {formatType}\n" +
                          $"Source: {sourceType}\n" +
                          $"Intrinsic: fx:{data.fx:f1}, fy:{data.fy:f1} \n" +
                          $"           cx:{data.cx:f1}, cy:{data.cy:f1}\n" +
                          $"Extrinsic: Position(x:{data.x:f1}, y:{data.y:f1}, z:{data.z:f1})\n" +
                          $"Rotation(w:{data.rw:f1}, x:{data.rx:f1}, y:{data.ry:f1}, z:{data.rz:f1})";
            m_CameraData.text = text;
        }

        private void SetVSTCameraResolution() { YVRVSTCameraPlugin.SetVSTCameraResolution(m_ResolutionType); }

        private void SetVSTCameraFormat() { YVRVSTCameraPlugin.SetVSTCameraFormat(m_FormatType); }

        private void SetVSTCameraOutputSource()
        {
            Debug.Log($"sss set source is {m_SourceType}");
            YVRVSTCameraPlugin.SetVSTCameraOutputSource(m_SourceType);
        }

        public void OpenVSTCamera() { YVRVSTCameraPlugin.OpenVSTCamera(); }

        public void CloseVSTCamera() { YVRVSTCameraPlugin.CloseVSTCamera(); }

        public void AcquireVSTCameraFrame()
        {
            byte[][] frameBytes = new byte[2][];

            m_LeftImage.texture = null;
            m_RightImage.texture = null;
            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);
            for (int i = 0; i < frameData.cameraFrameItem.data.Length; i++)
            {
                if (frameData.cameraFrameItem.data[i] == IntPtr.Zero) continue;
                byte[] data = s_ArrayPool.Rent(frameData.cameraFrameItem.dataSize);
                Marshal.Copy(frameData.cameraFrameItem.data[i], data, 0, frameData.cameraFrameItem.dataSize);
                frameBytes[i] = data;
            }

            if (frameData.cameraFrameItem.data[0] != IntPtr.Zero)
            {
                if (m_LeftImage.texture != null) Destroy(m_LeftImage.texture);

                Texture2D texture2DLeft = ImageConversionLibrary.LoadNV21Image(frameBytes[0], frameData.cameraFrameItem.width,
                                                                               frameData.cameraFrameItem.height);
                m_LeftImage.texture = texture2DLeft;
            }

            if (frameData.cameraFrameItem.data[1] != IntPtr.Zero)
            {
                if (m_RightImage.texture != null) Destroy(m_RightImage.texture);

                Texture2D texture2DRight = ImageConversionLibrary.LoadNV21Image(frameBytes[1], frameData.cameraFrameItem.width,
                                                                               frameData.cameraFrameItem.height);
                m_RightImage.texture = texture2DRight;
            }

            s_ArrayPool.Return(frameBytes[0]);
            s_ArrayPool.Return(frameBytes[1]);
        }
    }
}