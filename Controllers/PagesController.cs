using Asp.Versioning;
using blog_api.Context;
using blog_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace blog_api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagesController(ILogger<PagesController> logger, ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize (Roles = "Admin")]
        [HttpPost("new")]
        public async Task<ActionResult<Page>> CreatePage(PageDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var page = new Page
            {
                Id = payload.Id,
                Title = payload.Title,
                Author = payload.Author,
                Body = payload.Body,
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSinglePage), new { Id = payload.Id }, page);
        }

        [HttpGet("id:int")]
        public async Task<ActionResult<PageDto>> GetSinglePage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            var pageDto = new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Author = page.Author,
                Body = page.Body,
            };

            return pageDto;

        }

        [HttpGet]
        public async Task<PagesDto> GetPages()
        {
            var pagesFromDb = await _context.Pages.ToListAsync();

            var pagesDto = new PagesDto();

            foreach (var page in pagesFromDb)
            {
                var pageDto = new PageDto
                {
                    Id = page.Id,
                    Author = page.Author,
                    Body = page.Body,
                    Title = page.Title
                };

                pagesDto.Pages.Add(pageDto);
            }

            return pagesDto;
        }
    }
}
