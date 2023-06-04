using System.Security.AccessControl;
using static CWRetails_Utility.SD;

namespace CWRetails_Web.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public string? Uri { get; set; }
        public object? Data { get; set; }
    }
}
