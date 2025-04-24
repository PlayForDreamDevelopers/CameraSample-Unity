using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace YVR.Enterprise.Camera.Samples.Utilities
{
    public class UndistortionTextureWrapper
    {
        public Texture2D leftTexture = null;
        public Texture2D rightTexture = null;

        public NV21DataConverter leftNV21DataConverter = null;
        public NV21DataConverter rightNV21DataConverter = null;
        private UndistortionMap m_LeftUndistortionMap = null;
        private UndistortionMap m_RightUndistortionMap = null;

        public UndistortionTextureWrapper(int width, int height, string leftMapXFile, string leftMapYFile,
                                          string rightMapXFile, string rightMapYFile)
        {
            // As the image is rotated 90 degrees, the width and height of image are swapped

            leftNV21DataConverter = new NV21DataConverter(660, 616);
            m_LeftUndistortionMap = new UndistortionMap(660, 616, leftMapXFile, leftMapYFile);
            leftTexture = new Texture2D(height, width, TextureFormat.RGB24, false);

            rightNV21DataConverter = new NV21DataConverter(660, 616);
            m_RightUndistortionMap = new UndistortionMap(660, 616, rightMapXFile, rightMapYFile);
            rightTexture = new Texture2D(height, width, TextureFormat.RGB24, false);
        }

        public void RefreshTexture(VSTCameraFrameData frameData)
        {

            if (frameData.cameraFrameItem.data[0] == IntPtr.Zero || frameData.cameraFrameItem.data[1] == IntPtr.Zero)
            {
                return;
            }

            using NativeArray<byte> nv21NativeLeft
                = IntPtrToNativeArray(frameData.cameraFrameItem.data[0], frameData.cameraFrameItem.dataSize);

            JobHandle leftDistortionJobHandle
                = leftNV21DataConverter.GetNormalizeRGBDataJobHandle(nv21NativeLeft, m_LeftUndistortionMap);
            using NativeArray<byte> nv21NativeRight
                = IntPtrToNativeArray(frameData.cameraFrameItem.data[1], frameData.cameraFrameItem.dataSize);
            JobHandle rightDistortionJobHandle = rightNV21DataConverter.GetNormalizeRGBDataJobHandle(nv21NativeRight,
             m_RightUndistortionMap);

            JobHandle combinedJobHandle = JobHandle.CombineDependencies(leftDistortionJobHandle,
                                                                        rightDistortionJobHandle);
            combinedJobHandle.Complete();

            leftTexture.LoadRawTextureData(leftNV21DataConverter.normalizedRGBDataArray);
            leftTexture.Apply();
            rightTexture.LoadRawTextureData(rightNV21DataConverter.normalizedRGBDataArray);
            rightTexture.Apply();
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
    }
}