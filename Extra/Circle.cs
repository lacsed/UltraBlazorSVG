using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Circle : Drawables
    {
        public Vector2D center;
        public float radius;
        public bool marked;

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D centerSvg = center.ToSvgCoordinates(canvasOrigin);

            string circleSvg = "<circle cx=\"" + centerSvg.x + "\" cy=\"" + centerSvg.y + "\" r=\"" + properties.stateRadius + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            string circleMarkedSvg = "";

            if (marked)
            {
                circleMarkedSvg = "<circle cx=\"" + centerSvg.x + "\" cy=\"" + centerSvg.y + "\" r=\"" + properties.stateRadius * properties.markedRatio + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            }

            return circleSvg + circleMarkedSvg;
        }

        public override DrawableLimits Limits()
        {
            float x = center.x;
            float y = center.y;
            float minX = x - radius;
            float maxX = x + radius;
            float minY = y - radius;
            float maxY = y + radius;

            return new DrawableLimits(minX, maxX, minY, maxY);
        }

        public Circle(Vector2D inputCenter, float inputRadius, bool inputMarked)
        {
            center = inputCenter;
            radius = inputRadius;
            marked = inputMarked;
        }
    }
}
