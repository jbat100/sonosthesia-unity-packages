using UnityEngine;

namespace Sonosthesia.Mesh
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshController : MonoBehaviour
    {
        private UnityEngine.Mesh m_Mesh;

        protected virtual string MeshName => "ProceduralMesh";
        
        public UnityEngine.Mesh Mesh
        {
            get
            {
                if (m_Mesh != null)
                {
                    return m_Mesh;
                }

                m_Mesh = new UnityEngine.Mesh { name = MeshName };
                GetComponent<MeshFilter>().mesh = m_Mesh;
                return m_Mesh;
            }
        }
    }
}