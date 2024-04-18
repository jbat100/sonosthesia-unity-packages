namespace Sonosthesia.Mesh
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ExtrusionSegment
    {
        public ExtrusionPoint start { get; set; }
        public ExtrusionPoint end { get; set; }
    }
}