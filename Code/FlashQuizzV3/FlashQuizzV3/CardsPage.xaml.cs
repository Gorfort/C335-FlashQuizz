using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using FlashQuizzV3.Models;
using FlashQuizzV3.Data;
using Microsoft.Maui.Controls;

namespace FlashQuizzV3
{
    public partial class CardsPage : ContentPage
    {
        private CardDatabase _database;

        public ObservableCollection<Card> Cards { get; set; }

        public CardsPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cards.db3");
            _database = new CardDatabase(dbPath);
            Cards = new ObservableCollection<Card>();
            BindingContext = this; 
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCardsAsync();
        }

        private async Task LoadCardsAsync()
        {
            var cards = await _database.GetCardsAsync();
            Cards.Clear();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var card = button?.CommandParameter as Card;
            if (card != null)
            {
                string updatedQuestion = await DisplayPromptAsync("Edit Question", "Enter the updated question", "OK", "Cancel", card.Question);
                if (!string.IsNullOrWhiteSpace(updatedQuestion)) // Check if the entered question is not null or empty
                {
                    string updatedAnswer = await DisplayPromptAsync("Edit Answer", "Enter the updated answer", "OK", "Cancel", card.Answer);
                    if (!string.IsNullOrWhiteSpace(updatedAnswer)) // Check if the entered answer is not null or empty
                    {
                        card.Question = updatedQuestion;
                        card.Answer = updatedAnswer;
                        await _database.UpdateCardAsync(card);
                        await LoadCardsAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Please enter a valid answer.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Please enter a valid question.", "OK");
                }
            }
        }


        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var card = button?.CommandParameter as Card;
            if (card != null)
            {
                if (await DisplayAlert("Delete Card", "Are you sure you want to delete this card?", "Yes", "No"))
                {
                    await _database.DeleteCardAsync(card);
                    await LoadCardsAsync(); // Refresh the list after deletion
                }
            }
        }

        private async void OnGoBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); 
        }
    }
}
