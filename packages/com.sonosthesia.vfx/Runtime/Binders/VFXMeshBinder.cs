using Sonosthesia.Mesh;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Sonosthesia.VFX
{
    [VFXBinder("MeshController/Mesh")]
    public class VFXMeshBinder : VFXBinderBase
    {
        public string MeshProperty 
        { 
            get => (string)m_MeshProperty;
            set => m_MeshProperty = value;
        }
        
        [VFXPropertyBinding("UnityEngine.Mesh"), SerializeField]
        protected ExposedProperty m_MeshProperty = "Mesh";

        public MeshController MeshController;

        public override bool IsValid(VisualEffect component)
        {
            return MeshController != null && component.HasMesh(MeshProperty);
        }

        public override void UpdateBinding(VisualEffect component)
        {
            component.SetMesh(MeshProperty, MeshController.Mesh);
        }
    }
}