using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Homework_App.UserControls; 

public partial class IconButton : UserControl {
    public IconButton() {
        InitializeComponent();
    }
    
    /// <summary>
    /// Gets or sets the Content value for the label.
    /// </summary>
    public string? SegoeType {
        get => menuButton.Content.ToString();
        set => menuButton.Content = value;
    }

    /// <summary>
    /// Gets or sets the Background color of the ring.
    /// </summary>
    public string? RingColor {
        get => colorBorder.Background.ToString();
        set => colorBorder.BorderBrush = new BrushConverter().ConvertFrom(value) as Brush;
    }
    
    private void BorderMouseEnter(object sender, MouseEventArgs e) {
        menuBorder.BeginAnimation(UIElement.OpacityProperty,
            new DoubleAnimation(1d, TimeSpan.FromSeconds(0.25)));

        colorBorder.BeginAnimation(Ellipse.WidthProperty,
            new DoubleAnimation(35d, TimeSpan.FromSeconds(0.4)));
        
        colorBorder.BeginAnimation(Ellipse.HeightProperty,
            new DoubleAnimation(35d, TimeSpan.FromSeconds(0.4)));
        
        colorBorder.BeginAnimation(Ellipse.OpacityProperty,
            new DoubleAnimation(0d, TimeSpan.FromSeconds(0.8)));
    }


    private void BorderMouseLeave(object sender, MouseEventArgs e) {
        menuBorder.BeginAnimation(UIElement.OpacityProperty,
            new DoubleAnimation(0d, TimeSpan.FromSeconds(0.25)));
        
        colorBorder.BeginAnimation(Ellipse.WidthProperty,
            new DoubleAnimation(0d, TimeSpan.FromSeconds(0.4)));
        
        colorBorder.BeginAnimation(Ellipse.HeightProperty,
            new DoubleAnimation(0d, TimeSpan.FromSeconds(0.4)));
        
        colorBorder.BeginAnimation(Ellipse.OpacityProperty,
            new DoubleAnimation(0.45d, TimeSpan.FromSeconds(0.3)));
    }
}