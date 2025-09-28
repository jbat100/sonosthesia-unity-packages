using System;
using Sonosthesia.Interaction;

namespace Sonosthesia.Pointer
{
    [Serializable]
    public class FloatPointerExtractorSettings
    {
        public IExtractorSession<PointerEvent, float> MakeSession()
        {
            return null;
        }
    }
}