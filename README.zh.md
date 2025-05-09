[![en](https://img.shields.io/badge/lang-en-red.svg)](./README.md)

<!--
请先阅读此文档！！！！
将以下占位符替换为实际值：
    - {{PROJECT_REPO_URL}}: 项目仓库的URL
    - {{DocumentationURL}}: 项目文档的URL，尽可能使用docfx生成的GitHub Pages
    - {{BriefDescription}}: 项目简介
    - {SampleURL}: 示例项目的URL，对于包项目，应为示例仓库的URL。如果包项目有多个示例，请链接到“关于项目”部分的“示例”标题。
    - {BugIssueURL}: 报告错误问题的URL
      - 例如：https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=bug_report.yml
    - {FeatureIssueURL}: 请求功能问题的URL
      - 例如：https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=feature_request.yml
    - {DocumentationIssueURL}: 文档问题的URL
      - 例如：https://github.com/PlayForDreamDevelopers/unity-template/issues/new?template=documentation_update.yml
-->

<br />
<div align="center">
    <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity">
        <img src="https://www.pfdm.cn/en/static/img/logo.2b1b07e.png" alt="Logo" width="20%">
    </a>
    <h1 align="center"> CameraSample-Unity </h1>
    <p align="center">
        示例展示如何获取 Play For Dream MR 设备的硬件相机信息。
        <br />
        <a href="#samples">查看示例</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=bug_report.yml">报告错误</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=feature_request.yml">请求功能</a>
        &middot;
        <a href="https://github.com/PlayForDreamDevelopers/CameraSample-Unity/issues/new?template=documentation_update.yml">改进文档</a>
    </p>

</div>

> [!tip]
>
> 这些示例仅适用于企业设备。更多详情请参考 [企业服务](https://www.pfdm.cn/yvrdoc/biz/docs/0.Overview.html)

## 关于项目

这个项目提供了几个示例，展示如何获取 Play For Dream MR 设备的硬件相机信息。

<img src="https://github.com/user-attachments/assets/bef87eb2-7d8b-48e1-a894-0a751b7f5ff1" alt="Camera" width="80%">

<img src="https://github.com/user-attachments/assets/04df5b8f-90b8-424c-9d5f-b6ac7f70d97c" alt="Camera" width="80%">

### 示例

> [!tip]
>
> 示例中的展示的图像数据，都是直接对图像的 byte[] 数据进行渲染，不会进行任何处理。因此会有画面角度不正确/颠倒的问题，如果你的项目中需要使用到相机数据进行渲染，请根据实际需求进行处理。

### 追踪相机

![2025 03 24_111307844](https://github.com/user-attachments/assets/075c4193-0994-4787-ac45-23b37c8b50b6)

此示例展示了如何获取 Play For Dream MR 设备的追踪相机信息，包括：

-   Tracking Master
-   Tracking Slave
-   Tracking Aux
-   Eye Tracking


在此示例中，要获取跟踪相机信息，首先需要在 `Camera Control` 面板左上角的下拉列表中选择目标相机类型，然后点击 `Open Tracking Camera` 按钮打开所选相机，然后可以使用 `Acquiring Camera Frame` 获取静态相机帧，使用 `Subscribe Camera Data` 获取相机流。

-   所有相机数据均为 `Y8` 格式。

### VST Camera

![2025 03 18_231239385](https://github.com/user-attachments/assets/bc46b8da-e86a-43df-9f51-b7d3a8fff6da)

此示例展示了如何获取 Play For Dream MR 设备的 VST 相机信息。

在此示例中，要获取 VST 相机帧，首先需要点击 `Open VST Camera` 按钮打开相机，然后可以使用 `Acquiring Camera Frame` 获取静态相机帧。

-   VST 相机数据为 `NV21` 格式
-   目前，VST 相机不支持获取相机流
   

### QrCode Scan

![2025 04 08_152536064](https://github.com/user-attachments/assets/24c3b682-3840-4da4-8b58-ec7bfad9513a)

此示例展示了如何通过 Play For Dream MR 设备扫描二维码

此示例中，要扫描二维码，要点击 `BeginScanning Scanning Button` 开始扫描，扫描成功会，面板中 `ScanInfo` 会显示扫描信息，`AcquireVSTCameraFrame` 会显示扫描成功时的静态相机帧

## 要求

-   Unity 2022 3.58f1 或更高版本
-   [YVR Core](https://github.com/PlayForDreamDevelopers/com.yvr.core-mirror)
-   [YVR Utilities](https://github.com/PlayForDreamDevelopers/com.yvr.Utilities-mirror)
-   [YVR Enterprise](https://github.com/PlayForDreamDevelopers/com.yvr.enterprise-mirror)
-   [YVR Android-Device Core](https://github.com/PlayForDreamDevelopers/com.yvr.android-device.core-mirror)
-   [YVR Interaction](https://github.com/PlayForDreamDevelopers/com.yvr.interaction-mirror)
-   Play For Dream MR 设备
-   Play For Dream OS ENT 3.1.1 或更高版本
