using Ca.Core.Domain.Blog;
using Ca.Services.BlogService;
using FluentAssertions;
using GenFu;
using NUnit.Framework;
using System;
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
            var blogPost = await CreateNewBlogPost(title, body, DateTime.UtcNow);

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
        [TestCase("New Title")]
        public async Task Update_BlogPost(string title)
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Title = title;

            await _blogPostService.UpdateBlogPost(blogPost);

            var updatedBlogPost = await _blogPostService.GetBlogPostById(blogPost.Id);

            updatedBlogPost.Should().NotBeNull();

            updatedBlogPost.Title.Should().Be(title);
        }

        [Test]
        public async Task Delete_BlogPost()
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Should().NotBeNull();

            await _blogPostService.DeleteBlogPost(blogPost);

            var bogPost = await _blogPostService.GetBlogPostById(blogPost.Id);

            bogPost.Should().BeNull();
        }

        [Test]
        public async Task Add_Comment()
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Should().NotBeNull();

            var comment = await CreateComment(blogPost);

            comment.Should().NotBeNull();

            comment.CommentText.Should().NotBeEmpty();
        }

        [Test]
        [TestCase("New Comment Text")]
        public async Task Update_Comment(string commentText)
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Should().NotBeNull();

            var comment = await CreateComment(blogPost);

            comment.Should().NotBeNull();

            comment.CommentText.Should().NotBeEmpty();

            comment.CommentText = commentText;

            await _blogPostService.UpdateComment(comment);

            var updatedComment = await _blogPostService.GetCommentById(comment.Id);

            updatedComment.CommentText.Should().Be(commentText);
        }

        [Test]
        public async Task Delete_Comment()
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Should().NotBeNull();

            var comment = await CreateComment(blogPost);

            comment.Should().NotBeNull();

            var addedComment = await _blogPostService.GetCommentById(comment.Id);

            addedComment.Should().NotBeNull();

            await _blogPostService.DeleteComment(addedComment);

            var deletedComment = await _blogPostService.GetCommentById(comment.Id);

            deletedComment.Should().BeNull();
        }

        [Test]
        public async Task Get_Post_Comment()
        {
            var blogPost = await CreateNewBlogPost();

            blogPost.Should().NotBeNull();

            await CreateComment(blogPost);
            await CreateComment(blogPost);

            var fullPost = await _blogPostService.GetBlogPostByIdWithComments(blogPost.Id);

            fullPost.Should().NotBeNull();

            fullPost.Comments.Should().HaveCount(2);
        }

        private async Task<BlogPost> CreateNewBlogPost()
        {
            A.Configure<BlogPost>()
                .Fill(x => x.Body).AsLoremIpsumSentences(5)
                .Fill(x => x.Title).AsArticleTitle()
                .Fill(x => x.CreatedOn).AsPastDate();

            var blogPost = A.New<BlogPost>();

            return await CreateNewBlogPost(blogPost.Title, blogPost.Body, blogPost.CreatedOn);
        }

        private async Task<BlogPost> CreateNewBlogPost(string title, string body, DateTime createdOn)
        {
            var blogPost = new BlogPost
            {
                Title = title,
                Body = body,
                CreatedOn = createdOn
            };

            await _blogPostService.AddBlogPost(blogPost);

            return blogPost;
        }

        private async Task<BlogComment> CreateComment(BlogPost blogPost)
        {
            A.Configure<BlogComment>()
                .Fill(x => x.CommentText).AsLoremIpsumSentences(2)
                .Fill(x => x.CreatedOn).AsFutureDate();

            var comment = A.New<BlogComment>();

            comment.BlogPost = blogPost;
            comment.BlogPostId = blogPost.Id;

            await _blogPostService.AddComment(comment);

            return comment;
        }
    }
}