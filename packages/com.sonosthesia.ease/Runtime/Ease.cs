using System;
using UnityEngine;

namespace Sonosthesia.Ease
{

	public interface ICurve<T> where T: struct
    {
	    void Evaluate(float t, out T value);
	    
	    float StartTime { get; }
	    float EndTime { get; }
    }

    public readonly struct CurveEndpoint<T> where T: struct
    {
	    public readonly float Time;
	    public readonly T Value;
    }
    
    public class EaseCurve<T> : ICurve<T> where T: struct
    {
	    private readonly CurveEndpoint<T> _start;
	    public CurveEndpoint<T> Start => _start;
	    public float StartTime => _start.Time;

	    private readonly CurveEndpoint<T> _end;
	    public CurveEndpoint<T> End => _end;
	    public float EndTime => _end.Time;

	    private readonly Func<float, float, float, float> _easingFunction;
	    private readonly Func<float, T, T, T> _lerpingFunction;
	    
	    public EaseCurve(EaseType easeType, CurveEndpoint<T> start, CurveEndpoint<T> end, Func<float, T, T, T> lerpingFunction)
	    {
		    _start = start;
		    _end = end;
		    _easingFunction = FloatEaseCurves.GetEasingFunction(easeType);
		    _lerpingFunction = lerpingFunction;
	    }

	    public void Evaluate(float t, out T value)
	    {
		    if (_lerpingFunction == null)
		    {
			    value = End.Value;
			    return;
		    }
		    float lerp = _easingFunction(StartTime, EndTime, t);
		    value = _lerpingFunction(lerp, _start.Value, _end.Value);
	    }
    }
    
    public enum EaseType 
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
    }


    
}