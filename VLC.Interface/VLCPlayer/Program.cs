using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VLCPlayer {
class Program {
    public static string Path = string.Empty;
    [STAThread]
    static void Main(string[] args) {
        if (args.Length > 0) {
            Path = args[0].ToString();
        }

        VLCPlayer.App app = new VLCPlayer.App();
        VLCPlayerWin window = new VLCPlayerWin(Path);
        app.MainWindow = window;
        window.Show();
        app.Run();

    }
}
}
