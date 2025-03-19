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

<img src="https://github.com/user-attachments/assets/2bcabde9-368c-4c89-a5b8-ee7c85d79bf4" alt="Camera" width="80%">

<img src="https://github.com/user-attachments/assets/121d0d30-8232-4a90-8170-f7af3832aa6c" alt="Camera" width="80%">

### Samples

### Tracking Camera

![2025 03 18_103505244](https://github.com/user-attachments/assets/72805eef-d2f9-4248-a9e7-fe6d84149002)

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

## Requirements

-   Unity 2022 3.58f1 or later
-   [YVR Core](https://github.com/PlayForDreamDevelopers/com.yvr.core-mirror)
-   [YVR Utilities](https://github.com/PlayForDreamDevelopers/com.yvr.Utilities-mirror)
-   [YVR Enterprise](https://github.com/PlayForDreamDevelopers/com.yvr.enterprise-mirror)
-   [YVR Android-Device Core](https://github.com/PlayForDreamDevelopers/com.yvr.android-device.core-mirror)
-   [YVR Interaction](https://github.com/PlayForDreamDevelopers/com.yvr.interaction-mirror)
-   Play For Dream MR Device
-   Play For Dream OS ENT 3.1.1 or later
