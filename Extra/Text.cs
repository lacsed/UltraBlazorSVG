using System;
using System.Collections.Generic;
using System.Text;

namespace Extra
{
    public class Text : Drawables
    {
        public Vector2D position;
        public string text;
        public Anchor anchor;

        public Text(Vector2D inputPosition, string inputText, Anchor inpuAnchor)
        {
            position = inputPosition;
            text = inputText;
            anchor = inpuAnchor;
        }

        public override string ConvertToSvg(Vector2D canvasOrigin, SVGProperties properties)
        {
            Vector2D svgPosition = position.ToSvgCoordinates(canvasOrigin);

            // Construct the SVG text element with the specified attributes.
            string svgTextElement = $@"
                <text x=""{svgPosition.x}"" y=""{svgPosition.y}"" 
                      text-anchor=""{anchor}"" dominant-baseline=""central""
                      font-size=""{properties.textSize}"" fill=""{properties.textColor}"">
                    {text}
                </text>
                {Environment.NewLine}";

            return svgTextElement;
        }

        public override DrawableLimits Limits()
        {
            float x = position.x;
            float y = position.y;
            float minX = x;
            float maxX = x;
            float minY = y;
            float maxY = y;

            return new DrawableLimits(minX, maxX, minY, maxY);
        }

        public static Anchor GetAnchor(Vector2D textDirection)
        {
            float angle = textDirection.Angle();

            Anchor textAnchor;
            if (angle >= 5.4978f || angle <= 0.7854f)
                textAnchor = Anchor.start;
            else if (angle > 0.7854f && angle < 2.3562f)
                textAnchor = Anchor.middle;
            else if (angle >= 2.3562f && angle <= 3.9269f)
                textAnchor = Anchor.end;
            else
                textAnchor = Anchor.middle;

            return textAnchor;
        }
    }

    public enum Anchor
    {
        start,
        middle,
        end
    }

}
