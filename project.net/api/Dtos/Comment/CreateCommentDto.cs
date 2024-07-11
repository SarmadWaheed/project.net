using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "title cannot be less than 5 characters")]
        [MaxLength(300, ErrorMessage = "title cannot be more than 280 characters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Content cannot be less than 5 characters")]
        [MaxLength(300, ErrorMessage = "Content cannot be more than 280 characters")]


        public string Content { get; set; } = string.Empty;

    }
}