using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class CoordinateSystem
    {
        Vector2D origin;
        Vector2D xAxis;
        Vector2D yAxis;

        public CoordinateSystem(Vector2D origin, Vector2D xAxis, Vector2D yAxis)
        {
            this.origin = origin;
            this.xAxis = xAxis;
            this.yAxis = yAxis;
        }

        public void MoveOrigin(Vector2D displacement)
        {
            origin += displacement;
        }

        public Vector2D ToCartesian(Vector2D point)
        {
            Vector2D xDisplacement = point.x * this.xAxis;
            Vector2D yDisplacement = point.y * this.yAxis;
            Vector2D totalDisplacement = xDisplacement + yDisplacement;
            Vector2D cartesianPoint = this.origin + totalDisplacement;
            return cartesianPoint;
        }

        public Vector2D fromCartesian(Vector2D point)
        {
            Vector2D displacement = point - this.origin;
            float scalarProjectionX = displacement.Dot(this.xAxis);
            float scalarProjectionY = displacement.Dot(this.yAxis);
            Vector2D customPoint = new Vector2D(scalarProjectionX, scalarProjectionY);
            return customPoint;
        }
    }
}
