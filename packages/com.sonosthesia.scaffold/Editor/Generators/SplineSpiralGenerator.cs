using Sonosthesia.Utils.Editor;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace Sonosthesia.Scaffold.Editor
{
    public class SplineSpiralGenerator : SplineGenerator
    {
        private const string REVOLUTIONS_FIELD = "RevolutionsField";
        private const string RADIUS_FIELD = "RadiusField";
        private const string HEIGHT_FIELD = "HeightField";
        
        private IntegerField _revolutionsField;
        private CurveField _radiusField;
        private CurveField _heightField;
        
        public override bool Setup(VisualElement generatorRoot)
        {
            bool success = generatorRoot.TryGetElementByName(REVOLUTIONS_FIELD, out _revolutionsField) &&
                   generatorRoot.TryGetElementByName(RADIUS_FIELD, out _radiusField) &&
                   generatorRoot.TryGetElementByName(HEIGHT_FIELD, out _heightField);

            if (!success)
            {
                return false;
            }

            _revolutionsField.RegisterValueChangedCallback(_ => RefreshRequest());
            _radiusField.RegisterValueChangedCallback(_ => RefreshRequest());
            _heightField.RegisterValueChangedCallback(_ => RefreshRequest());

            return true;
        }

        protected override Spline GenerateSpline()
        {
            int revolutions = _revolutionsField.value;
            AnimationCurve radius = _radiusField.value;
            AnimationCurve offset = _heightField.value;
            
            Spline spline = new Spline();

            CreateAutoSpline(spline, revolutions, radius, offset);

            return spline;
        }
        
        private const float alpha = 0.5f * math.PI;
        private readonly float cosAlpha = math.cos(alpha);
        private readonly float sinAlpha = math.sin(alpha);
        
        // approximation, the maths the work out the tangents and exact rotations with variable height and radius is 
        // really tricky...
        public static void CreateAutoSpline(Spline spline, int revolutions, AnimationCurve radiusCurve, AnimationCurve heightCurve)
        {
            int points = revolutions * 4 + 1;
            float curveStep = 1f / (revolutions * 4);
            float curve = 0f;

            for (int p = 0; p < points; p++, curve += curveStep)
            {
                float radius = radiusCurve.Evaluate(curve);
                float height = heightCurve.Evaluate(curve);

                Vector3 heightDiff = Vector3.up * (p > 0 ? 
                    height - radiusCurve.Evaluate(curve - curveStep) : 
                    radiusCurve.Evaluate(curve + curveStep) - height);

                int mod = p % 4;

                float x = mod switch
                {
                    0 => -1f,
                    1 => 0f,
                    2 => 1f,
                    3 => 0f,
                    _ => 0f
                };
                
                float z = mod switch
                {
                    0 => 0f,
                    1 => 1f,
                    2 => 0f,
                    3 => -1f,
                    _ => 0f
                };

                quaternion rotation = mod switch
                {
                    0 => quaternion.LookRotation(radius * Vector3.forward + heightDiff, Vector3.up),
                    1 => quaternion.LookRotation(radius * Vector3.right + heightDiff, Vector3.up),
                    2 => quaternion.LookRotation(radius * Vector3.back + heightDiff, Vector3.up),
                    3 => quaternion.LookRotation(radius * Vector3.left + heightDiff, Vector3.up),
                    _ => quaternion.identity,
                };
                
                Debug.Log($"{nameof(SplineSpiralGenerator)} adding point {p} at curve {curve} radius {radius} height {height}");
                
                BezierKnot knot = new BezierKnot( new float3(radius * x, height, radius * z), float3.zero, float3.zero, rotation);

                spline.Add(knot, TangentMode.AutoSmooth);                
            }
        }
        
        public static void CreateHelixFirstRevolution(Spline spline, float3 radii, float3 heights)
        {
            // this code imported from the factory makes the assumption that revolutions are the same radius and height which we don't want
            
            int revolutions = 0;
            float height = 0;
            float radius = 0;
            
            var revHeight = height / revolutions;

            var p = revHeight / (2f * math.PI);
            var ax = radius * math.cos(alpha);
            var az = radius * math.sin(alpha);
            var b = p * alpha * (radius - ax) * (3f * radius - ax) / (az * (4f * radius - ax) * math.tan(alpha));
            
            var yOffset = revHeight * 0.25f;
            var p0 = new float3(ax,  -alpha * p + yOffset, -az);
            var p1 = new float3((4f * radius - ax) / 3f, -b + yOffset, -(radius - ax) * (3f * radius - ax) / (3f * az));
            var p2 = new float3((4f * radius - ax) / 3f, b + yOffset, (radius - ax) * (3f * radius - ax) / (3f * az)); 
            var p3 = new float3(ax, alpha * p + yOffset, az);

            // Create the first two points and tangents forming the first half of the helix.
            var tangent = p1 - p0;
            var tangentLength = math.length(tangent);
            var tangentNorm = math.normalize(tangent);
            var normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p0, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));
            tangent = p3 - p2;
            tangentNorm = math.normalize(tangent);
            normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p3, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));

            // Rotate and offset the first half to form a full single revolution helix.
            var rotation = quaternion.AxisAngle(math.up(), math.radians(180f));
            yOffset = revHeight * 0.5f;
            p3 = math.rotate(rotation, p3);
            p3.y += yOffset;
            tangent = p1 - p0;
            tangentNorm = math.normalize(tangent);
            normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p3, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));
        }
        
        
        public static Spline CreateHelix(float radius, float height, int revolutions)
        {
            revolutions = math.max(1, revolutions);
            var revHeight = height / revolutions;
            var alpha = 0.5f * math.PI;
            var p = revHeight / (2f * math.PI);
            var ax = radius * math.cos(alpha);
            var az = radius * math.sin(alpha);
            var b = p * alpha * (radius - ax) * (3f * radius - ax) / (az * (4f * radius - ax) * math.tan(alpha));
            
            var yOffset = revHeight * 0.25f;
            var p0 = new float3(ax,  -alpha * p + yOffset, -az);
            var p1 = new float3((4f * radius - ax) / 3f, -b + yOffset, -(radius - ax) * (3f * radius - ax) / (3f * az));
            var p2 = new float3((4f * radius - ax) / 3f, b + yOffset, (radius - ax) * (3f * radius - ax) / (3f * az)); 
            var p3 = new float3(ax, alpha * p + yOffset, az);

            Spline spline = new Spline();

            // Create the first two points and tangents forming the first half of the helix.
            var tangent = p1 - p0;
            var tangentLength = math.length(tangent);
            var tangentNorm = math.normalize(tangent);
            var normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p0, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));
            tangent = p3 - p2;
            tangentNorm = math.normalize(tangent);
            normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p3, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));

            // Rotate and offset the first half to form a full single revolution helix.
            var rotation = quaternion.AxisAngle(math.up(), math.radians(180f));
            yOffset = revHeight * 0.5f;
            p3 = math.rotate(rotation, p3);
            p3.y += yOffset;
            tangent = p1 - p0;
            tangentNorm = math.normalize(tangent);
            normal = math.cross(math.cross(tangentNorm, math.up()), tangentNorm);
            spline.Add(new BezierKnot(p3, new float3(0f, 0f, -tangentLength),  new float3(0f, 0f, tangentLength), quaternion.LookRotation(tangentNorm, normal)));
            
            // Create knots for remaining revolutions
            var revYOffset = new float3(0f, revHeight, 0f);
            for (int i = 1; i < revolutions; ++i)
            {
                var knotA = spline[^1];
                knotA.Position += revYOffset;
                var knotB = spline[^2];
                knotB.Position += revYOffset;
                
                spline.Add(knotB);
                spline.Add(knotA);
            }
            
            return spline;
        }
    }
}