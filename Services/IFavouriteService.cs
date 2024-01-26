namespace DatingApp.Services;

public interface IFavouriteService
{
    Task<string> AddFavouriteList(int id1, int id2);
}