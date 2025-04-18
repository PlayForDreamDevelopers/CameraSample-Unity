using UnityEngine;
using UnityEngine.UI;

namespace YVR.Enterprise.Camera.Samples.QRCode
{ 
    public class QrCodeControlPanel : MonoBehaviour
    {
        [SerializeField]private QrCodeInfoDisplay QrCodeInfoDisplayPanel;
        
        [SerializeField]private Button BeginScanningButton;
        [SerializeField]private Button StopScanningButton;
        public bool canScan = false;
        public bool IsScanning = false;

        private void Start()
        {
            BeginScanningButton.onClick.AddListener(() =>
            {
                GetComponent<VSTCameraControl>().OpenVSTCamera();
                IsScanning = true;
                ResetPanel(IsScanning);
                canScan = true;
                QrCodeInfoDisplayPanel.isRecode = false;
            });
            StopScanningButton.onClick.AddListener(() =>
            {
                GetComponent<VSTCameraControl>().CloseVSTCamera();
                IsScanning = false;
                ResetPanel(IsScanning);
                canScan = false;
            });
        }
        
        public void ResetPanel(bool isScanning = false)
        {
            QrCodeInfoDisplayPanel.ScanTip.text = "ScanTips:";
            QrCodeInfoDisplayPanel.ScanTip.text += isScanning ? "Scanning..." : "StopScan";
            QrCodeInfoDisplayPanel.ScanInfo.text = "ScanInfo:";
            QrCodeInfoDisplayPanel.ScanInfo.text += "";
        }
    }
}