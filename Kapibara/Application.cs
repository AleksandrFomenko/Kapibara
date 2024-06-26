﻿using System;
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
using System.Security.AccessControl;
using Autodesk.Revit.DB.Plumbing;

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
            qwe(application);


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
            PushButtonData pbdTFive = new PushButtonData("FiveButton", "Нумерация", assemblyPath, "Kapibara.Numeration");
            PushButtonData pbdTSix = new PushButtonData("SixButton", "Сортировка", assemblyPath, "Kapibara.NumerationGeneralFamilies");


            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib.png"));
            Uri uri1 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_length.png"));
            Uri uri2 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_write.png"));
            Uri uri3 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_floor.png"));
            Uri uri4 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_numeration.png"));
            Uri uri5 = new Uri(Path.Combine(Path.GetDirectoryName(assemblyPath), "Kapibara", "kapib_sorted.png"));
            BitmapImage bm_first = new BitmapImage(uri);
            BitmapImage bm_second = new BitmapImage(uri1);
            BitmapImage bm_Third = new BitmapImage(uri2);
            BitmapImage bm_fourth = new BitmapImage(uri3);
            BitmapImage bm_five = new BitmapImage(uri4);
            BitmapImage bm_six = new BitmapImage(uri5);
            pbdOne.LargeImage = bm_first;
            pbdTwo.LargeImage = bm_second;
            pbdThree.LargeImage = bm_Third;
            pbdTFour.LargeImage = bm_fourth;
            pbdTFive.LargeImage = bm_five;
            pbdTSix.LargeImage = bm_six;
            panel.AddItem(pbdOne);
            panel.AddItem(pbdTwo);
            panel.AddItem(pbdThree);
            panel.AddItem(pbdTFour);
            
           SplitButtonData splitButtonData = new SplitButtonData("SplitButton", "Меню");
           SplitButton sbOne = panel.AddItem(splitButtonData) as SplitButton;
           PushButton name1 = sbOne.AddPushButton(pbdTFive);
           PushButton name2 = sbOne.AddPushButton(pbdTSix);
           sbOne.AddSeparator();
           return Result.Succeeded;
        }
    private void qwe(UIControlledApplication application)
        {

            try
                {
                    PipeUpdater pipeUpdater = new PipeUpdater(application.ActiveAddInId);

                    UpdaterRegistry.RegisterUpdater(pipeUpdater);

                    ElementClassFilter pipeFilter = new ElementClassFilter(typeof(Pipe));
                    UpdaterRegistry.AddTrigger(pipeUpdater.GetUpdaterId(), pipeFilter, Element.GetChangeTypeElementAddition());

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("ошибка", "ошибка " + ex.Message);
                }
        }
        
    }
}

