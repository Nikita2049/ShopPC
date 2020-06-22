using ShopPCBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPCBusinessLogic.HelperModels
{
    class PdfInfo
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public List<ReportProductComponentViewModel> ProductComponents { get; set; }
    }
}