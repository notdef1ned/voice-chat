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
            settings = new SettingsControl
            {
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };

            Header = ChatHelper.SETTINGS;
            this.client = client;
            Content = settings;

            #region Initializing Settings Page
            settings.LaunchOnStartup.IsChecked = client.LaunchOnStartup;
            settings.DoubleClickToCall.IsChecked = client.DoubleClickToCall;
            settings.CmbScheme.SelectedItem = client.Scheme ?? ChatHelper.DARK;
            #endregion

            #region Event subscription
            Loaded += SettingsPage_Loaded;
            settings.SettingsTabControl.KeyDown += SettingsPage_KeyDown;
            settings.CmbScheme.SelectionChanged += CmbSchemeOnSelectionChanged;
            #endregion

        }

        private void CmbSchemeOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SetEdited();
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
            client.Scheme = settings.CmbScheme.SelectedItem.ToString();

            if (settings.DoubleClickToCall.IsChecked != null)
                client.DoubleClickToCall = (bool) settings.DoubleClickToCall.IsChecked;

            if (settings.LaunchOnStartup.IsChecked != null)
                client.LaunchOnStartup = (bool) settings.LaunchOnStartup.IsChecked;

            RegistryHelper.Write(client);

            Header = ChatHelper.SETTINGS;
            isEdited = false;
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

            #region Events Subscription

            settings.Playback.SelectionChanged += Playback_SelectionChanged;
            settings.Recording.SelectionChanged += Recording_SelectionChanged;
            
            #endregion
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
