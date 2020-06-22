﻿using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.Interfaces;
using ShopPCBusinessLogic.ViewModels;
using ShopPCDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopPCDatabaseImplement.Implements
{
    public class ProductLogic : IProductLogic
    {
        public void CreateOrUpdate(ProductBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Product element = context.Products.FirstOrDefault(rec =>
                       rec.ProductName == model.ProductName && rec.Id != model.Id);
                        if (element != null)
                        {
                            throw new Exception("Уже есть системныей блок с таким названием");
                        }
                        if (model.Id.HasValue)
                        {
                            element = context.Products.FirstOrDefault(rec => rec.Id ==
                           model.Id);
                            if (element == null)
                            {
                                throw new Exception("Элемент не найден");
                            }
                        }
                        else
                        {
                            element = new Product();
                            context.Products.Add(element);
                        }
                        element.ProductName = model.ProductName;
                        element.Price = model.Price;
                        context.SaveChanges();
                        if (model.Id.HasValue)
                        {
                            var ProductComponents = context.ProductComponents.Where(rec
                           => rec.ProductId == model.Id.Value).ToList();
                            // удалили те, которых нет в модели
                            context.ProductComponents.RemoveRange(ProductComponents.Where(rec =>
                            !model.ProductComponents.ContainsKey(rec.ComponentId)).ToList());
                            context.SaveChanges();
                            // обновили количество у существующих записей
                            foreach (var updateComponent in ProductComponents)
                            {
                                updateComponent.Count =
                               model.ProductComponents[updateComponent.ComponentId].Item2;

                                model.ProductComponents.Remove(updateComponent.ComponentId);
                            }
                            context.SaveChanges();
                        }
                        // добавили новые
                        foreach (var pc in model.ProductComponents)
                        {
                            context.ProductComponents.Add(new ProductComponent
                            {
                                ProductId = element.Id,
                                ComponentId = pc.Key,
                                Count = pc.Value.Item2
                            });
                            context.SaveChanges();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void Delete(ProductBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // удаяем записи по продуктам при удалении закуски
                        context.ProductComponents.RemoveRange(context.ProductComponents.Where(rec =>
                        rec.ProductId == model.Id));
                        Product element = context.Products.FirstOrDefault(rec => rec.Id
                        == model.Id);
                        if (element != null)
                        {
                            context.Products.Remove(element);
                            context.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Элемент не найден");
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<ProductViewModel> Read(ProductBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                return context.Products
                .Where(rec => model == null || rec.Id == model.Id)
                .ToList()
               .Select(rec => new ProductViewModel
               {
                   Id = rec.Id,
                   ProductName = rec.ProductName,
                   Price = rec.Price,
                   ProductComponents = context.ProductComponents
                .Include(recPC => recPC.Component)
               .Where(recPC => recPC.ProductId == rec.Id)
               .ToDictionary(recPC => recPC.ComponentId, recPC =>
                (recPC.Component?.ComponentName, recPC.Count))
               })
               .ToList();
            }
        }
    }
}