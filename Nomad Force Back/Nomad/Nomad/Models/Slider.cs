using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomad.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
