using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Events;
using System.Threading.Tasks;

namespace IPaspy.TweetinviSpider {


    public class TweetinviSpider {
        // Default settings
        const int MAX_CACHE_TWEETS = 20;
        const int REQUEST_TIMEOUT = 5000;
        const int MAX_FETCH_TIME = 300000;
        const string
            API_SETTING_PATH = "\\TweetinviSpider",
            USER_CACHE_SETTING_PATH = "\\TweetinviSpider\\Cache";

        // Hard code default credential for now
        const string
            DEFAULT_CONSUMER_KEY = "IXofRdBjZyhwIK5sGvkHkCjOD",
            DEFAULT_CONSUMER_SECRET = "T0cBe2w1TWfNA42HbLEE5LJjpzagn8ed8ceqsgKhLCOpFWTsYV",
            DEFAULT_ACCESS_TOKEN = "808226521949159424-6F3yuUTK9g09PHDBWTm51nkLO0mebtO",
            DEFAULT_ACCESS_TOKEN_SECRET = "EyKFTO2DV253nLATCiOmDBgftgmwHPRcHR59HFzbFva20";


        private string
            m_sConsumerKey,
            m_sConsumerSecret,
            m_sAccessToken,
            m_sAccessTokenSecret;

        private Dictionary<string, List<RawTweet>>  m_dictRawTweets;

        private Tweetinvi.Streaming.IUserStream     m_tSpiderStream;

        private Task<bool> m_asycSpiderStreamTask;

        private string m_sLastMessage;

        public TweetinviSpider(
            string _consumerKey = DEFAULT_CONSUMER_KEY,
            string _consumerSecret = DEFAULT_CONSUMER_SECRET,
            string _accessToken = DEFAULT_ACCESS_TOKEN,
            string _accessTokenSecret = DEFAULT_ACCESS_TOKEN_SECRET) {
            m_sConsumerKey = _consumerKey;
            m_sConsumerSecret = _consumerSecret;
            m_sAccessToken = _accessToken;
            m_sAccessTokenSecret = _accessTokenSecret;
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + USER_CACHE_SETTING_PATH);

        }

        public bool Initialize() {
            Auth.SetUserCredentials(m_sConsumerKey, m_sConsumerSecret, m_sAccessToken, m_sAccessTokenSecret);
            var curUser = User.GetAuthenticatedUser();
            if (curUser == null) {
                var latestException = ExceptionHandler.GetLastException();
                m_sLastMessage = "The following error occurred : " + latestException.TwitterDescription;
                return false;
            }
            var friendsOfCurUser = curUser.GetFriends();
            m_dictRawTweets = new Dictionary<string, List<RawTweet>>();
            foreach (var friend in friendsOfCurUser) {
                if (!CacheUserProfileImages(friend.ScreenName) || !CacheUserLatestTweets(friend.ScreenName)) {
                    return false;
                }
            }
            m_tSpiderStream = Tweetinvi.Stream.CreateUserStream();

            //StartStream();
            m_tSpiderStream.TweetCreatedByFriend += M_tSpiderStream_TweetCreatedByFriend;
            Task<bool> listenAsync = Sync.ExecuteTaskAsync(StartStream);
            m_sLastMessage = "TweetinviSpider has been initialized. (TweetinviSpider Initializer)";
            return true;
        }
        public void ResetSpider() {

        }
        public override string ToString() {
            return m_sLastMessage;
        }

        public bool StartStream() {
            if (m_tSpiderStream.StreamState == StreamState.Stop) {
                
                m_tSpiderStream.StartStream();
                return true;
            }
            return false;
        }

        public bool PauseStream() {
            if (m_tSpiderStream.StreamState == StreamState.Running) {
                m_tSpiderStream.PauseStream();
                return true;
            }
            return false;
        }
        public bool ResumeStream() {
            if (m_tSpiderStream.StreamState == StreamState.Pause) {
                m_tSpiderStream.ResumeStream();
                return true;
            }
            return false;
        }

        private void M_tSpiderStream_TweetCreatedByFriend(object sender, TweetReceivedEventArgs args) {
            var name = args.Tweet.CreatedBy.Name;
            Console.WriteLine("Tweet created by {0} at {1}", args.Tweet.CreatedBy.Name, args.Tweet.CreatedAt);

            CacheUserLatestTweets(name);
        }

        

        public string ConsumerKey { get { return m_sConsumerKey; } }
        public string ConsumerSecre { get { return m_sConsumerSecret; } }
        public string AccessToken { get { return m_sAccessToken; } }
        public string AccessTokenSecret { get { return m_sAccessTokenSecret; } }

        bool CacheUserProfileImages(string userName) {
            var user = User.GetUserFromScreenName(userName);
            var originalStream = user.GetProfileImageStream(ImageSize.original);
            var normalStream = user.GetProfileImageStream(ImageSize.normal);
            if (user == null || originalStream == null || normalStream == null) {
                var latestException = ExceptionHandler.GetLastException();
                m_sLastMessage = "The following error occurred : " + latestException.TwitterDescription;
                return false;
            }
            string outputPath = Directory.GetCurrentDirectory() + USER_CACHE_SETTING_PATH + string.Format("\\{0}", user.Id);
            Directory.CreateDirectory(outputPath);
            var originalfileStream = new FileStream(outputPath + string.Format("\\{0}_orginal.jpg", user.Id), FileMode.Create);
            var normalfileStream = new FileStream(outputPath + string.Format("\\{0}_normal.jpg", user.Id), FileMode.Create);
            originalStream.CopyTo(originalfileStream);
            normalStream.CopyTo(normalfileStream);
            return true;
        }

        bool CacheUserLatestTweets(string userName) {
            m_dictRawTweets.Clear();
            var user = User.GetUserFromScreenName(userName);
            var timelineTweets = user.GetUserTimeline(MAX_CACHE_TWEETS);
            if (user == null || timelineTweets == null) {
                var latestException = ExceptionHandler.GetLastException();
                m_sLastMessage = "The following error occurred : " + latestException.TwitterDescription;
                return false;
            }
            m_dictRawTweets.Add(userName, new List<RawTweet>());
            foreach (var tweet in timelineTweets) {
                RawTweet rawTweet = new RawTweet(tweet.CreatedBy.Id, tweet.CreatedBy.ScreenName, tweet.CreatedBy.Name);
                rawTweet.TweetContent = tweet.Text;
                rawTweet.TweetUrl = tweet.Url;
                int numOfMedias = tweet.Entities.Medias.Count;
                int numOfUrls = tweet.Entities.Urls.Count;
                if (numOfMedias > 0) {
                    rawTweet.TweetMediaUrls = new string[numOfMedias];
                    for (int i = 0; i < numOfMedias; i++) {
                        rawTweet.TweetMediaUrls[i] = tweet.Entities.Medias[i].MediaURL;
                    }
                }
                if (numOfUrls > 0) {
                    rawTweet.TweetEntityUrls = new string[numOfUrls];
                    for (int i = 0; i < numOfUrls; i++) {
                        rawTweet.TweetEntityUrls[i] = tweet.Entities.Urls[i].URL;
                    }
                }
                DateTime tweetedTimeLocal = tweet.CreatedAt;
                //var tokyoTime = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                rawTweet.TweetedTimeUTC = TimeZoneInfo.ConvertTimeToUtc(tweetedTimeLocal);
                m_dictRawTweets[userName].Add(rawTweet);
            }
            return true;
        }


    }

    public class TestingClass {
        /// <summary>
        /// Test program entry point
        /// </summary>
        public static void Main() {
            TweetinviSpider tApi = new TweetinviSpider();
            tApi.Initialize();
            //var t = new Thread(() => {

            //});
            //t.Start();
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            tApi.PauseStream();
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
