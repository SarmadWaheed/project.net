using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class CreateStockRequestDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Symbol cannot be less than 5 characters")]
        [MaxLength(300, ErrorMessage = "Symbol cannot be more than 280 characters")]


        public string Symbol { get; set; } = string.Empty;

        [Required]
        [Range(0.001, 1000)]



        public decimal Purchase { get; set; }

    }
}