﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using YaqraApi.Services.IServices;

namespace YaqraApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly Mapper _mapper;
        public UserService(
            UserManager<ApplicationUser> userManager, 
            IGenreService genreService,
            IAuthorService authorService,
            IBookService bookService)
        {
            _userManager = userManager;
            _genreService = genreService;
            _authorService = authorService;
            _bookService = bookService;
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
           
            var oldPicPath = user.ProfilePicture;

            var createPic = Task.Run(async () =>
            {
                var picName = Path.GetFileName(pic.FileName);
                var picExtension = Path.GetExtension(picName);
                var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";

                var picPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfilePictures", picWithGuid);

                using(var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    user.ProfilePicture = picPath;
                }
            });
            var deleteOldPic = Task.Run(() =>
            {
                if(string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

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
           
            var oldPicPath = user.ProfileCover;

            var createPic = Task.Run(async () =>
            {
                var picName = Path.GetFileName(pic.FileName);
                var picExtension = Path.GetExtension(picName);
                var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";

                var picPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileCovers", picWithGuid);

                using(var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    user.ProfileCover = picPath;
                }
            });
            var deleteOldPic = Task.Run(() =>
            {
                if(string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

            var identityResult = await _userManager.UpdateAsync(user);
            if (identityResult.Succeeded == false)
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
        }
        public async Task<GenericResultDto<UserFollowerDto>> FollowUserAsync(UserIdDto dto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GenericResultDto<UserFollowerDto> { Succeeded = false, ErrorMessage = "user not found" };
            
            var followedUser = await _userManager.FindByIdAsync(dto.UserId);
            if (followedUser == null)
                return new GenericResultDto<UserFollowerDto> { Succeeded = false, ErrorMessage = "the user you want to follow not found" };

            user.Followings = new List<ApplicationUser> { followedUser };

            var identityResult = await _userManager.UpdateAsync(user);
            if(identityResult.Succeeded == false)
                return new GenericResultDto<UserFollowerDto>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };
            return new GenericResultDto<UserFollowerDto> { Succeeded = true, Result = new UserFollowerDto {Follower= user, Followed= followedUser} };
        }
        public async Task<GenericResultDto<UserDto>> GetUserAsync(string userId)
        {
            var dto = await _userManager.Users.Select(u => new
            {
                Id = u.Id,
                Username = u.UserName,
                Bio = u.Bio,
                ProfilePicture = u.ProfilePicture,
                ProfileCover = u.ProfileCover,
                FollowersCount = u.Followers.Count(),
                FollowingsCount = u.Followings.Count(),
            }).SingleOrDefaultAsync(x => x.Id == userId);

            if (dto == null)
                return new GenericResultDto<UserDto> { Succeeded = false, ErrorMessage = "user not found" };

            return new GenericResultDto<UserDto>
            {
                Succeeded = true,
                Result = new UserDto
                {
                    UserId = dto.Id,
                    Username = dto.Username,
                    Bio = dto?.Bio,
                    ProfilePicture = dto?.ProfilePicture,
                    ProfileCover = dto?.ProfileCover,
                    FollowersCount = dto.FollowersCount,
                    FollowingsCount = dto.FollowingsCount,
                }
            };
            
        }
        public GenericResultDto<List<UserNameAndId>> GetUserFollowersNames(string userId, int page )
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

            return new GenericResultDto<List<UserNameAndId>> { Succeeded= true, Result = followersDto };
        
        }
        public GenericResultDto<List<UserNameAndId>> GetUserFollowingsNames(string userId, int page)
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

            return new GenericResultDto<List<UserNameAndId>> { Succeeded = true, Result = followingDto };
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
        public async Task<GenericResultDto<List<GenreDto>>> GetAllGenresExceptUserGenresAsync(string userId, int page)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteGenres).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<GenreDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserGenreIds = new HashSet<int>();

            foreach (var item in user.FavouriteGenres)
                UserGenreIds.Add(item.Id);

            var genresExceptUser = new List<GenreDto>();
            var genres = (await _genreService.GetAllAsync(page)).Result;

            foreach (var item in genres)
            {
                if (UserGenreIds.Contains(item.GenreId) == false)
                    genresExceptUser.Add(item);
            }

            return new GenericResultDto<List<GenreDto>> { Succeeded = true, Result = genresExceptUser.ToList() };
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
        public async Task<GenericResultDto<ApplicationUser>> UpdateAllAsync(IFormFile? pic, IFormFile? cover, UserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return new GenericResultDto<ApplicationUser> { Succeeded = false, ErrorMessage = "user not found" };

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
                return new GenericResultDto<ApplicationUser>
                {
                    Succeeded = false,
                    ErrorMessage = UserHelpers.GetErrors(identityResult)
                };

            return new GenericResultDto<ApplicationUser> { Succeeded = true, Result = user };
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
        public async Task<GenericResultDto<List<AuthorDto>>> GetFavouriteAuthorsAsync(string userId, int page)
        {
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var result = new List<AuthorDto>();
            foreach (var item in user.FavouriteAuthors.Skip((page - 1) * Pagination.Authors).Take(Pagination.Authors))
                result.Add(new AuthorDto { Id = item.Id, Name = item.Name, Bio = item.Bio, Picture = item.Picture });
            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = result };
        }
        public async Task<GenericResultDto<List<AuthorDto>>> GetAllAuthorsExceptUserAuthorsAsync(string userId, int page)
        {
            page = page == 0 ? 1 : page;
            var user = await _userManager.Users.Include(x => x.FavouriteAuthors).SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<AuthorDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var UserAuthorsIds = new HashSet<int>();

            foreach (var item in user.FavouriteAuthors)
                UserAuthorsIds.Add(item.Id);

            var authorsExceptUser = new List<AuthorDto>();
            var authors = (await _authorService.GetAll(page)).Result;

            foreach (var item in authors)
            {
                if (UserAuthorsIds.Contains(item.Id) == false)
                    authorsExceptUser.Add(item);
            }

            return new GenericResultDto<List<AuthorDto>> { Succeeded = true, Result = authorsExceptUser.ToList() };
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
        public async Task<GenericResultDto<List<ReadingGoalDto>>> GetAllReadingGoalsAsync(string userId, int page)
        {
            page = page==0?1: page;
            var user = await _userManager.Users
                .Include(x => x.ReadingGoals.Skip((page-1)*Pagination.ReadingGoals).Take(Pagination.ReadingGoals))
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<ReadingGoalDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var result = new List<ReadingGoalDto>();
            foreach (var item in user.ReadingGoals)
                result.Add(_mapper.Map<ReadingGoalDto>(item));

            return new GenericResultDto<List<ReadingGoalDto>> { Succeeded = true, Result = result };
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

            if(dto.StartDate != null)
                goalToEdit.StartDate = dto.StartDate.Value;
            if(dto.DurationInDays != null)
                goalToEdit.DurationInDays = dto.DurationInDays.Value;
            if(dto.NumberOfBooksToRead != null)
                goalToEdit.NumberOfBooksToRead = dto.NumberOfBooksToRead.Value;
            if(dto.Description != null)
                goalToEdit.Description = dto.Description;
            if(dto.Title != null)
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
        public async Task<GenericResultDto<List<BookDto>>> GetBooksAsync(int? status, string userId, int page)
        {
            page = page==0? 1 : page;
            var bookStatus = IntToUserBookStatus(status);
            var user = await _userManager
                .Users
                .Include(x => x.UserBooks.Where(b => b.Status == bookStatus).Skip((page-1)*Pagination.Books).Take(Pagination.Books))
                .ThenInclude(x => x.Book)
                .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new GenericResultDto<List<BookDto>> { Succeeded = false, ErrorMessage = "user not found" };

            var userBooksWithStatus = user.UserBooks;
            var result = new List<BookDto>();
            foreach (var bookWithStatus in userBooksWithStatus)
                result.Add(_mapper.Map<BookDto>(bookWithStatus.Book));

            return new GenericResultDto<List<BookDto>> { Succeeded = true, Result = result };
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
    }
}
