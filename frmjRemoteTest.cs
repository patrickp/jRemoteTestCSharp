using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using com.jbase.jremote;
using Newtonsoft.Json;



namespace jRemoteTest
{
    public partial class frmjRemoteTest
    {
        internal frmjRemoteTest()
        {
            InitializeComponent();
        }

        private JConnection Conn;
        private JFile TestFile;

        private string strTemp;
        private string strCommand;
        private System.Exception errCode;
        private JExecuteResults Results;
        private JSelectList MyList;

        private JDynArray[] Params = new JDynArray[1];

        private void btnConnect_Click(object sender, System.EventArgs e)
        {
            string hostName = txtIPAddress.Text;
            string portNumber = "20003";

            try
            {
                DefaultJConnectionFactory connf = new DefaultJConnectionFactory { Host = hostName, Port = Convert.ToInt32(portNumber) /*This should match jAgent’s port*/ };
                com.jbase.jremote.Overides.RawJDynArrayToString = true;
                Conn = connf.getConnection(txtUser.Text, txtPassword.Text);

                
                
                /* Commented all of this out.  The connect button should just connect!!!
                JSubroutineParameters callParams = new JSubroutineParameters();
                callParams.add(new JDynArray(""));
                callParams.add(new JDynArray(""));

                JSubroutineParameters returnParams = Conn.call("GET.PORTNO", callParams);

                txtPort.Text = returnParams.get(0).ToString();

                callParams = new JSubroutineParameters();
                callParams.add(new JDynArray(""));
                callParams.add(new JDynArray(""));
                callParams.add(new JDynArray(""));

                returnParams = Conn.call("GET.VERSION", callParams);

                string strMajor = returnParams.get(0).get(1);
                string strMinor = returnParams.get(1).get(1);
                txtVersion.Text = strMajor + "." + strMinor;
                */
                txtResult.Text = "Logged in";
                
            }

            catch (Exception ex)
            {
                txtResult.Text = "Can't log in to server" + Environment.NewLine + ex.Message;
                return;
            }
        }

        private void btnFileOpen_Click(object sender, System.EventArgs e)
        {
            ClearResult();

            try
            {
                TestFile = Conn.open("testfile");
                txtResult.Text = "File 'testfile' opened sucessfully";
            }
            catch (Exception ex)
            {
                errCode = ex.GetBaseException();

                txtResult.Text = "Error opening file : " + Environment.NewLine + ex.Message;
                return;
            }
        }

        private void btnDisconnect_Click(object sender, System.EventArgs e)
        {
            ClearResult();

            try
            {
                Conn.close();
                txtPort.Text = "";
                txtVersion.Text = "";
            }
            catch (Exception ex)
            {
                txtResult.Text = "Unable to disconnect from server" + Environment.NewLine + ex.Message;
                return;
            }
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void btnExecute_Click(object sender, System.EventArgs e)
        {
            ClearResult();

            try
            {
                strCommand = txtExecute.Text;
                Results = Conn.execute(strCommand);
                txtResult.Text = Results.CapturingVar.ToString();

            }
            catch (Exception ex)
            {
                errCode = ex.GetBaseException();

                MessageBox.Show("Error executing " + strCommand + Environment.NewLine + ex.Message);
                return;
            }
        }

        private void ClearResult()
        {
            txtResult.Text = "";
            Application.DoEvents();
            txtResult.Text = "Cleared Results";
        }

        private static frmjRemoteTest _DefaultInstance;
        public static frmjRemoteTest DefaultInstance
        {
            get
            {
                if (_DefaultInstance == null)
                    _DefaultInstance = new frmjRemoteTest();

                return _DefaultInstance;
            }
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            try
            {
                // changed to MD
                TestFile = Conn.open("MD");
                MyList = TestFile.select();
                Conn.execute("SAVE-LIST TestList", MyList);
                txtResult.Text = "Saved list TestList";
            }

            catch (Exception ex)
            {
                errCode = ex.GetBaseException();

                MessageBox.Show("Error with SavedList" + Environment.NewLine + ex.Message);
                return;
            }
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            try
            {
                Conn.execute("GET-LIST TestList", MyList);
            }

            catch (Exception ex)
            {
                errCode = ex.GetBaseException();

                MessageBox.Show("Error Getting List" + Environment.NewLine + ex.Message);
                return;
            }
        }

        private void btnClearResult_Click(object sender, EventArgs e)
        {
            ClearResult();
        }

        private void btnOconvTest_Click(object sender, EventArgs e)
        {
            try
            {
                JFile file = Conn.open("MD");
                string myDate = Conn.iconv("29/02/2012", "D4");

                JDynArray initrec = new JDynArray();
                initrec.insert("à", 1, 0, 0);
                initrec.insert("é", 2, 0, 0);
                initrec.insert("Ravé", 3, 0, 0);
                initrec.insert(myDate, 4, 0, 0);
                file.write("1", initrec);

                JDynArray dyn = file.read("1");

                txtResult.Text = ("First attribute :" + dyn.get(1));
                txtResult.Text = txtResult.Text + "\n" + ("Second attribute:" + dyn.get(2));

                string s = dyn.get(3);

                myDate = Conn.oconv(dyn.get(4), "D4");

                string oconvString = Conn.oconv(s, "T3,1");
                txtResult.Text = txtResult.Text + "\n" + "After oconv for third character :" + oconvString;
                oconvString = Conn.oconv(s, "T4,1");
                txtResult.Text = txtResult.Text + "\n" + "After oconv for third character:" + oconvString;

                txtResult.Text = txtResult.Text + "\n" + "Complete string:" + s;

                dyn.replace("£", 2, 2);
                file.write("TestMD", dyn);
            }
            catch (Exception ex)
            {
                errCode = ex.GetBaseException();

                MessageBox.Show("Error during Oconv process: " + Environment.NewLine + ex.Message);
                return;
            }
        }

        public sealed class JSubParmsReflection : JSubroutineParameters
        {
            public override string ToString()
            {
                return ObjectExtensions.ToStringReflection(this);
            }
        }

        private void btnjDynArray_Click(object sender, EventArgs e)
        {
            //com.jbase.jremote.Overides.RawJDynArrayToString = true;

            ClearResult();

            JDynArray dyn = new JDynArray();
            dyn.replace("1", 1);
            dyn.replace("2", 2);
            dyn.replace("3", 3);
            dyn.replace("4", 4);

            dyn.Lower();

            dyn.Raise();

            txtResult.Text = dyn.ToString();
        }

        private void btnCallSub_Click(object sender, EventArgs e)
        {
            try
            {
                JSubroutineParameters pIn = new JSubroutineParameters();
                JDynArray p1 = new JDynArray();
                p1.insert("/tmp/MCR2015.txt", -1);
                p1.insert("MCR", -1);
                pIn.add(p1);
                pIn.add(new JDynArray(""));
                // This does not work as it is customer specific
                // it does although show how to call a subroutine
                JSubroutineParameters pOut = Conn.call("ACE.MCR.IMPORT", pIn);
                return;
            }
            catch (JRemoteException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //JSubroutineParameters callParams = new JSubroutineParameters();
            //callParams.add(new JDynArray(""));
            ////callParams.add(new JDynArray(""));

            ////JSubroutineParameters returnParams = Conn.call("TEST.JAGENT", callParams);
            //JSubroutineParameters returnParams = Conn.call("PASSLIST.SUB", callParams);

            //txtResult.Text = "";

            //for (int i = 0; i <= returnParams.Count; i++)
            //{
            //    txtResult.Text = txtResult.Text + String.Format("Result {1) = {2}",i, returnParams.get(i).get(1));
            //}
        }
    }
}