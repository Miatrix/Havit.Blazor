﻿using Microsoft.Extensions.Localization;

namespace Havit.Blazor.Components.Web.Bootstrap;

/// <summary>
/// Sidebar component - responsive navigation sidebar.<br />
/// Full documentation and demos: <see href="https://havit.blazor.eu/components/HxSidebar">https://havit.blazor.eu/components/HxSidebar</see>
/// </summary>
public partial class HxSidebar : ComponentBase
{
	/// <summary>
	/// Sidebar header.
	/// </summary>
	[Parameter] public RenderFragment HeaderTemplate { get; set; }

	/// <summary>
	/// Sidebar items. Use <see cref="HxSidebarItem"/>.
	/// </summary>
	[Parameter] public RenderFragment ItemsTemplate { get; set; }

	/// <summary>
	/// Sidebar footer (e.g. logged user, language switch, ...).
	/// </summary>
	[Parameter] public RenderFragment<SidebarFooterTemplateContext> FooterTemplate { get; set; }

	/// <summary>
	/// Vertical toggler (desktop version) to be rendered instead of the default bar/arrow.
	/// </summary>
	[Parameter] public RenderFragment<SidebarTogglerTemplateContext> TogglerTemplate { get; set; }

	/// <summary>
	/// Additional CSS class.
	/// </summary>
	[Parameter] public string CssClass { get; set; }

	/// <summary>
	/// ID of the root sidebar HTML element.
	/// (Autogenerated if not set.)
	/// </summary>
	[Parameter] public string Id { get; set; } = "hx-" + Guid.NewGuid().ToString("N");

	/// <summary>
	/// Indicates whether the <see cref="HxSidebar"/> is collapsed, can be used to alter the state (expand or collapse the sidebar).
	/// </summary>
	[Parameter] public bool Collapsed { get; set; } = false;
	/// <summary>
	/// Fires when the sidebar is expanded or collapsed.
	/// </summary>
	[Parameter] public EventCallback<bool> CollapsedChanged { get; set; }
	/// <summary>
	/// Triggers the <see cref="CollapsedChanged"/> event. Allows interception of the event in derived components.
	/// </summary>
	protected virtual Task InvokeCollapsedChangedAsync(bool collapsed) => CollapsedChanged.InvokeAsync(collapsed);

	/// <summary>
	/// Whether multiple items can be in expanded state at once.
	/// If set to <c>false</c>, upon item expansion, all other items are collapsed.
	/// Default is <c>true</c>.
	/// </summary>
	[Parameter] public bool MultipleItemsExpansion { get; set; } = true;

	/// <summary>
	/// Breakpoint below which the sidebar switches to mobile version (exclusive).<br/>
	/// Default is <see cref="SidebarResponsiveBreakpoint.Medium"/>.
	/// </summary>
	[Parameter] public SidebarResponsiveBreakpoint ResponsiveBreakpoint { get; set; } = SidebarResponsiveBreakpoint.Medium;

	[Inject] protected IStringLocalizer<HxSpinner> Localizer { get; set; }

	protected internal string NavContentElementId => Id + "-nav-content";

	/// <summary>
	/// Id of the immediate parent of the contained <see cref="HxSidebarItem"/> components.
	/// </summary>
	internal string navId = "hx" + Guid.NewGuid().ToString("N");

	private string GetCollapsedCssClass() => Collapsed ? "collapsed" : null;

	private async Task HandleCollapseToggleClick()
	{
		Collapsed = !Collapsed;
		await InvokeCollapsedChangedAsync(Collapsed);
	}

	private string GetResponsiveCssClass(string cssClassPattern)
	{
		return this.ResponsiveBreakpoint switch
		{
			SidebarResponsiveBreakpoint.None => cssClassPattern.Replace("-??-", "-"), // !!! Simplified for the use case of this component.
			SidebarResponsiveBreakpoint.Small => cssClassPattern.Replace("??", "sm"),
			SidebarResponsiveBreakpoint.Medium => cssClassPattern.Replace("??", "md"),
			SidebarResponsiveBreakpoint.Large => cssClassPattern.Replace("??", "lg"),
			SidebarResponsiveBreakpoint.ExtraLarge => cssClassPattern.Replace("??", "xl"),
			SidebarResponsiveBreakpoint.Xxl => cssClassPattern.Replace("??", "xxl"),
			_ => throw new InvalidOperationException($"Unknown nameof(ResponsiveBreakpoint) value {this.ResponsiveBreakpoint}")
		};
	}
}
