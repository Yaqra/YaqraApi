using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using YaqraApi.AutoMapperConfigurations;
using YaqraApi.DTOs;
using YaqraApi.DTOs.Author;
using YaqraApi.DTOs.Book;
using YaqraApi.DTOs.Genre;
using YaqraApi.DTOs.ReadingGoal;
using YaqraApi.DTOs.User;
using YaqraApi.DTOs.UserBookWithStatus;
using YaqraApi.Helpers;
using YaqraApi.Models;
using YaqraApi.Models.Enums;
using YaqraApi.Repositories.Context;
using YaqraApi.Repositories.IRepositories;
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly IGenreRepository _genreRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IRecommendationService _recommendationService;
        private readonly IWebHostEnvironment _environment;
        private readonly Mapper _mapper;
        public UserService(
            UserManager<ApplicationUser> userManager, 
            IGenreService genreService,
            IAuthorService authorService,
            IBookService bookService,
            IGenreRepository genreRepository,
            IAuthorRepository authorRepository,
            IRecommendationService recommendationService,
            IWebHostEnvironment environment
            )
        {
            _userManager = userManager;
            _genreService = genreService;
            _authorService = authorService;
            _bookService = bookService;
            _genreRepository = genreRepository;
            _authorRepository = authorRepository;
            _recommendationService = recommendationService;
            _environment = environment;
            _mapper = AutoMapperConfig.InitializeAutoMapper();
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdatePasswordAsync(PasswordUpdateDto dto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };
            
            var identityResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser> 
                { 
                    Succeeded = false, 
                    ErrorMessage = UserHelpers.GetErrors(identityResult) 
                };
            
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateProfilePictureAsync(IFormFile pic, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };

            //var oldPicPath = user.ProfilePicture;

            //var picName = Path.GetFileName(pic.FileName);
            //var picExtension = Path.GetExtension(picName);
            //var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";
            //var dir = Path.Combine(_environment.WebRootPath, "ProfilePictures");
            //if (Directory.Exists(dir) == false)
            //    Directory.CreateDirectory(dir);
            //var picPath = Path.Combine(dir, picWithGuid);

            //var createPic = Task.Run(async () =>
            //{
            //    using(var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
            //    {
            //        await pic.CopyToAsync(stream);
            //        user.ProfilePicture = $"/ProfilePictures/{picWithGuid}";
            //    }
            //});
            //var deleteOldPic = Task.Run(() =>
            //{
            //    if(string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
            //        File.Delete(oldPicPath);
            //});
            //Task.WaitAll(createPic, deleteOldPic);

            user.ProfilePicture = ImageHelpers.UploadImage(ImageHelpers.ProfilePicturesDir, user.ProfilePicture, pic, _environment);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<ApplicationUser>> UpdateProfileCoverAsync(IFormFile pic, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };

            user.ProfileCover = ImageHelpers.UploadImage(ImageHelpers.ProfileCoversDir, user.ProfileCover, pic, _environment);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<FollowDto>> FollowUserAsync(UserIdDto dto, string userId)
        {
            var user = await _userManager.Users.Include(u=>u.Followings).SingleOrDefaultAsync(u=>u.Id == userId);
            if (user == null)
                return new GenericResultDto<FollowDto> { Succeeded = false, ErrorMessage = "user not found" };

            var result = new FollowDto();

            var followedUser = user.Followings.FirstOrDefault(f => f.Id == dto.UserId);
            if (followedUser != null)//unfollow
            {
                user.Followings.Remove(followedUser);
                result.IsFollowed = false;
            }
            else//follow
            {
                followedUser = await _userManager.FindByIdAsync(dto.UserId);
                if (followedUser == null)
                    return new GenericResultDto<FollowDto> { Succeeded = false, ErrorMessage = "the user you want to follow not found" };

                user.Followings.Add(followedUser);
                result.IsFollowed = true;
            }

            var identityResult = await _userManager.UpdateAsync(user);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<FollowDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            result.FollowersCount = GetUserFollowersNamesCount(dto.UserId);

            return new GenericResultDto<FollowDto> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<UserDto>> GetUserAsync(string userId, string followerId)
        {
            var dto = await _userManager.Users
                .Include(u=>u.Followers)
                .Select(u => new UserDto
            {
                UserId = u.Id,
                Username = u.UserName,
                Bio = u.Bio,
                ProfilePicture = u.ProfilePicture,
                ProfileCover = u.ProfileCover,
                FollowersCount = u.Followers.Count(),
                FollowingsCount = u.Followings.Count(),
                IsFollowed = IsUserFollowed(u.Followers, followerId)
                
            }).SingleOrDefaultAsync(x => x.UserId == userId);

            if (dto == null)
                return new GenericResultDto<UserDto> { Succeeded = false, ErrorMessage = "user not found" };

            return new GenericResultDto<UserDto>
            {
                Succeeded = true,
                Result = dto
            };
            
        }
        public int GetUserFollowersNamesCount(string userId)
        {
            return (from user in _userManager.Users
                         where user.Id == userId
                         select user.Followers.Count())
                         .ToList()
                         .FirstOrDefault();
        }
        public GenericResultDto<PagedResult<UserNameAndId>> GetUserFollowersNames(string userId, int page )
        {
            page = page==0? 1 : page;
            var followersList = (from user in _userManager.Users
                                where user.Id == userId
                                from follower in user.Followers
                                select new { follower.Id, follower.UserName })
                                .Skip((page-1)*Pagination.UserFollowersNames)
                                .Take(Pagination.UserFollowersNames); 

            var followersDto = new List<UserNameAndId>();
            foreach (var follower in followersList)
                followersDto.Add(new UserNameAndId { UserId = follower.Id, Username = follower.UserName });

            var result = new PagedResult<UserNameAndId> { 
                Data = followersDto, 
                PageNumber = page,
                PageSize = Pagination.UserFollowersNames,
                TotalPages = Pagination.CalculatePagesCount(GetUserFollowersNamesCount(userId), Pagination.UserFollowersNames)
            };
            return new GenericResultDto<PagedResult<UserNameAndId>> { Succeeded= true, Result = result };
        
        }
        public int GetUserFollowingsNamesCount(string userId)
        {
            return (from user in _userManager.Users
                    where user.Id == userId
                    select user.Followings.Count())
                         .ToList()
                         .FirstOrDefault();
        }
        public GenericResultDto<PagedResult<UserNameAndId>> GetUserFollowingsNames(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var followingsList = (from user in _userManager.Users
                                where user.Id == userId
                                from following in user.Followings
                                select new { following.Id, following.UserName })
                                .Skip((page - 1) * Pagination.UserFollowingsNames)
                                .Take(Pagination.UserFollowingsNames);

            var followingDto = new List<UserNameAndId>();
            foreach (var following in followingsList)
                followingDto.Add(new UserNameAndId { UserId = following.Id, Username = following.UserName });

            var result = new PagedResult<UserNameAndId>
            {
                Data = followingDto,
                PageNumber = page,
                PageSize = Pagination.UserFollowingsNames,
                TotalPages = Pagination.CalculatePagesCount(GetUserFollowingsNamesCount(userId), Pagination.UserFollowingsNames)
            };
            return new GenericResultDto<PagedResult<UserNameAndId>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<List<GenreDto>>> GetFavouriteGenresAsync(string userId)
        {
            var user = await _userManager.Users.Include(x=>x.FavouriteGenres).SingleOrDefaultAsync(u=>u.Id == userId);
            if (user == null)
                return new GenericResultDto<List<GenreDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var result = new List<GenreDto>();
            foreach (var item in user.FavouriteGenres)
                result.Add(new GenreDto { GenreId = item.Id, GenreName = item.Name });
            return new GenericResultDto<List<GenreDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<PagedResult<GenreDto>>> GetAllGenresExceptUserGenresAsync(string userId, int page)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteGenres).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<PagedResult<GenreDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserGenreIds = new HashSet<int>();

            foreach (var item in user.FavouriteGenres)
                UserGenreIds.Add(item.Id);

            var genresExceptUser = new List<GenreDto>();
            var genres = (await _genreService.GetAllAsync(page)).Result;

            foreach (var item in genres.Data)
            {
                if (UserGenreIds.Contains(item.GenreId) == false)
                    genresExceptUser.Add(item);
            }

            var result = new PagedResult<GenreDto>
            {
                PageSize = genresExceptUser.Count,
                Data = genresExceptUser,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_genreRepository.GetCount() - genresExceptUser.Count, genresExceptUser.Count),
            };

            return new GenericResultDto<PagedResult<GenreDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<List<GenreDto>>> AddFavouriteGenresAsync(List<GenreIdDto> genres, string userId)
        {
            var user = await _userManager.Users
                .Include(x => x.FavouriteGenres)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<GenreDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserGenreIds = new HashSet<int>();

            foreach (var item in user.FavouriteGenres)
                UserGenreIds.Add(item.Id);

            foreach (var genre in genres)
            {
                if(UserGenreIds.Contains(genre.GenreId) == false)
                {
                    var dto = (await _genreService.GetByIdAsync(genre.GenreId)).Result;
                    if (dto == null)
                        continue;
                    user.FavouriteGenres.Add(new Genre {Id= dto.GenreId, Name = dto.GenreName});
                    await _recommendationService.IncrementPoints(user.Id, dto.GenreId);
                }
            }

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<List<GenreDto>>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var result = new List<GenreDto>();
            foreach (var original in user.FavouriteGenres)
            {
                result.Add(new GenreDto { GenreId=original.Id, GenreName=original.Name });
            }

            return new GenericResultDto<List<GenreDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<string>> DeleteFavouriteGenreAsync(GenreIdDto genre, string userId)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteGenres).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };

            var genreToDelete = user.FavouriteGenres.SingleOrDefault(x => x.Id == genre.GenreId);
            if (genreToDelete == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "genre not found" };

            await _recommendationService.DecrementPoints(user.Id, genreToDelete.Id);
            user.FavouriteGenres.Remove(genreToDelete);
            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<string>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            return new GenericResultDto<string> { Succeeded = true, Result = "genre deleted successfully from ur favourite genres" };
        }
        public async Task<GenericResultDto<UserDto>> UpdateAllAsync(IFormFile? pic, IFormFile? cover, UserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return new GenericResultDto<UserDto> { Succeeded = false, ErrorMessage = "user not found" };

            if (pic != null)
                await UpdateProfilePictureAsync(pic, user.Id);
            if(cover != null)
                await UpdateProfileCoverAsync(cover,user.Id);
            
            if(dto.Username != null)
                user.UserName = dto.Username;
            if(dto.Bio != null)
                user.Bio = dto.Bio;

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<UserDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var userDto = new UserDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                ProfileCover = user.ProfileCover,
                FollowersCount = GetUserFollowersNamesCount(user.Id),
                FollowingsCount = GetUserFollowingsNamesCount(user.Id),
            };
            return new GenericResultDto<UserDto> { Succeeded = true, Result = userDto };
        }
        public async Task<GenericResultDto<List<AuthorDto>>> AddFavouriteAuthorsAsync(List<AuthorIdDto> authors, string userId, int page)
        {
            page = page==0? 1 : page;
            var user = await _userManager.Users
                .Include(x => x.FavouriteAuthors)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserAuthorIds = new HashSet<int>();

            foreach (var item in user.FavouriteAuthors)
                UserAuthorIds.Add(item.Id);

            foreach (var author in authors)
            {
                if (UserAuthorIds.Contains(author.AuthorId) == false)
                {
                    var dto = (await _authorService.GetByIdAsync(author.AuthorId)).Result;
                    if (dto == null)
                        continue;
                    user.FavouriteAuthors.Add(new Author { Id = dto.Id, Name = dto.Name, Bio = dto.Bio, Picture = dto.Picture });
                }
            }

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<List<AuthorDto>>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var result = new List<AuthorDto>();
            foreach (var original in user.FavouriteAuthors.Skip((page-1)*Pagination.Authors).Take(Pagination.Authors))
                result.Add(new AuthorDto { Id = original.Id, Name = original.Name, Bio = original.Bio, Picture = original.Picture });

            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<PagedResult<AuthorDto>>> GetFavouriteAuthorsAsync(string userId, int page)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return new GenericResultDto<PagedResult<AuthorDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var authorsDto = new List<AuthorDto>();
            foreach (var item in user.FavouriteAuthors.Skip((page - 1) * Pagination.Authors).Take(Pagination.Authors))
                authorsDto.Add(new AuthorDto { Id = item.Id, Name = item.Name, Bio = item.Bio, Picture = item.Picture });

            var result = new PagedResult<AuthorDto>
            {
                PageSize = Pagination.Authors,
                Data = authorsDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(user.FavouriteAuthors.Count(), Pagination.Authors)
            };

            return new GenericResultDto<PagedResult<AuthorDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<PagedResult<AuthorDto>>> GetAllAuthorsExceptUserAuthorsAsync(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<PagedResult<AuthorDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserAuthorsIds = new HashSet<int>();

            foreach (var item in user.FavouriteAuthors)
                UserAuthorsIds.Add(item.Id);

            var authorsExceptUser = new List<AuthorDto>();
            var authors = (await _authorService.GetAll(page)).Result;

            foreach (var item in authors.Data)
            {
                if (UserAuthorsIds.Contains(item.Id) == false)
                    authorsExceptUser.Add(item);
            }

            var result = new PagedResult<AuthorDto>
            {
                PageSize = authorsExceptUser.Count,
                Data = authorsExceptUser,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(_authorService.GetCount()-authorsExceptUser.Count, authorsExceptUser.Count),
            };

            return new GenericResultDto<PagedResult<AuthorDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<string>> DeleteFavouriteAuthorAsync(AuthorIdDto author, string userId)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };

            var authorToDelete = user.FavouriteAuthors.SingleOrDefault(x => x.Id == author.AuthorId);
            if (authorToDelete == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "author not found" };

            user.FavouriteAuthors.Remove(authorToDelete);
            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<string>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            return new GenericResultDto<string> { Succeeded = true, Result = "author deleted successfully from ur favourite authors" };
        }
        public async Task<GenericResultDto<ReadingGoalDto>> AddReadingGoalAsync(ReadingGoalDto dto, string userId)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<ReadingGoalDto> { Succeeded = false, ErrorMessage = "user not found" };

            user.ReadingGoals = new List<ReadingGoal> { _mapper.Map<ReadingGoal>(dto) };
            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ReadingGoalDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var result = user.ReadingGoals.FirstOrDefault();
            return new GenericResultDto<ReadingGoalDto> { Succeeded = true, Result = _mapper.Map<ReadingGoalDto>(result) };
        }
        public int GetAllReadingGoalsCountAsync(string userId)
        {
            return (from user in _userManager.Users
                        where user.Id == userId
                        select user.ReadingGoals.Count)
                        .ToList().FirstOrDefault();
        }
        public async Task<GenericResultDto<PagedResult<ReadingGoalDto>>> GetAllReadingGoalsAsync(string userId, int page)
        {
            page = page==0?1: page;
            var user = await _userManager.Users
                .Include(x => x.ReadingGoals.Skip((page-1)*Pagination.ReadingGoals).Take(Pagination.ReadingGoals))
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<PagedResult<ReadingGoalDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var readingGoalsDto = new List<ReadingGoalDto>();
            foreach (var item in user.ReadingGoals)
                readingGoalsDto.Add(_mapper.Map<ReadingGoalDto>(item));

            var result = new PagedResult<ReadingGoalDto>
            {
                PageSize = Pagination.ReadingGoals,
                Data = readingGoalsDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(GetAllReadingGoalsCountAsync(userId), Pagination.ReadingGoals)
            };

            return new GenericResultDto<PagedResult<ReadingGoalDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<string>> DeleteReadingGoalAsync(int goalId, string userId)
        {
            var user = await _userManager.Users
                .Include(x => x.ReadingGoals)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };

            var goalToDelete = user.ReadingGoals.SingleOrDefault(u => u.Id == goalId);
            if(goalToDelete == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "reading goal not found" };
            
            user.ReadingGoals.Remove(goalToDelete);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<string>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            return new GenericResultDto<string> { Succeeded = true, Result = "reading goal deleted successfully" };
        }
        public async Task<GenericResultDto<ReadingGoalDto>> UpdateReadingGoalAsync(UpdateReadingGoalDto dto, string userId)
        {
            var user = await _userManager.Users
                .Include(x => x.ReadingGoals.Where(u => u.Id == dto.Id))
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<ReadingGoalDto> { Succeeded = false, ErrorMessage = "user not found" };

            var goalToEdit = user.ReadingGoals.FirstOrDefault();
            if (goalToEdit == null)
                return new GenericResultDto<ReadingGoalDto> { Succeeded = false, ErrorMessage = "reading goal not found" };

                goalToEdit.StartDate = dto.StartDate;
                goalToEdit.DurationInDays = dto.DurationInDays;
                goalToEdit.NumberOfBooksToRead = dto.NumberOfBooksToRead;
                goalToEdit.Description = dto.Description;
                goalToEdit.Title = dto.Title;

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ReadingGoalDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            return new GenericResultDto<ReadingGoalDto> { Succeeded = true, Result = _mapper.Map<ReadingGoalDto>(goalToEdit) };
        }
        public async Task<GenericResultDto<BookDto>> AddBookToCollectionAsync(UserBookWithStatusDto dto, string userId)
        {
            var user = await _userManager
                .Users
                .Include(x => x.UserBooks)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "user not found" };

            if (user.UserBooks.Any(ub => ub.BookId == dto.BookId) == true)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "u already have this book in one of ur collections [to read, currently reading, already read]" };

            var bookResult = await _bookService.GetByIdAsync(dto.BookId);
            if(bookResult.Succeeded == true)
            {
                var book = bookResult.Result;
                await _bookService.AddTrendingBook(book.Id);
                
                foreach (var genreId in book.GenresDto.Select(g=>g.GenreId))
                {
                    await _recommendationService.IncrementPoints(user.Id, genreId);
                }
            }

            user.UserBooks.Add(_mapper.Map<UserBookWithStatus>(dto));
            var identityResult = await _userManager.UpdateAsync(user);

            if (identityResult.Succeeded == false)
                return new GenericResultDto<BookDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var result = await _bookService.GetByIdAsync(dto.BookId);
            if (result.Succeeded == false)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book added successfully but something went wrong while returing it in response" };
            return new GenericResultDto<BookDto> { Succeeded = true, Result = _mapper.Map<BookDto>(result.Result) };
        }
        public int GetBooksCount(UserBookStatus status, string userId)
        {
            return (from user in _userManager.Users
                    where user.Id == userId
                    select user.UserBooks.Where(b => b.Status == status))
                    .ToList()
                    .FirstOrDefault()
                    .Count();
        }
        public async Task<GenericResultDto<PagedResult<BookDto>>> GetBooksAsync(int? status, string userId, int page)
        {
            page = page==0? 1 : page;
            var bookStatus = IntToUserBookStatus(status);
            var user = await _userManager
                .Users
                .Include(x => x.UserBooks.Where(b => b.Status == bookStatus).Skip((page-1)*Pagination.Books).Take(Pagination.Books))
                .ThenInclude(x => x.Book)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<PagedResult<BookDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var userBooksWithStatus = user.UserBooks;
            var booksDto = new List<BookDto>();
            foreach (var bookWithStatus in userBooksWithStatus)
                booksDto.Add(_mapper.Map<BookDto>(bookWithStatus.Book));

            var result = new PagedResult<BookDto>
            {
                PageSize = Pagination.Books,
                Data = booksDto,
                PageNumber = page,
                TotalPages = Pagination.CalculatePagesCount(GetBooksCount(bookStatus, userId), Pagination.Books)
            };

            return new GenericResultDto<PagedResult<BookDto>> { Succeeded = true, Result = result };
        }
        private UserBookStatus IntToUserBookStatus(int? status)
        {
            var bookStatus = UserBookStatus.CURRENTLY_READING;
            switch (status)
            {
                case 0:
                    bookStatus = UserBookStatus.TO_READ;
                    break;
                case 1:
                case null:
                    bookStatus = UserBookStatus.CURRENTLY_READING;
                    break;
                case 2:
                    bookStatus = UserBookStatus.ALREADY_READ;
                    break;
                default:
                    bookStatus = UserBookStatus.CURRENTLY_READING;
                    break;
            }
            return bookStatus;
        }
        public async Task<GenericResultDto<BookDto>> UpdateBookStatusAsync(int bookId, int? status, string userId)
        {
            var bookStatus = IntToUserBookStatus(status);
            var user = await _userManager
                .Users
                .Include(x => x.UserBooks)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "user not found" };


            var book = user.UserBooks.SingleOrDefault(b => b.BookId == bookId);
            if(book == null)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "book not found" };

            book.Status = bookStatus;
            book.AddedDate = DateTime.UtcNow;

            var identityResult = await _userManager.UpdateAsync(user);

            if (identityResult.Succeeded == false)
                return new GenericResultDto<BookDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            var result = await _bookService.GetByIdAsync(bookId);

            if (result.Succeeded == false)
                return new GenericResultDto<BookDto> { Succeeded = false, ErrorMessage = "u updated the the book status successfully but something went wrong while fecthig ur updated book" };

            return new GenericResultDto<BookDto> { Succeeded = true, Result = result.Result };
        }
        public async Task<GenericResultDto<string>> DeleteBookAsync(int bookId, string userId)
        {
            var user = await _userManager
                .Users
                .Include(x => x.UserBooks.Where(b=>b.BookId == bookId))
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };

            var bookToDelete = user.UserBooks.SingleOrDefault(b => b.BookId == bookId);
            if (bookToDelete == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "book not found" };


            var bookResult = await _bookService.GetByIdAsync(bookToDelete.BookId);
            if (bookResult.Succeeded == true)
            {
                var book = bookResult.Result;
                foreach (var genreId in book.GenresDto.Select(g => g.GenreId))
                {
                    await _recommendationService.DecrementPoints(user.Id, genreId);
                }
            }

            user.UserBooks.Remove(bookToDelete);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<string>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<string> { Succeeded = true, Result = "book deleted successfully" };
        }
        public GenericResultDto<int> GetUserFollowersPagesCount(string userId)
        {
            var followersCount = (from user in _userManager.Users
                                 where user.Id == userId
                                 select user.Followers.Count).FirstOrDefault();

            var pagesCount = (int)Math.Ceiling((double)followersCount / Pagination.UserFollowersNames);
            return new GenericResultDto<int> { Succeeded = true, Result = pagesCount};
        }
        public GenericResultDto<int> GetUserFollowingsPagesCount(string userId)
        {
            var followingsCount = (from user in _userManager.Users
                                  where user.Id == userId
                                  select user.Followings.Count).FirstOrDefault();

            var pagesCount = (int)Math.Ceiling((double)followingsCount / Pagination.UserFollowingsNames);
            return new GenericResultDto<int> { Succeeded = true, Result = pagesCount };
        }
        public async Task<GenericResultDto<int>> GetGenresExceptUserGenresPagesCountAsync(string userId)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteGenres).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<int> { Succeeded = false, ErrorMessage = "user not found" };

            var userGenresCount = user.FavouriteGenres.Count;
            var genresCount = _genreRepository.GetCount();
            var genresExceptUserGenresCount = genresCount - userGenresCount;
            var result = (int)Math.Ceiling((double)genresExceptUserGenresCount / Pagination.Genres);

            return new GenericResultDto<int> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<int>> GetFavouriteAuthorsPagesCountAsync(string userId)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<int> { Succeeded = false, ErrorMessage = "user not found" };

            var userFavouriteAuthors = user.FavouriteAuthors.Count;
            var result = (int)Math.Ceiling((double)userFavouriteAuthors / Pagination.Authors);

            return new GenericResultDto<int> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<int>> GetFavouriteAuthorsExceptUserPagesCountAsync(string userId)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<int> { Succeeded = false, ErrorMessage = "user not found" };

            var userAuthorsCount = user.FavouriteAuthors.Count;
            var AuthorsCount = _authorRepository.GetCount();
            var authorsExceptUserCount = AuthorsCount - userAuthorsCount;
            var result = (int)Math.Ceiling((double) authorsExceptUserCount/ Pagination.Authors);
            return new GenericResultDto<int> { Succeeded = true, Result = result};
        }
        public async Task<GenericResultDto<int>> GetReadingGoalsPagesCountAsync(string userId)
        {
            var user = await _userManager.Users.Include(x => x.ReadingGoals).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<int> { Succeeded = false, ErrorMessage = "user not found" };

            var readingGoalsCount = user.ReadingGoals.Count;
            var result = (int)Math.Ceiling((double) readingGoalsCount/ Pagination.ReadingGoals);
            return new GenericResultDto<int> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<BookCollectionPages>> GetBooksPagesCountAsync(string userId)
        {
            var user = await _userManager.Users.Include(x => x.UserBooks).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<BookCollectionPages> { Succeeded = false, ErrorMessage = "user not found" };

            var alreadyReadBooksCount = user.UserBooks.Where(b => b.Status == UserBookStatus.ALREADY_READ).Count(); 
            var currentlyReadingBooksCount = user.UserBooks.Where(b => b.Status == UserBookStatus.CURRENTLY_READING).Count(); 
            var toReadBooksCount = user.UserBooks.Where(b => b.Status == UserBookStatus.TO_READ).Count();

            var result = new BookCollectionPages
            {
                AlreadyReadBooksPages = (int)Math.Ceiling((double)alreadyReadBooksCount / Pagination.BookTitlesAndIds),
                CurrentlyReadingBooksPages = (int)Math.Ceiling((double)currentlyReadingBooksCount / Pagination.BookTitlesAndIds),
                ToReadBooksPages = (int)Math.Ceiling((double)toReadBooksCount / Pagination.BookTitlesAndIds),
            };
            return new GenericResultDto<BookCollectionPages> { Succeeded = true, Result = result };
        }

        public async Task<GenericResultDto<string>> AddConnectionIdToUser(string userId, string connectionId)
        {
            if(userId == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user id is null" };
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };
            user.Connections.Add(new Connection { ConnectionId = connectionId });
            await _userManager.UpdateAsync(user);
            return new GenericResultDto<string> { Succeeded = true };
        }

        public async Task<GenericResultDto<string>> RemoveConnectionIdFromUser(string userId, string connectionId)
        {
            if (userId == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user id is null" };
            var user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<string> { Succeeded = false, ErrorMessage = "user not found" };
            var connection = user.Connections.SingleOrDefault(c => c.ConnectionId == connectionId);
            if (connection == null)
                return new GenericResultDto<string> { Succeeded = true };
            user.Connections.Remove(connection);
            await _userManager.UpdateAsync(user);
            return new GenericResultDto<string> { Succeeded = true };
        }

        public async Task<List<string>?> GetUserConnections(string userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return null;
            var connections = user.Connections.Select(c => c.ConnectionId);
            return connections.ToList();
        }
        public static bool IsUserFollowed(ICollection<ApplicationUser> followers, string followerId)
        {
            if (followers == null || followerId == null)
                return false;

            return followers.Any(f => f.Id == followerId);
        }

    }
}
