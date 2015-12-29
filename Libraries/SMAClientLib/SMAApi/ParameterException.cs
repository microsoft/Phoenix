using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMAApi.Entities;

namespace SMAApi
{
    public class ParameterException : Exception
    {
        public ParameterException()
            : base()
        {
        }

        public ParameterException(List<ParameterError> errors)          
        {
            this.Errors = errors;
        }

        public List<ParameterError> Errors { get; private set; }

    }

    public class ParameterError
    {
        public SMARunbookParameter Parameter { get; set; }

        public string ErrorMessage { get; set; }
    }
}
