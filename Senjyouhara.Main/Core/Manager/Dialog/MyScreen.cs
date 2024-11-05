using System;
using System.Windows;
using Stylet;
using Stylet.Logging;

namespace Senjyouhara.Main.Core.Manager.Dialog;

public class MyScreen : Screen, IMyScreen
{
    private readonly ILogger logger;

    public event EventHandler<EventArgs> ViewLoaded;
    public UIElement View { get; private set; }
    
    public MyScreen() : this(null) { }

    public MyScreen(IModelValidator validator) : base(validator)
    {
        Type type = this.GetType();
        this.DisplayName = type.FullName;
        this.logger = LogManager.GetLogger(type);
    }

    void IViewAware.AttachView(UIElement view)
    {
        if (this.View != null)
            throw new InvalidOperationException(string.Format(
                "Tried to attach View {0} to ViewModel {1}, but it already has a view attached", view.GetType().Name,
                this.GetType().Name));

        View = view;

        logger.Info("Attaching view {0}", view);

        if (view is FrameworkElement viewAsFrameworkElement)
        {
            if (viewAsFrameworkElement.IsLoaded)
            {
                this.OnViewLoaded();
                ViewLoaded?.Invoke(viewAsFrameworkElement, EventArgs.Empty);
            }
            else
            {
                viewAsFrameworkElement.Loaded += (o, e) =>
                {
                    this.OnViewLoaded();
                    ViewLoaded?.Invoke(o, e);
                };
            }
        }
    }
}