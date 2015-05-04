using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend.Client;
using Backend.Helpers;
using BaseControls;
using ChatControls.Client.Controls;
using NAudio.CoreAudioApi;

namespace ChatControls.Client.Pages
{
    public class SettingsPage : ChatTabItem
    {
        private readonly SettingsControl settings;
        private readonly ChatClient client;
        private bool isEdited;
        
        public SettingsPage(ChatClient client)
        {
            settings = new SettingsControl();
            Header = ChatHelper.SETTINGS;
            this.client = client;
            Content = settings;

            #region Event subscription
            settings.Playback.SelectionChanged += Playback_SelectionChanged;
            settings.Recording.SelectionChanged += Recording_SelectionChanged;
            Loaded += SettingsPage_Loaded;
            KeyDown += SettingsPage_KeyDown;
            #endregion

        }

        private void SettingsPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
            {
                ApplyChanges();
            }
        }


        private void ApplyChanges()
        {
            client.InputAudioDevice = settings.Recording.SelectedIndex;
            client.OutputAudioDevice = settings.Playback.SelectedIndex;
        }

        private void SetEdited()
        {
            if (isEdited) 
                return;
            isEdited = true;
            Header += "*";
        }

        private void Recording_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEdited();   
        }

        private void Playback_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEdited();
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var outDevices = GetOutputDevices();
            foreach (var device in outDevices)
               settings.Playback.Items.Add(device);
            
            var inDevices = GetInputDevices();
            foreach (var device in inDevices)
                settings.Recording.Items.Add(device);
            
            settings.Recording.SelectedIndex = client.InputAudioDevice;
            settings.Playback.SelectedIndex = client.OutputAudioDevice;
        }


        #region Obtain devices

        /// <summary>
        /// Gets all system sound devices
        /// </summary>
        /// <returns></returns>
        public static string[] GetOutputDevices()
        {
            var enumerator = new MMDeviceEnumerator();

            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).
                Select(endpoint => endpoint.FriendlyName).ToArray();
        }

        public static string[] GetInputDevices()
        {
            var enumerator = new MMDeviceEnumerator();

            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).
                Select(endpoint => endpoint.FriendlyName).ToArray();
        }

        #endregion
    }
}
