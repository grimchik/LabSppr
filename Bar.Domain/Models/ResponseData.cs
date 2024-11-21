using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bar.Domain.Models
{
    public class ResponseData<T>
    {
        public T? Data { get; set; }
        public bool Successful { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public static ResponseData<T> Success(T data)
        {
            return new ResponseData<T> { Data = data };
        }
        public static ResponseData<T> Error(string message, T? data = default)
        {
            return new ResponseData<T> { ErrorMessage = message, Successful = false, Data = data };
        }
    }
}
