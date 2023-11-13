using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace Sonosthesia.Mesh
{
    public struct GeoIcoSphere : IMeshGenerator
    {
        private struct Strip
        {
            public int id;
            public float3 lowLeftCorner, lowRightCorner, highLeftCorner, highRightCorner;
            public float3
                bottomLeftAxis, bottomRightAxis,
                midLeftAxis, midCenterAxis, midRightAxis,
                topLeftAxis, topRightAxis;
        }
        
        private static float EdgeRotationAngle => acos(dot(up(), GetCorner(0, 1)));
        
        private static float3 GetCorner (int id, int ySign) => float3(
            0.4f * sqrt(5f) * sin(0.2f * PI * id),
            ySign * 0.2f * sqrt(5f),
            -0.4f * sqrt(5f) * cos(0.2f * PI * id)
        );
        
        private static Strip GetStrip (int id) => id switch {
            0 => CreateStrip(0),
            1 => CreateStrip(1),
            2 => CreateStrip(2),
            3 => CreateStrip(3),
            _ => CreateStrip(4)
        };

        static Strip CreateStrip(int id)
        {
            Strip s = new Strip {
                id = id,
                lowLeftCorner = GetCorner(2 * id, -1),
                lowRightCorner = GetCorner(id == 4 ? 0 : 2 * id + 2, -1),
                highLeftCorner = GetCorner(id == 0 ? 9 : 2 * id - 1, 1),
                highRightCorner = GetCorner(2 * id + 1, 1)
            };
            s.bottomLeftAxis = normalize(cross(down(), s.lowLeftCorner));
            s.bottomRightAxis = normalize(cross(down(), s.lowRightCorner));
            s.midLeftAxis = normalize(cross(s.lowLeftCorner, s.highLeftCorner));
            s.midCenterAxis = normalize(cross(s.lowLeftCorner, s.highRightCorner));
            s.midRightAxis = normalize(cross(s.lowRightCorner, s.highRightCorner));
            s.topLeftAxis = normalize(cross(s.highLeftCorner, up()));
            s.topRightAxis = normalize(cross(s.highRightCorner, up()));
            return s;
        }
        
        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            int u = i / 5;
            Strip strip = GetStrip(i - 5 * u);
            int vi = ResolutionV * (Resolution * strip.id + u) + 2;
            int ti = 2 * ResolutionV * (Resolution * strip.id + u);
            bool firstColumn = u == 0;
            
            int4 quad = int4(
                vi,
                firstColumn ? strip.id : vi - ResolutionV,
                firstColumn ? strip.id == 0 ? 4 * ResolutionV * Resolution + 2 : vi - ResolutionV * (Resolution + u) : vi - ResolutionV + 1,
                vi + 1
            );
            
            u += 1;
            
            float3 columnBottomDir = strip.lowRightCorner - down();
            float3 columnBottomStart = down() + columnBottomDir * u / Resolution;
            float3 columnBottomEnd = strip.lowLeftCorner + columnBottomDir * u / Resolution;

            float3 columnLowDir = strip.highRightCorner - strip.lowLeftCorner;
            float3 columnLowStart = strip.lowRightCorner + columnLowDir * ((float)u / Resolution - 1f);
            float3 columnLowEnd = strip.lowLeftCorner + columnLowDir * u / Resolution;
            
            float3 columnHighDir = strip.highRightCorner - strip.lowLeftCorner;
            float3 columnHighStart = strip.lowLeftCorner + columnHighDir * u / Resolution;
            float3 columnHighEnd = strip.highLeftCorner + columnHighDir * u / Resolution;
            
            float3 columnTopDir = up() - strip.highLeftCorner;
            float3 columnTopStart = strip.highRightCorner + columnTopDir * ((float)u / Resolution - 1f);
            float3 columnTopEnd = strip.highLeftCorner + columnTopDir * u / Resolution;

            var vertex = new Vertex();
            if (i == 0) 
            {
                vertex.position = down();
                streams.SetVertex(0, vertex);
                vertex.position = up();
                streams.SetVertex(1, vertex);
            }

            vertex.position = normalize(columnBottomStart);
            streams.SetVertex(vi, vertex);
            
            vi += 1;
            
            for (int v = 1; v < ResolutionV; v++, vi++, ti += 2) 
            {
                if (v <= Resolution - u) 
                {
                    vertex.position = lerp(columnBottomStart, columnBottomEnd, (float)v / Resolution);
                }
                else if (v < Resolution) 
                {
                    vertex.position = lerp(columnLowStart, columnLowEnd, (float)v / Resolution);
                }
                else if (v <= ResolutionV - u) 
                {
                    vertex.position = lerp(columnHighStart, columnHighEnd, (float)v / Resolution - 1f);
                }
                else 
                {
                    vertex.position = lerp(columnTopStart, columnTopEnd, (float)v / Resolution - 1f);
                }
                vertex.position = normalize(vertex.position);
                streams.SetVertex(vi, vertex);
                streams.SetTriangle(ti + 0, quad.xyz);
                streams.SetTriangle(ti + 1, quad.xzw);
                
                quad.y = quad.z;
                quad += int4(1, 0, firstColumn && v <= Resolution - u ? ResolutionV : 1, 1);
            }
            
            if (!firstColumn) 
            {
                quad.z = ResolutionV * Resolution * (strip.id == 0 ? 5 : strip.id) - Resolution + u + 1;
            }
            quad.w = u < Resolution ? quad.z + 1 : 1;
            
            streams.SetTriangle(ti + 0, quad.xyz);
            streams.SetTriangle(ti + 1, quad.xzw);
        }

        private int ResolutionV => 2 * Resolution;
        
        public int VertexCount => 5 * ResolutionV * Resolution + 2;
        public int IndexCount =>  6 * 5 * ResolutionV * Resolution;
        public int JobLength =>  5 * Resolution;
        
        public int Resolution { get; set; }
        
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));

    }
}