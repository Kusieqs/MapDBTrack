using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapDBTrack
{
    public static class HelpingClass
    {
        public static bool NetworkCheck(Window window)
        {
            while(true)
            {
                var client = new WebClient();
                var stream = client.OpenRead("http://www.google.com");
                if (stream == null)
                {
                    client.Dispose();
                    stream.Close();
                    MessageBoxResult result = MessageBox.Show("No internet connection\nDo you want try again?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if(result == MessageBoxResult.Yes)
                    {
                        continue;
                    }
                    else
                    {
                        window.Close();
                    }
                }
                break;
            }
            return true;
        }
    }
}
