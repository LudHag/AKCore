using AKCore.DataModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AKCore.Models
{
    public class AlbumEditModel
    {
        public IList<IFormFile> TrackFiles { get; set; }
        public int AlbumId { get; set; }
        public IList<Album> Albums { get; set; }

        public AlbumEditModel()
        {
            Albums = new List<Album>();
        }
    }
}