using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Spring
    {
        public Particle a, b;

        public Spring(Particle input_a, Particle input_b)
        {
            a = input_a;
            b = input_b;
        }

        public static void DisplaceSprings(List<Spring> springs, float attenuation, float elastic)
        {
            foreach (Spring spring in springs)
            {
                DisplaceSpring(spring, attenuation, elastic);
            }
        }

        static void DisplaceSpring(Spring spring, float attenuation, float elastic)
        {
            Vector2D particle_a_position = spring.a.position;
            Vector2D particle_b_position = spring.b.position;

            Vector2D displacement = attenuation * elastic * (particle_b_position - particle_a_position);

            spring.a.displacement += displacement;
            spring.b.displacement -= displacement;
        }
    }
}
