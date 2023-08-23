using System.ComponentModel;
using System.Reflection;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace HackerNews;

// Fix Thank you to https://github.com/vhugogarcia
public class ShellWithLargeTitles : ShellRenderer
{
	public static IShellPageRendererTracker? Tracker { get; set; }

	protected override IShellPageRendererTracker CreatePageRendererTracker()
	{
		if (Tracker is not null)
			throw new InvalidOperationException("This should have been cleared out by CustomShellSectionRenderer");

		return Tracker = new CustomShellPageRendererTracker(this);
	}

	protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
	{
		return new CustomShellSectionRenderer(this);
	}
}


public class CustomShellSectionRootHeader : ShellSectionRootHeader
{
	volatile bool _isRotating;

	public CustomShellSectionRootHeader(IShellContext shellContext) : base(shellContext)
	{
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (_isRotating)
			Invoke(CollectionView.ReloadData, 0);

		_isRotating = false;
	}


	public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
	{
		base.ViewWillTransitionToSize(toSize, coordinator);
		_isRotating = true;
	}
}

public class CustomShellSectionRootRenderer : ShellSectionRootRenderer
{
	readonly CustomShellSectionRenderer _customShellSectionRenderer;

	UIEdgeInsets _additionalSafeArea = UIEdgeInsets.Zero;

	public CustomShellSectionRootRenderer(ShellSection shellSection, IShellContext shellContext, CustomShellSectionRenderer customShellSectionRenderer) : base(shellSection, shellContext)
	{
		ShellSection = shellSection;
		_customShellSectionRenderer = customShellSectionRenderer;
	}

	public ShellSection ShellSection { get; }

	public CustomShellSectionRootHeader? CustomShellSectionRootHeader { get; set; }

	IShellSectionController ShellSectionController => ShellSection;

	public override void AddChildViewController(UIViewController childController)
	{
		base.AddChildViewController(childController);
		UpdateAdditionalSafeAreaInsets(childController);
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();
		UpdateAdditionalSafeAreaInsets();
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();
		_customShellSectionRenderer.SnagTracker();
	}

	protected override IShellSectionRootHeader CreateShellSectionRootHeader(IShellContext shellContext)
	{
		return CustomShellSectionRootHeader = new CustomShellSectionRootHeader(shellContext);
	}

	protected override void LayoutRenderers()
	{
		base.LayoutRenderers();
		UpdateAdditionalSafeAreaInsets();
	}

	void UpdateAdditionalSafeAreaInsets()
	{
		if (OperatingSystem.IsIOSVersionAtLeast(11) && ChildViewControllers is not null)
		{
			ShellSectionController.GetItems();

			for (int i = 0; i < ChildViewControllers.Length; i++)
			{
				var shellContent = ChildViewControllers[i];
				UpdateAdditionalSafeAreaInsets(shellContent);
			}
		}
	}

	void UpdateAdditionalSafeAreaInsets(UIViewController viewController)
	{
		if (viewController is not PageViewController)
			return;

		if (ShellSectionController.GetItems().Count > 1)
		{
			_additionalSafeArea = new UIEdgeInsets(35, 0, 0, 0);
		}
		else
		{
			_additionalSafeArea = UIEdgeInsets.Zero;
		}

		if (OperatingSystem.IsIOSVersionAtLeast(11) && viewController is not null)
		{
			if (!viewController.AdditionalSafeAreaInsets.Equals(_additionalSafeArea))
				viewController.AdditionalSafeAreaInsets = _additionalSafeArea;
		}
	}
}

public class CustomShellSectionRenderer : ShellSectionRenderer
{
	readonly UINavigationControllerDelegate _navDelegate;
	readonly Dictionary<Element, IShellPageRendererTracker> _trackers = new();

	CustomShellSectionRootRenderer? _customShellSectionRootRenderer;

	public CustomShellSectionRenderer(IShellContext context) : base(context)
	{
		_navDelegate = (UINavigationControllerDelegate)Delegate;
		Delegate = new NavDelegate(_navDelegate, this);
		Context = context;
		UpdateLargeTitle();
	}

	public IShellContext Context { get; }

	public void SnagTracker()
	{
		if (ShellWithLargeTitles.Tracker is null)
			return;

		_trackers[ShellWithLargeTitles.Tracker.Page] = ShellWithLargeTitles.Tracker;
		ShellWithLargeTitles.Tracker = null;
	}

	protected override void HandleShellPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		base.HandleShellPropertyChanged(sender, e);

		if (e.PropertyName == ShellAttachedProperties.PrefersLargeTitlesProperty.PropertyName)
			UpdateLargeTitle();
	}

	protected override IShellSectionRootRenderer CreateShellSectionRootRenderer(ShellSection shellSection, IShellContext shellContext)
	{
		return _customShellSectionRootRenderer = new CustomShellSectionRootRenderer(shellSection, shellContext, this);
	}

	protected override void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
	{
		SnagTracker();
		base.OnNavigationRequested(sender, e);
		SnagTracker();
	}

	protected override void OnPushRequested(NavigationRequestedEventArgs e)
	{
		SnagTracker();
		base.OnPushRequested(e);
		SnagTracker();
	}

	protected override void OnInsertRequested(NavigationRequestedEventArgs e)
	{
		SnagTracker();
		base.OnInsertRequested(e);
		SnagTracker();
	}

	public override void PushViewController(UIViewController viewController, bool animated)
	{
		SnagTracker();
		base.PushViewController(viewController, animated);
		SnagTracker();
	}

	void UpdateLargeTitle()
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(11))
			return;

		var value = ShellAttachedProperties.GetPrefersLargeTitles(Shell.Current);
		ViewController.NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Always;
		NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Always;
		NavigationBar.PrefersLargeTitles = value;

		Invoke(() =>
		{
			if (_customShellSectionRootRenderer?.CustomShellSectionRootHeader is UICollectionViewController uvcvc)
			{
				uvcvc.CollectionView.ReloadData();
			}
		}, 0);
	}

	class NavDelegate : UINavigationControllerDelegate
	{
		readonly UINavigationControllerDelegate _navDelegate;
		readonly CustomShellSectionRenderer _self;

		public NavDelegate(UINavigationControllerDelegate navDelegate, CustomShellSectionRenderer customShellSectionRenderer)
		{
			_navDelegate = navDelegate;
			_self = customShellSectionRenderer;
		}

		// This is currently working around a Mono Interpreter bug
		// if you remove this code please verify that hot restart still works
		// https://github.com/xamarin/Xamarin.Forms/issues/10519
		[Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
		[Foundation.Preserve(Conditional = true)]
		public new static IUIViewControllerAnimatedTransitioning? GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
		{
			return null;
		}

		public override void DidShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
		{
			_navDelegate.DidShowViewController(navigationController, viewController, animated);
		}

		public override void WillShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
		{
			_navDelegate.WillShowViewController(navigationController, viewController, animated);

			// Because the back button title needs to be set on the previous VC
			// We want to set the BackButtonItem as early as possible so there is no flickering
			var currentPage = _self.Context?.Shell?.CurrentPage;
			var trackers = _self._trackers;
			if (currentPage?.Handler is IPlatformViewHandler pvh &&
				pvh.ViewController == viewController &&
				trackers.TryGetValue(currentPage, out var tracker) &&
				tracker is CustomShellPageRendererTracker shellRendererTracker)
			{
				shellRendererTracker.UpdateToolbarItemsInternal(false);
			}
		}
	}
}

public class CustomShellPageRendererTracker : ShellPageRendererTracker
{
	public CustomShellPageRendererTracker(IShellContext context) : base(context)
	{
		Context = context;
	}

	public IShellContext Context { get; }

	Page? ToolbarCurrentPage
	{
		get
		{
			var toolBar = ((IToolbarElement)Context.Shell).Toolbar;
			var toolbarType = toolBar?.GetType();
			var currentPageProperty = toolbarType?.GetField("_currentPage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var result = (Page?)currentPageProperty?.GetValue(toolBar);
			return result;

		}
	}

	internal void UpdateToolbarItemsInternal(bool updateWhenLoaded = true)
	{
		if (updateWhenLoaded && Page.IsLoaded || !updateWhenLoaded)
			UpdateToolbarItems();
	}

	protected override void UpdateToolbarItems()
	{
		base.UpdateToolbarItems();

		if (ViewController?.NavigationItem is null)
		{
			return;
		}

		UpdateBackButtonTitle();
	}

	protected override void UpdateTitle()
	{
		if (!IsToolbarReady())
			return;

		base.UpdateTitle();
	}

	protected override void UpdateTitleView()
	{
		if (!IsToolbarReady())
			return;

		if (ViewController?.NavigationItem is null)
		{
			return;
		}

		var titleView = Shell.GetTitleView(Page) ?? Shell.GetTitleView(Context.Shell);
		if (titleView is null)
		{
			var view = ViewController.NavigationItem.TitleView;
			ViewController.NavigationItem.TitleView = null;
			view?.Dispose();
		}
		else
		{
			var view = new CustomTitleViewContainer(titleView);
			ViewController.NavigationItem.TitleView = view;
		}
	}

	bool IsToolbarReady()
	{
		return ToolbarCurrentPage == Page;
	}

	void UpdateBackButtonTitle()
	{
		var behavior = Shell.GetBackButtonBehavior(Page);
		var text = behavior.GetPropertyIfSet<string?>(BackButtonBehavior.TextOverrideProperty, null);

		var navController = ViewController?.NavigationController;

		if (navController is not null)
		{
			var viewControllers = ViewController?.NavigationController?.ViewControllers ?? throw new InvalidOperationException($"{nameof(ViewController.NavigationController.ViewControllers)} cannot be null");
			var count = viewControllers.Length;

			if (count > 1 && viewControllers[count - 1] == ViewController)
			{
				var previousNavItem = viewControllers[count - 2].NavigationItem;
				if (previousNavItem is not null)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						var barButtonItem = previousNavItem.BackBarButtonItem ??= new UIBarButtonItem();
						barButtonItem.Title = text;
					}
					else if (previousNavItem.BackBarButtonItem is not null)
					{
						previousNavItem.BackBarButtonItem = null;
					}
				}
			}
		}
	}
}

public class CustomTitleViewContainer : UIContainerView
{
	public CustomTitleViewContainer(View view) : base(view)
	{
		MatchHeight = true;

		if (OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11))
		{
			TranslatesAutoresizingMaskIntoConstraints = false;
		}
		else
		{
			TranslatesAutoresizingMaskIntoConstraints = true;
			AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
		}
	}

	public override CGRect Frame
	{
		get => base.Frame;
		set
		{
			if (!(OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11)) && Superview is not null)
			{
				value.Y = Superview.Bounds.Y;
				value.Height = Superview.Bounds.Height;
			}

			base.Frame = value;
		}
	}

	public override CGSize IntrinsicContentSize => UILayoutFittingExpandedSize;

	public override CGSize SizeThatFits(CGSize size) => size;

	public override void LayoutSubviews()
	{
		if (Height is null or 0)
		{
			UpdateFrame(Superview);
		}

		base.LayoutSubviews();
	}

	public override void WillMoveToSuperview(UIView? newSuperView)
	{
		if (newSuperView is not null)
			UpdateFrame(newSuperView);

		base.WillMoveToSuperview(newSuperView);
	}

	void UpdateFrame(UIView newSuperView)
	{
		if (newSuperView.Bounds != CGRect.Empty)
		{
			if (!(OperatingSystem.IsIOSVersionAtLeast(11) || OperatingSystem.IsTvOSVersionAtLeast(11)))
				Frame = new CGRect(Frame.X, newSuperView.Bounds.Y, Frame.Width, newSuperView.Bounds.Height);

			Height = newSuperView.Bounds.Height;
		}
	}
}

public class UIContainerView : UIView
{
	readonly View _view;

	IPlatformViewHandler? _renderer;
	UIView? _platformView;
	bool _disposed;
	double _measuredHeight;

	public UIContainerView(View view)
	{
		_view = view;

		UpdatePlatformView();
		ClipsToBounds = true;
		MeasuredHeight = double.NaN;
		Margin = new Thickness(0);
	}

	public virtual Thickness Margin { get; }

	internal event EventHandler? HeaderSizeChanged;

	internal View View => _view;

	internal bool MatchHeight { get; set; }

	internal double MeasuredHeight
	{
		get
		{
			if (MatchHeight && Height is not null)
				return Height.Value;

			return _measuredHeight;
		}

		private set => _measuredHeight = value;
	}

	internal double? Height { get; set; }

	internal double? Width { get; set; }

	public override CGSize SizeThatFits(CGSize size)
	{
		var measuredSize = (_view as IView).Measure(size.Width, size.Height);

		if (Height is not null && MatchHeight)
		{
			MeasuredHeight = Height.Value;
		}
		else
		{
			MeasuredHeight = measuredSize.Height;
		}

		return new CGSize(size.Width, MeasuredHeight);
	}

	public override void WillRemoveSubview(UIView uiview)
	{
		UIContainerView.Disconnect();
		base.WillRemoveSubview(uiview);
	}

	public override void LayoutSubviews()
	{
		if (!IsPlatformViewValid())
			return;

		var height = Height ?? MeasuredHeight;
		var width = Width ?? Frame.Width;

		if (double.IsNaN(height))
			return;

		var platformFrame = new Rect(0, 0, width, height);


		if (MatchHeight)
			((IView)_view).Measure(width, height);

		((IView)_view).Arrange(platformFrame);
	}

	internal static void Disconnect()
	{
	}

	internal void UpdatePlatformView()
	{
		if (GetMauiContext(_view) is IMauiContext mauiContext)
		{
			_renderer = _view.ToHandler(mauiContext);
			_platformView = _renderer.ContainerView ?? _renderer.PlatformView;

			if (_platformView is not null && _platformView.Superview != this)
				AddSubview(_platformView);
		}
	}

	private protected void OnHeaderSizeChanged()
	{
		HeaderSizeChanged?.Invoke(this, EventArgs.Empty);
	}

	protected override void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		if (disposing)
		{
			UIContainerView.Disconnect();

			if (_platformView?.Superview == this)
				_platformView.RemoveFromSuperview();

			_renderer = null;
			_platformView = null;
			_disposed = true;
		}

		base.Dispose(disposing);
	}

	bool IsPlatformViewValid()
	{
		if (View is null || _platformView is null || _renderer is null)
			return false;

		return _platformView.Superview == this;
	}

	static IMauiContext? GetMauiContext(in Element? element, bool fallbackToAppMauiContext = false)
	{
		if (element is IElement mauiCoreElement && mauiCoreElement.Handler?.MauiContext != null)
			return mauiCoreElement.Handler.MauiContext;

		foreach (var parent in GetParentsPath(element))
		{
			if (parent is IElement parentView && parentView.Handler?.MauiContext != null)
				return parentView.Handler.MauiContext;
		}

		return fallbackToAppMauiContext ? GetMauiContext(Application.Current) : default;
	}

	static IEnumerable<Element> GetParentsPath(Element? self)
	{
		Element? current = self;

		while (!(current?.RealParent is not null || current?.RealParent is IApplication))
		{
			if (current is not null)
			{
				current = current.RealParent;
				yield return current;
			}
		}
	}
}

public static class ShellAttachedProperties
{
	public static readonly BindableProperty PrefersLargeTitlesProperty =
		BindableProperty.Create("PrefersLargeTitles", typeof(bool), typeof(ShellAttachedProperties), false);

	public static bool GetPrefersLargeTitles(BindableObject element) => (bool)element.GetValue(PrefersLargeTitlesProperty);

	public static void SetPrefersLargeTitles(BindableObject element, bool value)
	{
		element.SetValue(PrefersLargeTitlesProperty, value);
	}

	public static IPlatformElementConfiguration<iOS, Shell> SetPrefersLargeTitles(this IPlatformElementConfiguration<iOS, Shell> config, bool value)
	{
		SetPrefersLargeTitles(config.Element, value);
		return config;
	}

	public static bool PrefersLargeTitles(this IPlatformElementConfiguration<iOS, Shell> config)
	{
		return GetPrefersLargeTitles(config.Element);
	}
}