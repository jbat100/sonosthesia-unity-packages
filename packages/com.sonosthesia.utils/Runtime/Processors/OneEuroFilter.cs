using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    // source https://github.com/keijiro/OneEuroFilter/blob/main/Assets/Script/OneEuroFilter.cs
    
    // it's a pain but code repetition seems the least bad option to have 1, 2 and 3 dimension versions
    
    public sealed class OneEuroFilter1
    {
        public float Beta { get; set; }
        public float MinCutoff { get; set; }

        public float Step(float t, float x)
        {
            var t_e = t - _prev.t;

            // Do nothing if the time difference is too small.
            if (t_e < 1e-5f) return _prev.x;

            var dx = (x - _prev.x) / t_e;
            var dx_res = math.lerp(_prev.dx, dx, Alpha(t_e, DCutOff));

            var cutoff = MinCutoff + Beta * math.length(dx_res);
            var x_res = math.lerp(_prev.x, x, Alpha(t_e, cutoff));

            _prev = (t, x_res, dx_res);

            return x_res;
        }

        const float DCutOff = 1.0f;

        static float Alpha(float t_e, float cutoff)
        {
            float r = 2 * math.PI * cutoff * t_e;
            return r / (r + 1);
        }
        
        private (float t, float x, float dx) _prev;
    }
    
    public sealed class OneEuroFilter2
    {
        public float Beta { get; set; }
        public float MinCutoff { get; set; }

        public float2 Step(float t, float2 x)
        {
            var t_e = t - _prev.t;

            // Do nothing if the time difference is too small.
            if (t_e < 1e-5f) return _prev.x;

            var dx = (x - _prev.x) / t_e;
            var dx_res = math.lerp(_prev.dx, dx, Alpha(t_e, DCutOff));

            var cutoff = MinCutoff + Beta * math.length(dx_res);
            var x_res = math.lerp(_prev.x, x, Alpha(t_e, cutoff));

            _prev = (t, x_res, dx_res);

            return x_res;
        }

        const float DCutOff = 1.0f;

        static float Alpha(float t_e, float cutoff)
        {
            float r = 2 * math.PI * cutoff * t_e;
            return r / (r + 1);
        }
        
        private (float t, float2 x, float2 dx) _prev;
    }
    
    public sealed class OneEuroFilter3
    {
        public float Beta { get; set; }
        public float MinCutoff { get; set; }

        public float3 Step(float t, float3 x)
        {
            var t_e = t - _prev.t;

            // Do nothing if the time difference is too small.
            if (t_e < 1e-5f) return _prev.x;

            var dx = (x - _prev.x) / t_e;
            var dx_res = math.lerp(_prev.dx, dx, Alpha(t_e, DCutOff));

            var cutoff = MinCutoff + Beta * math.length(dx_res);
            var x_res = math.lerp(_prev.x, x, Alpha(t_e, cutoff));

            _prev = (t, x_res, dx_res);

            return x_res;
        }

        const float DCutOff = 1.0f;

        static float Alpha(float t_e, float cutoff)
        {
            float r = 2 * math.PI * cutoff * t_e;
            return r / (r + 1);
        }
        
        private (float t, float3 x, float3 dx) _prev;
    }
}