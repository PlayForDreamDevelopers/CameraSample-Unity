using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using YVR.Core;

namespace YVR.Enterprise.Camera.Samples.QRCode
{
    public class Camera2VSTExtrinsicSample : MonoBehaviour
    {
        public GameObject cubePrefab;

        private void Update()
        {
            if (!YVRInput.GetDown(YVRInput.RawButton.A)) return;

            VSTCameraExtrinsicData camera2LeftVST = default;
            VSTCameraExtrinsicData camera2RightVST = default;
            YVRVSTCameraPlugin.GetEyeCenterToVSTCameraExtrinsic(YVREyeNumberType.LeftEye, ref camera2LeftVST);
            YVRVSTCameraPlugin.GetEyeCenterToVSTCameraExtrinsic(YVREyeNumberType.RightEye, ref camera2RightVST);

            var headInputDevice = new List<InputDevice>();
            var headCharacteristics = InputDeviceCharacteristics.HeadMounted;
            InputDevices.GetDevicesWithCharacteristics(headCharacteristics, headInputDevice);

            if (headInputDevice.Count > 0)
            {
                headInputDevice[0].TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 position);
                headInputDevice[0].TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion quat);
                
                GameObject centerEye = Instantiate(cubePrefab);
                centerEye.GetComponent<MeshRenderer>().material.color = Color.red;
                centerEye.transform.position = position;
                centerEye.transform.rotation = quat;
                
                GameObject leftEye = Instantiate(cubePrefab, centerEye.transform, true);
                leftEye.GetComponent<MeshRenderer>().material.color = Color.green;
                Vector3 parentScale = centerEye.transform.localScale;
                Vector3 leftPositionBias = camera2LeftVST.translation / parentScale.x;
                Quaternion leftRotationBias = camera2LeftVST.rotation;
                leftEye.transform.localPosition = leftPositionBias;
                leftEye.transform.localRotation = leftRotationBias;

                GameObject rightEye = Instantiate(cubePrefab, centerEye.transform, true);
                rightEye.GetComponent<MeshRenderer>().material.color = Color.blue;
                Vector3 rightPositionBias = camera2RightVST.translation / parentScale.x;
                Quaternion rightRotationBias = camera2RightVST.rotation;
                rightEye.transform.localPosition = rightPositionBias;
                rightEye.transform.localRotation = rightRotationBias;
            }

        }
    }
}