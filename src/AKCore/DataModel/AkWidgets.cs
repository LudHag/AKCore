using AKCore.Models;
using System.Collections;
using System.Collections.Generic;

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
        public const string Hire = "Hire";
        public const string MemberList = "MemberList";
        public const string PostList = "PostList";
        public const string HeaderText = "HeaderText";
        public const string ThreePuffs = "ThreePuffs";

        public static readonly IList Widgets = new List<string>()
        {
            TextImage,
            Text,
            Image,
            Video,
            Music,
            Join,
            MemberList,
            PostList,
            Hire,
            HeaderText,
            ThreePuffs
        };
    }

    public class Video
    {
        public string Link { get; set; }
        public string Title { get; set; }
    }
    public class Widget
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public List<Video> Videos { get; set; }
        public List<int> Albums { get; set; }
        public JoinUsModel JoinUsModel { get; set; }
        public HireModel HireModel { get; set; }
    }
}
