using DotNetEnv;
using System;
using System.Windows;

namespace work
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Env.Load();                      
            base.OnStartup(e);
        }
    }
}
