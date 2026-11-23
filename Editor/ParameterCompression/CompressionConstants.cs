namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class CompressionConstants
    {
        internal const string ReservedPrefix = "LiteKit_PC_";
        internal const string SyncPrefix = ReservedPrefix + "Sync_";
        internal const string TimingPath = "__LiteKit_PC_Timing__";
        internal const string IndexParameter = SyncPrefix + "Index";
        internal const string FloatChannelPrefix = SyncPrefix + "Float_";
        internal const string IntChannelPrefix = SyncPrefix + "Int_";
        internal const string LayerName = "LiteKit Parameter Compressor [4ED03D94]";
        internal const float BatchHoldSeconds = 3f / 30f;
        internal const int MaximumBatchIndex = 255;
        internal const int ParameterBudgetBits = 256;
    }
}
