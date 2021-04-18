using System;
using System.Linq;
using System.Windows;

namespace TheEliteUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IBackProvider _provider;

        public MainWindow()
            : this(new BackProvider())
        { }

        public MainWindow(IBackProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            InitializeComponent();
            RankingDatePicker.SelectedDateChanged += RankingDatePicker_SelectedDateChanged;
            RankingDatePicker.SelectedDate = DateTime.Now.Date;
        }

        private void RankingDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var itemsSource = _provider
                .GetRankingAsync(Model.Game.GoldenEye, RankingDatePicker.SelectedDate.Value)
                .GetAwaiter()
                .GetResult();

            RankingView.ItemsSource = itemsSource
                .Select((_ , r) => new ViewModel.RankingViewModel(_, r + 1))
                .ToList();
        }
    }
}
