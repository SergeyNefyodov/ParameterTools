using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParameterTools.CopyParameters
{
    public class CopyParametersViewModel
    {
        private ExternalCommandData commandData;

        private Document document
        {
            get
            {
                return commandData.Application.ActiveUIDocument.Document;
            }
        }

        public List<SharedParameterElement> selectedParametersList { get; set; } = new List<SharedParameterElement>(8) 
        { 
            null, null, null, null, null, null, null, null
        };

        public List<SharedParameterElement> allParametersList 
        {
            get
            {                
                {
                    var _params = new FilteredElementCollector(document)
                .OfClass(typeof(SharedParameterElement))
                .WhereElementIsNotElementType()
                .Cast<SharedParameterElement>()
                .ToList();
                    return _params;
                }
            }
        }

        public List<BuiltInCategory> preSelectedBuiltInCategories 
        {
            get
            {
                return new List<BuiltInCategory>()
                {
                    BuiltInCategory.OST_FlexPipeCurves,
                    BuiltInCategory.OST_PipeCurves,
                    BuiltInCategory.OST_DuctCurves,
                    BuiltInCategory.OST_FlexDuctCurves,
                    BuiltInCategory.OST_PipeAccessory,
                    BuiltInCategory.OST_PipeFitting,
                    BuiltInCategory.OST_DuctAccessory,
                    BuiltInCategory.OST_DuctFitting,
                    BuiltInCategory.OST_PlumbingFixtures,
                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_GenericModel
                };
            }
        }
                
        public Dictionary<Category, BoolWrapper> dictCategories { get; set; }
        
        public void createDictCategories()
        {
            Categories categories = document.Settings.Categories;
            List<Category> categoriesToRecord = new List<Category>();

            foreach (var category in preSelectedBuiltInCategories)
            {
                categoriesToRecord.Add(Category.GetCategory(document, category));
            }

            categoriesToRecord.Sort((firstCat, secondCat) => firstCat.Name.CompareTo(secondCat.Name));

            Dictionary<Category, BoolWrapper> dict = new Dictionary<Category, BoolWrapper>();
            foreach (Category category in categoriesToRecord)
            {
                dict.Add(category, new BoolWrapper());
            }
            dictCategories = dict;
        }

        public DelegateCommand MainCommand { get; }

        public CopyParametersViewModel(ExternalCommandData commandData)
        {
            this.commandData = commandData;            
            MainCommand = new DelegateCommand(copyParameters);            
        }
        
        private void copyParameters()
        {            
            using (Transaction t = new Transaction(document, "Копирование параметров во вложенные элементы"))
            {
                t.Start();
                List<Element> sourceElements = getElementsToCopyFrom();
                foreach (var sourceElement in sourceElements)
                {
                    List<Element> targetElements = getDependentElementsToCopyTo(sourceElement);                    
                    copyParametersByGuids(sourceElement, targetElements);
                }
                t.Commit();
            }
        }
        private List<Element> getElementsToCopyFrom()
        {
            List<Element> elements = new List<Element>();
            foreach (var c in dictCategories)
            {
                if (c.Value.Value == false)
                    continue;
                else
                {
                    Category category = c.Key;
                    ElementCategoryFilter ecf = new ElementCategoryFilter(category.Id);
                    List<Element> elementsOfCategory = new FilteredElementCollector(document)
                        .WherePasses(ecf)
                        .WhereElementIsNotElementType()
                        .ToElements()
                        .ToList();
                    elements.AddRange(elementsOfCategory);
                }
            }
            return elements;
        }
        private List<Element> getDependentElementsToCopyTo(Element elem)
        {
            ElementFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
            ElementFilter insulationPipeFilter = new ElementCategoryFilter(BuiltInCategory.OST_PipeInsulations);
            ElementFilter insulationDuctFilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctInsulations);

            List<ElementId> idsFamily = elem.GetDependentElements(familyFilter).ToList();
            List<ElementId> idsPipeInsulation = elem.GetDependentElements(insulationPipeFilter).ToList();
            List<ElementId> idsDuctInsulation = elem.GetDependentElements(insulationDuctFilter).ToList();

            idsFamily.AddRange(idsPipeInsulation);
            idsFamily.AddRange(idsDuctInsulation);

            List<Element> elems = new List<Element>();
            foreach (ElementId id in idsFamily)
            {
                elems.Add(document.GetElement(id));
            }
            return elems;
        }
        private void copyParametersByGuids(Element sourceElement, List<Element> targetElements)
        {            
            List<Guid> sourceGuids = getSourceParametersGuids(sourceElement);
            foreach (Element targetElement in targetElements)
            {
                for (int i = 0; i < sourceGuids.Count; i++)
                {
                    Parameter sourceParameter = sourceElement.get_Parameter(sourceGuids[i]);
                    Parameter targetParameter = targetElement.get_Parameter(sourceGuids[i]);
                    if (sourceParameter != null &&
                        targetParameter != null &&
                        targetParameter.IsReadOnly == false)
                    {
                        switch (sourceParameter.StorageType)
                        {
                            case StorageType.String:
                                targetParameter.Set(sourceParameter.AsString());
                                break;
                            case StorageType.Integer:
                                targetParameter.Set(sourceParameter.AsInteger());
                                break;
                            case StorageType.ElementId:
                                targetParameter.Set(sourceParameter.AsElementId());
                                break;
                            case StorageType.Double:
                                targetParameter.Set(sourceParameter.AsDouble());
                                break;
                            default:
                                continue;
                        }
                    }
                }
            }
        }
           
        private List<Guid> getSourceParametersGuids(Element elem)
        {            
            List<Guid> guids = new List<Guid>();
            foreach (var param in selectedParametersList)
            {
                if (null == param)
                    continue;
                Parameter parameter = elem.get_Parameter(param.GuidValue);
                if (null != parameter)
                    guids.Add(param.GuidValue);
            }
            return guids;
        }
        
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
