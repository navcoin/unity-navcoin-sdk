using System.Runtime.InteropServices;

/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>
public static class WebGLPluginJS
{
    [DllImport("__Internal")]
    public static extern string Connect();

    [DllImport("__Internal")]
    public static extern string GetNFTCollections();
}