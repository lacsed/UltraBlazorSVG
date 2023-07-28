using System;

namespace Extra
{
    public class SVGProperties
    {
        // Text properties
        public float textDistance { get; set; }
        public string textColor { get; set; }
        public float textSize { get; set; }

        // Stroke properties
        public string strokeColor { get; set; }
        public string strokeFill { get; set; }
        public float strokeWidth { get; set; }

        // State properties
        public float stateRadius { get; set; }
        public float markedRatio { get; set; }

        // Arrow properties
        public float arrowLength { get; set; }
        public float arrowWidth { get; set; }
        public string arrowColor { get; set; }

        // Additional properties
        public float overlap { get; set; }
        public float autoRadius { get; set; }
        public float arcSize { get; set; }

        // Constructor
        public SVGProperties(float textDistance, string textColor, float textSize,
            string strokeColor, string strokeFill, float strokeWidth,
            float stateRadius, float markedRatio,
            float arrowLength, float arrowWidth, string arrowColor,
            float overlap, float autoRadius, float arcSize)
        {
            this.textDistance = textDistance;
            this.textColor = textColor;
            this.textSize = textSize;

            this.strokeColor = strokeColor;
            this.strokeFill = strokeFill;
            this.strokeWidth = strokeWidth;

            this.stateRadius = stateRadius;
            this.markedRatio = markedRatio;

            this.arrowLength = arrowLength;
            this.arrowWidth = arrowWidth;
            this.arrowColor = arrowColor;

            this.overlap = overlap;
            this.autoRadius = autoRadius;
            this.arcSize = arcSize;
        }

    }
}
