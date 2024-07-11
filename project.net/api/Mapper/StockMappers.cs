using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Models;

namespace api.Mapper
{
    public static class StockMappers
    {
        public static StockDto ToStockDTO(this Stock stockModel)
        {

            return new StockDto
            {

                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                Purchase = stockModel.Purchase,
                Comments = stockModel.Comments.Select(c => c.ToCommentDTO()).ToList()



            };
        }

        public static Stock ToStockFromCreateDto(this CreateStockRequestDto stockDto)
        {

            return new Stock
            {

                Symbol = stockDto.Symbol,
                Purchase = stockDto.Purchase
            };
        }

        public static Stock ToStockFromFMP(this FMPStock fmpStock)
        {
            return new Stock
            {
                Symbol = fmpStock.symbol,
                Purchase = (decimal)fmpStock.price,

            };
        }
    }
}