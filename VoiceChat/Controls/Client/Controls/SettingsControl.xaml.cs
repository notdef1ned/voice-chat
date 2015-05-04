using System.Windows.Controls;

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


        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
