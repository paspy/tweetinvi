using System;
using Tweetinvi;


namespace IPaspy.TweetinviAPI {
    public class Tweet {

        private Guid m_tUUID;
        private string
            m_sAvatarUrl,
            m_sUserId,
            m_sScreenName,
            m_sTweetContent,
            m_sTweetPictureUrl,
            m_sTweetLinkUrl;
        private DateTime m_tTimeSent;
        public Tweet(string _userId) {
            m_tUUID = new Guid();

        }
        public override string ToString() {
            return null;
        }
        public string AvatarUrl {
            get { return m_sAvatarUrl; }
            set { m_sAvatarUrl = value; }
        }
        public string UserId {
            get { return m_sUserId; }
            set { m_sUserId = value; }
        }
        public string ScreenName {
            get { return m_sScreenName; }
            set { m_sScreenName = value; }
        }
        public string TweetContent {
            get { return m_sTweetContent; }
            set { m_sTweetContent = value; }
        }
        public string TweetPictureUrl {
            get { return m_sTweetPictureUrl; }
            set { m_sTweetPictureUrl = value; }
        }
        public string TweetLinkUrl {
            get { return m_sTweetLinkUrl; }
            set { m_sTweetLinkUrl = value; }
        }
        public System.Guid UUID {
            get { return m_tUUID; }
        }

    }
}
