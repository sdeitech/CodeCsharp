namespace App.Api.Models
{
    public class JsonModel
    {

        public JsonModel(object responseData, string message, int statusCode, string appError = "")
        {
            Data = responseData;
            Message = message;
            StatusCode = statusCode;
            AppError = appError;
        }

        public object Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public string AppError { get; set; }
    }

}
