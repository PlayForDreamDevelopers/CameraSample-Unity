using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class ZXingQrCodeWrapper : MonoBehaviour
{
    #region GenerateQRCode
    
    public static Texture2D GenerateQrCode(string content,string qrCodeFileName) 
    {
        return GenerateQrCode(content,256,256,qrCodeFileName);
    }

    public static Texture2D GenerateQrCode(string content, int width, int height,string qrCodeFileName)
    {
        EncodingOptions options = null;
        BarcodeWriter writer = new BarcodeWriter();
        options = new EncodingOptions
        {
            Width = width,
            Height = height,
            Margin = 1,
        };
        options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
        writer.Format = BarcodeFormat.QR_CODE;
        writer.Options = options;
        Color32[] colors = writer.Write(content);
 
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels32(colors);
        texture.Apply(); 
        byte[] bytes = texture.EncodeToPNG();
        string path = System.IO.Path.Combine(Application.persistentDataPath, qrCodeFileName + ".png");
        System.IO.File.WriteAllBytes(path, bytes); 
        return texture;
    }

    public static Texture2D GenerateQrCode(string content,Color color,string qrCodeFileName)
    {
        return GenerateQrCode(content, 256, 256, color, qrCodeFileName);
    }
    
    public static Texture2D GenerateQrCode(string content, int width, int height, Color color,string qrCodeFileName)
    {
        BitMatrix bitMatrix;
        Texture2D texture = GenerateQrCode(content, width, height, color, out bitMatrix,qrCodeFileName); 
        return texture;
    }
    
    public static Texture2D GenerateQrCode(string content, int width, int height, Color color, out BitMatrix bitMatrix,string qrCodeFileName)
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

    public static Texture2D GenerateQrCode(string content, Color color, Texture2D centerIcon ,string qrCodeFileName)
    {
        return GenerateQrCode(content, 256, 256, color, centerIcon,qrCodeFileName);
    }

    public static Texture2D GenerateQrCode(string content, int width, int height, Color color, Texture2D centerIcon,string qrCodeFileName)
    {
        BitMatrix bitMatrix;
        Texture2D texture = GenerateQrCode(content, width, height, color, out bitMatrix,qrCodeFileName);
        int w = bitMatrix.Width;
        int h = bitMatrix.Height;
 

        int halfWidth = texture.width / 2;
        int halfHeight = texture.height / 2;
        int halfWidthOfIcon = centerIcon.width / 2;
        int halfHeightOfIcon = centerIcon.height / 2;
        int centerOffsetX = 0;
        int centerOffsetY = 0;
        for (int x = 0; x < h; x++)
        {
            for (int y = 0; y < w; y++)
            {
                centerOffsetX = x - halfWidth;
                centerOffsetY = y - halfHeight;
                if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                {
                    texture.SetPixel(x, y, centerIcon.GetPixel(centerOffsetX + halfWidthOfIcon, centerOffsetY + halfHeightOfIcon));
                }
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

    private static LuminanceSource s_MLuminanceSource;

    private static HybridBinarizer s_MBinarizer;

    private static BinaryBitmap s_MBinaryBitmap;

    private static Result s_MResult;
    
    private static PlanarYUVLuminanceSource s_MmLuminanceSource;

    private static QRCodeReader s_QrCodeReader;
    

    public static Result ScanQrCode(byte[] texture, int textureDataWidth, int textureDataHeight)
    {
        s_QrCodeReader ??= new QRCodeReader();
        
        s_MLuminanceSource = CreateLuminanceSource(texture, textureDataWidth, textureDataHeight);
        
        s_MBinarizer = new HybridBinarizer(s_MLuminanceSource);
        
        s_MBinaryBitmap = new BinaryBitmap(s_MBinarizer);
        
        s_MResult = s_QrCodeReader.decode(s_MBinaryBitmap); 
        
        return s_MResult;
    }
    private static LuminanceSource CreateLuminanceSource(byte[] textureData,int textureDataWidth, int textureDataHeight)
    {
        return new PlanarYUVLuminanceSource(ConvertNV21ToYUV420P(textureData,textureDataWidth,textureDataHeight),
                                            textureDataWidth,
                                            textureDataHeight,
                                            0, 0,
                                            textureDataWidth,
                                            textureDataHeight,
                                            false
                                            );
    }
    
    public static byte[] ConvertNV21ToYUV420P(byte[] nv21Data, int width, int height)
    {
        int ySize = width * height;
        int uvSize = ySize / 4;
        
        byte[] yuv420p = new byte[ySize + 2 * uvSize];
        
        Buffer.BlockCopy(nv21Data, 0, yuv420p, 0, ySize);
        
        int srcOffset = ySize;
        int uOffset = ySize;
        int vOffset = ySize + uvSize;

        for (int i = 0; i < uvSize; i++)
        {
            yuv420p[vOffset + i] = nv21Data[srcOffset + 2 * i];     // V
            yuv420p[uOffset + i] = nv21Data[srcOffset + 2 * i + 1]; // U
        }

        return yuv420p;
    }
    #endregion
}

}
