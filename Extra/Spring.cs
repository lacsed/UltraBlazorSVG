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

        static void DisplaceSpring(Spring spring, float attn, float elastic)
        {
            Vector2D prtAPos = spring.a.position;
            Vector2D prtBPos = spring.b.position;

            float dist = (prtBPos - prtAPos).Length();
            float force = (1 - attn) * elastic * dist;

            Vector2D forceDir = (prtBPos - prtAPos).Normalized();

            spring.a.displacement += force * forceDir;
            spring.b.displacement -= force * forceDir;
        }
    }
}
