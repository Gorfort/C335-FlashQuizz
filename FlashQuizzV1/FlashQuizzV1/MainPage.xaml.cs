using Microsoft.Maui.Controls;
using System;

namespace FlashQuizzV1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            // Navigate to the MenuPage using push navigation
            await Navigation.PushAsync(new Menu());
        }
    }
}
