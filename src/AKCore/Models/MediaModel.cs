using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;

namespace AKCore.Models
{
    public class MediaModel
    {
        public IFormFile UploadFile { get; set; }
        public List<Media> MediaFiles { get; set; }

        public MediaModel()
        {
            MediaFiles = new List<Media>();
        }
    }
}