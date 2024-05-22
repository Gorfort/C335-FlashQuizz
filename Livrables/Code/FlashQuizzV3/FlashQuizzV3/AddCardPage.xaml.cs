using Microsoft.Maui.Controls;
using System;
using System.IO;
using FlashQuizzV3.Models;
using FlashQuizzV3.Data;

namespace FlashQuizzV3
{
    public partial class AddCardPage : ContentPage
    {
        private CardDatabase _database;

        public AddCardPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cards.db3");
            _database = new CardDatabase(dbPath);
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            string question = QuestionEntry.Text;
            string answer = AnswerEntry.Text;

            if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
            {
                if (question.Length <= 30 && answer.Length <= 30)
                {
                    var card = new Card { Question = question, Answer = answer };
                    await _database.SaveCardAsync(card);
                    await DisplayAlert("Saved", "The question and answer have been saved.", "OK");

                    // Clear entries after saving
                    QuestionEntry.Text = string.Empty;
                    AnswerEntry.Text = string.Empty;
                }
                else
                {
                    await DisplayAlert("Error", "Question and answer must be 30 characters or less.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please fill in both fields.", "OK");
            }
        }

        private async void OnGoBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
