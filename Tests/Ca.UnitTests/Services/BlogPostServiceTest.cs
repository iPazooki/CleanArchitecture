using Ca.Core.Domain.Blog;
using Ca.Services.BlogService;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ca.UnitTests.Services
{
    [TestFixture]
    public class BlogPostServiceTest : BaseTest
    {
        private IBlogPostService _blogPostService;

        [SetUp]
        public void Setup()
        {
            _blogPostService = GetService<IBlogPostService>();
        }

        [Test]
        [TestCase("Title")]
        public async Task Creat_BlogPost(string title)
        {
            var blogPost = new BlogPost
            {
                Title = title,
                Body = "Some body",
                CreatedOn = DateTime.UtcNow
            };

            await _blogPostService.InsertBlogPost(blogPost);

            var result = await _blogPostService.GetBlogPostById(blogPost.Id);

            result.Should().NotBeNull();

            result.Title.Should().Be(title);
        }
    }
}