using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using De02.Models;

namespace De02
{
    public partial class frmQLSP : Form
    {
        private Model_QLSanpham context;
        private BindingSource bindingSource;

        public frmQLSP()
        {
            InitializeComponent();
            context = new Model_QLSanpham();
            bindingSource = new BindingSource();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                List<LoaiSP> loaiSPs = context.LoaiSPs.ToList();
                List<Sanpham> sanphams = context.Sanphams.ToList();
                FillTypeComboBox(loaiSPs);
                BindDataToGridView(sanphams);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillTypeComboBox(List<LoaiSP> loaiSPs)
        {
            cmbType.DataSource = loaiSPs;
            cmbType.DisplayMember = "TenLoai";
            cmbType.ValueMember = "MaLoai";
        }

        private void BindDataToGridView(List<Sanpham> sanphams)
        {
            dgvLoaiSP.Rows.Clear();
            foreach (Sanpham sanpham in sanphams)
            {
                int index = dgvLoaiSP.Rows.Add();
                dgvLoaiSP.Rows[index].Cells[0].Value = sanpham.MaSP;
                dgvLoaiSP.Rows[index].Cells[1].Value = sanpham.TenSP;
                dgvLoaiSP.Rows[index].Cells[2].Value = sanpham.Ngaynhap;
                dgvLoaiSP.Rows[index].Cells[3].Value = sanpham.LoaiSP.TenLoai;
            }
        }

        private void ClearInput()
        {
            txtID.Text = txtName.Text = string.Empty;
            dtpDate.Value = DateTime.Now;
            cmbType.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Sanpham sanpham = new Sanpham
                {
                    MaSP = txtID.Text,
                    TenSP = txtName.Text,
                    Ngaynhap = dtpDate.Value,
                    MaLoai = cmbType.SelectedValue.ToString()
                };

                context.Sanphams.Add(sanpham);
                btnSave.Enabled = true;
                btnNoSave.Enabled = true;
                BindDataToGridView(context.Sanphams.Include(s => s.LoaiSP).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                List<Sanpham> sanphams = context.Sanphams.Include(s => s.LoaiSP).ToList();

                var sanpham = sanphams.FirstOrDefault(s => s.MaSP == txtID.Text);

                if (sanpham != null)
                {
                    if (sanphams.Any(s => s.MaSP == txtID.Text && s.MaSP != sanpham.MaSP))
                    {
                        MessageBox.Show("Mã sản phẩm đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    sanpham.TenSP = txtName.Text;
                    sanpham.Ngaynhap = dtpDate.Value;
                    sanpham.MaLoai = cmbType.SelectedValue.ToString();
                    btnSave.Enabled = true;
                    btnNoSave.Enabled = true;
                    BindDataToGridView(sanphams);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<Sanpham> sanphams = context.Sanphams.Include(s => s.LoaiSP).ToList();

                var sanpham = sanphams.FirstOrDefault(s => s.MaSP == txtID.Text);
                if (sanpham != null)
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        return;
                    }

                    context.Sanphams.Remove(sanpham);
                    btnSave.Enabled = true;
                    btnNoSave.Enabled = true;
                    BindDataToGridView(sanphams);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                context.SaveChanges();
                MessageBox.Show("Lưu thay đổi thành công");
                ClearInput();
                btnSave.Enabled = false;
                btnNoSave.Enabled = false;
                BindDataToGridView(context.Sanphams.Include(s => s.LoaiSP).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNoSave_Click(object sender, EventArgs e)
        {
            try
            {
                context = new Model_QLSanpham();
                MessageBox.Show("Không lưu thay đổi thành công");
                ClearInput();
                btnSave.Enabled = false;
                btnNoSave.Enabled = false;
                BindDataToGridView(context.Sanphams.Include(s => s.LoaiSP).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát chương trình?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void dgvLoaiSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLoaiSP.Rows[e.RowIndex];
                txtID.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                dtpDate.Value = Convert.ToDateTime(row.Cells[2].Value);
                cmbType.Text = row.Cells[3].Value.ToString();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = txtSearch.Text.ToLower();
                List<Sanpham> sanphams = context.Sanphams.Include(s => s.LoaiSP).ToList();

                var filteredSanphams = sanphams.Where(sp =>
                    sp.MaSP.ToLower().Contains(search) ||
                    sp.TenSP.ToLower().Contains(search) ||
                    sp.LoaiSP.TenLoai.ToLower().Contains(search)).ToList();

                BindDataToGridView(filteredSanphams);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
