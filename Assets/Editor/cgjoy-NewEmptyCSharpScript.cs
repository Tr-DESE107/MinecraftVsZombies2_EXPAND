using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PrefabScreenshotTool : EditorWindow
{
    private string sourceFolderPath = "Assets";
    private string targetFolderPath = "Assets/Screenshots";
    private Vector2Int screenshotSize = new Vector2Int(512, 512);
    private Color backgroundColor = Color.gray;
    private Camera renderCamera;
    private RenderTexture renderTexture;
    private bool includeSubfolders = true;
    private bool captureFront = true;
    private bool captureBack = true;
    private bool captureLeft = false;
    private bool captureRight = false;
    private bool captureTop = false;
    private bool captureBottom = false;
    private Vector2 scrollPosition;
    private Rect sourceDropArea;
    private Rect targetDropArea;
    private bool isDraggingToSource = false;
    private bool isDraggingToTarget = false;

    [MenuItem("Tools/预制体截图工具")]
    public static void ShowWindow()
    {
        GetWindow<PrefabScreenshotTool>("预制体截图工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("预制体截图工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 源文件夹设置 - 支持拖拽
        EditorGUILayout.LabelField("源文件夹设置", EditorStyles.boldLabel);

        // 拖拽区域
        Rect dropAreaRect = EditorGUILayout.BeginVertical("Box");
        GUIStyle dropAreaStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        dropAreaStyle.normal.textColor = isDraggingToSource ? Color.white : Color.gray;
        dropAreaStyle.alignment = TextAnchor.MiddleCenter;
        dropAreaStyle.padding = new RectOffset(10, 10, 10, 10);

        GUI.Box(dropAreaRect, GUIContent.none);

        GUILayout.Label("将文件夹或预制体拖拽到此处", dropAreaStyle);
        GUILayout.Label("或使用下方的路径输入框", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        // 处理拖拽事件
        HandleDropEvents(dropAreaRect, ref isDraggingToSource, true);

        // 路径输入区域
        EditorGUILayout.BeginHorizontal();
        sourceFolderPath = EditorGUILayout.TextField("预制体文件夹路径", sourceFolderPath);

        // 浏览按钮
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择预制体文件夹", sourceFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                sourceFolderPath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        EditorGUILayout.EndHorizontal();

        // 显示当前选择的文件/文件夹信息
        if (!string.IsNullOrEmpty(sourceFolderPath))
        {
            string displayPath = sourceFolderPath;
            if (displayPath.StartsWith("Assets/"))
            {
                displayPath = displayPath.Substring(7);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("当前选择:", EditorStyles.miniLabel, GUILayout.Width(60));

            // 检查是文件还是文件夹
            if (File.Exists(sourceFolderPath))
            {
                // 如果是文件，显示文件名
                string fileName = Path.GetFileName(sourceFolderPath);
                GUILayout.Label($"文件: {fileName}", EditorStyles.miniLabel);
            }
            else if (Directory.Exists(sourceFolderPath))
            {
                // 如果是文件夹，显示文件夹名
                string folderName = Path.GetFileName(sourceFolderPath);
                if (string.IsNullOrEmpty(folderName))
                {
                    folderName = "根目录";
                }
                GUILayout.Label($"文件夹: {folderName}", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // 目标文件夹设置 - 支持拖拽
        EditorGUILayout.LabelField("保存文件夹设置", EditorStyles.boldLabel);

        // 拖拽区域
        Rect targetDropAreaRect = EditorGUILayout.BeginVertical("Box");
        GUIStyle targetDropAreaStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        targetDropAreaStyle.normal.textColor = isDraggingToTarget ? Color.white : Color.gray;
        targetDropAreaStyle.alignment = TextAnchor.MiddleCenter;
        targetDropAreaStyle.padding = new RectOffset(10, 10, 10, 10);

        GUI.Box(targetDropAreaRect, GUIContent.none);

        GUILayout.Label("将文件夹拖拽到此处", targetDropAreaStyle);
        GUILayout.Label("或使用下方的路径输入框", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        // 处理拖拽事件
        HandleDropEvents(targetDropAreaRect, ref isDraggingToTarget, false);

        // 路径输入区域
        EditorGUILayout.BeginHorizontal();
        targetFolderPath = EditorGUILayout.TextField("截图保存路径", targetFolderPath);

        // 浏览按钮
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择保存文件夹", targetFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                targetFolderPath = "Assets" + path.Replace(Application.dataPath, "");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 截图设置
        EditorGUILayout.LabelField("截图设置", EditorStyles.boldLabel);
        screenshotSize = EditorGUILayout.Vector2IntField("截图尺寸(宽×高)", screenshotSize);
        backgroundColor = EditorGUILayout.ColorField("背景颜色", backgroundColor);
        includeSubfolders = EditorGUILayout.Toggle("包含子文件夹", includeSubfolders);

        EditorGUILayout.Space();

        // 拍摄角度选择
        EditorGUILayout.LabelField("拍摄角度选择", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("Box");
        captureFront = EditorGUILayout.Toggle("正面视图", captureFront);
        captureBack = EditorGUILayout.Toggle("背面视图", captureBack);
        captureLeft = EditorGUILayout.Toggle("左侧视图", captureLeft);
        captureRight = EditorGUILayout.Toggle("右侧视图", captureRight);
        captureTop = EditorGUILayout.Toggle("顶部视图", captureTop);
        captureBottom = EditorGUILayout.Toggle("底部视图", captureBottom);
        EditorGUILayout.EndVertical();

        // 快速选择按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全选"))
        {
            captureFront = captureBack = captureLeft = captureRight = captureTop = captureBottom = true;
        }
        if (GUILayout.Button("全不选"))
        {
            captureFront = captureBack = captureLeft = captureRight = captureTop = captureBottom = false;
        }
        if (GUILayout.Button("仅正背面"))
        {
            captureFront = captureBack = true;
            captureLeft = captureRight = captureTop = captureBottom = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 信息显示
        int selectedAnglesCount = GetSelectedAnglesCount();
        EditorGUILayout.HelpBox($"截图尺寸: {screenshotSize.x}×{screenshotSize.y}\n" +
                               $"选中角度: {selectedAnglesCount}个\n" +
                               $"源文件夹: {sourceFolderPath}\n" +
                               $"保存文件夹: {targetFolderPath}", MessageType.Info);

        EditorGUILayout.Space();

        // 执行按钮
        GUI.enabled = !string.IsNullOrEmpty(sourceFolderPath) &&
                     !string.IsNullOrEmpty(targetFolderPath) &&
                     selectedAnglesCount > 0;

        if (GUILayout.Button("生成预制体截图", GUILayout.Height(30)))
        {
            GeneratePrefabScreenshots();
        }
        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }

    private void HandleDropEvents(Rect dropArea, ref bool isDragging, bool isSource)
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                // 检查拖拽的内容
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    string path = DragAndDrop.paths[0];

                    // 如果是源文件夹区域，接受文件和文件夹
                    // 如果是目标文件夹区域，只接受文件夹
                    if (isSource || Directory.Exists(path))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        isDragging = true;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            // 处理拖拽的内容
                            if (isSource)
                            {
                                // 源文件夹：如果是文件，取其所在文件夹；如果是文件夹，直接用
                                if (File.Exists(path))
                                {
                                    // 如果是Unity资产，转换为相对路径
                                    if (path.StartsWith(Application.dataPath))
                                    {
                                        sourceFolderPath = "Assets" + path.Substring(Application.dataPath.Length);

                                        // 如果是预制体文件，取其所在文件夹
                                        if (Path.GetExtension(sourceFolderPath).ToLower() == ".prefab")
                                        {
                                            sourceFolderPath = Path.GetDirectoryName(sourceFolderPath);
                                        }
                                    }
                                    else
                                    {
                                        // 非项目内文件，取其目录
                                        sourceFolderPath = Path.GetDirectoryName(path);
                                    }
                                }
                                else if (Directory.Exists(path))
                                {
                                    // 文件夹
                                    if (path.StartsWith(Application.dataPath))
                                    {
                                        sourceFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                                    }
                                    else
                                    {
                                        sourceFolderPath = path;
                                    }
                                }
                            }
                            else
                            {
                                // 目标文件夹：只接受文件夹
                                if (Directory.Exists(path))
                                {
                                    if (path.StartsWith(Application.dataPath))
                                    {
                                        targetFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                                    }
                                    else
                                    {
                                        targetFolderPath = path;
                                    }
                                }
                            }

                            isDragging = false;
                            evt.Use();
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
                break;

            case EventType.DragExited:
                isDragging = false;
                break;
        }
    }

    private int GetSelectedAnglesCount()
    {
        int count = 0;
        if (captureFront) count++;
        if (captureBack) count++;
        if (captureLeft) count++;
        if (captureRight) count++;
        if (captureTop) count++;
        if (captureBottom) count++;
        return count;
    }

    private void GeneratePrefabScreenshots()
    {
        // 确保保存文件夹存在
        if (!Directory.Exists(targetFolderPath))
        {
            Directory.CreateDirectory(targetFolderPath);
        }

        List<string> validPrefabPaths = new List<string>();

        // 检查源路径是文件还是文件夹
        if (File.Exists(sourceFolderPath))
        {
            // 如果是单个文件，检查是否是预制体
            string extension = Path.GetExtension(sourceFolderPath).ToLower();
            if (extension == ".prefab")
            {
                validPrefabPaths.Add(sourceFolderPath);
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "选择的文件不是预制体文件(.prefab)", "确定");
                return;
            }
        }
        else if (Directory.Exists(sourceFolderPath))
        {
            // 如果是文件夹，查找所有预制体
            string searchPattern = "*.prefab";
            SearchOption searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            string[] prefabPaths = Directory.GetFiles(sourceFolderPath, searchPattern, searchOption);

            foreach (string path in prefabPaths)
            {
                if (path.StartsWith("Assets") || Path.IsPathRooted(path))
                {
                    validPrefabPaths.Add(path);
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("路径错误", "源路径不存在或无法访问", "确定");
            return;
        }

        if (validPrefabPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("未找到预制体", "在选定的路径中未找到预制体文件。", "确定");
            return;
        }

        // 设置相机和渲染纹理
        SetupCamera();

        int successCount = 0;
        int totalCount = validPrefabPaths.Count;
        int totalScreenshots = totalCount * GetSelectedAnglesCount();

        // 处理每个预制体
        for (int i = 0; i < validPrefabPaths.Count; i++)
        {
            string prefabPath = validPrefabPaths[i];

            // 更新进度条
            EditorUtility.DisplayProgressBar("生成截图中",
                $"正在处理: {Path.GetFileNameWithoutExtension(prefabPath)} ({i + 1}/{totalCount})",
                (float)i / totalCount);

            if (GenerateScreenshotsForPrefab(prefabPath))
            {
                successCount++;
            }
        }

        // 清理
        CleanupCamera();
        EditorUtility.ClearProgressBar();

        // 刷新资源数据库以显示新截图
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("截图生成完成",
            $"成功处理 {successCount} 个预制体。\n生成了 {totalScreenshots} 张截图。", "确定");
    }

    private void SetupCamera()
    {
        // 创建相机游戏对象
        GameObject cameraGO = new GameObject("ScreenshotCamera");
        renderCamera = cameraGO.AddComponent<Camera>();

        // 配置相机
        renderCamera.orthographic = false;
        renderCamera.fieldOfView = 60f;
        renderCamera.nearClipPlane = 0.01f;
        renderCamera.farClipPlane = 1000f;
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = backgroundColor;

        // 创建渲染纹理
        renderTexture = new RenderTexture(screenshotSize.x, screenshotSize.y, 24);
        renderCamera.targetTexture = renderTexture;
    }

    private void CleanupCamera()
    {
        if (renderCamera != null && renderCamera.gameObject != null)
        {
            DestroyImmediate(renderCamera.gameObject);
        }
        if (renderTexture != null)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = null;
        }
    }

    private bool GenerateScreenshotsForPrefab(string prefabPath)
    {
        try
        {
            // 加载预制体
            GameObject prefab = null;

            if (prefabPath.StartsWith("Assets/"))
            {
                // Unity资产路径
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
            else
            {
                // 外部路径（可能来自拖拽的外部文件）
                Debug.LogWarning($"无法加载外部预制体: {prefabPath}。请确保预制体在项目Assets文件夹内。");
                return false;
            }

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                return false;
            }

            // 实例化预制体
            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            // 获取边界框用于相机定位
            Bounds bounds = CalculateBounds(instance);

            // 为选定的角度生成截图
            int screenshotsTaken = 0;

            if (captureFront)
            {
                PositionCameraForAngle(bounds, "front");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "front");
                screenshotsTaken++;
            }

            if (captureBack)
            {
                PositionCameraForAngle(bounds, "back");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "back");
                screenshotsTaken++;
            }

            if (captureLeft)
            {
                PositionCameraForAngle(bounds, "left");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "left");
                screenshotsTaken++;
            }

            if (captureRight)
            {
                PositionCameraForAngle(bounds, "right");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "right");
                screenshotsTaken++;
            }

            if (captureTop)
            {
                PositionCameraForAngle(bounds, "top");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "top");
                screenshotsTaken++;
            }

            if (captureBottom)
            {
                PositionCameraForAngle(bounds, "bottom");
                renderCamera.Render();
                SaveRenderTextureToFile(prefab.name, "bottom");
                screenshotsTaken++;
            }

            // 清理实例
            DestroyImmediate(instance);

            Debug.Log($"为 {prefab.name} 生成了 {screenshotsTaken} 张截图");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"为 {prefabPath} 生成截图时出错: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private Bounds CalculateBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(target.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private void PositionCameraForAngle(Bounds bounds, string angle)
    {
        Vector3 cameraPosition = bounds.center;
        Vector3 lookAtPosition = bounds.center;

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = objectSize * 1.5f; // 增加距离以获得更好的视角

        switch (angle.ToLower())
        {
            case "front":
                cameraPosition += Vector3.forward * distance;
                break;
            case "back":
                cameraPosition += Vector3.back * distance;
                break;
            case "left":
                cameraPosition += Vector3.left * distance;
                break;
            case "right":
                cameraPosition += Vector3.right * distance;
                break;
            case "top":
                cameraPosition += Vector3.up * distance;
                break;
            case "bottom":
                cameraPosition += Vector3.down * distance;
                break;
        }

        renderCamera.transform.position = cameraPosition;
        renderCamera.transform.LookAt(lookAtPosition);
    }

    private void SaveRenderTextureToFile(string prefabName, string angleName)
    {
        // 清理文件名（移除非法字符）
        string cleanPrefabName = CleanFileName(prefabName);

        // 从渲染纹理创建Texture2D
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // 编码为PNG
        byte[] bytes = texture.EncodeToPNG();
        DestroyImmediate(texture);

        // 保存文件
        string filename = $"{cleanPrefabName}_{angleName}.png";
        string filepath = Path.Combine(targetFolderPath, filename);

        // 确保文件名唯一
        int counter = 1;
        while (File.Exists(filepath))
        {
            filename = $"{cleanPrefabName}_{angleName}_{counter}.png";
            filepath = Path.Combine(targetFolderPath, filename);
            counter++;
        }

        File.WriteAllBytes(filepath, bytes);
    }

    private string CleanFileName(string fileName)
    {
        // 移除文件名中的非法字符
        string invalidChars = new string(Path.GetInvalidFileNameChars());
        string invalidRegStr = string.Format("[{0}]", Regex.Escape(invalidChars));
        return Regex.Replace(fileName, invalidRegStr, "_");
    }
}