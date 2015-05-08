using System.Windows.Controls;
using Backend.Helpers;

namespace ChatControls.Client.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl 
    {
        public ComboBox Recording 
        {
            get
            {
                return cmbRecording;
            }
        }

        public ComboBox Playback
        {
            get
            {
                return cmbPlayback;
            }
        }

        public CheckBox LaunchOnStartup
        {
            get
            {
                return CmbLaunchOnStartup;
            }
        }

        public CheckBox DoubleClickToCall
        {
            get
            {
                return CmbDoubleClickToCall;
            }
        }


        public TabControl SettingsTabControl
        {
            get
            {
                return settingsTabControl;
            }
        }

        public ComboBox Scheme
        {
            get
            {
                return CmbScheme;
            }
        }

        public SettingsControl()
        {
            InitializeComponent();
            InitThemes();
        }

        public void InitThemes()
        {
            CmbScheme.Items.Add(ChatHelper.DARK);
            CmbScheme.Items.Add(ChatHelper.LIGHT);
        }

    }
}
