using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.HelperModels;
using ShopPCBusinessLogic.Interfaces;
using ShopPCBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopPCBusinessLogic.BusinessLogics
{
    public class ReportLogic
    {
        private readonly IComponentLogic ComponentLogic;
        private readonly IProductLogic ProductLogic;
        private readonly IOrderLogic orderLogic;
        public ReportLogic(IProductLogic ProductLogic, IComponentLogic ComponentLogic,
       IOrderLogic orderLogic)
        {
            this.ProductLogic = ProductLogic;
            this.ComponentLogic = ComponentLogic;
            this.orderLogic = orderLogic;
        }
        /// <summary>
        /// Получение списка компонент с указанием, в каких изделиях используются
        /// </summary>
        /// <returns></returns>
        public List<ReportProductComponentViewModel> GetProductComponent()
        {
            var Products = ProductLogic.Read(null);
            var list = new List<ReportProductComponentViewModel>();
            foreach (var product in Products)
            {
                foreach (var sf in product.ProductComponents)
                {
                    var record = new ReportProductComponentViewModel
                    {
                        ProductName = product.ProductName,
                        ComponentName = sf.Value.Item1,
                        Count = sf.Value.Item2
                    };
                    list.Add(record);
                }
            }
            return list;
        }
        public List<IGrouping<DateTime, OrderViewModel>> GetOrders(ReportBindingModel model)
        {
            var list = orderLogic
            .Read(new OrderBindingModel
            {
                DateFrom = model.DateFrom,
                DateTo = model.DateTo
            })
            .GroupBy(rec => rec.DateCreate.Date)
            .OrderBy(recG => recG.Key)
            .ToList();

            return list;
        }
        /// <summary>
        /// Сохранение компонент в файл-Word
        /// </summary>
        /// <param name="model"></param>
        public void SaveProductsToWordFile(ReportBindingModel model)
        {
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = model.FileName,
                Title = "Список системных блоков",
                Products = ProductLogic.Read(null)
            });
        }
        /// <summary>
        /// Сохранение закусок с указаеним продуктов в файл-Excel
        /// </summary>
        /// <param name="model"></param>
        public void SaveOrdersToExcelFile(ReportBindingModel model)
        {
            SaveToExcel.CreateDoc(new ExcelInfo
            {
                FileName = model.FileName,
                Title = "Список заказов",
                Orders = GetOrders(model)
            });
        }
        /// <summary>
        /// Сохранение системных блоках с компонентами в файл-Pdf
        /// </summary>
        /// <param name="model"></param>
        public void SaveProductsToPdfFile(ReportBindingModel model)
        {
            SaveToPdf.CreateDoc(new PdfInfo
            {
                FileName = model.FileName,
                Title = "Список системных блоков по компонентам",
                ProductComponents = GetProductComponent(),
            });
        }
    }
}