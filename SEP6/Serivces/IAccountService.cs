using Microsoft.AspNetCore.Mvc;
using SEP6.Models;

namespace SEP6.Serivces
{
    public interface IAccountService
    {
        Task<Account> Login(string username, string password);
        Task<IActionResult?> Register(string username, string password);
        Task<int> GetRating(int movieID);
        Task<List<MovieReview>> GetReviews(int movieID);
        Task<IActionResult> PostReview(int movieID, string review);
        Task<IActionResult> AddRating(int movieId, decimal rating);
        Task<IActionResult> AddFavouriteMovie(int movieId);
        Task<List<MovieData>> GetFavouriteMovies();
    }
}
