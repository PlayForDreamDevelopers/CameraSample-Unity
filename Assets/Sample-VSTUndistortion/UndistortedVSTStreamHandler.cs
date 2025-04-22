using System;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace YVR.Enterprise.Camera.Samples.UnDistortion
{
    public class UndistortedVSTStreamHandler : MonoBehaviour
    {
        [SerializeField] private RawImage m_LeftImage;
        [SerializeField] private RawImage m_RightImage;

        private NativeArray<float> m_MapLeftXDataArray = default;
        private NativeArray<float> m_MapLeftYDataArray = default;
        private NativeArray<float> m_MapRightXDataArray = default;
        private NativeArray<float> m_MapRightYDataArray = default;

        private NativeArray<byte> m_LeftRgbDataArray = default;
        private NativeArray<byte> m_RightRgbDataArray = default;
        private NativeArray<byte> m_LeftUnDistortedRgbDataArray = default;
        private NativeArray<byte> m_RightUnDistortedRgbDataArray = default;
        private Texture2D m_LeftTexture = null;
        private Texture2D m_RightTexture = null;

        public string mapLeftXFile = "map_left_x";
        public string mapLeftYFile = "map_left_y";
        
        public string mapRightXFile = "map_right_x";
        public string mapRightYFile = "map_right_y";

        private void Start()
        {
            YVRVSTCameraPlugin.OpenVSTCamera();
            YVRVSTCameraPlugin.SetVSTCameraFrequency(VSTCameraFrequencyType.VSTFrequency30Hz);
            YVRVSTCameraPlugin.SetVSTCameraResolution(VSTCameraResolutionType.VSTResolution660_616);
            YVRVSTCameraPlugin.SetVSTCameraFormat(VSTCameraFormatType.VSTCameraFmtNv21);
            YVRVSTCameraPlugin.SetVSTCameraOutputSource(VSTCameraSourceType.VSTCameraBothEyes);

            int width = 660, height = 616; // Hard Code here
            m_MapLeftXDataArray = LoadLut(mapLeftXFile, width * height);
            m_MapLeftYDataArray = LoadLut(mapLeftYFile, width * height);
            m_MapRightXDataArray = LoadLut(mapRightXFile, width * height);
            m_MapRightYDataArray = LoadLut(mapRightYFile, width * height);


            m_LeftRgbDataArray = new NativeArray<byte>(width * height * 3, Allocator.Persistent);
            m_LeftUnDistortedRgbDataArray = new NativeArray<byte>(width * height * 3, Allocator.Persistent);
            m_LeftTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

            m_RightRgbDataArray = new NativeArray<byte>(width * height * 3, Allocator.Persistent);
            m_RightUnDistortedRgbDataArray = new NativeArray<byte>(width * height * 3, Allocator.Persistent);
            m_RightTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        private void OnDestroy()
        {
            m_MapLeftXDataArray.Dispose();
            m_MapLeftYDataArray.Dispose();
            m_MapRightXDataArray.Dispose();
            m_MapRightYDataArray.Dispose();
        }

        private void Update()
        {
            // As the frequency of VST camera is 30Hz, we can acquire the frame every 3 frames.
            if (Time.frameCount % 3 == 0)
                AcquireVSTCameraFrame();
        }

        private void AcquireVSTCameraFrame()
        {
            m_LeftImage.texture = null;
            m_RightImage.texture = null;
            VSTCameraFrameData frameData = default;
            YVRVSTCameraPlugin.AcquireVSTCameraFrame(ref frameData);

            if (frameData.cameraFrameItem.data[0] == IntPtr.Zero || frameData.cameraFrameItem.data[1] == IntPtr.Zero)
            {
                return;
            }

            VSTCameraIntrinsicExtrinsicData leftIntrinsicData = default;
            VSTCameraIntrinsicExtrinsicData rightIntrinsicData = default;
            YVRVSTCameraPlugin.GetVSTCameraIntrinsicExtrinsic(YVREyeNumberType.LeftEye, ref leftIntrinsicData);
            YVRVSTCameraPlugin.GetVSTCameraIntrinsicExtrinsic(YVREyeNumberType.RightEye, ref rightIntrinsicData);

            using NativeArray<byte> nv21NativeLeft
                = IntPtrToNativeArray(frameData.cameraFrameItem.data[0], frameData.cameraFrameItem.dataSize);
            using NativeArray<byte> nv21NativeRight
                = IntPtrToNativeArray(frameData.cameraFrameItem.data[1], frameData.cameraFrameItem.dataSize);

            JobHandle leftDistortionJobHandle = GetUndistortionJob(nv21NativeLeft,
                                                                   m_LeftRgbDataArray,
                                                                   m_LeftUnDistortedRgbDataArray,
                                                                   m_MapLeftXDataArray,
                                                                   m_MapLeftYDataArray,
                                                                   frameData.cameraFrameItem.width,
                                                                   frameData.cameraFrameItem.height,
                                                                   leftIntrinsicData);

            JobHandle rightDistortionJobHandle = GetUndistortionJob(nv21NativeRight,
                                                                    m_RightRgbDataArray,
                                                                    m_RightUnDistortedRgbDataArray,
                                                                    m_MapRightXDataArray,
                                                                    m_MapRightYDataArray,
                                                                    frameData.cameraFrameItem.width,
                                                                    frameData.cameraFrameItem.height,
                                                                    rightIntrinsicData);

            JobHandle combinedJobHandle = JobHandle.CombineDependencies(leftDistortionJobHandle,
                                                                        rightDistortionJobHandle);
            combinedJobHandle.Complete();

            m_LeftTexture.LoadRawTextureData(m_LeftUnDistortedRgbDataArray);
            m_LeftTexture.Apply();
            m_RightTexture.LoadRawTextureData(m_RightUnDistortedRgbDataArray);
            m_RightTexture.Apply();

            m_LeftImage.texture = m_LeftTexture;
            m_RightImage.texture = m_RightTexture;
        }

        private JobHandle GetUndistortionJob(NativeArray<byte> nv21Data,
                                             NativeArray<byte> rgbData,
                                             NativeArray<byte> unDistortedData,
                                             NativeArray<float> mapXData,
                                             NativeArray<float> mapYData,
                                             int width, int height,
                                             VSTCameraIntrinsicExtrinsicData intrinsic)
        {
            var job = new NV21ToRGBJob
            {
                nv21Data = nv21Data,
                rgbData = rgbData,
                width = width,
                height = height
            };

            JobHandle rgbJobHandle = job.Schedule(width * height, 256);

            intrinsic.fx = intrinsic.fx / 2640f * width;
            intrinsic.fy = intrinsic.fy / 2464f * height;
            intrinsic.cx = intrinsic.cx / 2640f * width;
            intrinsic.cy = intrinsic.cy / 2464f * height;

            var undistortedJob = new FisheyeUndistortJob
            {
                srcRgb = rgbData,
                dstRgb = unDistortedData,
                mapX = mapXData,
                mapY = mapYData,
                width = width,
                height = height,
                channels = 3
            };
            JobHandle handle = undistortedJob.Schedule(width * height, 256, rgbJobHandle);
            handle.Complete();

            return handle;
        }

        private static NativeArray<byte> IntPtrToNativeArray(IntPtr ptr, int length)
        {
            unsafe
            {
                NativeArray<byte> arr = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(
                     (void*) ptr, length, Allocator.None);
                return arr;
            }
        }

        private NativeArray<float> LoadLut(string resourceName, int length)
        {
            var binAsset = Resources.Load<TextAsset>(resourceName);
            if (binAsset == null)
                throw new FileNotFoundException("Resource not found: " + resourceName);

            byte[] bytes = binAsset.bytes;

            var arr = new NativeArray<float>(length, Allocator.Persistent);
            for (int i = 0; i < length; ++i)
                arr[i] = BitConverter.ToSingle(bytes, i * 4);
            return arr;
        }
    }
}