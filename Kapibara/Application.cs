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

            
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPurple = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 255));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPink = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 218, 221));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushYellow = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 253, 208));

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
            PushButtonData pbdTwo = new PushButtonData("SecondButton", "Длина инженерных\nсетей", assemblyPath, "Kapibara.Length");
            PushButtonData pbdThree = new PushButtonData("ThidButton", "Тётя Лена", assemblyPath, "Kapibara.WritingToParameter");
            PushButtonData pbdTFour = new PushButtonData("FourthButton", "Этаж", assemblyPath, "Kapibara.Floor");
           // PushButtonData pbdTFive = new PushButtonData("FivethhButton", "тестовое задание", assemblyPath, "Kapibara.test_task");




            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib.png"));
            Uri uri1 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_length.png"));
            Uri uri2 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_write.png"));
            Uri uri3 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_floor.png"));
            BitmapImage bm_first = new BitmapImage(uri);
            BitmapImage bm_second = new BitmapImage(uri1);
            BitmapImage bm_Third = new BitmapImage(uri2);
            BitmapImage bm_fourth = new BitmapImage(uri3);
            pbdOne.LargeImage = bm_first;
            pbdTwo.LargeImage = bm_second;
            pbdThree.LargeImage = bm_Third;
            pbdTFour.LargeImage = bm_fourth;




            panel.AddItem(pbdOne);
            panel.AddItem(pbdTwo);
            panel.AddItem(pbdThree);
            panel.AddItem(pbdTFour);
           
            return Result.Succeeded;
         
        }
  
    }
}

