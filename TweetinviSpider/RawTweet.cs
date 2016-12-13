using System;


namespace IPaspy.TweetinviSpider {
    public class RawTweet {

        private Guid m_tUUID;
        private string
            m_sNickName,
            m_sScreenName,
            m_sTweetContent,
            m_sTweetUrl;

        private string[]
            m_sTweetMediaUrls,
            m_sTweetEntityUrls;

        private long
            m_iUserId;

        private DateTime m_tTweetedTimeUTC;

        public RawTweet(long _id, string _screenName, string _nickName) {
            m_tUUID = Guid.NewGuid();
            m_iUserId = _id;
            m_sScreenName = _screenName;
            m_sNickName = _nickName;
        }
        
        public long UserId {
            get { return m_iUserId; }
            set { m_iUserId = value; }
        }
        public string NickName {
            get { return m_sNickName; }
            set { m_sNickName = value; }
        }
        public string ScreenName {
            get { return m_sScreenName; }
            set { m_sScreenName = value; }
        }
        public string TweetContent {
            get { return m_sTweetContent; }
            set { m_sTweetContent = value; }
        }
        public string[] TweetMediaUrls {
            get { return m_sTweetMediaUrls; }
            set { m_sTweetMediaUrls = value; }
        }
        public string[] TweetEntityUrls {
            get { return m_sTweetEntityUrls; }
            set { m_sTweetEntityUrls = value; }
        }
        public string TweetUrl {
            get { return m_sTweetUrl; }
            set { m_sTweetUrl = value; }
        }
        public System.DateTime TweetedTimeUTC {
            get { return m_tTweetedTimeUTC; }
            set { m_tTweetedTimeUTC = value; }
        }
        public System.Guid UUID {
            get { return m_tUUID; }
        }

        public override string ToString() {
            return null;
        }

    }
}
