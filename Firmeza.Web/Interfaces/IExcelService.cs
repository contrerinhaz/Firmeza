using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Firmeza.Web.Interfaces
{
    public interface IExcelService
    {
        Task<bool> ProcessExcelAsync(IFormFile file);
    }
}