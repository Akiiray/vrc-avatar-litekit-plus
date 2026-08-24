using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    [CustomEditor(typeof(LiteKitParameterCompressor))]
    internal sealed class LiteKitParameterCompressorInspector : UnityEditor.Editor
    {
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
            EditorGUILayout.HelpBox("NDMF Build時に同期Float / Intを共有チャンネルで時分割送信します。元アセットは変更しません。", MessageType.Info);
            EditorGUILayout.PropertyField(enableCompression, new GUIContent("圧縮を有効にする"));

            var excluded = ReadExcluded();
            var provisional = CompressionPlan.Create(scanned, excluded, 1);
            var sliderMax = Math.Max(1, provisional.MaxUsefulChannelCount);
            syncChannelCount.intValue = EditorGUILayout.IntSlider(new GUIContent("同期チャンネル数"),
                Mathf.Clamp(syncChannelCount.intValue, 1, sliderMax), 1, sliderMax);
            EditorGUILayout.HelpBox("リモートへの同時送信数です。増やすと反映が速くなりますが、bit消費が増えます。", MessageType.None);
            serializedObject.ApplyModifiedProperties();

            if (descriptor == null)
                EditorGUILayout.HelpBox("このComponentはVRCAvatarDescriptorがあるAvatar Rootで使用してください。", MessageType.Error);

            DrawCompatibilityMessages();
            DrawPreview(CompressionPlan.Create(scanned, ReadExcluded(), syncChannelCount.intValue));

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameter一覧", EditorStyles.boldLabel);
            if (GUILayout.Button("再スキャン", GUILayout.Width(90))) Rescan();
            EditorGUILayout.EndHorizontal();
            DrawParameterList();
            EditorGUILayout.HelpBox("プレビュー値はEditor上で取得できるParameterに基づく推定です。NDMF Build時の最終Parameter構成によって結果が異なる場合があります。Build時の結果を正としてください。", MessageType.Info);
        }

        private void DrawPreview(CompressionPlan plan)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("圧縮結果プレビュー", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"対象: Float {plan.FloatCount}個 + Int {plan.IntCount}個 = {plan.Targets.Count}個");
            if (plan.Targets.Count == 0)
            {
                EditorGUILayout.HelpBox("圧縮対象の同期Float / Int Parameterはありません。", MessageType.Info);
                return;
            }
            EditorGUILayout.LabelField($"圧縮前: {plan.OriginalBits}bit");
            EditorGUILayout.LabelField($"圧縮後: {plan.CompressedBits}bit");
            var percent = plan.OriginalBits == 0 ? 0f : plan.SavedBits * 100f / plan.OriginalBits;
            EditorGUILayout.LabelField($"節約: {plan.SavedBits}bit ({percent:0.0}%)");
            EditorGUILayout.LabelField($"同期チャンネル: {plan.EffectiveChannelCount} ch / Batch数: {plan.Batches.Count}");
            EditorGUILayout.LabelField($"同期遅延（最大一周）: 約 {plan.CycleSeconds:0.00}秒");
            if (plan.SavedBits <= 0) EditorGUILayout.HelpBox("圧縮によるbit削減効果がないため、Build時は処理しません。", MessageType.Warning);
            if (plan.Batches.Count > CompressionConstants.MaximumBatchIndex)
                EditorGUILayout.HelpBox("Parameter Compressorで扱えるBatch数の上限を超えています。同期チャンネル数を増やすか、圧縮対象を減らしてください。", MessageType.Error);
            if (plan.CycleSeconds > 3f) EditorGUILayout.HelpBox("同期遅延が大きくなります。リモート側への反映に影響する可能性があります。", MessageType.Warning);
            else if (plan.CycleSeconds > 1f) EditorGUILayout.HelpBox("同期チャンネル数を増やすと遅延を短縮できます。", MessageType.Info);
            if (plan.Targets.Any(p => p.Saved)) EditorGUILayout.HelpBox("saved Parameterが含まれています。ワールド移動やAvatar読み込み直後は、リモート側への値の再同期に時間がかかる場合があります。saved設定自体は維持されます。", MessageType.Info);

            var currentBits = scanned.Where(p => p.NetworkSynced).Sum(p => p.BitCost);
            var finalBits = Math.Max(0, currentBits - plan.OriginalBits + plan.CompressedBits);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameter使用量 (256bit)", EditorStyles.boldLabel);
            DrawBudgetBar(finalBits, plan.CompressedBits);
            EditorGUILayout.LabelField($"圧縮後 {finalBits}bit / 256bit（削減 {plan.SavedBits}bit）");
            if (finalBits > CompressionConstants.ParameterBudgetBits)
                EditorGUILayout.HelpBox("圧縮後もParameter予算 256bit を超過しています。", MessageType.Error);
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
            GUI.Label(rect, "  非圧縮（青） / 圧縮Channel（橙） / 空き", EditorStyles.miniLabel);
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
                    EditorGUILayout.LabelField($"{p.Name}  [{p.Type} / {p.BitCost}bit / {(p.NetworkSynced ? "Synced" : "Local")} / {(p.Saved ? "Saved" : "Not Saved")}]", EditorStyles.miniLabel);
                    var eligible = p.NetworkSynced && p.Type != VRCExpressionParameters.ValueType.Bool &&
                                   !ParameterExclusionRules.IsAutomaticallyExcluded(p.Name, null);
                    using (new EditorGUI.DisabledScope(!eligible))
                    {
                        var excluded = ContainsExcluded(p.Name);
                        var nextExcluded = GUILayout.Toggle(excluded, excluded ? "圧縮除外" : "圧縮対象", "Button", GUILayout.Width(80));
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
                EditorGUILayout.HelpBox("MenuBuilderの圧縮機能が検出されました。Parameter Compressorとの併用は推奨されません。MenuBuilder側の圧縮設定をOFFにしてください。", MessageType.Warning);
            if (scanned.Any(p => ParameterExclusionRules.IsVrcFurySync(p.Name)))
                EditorGUILayout.HelpBox("VRCFuryの同期用と思われるParameterを検出しました。これらは圧縮対象から自動的に除外します。", MessageType.Info);
            if (scanned.Any(p => ParameterExclusionRules.IsFaceTracking(p.Name)))
                EditorGUILayout.HelpBox("Face Tracking Parameterが検出されました。高頻度で更新されるため、Parameter Compressorの圧縮対象から自動的に除外されます。", MessageType.Info);
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
