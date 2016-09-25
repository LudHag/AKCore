using System.Collections;
using System.Collections.Generic;

namespace AKCore.DataModel
{
    public class AkWidgets
    {
        public const string TextImage = "TextImage";
        public const string Text = "Text";
        public const string Image = "Image";

        public static readonly IList Widgets = new List<string>()
        {
            TextImage,
            Text,
            Image
        };
    }

    public class Widget
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }

        public string GetHeader()
        {
            if (Type == "Text")
            {
                return "Text-widget";
            }
            else if (Type == "Image")
            {
                return "Bild-widget";
            }
            return "Text-bild-widget";
        }
    }
}
