/**
*   @author Adrian Wojcik
*   @file MainForm.cs
*   @date 02.11.17
*   @brief Main form class.
*   Based on project by Amund Gjersøe (www.codeproject.com/Articles/75770/Basic-serial-port-listening-application)
*/

/*
 * System libraries
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortApp.Serial;
using System.IO;

namespace SerialPortApp
{
    /*
    * Main form class. Inherit from form Form class.
    * Partial definition -  remider of the class defined in 
    * automatically generated file MainForm.designer.cs
    */
    public partial class MainForm : Form
    {
        //! Default constructor.
        public MainForm()
        {
            InitializeComponent();
            UserInitialization();
        }

        #region Fields

        SerialPortManager _spManager; /**< Custom serial port manager class object */

        #endregion

        #region Event handlers

        /*
         * Main form window closing event handling function.
         * @param sender - contains a reference to the control/object that raised the event.
         * @param e - contains the form closing event data.
         */
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();
        }

        /*
        * New serial port data recived event handlig function. Update of "tbDataReceive" text box.
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the serial port event data.
        */
        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {

            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbDataReceive.TextLength > maxTextLength)
                tbDataReceive.Text = tbDataReceive.Text.Remove(0, tbDataReceive.TextLength - maxTextLength);

            // Byte array to string
            string str = Encoding.ASCII.GetString(e.Data);

            tbDataReceive.AppendText(str); // tu zmień tylko nazwę która jest na dole toolbox1 chyba, zamień tbDataReceive wlasnie na te nazwe
            string s = tbDataReceive.Text;
            int s2 = s.Substring(0, s.LastIndexOf('x') - 1).LastIndexOf('x');
            int s3= s.LastIndexOf('x');
            if (s.Split('x').Length>2)
            {
                labelLux.Text = s.Substring(s2+1, s3-s2);
            }
                
            tbDataReceive.ScrollToCaret();
            
        }

        /*
        * Error handling function. Display message in groupBox "Exceptions".
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void _spManager_ErrorHandler(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<EventArgs>(_spManager_ErrorHandler), new object[] { sender, e });
                return;
            }

            error_label.Text = ((Exception)sender).Message;
        }

        /*
        * Handles the "Connect"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnStart_Click(object sender, EventArgs e)
        {
            Connect();
        }

        /*
        * Handles the "Diconnect"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnStop_Click(object sender, EventArgs e)
        {
            Disonnect();
        }

        /*
        * Handles the "Send"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnSend_Click(object sender, EventArgs e)
        {
            _spManager.Send(tbDataTransmit.Text);
        }

        /*
        * Handles the "Clear"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnClear_Click(object sender, EventArgs e)
        {
            tbDataReceive.Clear();
        }

        #endregion

        #region Methods

        /*
         * User custom initialization.
         */ 
        private void UserInitialization()
        {
            // New serial port manager
            _spManager = new SerialPortManager();

            // Read current serial port settings 
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;

            // Bind serial port & user interface data sources with serial port settings
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            // Add evnet handling functions to serial port manager
            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            _spManager.ErrorHandler += new EventHandler<EventArgs>(_spManager_ErrorHandler);

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            // Diable "Disconnect" button
            btnStop.Enabled = false;
        }

        /*
        * Connect procedure - open and start listening on COM port
        */ 
        private void Connect()
        {
            if (_spManager.StartListening())
            {
                btnStop.Enabled = true;
                btnStart.Enabled = false;
                portNameComboBox.Enabled = false;
                baudRateComboBox.Enabled = false;
                dataBitsComboBox.Enabled = false;
                parityComboBox.Enabled = false;
                stopBitsComboBox.Enabled = false;
            }
        }

        /*
        * Disconnect procedure - close and stop listening on COM port
        */
        private void Disonnect()
        {
            _spManager.StopListening();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            portNameComboBox.Enabled = true;
            baudRateComboBox.Enabled = true;
            dataBitsComboBox.Enabled = true;
            parityComboBox.Enabled = true;
            stopBitsComboBox.Enabled = true;
        }

        #endregion

        private void groupBox_transmit_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox_receive_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int podana_wartosc;
            if (int.TryParse(textBoxZadana.Text, out podana_wartosc))
            {
                if (textBoxZadana.Text.Length < 4)
                {
                    labelError.Text = "";

                    string text = podana_wartosc.ToString();
                    for (int i = 0; i < 3 - textBoxZadana.Text.Length; i++)
                        text = "0" + text;

                    _spManager.Send(text);
                    labelZadane.Text = podana_wartosc.ToString();
                }
                else if (podana_wartosc < 0)
                {
                    labelError.Text = "Dioda LED nie pochłonie światła";
                }
                else
                {
                    labelError.Text = "Zaduża wartość";
                }
            }
            else
                labelError.Text = "To nie liczba, gamoniu!";
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
