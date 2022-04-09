using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Homework_App.Pages; 

public partial class Test : Window {
    public Test() {
        InitializeComponent();
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