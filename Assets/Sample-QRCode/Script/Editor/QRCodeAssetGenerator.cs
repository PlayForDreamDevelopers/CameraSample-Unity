using UnityEngine;
using UnityEditor;
using ZXing;
using ZXing.QrCode;
using System.IO;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    [CreateAssetMenu(fileName = "QRCodeAsset", menuName = "QRCode/QRCode Asset Generator")]
    public class QRCodeAssetGenerator : ScriptableObject
    {
        public string qrContent = "Hello! Play For Dream";
        public string savePath = "Assets/Sample-QRCode/QRCode.png";
        public int qrSize = 512;

        public void GenerateAndSaveQRCode()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(qrContent))
            {
                Debug.LogError("There is no QRCode content!");
                return;
            }

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = qrSize,
                    Width = qrSize,
                    Margin = 2
                }
            };

            Color32[] color32 = writer.Write(qrContent);

            var tex = new Texture2D(qrSize, qrSize, TextureFormat.RGBA32, false);
            tex.SetPixels32(color32);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            DestroyImmediate(tex);

            string path = savePath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Application.dataPath, path.Replace("Assets/", ""));
            }

            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(path, pngData);
            Debug.Log($"QR Code Saved: {path}");
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
#endif
        }
    }
}