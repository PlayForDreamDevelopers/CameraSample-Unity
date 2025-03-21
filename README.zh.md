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

### 示例

### 追踪相机

![2025 03 18_103505244](https://github.com/user-attachments/assets/72805eef-d2f9-4248-a9e7-fe6d84149002)

此示例展示了如何获取 Play For Dream MR 设备的追踪相机信息，包括：

-   Tracking Master
-   Tracking Slave
-   Tracking Aux
-   Eye Tracking

    <img src="https://github.com/user-attachments/assets/b36a2b5d-9fbf-4a96-88bd-6ddf467db5af" alt="Camera" width="80%">

    <img src="https://github.com/user-attachments/assets/04df5b8f-90b8-424c-9d5f-b6ac7f70d97c" alt="Camera" width="80%">

在此示例中，要获取跟踪相机信息，首先需要在 `Camera Control` 面板左上角的下拉列表中选择目标相机类型，然后点击 `Open Tracking Camera` 按钮打开所选相机，然后可以使用 `Acquiring Camera Frame` 获取静态相机帧，使用 `Subscribe Camera Data` 获取相机流。

-   所有相机数据均为 `Y8` 格式。

## 要求

-   Unity 2022 3.58f1 或更高版本
-   [YVR Core](https://github.com/PlayForDreamDevelopers/com.yvr.core-mirror)
-   [YVR Utilities](https://github.com/PlayForDreamDevelopers/com.yvr.Utilities-mirror)
-   [YVR Enterprise](https://github.com/PlayForDreamDevelopers/com.yvr.enterprise-mirror)
-   [YVR Android-Device Core](https://github.com/PlayForDreamDevelopers/com.yvr.android-device.core-mirror)
-   [YVR Interaction](https://github.com/PlayForDreamDevelopers/com.yvr.interaction-mirror)
-   Play For Dream MR 设备
-   Play For Dream OS ENT 3.1.1 或更高版本
