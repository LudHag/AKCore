using System.Collections.Generic;

namespace AKCore.DataModel
{
    public class AkWidgets
    {
        public const string TextImage = "TextImage";
        public const string Text = "Text";
        public const string Image = "Image";
        public static readonly List<string> Widgets = new List<string>()
        {
            TextImage,
            Text,
            Image
        };
    }
}

