using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Add this using directive
using api.Data;
using api.Interfaces;
using api.Models;
using api.Dtos.Stock;
using api.Helpers;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);


            if (stockModel == null)
            {

                return null;
            }

            _context.Stocks.Remove(stockModel);

            await _context.SaveChangesAsync();

            return stockModel;

        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();
            //FILTERING
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {

                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }


            if (!string.IsNullOrWhiteSpace(query.Purchase))
            {

                stocks = stocks.Where(s => s.Symbol.Contains(query.Purchase));
            }
            //SORTING
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))

                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }
            //PAGINATION
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        // Find does not work with include so we use firstordefault
        public async Task<Stock?> GetbyIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        // IT WILL CHECK IF STOCK EXISTS OR NOT
        public Task<bool> StockExists(int id)

        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto StockDto)
        {
            var existingstock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingstock == null)
            {

                return null;
            }

            existingstock.Symbol = StockDto.Symbol;
            existingstock.Purchase = StockDto.Purchase;

            await _context.SaveChangesAsync();

            return existingstock;
        }
    }
}
