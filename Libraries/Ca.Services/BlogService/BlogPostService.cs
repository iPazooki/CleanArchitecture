using AutoMapper;
using Ca.Core.Domain.Blog;
using Ca.Services.Caching;
using Ca.Services.DTOs.Blog;
using Ca.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ca.Services.BlogService
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IMapper _mapper;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<BlogPost> _blogRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;

        public BlogPostService(
            IMapper mapper,
            ICacheManager cacheManager,
            IRepository<BlogPost> blogRepository,
            IRepository<BlogComment> blogCommentRepository)
        {
            _mapper = mapper;
            _cacheManager = cacheManager;
            _blogRepository = blogRepository;
            _blogCommentRepository = blogCommentRepository;
        }

        public async Task<IReadOnlyList<BlogPostDto>> GetAll(Expression<Func<BlogPost, bool>> expression = default)
        {
            var items = await _blogRepository.GetAllAsync(expression);

            return _mapper.Map<IReadOnlyList<BlogPostDto>>(items);
        }

        public virtual async Task<BlogPostDto> GetBlogPostById(Guid id)
        {
            var item = await _cacheManager
                .Get(id.ToString(), async _ => await _blogRepository.GetByIdAsync(id));

            return _mapper.Map<BlogPostDto>(item);
        }

        public virtual async ValueTask<BlogPostDto> UpdateBlogPost(BlogPostDto entity)
        {
            _cacheManager.Delete(entity.Id.ToString());

            var item = await _blogRepository.GetByIdAsync(entity.Id);

            _mapper.Map(entity, item);

            item.ModifiedOn = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(item);

            return entity;
        }

        public virtual async ValueTask<BlogPostDto> AddBlogPost(BlogPostDto entity)
        {
            var item = _mapper.Map<BlogPost>(entity);

            await _blogRepository.AddAsync(item);

            return entity;
        }

        public virtual async Task DeleteBlogPost(Guid Id)
        {
            _cacheManager.Delete(Id.ToString());

            var item = await _blogRepository.GetByIdAsync(Id);

            await _blogRepository.DeleteAsync(item);
        }

        public async Task AddComment(BlogCommentDto comment)
        {
            var item = _mapper.Map<BlogComment>(comment);

            await _blogCommentRepository.AddAsync(item);
        }

        public async Task UpdateComment(BlogCommentDto comment)
        {
            var item = await _blogCommentRepository.GetByIdAsync(comment.Id);

            _mapper.Map(comment, item);

            await _blogCommentRepository.UpdateAsync(item);
        }

        public async Task DeleteComment(Guid id)
        {
            var comment = await _blogCommentRepository.GetByIdAsync(id);

            await _blogCommentRepository.DeleteAsync(comment);
        }

        public async Task<BlogPostWithCommentDto> GetBlogPostByIdWithComments(Guid id)
        {
            var item = await _blogRepository.GetByIdAsync(id, x => x.Comments);

            return _mapper.Map<BlogPostWithCommentDto>(item);
        }

        public async Task<IReadOnlyList<BlogCommentDto>> GetAllComments(Guid blogPostId)
        {
            var item = await _blogCommentRepository.GetAllAsync(x => x.BlogPostId == blogPostId);

            return _mapper.Map<IReadOnlyList<BlogCommentDto>>(item);
        }

        public async Task<BlogCommentDto> GetCommentById(Guid id)
        {
            var item = await _blogCommentRepository.GetByIdAsync(id);

            return _mapper.Map<BlogCommentDto>(item);
        }
    }
}