
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManagementAPI.Erorrs
{
    public class ApiErrorResponse
    {
        public ApiErrorResponse(int code,string? message=null) {
            StatusCode = code;
            Message = message ?? getDefaultErrorMessage(code);
        }

        private string getDefaultErrorMessage(int code)
        {
            return code switch
            {
                400 => "Bad Request.",
                401 => "You are not Authorized.",
                403 => "Forbidden to do that!",
                404 => "Resources Not Fount.",
                500 => "Server Error! Try Again later.",
                _ => "Unexpecting Error."
            } ;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
