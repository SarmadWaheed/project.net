using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;


        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);


            if (commentModel == null)
            {

                return null;
            }

            _context.Comments.Remove(commentModel);

            await _context.SaveChangesAsync();


            return commentModel;

        }



        public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)
        {
            var comment = _context.Comments.Include(a => a.AppUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                comment = comment.Where(s => s.Stock.Symbol == queryObject.Symbol);
            };
            if (queryObject.IsDecsending == true)
            {
                comment = comment.OrderByDescending(c => c.CreatedOn);
            }
            return await comment.ToListAsync();
        }


        public async Task<Comment?> GetByIdAsync(int id)
        {
            {
                return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
            }
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingcommment = await _context.Comments.FindAsync(id);

            if (existingcommment == null)
            {
                return null;
            }

            existingcommment.Title = commentModel.Title;
            existingcommment.Content = commentModel.Content;

            await _context.SaveChangesAsync();

            return existingcommment;
        }
    }
}

