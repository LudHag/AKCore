using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AKCore.Models
{
    public class AlbumEditModel
    {
        public IFormFile UploadFile { get; set; }
        public IList<Album> Albums { get; set; }

        public string AlbumJson => JsonConvert.SerializeObject(Albums);
    }
}