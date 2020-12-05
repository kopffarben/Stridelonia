using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Platform;
using Avalonia.Platform.Interop;
using Avalonia.Shared.PlatformSupport;

namespace Avalonia
{
    static class StrideRuntimePlatformServices
    {
        public static void Register(Assembly assembly = null)
        {
            var standardPlatform = new StrideRuntimePlatform();
            AssetLoader.RegisterResUriParsers();
            AvaloniaLocator.CurrentMutable
                .Bind<IRuntimePlatform>().ToConstant(standardPlatform)
                .Bind<IAssetLoader>().ToConstant(new AssetLoader(assembly))
                .Bind<IDynamicLibraryLoader>().ToConstant(
#if __IOS__
                    new IOSLoader()
#else
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? (IDynamicLibraryLoader)new Win32Loader()
                        : new UnixLoader()
#endif
                );
        }
    }
}
