﻿using Microsoft.JSInterop;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	/// <summary>
	/// <see href="https://getbootstrap.com/docs/5.1/components/collapse/">Bootstrap 5 Collapse</see> component.<br />
	/// Full documentation and demos: <see href="https://havit.blazor.eu/components/HxCollapse">https://havit.blazor.eu/components/HxCollapse</see>
	/// </summary>
	public partial class HxCollapse : IAsyncDisposable
	{
		/// <summary>
		/// Element ID. To be referenced by <see cref="HxCollapseToggleButton.CollapseTarget"/>.
		/// (Autogenerated GUID if not set explicitly.)
		/// </summary>
		[Parameter] public string Id { get; set; }

		/// <summary>
		/// Direction of the animation.
		/// Default is <see cref="CollapseDirection.Vertical"/>.
		/// </summary>
		[Parameter] public CollapseDirection CollapseDirection { get; set; }

		/// <summary>
		/// If parent is provided, then all collapsible elements under the specified parent will be closed when this collapsible item is shown.
		/// (Similar to traditional accordion behavior.)
		/// </summary>
		[Parameter] public string Parent { get; set; }

		/// <summary>
		/// Additional CSS class.
		/// </summary>
		[Parameter] public string CssClass { get; set; }

		/// <summary>
		/// Determines whether the collapse will be open or closed (expanded or collapsed) when first rendered.
		/// </summary>
		[Parameter] public bool InitiallyExpanded { get; set; }

		/// <summary>
		/// Content of the collapse.
		/// </summary>
		[Parameter] public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// This event is fired when a collapse element has been made visible to the user (will wait for CSS transitions to complete).
		/// </summary>
		[Parameter] public EventCallback<string> OnShown { get; set; }
		/// <summary>
		/// Triggers the <see cref="OnShown"/> event. Allows interception of the event in derived components.
		/// </summary>
		protected virtual Task InvokeOnShownAsync(string elementId) => OnShown.InvokeAsync(elementId);

		/// <summary>
		/// This event is fired when a collapse element has been hidden from the user (will wait for CSS transitions to complete).
		/// </summary>
		[Parameter] public EventCallback<string> OnHidden { get; set; }
		/// <summary>
		/// Triggers the <see cref="OnHidden"/> event. Allows interception of the event in derived components.
		/// </summary>
		protected virtual Task InvokeOnHiddenAsync(string elementId) => OnHidden.InvokeAsync(elementId);

		/// <summary>
		/// Additional attributes to be splatted onto an underlying <c>div</c> element.
		/// </summary>
		[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }


		[Inject] protected IJSRuntime JSRuntime { get; set; }

		private ElementReference collapseHtmlElement;
		private DotNetObjectReference<HxCollapse> dotnetObjectReference;
		private IJSObjectReference jsModule;
		private string defaultId = "hx" + Guid.NewGuid().ToString("N");
		private bool disposed;
		private bool isShown;
		private bool showInProgress;
		private bool hideInProgress;
		private bool initialized;

		/// <summary>
		/// Indicates ShowAsync() was called before OnAfterRenderAsync (DOM not initialized). Therefor we will remember the request for Show and initialize the collapse with toggle=true.
		/// </summary>
		private bool shouldToggle;

		public HxCollapse()
		{
			dotnetObjectReference = DotNetObjectReference.Create(this);
		}

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			await base.SetParametersAsync(parameters);

			// to be able to use another default value in ancestors (HxNavbarCollapse)
			this.Id = parameters.GetValueOrDefault(nameof(Id), defaultId);
		}

		/// <inheritdoc cref="ComponentBase.OnAfterRenderAsync(bool)" />
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			await base.OnAfterRenderAsync(firstRender);

			if (firstRender)
			{
				await EnsureJsModuleAsync();
				if (disposed)
				{
					return;
				}
				await jsModule.InvokeVoidAsync("initialize", collapseHtmlElement, dotnetObjectReference, shouldToggle);
				initialized = true;
			}
		}

		protected override void OnInitialized()
		{
			if (InitiallyExpanded)
			{
				isShown = true;
			}
		}

		/// <summary>
		/// Expands the item.
		/// </summary>
		public async Task ShowAsync()
		{
			if (initialized)
			{
				await EnsureJsModuleAsync();
				showInProgress = true;
				await jsModule.InvokeVoidAsync("show", collapseHtmlElement);
			}
			else
			{
				shouldToggle = true;
			}
		}

		/// <summary>
		/// Collapses the item.
		/// </summary>
		public async Task HideAsync()
		{
			await EnsureJsModuleAsync();
			hideInProgress = true;
			await jsModule.InvokeVoidAsync("hide", collapseHtmlElement);
		}

		/// <summary>
		/// Receives notification from javascript when item is about to start showing.
		/// </summary>
		[JSInvokable("HxCollapse_HandleJsShow")]
		public void HandleJsShow()
		{
			showInProgress = true;
		}

		/// <summary>
		/// Receives notification from javascript when item is shown.
		/// </summary>
		/// <remarks>
		/// the shown-event gets raised as the "show" CSS class is added to the HTML element and the transition is completed
		/// this covers both user-interaction (DOM state) and Blazor-interaction (HxAccordition.ExpandedItemId change)
		/// </remarks>
		[JSInvokable("HxCollapse_HandleJsShown")]
		public async Task HandleJsShown()
		{
			isShown = true;
			bool wasInProgress = showInProgress;
			showInProgress = false;
			await InvokeOnShownAsync(this.Id);

			if (wasInProgress)
			{
				// ShouldRender() disables rendering when transitioning from hidden to shown.
				// It therefore needs to be called explicitly after the event is handled.
				StateHasChanged();
			}
		}

		/// <summary>
		/// Receives notification from javascript when item is about to hide.
		/// </summary>
		[JSInvokable("HxCollapse_HandleJsHide")]
		public void HandleJsHide()
		{
			hideInProgress = true;
		}

		/// <summary>
		/// Receives notification from javascript when item is hidden.
		/// </summary>
		[JSInvokable("HxCollapse_HandleJsHidden")]
		public async Task HandleJsHidden()
		{
			isShown = false;
			bool wasInProgress = hideInProgress;
			hideInProgress = false;
			await InvokeOnHiddenAsync(this.Id);

			if (wasInProgress)
			{
				// ShouldRender() disables rendering when transitioning from shown to hidden.
				// It therefore needs to be called explicitly after the event is handled.
				StateHasChanged();
			}
		}

		private async Task EnsureJsModuleAsync()
		{
			jsModule ??= await JSRuntime.ImportHavitBlazorBootstrapModuleAsync(nameof(HxCollapse));
		}

		protected virtual string GetCssClass()
		{
			return CssClassHelper.Combine(
				"collapse",
				this.CollapseDirection == CollapseDirection.Horizontal ? "collapse-horizontal" : null,
				isShown ? "show" : null,
				this.CssClass);
		}

		protected override bool ShouldRender()
		{
			if (showInProgress || hideInProgress)
			{
				// do not re-render if the transition (animation) is in progress
				return false;
			}
			return base.ShouldRender();
		}

		/// <inheritdoc/>

		public async ValueTask DisposeAsync()
		{
			await DisposeAsyncCore();

			//Dispose(disposing: false);
		}

		protected virtual async ValueTask DisposeAsyncCore()
		{
			disposed = true;

			if (jsModule != null)
			{
#if NET6_0_OR_GREATER
				try
				{
					await jsModule.InvokeVoidAsync("dispose", collapseHtmlElement);
					await jsModule.DisposeAsync();
				}
				catch (JSDisconnectedException)
				{
					// NOOP
				}
#else
				await jsModule.InvokeVoidAsync("dispose", collapseHtmlElement);
				await jsModule.DisposeAsync();
#endif
			}

			dotnetObjectReference?.Dispose();
		}
	}
}
