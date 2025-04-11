using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.Common;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class ZXingQrCodeWrapper : MonoBehaviour
{
    #region GenerateQRCode
 
    /*
     使用方法，类似如下：
     ZXingQRCodeWrapper.GenerateQRCode("Hello Wrold!", 512, 512);
     ZXingQRCodeWrapper.GenerateQRCode("I Love You!", 256, 256, Color.red);
     ZXingQRCodeWrapper.GenerateQRCode("中间带图片的二维码图片", Color.green, icon);
     .......
         */
    /// <summary>
    /// 生成2维码 方法一
    /// </summary>
    /// <param name="content"></param>
    /// <param name="qrCodeFileName"></param>
    /// <returns></returns>
    public static Texture2D GenerateQrCode(string content,string qrCodeFileName) 
    {
        return GenerateQrCode(content,256,256,qrCodeFileName);
    }

    /// <summary>
    /// 生成2维码 方法一
    /// 经测试：只能生成256x256的
    /// </summary>
    /// <param name="content"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="qrCodeFileName"></param>
    public static Texture2D GenerateQrCode(string content, int width, int height,string qrCodeFileName)
    {
        // 编码成color32
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
 
        // 转成texture2d
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels32(colors);
        texture.Apply(); 
        byte[] bytes = texture.EncodeToPNG();
        string path = System.IO.Path.Combine(Application.persistentDataPath, qrCodeFileName + ".png");
        System.IO.File.WriteAllBytes(path, bytes); 
        return texture;
    }

    /// <summary>
    /// 生成2维码 方法二
    /// </summary>
    /// <param name="content"></param>
    /// <param name="color"></param>
    /// <param name="qrCodeFileName"></param>
    /// <returns></returns>
    public static Texture2D GenerateQrCode(string content,Color color,string qrCodeFileName)
    {
        return GenerateQrCode(content, 256, 256, color, qrCodeFileName);
    }

    /// <summary>
    /// 生成2维码 方法二
    /// 经测试：能生成任意尺寸的正方形
    /// </summary>
    /// <param name="content"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="qrCodeFileName"></param>
    /// <returns></returns>
    public static Texture2D GenerateQrCode(string content, int width, int height, Color color,string qrCodeFileName)
    {
        BitMatrix bitMatrix;
        Texture2D texture = GenerateQrCode(content, width, height, color, out bitMatrix,qrCodeFileName); 
        return texture;
    }

    /// <summary>
    /// 生成2维码 方法二
    /// 经测试：能生成任意尺寸的正方形
    /// </summary>
    /// <param name="content"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="bitMatrix"></param>
    /// <param name="qrCodeFileName"></param>
    public static Texture2D GenerateQrCode(string content, int width, int height, Color color, out BitMatrix bitMatrix,string qrCodeFileName)
    {
        // 编码成color32
        MultiFormatWriter writer = new MultiFormatWriter();
        Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
        //设置字符串转换格式，确保字符串信息保持正确
        hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
        // 设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
        hints.Add(EncodeHintType.MARGIN, 1);
        hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
        //实例化字符串绘制二维码工具
        bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);
 
        // 转成texture2d
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

    /// <summary>
    /// 生成2维码 方法三
    /// 在方法二的基础上，添加小图标 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="color"></param>
    /// <param name="centerIcon"></param>
    /// <param name="qrCodeFileName"></param>
    /// <returns></returns>
    public static Texture2D GenerateQrCode(string content, Color color, Texture2D centerIcon ,string qrCodeFileName)
    {
        return GenerateQrCode(content, 256, 256, color, centerIcon,qrCodeFileName);
    }

    /// <summary>
    /// 生成2维码 方法三
    /// 在方法二的基础上，添加小图标
    /// </summary>
    /// <param name="content"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="centerIcon"></param>
    /// <param name="qrCodeFileName"></param>
    /// <returns></returns>
    public static Texture2D GenerateQrCode(string content, int width, int height, Color color, Texture2D centerIcon,string qrCodeFileName)
    {
        BitMatrix bitMatrix;
        Texture2D texture = GenerateQrCode(content, width, height, color, out bitMatrix,qrCodeFileName);
        int w = bitMatrix.Width;
        int h = bitMatrix.Height;
 
        // 添加小图
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
    /*
     使用方法，类似如下：
     Result result = ZXingQRCodeWrapper.ScanQRCode(data, webCamTexture.width, webCamTexture.height);
    */
 
    //二维码识别类
    private static BarcodeReader s_BarcodeReader;//库文件的对象（二维码信息保存的地方）
 
    /// <summary>
    /// 传入图片识别
    /// </summary>
    /// <param name="textureData"></param>
    /// <param name="textureDataWidth"></param>
    /// <param name="textureDataHeight"></param>
    /// <returns></returns>
    public static Result ScanQrCode(Texture2D textureData, int textureDataWidth, int textureDataHeight)
    {
        return ScanQrCode(textureData.GetPixels32(), textureDataWidth, textureDataHeight);
    }
 
    /// <summary>
    /// 传入图片像素识别
    /// </summary>
    /// <param name="textureData"></param>
    /// <param name="textureDataWidth"></param>
    /// <param name="textureDataHeight"></param>
    /// <returns></returns>
    public static Result ScanQrCode(Color32[] textureData, int textureDataWidth, int textureDataHeight) {
        s_BarcodeReader ??= new BarcodeReader();
        Result result = s_BarcodeReader.Decode(textureData, textureDataWidth, textureDataHeight);
        return result;
    } 
    #endregion
}

}
