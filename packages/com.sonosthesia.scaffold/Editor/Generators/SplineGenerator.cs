using System;
using UniRx;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace Sonosthesia.Scaffold.Editor
{
    public abstract class SplineGenerator
    {
        private readonly Subject<Unit> _refreshRequestSubject = new Subject<Unit>();
        public IObservable<Unit> RefreshRequestObservable => _refreshRequestSubject.AsObservable();

        public abstract bool Setup(VisualElement generatorRoot);

        protected abstract Spline GenerateSpline();

        protected void RefreshRequest() => _refreshRequestSubject.OnNext(Unit.Default);

        public Spline Generate() => GenerateSpline();
    }
}