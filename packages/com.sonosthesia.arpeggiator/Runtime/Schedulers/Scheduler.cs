using System;
using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public abstract class Scheduler : MonoBehaviour
    {
        private enum TimingMode
        {
            None,
            Seconds,
            Beats
        }

        public interface ISession : IDisposable
        {
            // Thought about start stop but that invalidates Stream completion 
            
            /// <summary>
            /// Value is the normalized position (which can loop if specified)
            /// </summary>
            IObservable<float> Stream { get; }
        }
        
        [SerializeField] private bool _loop;

        // note this does not allow warping during session
        public ISession Session()
        {
            return CreateSession(_loop);
        }

        protected abstract ISession CreateSession(bool loop);

    }
}