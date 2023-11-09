using UnityEngine;

namespace Sonosthesia.Builder
{
    public interface ITubeGenerator
    {
        void Execute<S>(int i, S stream) where S : struct, IMeshStreams;
    
        int PathResolution { get; set; }
        
        int SliceResolution { get; set; }
    
        int VertexCount { get; }
	
        int IndexCount { get; }
    
        int JobLength { get; }
    
        Bounds Bounds { get; }
    }
}