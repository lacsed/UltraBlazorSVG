using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Arc : Drawables
    {
        public Vector2D center;
        public Vector2D start;
        public Vector2D end;

        public Arc(Vector2D inputCenter, Vector2D inputStart, Vector2D inputEnd)
        {
            center = inputCenter;
            start = inputStart;
            end = inputEnd;
        }

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D startSVG = start.ToSvgCoordinates(canvasOrigin);
            Vector2D endSVG = end.ToSvgCoordinates(canvasOrigin);

            return  "<path stroke-width=\"1\" stroke=\"black\" fill=\"none\" d=\" M " + startSVG.x + " " + startSVG.y + " A " + properties.autoRadius + " " + properties.autoRadius + " 0 1 1 " + endSVG.x + " " + endSVG.y + "\" />" + Environment.NewLine;
        }

        public override DrawableLimits Limits()
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            Vector2D[] points = { center, start, end };

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
