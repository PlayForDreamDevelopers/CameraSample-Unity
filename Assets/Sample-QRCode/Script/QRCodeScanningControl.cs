using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using YVR.Enterprise.Camera.Samples.Utilities;
using ZXing;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class QRCodeScanningControl : MonoBehaviour
    {
        [SerializeField] private QrCodeInfoDisplay qrCodeInfoDisplay = null;

        private UndistortionTextureWrapper m_UndistortionTextureWrapper = null;
        private object m_VSTStateLock = new object();
        private bool m_VSTCameraOpened = false;

        private Task m_QRScanTask;
        private Result m_LastResult;
        private CancellationTokenSource m_QRScanCancel;


        private void Start()
        {
            qrCodeInfoDisplay.RefreshScanningState(false);
            qrCodeInfoDisplay.RefreshScanningResult(null);

            YVRVSTCameraPlugin.SetVSTCameraFrequency(VSTCameraFrequencyType.VSTFrequency30Hz);
            YVRVSTCameraPlugin.SetVSTCameraResolution(VSTCameraResolutionType.VSTResolution660_616);
            YVRVSTCameraPlugin.SetVSTCameraFormat(VSTCameraFormatType.VSTCameraFmtNv21);
            YVRVSTCameraPlugin.SetVSTCameraOutputSource(VSTCameraSourceType.VSTCameraBothEyes);

            int width = 660, height = 616; // Hard Code here
            m_UndistortionTextureWrapper = new UndistortionTextureWrapper(width, height, "map_left_x", "map_left_y",
                                                                          "map_right_x", "map_right_y");
        }

        private void Update()
        {
            // As the frequency of VST camera is 30Hz, we can acquire the frame every 3 frames.
            if (!m_VSTCameraOpened || Time.frameCount % 3 != 0) return;

            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);

            m_UndistortionTextureWrapper.RefreshTexture(frameData);

            qrCodeInfoDisplay.RefreshVSTImage(m_UndistortionTextureWrapper.leftTexture,
                                              m_UndistortionTextureWrapper.rightTexture);

            if (m_QRScanTask == null || m_QRScanTask.IsCompleted)
            {
                byte[] left = m_UndistortionTextureWrapper.leftNV21DataConverter.normalizedRGBDataArray.ToArray();
                byte[] right = m_UndistortionTextureWrapper.rightNV21DataConverter.normalizedRGBDataArray.ToArray();

                m_QRScanCancel?.Cancel();
                m_QRScanCancel = new CancellationTokenSource();

                m_QRScanTask = Task.Run(() => ScanQRCode(left, 616, 660) ?? ScanQRCode(right, 616, 660),
                                        m_QRScanCancel.Token).ContinueWith(task =>
                {
                    lock (m_VSTStateLock)
                    {
                        if (task.Status == TaskStatus.RanToCompletion && m_VSTCameraOpened)
                            m_LastResult = task.Result;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            qrCodeInfoDisplay.RefreshScanningResult(m_LastResult);

            if (m_LastResult != null)
            {
                CloseVSTCamera();
            }
        }

        public void OpenVSTCamera()
        {
            YVRVSTCameraPlugin.OpenVSTCamera();
            qrCodeInfoDisplay.RefreshScanningState(true);
            m_VSTCameraOpened = true;
        }

        public void CloseVSTCamera()
        {
            YVRVSTCameraPlugin.CloseVSTCamera();
            if(!m_VSTCameraOpened)
                return;
            qrCodeInfoDisplay.RefreshScanningState(false);
            m_QRScanCancel.Cancel();
            lock (m_VSTStateLock)
            {
                m_VSTCameraOpened = false;
                m_LastResult = null;
            }
        }

        private static Result ScanQRCode(byte[] rgbData, int width, int height)
        {
            var source = new RGBLuminanceSource(rgbData, width, height, RGBLuminanceSource.BitmapFormat.RGB24);
            var reader = new BarcodeReader();

            return reader.Decode(source);
        }
    }
}