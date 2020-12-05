﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stridelonia;
using Stridelonia.Implementation;

namespace Stride.UI.Controls
{
    public class WindowElement : UIElement, IWindowImpl
    {
        private WindowState _windowState;
        public WindowState WindowState { get => _windowState; set => SetWindowState(value); }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Action GotInputWhenDisabled { get; set; }
        public Func<bool> Closing { get; set; }

        public bool IsClientAreaExtendedToDecorations => false;

        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        public bool NeedsManagedDecorations => true;

        public Avalonia.Thickness ExtendedMargins => new Avalonia.Thickness(0);

        public Avalonia.Thickness OffScreenMargin => new Avalonia.Thickness(0);

        public double DesktopScaling => 1;

        public PixelPoint Position { get; private set; }

        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }

        public IPlatformHandle Handle => new PlatformHandle(IntPtr.Zero, "No Handle");

        private Size _minSize;
        private Size _maxSize;
        public Size MaxAutoSizeHint => _maxSize;

        public IScreenImpl Screen { get; }

        public Size ClientSize => new Size(Width, Height);

        public double RenderScaling => 1;

        public IEnumerable<object> Surfaces { get; }

        public Action<RawInputEventArgs> Input { get; set; }
        public Action<Rect> Paint { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<double> ScalingChanged { get; set; }
        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }
        public Action Closed { get; set; }
        public Action LostFocus { get; set; }

        public IMouseDevice MouseDevice { get; }

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.Transparent;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new AcrylicPlatformCompensationLevels();

        internal IInputRoot InputRoot { get; set; }
        internal UIComponent LinkedComponent { get; set; }

        private readonly StrideExternalRenderTarget renderTarget;
        public Texture Texture => renderTarget.Texture;

        public WindowElement()
        {
            Screen = new ScreenImpl();
            MouseDevice = new MouseDevice(new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true));

            var game = AvaloniaLocator.Current.GetService<IGame>();
            renderTarget = new StrideExternalRenderTarget(game.GraphicsDevice)
            {
                ClientSize = ClientSize
            };
            Surfaces = new object[] { renderTarget };
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void CanResize(bool value)
        {

        }

        public IPopupImpl CreatePopup() => null;

        public IRenderer CreateRenderer(IRenderRoot root)
        {
            var options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();
            IRenderer renderer;
            if (options.UseDeferredRendering)
                renderer = new DeferredRenderer(root, AvaloniaLocator.Current.GetService<IRenderLoop>());
            else
                renderer = new ImmediateRenderer(root);

            renderer.DrawFps = options.DrawFps;
            return renderer;
        }

        public void Dispose()
        {

        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
            ulong timestamp = (ulong)(Environment.TickCount & int.MaxValue);
            Input?.Invoke(new RawPointerEventArgs(MouseDevice, timestamp, InputRoot,
                RawPointerEventType.LeaveWindow, new Avalonia.Point(-1, -1), RawInputModifiers.None));
        }

        public void Invalidate(Rect rect)
        {
            Dispatcher.UIThread.Post(() => Paint?.Invoke(rect));
        }

        public void Move(PixelPoint point)
        {
            Position = point;
            PositionChanged?.Invoke(point);
        }

        public Avalonia.Point PointToClient(PixelPoint point)
        {
            return point.ToPoint(1);
        }

        public PixelPoint PointToScreen(Avalonia.Point point)
        {
            return PixelPoint.FromPoint(point, 1);
        }

        public void Resize(Size clientSize)
        {
            Width = (float)clientSize.Width;
            Height = (float)clientSize.Height;
            renderTarget.ClientSize = ClientSize;
            renderTarget.DestroyRenderTarget();
            Resized?.Invoke(clientSize);
        }

        public void SetCursor(IPlatformHandle cursor)
        {
        }

        public void SetEnabled(bool enable)
        {
            IsEnabled = enable;
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            throw new NotImplementedException();
        }

        public void SetIcon(IWindowIconImpl icon)
        {
        }

        public void SetInputRoot(IInputRoot inputRoot) => InputRoot = inputRoot;

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            _minSize = minSize;
            _maxSize = maxSize;
        }

        public void SetParent(IWindowImpl parent)
        {
            throw new NotImplementedException();
        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {

        }

        public void SetTitle(string title)
        {
            Name = title;
        }

        public void SetTopmost(bool value)
        {
            Depth = 0;
        }

        public void SetTransparencyLevelHint(WindowTransparencyLevel transparencyLevel)
        {

        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void ShowTaskbarIcon(bool value)
        {

        }

        private void SetWindowState(WindowState state)
        {
            _windowState = state;
            WindowStateChanged?.Invoke(state);
            switch (state)
            {
                case WindowState.FullScreen:
                case WindowState.Maximized:
                    var size = Screen.AllScreens[0].Bounds.Size;
                    Width = size.Width;
                    Height = size.Height;
                    renderTarget.ClientSize = ClientSize;
                    renderTarget.DestroyRenderTarget();
                    Resized?.Invoke(ClientSize);
                    Position = new PixelPoint(0, 0);
                    PositionChanged?.Invoke(Position);
                    break;
            }
        }
    }
}
