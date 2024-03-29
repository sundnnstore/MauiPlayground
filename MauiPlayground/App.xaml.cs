﻿using ReloadPreview;

namespace MauiPlayground
{
    public partial class App : Application
    {
        const int WindowWidth = 500;
        const int WindowHeight = 500;

        public App()
        {
            InitializeComponent();

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
#if WINDOWS
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;

                nativeWindow.Activate();
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                appWindow.Resize(new Windows.Graphics.SizeInt32(WindowWidth, WindowHeight));
#endif
            });

            //页面预览
            HotReload.Instance.Init("192.168.0.144");
            HotReload.Instance.Reload += () =>
            {
                this.Dispatcher.Dispatch(() =>
                {
                    var view = HotReload.Instance.ReloadClass<MainPage>();
                    Console.WriteLine(view is null);
                    MainPage = view;
                });
            };
            MainPage = new MainPage();
        }
    }
}