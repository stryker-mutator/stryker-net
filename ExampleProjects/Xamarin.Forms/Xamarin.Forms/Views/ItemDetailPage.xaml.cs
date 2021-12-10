using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.ViewModels;

namespace Xamarin.Forms.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}