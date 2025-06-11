using System;
using System.Reflection;
using System.Text;
using MVZ2.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MVZ2.Models
{
    public class GraphicsManager : MonoBehaviour
    {
        public void Init()
        {
            LogGraphics();
            CheckSupportedColorFormat();
            CheckSupportedDepthFormat();
            Shader.SetGlobalInt("_LightStarted", 1);
            ResetLighting();
        }
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void OnInitailize()
        {
            Shader.SetGlobalColor("_LightBackground", Color.white);
            Shader.SetGlobalColor("_LightGlobal", Color.white);
        }
#endif
        public void SetLighting(Color background, Color global)
        {
            Shader.SetGlobalColor("_LightBackground", background);
            Shader.SetGlobalColor("_LightGlobal", global);
        }
        public void ResetLighting()
        {
            SetLighting(Color.white, Color.white);
        }
        public GraphicsFormat GetSupportedColorFormat()
        {
            if (confirmedColorFormat)
            {
                return supportedColorFormat;
            }
            return CheckSupportedColorFormat();
        }
        public GraphicsFormat GetSupportedDepthFormat()
        {
            if (confirmedDepthFormat)
            {
                return supportedDepthFormat;
            }
            return CheckSupportedDepthFormat();
        }
        private GraphicsFormat CheckSupportedColorFormat()
        {
            Debug.Log("Checking supported color formats...");
            var format = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            if (!SystemInfo.IsFormatSupported(format, FormatUsage.Render))
            {
                Debug.LogWarning("Cannot find a supported color format for device, using default color format.");
                return GraphicsFormat.R8G8B8A8_SRGB;
            }
            else
            {
                Debug.Log($"Found supported color format {format}.");
                confirmedColorFormat = true;
                supportedColorFormat = format;
                return format;
            }
        }
        private GraphicsFormat CheckSupportedDepthFormat()
        {
            Debug.Log("Checking supported depth formats...");
            var format = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
            if (!SystemInfo.IsFormatSupported(format, FormatUsage.Render))
            {
                Debug.LogWarning("Cannot find a supported depth format for device, using default depth format.");
                return GraphicsFormat.None;
            }
            else
            {
                Debug.Log($"Found supported depth format {format}.");
                confirmedDepthFormat = true;
                supportedDepthFormat = format;
                return format;
            }
        }
        private void LogGraphics()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Quality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");

            sb.AppendLine("系统信息：");
            sb.AppendLine($"deviceModel: {SystemInfo.deviceModel}");
            sb.AppendLine($"deviceType: {SystemInfo.deviceType}");
            sb.AppendLine($"operatingSystem: {SystemInfo.operatingSystem}");
            sb.AppendLine($"operatingSystemFamily: {SystemInfo.operatingSystemFamily}");
            sb.AppendLine($"processorCount: {SystemInfo.processorCount}");
            sb.AppendLine($"processorFrequency: {SystemInfo.processorFrequency}");
            sb.AppendLine($"processorType: {SystemInfo.processorType}");
            sb.AppendLine($"supportsAccelerometer: {SystemInfo.supportsAccelerometer}");
            sb.AppendLine($"supportsAudio: {SystemInfo.supportsAudio}");
            sb.AppendLine($"supportsGyroscope: {SystemInfo.supportsGyroscope}");
            sb.AppendLine($"supportsLocationService: {SystemInfo.supportsLocationService}");
            sb.AppendLine($"supportsVibration: {SystemInfo.supportsVibration}");
            sb.AppendLine($"systemMemorySize: {SystemInfo.systemMemorySize}");

            sb.AppendLine();
            sb.AppendLine("显示设备信息：");
            sb.AppendLine($"graphicsDeviceID: {SystemInfo.graphicsDeviceID}");
            sb.AppendLine($"graphicsDeviceName: {SystemInfo.graphicsDeviceName}");
            sb.AppendLine($"graphicsDeviceType: {SystemInfo.graphicsDeviceType}");
            sb.AppendLine($"graphicsDeviceVendor: {SystemInfo.graphicsDeviceVendor}");
            sb.AppendLine($"graphicsDeviceVendorID: {SystemInfo.graphicsDeviceVendorID}");
            sb.AppendLine($"graphicsDeviceVersion: {SystemInfo.graphicsDeviceVersion}");
            sb.AppendLine($"graphicsMemorySize: {SystemInfo.graphicsMemorySize}");
            sb.AppendLine($"graphicsMultiThreaded: {SystemInfo.graphicsMultiThreaded}");
            sb.AppendLine($"graphicsShaderLevel: {SystemInfo.graphicsShaderLevel}");
            sb.AppendLine($"graphicsUVStartsAtTop: {SystemInfo.graphicsUVStartsAtTop}");
            sb.AppendLine($"maxGraphicsBufferSize: {SystemInfo.maxGraphicsBufferSize}");
            sb.AppendLine($"supportsGraphicsFence: {SystemInfo.supportsGraphicsFence}");
            sb.AppendLine($"renderingThreadingMode: {SystemInfo.renderingThreadingMode}");
            sb.AppendLine($"hasHiddenSurfaceRemovalOnGPU: {SystemInfo.hasHiddenSurfaceRemovalOnGPU}");
            sb.AppendLine($"hasDynamicUniformArrayIndexingInFragmentShaders: {SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders}");
            sb.AppendLine($"supportsShadows: {SystemInfo.supportsShadows}");
            sb.AppendLine($"supportsRawShadowDepthSampling: {SystemInfo.supportsRawShadowDepthSampling}");
            sb.AppendLine($"supportsMotionVectors: {SystemInfo.supportsMotionVectors}");
            sb.AppendLine($"supports3DTextures: {SystemInfo.supports3DTextures}");
            sb.AppendLine($"supports2DArrayTextures: {SystemInfo.supports2DArrayTextures}");
            sb.AppendLine($"supports3DRenderTextures: {SystemInfo.supports3DRenderTextures}");
            sb.AppendLine($"supportsCubemapArrayTextures: {SystemInfo.supportsCubemapArrayTextures}");
            sb.AppendLine($"copyTextureSupport: {SystemInfo.copyTextureSupport}");
            sb.AppendLine($"supportsComputeShaders: {SystemInfo.supportsComputeShaders}");
            sb.AppendLine($"renderingThreadingMode: {SystemInfo.renderingThreadingMode}");
            sb.AppendLine($"supportsGeometryShaders: {SystemInfo.supportsGeometryShaders}");
            sb.AppendLine($"supportsTessellationShaders: {SystemInfo.supportsTessellationShaders}");
            sb.AppendLine($"supportsInstancing: {SystemInfo.supportsInstancing}");
            sb.AppendLine($"supportsHardwareQuadTopology: {SystemInfo.supportsHardwareQuadTopology}");
            sb.AppendLine($"supports32bitsIndexBuffer: {SystemInfo.supports32bitsIndexBuffer}");
            sb.AppendLine($"supportsSparseTextures: {SystemInfo.supportsSparseTextures}");
            sb.AppendLine($"supportedRenderTargetCount: {SystemInfo.supportedRenderTargetCount}");
            sb.AppendLine($"supportsSeparatedRenderTargetsBlend: {SystemInfo.supportsSeparatedRenderTargetsBlend}");
            sb.AppendLine($"supportedRandomWriteTargetCount: {SystemInfo.supportedRandomWriteTargetCount}");
            sb.AppendLine($"supportsMultisampledTextures: {SystemInfo.supportsMultisampledTextures}");
            sb.AppendLine($"supportsMultisampleAutoResolve: {SystemInfo.supportsMultisampleAutoResolve}");
            sb.AppendLine($"supportsTextureWrapMirrorOnce: {SystemInfo.supportsTextureWrapMirrorOnce}");
            sb.AppendLine($"usesReversedZBuffer: {SystemInfo.usesReversedZBuffer}");
            sb.AppendLine($"npotSupport: {SystemInfo.npotSupport}");
            sb.AppendLine($"maxTextureSize: {SystemInfo.maxTextureSize}");
            sb.AppendLine($"maxCubemapSize: {SystemInfo.maxCubemapSize}");
            sb.AppendLine($"maxComputeBufferInputsVertex: {SystemInfo.maxComputeBufferInputsVertex}");
            sb.AppendLine($"maxComputeBufferInputsFragment: {SystemInfo.maxComputeBufferInputsFragment}");
            sb.AppendLine($"maxComputeBufferInputsGeometry: {SystemInfo.maxComputeBufferInputsGeometry}");
            sb.AppendLine($"maxComputeBufferInputsDomain: {SystemInfo.maxComputeBufferInputsDomain}");
            sb.AppendLine($"maxComputeBufferInputsHull: {SystemInfo.maxComputeBufferInputsHull}");
            sb.AppendLine($"maxComputeBufferInputsCompute: {SystemInfo.maxComputeBufferInputsCompute}");
            sb.AppendLine($"maxComputeWorkGroupSize: {SystemInfo.maxComputeWorkGroupSize}");
            sb.AppendLine($"maxComputeWorkGroupSizeX: {SystemInfo.maxComputeWorkGroupSizeX}");
            sb.AppendLine($"maxComputeWorkGroupSizeY: {SystemInfo.maxComputeWorkGroupSizeY}");
            sb.AppendLine($"maxComputeWorkGroupSizeZ: {SystemInfo.maxComputeWorkGroupSizeZ}");
            sb.AppendLine($"supportsAsyncCompute: {SystemInfo.supportsAsyncCompute}");
            sb.AppendLine($"supportsGraphicsFence: {SystemInfo.supportsGraphicsFence}");
            sb.AppendLine($"supportsAsyncGPUReadback: {SystemInfo.supportsAsyncGPUReadback}");
            sb.AppendLine($"supportsRayTracing: {SystemInfo.supportsRayTracing}");
            sb.AppendLine($"supportsSetConstantBuffer: {SystemInfo.supportsSetConstantBuffer}");
            sb.AppendLine($"minConstantBufferOffsetAlignment: {SystemInfo.constantBufferOffsetAlignment}");
            sb.AppendLine($"hasMipMaxLevel: {SystemInfo.hasMipMaxLevel}");
            sb.AppendLine($"supportsMipStreaming: {SystemInfo.supportsMipStreaming}");
            sb.AppendLine($"usesLoadStoreActions: {SystemInfo.usesLoadStoreActions}");

            sb.Append($"supportedTextureFormats: ");
            var enumType = typeof(TextureFormat);
            foreach (var v in Enum.GetValues(enumType))
            {
                try
                {
                    var format = (TextureFormat)v;
                    if (!IsValidEnumValue(format))
                        continue;
                    if (SystemInfo.SupportsTextureFormat(format))
                    {
                        sb.Append($"{format},");
                    }
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
            sb.AppendLine();
            sb.Append($"supportedRenderTextureFormats: ");
            foreach (var v in Enum.GetValues(typeof(RenderTextureFormat)))
            {
                try
                {
                    var format = (RenderTextureFormat)v;
                    if (!IsValidEnumValue(format))
                        continue;
                    if (SystemInfo.SupportsRenderTextureFormat(format))
                    {
                        sb.Append($"{format},");
                    }
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
            sb.AppendLine();
            sb.Append($"supportedGraphicsFormats: ");
            foreach (var v in Enum.GetValues(typeof(GraphicsFormat)))
            {
                try
                {
                    var format = (GraphicsFormat)v;
                    if (!IsValidEnumValue(format))
                        continue;
                    if (SystemInfo.IsFormatSupported(format, FormatUsage.Render))
                    {
                        sb.Append($"{format},");
                    }
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
            sb.AppendLine();
            Debug.Log(sb.ToString());
        }
        private bool IsValidEnumValue(Enum value)
        {
            var enumType = value.GetType();
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in fields)
            {
                if (object.Equals(fieldInfo.GetValue(null), value))
                {
                    return fieldInfo.GetCustomAttribute<ObsoleteAttribute>() == null;
                }
            }
            return false;
        }
        public MainManager Main => main;
        private bool confirmedColorFormat = false;
        private GraphicsFormat supportedColorFormat = GraphicsFormat.R8G8B8A8_SRGB;
        private bool confirmedDepthFormat = false;
        private GraphicsFormat supportedDepthFormat = GraphicsFormat.D32_SFloat_S8_UInt;
        [SerializeField]
        private MainManager main;
    }
}
