using UnityEngine;
using UnityEngine.UI;
using YVR.Enterprise.Camera.Samples.Utilities;

namespace YVR.Enterprise.Camera.Samples.UnDistortion
{
    public class UndistortedVSTStreamHandler : MonoBehaviour
    {
        [SerializeField] private RawImage m_LeftImage;
        [SerializeField] private RawImage m_RightImage;

        private UndistortionTextureWrapper m_UndistortionTextureWrapper = null;

        private void Start()
        {
            OpenVSTCamera();

            int width = 660, height = 616; // Hard Code here
            m_UndistortionTextureWrapper = new UndistortionTextureWrapper(width, height, "map_left_x", "map_left_y",
                                                                          "map_right_x", "map_right_y");
        }

        private void Update()
        {
            // As the frequency of VST camera is 30Hz, we can acquire the frame every 3 frames.
            if (Time.frameCount % 3 != 0) return;

            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);

            m_UndistortionTextureWrapper.RefreshTexture(frameData);
            m_LeftImage.texture = m_UndistortionTextureWrapper.leftTexture;
            m_RightImage.texture = m_UndistortionTextureWrapper.rightTexture;
        }

        public void OpenVSTCamera()
        {
            YVRVSTCameraPlugin.OpenVSTCamera();
            YVRVSTCameraPlugin.SetVSTCameraFrequency(VSTCameraFrequencyType.VSTFrequency30Hz);
            YVRVSTCameraPlugin.SetVSTCameraResolution(VSTCameraResolutionType.VSTResolution660_616);
            YVRVSTCameraPlugin.SetVSTCameraFormat(VSTCameraFormatType.VSTCameraFmtNv21);
            YVRVSTCameraPlugin.SetVSTCameraOutputSource(VSTCameraSourceType.VSTCameraBothEyes);
        }
        public void CloseVSTCamera()
        {
            YVRVSTCameraPlugin.CloseVSTCamera();
        }
    }
}