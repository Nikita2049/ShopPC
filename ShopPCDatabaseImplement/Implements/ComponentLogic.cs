﻿using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.Interfaces;
using ShopPCBusinessLogic.ViewModels;
using ShopPCDatabaseImplement;
using ShopPCDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopPCDatabaseImplement.Implements
{
    public class ComponentLogic : IComponentLogic
    {
        public void CreateOrUpdate(ComponentBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                Component element = context.Components.FirstOrDefault(rec =>
               rec.ComponentName == model.ComponentName && rec.Id != model.Id);
                if (element != null)
                {
                    throw new Exception("Уже есть компонент с таким названием");
                }
                if (model.Id.HasValue)
                {
                    element = context.Components.FirstOrDefault(rec => rec.Id ==
                   model.Id);
                    if (element == null)
                    {
                        throw new Exception("Элемент не найден");
                    }
                }
                else
                {
                    element = new Component();
                    context.Components.Add(element);
                }
                element.ComponentName = model.ComponentName;
                context.SaveChanges();
            }
        }
        public void Delete(ComponentBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                Component element = context.Components.FirstOrDefault(rec => rec.Id ==
               model.Id);
                if (element != null)
                {
                    context.Components.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        public List<ComponentViewModel> Read(ComponentBindingModel model)
        {
            using (var context = new ShopPCDatabase())
            {
                return context.Components
                .Where(rec => model == null || rec.Id == model.Id)
                .Select(rec => new ComponentViewModel
                {
                    Id = rec.Id,
                    ComponentName = rec.ComponentName
                })
                .ToList();
            }
        }
    }
}