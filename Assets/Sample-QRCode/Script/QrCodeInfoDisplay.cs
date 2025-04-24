using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class QrCodeInfoDisplay : MonoBehaviour
    {
        [SerializeField] private RawImage left;
        [SerializeField] private RawImage right;
        [SerializeField] private TMP_Text scanningState;
        [SerializeField] private TMP_Text scanningResult;

        public void RefreshVSTImage(Texture left, Texture right)
        {
            this.left.texture = left;
            this.right.texture = right;
        }

        public void RefreshScanningState(bool isScanning)
        {
            scanningState.text = $"Scanning State: {(isScanning ? "Scanning..." : "Stopped")}";
        }

        public void RefreshScanningResult(Result result)
        {
            bool noResult = result == null;
            string resultPart = $"<color=red><size={12}>{(noResult ? "No Result" : result.Text)}</size></color>";
            scanningResult.text = $"Scanning Result: {resultPart}";
            scanningResult.text = $"Scanning Result: \n {resultPart}";
        }
    }
}