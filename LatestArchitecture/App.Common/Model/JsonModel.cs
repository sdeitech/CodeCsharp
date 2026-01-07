namespace App.Common.Models
{
    public class JsonModel
    {
        public JsonModel() { }

        public JsonModel(object responseData, string message, int statusCode, string appError = "", Meta meta = null)
        {
            Data = responseData;
            Message = message;
            StatusCode = statusCode;
            AppError = appError;
            Meta = meta;
        }

        public object Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string AppError { get; set; }
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        public Meta() { }

        public Meta(int totalRecords, int pageNumber, int pageSize)
        {
            TotalRecords = totalRecords;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            DefaultPageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);
        }

        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int DefaultPageSize { get; set; }
        public int TotalRecords { get; set; }
    }

}