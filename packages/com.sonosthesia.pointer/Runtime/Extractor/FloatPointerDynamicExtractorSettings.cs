using System;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [Serializable]
    public class FloatPointerDynamicExtractorSettings : FloatDynamicExtractorSettings<PointerEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Pressure,
            Scroll,
            Raycast,
            Screen
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private PointerRaycastSpace _space = PointerRaycastSpace.Target;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;

        protected override bool BypassFollow => _extractorType is ExtractorType.Constant;
        
        protected override IDynamicExtractorSession<PointerEvent, float> MakeRawSession()
        {
            IDynamicExtractorSession<PointerEvent, float> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Pressure => new PressureSession(),
                ExtractorType.Scroll => new ScrollSession(_axes),
                ExtractorType.Raycast => new RaycastSession(_axes, _space),
                ExtractorType.Screen => new ScreenSession(_axes),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return session;
        }
        
        private class PressureSession : StatelessExtractorSession<PointerEvent, float>
        {
            protected override bool Extract(PointerEvent e, out float value)
            {
                value = e.Data.pressure;
                return true;
            }
        }

        private class ScrollSession : StatelessExtractorSession<PointerEvent, float>
        {
            private readonly Axes _axes;
            private Vector2 _cumulativeScroll = Vector2.zero;
            
            public ScrollSession(Axes axes)
            {
                _axes = axes;
            }

            protected override bool Extract(PointerEvent e, out float value)
            {
                _cumulativeScroll += e.Data.scrollDelta;
                value = _cumulativeScroll.FilterAxes(_axes).magnitude;
                return true;
            }
        }

        private class ScreenSession : StatelessExtractorSession<PointerEvent, float>
        {
            private readonly Axes _axes;

            public ScreenSession(Axes axes)
            {
                _axes = axes;
            }
            
            protected override bool Extract(PointerEvent e, out float value) => e.ExtractScreen(_axes, out value);
        }

        private class RaycastSession : StatelessExtractorSession<PointerEvent, float>
        {
            private readonly Axes _axes;
            private readonly PointerRaycastSpace _space;

            public RaycastSession(Axes axes, PointerRaycastSpace space)
            {
                _axes = axes;
                _space = space;
            }
            
            protected override bool Extract(PointerEvent e, out float value) => e.ExtractRaycast(_space, _axes, out value);
        }
    }
}