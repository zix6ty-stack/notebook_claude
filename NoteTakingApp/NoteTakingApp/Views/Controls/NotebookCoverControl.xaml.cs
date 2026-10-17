using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NoteTakingApp.Models;

namespace NoteTakingApp.Controls
{
    public sealed partial class NotebookCoverControl : UserControl
    {
        public NotebookCoverControl()
        {
            this.InitializeComponent();
        }

        public Notebook Notebook
        {
            get { return (Notebook)GetValue(NotebookProperty); }
            set { SetValue(NotebookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notebook.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotebookProperty =
            DependencyProperty.Register("Notebook", typeof(Notebook), typeof(NotebookCoverControl), new PropertyMetadata(null));
    }
}