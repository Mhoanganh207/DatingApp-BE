using DatingApp.Data;
using DatingApp.Repository;

namespace DatingApp.Services;

public class FavouriteService : IFavouriteService
{
    private readonly AccountRepository _accountRepository = new AccountRepository(new AppDbContext());

   
    public async Task<string> AddFavouriteList(int id1, int id2)
    {
        return await _accountRepository.AddToFavouriteList(id1, id2);
    }
}