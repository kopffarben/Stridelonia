using Stride.Core;

namespace Stride.UI.Renderers
{
    internal class AvaloniaRenderer : ElementRenderer
    {
        public AvaloniaRenderer(IServiceRegistry services)
            : base(services)
        {

        }

        public override void RenderColor(UIElement element, UIRenderingContext context)
        {
            base.RenderColor(element, context);
        }
    }
}
