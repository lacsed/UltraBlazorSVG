using System;
using System.Collections.Generic;

namespace Extra
{
    public class Particle
    {
        public Vector2D position;
        public Vector2D displacement;
        public string name;
        public bool marked;
        public Guid id;

        public Particle()
        {
            position = new Vector2D();
            name = "";
            marked = false;
            displacement = new Vector2D();
            id = Guid.NewGuid();
        }

        public Particle(string input_name, bool isMarked)
        {
            position = new Vector2D();
            name = input_name;
            marked = isMarked;
            displacement = new Vector2D();
            id = Guid.NewGuid();
        }

        public Particle(Vector2D inputPosition, string inputName, bool inputMarked)
        {
            position = inputPosition;
            name = inputName;
            marked = inputMarked;
            displacement = new Vector2D();
            id = Guid.NewGuid();
        }

        /*
        * Particle's loop
        * 
        * Responsible for simulating the repelling forces between
        * all the particles in the system.
        * The force is directed from particle a to particle b,
        * therefore it has to be inverted before being applied to a.
        */
        public static void InteractParticles(List<Particle> particles, float attenuation, float spacing)
        {
            for (int i = 0; i < particles.Count - 1; i++)
            {
                for (int j = i + 1; j < particles.Count; j++)
                {
                    particles[i].InteractParticle(particles[j], attenuation, spacing);
                }
            }
        }

        void InteractParticle(Particle prtB, float atten, float spacing)
        {
            Vector2D dispDir = (prtB.position - this.position).Normalized();
            float prtDist = (prtB.position - this.position).Length();

            if (prtDist < 1e-6)
            {
                prtDist = 1e-6f;
            }

            float force = (1 - atten) * spacing / prtDist;

            prtB.displacement += force * dispDir;
            this.displacement -= force * dispDir;
        }


        public static void InitialPositioning(List<Particle> particles)
        {
            float initial_radius = 10.0f;

            for (int i = 0; i < particles.Count; i++)
            {
                float step_angle = (float) (2 * Math.PI / particles.Count);
                Particle current_particle = particles[i];
                current_particle.position.x = (float) (initial_radius * Math.Cos(i * step_angle));
                current_particle.position.y = (float) (initial_radius * Math.Sin(i * step_angle));
            }
        }

        public static float DisplaceParticles(List<Particle> particles)
        {
            float maximumDisplacement = 0.0f;
            foreach (Particle particle in particles)
            {
                float currentDisplacement = particle.displacement.Length();

                if (currentDisplacement > maximumDisplacement)
                    maximumDisplacement = currentDisplacement;

                particle.Displace();
            }

            return maximumDisplacement;
        }


        void Displace(Vector2D input_displacement)
        {
            this.position += input_displacement;
        }

        void Displace()
        {
            this.position += displacement;
        }

        public static void ResetDisplacement(List<Particle> particles)
        {
            foreach (Particle particle in particles)
            {
                particle.displacement.Reset();
            }
        }
    }
}
