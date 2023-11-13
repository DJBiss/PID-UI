using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;
using System.Collections.Generic;

namespace PidUI.ViewModel
{
    public class CustomTooltip : IChartTooltip<SkiaSharpDrawingContext>
    {
        private StackPanel<RectangleGeometry, SkiaSharpDrawingContext>? stackPanel;
        private static readonly int zIndex = 10100;
        private readonly SolidColorPaint backgroundPaint = new(new SKColor(100, 100, 100, 220)) { ZIndex = zIndex };
        private readonly SolidColorPaint fontPaint = new(new SKColor(250, 250, 250)) { ZIndex = zIndex + 1 };

        public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            if (stackPanel is null)
            {
                stackPanel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
                {
                    Padding = new Padding(5),
                    Orientation = ContainerOrientation.Vertical,
                    HorizontalAlignment = Align.Start,
                    VerticalAlignment = Align.Middle,
                    BackgroundPaint = backgroundPaint,
                };
            }

            // clear the previous elements.
            foreach (var child in stackPanel.Children.ToArray())
            {
                _ = stackPanel.Children.Remove(child);
                chart.RemoveVisual(child);
            }

            foreach (var point in foundPoints)
            {
                var sketch = ((IChartSeries<SkiaSharpDrawingContext>)point.Context.Series).GetMiniaturesSketch();
                var relativePanel = sketch.AsDrawnControl(zIndex);

                var label = new LabelVisual
                {
                    Text = point.Coordinate.PrimaryValue.ToString("0.###"),
                    Paint = fontPaint,
                    TextSize = 15,
                    Padding = new Padding(5, 0, 0, 0),
                    ClippingMode = ClipMode.None, // required on tooltips
                    VerticalAlignment = Align.Start,
                    HorizontalAlignment = Align.Start
                };

                var sp = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
                {
                    Padding = new Padding(0, 4),
                    VerticalAlignment = Align.Middle,
                    HorizontalAlignment = Align.Middle,
                    Children = { relativePanel, label }
                };

                stackPanel?.Children.Add(sp);
            }

            var size = stackPanel.Measure(chart);

            var location = foundPoints.GetTooltipLocation(size, chart);

            stackPanel.X = location.X;
            stackPanel.Y = location.Y;

            chart.AddVisual(stackPanel);
        }

        public void Hide(Chart<SkiaSharpDrawingContext> chart)
        {
            if (chart is null || stackPanel is null) return;
            chart.RemoveVisual(stackPanel);
        }
    }
}
