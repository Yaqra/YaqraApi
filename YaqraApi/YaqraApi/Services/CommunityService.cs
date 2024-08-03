using AutoMapper;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Community;
using YaqraApi.Models;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly Mapper _mapper;
        public CommunityService(ICommunityRepository communityRepository)
        {
            _communityRepository = communityRepository;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<ReviewDto>> AddReviewAsync(ReviewDto review)
        {
            var result = await _communityRepository.AddReviewAsync(_mapper.Map<Review>(review));
            if (result == null)
                return new GenericResultDto<ReviewDto> { Succeeded = false, ErrorMessage = "something went wrong" };

            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = _mapper.Map<ReviewDto>(result) };
        }

        public async Task<GenericResultDto<string>> Delete(int postId)
        {
            var review = await _communityRepository.GetReviewAsync(postId);
            if (review == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "no review with that id" };
            _communityRepository.Delete(_mapper.Map<Review>(review));

            return new GenericResultDto<string> { Succeeded = true, Result = "review deleted successfully" }; 
        }

        public async Task<GenericResultDto<ReviewDto>> GetReviewAsync(int reviewId)
        {
            var review = await _communityRepository.GetReviewAsync(reviewId);
            if (review == null)
                return new GenericResultDto<ReviewDto> { Succeeded = false, ErrorMessage = "no review with that id was found" };
            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = _mapper.Map<ReviewDto>(review) };
        }

        public async Task<GenericResultDto<ReviewDto>> UpdateReviewAsync(ReviewDto editedReview)
        {
            var review = _mapper.Map<Review>(editedReview);
            var result = await _communityRepository.UpdateReviewAsync(review);
            return new GenericResultDto<ReviewDto> { Succeeded = true, Result = editedReview };
        }
    }
}
