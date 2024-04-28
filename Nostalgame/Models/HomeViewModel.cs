using System.Collections.Generic;

namespace Nostalgame.Models
{
    public class HomeViewModel
    {
        public List<Videogioco> TuttiVideogiochi { get; set; }
        public List<Videogioco> GiochiAppenaAggiunti { get; set; }
        public List<Videogioco> GiochiPerGenere { get; set; }
        public List<string> Piattaforme { get; set; }

        public bool HasAvatar { get; set; }

    }
}
