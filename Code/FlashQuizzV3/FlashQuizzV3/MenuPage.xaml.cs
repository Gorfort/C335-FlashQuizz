namespace FlashQuizzV3
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private async void OnCardsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CardsPage());
        }

        private async void OnQuizClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QuizPage());
        }

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCardPage());
        }
    }
}
