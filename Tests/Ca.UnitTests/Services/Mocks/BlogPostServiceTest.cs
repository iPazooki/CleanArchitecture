using Ca.Services.BlogService;
using Ca.Services.DTOs.Blog;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ca.UnitTests.Services.Mocks
{
    [TestFixture]
    public class BlogPostServiceTest
    {
        Mock<IBlogPostService> blogPostService;

        [SetUp]
        public void Setup()
        {
            blogPostService = new Mock<IBlogPostService>(MockBehavior.Strict);
        }

        [Test]
        [TestCase("FE44A4BA-6B3F-43EF-8D99-1AB08D8E88EB", "Blog title")]
        public async Task Add_BlogPost(Guid id, string title)
        {
            blogPostService.Setup(x => x.GetBlogPostById(id))
                .ReturnsAsync(new BlogPostDto
                {
                    Id = id,
                    Title = title
                });

            var blogPost = await blogPostService.Object.GetBlogPostById(id);

            blogPost.Should().NotBeNull();

            blogPost.Id.Should().Be(id);

            blogPost.Title.Should().Be(title);
        }
    }
}