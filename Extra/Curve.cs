using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Curve : Drawables
    {
        public Vector2D start;
        public Vector2D end;
        public Vector2D control;

        public Curve(Vector2D inputStart, Vector2D inputEnd, Vector2D inputControl)
        {
            start = inputStart;
            end = inputEnd;
            control = inputControl;
        }

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D startSVG = start.ToSvgCoordinates(canvasOrigin);
            Vector2D endSVG = end.ToSvgCoordinates(canvasOrigin);
            Vector2D controlSVG = control.ToSvgCoordinates(canvasOrigin);

            string path = $"M {startSVG.x} {startSVG.y} Q {controlSVG.x} {controlSVG.y} {endSVG.x} {endSVG.y}";

            string stroke = $"stroke=\"{properties.strokeColor}\"";

            string strokeWidth = $"stroke-width=\"{properties.strokeWidth}\"";

            string fill = "fill=\"none\"";

            string svgPathElement = $"<path d=\"{path}\" {stroke} {strokeWidth} {fill} />{Environment.NewLine}";

            return svgPathElement;
        }

        public override DrawableLimits Limits()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            Vector2D[] points = { start, end, control };

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
