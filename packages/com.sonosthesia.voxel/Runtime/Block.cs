using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Block
    {
        public Mesh mesh;
        public Chunk parentChunk;
        
        public Block(Vector3 offset, VoxelUtils.BlockType blockType, Chunk chunk)
        {
            parentChunk = chunk;
            Vector3 blockLocalPos = offset - chunk.location;
            
            if (blockType == VoxelUtils.BlockType.AIR)
            {
                return;
            }
            
            List<Quad> quads = new List<Quad>();
            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y - 1, (int) blockLocalPos.z))
            {
                if (blockType == VoxelUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(VoxelUtils.BlockSide.BOTTOM, offset, VoxelUtils.BlockType.DIRT));  
                else 
                    quads.Add(new Quad(VoxelUtils.BlockSide.BOTTOM, offset, blockType));
            }
            if (!HasSolidNeighbour((int) blockLocalPos.x, (int) blockLocalPos.y + 1, (int) blockLocalPos.z))
            {
                if (blockType == VoxelUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(VoxelUtils.BlockSide.TOP, offset, VoxelUtils.BlockType.GRASSTOP));  
                else 
                    quads.Add(new Quad(VoxelUtils.BlockSide.TOP, offset, blockType));  
            }
            if (!HasSolidNeighbour((int)blockLocalPos.x - 1, (int)blockLocalPos.y, (int)blockLocalPos.z))
                quads.Add(new Quad(VoxelUtils.BlockSide.LEFT, offset, blockType));
            if (!HasSolidNeighbour((int)blockLocalPos.x + 1, (int)blockLocalPos.y, (int)blockLocalPos.z))
                quads.Add(new Quad(VoxelUtils.BlockSide.RIGHT, offset, blockType));
            if (!HasSolidNeighbour((int)blockLocalPos.x, (int)blockLocalPos.y, (int)blockLocalPos.z + 1))
                quads.Add(new Quad(VoxelUtils.BlockSide.FRONT, offset, blockType));
            if (!HasSolidNeighbour((int)blockLocalPos.x, (int)blockLocalPos.y, (int)blockLocalPos.z - 1))
                quads.Add(new Quad(VoxelUtils.BlockSide.BACK, offset, blockType));

            if (quads.Count == 0)
            {
                return;
            }

            Mesh[] sideMeshes = new Mesh[quads.Count];
            for (int i = 0; i < quads.Count; i++)
            {
                sideMeshes[i] = quads[i].mesh;
            }

            mesh = VoxelUtils.MergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }

        public bool HasSolidNeighbour(int x, int y, int z)
        {
            if (x < 0 || x >= parentChunk.width ||
                y < 0 || y >= parentChunk.height ||
                z < 0 || z >= parentChunk.depth)
            {
                return false;
            }

            return parentChunk.ChunkData.FlatGet(parentChunk.width, parentChunk.depth, x, y, z) != VoxelUtils.BlockType.AIR 
                   && parentChunk.ChunkData.FlatGet(parentChunk.width, parentChunk.depth, x, y, z) != VoxelUtils.BlockType.WATER;
        }
    }
}