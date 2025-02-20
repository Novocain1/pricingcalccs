using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PricingCalculator
{
    public class PricingGraph : Panel
    {
        // Pricing adjustment config reference
        private ReasonableIncreaseConfig config;

        // Graph style properties
        private Color gridColor = Color.FromArgb(230, 230, 230);
        private Color axisColor = Color.FromArgb(80, 80, 80);
        private Color lineColor = Color.FromArgb(0, 120, 215);
        private Color fillColor = Color.FromArgb(80, 180, 240, 80);
        private Color percentLineColor = Color.FromArgb(220, 90, 20);
        private Color labelColor = Color.FromArgb(60, 60, 60);
        private Font labelFont = new Font("Segoe UI", 8F);
        private Font titleFont = new Font("Segoe UI", 9F, FontStyle.Bold);

        // Price points to display
        private List<decimal> pricePoints = new List<decimal>();

        // Graph margins
        private int marginLeft = 50;
        private int marginRight = 20;
        private int marginTop = 40;
        private int marginBottom = 40;

        // Graph title
        private string title = "Price Adjustment Visualization";

        public PricingGraph()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;

            // Generate default price points
            GeneratePricePoints();
        }

        // Method to update the config reference
        public void SetConfig(ReasonableIncreaseConfig config)
        {
            this.config = config;
            this.Invalidate(); // Force redraw
        }

        // Generate price points ($0 to $150 in increments)
        private void GeneratePricePoints()
        {
            pricePoints.Clear();
            for (decimal price = 0; price <= 150; price += 5)
            {
                pricePoints.Add(price);
            }
        }

        // Calculate adjusted price based on config
        private decimal CalculateAdjustedPrice(decimal price)
        {
            if (config == null)
                return price;

            decimal additionalIncrease = 0;

            if (config.UseInterpolation)
            {
                // Smooth interpolation between price tiers
                if (price <= config.LowPriceThreshold)
                {
                    additionalIncrease = config.LowPriceAdditional;
                }
                else if (price <= config.MidPriceThreshold)
                {
                    decimal ratio = (price - config.LowPriceThreshold) /
                                  (config.MidPriceThreshold - config.LowPriceThreshold);
                    additionalIncrease = config.LowPriceAdditional -
                                       (ratio * (config.LowPriceAdditional - config.MidPriceAdditional));
                }
                else if (price <= config.HighPriceThreshold)
                {
                    decimal ratio = (price - config.MidPriceThreshold) /
                                  (config.HighPriceThreshold - config.MidPriceThreshold);
                    additionalIncrease = config.MidPriceAdditional -
                                       (ratio * (config.MidPriceAdditional - config.HighPriceAdditional));
                }
                else
                {
                    additionalIncrease = config.HighPriceAdditional;
                }
            }
            else
            {
                // Discrete tier approach
                if (price <= config.LowPriceThreshold)
                    additionalIncrease = config.LowPriceAdditional;
                else if (price <= config.MidPriceThreshold)
                    additionalIncrease = config.MidPriceAdditional;
                else if (price <= config.HighPriceThreshold)
                    additionalIncrease = config.HighPriceAdditional;
                // For prices above HighPriceThreshold, only the base increase applies
            }

            decimal totalIncrease = Math.Min(config.BaseIncrease + additionalIncrease, config.MaxIncrease);
            return price * (1 + totalIncrease);
        }

        // Calculate increase percentage
        private decimal CalculateIncreasePercent(decimal price)
        {
            if (price == 0)
                return 0;

            decimal adjustedPrice = CalculateAdjustedPrice(price);
            return ((adjustedPrice / price) - 1) * 100;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // If config is null, draw placeholder text
            if (config == null)
            {
                g.DrawString("Configure the reasonable increase settings to see visualization",
                           titleFont, new SolidBrush(labelColor),
                           new Rectangle(0, 0, Width, Height),
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                return;
            }

            // Drawing area
            int graphWidth = this.Width - marginLeft - marginRight;
            int graphHeight = this.Height - marginTop - marginBottom;

            if (graphWidth <= 0 || graphHeight <= 0)
                return;

            // Draw title
            g.DrawString(title, titleFont, new SolidBrush(labelColor),
                        new RectangleF(marginLeft, 10, graphWidth, 20),
                        new StringFormat { Alignment = StringAlignment.Center });

            // Draw grid and axes
            DrawGrid(g, graphWidth, graphHeight);

            // Draw data series
            DrawPriceSeries(g, graphWidth, graphHeight);
            DrawPercentageSeries(g, graphWidth, graphHeight);

            // Draw legends
            DrawLegends(g, graphWidth, graphHeight);

            // Draw threshold markers
            DrawThresholdMarkers(g, graphWidth, graphHeight);
        }

        private void DrawGrid(Graphics g, int width, int height)
        {
            // Calculate grid spacing
            int horzLines = 5; // Number of horizontal grid lines
            int vertLines = 6; // Number of vertical grid lines

            Pen gridPen = new Pen(gridColor);
            Pen axisPen = new Pen(axisColor);

            // Draw horizontal grid lines
            for (int i = 0; i <= horzLines; i++)
            {
                int y = marginTop + (i * height / horzLines);

                // Draw line
                g.DrawLine(i == horzLines ? axisPen : gridPen,
                           marginLeft, y,
                           marginLeft + width, y);

                // Draw y-axis labels (price adjustment %)
                if (i < horzLines)
                {
                    decimal value = decimal.Round((decimal)(config.MaxIncrease * 100 * (horzLines - i) / horzLines), 1);
                    g.DrawString($"{value}%", labelFont, new SolidBrush(labelColor),
                               new RectangleF(5, y - 10, marginLeft - 10, 20),
                               new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw vertical grid lines
            for (int i = 0; i <= vertLines; i++)
            {
                int x = marginLeft + (i * width / vertLines);

                // Draw line
                g.DrawLine(i == 0 ? axisPen : gridPen,
                           x, marginTop,
                           x, marginTop + height);

                // Draw x-axis labels (price points)
                if (i <= vertLines)
                {
                    decimal price = 150 * i / vertLines;
                    g.DrawString($"${price}", labelFont, new SolidBrush(labelColor),
                               new RectangleF(x - 20, marginTop + height + 5, 40, 20),
                               new StringFormat { Alignment = StringAlignment.Center });
                }
            }

            // Draw axis titles
            g.DrawString("% Increase", labelFont, new SolidBrush(labelColor),
                        new RectangleF(5, marginTop + height / 2 - 40, 30, 80),
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionVertical });

            g.DrawString("Original Price ($)", labelFont, new SolidBrush(labelColor),
                        new RectangleF(marginLeft + width / 2 - 50, marginTop + height + 20, 100, 20),
                        new StringFormat { Alignment = StringAlignment.Center });
        }

        private void DrawPriceSeries(Graphics g, int width, int height)
        {
            if (pricePoints.Count == 0 || config == null)
                return;

            // Create points for the fillable area
            PointF[] areaPoints = new PointF[pricePoints.Count + 2];
            PointF[] linePoints = new PointF[pricePoints.Count];

            // Calculate all points first
            for (int i = 0; i < pricePoints.Count; i++)
            {
                decimal price = pricePoints[i];
                decimal adjustedPrice = CalculateAdjustedPrice(price);
                decimal priceDiff = adjustedPrice - price;

                // Calculate x position (based on original price)
                float x = marginLeft + (float)((decimal)width * price / 150);

                // Calculate y position (based on price difference)
                decimal maxPriceDiff = 150 * config.MaxIncrease; // Maximum possible price difference
                float y = marginTop + height - (float)((decimal)height * priceDiff / maxPriceDiff);

                // Ensure we stay within bounds
                x = Math.Max(marginLeft, Math.Min(marginLeft + width, x));
                y = Math.Max(marginTop, Math.Min(marginTop + height, y));

                // Store point for the line
                linePoints[i] = new PointF(x, y);

                // Store point for the area
                areaPoints[i] = new PointF(x, y);
            }

            // Complete the polygon for fill area
            areaPoints[pricePoints.Count] = new PointF(marginLeft + width, marginTop + height);
            areaPoints[pricePoints.Count + 1] = new PointF(marginLeft, marginTop + height);

            // Fill the area
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                g.FillPolygon(fillBrush, areaPoints);
            }

            // Draw the line
            using (Pen linePen = new Pen(lineColor, 2))
            {
                g.DrawLines(linePen, linePoints);
            }

            // Draw markers at key points
            using (SolidBrush markerBrush = new SolidBrush(lineColor))
            {
                foreach (var point in linePoints)
                {
                    g.FillEllipse(markerBrush, point.X - 3, point.Y - 3, 6, 6);
                }
            }
        }

        private void DrawPercentageSeries(Graphics g, int width, int height)
        {
            if (pricePoints.Count == 0 || config == null)
                return;

            PointF[] percentPoints = new PointF[pricePoints.Count];

            // Calculate all points first
            for (int i = 0; i < pricePoints.Count; i++)
            {
                decimal price = pricePoints[i];
                decimal percentIncrease = CalculateIncreasePercent(price);

                // Calculate x position (based on original price)
                float x = marginLeft + (float)((decimal)width * price / 150);

                // Calculate y position (based on percentage)
                decimal maxPercent = 100 * config.MaxIncrease; // Maximum possible percentage
                float y = marginTop + height - (float)((decimal)height * percentIncrease / maxPercent);

                // Ensure we stay within bounds
                x = Math.Max(marginLeft, Math.Min(marginLeft + width, x));
                y = Math.Max(marginTop, Math.Min(marginTop + height, y));

                // Store point
                percentPoints[i] = new PointF(x, y);
            }

            // Draw the percentage line
            using (Pen percentPen = new Pen(percentLineColor, 2))
            {
                percentPen.DashStyle = DashStyle.Dash;
                g.DrawLines(percentPen, percentPoints);
            }

            // Draw markers at key points
            using (SolidBrush markerBrush = new SolidBrush(percentLineColor))
            {
                foreach (var point in percentPoints)
                {
                    g.FillEllipse(markerBrush, point.X - 3, point.Y - 3, 6, 6);
                }
            }
        }

        private void DrawLegends(Graphics g, int width, int height)
        {
            // Draw legend
            int legendX = marginLeft + 10;
            int legendY = marginTop + 10;

            // Price Difference Legend
            g.DrawLine(new Pen(lineColor, 2), legendX, legendY, legendX + 20, legendY);
            g.DrawString("$ Amount Added", labelFont, new SolidBrush(labelColor),
                        legendX + 25, legendY - 7);

            // Percentage Legend
            Pen percentPen = new Pen(percentLineColor, 2);
            percentPen.DashStyle = DashStyle.Dash;
            g.DrawLine(percentPen, legendX, legendY + 20, legendX + 20, legendY + 20);
            g.DrawString("% Increase", labelFont, new SolidBrush(labelColor),
                        legendX + 25, legendY + 13);
        }

        private void DrawThresholdMarkers(Graphics g, int width, int height)
        {
            if (config == null)
                return;

            Pen thresholdPen = new Pen(Color.FromArgb(100, 100, 100), 1);
            thresholdPen.DashStyle = DashStyle.Dot;

            // Draw low price threshold
            if (config.LowPriceThreshold <= 150)
            {
                float xLow = marginLeft + (float)((decimal)width * config.LowPriceThreshold / 150);
                g.DrawLine(thresholdPen, xLow, marginTop, xLow, marginTop + height);
                g.DrawString($"Low: ${config.LowPriceThreshold}", labelFont, new SolidBrush(Color.FromArgb(100, 100, 100)),
                            xLow - 25, marginTop + 5);
            }

            // Draw mid price threshold
            if (config.MidPriceThreshold <= 150)
            {
                float xMid = marginLeft + (float)((decimal)width * config.MidPriceThreshold / 150);
                g.DrawLine(thresholdPen, xMid, marginTop, xMid, marginTop + height);
                g.DrawString($"Mid: ${config.MidPriceThreshold}", labelFont, new SolidBrush(Color.FromArgb(100, 100, 100)),
                            xMid - 25, marginTop + 20);
            }

            // Draw high price threshold (if within view)
            if (config.HighPriceThreshold <= 150)
            {
                float xHigh = marginLeft + (float)((decimal)width * config.HighPriceThreshold / 150);
                g.DrawLine(thresholdPen, xHigh, marginTop, xHigh, marginTop + height);
                g.DrawString($"High: ${config.HighPriceThreshold}", labelFont, new SolidBrush(Color.FromArgb(100, 100, 100)),
                            xHigh - 30, marginTop + 35);
            }
        }
    }
}