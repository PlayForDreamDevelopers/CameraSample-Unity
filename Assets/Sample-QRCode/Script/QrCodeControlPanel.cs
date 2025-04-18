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
                GetComponent<VSTControl>().OpenVSTCamera();
                canScan = true;
                IsScanning = true;
                QrCodeInfoDisplayPanel.isRecode = false;
                ResetPanel(IsScanning);
            });
            StopScanningButton.onClick.AddListener(() =>
            {
                GetComponent<VSTControl>().CloseVSTCamera();
                canScan = false;
                IsScanning = false;
                ResetPanel(IsScanning);
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