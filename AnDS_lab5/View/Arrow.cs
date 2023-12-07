using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnDS_lab5.View;

public sealed class Arrow : Shape
{
    public static readonly DependencyProperty X1Property =
        DependencyProperty.Register(nameof(X1), typeof(double), typeof(Arrow),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.AffectsMeasure 
                | FrameworkPropertyMetadataOptions.AffectsRender));
    
    public static readonly DependencyProperty X2Property =
        DependencyProperty.Register(nameof(X2), typeof(double), typeof(Arrow), 
            new FrameworkPropertyMetadata(0d, 
                FrameworkPropertyMetadataOptions.AffectsMeasure
                | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty Y1Property =
        DependencyProperty.Register(nameof(Y1), typeof(double), typeof(Arrow),
            new FrameworkPropertyMetadata(0d,
                FrameworkPropertyMetadataOptions.AffectsMeasure 
                | FrameworkPropertyMetadataOptions.AffectsRender));
    
    public static readonly DependencyProperty Y2Property =
        DependencyProperty.Register(nameof(Y2), typeof(double), typeof(Arrow), 
            new FrameworkPropertyMetadata(0d, 
                FrameworkPropertyMetadataOptions.AffectsMeasure
                | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty HeadHeightProperty =
        DependencyProperty.Register(nameof(HeadHeight), typeof(double), typeof(Arrow));

    public static readonly DependencyProperty HeadWidthProperty =
        DependencyProperty.Register(nameof(HeadWidth), typeof(double), typeof(Arrow));
    
    [TypeConverter(typeof(LengthConverter))]
    public double X1 
    { 
        get => (double)GetValue(X1Property);
        set => SetValue(X1Property, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double X2
    {
        get => (double)GetValue(X2Property);
        set => SetValue(X2Property, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double Y1
    {
        get => (double)GetValue(Y1Property);
        set => SetValue(Y1Property, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double Y2
    {
        get => (double)GetValue(Y2Property);
        set => SetValue(Y2Property, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double HeadHeight
    {
        get => (double)GetValue(HeadHeightProperty);
        set => SetValue(HeadHeightProperty, value);
    }

    [TypeConverter(typeof(LengthConverter))]
    public double HeadWidth
    {
        get => (double)GetValue(HeadWidthProperty);
        set => SetValue(HeadWidthProperty, value);
    }
    
    protected override Geometry DefiningGeometry
    {
        get
        {
            var geometry = new StreamGeometry
            {
                FillRule = FillRule.EvenOdd
            };
            using (var context = geometry.Open())
            {
                InternalDrawArrowGeometry(context);
            }

            geometry.Freeze();
            return geometry;
        }
    }
    
    private void InternalDrawArrowGeometry(StreamGeometryContext context)
    {
        double theta = Math.Atan2(Y1 - Y2, X1 - X2);
        double sint = Math.Sin(theta);
        double cost = Math.Cos(theta);

        var pt1 = new Point(X1, Y1);
        var pt2 = new Point(X2, Y2);

        double xTemp = (X1 + X2) / 2;
        double yTemp = (Y1 + Y2) / 2;
        
        double xMid = (X2 + xTemp) / 2;
        double yMid = (Y2 + yTemp) / 2;
        
        var ptMid = new Point(
            xMid, yMid);

        var pt3 = new Point(
            xMid + (HeadWidth * cost - HeadHeight * sint),
            yMid + (HeadWidth * sint + HeadHeight * cost));

        var pt4 = new Point(
            xMid + (HeadWidth * cost + HeadHeight * sint),
            yMid - (HeadHeight * cost - HeadWidth * sint));
        
        context.BeginFigure(pt1, true, false);
        context.LineTo(ptMid, true, true);
        context.LineTo(pt3, true, true);
        context.LineTo(ptMid, true, true);
        context.LineTo(pt4, true, true);
        context.LineTo(ptMid, true, true);
        context.LineTo(pt2, true, true);
    }
}