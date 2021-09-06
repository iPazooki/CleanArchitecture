using Ca.Core.Domain.Blog;
using Ca.Services.BlogService;
using Ca.Services.DTOs.Blog;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ca.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet]
        [Route("get-post/{id}")]
        public async Task<ActionResult<BlogPostDto>> GetPost(Guid id)
        {
            return Ok(await _blogPostService.GetBlogPostById(id));
        }

        [HttpGet]
        [Route("get-all-posts")]
        public async Task<ActionResult<IReadOnlyList<BlogPostDto>>> GetBlogPost()
        {
            return Ok(await _blogPostService.GetAll());
        }

        [HttpGet]
        [Route("get-all-comments/{blogPostId}")]
        public async Task<ActionResult<IReadOnlyList<BlogCommentDto>>> GetBlogPostComments(Guid blogPostId)
        {
            return Ok(await _blogPostService.GetAllComments(blogPostId));
        }

        [HttpGet]
        [Route("get-comment/{id}")]
        public async Task<ActionResult<BlogCommentDto>> GetComment(Guid id)
        {
            return Ok(await _blogPostService.GetCommentById(id));
        }

        [HttpPost]
        [Route("add-post")]
        public async Task<ActionResult> AddBlogPost(BlogPostDto blogPost)
        {
            await _blogPostService.AddBlogPost(blogPost);

            return Ok();
        }

        [HttpPost]
        [Route("add-comment")]
        public async Task<ActionResult> AddBlogPostComment(BlogCommentDto comment)
        {
            await _blogPostService.AddComment(comment);

            return Ok();
        }

        [HttpPut]
        [Route("update-post")]
        public async Task<ActionResult> UpdateBlogPost(BlogPostDto blogPost)
        {
            await _blogPostService.UpdateBlogPost(blogPost);

            return Ok();
        }

        [HttpPut]
        [Route("update-comment")]
        public async Task<ActionResult> UpdateComment(BlogCommentDto comment)
        {
            await _blogPostService.UpdateComment(comment);

            return Ok();
        }

        [HttpDelete]
        [Route("delete-post")]
        public async Task<ActionResult> DeleteBlogPost(Guid id)
        {
            await _blogPostService.DeleteBlogPost(id);

            return Ok();
        }

        [HttpDelete]
        [Route("delete-comment")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            await _blogPostService.DeleteComment(id);

            return Ok();
        }
    }
}