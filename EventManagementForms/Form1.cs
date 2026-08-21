using EventManagementForms.Entities;
using EventManagementForms.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventManagementForms
{
    public partial class Form1 : Form
    {
        private readonly OpenFileDialog ofd = new OpenFileDialog();
        private bool isDefaultImage = true;
        private string previousImage = "";
        private int intEventId = 0;
        private Event events = new Event();
        private readonly EventRepo repo = new EventRepo();





        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCustomerCombo();
            LoadEventGrid();
            ClearAll();
        }

        private void ClearAll()
        {

            string noImagePath = Path.Combine(Application.StartupPath, "images", "noimage.png");
            if (File.Exists(noImagePath))
            {
                using (var stream = new FileStream(noImagePath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
            }

            isDefaultImage = true;
            previousImage = "";
            chkMultipleDay.Checked = false;
            txtName.Clear();
            txtBudget.Clear();
            txtDuration.Clear();
            dgvNewPrograms.DataSource = null;
            dgvPrograms.DataSource = null;
            btnSave.Text = "Save";
            intEventId = 0;
            events = new Event();
        }


        private void LoadCustomerCombo()
        {
            DataTable dt = repo.GetAllCustomers();
            DataRow topRow = dt.NewRow();
            topRow[0] = 0;
            topRow[1] = "--Select Customer--";
            dt.Rows.InsertAt(topRow, 0);

            cmbCustomer.DataSource = dt;
            cmbCustomer.DisplayMember = "CustomerName";
            cmbCustomer.ValueMember = "CustomerId";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated())
            {
                MessageBox.Show("Provide correct information", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                events.EventId = intEventId;
                events.EventName = txtName.Text;
                events.IsMultipleProgramEvent = chkMultipleDay.Checked;
                events.StartDate = dtpStartDate.Value;
                events.EndDate = dtpEndDate.Value;
                events.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                events.Budget = Convert.ToInt32(txtBudget.Text);

                if (isDefaultImage)
                {
                    events.ImageUrl = "noimage.png";
                }
                else if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    // Handle image cleanup for updates
                    if (intEventId > 0 && previousImage != "" && previousImage != "noimage.png")
                    {
                        DeleteImageFile(previousImage);
                    }
                    events.ImageUrl = SaveImage(ofd.FileName);
                }

                if (intEventId == 0)
                {
                    if (repo.SaveEvent(events) > 0)
                    {
                        MessageBox.Show("Saved Successfully");
                    }
                }
                else
                {
                    if (repo.UpdateEvent(events) > 0)
                    {
                        MessageBox.Show("Updated Successfully");
                    }
                }

                LoadEventGrid();
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool IsValidated()
        {
            return !string.IsNullOrWhiteSpace(txtName.Text) &&
                   cmbCustomer.SelectedIndex > 0;
        }

        private void btnNewProgramsSave_Click(object sender, EventArgs e)
        {

            Programs programs = new Programs
            {
                ProgramsName = txtProgramsTitle.Text,
                Duration = Convert.ToInt32(txtDuration.Text)
            };
            events.Programs.Add(programs);
            LoadProgramsGrid(events.Programs, 0);
        }


        private void LoadProgramsGrid(List<Programs> programs, int eventId)
        {
            dgvNewPrograms.DataSource = null;
            dgvNewPrograms.Columns.Clear();

            if (eventId == 0)
                dgvNewPrograms.DataSource = ConvertToDataTable(programs);
            else
                dgvNewPrograms.DataSource = repo.GetProgramsByEventId(eventId);

            AddGridButtonToControl(dgvNewPrograms, "Delete", "Delete");
            ClearProgramsInputs();
        }


        private void AddGridButtonToControl(DataGridView grid, string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                Width = 60,
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(btn);
        }


        private void ClearProgramsInputs()
        {
            txtProgramsTitle.Clear();
        }


        private DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) dataTable.Columns.Add(prop.Name);

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++) values[i] = Props[i].GetValue(item, null);
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Images(.jpg,.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
                isDefaultImage = false;
                previousImage = "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            using (var stream = new FileStream(Application.StartupPath + "\\images\\noimage.png", FileMode.Open, FileAccess.Read))
            {
                pbUpload.Image = Image.FromStream(stream);
            }
            isDefaultImage = true;
            previousImage = "";
        }

        private void LoadEventGrid()
        {
            DataTable dt = repo.GetAllEvents();
            if (!dt.Columns.Contains("Image"))
                dt.Columns.Add("Image", typeof(byte[]));

            foreach (DataRow dr in dt.Rows)
            {
                string imgName = dr["ImageUrl"].ToString();
                string imagePath = Path.Combine(Application.StartupPath, "images", imgName);
                string defaultPath = Path.Combine(Application.StartupPath, "images", "noimage.png");

                try
                {
                    dr["Image"] = File.Exists(imagePath) ? File.ReadAllBytes(imagePath) : File.ReadAllBytes(defaultPath);
                }
                catch { dr["Image"] = null; }
            }

            dgvEvent.DataSource = null;
            dgvEvent.Columns.Clear();
            dgvEvent.DataSource = dt;

            // 1. Add the Buttons
            AddGridButton("Details", "Details");
            AddGridButton("Edit", "Edit");
            AddGridButton("Delete", "Delete");

            // 2. Set DisplayIndex to move buttons to the left
            // 0 is the leftmost position
            dgvEvent.Columns["Details"].DisplayIndex = 0;
            dgvEvent.Columns["Edit"].DisplayIndex = 1;
            dgvEvent.Columns["Delete"].DisplayIndex = 2;

            // 3. Adjust other columns
            dgvEvent.RowTemplate.Height = 80;

            if (dgvEvent.Columns["Image"] is DataGridViewImageColumn imgCol)
            {
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.DisplayIndex = 3; // Put the image after the buttons
            }

            if (dgvEvent.Columns.Contains("EventId")) dgvEvent.Columns["EventId"].Visible = false;
            if (dgvEvent.Columns.Contains("ImageUrl")) dgvEvent.Columns["ImageUrl"].Visible = false;
        }


        private void AddGridButton(string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                UseColumnTextForButtonValue = true
            };
            dgvEvent.Columns.Add(btn);
        }


        private void DeleteImageFile(string fileName)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "images", fileName);
                if (File.Exists(path) && fileName != "noimage.png")
                {
                    pbUpload.Image?.Dispose();
                    pbUpload.Image = null;
                    File.Delete(path);
                }
            }
            catch (Exception ex) { MessageBox.Show("Image Delete Error: " + ex.Message); }
        }


        private string SaveImage(string imgPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imgPath);
            string ext = Path.GetExtension(imgPath);
            // Limit filename length and add a timestamp to ensure uniqueness
            fileName = fileName.Length <= 15 ? fileName : fileName.Substring(0, 15);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + ext;
            string directoryPath = Path.Combine(Application.StartupPath, "images");
            // Ensure the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fullSavePath = Path.Combine(directoryPath, fileName);
            using (Bitmap bmp = new Bitmap(pbUpload.Image))
            {
                bmp.Save(fullSavePath);
            }
            return fileName;
        }

        private void dgvNewPrograms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;

            if (dgvNewPrograms.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this Programs?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int sId = Convert.ToInt32(dgvNewPrograms.Rows[e.RowIndex].Cells["EventId"].Value);
                        int mId = Convert.ToInt32(dgvNewPrograms.Rows[e.RowIndex].Cells["ProgramsId"].Value);

                        if (sId > 0 && mId > 0)
                        {
                            repo.DeleteProgramsByEventId(sId, mId);
                        }
                    }
                    catch { }

                    if (events.Programs.Count > e.RowIndex) events.Programs.RemoveAt(e.RowIndex);
                    LoadProgramsGrid(events.Programs, 0);
                }
            }
        }

        private void dgvEvent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;

            try
            {
                int sId = Convert.ToInt32(dgvEvent.Rows[e.RowIndex].Cells["EventId"].Value);
                string command = dgvEvent.Columns[e.ColumnIndex].Name;
                switch (command)
                {
                    case "Details":
                        dgvPrograms.DataSource = repo.GetProgramsByEventId(sId);
                        break;

                    case "Edit":
                        EditEventInfo(sId);
                        break;

                    case "Delete":
                        if (MessageBox.Show("Are you sure you want to delete this Event and their Programs?",
                                            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            DeleteEventInfo(sId);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the request: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DeleteEventInfo(int eventId)
        {
            DataTable dt = repo.GetEventById(eventId);
            if (dt.Rows.Count > 0) DeleteImageFile(dt.Rows[0]["ImageUrl"].ToString());

            repo.DeleteProgramsByEventId(eventId);
            repo.DeleteEvent(eventId);

            LoadEventGrid();
            ClearAll();
        }

        private void EditEventInfo(int eventId)
        {
            DataTable dt = repo.GetEventById(eventId);
            if (dt == null || dt.Rows.Count == 0) return;

            btnSave.Text = "Update";
            intEventId = eventId;
            DataRow row = dt.Rows[0];

            txtName.Text = row["EventName"].ToString();
            cmbCustomer.SelectedValue = row["CustomerId"];
            dtpStartDate.Value = Convert.ToDateTime(row["StartDate"]);
            dtpEndDate.Value = Convert.ToDateTime(row["EndDate"]);
            chkMultipleDay.Checked = row["IsMultipleProgramEvent"] != DBNull.Value && (bool)row["IsMultipleProgramEvent"];
            txtBudget.Text = row["Budget"].ToString();

            previousImage = row["ImageUrl"].ToString();
            string imgPath = Path.Combine(Application.StartupPath, "images", previousImage);

            if (File.Exists(imgPath))
            {
                using (var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
                isDefaultImage = false;
            }
            else { isDefaultImage = true; }

            events.Programs = ConvertDataTableToPrograms(eventId);
            LoadProgramsGrid(events.Programs, 0);
        }


        private List<Programs> ConvertDataTableToPrograms(int eventId)
        {
            List<Programs> list = new List<Programs>();
            DataTable dt = repo.GetProgramsByEventId(eventId);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Programs
                {
                    ProgramsId = Convert.ToInt32(row["ProgramsId"]),
                    ProgramsName = row["ProgramsName"].ToString(),
                    Duration = Convert.ToInt32(row["Duration"]),
                    EventId = eventId,
                    
                });
            }
            return list;
        }
    }
}
