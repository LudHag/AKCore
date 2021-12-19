using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AKCore.Models
{
    public class MediasModel
    {
        public IList<IFormFile> UploadFiles { get; set; }
        public string Tag { get; set; }
    }
}