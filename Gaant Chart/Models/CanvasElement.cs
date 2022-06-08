﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gaant_Chart.Models
{
    public class CanvasElement
    {
        public FrameworkElement element { get; set; }

        public double leftoffset { get; set; }

        public double topoffset { get; set; }

        protected SolidColorBrush WHITE = new SolidColorBrush(Colors.White);
        protected SolidColorBrush GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 0));
    }

    public class TaskLabel : CanvasElement
    {
        private Label label { get; set; }
        public TaskLabel(String taskName)
        {
            label = new Label();
            label.Content = taskName;
            label.FontSize = 10;
            label.Foreground = WHITE;
            label.Width = 200;
            label.Height = 30;
            label.HorizontalAlignment = HorizontalAlignment.Right;

            element = label;
        }
    }

    public class DateLabel : CanvasElement
    { 
        private Label label { get; set; }

        private int ROTATATION = 285;
        public DateLabel(DateTime date)
        {
            label = new Label();
            label.Foreground = GREEN;
            label.FontSize = 14;
            label.Content = date.ToString("d/M/y");
            label.LayoutTransform = new RotateTransform(ROTATATION);

            element = label;
        }
    }

    public class TaskGroupLabel : CanvasElement
    {
        private TextBlock textblock { get; set; }
        public TaskGroupLabel(String taskGroupLabel)
        {
            textblock = new TextBlock();
            textblock.Text = taskGroupLabel;
            textblock.FontSize = 10;
            textblock.Foreground = WHITE;
            textblock.Width = 100;
            textblock.TextWrapping = TextWrapping.Wrap;
            textblock.TextAlignment = TextAlignment.Center;
            textblock.VerticalAlignment = VerticalAlignment.Bottom;

            element = textblock;
        }
    }

    public class CanvasLine
    {
        public Line line { get; set; } 
        public CanvasLine(double x1, double y1, double x2, double y2, SolidColorBrush color)
        {
            line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = color;

            line.Margin = new Thickness(100);
        }
    }
}