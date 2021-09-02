using Ca.Core.Domain.Blog;
using Ca.Services.BlogService;
using FluentAssertions;
using GenFu;
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
        [TestCase("New blog post", "Blog post content")]
        public async Task Creat_BlogPost(string title, string body)
        {
            var blogPost = new BlogPost
            {
                Title = title,
                Body = body,
                CreatedOn = DateTime.UtcNow
            };

            await _blogPostService.AddBlogPost(blogPost);

            var result = await _blogPostService.GetBlogPostById(blogPost.Id);

            result.Should().NotBeNull();

            result.Title.Should().Be(title);
            result.Body.Should().Be(body);
        }

        [Test]
        public async Task List_BlogPosts()
        {
            A.Configure<BlogPost>()
                .Fill(x => x.Body).AsLoremIpsumSentences(5)
                .Fill(x => x.Title).AsArticleTitle();

            foreach (var blogPost in A.ListOf<BlogPost>(10))
                await _blogPostService.AddBlogPost(blogPost);

            var blogPosts = await _blogPostService.GetAll();

            blogPosts.Should().NotBeNull();
            blogPosts.Count.Should().BeGreaterThan(5);
        }

        [Test]
        [TestCase("New title")]
        public async Task Update_BlogPost(string title)
        {
            await Creat_BlogPost(title, string.Empty);

            var firstBlogPost = (await _blogPostService.GetAll())[0];

            firstBlogPost.Title = title;

            await _blogPostService.UpdateBlogPost(firstBlogPost);

            var updatedBlogPost = await _blogPostService.GetBlogPostById(firstBlogPost.Id);

            updatedBlogPost.Should().NotBeNull();

            updatedBlogPost.Title.Should().Be(title);
        }

        [Test]
        public async Task Delete_BlogPost()
        {
            await Creat_BlogPost("Sample Title", "Sample Content");

            var firstBlogPost = (await _blogPostService.GetAll())[0];

            firstBlogPost.Should().NotBeNull();

            await _blogPostService.DeleteBlogPost(firstBlogPost);

            var bogPost = await _blogPostService.GetBlogPostById(firstBlogPost.Id);

            bogPost.Should().BeNull();
        }
    }
}