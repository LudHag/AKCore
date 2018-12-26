using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;

namespace AKCore.Models
{
    public class MediasModel
    {
        public IList<IFormFile> UploadFiles { get; set; }
        public string Tag { get; set; }
    }
}