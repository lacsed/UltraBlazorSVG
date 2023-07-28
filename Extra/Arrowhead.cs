using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Arrowhead : Drawables
    {
        public Vector2D tip;
        public Vector2D rightWing;
        public Vector2D leftWing;

        public Arrowhead(Vector2D inputTip, Vector2D inputRightWing, Vector2D inputLeftWing)
        {
            tip = inputTip;
            rightWing = inputRightWing;
            leftWing = inputLeftWing;
        }

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D tipSVG = tip.ToSvgCoordinates(canvasOrigin);
            Vector2D rightWingSVG = rightWing.ToSvgCoordinates(canvasOrigin);
            Vector2D leftWingSVG = leftWing.ToSvgCoordinates(canvasOrigin);

            // Construct the SVG polygon element with the calculated points, arrow color, and stroke width.
            string svgPolygonElement = $"<polygon fill=\"{properties.arrowColor}\" stroke-width=\"1\" " +
                $"points=\"{tipSVG.x} {tipSVG.y} {rightWingSVG.x} {rightWingSVG.y} {leftWingSVG.x} {leftWingSVG.y}\" />{Environment.NewLine}";

            return svgPolygonElement;
        }

        public string ToSVGPoints(Vector2D svgOrigin)
        {
            Vector2D tipSVG = tip.ToSvgCoordinates(svgOrigin);
            Vector2D rightWingSVG = rightWing.ToSvgCoordinates(svgOrigin);
            Vector2D leftWingSVG = leftWing.ToSvgCoordinates(svgOrigin);

            string svgPoints = $"{tipSVG.x} {tipSVG.y} {rightWingSVG.x} {rightWingSVG.y} {leftWingSVG.x} {leftWingSVG.y}";
            return svgPoints;
        }

        public override DrawableLimits Limits()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            Vector2D[] points = { tip, rightWing, leftWing };

            foreach (Vector2D point in points)
            {
                float x = point.x;
                float y = point.y;

                if (x < minX)
                    minX = x;
                if (x > maxX)
                    maxX = x;
                if (y < minY)
                    minY = y;
                if (y > maxY)
                    maxY = y;
            }

            return new DrawableLimits(minX, maxX, minY, maxY);
        }
    }
}
