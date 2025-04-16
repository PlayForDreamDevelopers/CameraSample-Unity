using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class QrCodeInfoDisplay : MonoBehaviour
    {
        public float ScanInterval = 1f;
        [SerializeField] private QrCodeControlPanel QrCodeControlPanel;
        [SerializeField] private RawImage Left;
        [SerializeField] private RawImage Right;
        public TMP_Text ScanTip;
        public TMP_Text ScanInfo;
        
        private float m_Timer;
        private Texture[] m_NowVSTCameraFrame;
        private BarcodeReader m_BarcodeReader;
        private Result m_Result;
        private bool m_IsScanning;
        public bool isRecode; 
        Result result = null;
        private void Update()
        {
            if (!QrCodeControlPanel.canScan||isRecode)
            {
                if (m_IsScanning)
                {
                    m_IsScanning = false;
                }
                return;
            }

            m_Timer += Time.deltaTime;
            if (m_Timer >= ScanInterval && !m_IsScanning)
            {
                m_Timer = 0;;
                StartAsyncScan();
            }
        }

        private async void StartAsyncScan()
        {
             
            // 获取VST当前帧
            m_NowVSTCameraFrame = QrCodeControlPanel.gameObject.GetComponent<VSTControl>()
                                                 .AcquireVSTCameraFrame();
            Left.texture = m_NowVSTCameraFrame[0];
            Right.texture = m_NowVSTCameraFrame[1];
            
            // 扫描解析当前帧
            m_Result = await Task.Run(() => ScanFrames());
            // 更新UI界面
            if (m_Result != null)
            {
                ScanTip.text = "ScanTips:Scan Successfully";
                ScanInfo.text = "ScanInfo:";
                ScanInfo.text += m_Result.Text;
                isRecode = true;
            }
            else
            {
                ScanTip.text = "ScanTips:Scanning...";
                ScanInfo.text = "ScanInfo:"; }
        }

        private Result ScanFrames()
        {
            
            for (int i = 0; i < 2; i++)
            {
                if (m_NowVSTCameraFrame[i] is Texture2D tex)
                {
                    result = ZXingQrCodeWrapper.ScanQrCode(tex, tex.width, tex.height);
                }
            }
            return result;
        }
    }
}