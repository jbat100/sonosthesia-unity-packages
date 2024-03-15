using UnityEngine;

namespace Sonosthesia.Mesh
{
    // source https://catlikecoding.com/unity/tutorials/procedural-meshes/
    
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