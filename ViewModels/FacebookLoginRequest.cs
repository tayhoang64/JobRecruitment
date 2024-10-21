namespace CVRecruitment.ViewModels
{
    public class FacebookLoginRequest
    {
        public string Token { get; set; }
    }

    public class FacebookUser
    {
        public string Email { get; set; }
        public FacebookPicture Picture { get; set; }
    }

    public class FacebookPicture
    {
        public FacebookPictureData Data { get; set; }
    }

    public class FacebookPictureData
    {
        public string Url { get; set; }
    }
}
