using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StorageUI.Models
{
    public class ResultViewModel
    {
        public bool IsOk { get; set; }
        public List<string> Errors { get; set; }
    }
}