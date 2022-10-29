using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ParameterTools.CopyParameters

{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CopyParamsCommand : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("A0ACA4A4-191E-4D2A-ABD7-F26A7D7D6758"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (doc.IsDocumentFamily())
            {
                return Result.Failed;
            }

            CopyParametersView copyForm = new CopyParametersView(commandData);           
            copyForm.ShowDialog();

            return Result.Succeeded;
        }
    }
}
