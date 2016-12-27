using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;

namespace AKCore.Models
{
    public class AlbumEditModel
    {
        public IFormFile UploadFile { get; set; }
    }
}