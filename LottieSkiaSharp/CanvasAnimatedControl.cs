using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Xamarin.Forms;

namespace LottieUWP
{
    class CanvasAnimatedControl:Xamarin.Forms.ContentView
    {


        public bool ForceSoftwareRenderer
        {
            get { return (bool)GetValue(ForceSoftwareRendererProperty); }
            set { SetValue(ForceSoftwareRendererProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceSoftwareRenderer.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty ForceSoftwareRendererProperty =
            BindableProperty.Create(nameof(ForceSoftwareRenderer), typeof(bool), typeof(CanvasAnimatedControl),true );




        public bool Paused
        {
            get { return (bool)GetValue(PausedProperty); }
            set { SetValue(PausedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Paused.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty PausedProperty =
            BindableProperty.Create(nameof(Paused), typeof(bool), typeof(CanvasAnimatedControl), true);

        public event EventHandler<DrawEventArgs> Draw;
        public event EventHandler CanvasAnimatedControlLoaded;
        public void Invalidate()
        {

        }
    }
    class DrawEventArgs:EventArgs
    {
        public DrawEventArgs(SKSurface surface, object origin)
        {
            Surface = surface;
            Origin = origin;
        }

        /// <summary>
        /// Gets the surface that is currently being drawn on.
        /// </summary>
        public SKSurface Surface { get; }
        public object Origin { get; }
    }
}
