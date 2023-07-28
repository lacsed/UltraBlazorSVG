using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Elements
    {
        public List<Particle> state_particles;
        public List<Particle> transition_particles;
        public List<Particle> auto_loop_particles;
        public List<Particle> particles;
        public List<Spring> springs;

        public Elements(List<Particle> in_state_particles, List<Particle> in_transition_particles, List<Particle> in_auto_loop_particles, List<Particle> in_particles, List<Spring> in_springs)
        {
            state_particles = in_state_particles;
            transition_particles = in_transition_particles;
            auto_loop_particles = in_auto_loop_particles;
            particles = in_particles;
            springs = in_springs;
        }
    }
}
