﻿#if __ANDROID__
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiLib.CustomControls.Platform;
using Android.Views;
using APointF = Android.Graphics.PointF;
using View = Android.Views.View;

namespace MauiLib.CustomControls.DrawableView
{
    public partial class DrawableViewHandler : ViewHandler<IDrawableView, PlatformDrawableView>
    {
        protected override PlatformDrawableView CreatePlatformView()
        {
            var nativeDrawableView = new PlatformDrawableView(Context)
            {
            };

            return nativeDrawableView;
        }

        protected override void ConnectHandler(PlatformDrawableView nativeView)
        {
            base.ConnectHandler(nativeView);

            nativeView.ViewAttachedToWindow += OnViewAttachedToWindow;
            nativeView.ViewDetachedFromWindow += OnViewDetachedFromWindow;
            nativeView.Touch += OnTouch;
            nativeView.PlatformMeasure += NativeView_PlatformMeasure;
            nativeView.PlatformDraw += OnDraw;
        }

        private void NativeView_PlatformMeasure(object sender, EventArgs e)
        {
            VirtualView?.WhenMeasure(sender, e);
        }

        protected override void DisconnectHandler(PlatformDrawableView nativeView)
        {
            base.DisconnectHandler(nativeView);

            nativeView.ViewAttachedToWindow -= OnViewAttachedToWindow;
            nativeView.ViewDetachedFromWindow -= OnViewDetachedFromWindow;
            nativeView.Touch -= OnTouch;
            nativeView.PlatformDraw -= OnDraw;
            nativeView.PlatformMeasure -= NativeView_PlatformMeasure;
        }

#region Map Method
        public static void MapInvalidate(DrawableViewHandler handler, IDrawableView drawableView, object? arg)
        {
            handler.PlatformView?.Invalidate();
        }

        void OnViewAttachedToWindow(object? sender, View.ViewAttachedToWindowEventArgs e)
        {
            VirtualView?.Load();
        }

        void OnViewDetachedFromWindow(object? sender, View.ViewDetachedFromWindowEventArgs e)
        {
            VirtualView?.Unload();
        }

        void OnTouch(object? sender, View.TouchEventArgs e)
        {
            if (e.Event == null)
                return;

            float density = Context?.Resources?.DisplayMetrics?.Density ?? 1.0f;
            APointF point = new APointF(e.Event.GetX() / density, e.Event.GetY() / density);

            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    VirtualView?.OnTouchDown(new Point(point.X, point.Y));
                    break;
                case MotionEventActions.Move:
                    VirtualView?.OnTouchMove(new Point(point.X, point.Y));
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    VirtualView?.OnTouchUp(new Point(point.X, point.Y));
                    break;
                default:
                    break;
            }
        }

        private void OnDraw(object sender, PlatformDrawEventArgs e)
        {
            VirtualView?.OnDraw(sender, e);
        }
#endregion
    }
}
#endif