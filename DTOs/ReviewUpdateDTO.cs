using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.DTOs
{
    public class ReviewUpdateDTO
    {
       
            [Required]
            [Range(1, 5)]
            public int Rating { get; set; }

            public string? Comment { get; set; }
        }
}
