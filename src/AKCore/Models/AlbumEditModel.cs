using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AKCore.Models
{
    public class AlbumEditModel
    {
        public IList<IFormFile> TrackFiles { get; set; }
        public int AlbumId { get; set; }
        public IList<Album> Albums { get; set; }
    }
}