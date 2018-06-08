using System.Windows;
using Lidgren.Network;

namespace CitiesSkylinesMultiplayer.GUITester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private NetClient _netClient;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _netClient = new NetClient(new NetPeerConfiguration("Tango"));

            NetOutgoingMessage approvalMessage = _netClient.CreateMessage();
            approvalMessage.Write("12456");
            _netClient.Start();

            _netClient.Connect(IpBox.Text, int.Parse(PortBox.Text));

            MessageBox.Show("Should be connected?");
        }
    }
}
