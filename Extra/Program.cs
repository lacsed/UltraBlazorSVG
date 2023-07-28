using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using UltraDES;

namespace Extra
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Start the stopwatch before the code you want to measure
            stopwatch.Start();

            FSM(out var plants);

            Simulation simulation = IO.AutomatonInput(plants);

            simulation.Simulate();

            SVGProperties properties = new SVGProperties(
                textDistance: 25.0f,
                textColor: "black",
                textSize: 25.0f,
                strokeColor: "black",
                strokeFill: "none",
                strokeWidth: 1.5f,
                stateRadius: 40.0f,
                markedRatio: 0.8f,
                arrowLength: 10.0f,
                arrowWidth: 10.0f,
                arrowColor: "blue",
                overlap: 0.2f,
                autoRadius: 25.0f,
                arcSize: 100.0f
            );

            List<Drawables> drawables = simulation.GetDrawables(properties);

            string svgString = IO.GenerateSVGFromDrawables(drawables, properties);

            string file_path = @"C:\Users\Paulo\source\repos\Simulation\bin\Debug";
            string file_name = "OutputSVG";

            IO.WriteTextFile(file_path, file_name, svgString);

            stopwatch.Stop();

            // Get the elapsed time
            TimeSpan elapsedTime = stopwatch.Elapsed;

            // Output the elapsed time
            Console.WriteLine("Elapsed Time: " + elapsedTime);

            // You can also access the elapsed time in different formats
            Console.WriteLine("Elapsed Time (Milliseconds): " + elapsedTime.TotalMilliseconds);
            Console.WriteLine("Elapsed Time (Seconds): " + elapsedTime.TotalSeconds);
            Console.WriteLine("Elapsed Time (Minutes): " + elapsedTime.TotalMinutes);

            //(List<Particle>, List<Spring>) arguments = IO.AutomatonInput(plants);
            //List<Particle> particles = arguments.Item1;
            //List<Spring> springs = arguments.Item2;

            //float spacing = 100000000.0f;
            //float elastic = 1000.0f;
            //float stop_condition = 0.25f;
            //float attenuation = 0.00001f;
            //float average_displacement;

            //string file_name = "OutputSVG";

            //InitialPositioning(particles);

            //do
            //{
            //    /*
            //     * Particle's loop
            //     * 
            //     * Responsible for simulating the repelling forces between
            //     * all the particles in the system.
            //     * The force is directed from particle a to particle b,
            //     * therefore it has to be inverted before being applied to a.
            //     */
            //    Particle.InteractParticles(particles, attenuation, spacing);

            //    /*
            //     * Spring's loop
            //     * 
            //     * Responsible for simulating the attracting forces between the 
            //     * particles due to the springs between them.
            //     * The force is directed from a to b. But, since it is attracting in nature,
            //     * it has to be inverted for b and added normally for a.
            //     */
            //    Spring.DisplaceSprings(springs, attenuation, elastic);

            //    average_displacement = Particle.DisplaceParticles(particles);

            //    Particle.ResetDisplacement(particles);

            //} while (average_displacement > stop_condition);

            //IO.DrawSVG(Directory.GetCurrentDirectory(), file_name, particles, springs);

            //foreach (Particle particle in particles)
            //{
            //    Console.WriteLine("x: " + particle.position.x + " y: " + particle.position.y);
            //}

            //Console.WriteLine(Directory.GetCurrentDirectory());
        }

        private static void FSM(out DeterministicFiniteAutomaton plants)
        {
            var s = new List<State>(); // or State[] s = new State[6];
            for (var i = 0; i < 10; i++)
                s.Add(i == 0 ? new State(i.ToString(), Marking.Marked) : new State(i.ToString(), Marking.Unmarked));

            // Creating Events (0 to 100)
            var e = new List<Event>(); // or Event[] e = new Event[100];
            for (var i = 0; i < 100; ++i)
                e.Add(i % 2 != 0
                    ? new Event($"e{i}", Controllability.Controllable)
                    : new Event($"e{i}", Controllability.Uncontrollable));

            var c2 = new DeterministicFiniteAutomaton(
                new[]
                {
                    new Transition(s[0], e[99], s[1]),
                    new Transition(s[1], e[96], s[0]),
                    new Transition(s[0], e[94], s[0]),
                    new Transition(s[0], e[45], s[2]),
                    new Transition(s[1], e[97], s[2])
                },
                s[0], "C2");

            plants = c2;
        }
    }
}
