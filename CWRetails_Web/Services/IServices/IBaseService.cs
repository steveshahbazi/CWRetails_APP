using CWRetails_Web.Models;

namespace CWRetails_Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse ResponseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
