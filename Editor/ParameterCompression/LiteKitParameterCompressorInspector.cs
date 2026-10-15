using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Akiiray.VRCAvatarLiteKitPlus.Editor.Localization;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    [CustomEditor(typeof(LiteKitParameterCompressor))]
    internal sealed class LiteKitParameterCompressorInspector : UnityEditor.Editor
    {
        private static string T(LiteKitTextKey key) => LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, key);
        private List<ScannedParameter> scanned = new List<ScannedParameter>();
        private readonly Dictionary<string, bool> sourceFoldouts = new Dictionary<string, bool>();
        private VRCAvatarDescriptor descriptor;
        private SerializedProperty enableCompression;
        private SerializedProperty syncChannelCount;
        private SerializedProperty excludedParameters;

        private void OnEnable()
        {
            enableCompression = serializedObject.FindProperty("enableCompression");
            syncChannelCount = serializedObject.FindProperty("syncChannelCount");
            excludedParameters = serializedObject.FindProperty("excludedParameters");
            Rescan();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("VRC Avatar LiteKit Parameter Compressor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(T(LiteKitTextKey.CompressionNotice), MessageType.Info);
            EditorGUILayout.PropertyField(enableCompression, new GUIContent(T(LiteKitTextKey.EnableCompression)));

            var excluded = ReadExcluded();
            var provisional = CompressionPlan.Create(scanned, excluded, 1);
            var sliderMax = Math.Max(1, provisional.MaxUsefulChannelCount);
            syncChannelCount.intValue = EditorGUILayout.IntSlider(new GUIContent(T(LiteKitTextKey.SyncChannelCount)),
                Mathf.Clamp(syncChannelCount.intValue, 1, sliderMax), 1, sliderMax);
            EditorGUILayout.HelpBox(T(LiteKitTextKey.ChannelNotice), MessageType.None);
            serializedObject.ApplyModifiedProperties();

            if (descriptor == null)
                EditorGUILayout.HelpBox(T(LiteKitTextKey.DescriptorRequired), MessageType.Error);

            DrawCompatibilityMessages();
            DrawPreview(CompressionPlan.Create(scanned, ReadExcluded(), syncChannelCount.intValue));

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(T(LiteKitTextKey.Parameters), EditorStyles.boldLabel);
            if (GUILayout.Button(T(LiteKitTextKey.Rescan), GUILayout.Width(90))) Rescan();
            EditorGUILayout.EndHorizontal();
            DrawParameterList();
            EditorGUILayout.HelpBox(LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, LiteKitTextKey.PreviewDisclaimer), MessageType.Info);
        }

        private void DrawPreview(CompressionPlan plan)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(T(LiteKitTextKey.CompressionPreview), EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionTargets), plan.FloatCount, plan.IntCount, plan.Targets.Count));
            if (plan.Targets.Count == 0)
            {
                EditorGUILayout.HelpBox(T(LiteKitTextKey.NoCompressionTargets), MessageType.Info);
                return;
            }
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionBefore), plan.OriginalBits));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionAfter), plan.CompressedBits));
            var percent = plan.OriginalBits == 0 ? 0f : plan.SavedBits * 100f / plan.OriginalBits;
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionSaved), plan.SavedBits, percent));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionBatches), plan.EffectiveChannelCount, plan.Batches.Count));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionCycle), plan.CycleSeconds));
            if (plan.SavedBits <= 0) EditorGUILayout.HelpBox(T(LiteKitTextKey.NoSavings), MessageType.Warning);
            if (plan.Batches.Count > CompressionConstants.MaximumBatchIndex)
                EditorGUILayout.HelpBox(T(LiteKitTextKey.BatchLimit), MessageType.Error);
            if (plan.CycleSeconds > 3f) EditorGUILayout.HelpBox(T(LiteKitTextKey.HighLatency), MessageType.Warning);
            else if (plan.CycleSeconds > 1f) EditorGUILayout.HelpBox(T(LiteKitTextKey.ChannelLatencyHint), MessageType.Info);
            if (plan.Targets.Any(p => p.Saved)) EditorGUILayout.HelpBox(T(LiteKitTextKey.SavedParameterNotice), MessageType.Info);

            var currentBits = scanned.Where(p => p.NetworkSynced).Sum(p => p.BitCost);
            var finalBits = Math.Max(0, currentBits - plan.OriginalBits + plan.CompressedBits);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(T(LiteKitTextKey.ParameterBudget), EditorStyles.boldLabel);
            DrawBudgetBar(finalBits, plan.CompressedBits);
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.FinalBudget), finalBits, plan.SavedBits));
            if (finalBits > CompressionConstants.ParameterBudgetBits)
                EditorGUILayout.HelpBox(T(LiteKitTextKey.BudgetExceeded), MessageType.Error);
        }

        private static void DrawBudgetBar(int finalBits, int channelBits)
        {
            var rect = GUILayoutUtility.GetRect(10, 20, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f));
            var nonCompressed = Math.Max(0, finalBits - channelBits);
            var nonWidth = rect.width * Mathf.Clamp01(nonCompressed / 256f);
            var channelWidth = rect.width * Mathf.Clamp01(channelBits / 256f);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, nonWidth, rect.height), new Color(0.35f, 0.62f, 0.85f));
            EditorGUI.DrawRect(new Rect(rect.x + nonWidth, rect.y, Mathf.Min(channelWidth, rect.width - nonWidth), rect.height), new Color(0.95f, 0.62f, 0.20f));
            GUI.Label(rect, T(LiteKitTextKey.BudgetLegend), EditorStyles.miniLabel);
        }

        private void DrawParameterList()
        {
            foreach (var group in scanned.GroupBy(p => p.Source))
            {
                if (!sourceFoldouts.ContainsKey(group.Key)) sourceFoldouts[group.Key] = true;
                sourceFoldouts[group.Key] = EditorGUILayout.Foldout(sourceFoldouts[group.Key], group.Key, true);
                if (!sourceFoldouts[group.Key]) continue;
                EditorGUI.indentLevel++;
                foreach (var p in group)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{p.Name}  [{p.Type} / {p.BitCost}bit / {(p.NetworkSynced ? T(LiteKitTextKey.Synced) : T(LiteKitTextKey.LocalParameter))} / {(p.Saved ? T(LiteKitTextKey.Saved) : T(LiteKitTextKey.NotSaved))}]", EditorStyles.miniLabel);
                    var eligible = p.NetworkSynced && p.Type != VRCExpressionParameters.ValueType.Bool &&
                                   !ParameterExclusionRules.IsAutomaticallyExcluded(p.Name, null);
                    using (new EditorGUI.DisabledScope(!eligible))
                    {
                        var excluded = ContainsExcluded(p.Name);
                        var nextExcluded = GUILayout.Toggle(excluded, excluded ? T(LiteKitTextKey.Excluded) : T(LiteKitTextKey.Included), "Button", GUILayout.Width(80));
                        if (eligible && nextExcluded != excluded) SetExcluded(p.Name, nextExcluded);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawCompatibilityMessages()
        {
            if (scanned.Any(p => ParameterExclusionRules.IsMenuBuilder(p.Name)))
                EditorGUILayout.HelpBox(T(LiteKitTextKey.MenuBuilderNotice), MessageType.Warning);
            if (scanned.Any(p => ParameterExclusionRules.IsVrcFurySync(p.Name)))
                EditorGUILayout.HelpBox(T(LiteKitTextKey.VrcFuryNotice), MessageType.Info);
            if (scanned.Any(p => ParameterExclusionRules.IsFaceTracking(p.Name)))
                EditorGUILayout.HelpBox(T(LiteKitTextKey.FaceTrackingNotice), MessageType.Info);
        }

        private void Rescan()
        {
            var compressor = target as LiteKitParameterCompressor;
            descriptor = compressor != null ? compressor.GetComponent<VRCAvatarDescriptor>() : null;
            scanned = ParameterScanner.Scan(descriptor);
            Repaint();
        }

        private HashSet<string> ReadExcluded()
        {
            var result = new HashSet<string>();
            for (var i = 0; i < excludedParameters.arraySize; i++) result.Add(excludedParameters.GetArrayElementAtIndex(i).stringValue);
            return result;
        }

        private bool ContainsExcluded(string name) => ReadExcluded().Contains(name);

        private void SetExcluded(string name, bool excluded)
        {
            serializedObject.Update();
            var values = ReadExcluded();
            if (excluded) values.Add(name); else values.Remove(name);
            excludedParameters.arraySize = values.Count;
            var index = 0;
            foreach (var value in values.OrderBy(v => v, StringComparer.Ordinal)) excludedParameters.GetArrayElementAtIndex(index++).stringValue = value;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
