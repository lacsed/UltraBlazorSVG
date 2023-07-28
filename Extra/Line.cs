using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Line : Drawables
    {
        public Vector2D start;
        public Vector2D end;

        public Line(Vector2D inputStart, Vector2D inputEnd)
        {
            start = inputStart;
            end = inputEnd;
        }

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D startSVG = start.ToSvgCoordinates(canvasOrigin);
            Vector2D endSVG = end.ToSvgCoordinates(canvasOrigin);

            string svgLineElement = $"<line x1=\"{startSVG.x}\" y1=\"{startSVG.y}\" x2=\"{endSVG.x}\" y2=\"{endSVG.y}\" stroke=\"{properties.strokeColor}\" stroke-width=\"{properties.strokeWidth}\" />{Environment.NewLine}";

            return svgLineElement;
        }

        public string ToSVGPath(Vector2D svgOrigin)
        {
            Vector2D startSVG = start.ToSvgCoordinates(svgOrigin);
            Vector2D endSVG = end.ToSvgCoordinates(svgOrigin);
            string path = $"M {startSVG.x} {startSVG.y} L {endSVG.x} {endSVG.y}";
            return path;
        }

        public override DrawableLimits Limits()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            Vector2D[] points = { start, end };

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
