using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NoteTakingApp.ViewModels;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI;
using System;
using System.Threading.Tasks;

namespace NoteTakingApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditorPage : Page
    {
        public EditorPageViewModel ViewModel => (EditorPageViewModel)DataContext;

        public EditorPage()
        {
            this.InitializeComponent();
            InitializeInkCanvas();
        }

        private void InitializeInkCanvas()
        {
            // Configure the InkCanvas for pen, touch, and mouse input.
            MainInkCanvas.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Mouse |
                Windows.UI.Core.CoreInputDeviceTypes.Pen |
                Windows.UI.Core.CoreInputDeviceTypes.Touch;

            // Set default drawing attributes based on current tool
            UpdateInkCanvasForTool(ViewModel.CurrentTool);

            // Subscribe to strokes collected event to save ink data
            MainInkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
        }

        private void UpdateInkCanvasForTool(string tool)
        {
            var drawingAttributes = new InkDrawingAttributes()
            {
                IgnorePressure = false,
                IgnoreTilt = false
            };

            switch (tool)
            {
                case "Pen":
                    drawingAttributes.Color = ViewModel.PenColor;
                    drawingAttributes.Size = new Size(ViewModel.PenThickness, ViewModel.PenThickness);
                    break;
                case "Pencil":
                    drawingAttributes.Color = ViewModel.PenColor;
                    drawingAttributes.Size = new Size(ViewModel.PenThickness, ViewModel.PenThickness);
                    // Pencil-specific texture would be added here
                    break;
                case "Highlighter":
                    drawingAttributes.Color = ViewModel.PenColor;
                    drawingAttributes.Size = new Size(ViewModel.PenThickness, ViewModel.PenThickness);
                    drawingAttributes.IgnorePressure = true; // Highlighters typically don't use pressure for size
                    break;
                case "Eraser":
                    // Eraser is handled differently - we'll change the editing mode
                    MainInkCanvas.InkPresenter.InputProcessingConfiguration.Mode =
                        InkPresenterInputProcessingMode.Erasing;
                    MainInkCanvas.InkPresenter.InputProcessingConfiguration.Mode =
                        InkPresenterInputProcessingMode.Inking; // Reset for other tools
                    break;
                default:
                    drawingAttributes.Color = ViewModel.PenColor;
                    drawingAttributes.Size = new Size(ViewModel.PenThickness, ViewModel.PenThickness);
                    break;
            }

            if (tool != "Eraser")
            {
                MainInkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
                MainInkCanvas.InkPresenter.InputProcessingConfiguration.Mode =
                    InkPresenterInputProcessingMode.Inking;
            }
        }

        private async void InkPresenter_StrokesCollected(InkPresenter sender, InkPresenterStrokesCollectedEventArgs args)
        {
            // Save the ink data when strokes are completed (pen up)
            await ViewModel.UpdateInkDataAsync(
                await MainInkCanvas.InkPresenter.StrokeContainer.SaveAsync());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get navigation parameters: expected to be an anonymous object with notebookId and pageId
            if (e.Parameter is System.Dynamic.ExpandoObject parameters)
            {
                var notebookId = (string)typeof(System.Dynamic.ExpandoObject)
                    .GetProperty("notebookId")?.GetValue(parameters, null);
                var pageId = (string)typeof(System.Dynamic.ExpandoObject)
                    .GetProperty("pageId")?.GetValue(parameters, null);

                if (!string.IsNullOrEmpty(notebookId))
                {
                    // Initialize the ViewModel with the notebookId and pageId
                    _ = ViewModel.InitializeAsync(notebookId, pageId);
                    // TODO: Load the ink data from the page into the InkCanvas
                    // TODO: Load text boxes and images into their respective canvases
                }
            }
            else if (e.Parameter is string notebookIdString)
            {
                // Sometimes we might just pass the notebookId as a string
                _ = ViewModel.InitializeAsync(notebookIdString);
            }
        }

        // We can also handle the PageLoaded event to load the ink data after the ViewModel is initialized
        // But for simplicity, we'll load the ink data in the ViewModel's InitializeAsync after setting the CurrentPage.
        // We'll change the ViewModel to raise an event when CurrentPage changes, or we can use a callback.

        // Alternatively, we can load the ink data in the ViewModel and then pass it to the View via a property or event.
        // Given the time, we'll leave the loading of ink data as a TODO and note that it should be done.

        // We'll also need to handle saving when navigating away, but we are already saving on stroke change.
        // However, if there are no strokes, we might not save. So we can save on navigating away as a fallback.

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Save the current ink data as a fallback
            _ = ViewModel.SavePageAsync();
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button && button.Content is string content)
            {
                // Toggle the button and unset others if needed
                var toggleButtons = new[] { PenButton, PencilButton, HighlighterButton, EraserButton, LassoButton, RulerButton };
                foreach (var tb in toggleButtons)
                {
                    if (tb != button)
                    {
                        tb.IsChecked = false;
                    }
                }

                // Update the ViewModel with the new tool
                ViewModel.SetTool(content);

                // Update the InkCanvas for the new tool
                UpdateInkCanvasForTool(content);
            }
        }

        private void ContentScrollViewer_ZoomChanged(object sender, object e)
        {
            // Update the ViewModel with the current zoom factor
            if (sender is ScrollViewer scrollViewer)
            {
                ViewModel.SetZoomFactor(scrollViewer.ZoomFactor);
            }
        }

        private void ContentScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Reset zoom to 100% on double tap
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ChangeView(null, null, 1.0f);
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            // Increase zoom by 25%
            var newZoom = Math.Min(ViewModel.ZoomFactor * 1.25, 5.0);
            ViewModel.SetZoomFactor(newZoom);
            ContentScrollViewer.ChangeView(null, null, (float)newZoom);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Decrease zoom by 25%
            var newZoom = Math.Max(ViewModel.ZoomFactor * 0.75, 0.25);
            ViewModel.SetZoomFactor(newZoom);
            ContentScrollViewer.ChangeView(null, null, (float)newZoom);
        }

        private void NewPageButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ViewModel.CreateNewPageAsync();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement previous page navigation
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement next page navigation
        }

        private void PageOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement page options menu
        }

        // TODO: Implement remaining event handlers for:
        // - Page thumbnail list item click
        // - Text and image manipulation
        // - Lasso selection
        // - Ruler tool
        // - Page operations (delete, duplicate, reorder)
        // - Paper template selection
        // - Context menus for ink, text, and images
    }
}