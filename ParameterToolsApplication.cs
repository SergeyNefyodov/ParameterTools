using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace ParameterTools
{
    public class ParameterToolsApplication : IExternalApplication
    {
        static AddInId appId = new AddInId(new Guid("B5AA5145-BBB2-4EC4-A39D-CA84A6D8FA6C"));
        public Result OnStartup(UIControlledApplication app)
        {    
            string folderPath = @"C:\ProgramData\Autodesk\Revit\Addins\2022\CDSEngineeringTools"; 
            string dllPath = Path.Combine(folderPath, @"ParameterTools.dll");    
            
            RibbonPanel ParamsPanel = app.CreateRibbonPanel("Parameter Tools");
            PushButton ParamsCopyButton = (PushButton)ParamsPanel.AddItem(
                new PushButtonData("Copy Values", "Copy Values", dllPath, "PrintTools.CopyParameters.CopyParamsCommand"));
            
            if (File.Exists(Path.Combine(folderPath, "CopyParamsIcon.png")))
            {
                ParamsCopyButton.LargeImage = new BitmapImage(
                    new Uri(Path.Combine(folderPath, "CopyParamsIcon.png"), UriKind.Absolute));
            }
            ParamsCopyButton.ToolTip = 
                "Копирует значения выбранных общих параметров в общие вложенные семейства и в изоляцию труб и воздуховодов";
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
