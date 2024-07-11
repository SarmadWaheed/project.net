using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mapper
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDTO(this Comment commentModel)
        {

            return new CommentDto
            {

                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                StockId = commentModel.StockId,
                Createdby = commentModel.AppUser.UserName,


            };
        }

        public static Comment ToCreateCommentDTO(this CreateCommentDto commentDto, int stockId)
        {

            return new Comment
            {


                Title = commentDto.Title,
                Content = commentDto.Content,
                StockId = stockId,



            };
        }

        public static Comment ToUpdateCommentDTO(this UpdateCommentDto commentDto)
        {

            return new Comment
            {


                Title = commentDto.Title,
                Content = commentDto.Content,



            };
        }



    }
}