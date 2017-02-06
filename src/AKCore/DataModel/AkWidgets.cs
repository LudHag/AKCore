using System.Collections;
using System.Collections.Generic;
using AKCore.Models;

namespace AKCore.DataModel
{
    public class AkWidgets
    {
        public const string TextImage = "TextImage";
        public const string Text = "Text";
        public const string Image = "Image";
        public const string Video = "Video";
        public const string Music = "Music";
        public const string Join = "Join";
        public const string MemberList = "MemberList";
        public const string PostList = "PostList";

        public static readonly IList Widgets = new List<string>()
        {
            TextImage,
            Text,
            Image,
            Video,
            Music,
            Join,
            MemberList,
            PostList
        };
    }

    public class Video
    {
        public string Link { get; set; }
        public string Title { get; set; }
    }
    public class Widget
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public List<Video> Videos { get; set; }
        public List<int> Albums { get; set; }

        public JoinUsModel JoinUsModel { get; set; }
        public string GetHeader()
        {
            switch (Type)
            {
                case "Text":
                    return "Text-widget";
                case "Image":
                    return "Bild-widget";
                case "Video":
                    return "Video-widget";
                case "Music":
                    return "Musik-widget";
                case "Join":
                    return "Gå med-widget";
                case "MemberList":
                    return "Adressregister-widget";
                case "PostList":
                    return "Kamererspostlista-widget";
            }
            return "Text-bild-widget";
        }
    }
}
