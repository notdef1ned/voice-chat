using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaseControls
{
    /// <summary>
    /// Base chat window
    /// </summary>
    public class ChatWindow : Window
    {
        private void Header_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public override void OnApplyTemplate()
        {
            var border = FindName(Constants.WINDOW_BORDER) as Border;
            if (border != null)
            {
                border.MouseMove += Header_MouseMove;
            }
            base.OnApplyTemplate();
        }
    }
}
