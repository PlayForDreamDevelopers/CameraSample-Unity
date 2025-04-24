using UnityEditor;
using UnityEngine;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
#if UNITY_EDITOR
    [CustomEditor(typeof(QRCodeAssetGenerator))]
    public class QRCodeAssetGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var gen = (QRCodeAssetGenerator) target;
            if (GUILayout.Button("Generate QRCode"))
            {
                gen.GenerateAndSaveQRCode();
            }
        }
    }
#endif
}