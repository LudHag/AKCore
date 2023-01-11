using AKCore.Models;
using System.Collections.Generic;

namespace AKCore.DataModel
{
    public class Video
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public int? Index{ get; set; }
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
