using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlashQuizzV3.Data;
using FlashQuizzV3.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace FlashQuizzV3
{
    public partial class QuizPage : ContentPage
    {
        private CardDatabase _database;
        private List<Card> _cards;
        private int _currentIndex = 0;
        private int _yesCount = 0;
        private int _noCount = 0;
        private int _respondedQuestionsCount = 0; // Track the number of responded questions
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _isQuizEnded = false;

        private List<Color> _backgroundColors = new List<Color>
        {
            Colors.LightGray,
            Colors.LightBlue,
            Colors.LightGreen,
            Colors.LightPink
        };

        public bool IsQuizEnded
        {
            get { return _isQuizEnded; }
            set
            {
                _isQuizEnded = value;
                OnPropertyChanged(nameof(IsQuizEnded)); // Notify property changed
            }
        }

        public QuizPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cards.db3");
            _database = new CardDatabase(dbPath);
            LoadCards();
        }

        private async void LoadCards()
        {
            _cards = await _database.GetCardsAsync();
            if (_cards.Count > 0)
            {
                _cards = _cards.OrderBy(c => Guid.NewGuid()).ToList(); // Shuffle the cards list
                DisplayCurrentCard();
                _startTime = DateTime.Now; // Start time when the quiz begins

                // Reset incorrect count for all cards
                await _database.ResetIncorrectCountAsync();
            }
            else
            {
                await DisplayAlert("No Cards", "There are no cards available.", "OK");
            }
        }

        private void DisplayCurrentCard()
        {
            if (_currentIndex < _cards.Count)
            {
                var currentCard = _cards[_currentIndex];
                QuestionLabel.Text = currentCard.Question;
                AnswerLabel.Text = currentCard.Answer;
                AnswerLabel.IsVisible = false;
                ButtonsLayout.IsVisible = false;
                RevealAnswerButton.IsVisible = true; // Show the "Reveal Answer" button
                ChangeFrameBackgroundColor(); // Change the frame background color
            }
            else
            {
                EndQuiz();
            }
        }

        private void ChangeFrameBackgroundColor()
        {
            Random random = new Random();
            int index = random.Next(_backgroundColors.Count);
            QuestionFrame.BackgroundColor = _backgroundColors[index];
        }

        private async void OnYesButtonClicked(object sender, EventArgs e)
        {
            if (_currentIndex < _cards.Count)
            {
                _yesCount++;
                _respondedQuestionsCount++; // Increment the count of responded questions
                _cards[_currentIndex].KnowledgeLevel++; // Increase knowledge level
                await _database.UpdateCardAsync(_cards[_currentIndex]); // Update card in the database
                _currentIndex++;
                DisplayCurrentCard();
            }
        }

        private async void OnNoButtonClicked(object sender, EventArgs e)
        {
            if (_currentIndex < _cards.Count)
            {
                _noCount++;
                _respondedQuestionsCount++; // Increment the count of responded questions

                // Increment the incorrect count for the current card
                _cards[_currentIndex].IncorrectCount++;

                // Decrease knowledge level for incorrect answers, ensuring it doesn't go below 0
                if (_cards[_currentIndex].KnowledgeLevel > 0)
                {
                    _cards[_currentIndex].KnowledgeLevel--;
                    await _database.UpdateCardAsync(_cards[_currentIndex]); // Update card in the database
                }

                // Move the current card to the end of the list
                var currentCard = _cards[_currentIndex];
                _cards.RemoveAt(_currentIndex);
                _cards.Add(currentCard);

                DisplayCurrentCard();
            }
        }

        private void EndQuiz()
        {
            _endTime = DateTime.Now; // End time when the quiz is completed
            TimeSpan duration = _endTime - _startTime;
            double totalTimeMinutes = duration.TotalMinutes;
            double correctPercentage = (_yesCount / (double)(_yesCount + _noCount)) * 100;

            // Find the card with the maximum incorrect count
            var mostIncorrectCard = _cards.OrderByDescending(c => c.IncorrectCount).FirstOrDefault();

            QuestionLabel.Text = "Quiz Completed!";
            AnswerLabel.Text = $"Time spent: {totalTimeMinutes:F2} minutes\n" +
                               $"Percentage of correct answers: {correctPercentage:F2}%\n" +
                               $"Correct Answers: {_yesCount}\n" +
                               $"Incorrect Answers: {_noCount}\n" +
                               $"Responded Questions: {_respondedQuestionsCount}\n";

            // Include the most incorrectly answered question details
            if (mostIncorrectCard != null)
            {
                AnswerLabel.Text += $"\nMost incorrect question: {mostIncorrectCard.Question}\n" +
                                    $"Incorrect count: {mostIncorrectCard.IncorrectCount}";
            }

            AnswerLabel.IsVisible = true;
            ButtonsLayout.IsVisible = false;
            IsQuizEnded = true; // Set IsQuizEnded to true when the quiz ends
            RevealAnswerButton.IsVisible = false; // Hide the "Reveal Answer" button

            // Hide the "Stop Quiz" button and InstructionLabel
            StopButton.IsVisible = false;
            InstructionLabel.IsVisible = false;
        }

        private void OnStopButtonClicked(object sender, EventArgs e)
        {
            // Calculate quiz results
            _endTime = DateTime.Now; // Record end time when the quiz is stopped
            TimeSpan duration = _endTime - _startTime;
            double totalTimeMinutes = duration.TotalMinutes;
            double correctPercentage = (_yesCount + _noCount) > 0 ? (_yesCount / (double)(_yesCount + _noCount)) * 100 : 0;

            // Find the card with the maximum incorrect count
            var mostIncorrectCard = _cards.OrderByDescending(c => c.IncorrectCount).FirstOrDefault();

            // Display quiz results
            QuestionLabel.Text = "Quiz Ended";
            AnswerLabel.Text = $"Time spent: {totalTimeMinutes:F2} minutes\n" +
                               $"Total questions: {_cards.Count}\n" +
                               $"Responded questions: {_respondedQuestionsCount}\n" +
                               $"Correct answers: {_yesCount}\n" +
                               $"Incorrect answers: {_noCount}\n" +
                               $"Percentage of correct answers: {correctPercentage:F2}%\n";

            // Include the most incorrectly answered question details
            if (mostIncorrectCard != null)
            {
                AnswerLabel.Text += $"\nMost incorrect question: {mostIncorrectCard.Question}\n" +
                                    $"Incorrect count: {mostIncorrectCard.IncorrectCount}";
            }

            AnswerLabel.IsVisible = true;
            ButtonsLayout.IsVisible = false;
            StopButton.IsVisible = false;
            IsQuizEnded = true; // Set IsQuizEnded to true when the quiz ends

            // Hide the InstructionLabel
            InstructionLabel.IsVisible = false;

            // Hide the Reveal Answer button
            RevealAnswerButton.IsVisible = false;
        }

        private async void OnGoBackClicked(object sender, EventArgs e)
        {
            // Reset most incorrect question and its incorrect count
            var mostIncorrectCard = _cards.OrderByDescending(c => c.IncorrectCount).FirstOrDefault();
            if (mostIncorrectCard != null)
            {
                mostIncorrectCard.IncorrectCount = 0;
                await _database.UpdateCardAsync(mostIncorrectCard);
            }

            // Navigate back
            await Navigation.PopAsync();
        }


        private void OnRevealAnswerClicked(object sender, EventArgs e)
        {
            if (_currentIndex < _cards.Count)
            {
                ButtonsLayout.IsVisible = true;
                AnswerLabel.IsVisible = true;
            }
        }

        // Shake detection logic
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Accelerometer.ShakeDetected += OnShakeDetected;
            Accelerometer.Start(SensorSpeed.UI);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Accelerometer.ShakeDetected -= OnShakeDetected;
            Accelerometer.Stop();
        }

        private void OnShakeDetected(object sender, EventArgs e)
        {
            ShuffleRemainingCards();
        }

        private void ShuffleRemainingCards()
        {
            if (_currentIndex < _cards.Count - 1)
            {
                var remainingCards = _cards.Skip(_currentIndex).OrderBy(c => Guid.NewGuid()).ToList();
                _cards = _cards.Take(_currentIndex).Concat(remainingCards).ToList();
                DisplayCurrentCard();
            }
        }
    }
}
