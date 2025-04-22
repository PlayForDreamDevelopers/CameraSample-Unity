using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YVR.Utilities;

namespace YVR.Enterprise.Camera.Samples
{
    public class TrackingCameraControl : MonoBehaviour
    {
        private static ArrayPool<byte> s_ArrayPool;
        private Texture2D m_Texture2DLeft;
        private Texture2D m_Texture2DRight;

        private TrackingCameraType m_CameraType = TrackingCameraType.Max;

        [SerializeField] private Transform m_CameraImageCollection;
        [SerializeField] private GameObject m_CameraImagePrefab;
        [SerializeField] private TMP_Dropdown m_CameraTypeDropdown;

        private Dictionary<TrackingCameraType, GameObject> m_TrackingType2DisplayDict = new();
        private Dictionary<TrackingCameraType, (Texture2D, Texture2D)> m_TrackingType2TexturesDict = new();

        private void Start()
        {
            s_ArrayPool = ArrayPool<byte>.Shared;
            m_CameraType = (TrackingCameraType) m_CameraTypeDropdown.value;
            m_CameraTypeDropdown.onValueChanged.AddListener(value => m_CameraType = (TrackingCameraType) value);
        }

        public void OpenTrackingCamera()
        {
            if (m_TrackingType2DisplayDict.ContainsKey(m_CameraType)) return;

            YVRTrackingCameraPlugin.OpenTrackingCamera(m_CameraType);
            m_TrackingType2DisplayDict[m_CameraType] = Instantiate(m_CameraImagePrefab, m_CameraImageCollection);
            m_TrackingType2DisplayDict[m_CameraType].name = m_CameraType.ToString();
            m_TrackingType2DisplayDict[m_CameraType].transform.Find("TrackingType").GetComponent<TextMeshProUGUI>().text
                = m_CameraType.ToString();
        }

        public void CloseTrackingCamera()
        {
            YVRTrackingCameraPlugin.CloseTrackingCamera(m_CameraType);
            Destroy(m_TrackingType2DisplayDict[m_CameraType]);
            m_TrackingType2DisplayDict.Remove(m_CameraType);
        }

        public void SubscribeTrackingCameraFrame()
        {
            TrackingCameraType type = m_CameraType;
            YVRTrackingCameraPlugin.SubscribeFrame(m_CameraType, cameraData =>
            {
                UpdateCameraData(type, cameraData);
            });
        }

        public void UnsubscribeTrackingCameraFrame() { YVRTrackingCameraPlugin.UnsubscribeFrame(m_CameraType); }

        public void AcquireTrackingCameraFrame()
        {
            CameraFrameItemData cameraData = default;
            YVRTrackingCameraPlugin.AcquireTrackingCameraFrame(m_CameraType, ref cameraData);

            UpdateCameraData(m_CameraType, cameraData);
        }

        private void UpdateCameraData(TrackingCameraType type, CameraFrameItemData cameraData)
        {
            byte[][] frameBytes = new byte[2][];

            for (int i = 0; i < cameraData.data.Length; i++)
            {
                if (cameraData.data[i] == IntPtr.Zero) continue;
                if (frameBytes[i] != null)
                    s_ArrayPool.Return(frameBytes[i]);
                byte[] data = s_ArrayPool.Rent(cameraData.dataSize);
                Marshal.Copy(cameraData.data[i], data, 0, cameraData.dataSize);
                frameBytes[i] = data;
            }

            UIThreadDispatcher.instance.Enqueue(() =>
            {
                var leftImage = m_TrackingType2DisplayDict[type].transform.Find("LeftImage").GetComponent<RawImage>();
                var rightImage = m_TrackingType2DisplayDict[type].transform.Find("RightImage").GetComponent<RawImage>();

                if (cameraData.data[0] != IntPtr.Zero)
                {
                    LoadY8Image(type, true, frameBytes[0], cameraData.stride,
                                cameraData.height, tex =>
                                {
                                    leftImage.texture = tex;
                                });
                }

                if (cameraData.data[1] != IntPtr.Zero)
                {
                    LoadY8Image(type, false, frameBytes[1], cameraData.stride,
                                cameraData.height, tex => rightImage.texture = tex);
                }
            });
        }

        private void LoadY8Image(TrackingCameraType type, bool isLeft, byte[] y8Data, int width, int height, System.Action<Texture> onTextureLoad)
        {
            if (!m_TrackingType2TexturesDict.ContainsKey(type))
            {
                var leftTexture = new Texture2D(width, height, TextureFormat.Alpha8, false);
                var rightTexture = new Texture2D(width, height, TextureFormat.Alpha8, false);
                m_TrackingType2TexturesDict[type] = (leftTexture, rightTexture);
            }

            Texture2D texture
                = isLeft ? m_TrackingType2TexturesDict[type].Item1 : m_TrackingType2TexturesDict[type].Item2;

            texture.LoadRawTextureData(y8Data);
            texture.Apply();

            onTextureLoad?.Invoke(texture);
        }
    }
}