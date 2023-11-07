using UnityEngine;

namespace Sonosthesia.Builder
{
    public interface IMeshGenerator
    {
        void Execute<S>(int i, S stream) where S : struct, IMeshStreams;
        
        int Resolution { get; set; }
        
        int VertexCount { get; }
		
        int IndexCount { get; }
        
        int JobLength { get; }
        
        Bounds Bounds { get; }
    }
}