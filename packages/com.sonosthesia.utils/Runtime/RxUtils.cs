using UniRx;

namespace Sonosthesia.Utils
{
    public static class RxUtils
    {
        public static void Cleanup<T>(ref BehaviorSubject<T> subject)
        {
            if (subject == null)
            {
                return;
            }
            subject.OnCompleted();
            subject.Dispose();
            subject = null;
        }
        
        public static void Cleanup<T>(ref Subject<T> subject)
        {
            if (subject == null)
            {
                return;
            }
            subject.OnCompleted();
            subject.Dispose();
            subject = null;
        }

        public static void Toggle(this BoolReactiveProperty property)
        {
            if (property == null)
            {
                return;
            }

            property.Value = !property.Value;
        }
    }
}