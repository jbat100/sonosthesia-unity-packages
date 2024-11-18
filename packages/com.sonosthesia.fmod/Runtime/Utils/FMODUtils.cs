using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace Sonosthesia.FMOD
{
    public static class FMODUtils
    {
        public static bool HasParameter(this StudioEventEmitter emitter, string name)
        {
            if (emitter == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return emitter.EventReference.HasParameter(name);
        }
        
        public static bool HasParameter(this EventReference reference, string name)
        {
            RESULT result = RuntimeManager.StudioSystem.getEvent(reference.Path, out EventDescription description);
            return result == RESULT.OK && description.HasParameter(name);
        }
        
        public static bool HasParameter(this EventDescription description, string name)
        {
            if (description.isValid())
            {
                description.getParameterDescriptionCount(out int parameterCount);
                for (int i = 0; i < parameterCount; i++)
                {
                    description.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION parameterDescription);
                    if (parameterDescription.name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}