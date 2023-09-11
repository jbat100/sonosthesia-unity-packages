using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Block
    {
        public Mesh mesh;

        public Block(Vector3 offset, VoxelUtils.BlockType blockType)
        {
            Quad[] quads = new Quad[6];
            quads[0] = new Quad(VoxelUtils.BlockSide.BOTTOM, offset, blockType);
            quads[1] = new Quad(VoxelUtils.BlockSide.TOP, offset, blockType);
            quads[2] = new Quad(VoxelUtils.BlockSide.LEFT, offset, blockType);
            quads[3] = new Quad(VoxelUtils.BlockSide.RIGHT, offset, blockType);
            quads[4] = new Quad(VoxelUtils.BlockSide.FRONT, offset, blockType);
            quads[5] = new Quad(VoxelUtils.BlockSide.BACK, offset, blockType);

            Mesh[] sideMeshes = new Mesh[6];
            sideMeshes[0] = quads[0].mesh;
            sideMeshes[1] = quads[1].mesh;
            sideMeshes[2] = quads[2].mesh;
            sideMeshes[3] = quads[3].mesh;
            sideMeshes[4] = quads[4].mesh;
            sideMeshes[5] = quads[5].mesh;

            mesh = VoxelUtils.MergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }
}