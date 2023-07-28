using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public enum TransitionType
    {
        State = 1,
        Transition = 2,
        Auto = 3
    }

    public class SMTransition
    {
        public TransitionType type;
        public Particle origin;
        public Particle destination;
        public string name;

        public SMTransition(TransitionType inType, Particle inOrigin, Particle inDestination, string inName)
        {
            type = inType;
            origin = inOrigin;
            destination = inDestination;
            name = inName;
        }
    }
}
