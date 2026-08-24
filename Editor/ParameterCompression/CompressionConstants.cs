namespace Akiiray.VRCAvatarLiteKitPlus.Editor.ParameterCompression
{
    internal static class CompressionConstants
    {
        internal const string SyncPrefix = "LiteKit_PC_Sync_";
        internal const string IndexParameter = SyncPrefix + "Index";
        internal const string FloatChannelPrefix = SyncPrefix + "Float_";
        internal const string IntChannelPrefix = SyncPrefix + "Int_";
        internal const string LayerName = "LiteKit Parameter Compressor [4ED03D94]";
        internal const float BatchHoldSeconds = 3f / 30f;
        internal const int MaximumBatchIndex = 255;
        internal const int ParameterBudgetBits = 256;
    }
}
