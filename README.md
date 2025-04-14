[![zh](https://img.shields.io/badge/lang-zh-blue.svg)](./README.zh.md)

<!--
READ ME FIRST !!!!!!
Replace the following placeholders with the actual values:
    - {{PROJECT_REPO_URL}}: URL of the project repository
    - {{Project Name}}: Name of the project
    - {{DocumentationURL}}: URL of the project documentation, Use github pages with docfx if possible
    - {{BriefDescription}}: Brief description about the project
    - {SampleURL}: URL of the sample project, for package projects, it should be sample repository URL. If a package projects has multiple samples, then link to `Samples` header of the `About The Project` section.
    - {BugIssueURL}: URL of the bug reporting issue template
      - i.e.  https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=bug_report.yml
    - {FeatureIssueURL}: URL of the feature request issue template
      - i.e. https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=feature_request.yml
    - {DocumentationIssueURL}: URL of the documentation issue template
      - i.e. https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=documentation_update.yml
-->

<br />
<div align="center">
    <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity">
        <img src="https://www.pfdm.cn/en/static/img/logo.2b1b07e.png" alt="Logo" width="20%">
    </a>
    <h1 align="center"> CameraSample-Unity </h1>
    <p align="center">
        Samples to demonstrate how to get hardware camera information of the Play For Dream MR Devices.
        <br />
        <a href="#samples">View Samples</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=bug_report.yml">Report Bug</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=feature_request.yml">Request Feature</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=documentation_update.yml">Improve Documentation</a>
    </p>

</div>

> [!tip]
>
> These samples only work on enterprise devices. For more details, refer to [Enterprise Services](https://www.pfdm.cn/yvrdoc/biz/docs/0.Overview.html)

## About The Project

This projects provide several samples to demonstrate how to get hardware camera information of the Play For Dream MR Devices.

<img src="https://github.com/user-attachments/assets/b36a2b5d-9fbf-4a96-88bd-6ddf467db5af" alt="Camera" width="80%">

<img src="https://github.com/user-attachments/assets/04df5b8f-90b8-424c-9d5f-b6ac7f70d97c" alt="Camera" width="80%">

### Samples

> [!tip]
>
> The image data displayed in the examples is rendered directly from the byte[] data of the image without any processing. Therefore, there may be issues with incorrect or inverted image orientation. If your project requires rendering camera data, please handle it according to your specific needs.

### Tracking Camera

![2025 03 24_111307844](https://github.com/user-attachments/assets/075c4193-0994-4787-ac45-23b37c8b50b6)

This sample demonstrates how to get the tracking camera information of the Play For Dream MR Devices, including：

-   Tracking Master
-   Tracking Slave
-   Tracking Aux
-   Eye Tracking


In this sample, for getting the tracking camera information, you need first select the target camera type via dropdown list in the upper-left corner of the `Camera Control` panel, then click the `Open Tracking Camera` button to open the selected camera, and then you can use `Acquiring Camera Frame` to get static camera frame and `Subscribe Camera Data` to get camera stream.

-   All camera data is in `Y8` format.

### VST Camera

![2025 03 18_231239385](https://github.com/user-attachments/assets/bc46b8da-e86a-43df-9f51-b7d3a8fff6da)

This sample demonstrates how to get the VST camera information of the Play For Dream MR Devices.

In this sample, for getting the vst camera frame, you need first click the `Open VST Camera` button to open the camera, then you can use `Acquiring Camera Frame` to get static camera frame.

-   VST Camera data is in `NV21` format
-   For now, the vst camera dose not support get camera stream.

### QrCode Scan

![2025 04 08_152536064](https://github.com/user-attachments/assets/24c3b682-3840-4da4-8b58-ec7bfad9513a)

This sample shows how to scan a QR code with a Play For Dream MR device.

In this example, to scan the QR code, click `BeginScanning Scanning Button` to start scanning. If the scan is successful, `ScanInfo` in the panel will display the scan information, and `AcquireVSTCameraFrame` will display the static camera frame when the scan is successful.

## Requirements

-   Unity 2022 3.58f1 or later
-   [YVR Core](https://github.com/PlayForDreamDevelopers/com.yvr.core-mirror)
-   [YVR Utilities](https://github.com/PlayForDreamDevelopers/com.yvr.Utilities-mirror)
-   [YVR Enterprise](https://github.com/PlayForDreamDevelopers/com.yvr.enterprise-mirror)
-   [YVR Android-Device Core](https://github.com/PlayForDreamDevelopers/com.yvr.android-device.core-mirror)
-   [YVR Interaction](https://github.com/PlayForDreamDevelopers/com.yvr.interaction-mirror)
-   Play For Dream MR Device
-   Play For Dream OS ENT 3.1.1 or later
