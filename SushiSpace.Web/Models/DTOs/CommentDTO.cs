using SushiSpace.Web.Models.Entites;
using System.ComponentModel.DataAnnotations;

namespace SushiSpace.Web.Models.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public int ProductId { get; set; }
        public string UserName { get; set; }

        
    }
}
