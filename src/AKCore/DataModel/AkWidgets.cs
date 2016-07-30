﻿using System.Collections.Generic;

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

    public class Widget
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }

        public string GetHeader()
        {
            if (Type == "text")
            {
                return "Text-widget";
            }
            else if (Type == "image")
            {
                return "Bild-widget";
            }
            return "Text-bild-widget";
        }
    }
}