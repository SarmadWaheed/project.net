using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces;
using api.Mapper;
using api.Data;
using api.Dtos.Comment;
using Microsoft.AspNetCore.Identity;
using api.Models;
using api.Extensions;
using api.Helpers;
using Microsoft.AspNetCore.Authorization;
using api.Service;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        private readonly IFMPService _fmpService;

        private readonly UserManager<AppUser> _userManager;

        // Ensure there is only one constructor
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager,
        IFMPService fmpService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        [HttpGet]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject queryObject)

        {
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllAsync(queryObject);
            var commentDtos = comments.Select(s => s.ToCommentDTO());

            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetById([FromRoute] int id)

        {
            // if (!ModelState.IsValid)

            //     return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDTO());

        }

        [HttpPost]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

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

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCreateCommentDTO(stock.Id);
            commentModel.AppUserId = appUser.Id;
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDTO());
        }
        [HttpPut]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:int}")]

        public async Task<IActionResult> Update([FromRoute] int id, UpdateCommentDto updateDto)

        {

            // if (!ModelState.IsValid)

            //     return BadRequest(ModelState);
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToUpdateCommentDTO());

            if (comment == null)
            {
                return NotFound("comment not found");

            }

            return Ok(comment.ToCommentDTO());


        }

        [HttpDelete]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:int}")]

        public async Task<IActionResult> Delete([FromRoute] int id)


        {

            // if (!ModelState.IsValid)

            //     return BadRequest(ModelState);
            var commentModel = await _commentRepo.DeleteAsync(id);

            if (commentModel == null)
            {

                return NotFound("Comment not exist");

            }

            return Ok(commentModel);



        }
    }
}
