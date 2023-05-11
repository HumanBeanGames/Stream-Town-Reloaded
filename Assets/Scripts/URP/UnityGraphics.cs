using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;
namespace URP 
{
    public static class UnityGraphics 
	{
        private static FieldInfo MainLightCastShadows_FieldInfo;
        private static FieldInfo AdditionalLightCastShadows_FieldInfo;
        private static FieldInfo MainLightShadowmapResolution_FieldInfo;
        private static FieldInfo AdditionalLightShadowmapResolution_FieldInfo;
        private static FieldInfo Cascade2Split_FieldInfo;
        private static FieldInfo Cascade4Split_FieldInfo;
        private static FieldInfo SoftShadowsEnabled_FieldInfo;

        static UnityGraphics()
        {
            var pipelineAssetType = typeof(UniversalRenderPipelineAsset);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            MainLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowsSupported", flags);
            AdditionalLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightShadowsSupported", flags);
            MainLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowmapResolution", flags);
            AdditionalLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightsShadowmapResolution", flags);
            Cascade2Split_FieldInfo = pipelineAssetType.GetField("m_Cascade2Split", flags);
            Cascade4Split_FieldInfo = pipelineAssetType.GetField("m_Cascade4Split", flags);
            SoftShadowsEnabled_FieldInfo = pipelineAssetType.GetField("m_SoftShadowsSupported", flags);
        }


        public static bool MainLightCastShadows
        {
            get => (bool)MainLightCastShadows_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => MainLightCastShadows_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static bool AdditionalLightCastShadows
        {
            get => (bool)AdditionalLightCastShadows_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => AdditionalLightCastShadows_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static ShadowResolution MainLightShadowResolution
        {
            get => (ShadowResolution)MainLightShadowmapResolution_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => MainLightShadowmapResolution_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static ShadowResolution AdditionalLightShadowResolution
        {
            get => (ShadowResolution)AdditionalLightShadowmapResolution_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => AdditionalLightShadowmapResolution_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static float Cascade2Split
        {
            get => (float)Cascade2Split_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => Cascade2Split_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static Vector3 Cascade4Split
        {
            get => (Vector3)Cascade4Split_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => Cascade4Split_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }

        public static bool SoftShadowsEnabled
        {
            get => (bool)SoftShadowsEnabled_FieldInfo.GetValue(GraphicsSettings.currentRenderPipeline);
            set => SoftShadowsEnabled_FieldInfo.SetValue(GraphicsSettings.currentRenderPipeline, value);
        }
    }
}