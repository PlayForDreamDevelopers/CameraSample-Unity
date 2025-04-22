using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class ZXingQrCodeWrapper : MonoBehaviour
    {
        #region GenerateQRCode

        public static Texture2D GenerateQrCode(string content, Color color, string qrCodeFileName)
        {
            return GenerateQrCode(content, 256, 256, color, qrCodeFileName);
        }

        private static Texture2D GenerateQrCode(string content, int width, int height, Color color,
                                                string qrCodeFileName)
        {
            Texture2D texture = GenerateQrCode(content, width, height, color, out BitMatrix _, qrCodeFileName);
            return texture;
        }

        private static Texture2D GenerateQrCode(string content, int width, int height, Color color,
                                                out BitMatrix bitMatrix, string qrCodeFileName)
        {
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();

            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

            hints.Add(EncodeHintType.MARGIN, 1);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);

            bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);


            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            Texture2D texture = new Texture2D(w, h);
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    if (bitMatrix[x, y])
                    {
                        texture.SetPixel(y, x, color);
                    }
                    else
                    {
                        texture.SetPixel(y, x, Color.white);
                    }
                }
            }

            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            string path = System.IO.Path.Combine(Application.persistentDataPath, qrCodeFileName + ".png");
            System.IO.File.WriteAllBytes(path, bytes);
            return texture;
        }

        public static Texture2D GenerateQrCode(string content, Color color, Texture2D centerIcon, string qrCodeFileName)
        {
            return GenerateQrCode(content, 256, 256, color, centerIcon, qrCodeFileName);
        }

        public static Texture2D GenerateQrCode(string content, int width, int height, Color color, Texture2D centerIcon,
                                               string qrCodeFileName)
        {
            BitMatrix bitMatrix;
            Texture2D texture = GenerateQrCode(content, width, height, color, out bitMatrix, qrCodeFileName);
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;

            int halfWidth = texture.width / 2;
            int halfHeight = texture.height / 2;
            int halfWidthOfIcon = centerIcon.width / 2;
            int halfHeightOfIcon = centerIcon.height / 2;

            int centerOffsetX;
            int centerOffsetY;

            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < w; y++)
                {
                    centerOffsetX = x - halfWidth;
                    centerOffsetY = y - halfHeight;
                    if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                        texture.SetPixel(x, y, centerIcon.GetPixel(centerOffsetX + halfWidthOfIcon,
                                                                   centerOffsetY + halfHeightOfIcon));
                }
            }

            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            string path = System.IO.Path.Combine(Application.persistentDataPath, qrCodeFileName + ".png");
            System.IO.File.WriteAllBytes(path, bytes);
            return texture;
        }

        #endregion

        #region ScanQRCode

        private static QRCodeReader s_QrCodeReader;


        public static Result ScanQrCode(byte[] texture, int textureDataWidth, int textureDataHeight)
        {
            s_QrCodeReader ??= new QRCodeReader();

            var luminanceSource = CreateLuminanceSource(texture, textureDataWidth, textureDataHeight);

            var binarizer = new HybridBinarizer(luminanceSource);

            var binaryBitmap = new BinaryBitmap(binarizer);

            var result = s_QrCodeReader.decode(binaryBitmap);

            return result;
        }

        private static LuminanceSource CreateLuminanceSource(byte[] textureData, int textureDataWidth,
                                                             int textureDataHeight)
        {
            int ySize = textureDataWidth * textureDataHeight;
            int uvSize = ySize / 4;
            int totalSize = ySize + uvSize * 2;

            using var nv21Native = new NativeArray<byte>(textureData, Allocator.Persistent);
            var yuv420PNative = new NativeArray<byte>(totalSize, Allocator.Persistent);

            ImageConversionLibrary.ConvertNV21ToYuv420P(in nv21Native, ref yuv420PNative, textureDataWidth,
                                                        textureDataHeight);

            byte[] yuv420P = yuv420PNative.ToArray();
            var ret = new PlanarYUVLuminanceSource(yuv420P, textureDataWidth, textureDataHeight, 0, 0,
                                                   textureDataWidth, textureDataHeight, false);

            yuv420PNative.Dispose();
            return ret;
        }

        #endregion
    }
}