using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureViewer {
public class PictureViewHelper {
    static MainWindow mainWindow = new MainWindow();
    public static void ShowPictureView(List<ImageBean> listImageBeans,ImageBean currentImageBean) {
        try {
            if (mainWindow.IsVisible) mainWindow.Close();
            mainWindow = new MainWindow();
            mainWindow.Init(listImageBeans, currentImageBean).Show();
            mainWindow.Topmost = true;
        } catch (Exception ex) {
            Console.WriteLine(ex.StackTrace);
        }

    }

    public static void AppendImage(ImageBean imageBean) {
    }

}
}
