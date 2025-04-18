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
        private byte[][] m_NowVSTCameraFrameData = new byte[2][];
        private int[][] m_NowVSTCameraFrameInfo = new int[2][];
        
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
             

            m_NowVSTCameraFrame = QrCodeControlPanel.gameObject.GetComponent<VSTControl>()
                                                 .AcquireVSTCameraFrame(ref m_NowVSTCameraFrameData,ref m_NowVSTCameraFrameInfo);
            Left.texture = m_NowVSTCameraFrame[0];
            Right.texture = m_NowVSTCameraFrame[1];
            
            m_Result = await Task.Run(() => ScanFrames());
            
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
                result = ZXingQrCodeWrapper.ScanQrCode(m_NowVSTCameraFrameData[i],m_NowVSTCameraFrameInfo[i][0],m_NowVSTCameraFrameInfo[i][1]);
                    
                VSTControl.arrayPoolByte.Return(m_NowVSTCameraFrameData[i]);
                VSTControl.arrayPoolInt.Return(m_NowVSTCameraFrameInfo[i]);
            }
            return result;
        }
    }
}