using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Stride.Rendering;

namespace Avalonia
{
    public sealed class StrideBuilder : AppBuilderBase<StrideBuilder>
    {
        public StrideBuilder()
            : base(new StrideRuntimePlatform(), builder => StrideRuntimePlatformServices.Register(builder.ApplicationType.Assembly))
        {

        }
    }
}
