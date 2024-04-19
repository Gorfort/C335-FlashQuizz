using Microsoft.Maui.Controls;

namespace FlashQuizzV1
{
    public partial class Menu : ContentPage
    {
        public Menu()
        {
            InitializeComponent();
        }

        private async void OnPopClicked(object sender, EventArgs e)
        {
            // Navigate back to the previous page using pop navigation
            await Navigation.PopAsync();
        }
    }
}
