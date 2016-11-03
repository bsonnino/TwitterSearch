using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using TweetSharp;

namespace TweetSearchWP
{
    public partial class MainPage
    {
        private readonly ObservableCollection<TwitterStatus> _tweets;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _tweets = new ObservableCollection<TwitterStatus>();
        }

        private void PesquisaClick(object sender, RoutedEventArgs e)
        {
            _tweets.Clear();
            TwitterService service = AutenticaTwitter();
            PesquisaTexto(service);
        }

        private TwitterService AutenticaTwitter()
        {
            var service = new TwitterService(oAuthConsumerKey, oAuthConsumerSecret);
            service.AuthenticateWith(token, tokenSecret);
            return service;
        }

        private void PesquisaTexto(TwitterService service)
        {
            Dispatcher dispatcher = Deployment.Current.Dispatcher;
            service.Search(new SearchOptions
                           {
                               Count = 50,
                               Q = Uri.EscapeDataString(txtSearch.Text)
                           },
                (result, response) =>
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        foreach (TwitterStatus status in result.Statuses)
                        {
                            TwitterStatus tweet = status;
                            dispatcher.BeginInvoke(() => _tweets.Add(tweet));
                        }
                        dispatcher.BeginInvoke(() => lbxResults.ItemsSource = _tweets);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }
                });
        }

        #region Keys
        // These keys should be filled with your own

        private string oAuthConsumerKey = "";
        private string oAuthConsumerSecret = "";
        private string token = "";
        private string tokenSecret = "";

        #endregion
    }
}