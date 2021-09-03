using Ca.Core.Domain.Blog;
using Ca.Services.BlogService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
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
        public async Task<ActionResult<BlogPost>> GetPost(Guid id)
        {
            return Ok(await _blogPostService.GetBlogPostById(id));
        }

        [HttpGet]
        [Route("get-all-posts")]
        public async Task<ActionResult<IReadOnlyList<BlogPost>>> GetBlogPost()
        {
            return Ok(await _blogPostService.GetAll());
        }

        [HttpGet]
        [Route("get-all-comments/{blogPostId}")]
        public async Task<ActionResult<IReadOnlyList<BlogPost>>> GetBlogPostComments(Guid blogPostId)
        {
            return Ok(await _blogPostService.GetAllComments(blogPostId));
        }

        [HttpGet]
        [Route("get-comment/{id}")]
        public async Task<ActionResult<BlogComment>> GetComment(Guid id)
        {
            return Ok(await _blogPostService.GetCommentById(id));
        }

        [HttpPost]
        [Route("add-post")]
        public async Task<ActionResult<BlogPost>> AddBlogPost(BlogPost blogPost)
        {
            return Ok(await _blogPostService.AddBlogPost(blogPost));
        }

        [HttpPost]
        [Route("add-comment")]
        public async Task<ActionResult<BlogPost>> AddBlogPostComment(BlogComment comment)
        {
            await _blogPostService.AddComment(comment);
            return Ok();
        }

        [HttpPut]
        [Route("update-post")]
        public async Task<ActionResult<BlogPost>> UpdateBlogPost(BlogPost blogPost)
        {
            return Ok(await _blogPostService.UpdateBlogPost(blogPost));
        }

        [HttpPut]
        [Route("update-comment")]
        public async Task<ActionResult> UpdateComment(BlogComment comment)
        {
            await _blogPostService.UpdateComment(comment);
            return Ok();
        }

        [HttpDelete]
        [Route("delete-post")]
        public async Task<ActionResult> DeleteBlogPost(BlogPost blogPost)
        {
            await _blogPostService.DeleteBlogPost(blogPost);

            return Ok();
        }

        [HttpDelete]
        [Route("delete-comment")]
        public async Task<ActionResult> DeleteComment(BlogComment comment)
        {
            await _blogPostService.DeleteComment(comment);

            return Ok();
        }
    }
}