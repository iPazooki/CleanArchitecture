using Ca.Core.Domain.Blog;
using Ca.Services.DTOs.Blog;
using Ca.WebApi;
using FluentAssertions;
using GenFu;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ca.IntegrationTests
{
    public class BlogPostTest : IClassFixture<CustomWebApplicationFactory<Ca.WebApi.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public BlogPostTest(CustomWebApplicationFactory<Ca.WebApi.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Post_CreateBlogPost()
        {
            var blogPost = CreateNewBlogPost();

            var blogPostJson = JsonConvert.SerializeObject(blogPost);

            HttpContent content = new StringContent(blogPostJson, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:5000/api/BlogPost/add-post"),
                Content = content
            };

            var result = await _client.SendAsync(request);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        private BlogPostDto CreateNewBlogPost()
        {
            A.Configure<BlogPostDto>()
                .Fill(x => x.Body).AsLoremIpsumSentences(5)
                .Fill(x => x.Title).AsArticleTitle()
                .Fill(x => x.CreatedOn).AsPastDate();

            var blogPost = A.New<BlogPostDto>();

            return blogPost;
        }
    }
}