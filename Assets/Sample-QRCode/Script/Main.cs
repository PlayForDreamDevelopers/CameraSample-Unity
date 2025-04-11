using UnityEngine;
using UnityEngine.UI;
using YVR.Enterprise.Camera.Samples.QRCode;
using ZXing;

public class Main : MonoBehaviour
{    
    public RawImage img1;
    public RawImage img2;
    public RawImage img3;
    public Texture2D icon;
    void Start()
    {
        img1.texture = ZXingQrCodeWrapper.GenerateQrCode("This is the first QR code for test scanning.", Color.black,"One");
        img2.texture = ZXingQrCodeWrapper.GenerateQrCode("This is the Second QR code for test scanning.", Color.red,"two");
        img3.texture = ZXingQrCodeWrapper.GenerateQrCode("This is the third QR code for test scanning.", Color.black,icon,"three");
        Debug.Log(Application.persistentDataPath);
    }
}
