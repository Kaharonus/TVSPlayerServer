using System;
using System.Collections.Generic;
using System.Text;

namespace TVSPlayerServer
{
    class FileInformation{
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public int EpisodeId { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public string OriginalName { get; set; }
        public string GeneratedName { get; set; }
        public string NewName { get; set; }
        public string Quality { get; set; }
    }
}
