using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Comment;
using backend.Interface;
using backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(s => s.ToCommentDto());
            return Ok(comments);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment.ToCommentDto());
        }
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentRequestDto commentDto)
        {
            if (!await _stockRepo.StockExists(stockId)) return BadRequest("Stock does not exist");
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var commentModel = commentDto.ToCommentFromCreateDTO(stockId);
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById),
                new { id = commentModel.Id }, commentModel.ToCommentDto());
        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if (comment == null) return NotFound();
            return Ok(comment.ToCommentDto());
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null) return NotFound("Comment does not exist");
            return Ok(commentModel);
        }
    }
}