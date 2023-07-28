using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Simulation
    {
        public List<SMTransition> transitions;
        public List<Particle> particles;
        public List<Spring> springs;

        public Simulation()
        {
            transitions = new List<SMTransition>();
            particles = new List<Particle>();
            springs = new List<Spring>();
        }

        public Simulation(List<SMTransition> inTransitions, List<Particle> inParticles, List<Spring> inSprings)
        {
            transitions = inTransitions;
            particles = inParticles;
            springs = inSprings;
        }

        public void Simulate()
        {
            float spacing = 100000000.0f;
            float elastic = 1000.0f;
            float stop_condition = 0.0025f;
            float attenuation = 0.0001f;
            float maximum_displacement = 10 * stop_condition;

            Particle.InitialPositioning(particles);

            while (maximum_displacement > stop_condition)
            {
                Particle.InteractParticles(particles, attenuation, spacing);

                Spring.DisplaceSprings(springs, attenuation, elastic);

                maximum_displacement = Particle.DisplaceParticles(particles);

                //IO.DrawSVG(file_path, file_name + count++, this);

                Particle.ResetDisplacement(particles);
            }
        }

        public List<Drawables> GetDrawables(SVGProperties properties)
        {
            List<Drawables> drawables = new List<Drawables>();

            foreach (Particle particle in particles)
            {
                drawables.AddRange(GenerateStateDrawables(particle, properties));
            }

            HashSet<SMTransition> processedTransitions = new HashSet<SMTransition>();

            foreach (SMTransition transition in transitions)
            {
                if (transition.type == TransitionType.Transition && !processedTransitions.Contains(transition))
                {
                    SMTransition secondTransition = transitions.Find(x =>
                        x.origin.name == transition.destination.name &&
                        x.destination.name == transition.origin.name &&
                        !processedTransitions.Contains(x));

                    if (secondTransition == null)
                    {
                        drawables.AddRange(GenerateStraightTransitionDrawables(transition.origin.position, transition.destination.position, transition.name, properties));
                    }
                    else
                    {
                        drawables.AddRange(GenerateCurvedTransitionDrawables(transition.origin.position, transition.destination.position, transition.name, properties));
                        drawables.AddRange(GenerateCurvedTransitionDrawables(secondTransition.origin.position, secondTransition.destination.position, secondTransition.name, properties));
                    }
                }
            }

            HashSet<SMTransition> processedAutoTransitions = new HashSet<SMTransition>();

            foreach (SMTransition transition in transitions.FindAll(x => x.type == TransitionType.Auto))
            {
                List<SMTransition> adjacentTransitions = transitions.FindAll(x => (x.origin.name == transition.origin.name || x.destination.name == transition.origin.name) && x.type != TransitionType.Auto);
                HashSet<SMTransition> processedAdjacent = new HashSet<SMTransition>();

                List<Vector2D> transitionDirections = new List<Vector2D>();

                foreach (SMTransition adjacentTransition in adjacentTransitions)
                {
                    if (processedAdjacent.Contains(adjacentTransition))
                        continue;

                    SMTransition twoAdjacent = adjacentTransitions.Find(x => x.origin.name == adjacentTransition.destination.name && x.destination.name == adjacentTransition.origin.name);

                    if (twoAdjacent != null)
                    {
                        float arcSize = 100.0f;

                        SMTransition exitingTransition = (adjacentTransition.origin.name == transition.origin.name) ? adjacentTransition : twoAdjacent;

                        Vector2D middlePosition = exitingTransition.origin.position.Middle(exitingTransition.destination.position);
                        Vector2D transitionDirection = (exitingTransition.destination.position - exitingTransition.origin.position).Normalized();
                        Vector2D perpendicularDirection = transitionDirection.Perpendicular();

                        Vector2D transitionControl1 = middlePosition + perpendicularDirection * arcSize;
                        Vector2D transitionControl2 = middlePosition - perpendicularDirection * arcSize;

                        Vector2D transitionOne = (transitionControl1 - exitingTransition.origin.position).Normalized();
                        Vector2D transitionTwo = (transitionControl2 - exitingTransition.origin.position).Normalized();

                        transitionDirections.Add(transitionOne);
                        transitionDirections.Add(transitionTwo);
                        processedAdjacent.Add(adjacentTransition);
                        processedAdjacent.Add(twoAdjacent);
                    }
                    else
                    {
                        Vector2D singleTransition = (adjacentTransition.destination.position - adjacentTransition.origin.position).Normalized();

                        if (adjacentTransition.origin.name == transition.origin.name)
                        {
                            transitionDirections.Add(singleTransition);
                        }
                        else
                        {
                            transitionDirections.Add(-singleTransition);
                        }

                        processedAdjacent.Add(adjacentTransition);
                    }
                }

                int count = transitionDirections.Count();

                Vector2D startVector = new Vector2D();
                Vector2D endVector = new Vector2D();
                float maximumSpace = 0;

                for (int i = 0; i < count; i++)
                {
                    Vector2D currentVector = transitionDirections[i];
                    Vector2D nextVector = transitionDirections[(i + 1) % count];

                    float space = currentVector.SignedAngleBetween(nextVector);

                    if (space > maximumSpace)
                    {
                        maximumSpace = space;

                        startVector = currentVector;
                        endVector = nextVector;
                    }
                }

                Vector2D autoDirection = startVector.Rotated(maximumSpace / 2);

                drawables.AddRange(GenerateAutoTransitionDrawables(autoDirection, transition.origin.position, transition.name, properties));
            }

            return drawables;
        }

        static List<Drawables> GenerateStateDrawables(Particle particle, SVGProperties properties)
        {
            Vector2D circleCenter = particle.position;

            Circle circle = new Circle(circleCenter, properties.stateRadius, particle.marked);

            Vector2D textPosition = circleCenter;

            Text text = new Text(textPosition, particle.name, Anchor.middle);

            return new List<Drawables> { circle, text };
        }

        static List<Drawables> GenerateCurvedTransitionDrawables(Vector2D start, Vector2D end, string transitionName, SVGProperties properties)
        {
            Vector2D middlePoint = start.Middle(end);
            Vector2D transitionDirection = (end - start).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D controlPoint = middlePoint + perpendicularDirection * properties.arcSize;
            Vector2D transitionOrigin = start + (controlPoint - start).Normalized() * properties.stateRadius;
            Vector2D transitionDestination = end + (controlPoint - end).Normalized() * (properties.stateRadius + properties.arrowLength);

            Curve transitionCurve = new Curve(transitionOrigin, transitionDestination, controlPoint);

            Vector2D tip = end + (controlPoint - end).Normalized() * properties.stateRadius;
            Vector2D basePoint = tip + (controlPoint - end).Normalized() * properties.arrowLength;
            Vector2D rightWing = basePoint + (controlPoint - end).Normalized().Perpendicular() * properties.arrowWidth / 2;
            Vector2D leftWing = basePoint - (controlPoint - end).Normalized().Perpendicular() * properties.arrowWidth / 2;

            Arrowhead arrowhead = new Arrowhead(tip, rightWing, leftWing);

            Vector2D textPosition = middlePoint + perpendicularDirection * (properties.arcSize + properties.textDistance);

            Text text = new Text(textPosition, transitionName, Text.GetAnchor(perpendicularDirection));

            return new List<Drawables> { transitionCurve, arrowhead, text };
        }

        static List<Drawables> GenerateAutoTransitionDrawables(Vector2D direction, Vector2D statePosition, string transitionName, SVGProperties properties)
        {
            Vector2D normalizedTransitionDirection = direction.Normalized();

            float arrowCoverageAngle = Vector2D.AngleBetween(properties.arrowLength, properties.autoRadius, properties.autoRadius);
            float stateToTransitionDistance = properties.autoRadius + properties.stateRadius - properties.autoRadius * properties.overlap;

            Vector2D transitionCenter = statePosition + normalizedTransitionDirection * stateToTransitionDistance;

            float angleToIntersectPoint = Vector2D.AngleBetween(properties.autoRadius, stateToTransitionDistance, properties.stateRadius);

            Vector2D initialIntersect = statePosition + properties.stateRadius * normalizedTransitionDirection.Rotated(angleToIntersectPoint);
            Vector2D finalIntersect = statePosition + properties.stateRadius * normalizedTransitionDirection.Rotated(-angleToIntersectPoint);

            Vector2D transitionCenterToFinalIntersect = (finalIntersect - transitionCenter).Normalized();

            Vector2D transitionEnd = transitionCenter + properties.autoRadius * transitionCenterToFinalIntersect.Rotated(arrowCoverageAngle);
            Vector2D transitionStart = initialIntersect;

            Arc arc = new Arc(transitionCenter, transitionStart, transitionEnd);

            Vector2D arrowTip = finalIntersect;
            Vector2D arrowDirection = (arrowTip - transitionEnd).Normalized();
            Vector2D basePoint = arrowTip - arrowDirection * properties.arrowLength;
            Vector2D rightWing = basePoint + arrowDirection.Perpendicular() * properties.arrowWidth / 2;
            Vector2D leftWing = basePoint - arrowDirection.Perpendicular() * properties.arrowWidth / 2;

            Arrowhead arrowhead = new Arrowhead(arrowTip, rightWing, leftWing);

            Vector2D textPosition = transitionCenter + normalizedTransitionDirection * (properties.textDistance + properties.autoRadius);

            Text text = new Text(textPosition, transitionName, Text.GetAnchor(normalizedTransitionDirection));

            return new List<Drawables> { arc, arrowhead, text };
        }

        static List<Drawables> GenerateStraightTransitionDrawables(Vector2D origin, Vector2D destination, string transitionName, SVGProperties properties)
        {
            Vector2D transitionDirection = (destination - origin).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D transitionOrigin = (origin + transitionDirection * properties.stateRadius);
            Vector2D transitionDestination = (destination - transitionDirection * properties.stateRadius);
            Vector2D lineEnd = transitionDestination - transitionDirection * properties.arrowLength;

            Line line = new Line(transitionOrigin, lineEnd);

            Vector2D tip = transitionDestination;
            Vector2D basePoint = tip - transitionDirection * properties.arrowLength;
            Vector2D rightWing = basePoint + perpendicularDirection * properties.arrowWidth / 2;
            Vector2D leftWing = basePoint - perpendicularDirection * properties.arrowWidth / 2;

            Arrowhead arrowhead = new Arrowhead(tip, rightWing, leftWing);

            Vector2D textPosition = origin.Middle(destination) + perpendicularDirection * properties.textDistance;

            Text text = new Text(textPosition, transitionName, Text.GetAnchor(perpendicularDirection));

            return new List<Drawables> { line, arrowhead, text };
        }

    }
}
