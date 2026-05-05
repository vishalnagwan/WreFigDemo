using WreFigDemo.Models.ViewModels;

namespace WreFigDemo.Services;

public interface IUserService
{
    Task<List<UserListVm>> GetUsersAsync();
    Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(CreateUserVm model, string actorId, string actorName);
    Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(EditUserVm model, string actorId, string actorName);
    Task<(bool Success, string Error)> DeactivateUserAsync(string userId, string actorId, string actorName);
}
