using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;

namespace AKCore.Models
{
    public class MediaModel
    {
        public IFormFile UploadFile { get; set; }
        public IList MediaFiles { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public MediaModel()
        {
            MediaFiles = new List<Media>();
        }
    }
}