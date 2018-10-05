using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Xamarin.Forms;

namespace LottieUWP
{
    public class CanvasAnimatedControl:Xamarin.Forms.ContentView
    {
        SkiaSharp.Views.Forms.SKCanvasView SKCanvasView;
        SkiaSharp.Views.Forms.SKGLView SKGLView;
        public CanvasAnimatedControl()
        {

        }
        private void SetupSurface()
        {
            if(ForceSoftwareRenderer)
            {
                if (SKCanvasView == null)
                {
                    SKCanvasView = new SkiaSharp.Views.Forms.SKCanvasView();
                    SKCanvasView.PaintSurface += SKCanvasView_PaintSurface;
                }
                if (SKGLView != null)
                {
                    SKGLView = null;
                }
                Content = SKCanvasView;
            }
            else
            {
                if (SKGLView == null)
                {
                    SKGLView = new SkiaSharp.Views.Forms.SKGLView();
                    SKGLView.PaintSurface += SKGLView_PaintSurface;
                }
                if (SKCanvasView != null)
                {
                    SKCanvasView = null;
                }
                Content = SKGLView;
            }
        }

        private void SKGLView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintGLSurfaceEventArgs e)
        {
            Draw?.Invoke(this, new DrawEventArgs(e.Surface, e.RenderTarget));
            isdrawing = false;
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            Draw?.Invoke(this, new DrawEventArgs(e.Surface, e.Info));
            isdrawing = false;
        }

        bool isdrawing;
        bool isstart;
        private async void Start()
        {
            //isstart = true;
            //while (isstart)
            //{
            //    await Task.Delay(1000 / 6);
            //    //if(!isdrawing)
            //    //    Invalidate();
            //}
        }
        private void Stop()
        {
            isstart = false;
        }
        public bool ForceSoftwareRenderer
        {
            get { return (bool)GetValue(ForceSoftwareRendererProperty); }
            set { SetValue(ForceSoftwareRendererProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceSoftwareRenderer.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty ForceSoftwareRendererProperty =
            BindableProperty.Create(nameof(ForceSoftwareRenderer), typeof(bool), typeof(CanvasAnimatedControl),true,propertyChanged: softwarechanged);

        private static void softwarechanged(BindableObject bindableObject ,object old,object @new)
        {
            var control = (CanvasAnimatedControl)bindableObject;
            var rn = (bool)@new;
            if (rn)
            {

            }
            else
            {

            }
        }



        public bool Paused
        {
            get { return (bool)GetValue(PausedProperty); }
            set { SetValue(PausedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Paused.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty PausedProperty =
            BindableProperty.Create(nameof(Paused), typeof(bool), typeof(CanvasAnimatedControl), true,propertyChanged:pausechanged);
        private static void pausechanged(BindableObject bindableObject, object old, object @new)
        {
            var control = (CanvasAnimatedControl)bindableObject;
            var rn = (bool)@new;
            if (rn)
            {
                control.Start();
            }
            else
            {
                control.Stop();
            }

        }

        public event EventHandler<DrawEventArgs> Draw;
        public event EventHandler CanvasAnimatedControlLoaded;
        public void Invalidate()
        {
            isdrawing = true;

            Device.BeginInvokeOnMainThread(() =>
            {
                SetupSurface();
                SKCanvasView?.InvalidateSurface();
                SKGLView?.InvalidateSurface();

            });
        }

    }
    public class DrawEventArgs:EventArgs
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
