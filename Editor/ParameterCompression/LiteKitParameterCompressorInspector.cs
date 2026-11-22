using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
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
        private bool generatedFoldout;
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
            EditorGUI.BeginChangeCheck();
            var channels = EditorGUILayout.IntSlider(new GUIContent(T(LiteKitTextKey.SyncChannelCount)),
                Mathf.Clamp(syncChannelCount.intValue, 1, sliderMax), 1, sliderMax);
            if (EditorGUI.EndChangeCheck()) syncChannelCount.intValue = channels;
            EditorGUILayout.HelpBox(T(LiteKitTextKey.ChannelNotice), MessageType.None);
            serializedObject.ApplyModifiedProperties();

            var plan = CompressionPlan.Create(scanned, excluded, syncChannelCount.intValue);
            DrawCompatibilityMessages();
            DrawPreview(plan, CompressionValidation.Check(descriptor, plan, enableCompression.boolValue));
            EditorGUILayout.HelpBox(T(LiteKitTextKey.CompressionExternalNotice), MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(T(LiteKitTextKey.Parameters), EditorStyles.boldLabel);
            if (GUILayout.Button(T(LiteKitTextKey.Rescan), GUILayout.Width(90))) Rescan();
            EditorGUILayout.EndHorizontal();
            DrawParameterList(excluded);
            EditorGUILayout.HelpBox(LiteKitLocalization.Get(LiteKitLocalization.CurrentLanguage, LiteKitTextKey.PreviewDisclaimer), MessageType.Info);
        }

        private void DrawPreview(CompressionPlan plan, CompressionValidation validation)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(T(LiteKitTextKey.CompressionPreview), EditorStyles.boldLabel);
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionTargetSummary),
                plan.FloatCount, plan.IntCount, scanned.Count - plan.Targets.Count));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionBefore), validation.OriginalBits));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionAfter), validation.FinalBits));
            var savings = validation.OriginalBits - validation.FinalBits;
            var percent = validation.OriginalBits == 0 ? 0f : savings * 100f / validation.OriginalBits;
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionSaved), savings, percent));
            foreach (var error in validation.Errors) EditorGUILayout.HelpBox(error, MessageType.Error);

            if (!enableCompression.boolValue) return;
            if (plan.Targets.Count == 0)
                EditorGUILayout.HelpBox(T(LiteKitTextKey.NoCompressionTargets), MessageType.Info);
            else if (plan.SavedBits <= 0)
                EditorGUILayout.HelpBox(T(LiteKitTextKey.NoSavings), MessageType.Warning);
            if (!plan.IsUseful) return;

            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionChannelSummary),
                plan.FloatChannelCount, plan.IntChannelCount, plan.Batches.Count));
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionCycle), plan.CycleSeconds));
            if (plan.CycleSeconds > 3f) EditorGUILayout.HelpBox(T(LiteKitTextKey.HighLatency), MessageType.Warning);
            else if (plan.CycleSeconds > 1f) EditorGUILayout.HelpBox(T(LiteKitTextKey.ChannelLatencyHint), MessageType.Info);
            if (plan.Targets.Any(p => p.Saved)) EditorGUILayout.HelpBox(T(LiteKitTextKey.SavedParameterNotice), MessageType.Info);

            DrawBudgetBar(validation.FinalBits, plan.CompressedBits);
            generatedFoldout = EditorGUILayout.Foldout(generatedFoldout, T(LiteKitTextKey.CompressionGeneratedPreview), true);
            if (!generatedFoldout) return;
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionGeneratedLayer), CompressionConstants.LayerName));
            foreach (var parameter in plan.GetSyncParameters())
                EditorGUILayout.LabelField($"{parameter.Name}  [{parameter.Type}]");

            var fx = ParameterScanner.GetFxController(descriptor);
            if (fx != null)
            {
                var existing = new HashSet<string>(fx.parameters.Select(parameter => parameter.name));
                foreach (var parameter in plan.GetRequiredAnimatorParameters())
                    if (!existing.Contains(parameter.Name))
                        EditorGUILayout.LabelField(string.Format(T(LiteKitTextKey.CompressionGeneratedFxParameter),
                            parameter.Name, parameter.Type));
            }
            EditorGUI.indentLevel--;
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

        private void DrawParameterList(HashSet<string> excludedNames)
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
                    var type = p.IsTrigger ? "Trigger" : p.Type.ToString();
                    EditorGUILayout.LabelField($"{p.Name}  [{type} / {p.BitCost} bit / {(p.NetworkSynced ? T(LiteKitTextKey.Synced) : T(LiteKitTextKey.LocalParameter))} / {(p.Saved ? T(LiteKitTextKey.Saved) : T(LiteKitTextKey.NotSaved))}]", EditorStyles.miniLabel);
                    var reason = ParameterExclusionRules.GetExclusion(p, excludedNames);
                    if (reason != null && reason != LiteKitTextKey.CompressionExcludedManual)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField(T(reason.Value), EditorStyles.miniLabel);
                        continue;
                    }
                    var excluded = reason == LiteKitTextKey.CompressionExcludedManual;
                    var label = excluded ? T(LiteKitTextKey.CompressionExcludedManual) : T(LiteKitTextKey.Included);
                    var nextExcluded = GUILayout.Toggle(excluded, label, "Button");
                    if (nextExcluded != excluded) SetExcluded(p.Name, nextExcluded);
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
