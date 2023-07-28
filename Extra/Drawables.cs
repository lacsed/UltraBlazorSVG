using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public abstract class Drawables
    {
        public abstract DrawableLimits Limits();
        public abstract string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties);
    }

    public struct DrawableLimits
    {
        public float minX { get; set; }
        public float maxX { get; set; }
        public float minY { get; set; }
        public float maxY { get; set; }

        public DrawableLimits(float inputMinX, float inputMaxX, float inputMinY, float inputMaxY)
        {
            minX = inputMinX;
            maxX = inputMaxX;
            minY = inputMinY;
            maxY = inputMaxY;
        }

        public DrawableLimits Compare(DrawableLimits compareTo)
        {
            float finalMinX = (minX < compareTo.minX) ? minX : compareTo.minX;
            float finalMaxX = (maxX > compareTo.maxX) ? maxX : compareTo.maxX;
            float finalMinY = (minY < compareTo.minY) ? minY : compareTo.minY;
            float finalMaxY = (maxY > compareTo.maxY) ? maxY : compareTo.maxY;

            return new DrawableLimits(finalMinX, finalMaxX, finalMinY, finalMaxY);
        }
    }
}
