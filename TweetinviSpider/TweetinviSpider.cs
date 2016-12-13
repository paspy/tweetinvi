using System;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Tweetinvi;
using Tweetinvi.Models;

namespace IPaspy.TweetinviSpider {


    public class TweetinviSpider {
        // Default settings
        const uint MAX_CACHE_TWEETS = 20;
        const string
            API_SETTING_PATH        = "\\TweetinviSpider",
            USER_CACHE_SETTING_PATH = "\\TweetinviSpider\\Cache";

        // Hard code default credential for now
        const string
            DEFAULT_CONSUMER_KEY        = "IXofRdBjZyhwIK5sGvkHkCjOD",
            DEFAULT_CONSUMER_SECRET     = "T0cBe2w1TWfNA42HbLEE5LJjpzagn8ed8ceqsgKhLCOpFWTsYV",
            DEFAULT_ACCESS_TOKEN        = "808226521949159424-6F3yuUTK9g09PHDBWTm51nkLO0mebtO",
            DEFAULT_ACCESS_TOKEN_SECRET = "EyKFTO2DV253nLATCiOmDBgftgmwHPRcHR59HFzbFva20";


        private string
            m_sConsumerKey,
            m_sConsumerSecret,
            m_sAccessToken,
            m_sAccessTokenSecret;

        private Hashtable
            m_htUserCachedProfileImages,
            m_htUserCachedTweets;


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
            Auth.SetUserCredentials(m_sConsumerKey, m_sConsumerSecret, m_sAccessToken, m_sAccessTokenSecret);
            m_sLastMessage = "User credential has been set. (TweetinviSpider Constructor)";
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + USER_CACHE_SETTING_PATH);

        }

        public bool Initialize() {
            var curUser = User.GetAuthenticatedUser();
            if (curUser == null) {
                var latestException = ExceptionHandler.GetLastException();
                m_sLastMessage = "The following error occurred : " + latestException.TwitterDescription;
                return false;
            }
            m_htUserCachedProfileImages = new Hashtable();
            m_htUserCachedTweets = new Hashtable();
            var friendsOfCurUser = curUser.GetFriends();
            foreach (var friend in friendsOfCurUser) {
                UpdateUserProfileImages(friend.ScreenName);
            }

            m_sLastMessage = "TweetinviSpider has been initialized. (TweetinviSpider Initializer)";
            return true;
        }

        bool UpdateUserProfileImages(string userName) {
            var user = User.GetUserFromScreenName(userName);
            var stream = user.GetProfileImageStream(ImageSize.original);
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (assemblyPath != null) {

                string outputPath = Directory.GetCurrentDirectory() + USER_CACHE_SETTING_PATH + string.Format("\\{0}\\Profile", user.Id);
                Directory.CreateDirectory(outputPath);
                var fileStream = new FileStream(outputPath + string.Format("\\{0}.jpg", user.Id), FileMode.Create);
                stream.CopyTo(fileStream);
                return true;
            }

            return false;
        }

        bool UpdateUserLatestTweets() {
            return true;

        }

        public override string ToString() {
            return m_sLastMessage;
        }

        public string ConsumerKey { get { return m_sConsumerKey; } }
        public string ConsumerSecre { get { return m_sConsumerSecret; } }
        public string AccessToken { get { return m_sAccessToken; } }
        public string AccessTokenSecret { get { return m_sAccessTokenSecret; } }
    }

    public class TestingClass {
        /// <summary>
        /// Test program entry point
        /// </summary>
        public static void Main() {
            TweetinviSpider tApi = new TweetinviSpider();
            tApi.Initialize();
            Console.WriteLine("");
        }
    }
}
