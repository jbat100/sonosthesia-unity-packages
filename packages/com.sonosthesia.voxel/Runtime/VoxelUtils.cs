using System.Collections.Generic;
using UnityEngine;

using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

namespace Sonosthesia.Builder
{
    public static class VoxelUtils
    {
        public enum BlockSide
        {
            BOTTOM,
            TOP,
            LEFT,
            RIGHT,
            FRONT,
            BACK
        }

        public enum BlockType
        {
            GRASSTOP,
            GRASSSIDE,
            DIRT,
            WATER,
            STONE,
            SAND,
            AIR
        }
    

    public static Vector2[,] blockUVs = {
        /*GRASSTOP*/ {  new Vector2(0.125f, 0.375f), new Vector2(0.1875f,0.375f),
                        new Vector2(0.125f, 0.4375f), new Vector2(0.1875f,0.4375f) },
        /*GRASSSIDE*/ { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
        /*DIRT*/	  { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*WATER*/	  { new Vector2(0.875f,0.125f),  new Vector2(0.9375f,0.125f),
                        new Vector2(0.875f,0.1875f), new Vector2(0.9375f,0.1875f)},
        /*STONE*/	  { new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
                        new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},
        /*SAND*/	  { new Vector2(0.125f,0.875f),  new Vector2(0.1875f,0.875f),
                        new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)}
    };

    public static T FlatGet<T>(this T[] array, int width, int depth, int x, int y, int z)
    {
        return array[x + width * (y + depth * z)];
    }

    public static float FBM2(float x, float z, PerlinSettings settings)
    {
        return FBM2(x, z, settings.Octaves, settings.Scale, settings.HeightScale, settings.HeightOffset);
    }
    
    public static float FBM2(float x, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        float total = 0f;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2f;
        }
        return total + heightOffset;
    }

    public static float FBM3(float x, float y, float z, PerlinSettings settings)
    {
        return FBM3(x, y, z, settings.Octaves, settings.Scale, settings.HeightScale, settings.HeightOffset);   
    }

    public static float FBM3(float x, float y, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        float xy = FBM2(x, y, octaves, scale, heightScale, heightOffset);
        float yz = FBM2(y, z, octaves, scale, heightScale, heightOffset);
        float xz = FBM2(x, z, octaves, scale, heightScale, heightOffset);
        float yx = FBM2(y, x, octaves, scale, heightScale, heightOffset);
        float zy = FBM2(z, y, octaves, scale, heightScale, heightOffset);
        float zx = FBM2(z, x, octaves, scale, heightScale, heightOffset);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }

    public static Mesh MergeMeshes(Mesh[] meshes) {
        Mesh mesh = new Mesh();

        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> pointsHash = new HashSet<VertexData>();
        List<int> tris = new List<int>();

        int pIndex = 0;
        for (int i = 0; i < meshes.Length; i++) //loop through each mesh
        {
            if (meshes[i] == null) continue;
            for (int j = 0; j < meshes[i].vertices.Length; j++) //loop through each vertex of the current mesh
            {
                Vector3 v = meshes[i].vertices[j];
                Vector3 n = meshes[i].normals[j];
                Vector2 u = meshes[i].uv[j];
                VertexData p = new VertexData(v, n, u);
                if (!pointsHash.Contains(p)) {
                    pointsOrder.Add(p, pIndex);
                    pointsHash.Add(p);

                    pIndex++;
                }

            }

            for (int t = 0; t < meshes[i].triangles.Length; t++) {
                int triPoint = meshes[i].triangles[t];
                Vector3 v = meshes[i].vertices[triPoint];
                Vector3 n = meshes[i].normals[triPoint];
                Vector2 u = meshes[i].uv[triPoint];
                VertexData p = new VertexData(v, n, u);

                int index;
                pointsOrder.TryGetValue(p, out index);
                tris.Add(index);
            }
            meshes[i] = null;
        }

        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh) {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (VertexData v in list.Keys) {
            verts.Add(v.Item1);
            norms.Add(v.Item2);
            uvs.Add(v.Item3);
        }
        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
    }
    }
}