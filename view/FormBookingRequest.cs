using System;
using System.Drawing;
using System.Windows.Forms;
using SmartKos.model;

namespace SmartKos.view
{
    public partial class FormBookingRequest : Form
    {
        public M_Booking BookingData { get; private set; }
        private int _kamarId;
        private int _userId;
        private string _kamarNo;

        private TextBox txtNama;
        private TextBox txtKTP;
        private TextBox txtHP;
        private Button btnSubmit;
        private Button btnCancel;

        public FormBookingRequest(int kamarId, string kamarNo, int userId)
        {
            _kamarId = kamarId;
            _kamarNo = kamarNo;
            _userId = userId;
            InitializeComponent();
            UIHelper.SetFormStyle(this);
            this.Text = $"Booking Request - Kamar {_kamarNo}";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void InitializeComponent()
        {
            Label lblHeader = new Label
            {
                Text = "Isi Data Calon Penghuni",
                Font = UIHelper.HeaderFont,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(20),
                BackColor = Color.White
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            
            Label l1 = new Label { Text = "Nama Lengkap:", AutoSize = true, Anchor = AnchorStyles.Right };
            Label l2 = new Label { Text = "No KTP:", AutoSize = true, Anchor = AnchorStyles.Right };
            Label l3 = new Label { Text = "No HP:", AutoSize = true, Anchor = AnchorStyles.Right };

            txtNama = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            txtKTP = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            txtHP = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            table.Controls.Add(l1, 0, 0); table.Controls.Add(txtNama, 1, 0);
            table.Controls.Add(l2, 0, 1); table.Controls.Add(txtKTP, 1, 1);
            table.Controls.Add(l3, 0, 2); table.Controls.Add(txtHP, 1, 2);

            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            btnSubmit = new Button { Text = "Kirim Permintaan", Dock = DockStyle.Right, Width = 150 };
            btnCancel = new Button { Text = "Batal", Dock = DockStyle.Left, Width = 80, DialogResult = DialogResult.Cancel };
            
            UIHelper.StyleButton(btnSubmit, true);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.BackColor = UIHelper.SecondaryColor;

            btnSubmit.Click += BtnSubmit_Click;

            pnlButtons.Controls.Add(btnSubmit);
            pnlButtons.Controls.Add(btnCancel);

            this.Controls.Add(table);
            this.Controls.Add(lblHeader);
            this.Controls.Add(pnlButtons);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrWhiteSpace(txtKTP.Text))
            {
                MessageBox.Show("Nama dan KTP wajib diisi!", "Peringatan");
                return;
            }

            BookingData = new M_Booking
            {
                KamarID = _kamarId,
                UserID = _userId,
                Nama = txtNama.Text,
                NoKTP = txtKTP.Text,
                NoHP = txtHP.Text,
                Status = "Pending",
                TanggalBooking = DateTime.Now
            };
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
