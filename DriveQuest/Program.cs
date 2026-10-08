using System;
using System.Windows.Forms;
using DriveQuest.Data;
using DriveQuest.Forms;

namespace DriveQuest;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        DataStore.Seed();
        Application.Run(new LoginForm());
    }
}
