using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers

{

    [Route("api/portfolio")]

    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;

        private readonly IPortfolioRepository _portfolioRepo;

        private readonly IFMPService _fmpService;

        public PortfolioController(IStockRepository stockRepo, UserManager<AppUser> userManager,
        IPortfolioRepository portfolioRepo, IFMPService fmpService)

        {
            _stockRepo = stockRepo;

            _userManager = userManager;

            _portfolioRepo = portfolioRepo;

            _fmpService = fmpService;

        }

        [HttpGet]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]

        public async Task<IActionResult> GetUserPortfolio()
        {

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

            return Ok(userPortfolio);



        }


        [HttpPost]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)

        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)

            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exists");
                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }



            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

            if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))

                return BadRequest("Cannot add same stock to portfolio");

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            await _portfolioRepo.CreateAsync(portfolioModel);

            if (portfolioModel == null)
            {
                return StatusCode(500, "Could not create portfolio");
            }
            else
            {
                return StatusCode(200, "Succesfully created portfolio");
            }
        }


        [HttpDelete]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)

        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower());

            if (filteredStock.Count() == 1)
            {

                await _portfolioRepo.DeletePortfolio(appUser, symbol);


            }
            else
            {
                return BadRequest("stock is not in your portfolio");

            }

            return Ok();
        }


    }
}