using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class QrCodeInfoDisplay : MonoBehaviour
    {
        [SerializeField] private QrCodeControlPanel QrCodeControlPanel;
        [SerializeField] private RawImage Left;
        [SerializeField] private RawImage Right;
        public TMP_Text ScanTip;
        public TMP_Text ScanInfo;
        
        private Texture[] m_NowVSTCameraFrame;
        private byte[][] m_NowVSTCameraFrameData = new byte[2][];
        private int[][] m_NowVSTCameraFrameInfo = new int[2][];
        
        private BarcodeReader m_BarcodeReader;
        private Result m_Result;
        private bool m_IsScanning;
        public bool isRecode;
        private Result m_TmpResult = null;
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

            if (!m_IsScanning)
                StartAsyncScan();
        }

        private async void StartAsyncScan()
        {
             

            m_NowVSTCameraFrame = QrCodeControlPanel.gameObject.GetComponent<VSTCameraControl>()
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
            else if(m_Result == null && !isRecode && m_IsScanning)
            {
                ScanTip.text = "ScanTips:Scanning...";
                ScanInfo.text = "ScanInfo:"; 
            }
        }

        private Result ScanFrames()
        {
            if (!QrCodeControlPanel.IsScanning)
                return null;
            for (int i = 0; i < 2; i++)
            {
                m_TmpResult = ZXingQrCodeWrapper.ScanQrCode(m_NowVSTCameraFrameData[i],m_NowVSTCameraFrameInfo[i][0] < 0 ? 0 : m_NowVSTCameraFrameInfo[i][0],m_NowVSTCameraFrameInfo[i][1] < 0 ? 0 : m_NowVSTCameraFrameInfo[i][1]);
                
                VSTCameraControl.arrayPoolByte.Return(m_NowVSTCameraFrameData[i]);
                VSTCameraControl.arrayPoolInt.Return(m_NowVSTCameraFrameInfo[i]);
            }
            return m_TmpResult;
        }
    }
}