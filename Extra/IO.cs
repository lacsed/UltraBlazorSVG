using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UltraDES;
using System.Linq;

namespace Extra
{
    public class IO
    {
        public static Simulation AutomatonInput(DeterministicFiniteAutomaton input_automaton)
        {
            List<Particle> particles = new List<Particle>();
            List<Spring> springs = new List<Spring>();
            List<SMTransition> transitions = new List<SMTransition>();

            foreach (AbstractState state in input_automaton.States)
            {
                Particle current_particle = new Particle(state.ToString(), state.IsMarked);
                particles.Add(current_particle);
            }

            foreach (Transition transition in input_automaton.Transitions)
            {
                Particle particle_a = particles.Find(x => x.name == transition.Origin.ToString());
                Particle particle_b = particles.Find(x => x.name == transition.Destination.ToString());

                SMTransition existingTransition = transitions.Find(x => x.origin.name == particle_a.name && x.destination.name == particle_b.name);

                if (particle_a.name == particle_b.name)
                {
                    if (existingTransition == null)
                    {
                        SMTransition newTransition = new SMTransition(TransitionType.Auto, particle_a, particle_b, transition.Trigger.ToString());
                        transitions.Add(newTransition);
                    }
                    else
                    {
                        existingTransition.name += "," + transition.Trigger.ToString();
                    }
                }
                else
                {
                    if (existingTransition == null)
                    {
                        SMTransition returningTransition = transitions.Find(x => x.destination.name == particle_a.name && x.origin.name == particle_b.name);
                        SMTransition newTransition = new SMTransition(TransitionType.Transition, particle_a, particle_b, transition.Trigger.ToString());
                        Spring newSpring = new Spring(particle_a, particle_b);

                        if (returningTransition == null)
                            springs.Add(newSpring);

                        transitions.Add(newTransition);
                    }
                    else
                    {
                        existingTransition.name += "," + transition.Trigger.ToString();
                    }
                }
            }

            Simulation simulation = new Simulation(transitions, particles, springs);

            return simulation;
        }

        public static string GenerateSVGFromDrawables(List<Drawables> drawables, SVGProperties properties)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            DrawableLimits canvasLimits = new DrawableLimits();

            foreach (Drawables drawable in drawables)
            {
                canvasLimits = canvasLimits.Compare(drawable.Limits());
            }

            float canvasWidth = canvasLimits.maxX - canvasLimits.minX + 300;
            float canvasHeight = canvasLimits.maxY - canvasLimits.minY + 300;
            float canvasOriginX = (canvasLimits.maxX + canvasLimits.minX) / 2;
            float canvasOriginY = (canvasLimits.maxY + canvasLimits.minY) / 2;
            Vector2D canvasOrigin = new Vector2D(canvasOriginX - canvasWidth / 2, canvasOriginY + canvasHeight / 2);

            string svg = "";

            foreach (Drawables drawable in drawables)
            {
                svg += drawable.ConvertToSvg(canvasOrigin, properties);
            }

            string marker = "<defs><marker id=\"arrowhead\" markerWidth=\"10\" markerHeight=\"7\" refX = \"7\" refY = \"3.5\" orient = \"auto\" ><polygon points=\"0 0, 10 3.5, 0 7\" /></marker></defs>" + Environment.NewLine;

            return "<svg height=\"" + canvasHeight + "\" width=\"" + canvasWidth + "\">" + Environment.NewLine + marker + svg + " </svg>";
        }

        public static void WriteTextFile(string path, string file_name, string text)
        {
            StreamWriter sw = File.CreateText(path + file_name + ".html");
            sw.Write(text);
            sw.Close();
        }

        // Convert the particles and springs from a simulation into an SVG file.
        public static void DrawSVG(string path, string file_name, Simulation simulation)
        {
            // This line of code sets the current culture to invariant culture, which ensures that
            // decimal points are represented by commas instead of periods. This is important when
            // generating SVG files or any other data format that requires a specific format for
            // numerical values. The invariant culture ensures that the formatting is consistent
            // across different cultures and systems.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            List<Spring> auto_springs = new List<Spring>();

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

            // Canvas properties
            float maximumX = 0.0f;
            float minimumX = 0.0f;
            float maximumY = 0.0f;
            float minimumY = 0.0f;
            float canvasWidth;
            float canvasHeight;

            // SVG elements
            string svgStates = "";
            string svgTransitions = "";

            // Create the file and handler where the SVG will be written.
            StreamWriter sw = File.CreateText(path + file_name + ".html");

            // Determine the extreme values of x and y for the particles.
            foreach (Particle particle in simulation.particles)
            {
                Vector2D particle_position = particle.position;

                if (maximumX < particle_position.x)
                    maximumX = particle_position.x;
                if (minimumX > particle_position.x)
                    minimumX = particle_position.x;
                if (maximumY < particle_position.y)
                    maximumY = particle_position.y;
                if (minimumY > particle_position.y)
                    minimumY = particle_position.y;
            }

            // Determine the canvas dimensions.
            canvasHeight = maximumY - minimumY + 15 * properties.stateRadius;
            canvasWidth = maximumX - minimumX + 15 * properties.stateRadius;

            float canvas_origin_x = (maximumX + minimumX) / 2;
            float canvas_origin_y = (maximumY + minimumY) / 2;
            Vector2D canvasOrigin = new Vector2D(canvas_origin_x - canvasWidth / 2, canvas_origin_y + canvasHeight / 2);

            // Draw all the states representing the particles.
            foreach (Particle particle in simulation.particles)
            {
                svgStates += DrawStateSVG(canvasOrigin, particle, properties);
            }

            HashSet<SMTransition> processedTransitions = new HashSet<SMTransition>();

            foreach (SMTransition transition in simulation.transitions)
            {
                if (transition.type == TransitionType.Transition && !processedTransitions.Contains(transition))
                {
                    SMTransition secondTransition = simulation.transitions.Find(x =>
                        x.origin.name == transition.destination.name &&
                        x.destination.name == transition.origin.name &&
                        !processedTransitions.Contains(x));

                    if (secondTransition == null)
                    {
                        svgTransitions += GenerateStraightTransitionSVG(transition.origin.position, transition.destination.position, transition.name, canvasOrigin, properties);
                    }
                    else
                    {
                        svgTransitions += GenerateCurvedTransitionSVG(transition.origin.position, transition.destination.position, canvasOrigin, transition.name, properties);
                        svgTransitions += GenerateCurvedTransitionSVG(secondTransition.origin.position, secondTransition.destination.position, canvasOrigin, secondTransition.name, properties);

                        // Add both transitions to the processed set
                        processedTransitions.Add(transition);
                        processedTransitions.Add(secondTransition);
                    }
                }
            }

            HashSet<SMTransition> processedAutoTransitions = new HashSet<SMTransition>();

            foreach (SMTransition transition in simulation.transitions.FindAll(x => x.type == TransitionType.Auto))
            {
                List<SMTransition> adjacentTransitions = simulation.transitions.FindAll(x => (x.origin.name == transition.origin.name || x.destination.name == transition.origin.name) && x.type != TransitionType.Auto);
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

                svgTransitions += GenerateAutoPathSVG(autoDirection, transition.origin.position, canvasOrigin, transition.name, properties);
            }


            string marker = "<defs><marker id=\"arrowhead\" markerWidth=\"10\" markerHeight=\"7\" refX = \"7\" refY = \"3.5\" orient = \"auto\" ><polygon points=\"0 0, 10 3.5, 0 7\" /></marker></defs>" + Environment.NewLine;

            sw.Write("<svg height=\"" + canvasHeight + "\" width=\"" + canvasWidth + "\">" + Environment.NewLine + marker + svgTransitions + svgStates + " </svg>");
            sw.Close();

        }

        public static string DrawStateSVG(Vector2D origin, Particle input_particle, SVGProperties properties)
        {
            Vector2D circle_position = input_particle.position.ToSvgCoordinates(origin);
            bool is_marked = input_particle.marked;

            string circle_svg = "<circle cx=\"" + circle_position.x + "\" cy=\"" + circle_position.y + "\" r=\"" + properties.stateRadius + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            string circle_marked_svg = "";

            if (is_marked)
            {
                circle_marked_svg = "<circle cx=\"" + circle_position.x + "\" cy=\"" + circle_position.y + "\" r=\"" + properties.stateRadius * properties.markedRatio + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            }

            string circle_text_svg = "<text x=\"" + circle_position.x + "\" y=\"" + circle_position.y + "\" dominant-baseline=\"central\" font-size=\"" + properties.textSize +"\"em fill=\"" + properties.textColor + "\" text-anchor=\"middle\">" + input_particle.name + "</text>" + Environment.NewLine;
            return circle_svg + circle_marked_svg + circle_text_svg;
        }

        /// <summary>
        /// Generates an SVG representation of an auto path with an arrow and text label.
        /// </summary>
        /// <param name="direction">The direction of the path.</param>
        /// <param name="state">The center point of the state.</param>
        /// <param name="svgOrigin">The origin point of the SVG coordinates.</param>
        /// <param name="transitionName">The name of the transition.</param>
        /// <param name="properties">The SVG properties for styling the path, arrow, and text.</param>
        /// <returns>The generated SVG representation of the automatic path.</returns>
        public static string GenerateAutoPathSVG(Vector2D direction, Vector2D statePosition, Vector2D svgOrigin, string transitionName, SVGProperties properties)
        {
            Vector2D normalizedTransitionDirection = direction.Normalized();

            string output = "";

            float arrowCoverageAngle = Vector2D.AngleBetween(properties.arrowLength, properties.autoRadius, properties.autoRadius);
            float stateToTransitionDistance = properties.autoRadius + properties.stateRadius - properties.autoRadius * properties.overlap;

            Vector2D transitionCenter = statePosition + normalizedTransitionDirection * stateToTransitionDistance;

            float angleToIntersectPoint = Vector2D.AngleBetween(properties.autoRadius, stateToTransitionDistance, properties.stateRadius);

            Vector2D initialIntersect = statePosition + stateToTransitionDistance * normalizedTransitionDirection.Rotated(angleToIntersectPoint);
            Vector2D finalIntersect = statePosition + stateToTransitionDistance * normalizedTransitionDirection.Rotated(-angleToIntersectPoint);

            Vector2D transitionCenterToFinalIntersect = (finalIntersect - transitionCenter).Normalized();

            Vector2D transitionEnd = transitionCenter + properties.autoRadius * transitionCenterToFinalIntersect.Rotated(arrowCoverageAngle);
            Vector2D transitionStart = initialIntersect;

            Vector2D arrowTip = finalIntersect;
            Vector2D arrowDirection = (arrowTip - transitionEnd).Normalized();

            Vector2D transitionEndSVG = transitionEnd.ToSvgCoordinates(svgOrigin);
            Vector2D transitionStartSVG = transitionStart.ToSvgCoordinates(svgOrigin);

            Vector2D arrowTipSVG = arrowTip.ToSvgCoordinates(svgOrigin);
            Vector2D arrowDirectionSVG = arrowDirection.ToSvgCoordinates(svgOrigin);

            Vector2D textPosition = transitionCenter + normalizedTransitionDirection * (properties.textDistance + properties.autoRadius);

            output += "<path stroke-width=\"1\" stroke=\"black\" fill=\"none\" d=\" M " + transitionStartSVG.x + " " + transitionStartSVG.y + " A " + properties.autoRadius + " " + properties.autoRadius + " 0 1 1 " + transitionEndSVG.x + " " + transitionEndSVG.y + "\" />" + Environment.NewLine;
            output += GenerateArrowheadPolygonSVG(arrowTipSVG, properties, arrowDirectionSVG);
            output += GenerateSvgTextElement(textPosition, direction, svgOrigin, transitionName, properties);

            return output;
        }

        /// <summary>
        /// Generates an SVG line element from the specified origin to the destination with the given properties.
        /// </summary>
        /// <param name="origin">The starting point of the line.</param>
        /// <param name="destination">The ending point of the line.</param>
        /// <param name="properties">The SVG properties for styling the line.</param>
        /// <returns>The generated SVG line element.</returns>
        static string GenerateLineElementSVG(Vector2D origin, Vector2D destination, SVGProperties properties)
        {
            string svgLineElement = $"<line x1=\"{origin.x}\" y1=\"{origin.y}\" x2=\"{destination.x}\" y2=\"{destination.y}\" stroke=\"{properties.strokeColor}\" stroke-width=\"{properties.strokeWidth}\" />{Environment.NewLine}";

            return svgLineElement;
        }

        /// <summary>
        /// Generates an SVG polygon element representing an arrowhead at the specified tip point with the given properties and direction.
        /// </summary>
        /// <param name="tip">The tip point of the arrowhead.</param>
        /// <param name="properties">The SVG properties for styling the arrowhead.</param>
        /// <param name="direction">The direction vector of the arrowhead.</param>
        /// <returns>The generated SVG polygon element for the arrowhead.</returns>
        static string GenerateArrowheadPolygonSVG(Vector2D tip, SVGProperties properties, Vector2D direction)
        {
            // Calculate the base point of the arrowhead by subtracting the normalized direction vector multiplied by the arrow length from the tip.
            Vector2D basePoint = tip - direction.Normalized() * properties.arrowLength;

            // Calculate the side points of the arrowhead by adding/subtracting the perpendicular direction vector multiplied by half of the arrow width to/from the base point.
            Vector2D sidePoint1 = basePoint + direction.Perpendicular() * properties.arrowWidth / 2;
            Vector2D sidePoint2 = basePoint - direction.Perpendicular() * properties.arrowWidth / 2;

            // Construct the SVG polygon element with the calculated points, arrow color, and stroke width.
            string svgPolygonElement = $"<polygon fill=\"{properties.arrowColor}\" stroke-width=\"1\" " +
                $"points=\"{tip.x} {tip.y} {sidePoint1.x} {sidePoint1.y} {sidePoint2.x} {sidePoint2.y}\" />{Environment.NewLine}";

            return svgPolygonElement;
        }

        /// <summary>
        /// Generates an SVG representation of a straight arrow from the specified origin to the destination with the given properties.
        /// </summary>
        /// <param name="origin">The starting point of the arrow.</param>
        /// <param name="destination">The ending point of the arrow.</param>
        /// <param name="properties">The SVG properties for styling the arrow.</param>
        /// <returns>The generated SVG representation of the straight arrow.</returns>
        static string GenerateStraightArrowSVG(Vector2D origin, Vector2D destination, SVGProperties properties)
        {
            Vector2D insideArrow = destination + (origin - destination).Normalized() * properties.arrowLength / 2;

            string svgLineElement = GenerateLineElementSVG(origin, insideArrow, properties);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, properties, destination - origin);

            string svgRepresentation = svgLineElement + svgArrowheadElement;

            return svgRepresentation;
        }

        /// <summary>
        /// Generates an SVG representation of a straight transition arrow from the specified origin to the destination with the given properties and transition name.
        /// </summary>
        /// <param name="origin">The starting point of the transition arrow.</param>
        /// <param name="destination">The ending point of the transition arrow.</param>
        /// <param name="transitionName">The name of the transition.</param>
        /// <param name="svgOrigin">The origin point of the SVG coordinates.</param>
        /// <param name="properties">The SVG properties for styling the transition arrow and text.</param>
        /// <returns>The generated SVG representation of the straight transition arrow.</returns>
        static string GenerateStraightTransitionSVG(Vector2D origin, Vector2D destination, string transitionName, Vector2D svgOrigin, SVGProperties properties)
        {
            Vector2D transitionDirection = (destination - origin).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D transitionOrigin = (origin + transitionDirection * properties.stateRadius).ToSvgCoordinates(svgOrigin);
            Vector2D transitionDestination = (destination - transitionDirection * properties.stateRadius).ToSvgCoordinates(svgOrigin);

            Vector2D textPosition = origin.Middle(destination) + perpendicularDirection * properties.textDistance;

            string svgArrowElement = GenerateStraightArrowSVG(transitionOrigin, transitionDestination, properties);
            string svgTextElement = GenerateSvgTextElement(textPosition, perpendicularDirection, svgOrigin, transitionName, properties);

            string svgRepresentation = svgArrowElement + svgTextElement;

            return svgRepresentation;
        }

        /// <summary>
        /// Generates an SVG path element for a quadratic Bézier curve from the specified origin to the destination with the given control point.
        /// </summary>
        /// <param name="origin">The starting point of the curve.</param>
        /// <param name="destination">The ending point of the curve.</param>
        /// <param name="control">The control point that defines the curvature of the curve.</param>
        /// <param name="properties">The SVG properties for styling the curve.</param>
        /// <returns>The generated SVG path element for the quadratic Bézier curve.</returns>
        static string GenerateQuadraticBezierPathSVG(Vector2D origin, Vector2D destination, Vector2D control, SVGProperties properties)
        {
            // Construct the path attribute.
            string path = $"M {origin.x} {origin.y} Q {control.x} {control.y} {destination.x} {destination.y}";

            // Construct the stroke attribute.
            string stroke = $"stroke=\"{properties.strokeColor}\"";

            // Construct the stroke-width attribute.
            string strokeWidth = $"stroke-width=\"{properties.strokeWidth}\"";

            // Construct the fill attribute.
            string fill = "fill=\"none\"";

            // Construct the SVG path element.
            string svgPathElement = $"<path d=\"{path}\" {stroke} {strokeWidth} {fill} />{Environment.NewLine}";

            return svgPathElement;
        }

        /// <summary>
        /// Generates an SVG representation of a curved arrow from the specified origin to the destination with the given properties.
        /// </summary>
        /// <param name="origin">The starting point of the arrow.</param>
        /// <param name="destination">The ending point of the arrow.</param>
        /// <param name="control">The control point of the quadratic Bezier curve.</param>
        /// <param name="properties">The SVG properties for styling the arrow.</param>
        /// <returns>The generated SVG representation of the curved arrow.</returns>
        static string GenerateCurvedArrowSVG(Vector2D origin, Vector2D destination, Vector2D control, SVGProperties properties)
        {
            Vector2D insideArrow = destination + (control - destination).Normalized() * properties.arrowLength / 2;

            string svgPathElement = GenerateQuadraticBezierPathSVG(origin, insideArrow, control, properties);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, properties, destination - control);

            string svgRepresentation = svgPathElement + svgArrowheadElement;

            return svgRepresentation;
        }

        /// <summary>
        /// Generates an SVG representation of a curved transition arrow from the specified start point to the end point with the given properties and transition name.
        /// </summary>
        /// <param name="start">The starting point of the transition arrow.</param>
        /// <param name="end">The ending point of the transition arrow.</param>
        /// <param name="svgOrigin">The origin point of the SVG coordinates.</param>
        /// <param name="transitionName">The name of the transition.</param>
        /// <param name="properties">The SVG properties for styling the transition arrow and text.</param>
        /// <returns>The generated SVG representation of the curved transition arrow.</returns>
        static string GenerateCurvedTransitionSVG(Vector2D start, Vector2D end, Vector2D svgOrigin, string transitionName, SVGProperties properties)
        {
            Vector2D middlePosition = start.Middle(end);
            Vector2D transitionDirection = (end - start).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D controlPoint = middlePosition + perpendicularDirection * properties.arcSize;
            Vector2D transitionOrigin = start + ((controlPoint - start).Normalized() * properties.stateRadius);
            Vector2D transitionDestination = end + ((controlPoint - end).Normalized() * properties.stateRadius);

            Vector2D controlPointSVG = controlPoint.ToSvgCoordinates(svgOrigin);
            Vector2D originSVG = transitionOrigin.ToSvgCoordinates(svgOrigin);
            Vector2D destinationSVG = transitionDestination.ToSvgCoordinates(svgOrigin);

            Vector2D textPosition = middlePosition + perpendicularDirection * (properties.arcSize + properties.textDistance);

            string svgArrowElement = GenerateCurvedArrowSVG(originSVG, destinationSVG, controlPointSVG, properties);
            string svgTextElement = GenerateSvgTextElement(textPosition, perpendicularDirection, svgOrigin, transitionName, properties);

            string svgRepresentation = svgArrowElement + svgTextElement;

            return svgRepresentation;
        }

        /// <summary>
        /// Generates an SVG text element with the specified position, direction, and text content.
        /// </summary>
        /// <param name="position">The position of the text element.</param>
        /// <param name="direction">The direction of the text element.</param>
        /// <param name="svgOrigin">The origin of the SVG coordinate system.</param>
        /// <param name="text">The text content of the element.</param>
        /// <returns>The generated SVG text element.</returns>
        static string GenerateSvgTextElement(Vector2D position, Vector2D direction, Vector2D svgOrigin, string text, SVGProperties properties)
        {
            // Calculate the angle based on the direction.
            float angle = direction.Angle();

            // Determine the text anchor based on the angle.
            string textAnchor;
            if (angle >= 5.4978f || angle <= 0.7854f)
                textAnchor = "start";
            else if (angle > 0.7854f && angle < 2.3562f)
                textAnchor = "middle";
            else if (angle >= 2.3562f && angle <= 3.9269f)
                textAnchor = "end";
            else
                textAnchor = "middle";

            // Convert the position to SVG coordinates.
            Vector2D svgPosition = position.ToSvgCoordinates(svgOrigin);

            // Construct the SVG text element with the specified attributes.
            string svgTextElement = $@"
                <text x=""{svgPosition.x}"" y=""{svgPosition.y}"" 
                      text-anchor=""{textAnchor}"" 
                      font-size=""{properties.textSize}"" fill=""{properties.textColor}"">
                    {text}
                </text>
                {Environment.NewLine}";

            return svgTextElement;
        }


    }
}
