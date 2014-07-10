using System;

namespace Tools
{
    public class UniversalEventArgs<T> : EventArgs
    {
        public T Target { get; set; }

        public UniversalEventArgs(T a_target)
        {
            Target = a_target;
        }

        public UniversalEventArgs()
        {
            // TODO: Complete member initialization
        }
    }
}