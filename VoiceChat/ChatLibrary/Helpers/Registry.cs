using System.IO;
using Backend.Client;
using Microsoft.Win32;

namespace Backend.Helpers
{
    public class RegistryHelper
    {

        public static string AppPath
        {
            get
            {
                return Path.Combine(new[] { ChatHelper.SOFTWARE, ChatHelper.APP_NAME, ChatHelper.VERSION });
            }
        }


        public static void Write(ChatClient client)
        {
            var key = Registry.CurrentUser;
            if (key.OpenSubKey(AppPath) == null)
                key.CreateSubKey(AppPath);
            key = key.OpenSubKey(AppPath, true);
            if (key == null) 
                return;

            #region Write to registry
            key.SetValue(ChatHelper.LAUNCH_ON_STARTUP, client.LaunchOnStartup);
            key.SetValue(ChatHelper.DOUBLE_CLICK_TO_CALL, client.DoubleClickToCall);
            key.SetValue(ChatHelper.SCHEME, client.Scheme);
            #endregion
        }

        public static void Read(ChatClient client)
        {
            var key = Registry.CurrentUser;
            key = key.OpenSubKey(AppPath);
            
            if (key == null)
                return;

            #region Read registry keys
            var launchToStart = key.GetValue(ChatHelper.LAUNCH_ON_STARTUP);
            var dblClick = key.GetValue(ChatHelper.DOUBLE_CLICK_TO_CALL);
            var scheme = key.GetValue(ChatHelper.SCHEME);
            #endregion

            #region Initializing client
            client.DoubleClickToCall = bool.Parse((string) (dblClick ?? false));
            client.LaunchOnStartup = bool.Parse((string) (launchToStart ?? false));
            client.Scheme = (string) (scheme ?? ChatHelper.DARK);
            #endregion
        }
    }
}
