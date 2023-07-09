using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using adWin = Autodesk.Windows;
using System.Windows.Media;
using System.Reflection;


namespace Kapibara
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Application : IExternalApplication
    {
        
            
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

             application.CreateRibbonTab("Kapibarja");
            



            RibbonPanel panel = application.CreateRibbonPanel("Kapibarja","MEP общие");
            RibbonPanel panel_two = application.CreateRibbonPanel("Kapibarja", "Вентиляция");
            adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;

            //фиолетовый
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPurple = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 255));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPink = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 218, 221));

            foreach (adWin.RibbonTab tab in ribbon.Tabs)
            {
                foreach (adWin.RibbonPanel panel1 in tab.Panels)
                {
                    if (panel1.Source.Title == "MEP общие")
                    {
                        panel1.CustomPanelTitleBarBackground = panelBackgroundBrushPurple;

                    }
                    if (panel1.Source.Title == "Вентиляция")
                    {
                        panel1.CustomPanelTitleBarBackground = panelBackgroundBrushPink;

                    }
                }
            }


            PushButtonData pbdOne = new PushButtonData("FirstButton", "Имя системы", assemblyPath, "Kapibara.SystemName");
            PushButtonData pbdTwo = new PushButtonData("SecondButton", "Длина \n инженерных \n сетей", assemblyPath, "Kapibara.Length");



            
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib.png"));
            BitmapImage bm_first = new BitmapImage(uri);
            pbdOne.LargeImage = bm_first;




            panel.AddItem(pbdOne);
            panel.AddItem(pbdTwo);

        

            return Result.Succeeded;
         
        }
  
    }
}

