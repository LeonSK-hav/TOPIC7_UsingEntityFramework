using Lab7_Entity_Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_Entity_Framework
{
    public partial class UpdateFoodForm : Form
    {
        private RestaurantContext _dbContext;
        private int _foodId;
        public UpdateFoodForm(int? foodId = null)
        {
            InitializeComponent();
            _dbContext = new RestaurantContext();
            _foodId = foodId ?? 0;
        }

        private void UpdateFoodForm_Load(object sender, EventArgs e)
        {
            // Nạp danh sách nhóm thức ăn vào combobox
            LoadCategoriesToCombox();
            // Hiển thị thông tin món ăn lên form
            ShowFoodInformation();
        }

        private void LoadCategoriesToCombox()
        {
            // Lây tát cả danh mục thức ăn sắp theo tên
            var categories = _dbContext.Categories.OrderBy(x => x.Name).ToList();
            // Nạp danh mục vào combobox, hiển thị tên cho người dùng xem
            // nhưng khi được chọn thì lấy giá trị là ID
            cbbFoodCategory.DisplayMember = "Name";
            cbbFoodCategory.ValueMember = "Id";
            cbbFoodCategory.DataSource = categories;
        }

        private Food GetFoodById(int foodId)
        {
            // tìm món ăn theo ID
            return foodId > 0 ? _dbContext.Foods.Find(foodId) : null;
        }

        private void ShowFoodInformation()
        {
            // tìm món ăn theo ID đã được truyền vào form
            var food = GetFoodById(_foodId);
            // nếu khong tìm thấy thì thoát
            if (food == null) return;
            // ngược lại  hiển thị thông tin món ăn lên các điều khiển trên form
            txtFoodId.Text = food.Id.ToString();
            txtFoodName.Text = food.Name;
            cbbFoodCategory.SelectedValue = food.FoodCategoryId;
            txtFoodUnit.Text = food.Unit;
            nudFoodPrice.Value = food.Price;
            txtFoodNotes.Text = food.Notes;
        }

        private bool ValidateUserInput()
        {
            // Kiểm tra tên món ăn không được để trống
            if (string.IsNullOrWhiteSpace(txtFoodName.Text))
            {
                MessageBox.Show("Tên món ăn không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            // Kiểm tra đơn vị tính không được để trống
            if (string.IsNullOrWhiteSpace(txtFoodUnit.Text))
            {
                MessageBox.Show("Đơn vị tính không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            // Kiểm tra giá món ăn đã được nhập hay chưa
            if (nudFoodPrice.Value.Equals(0))
            {
                MessageBox.Show("Giá món ăn phải lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            // Kiểm tra nhóm món ăn phải được chọn
            if (cbbFoodCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn nhóm món ăn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        private Food GetUpdatedFood()
        {
            // Tạo một đối tượng món ăn mới từ thông tin từ các điều khiển trên form
            var food = new Food()
            {
                Name = txtFoodName.Text.Trim(),
                FoodCategoryId = (int)cbbFoodCategory.SelectedValue,
                Unit = txtFoodUnit.Text,
                Price = (int)nudFoodPrice.Value,
                Notes = txtFoodNotes.Text
            };
            // Nếu là cập nhật thì gán ID món ăn vào đối tượng
            if (_foodId > 0)
            {
                food.Id = _foodId;
            }
            return food;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // kiểm tra nếu dữ liệu nhâpj vào hợp lệ
            if (ValidateUserInput())
            {
                // thì lấy thông tin người dùng nhập vào
                var newFood = GetUpdatedFood();

                // và thử tìm xem đã có món ăn này trong CSDL chưa
                var oldFood = GetFoodById(_foodId);

                // Nếu chưa có
                if ( oldFood == null)
                {
                    // Thì thêm món ăn mới vào CSDL
                    _dbContext.Foods.Add(newFood);
                }
                else
                {
                    // Ngược lại cập nhật thông tin món ăn cũ
                    oldFood.Name = newFood.Name;
                    oldFood.FoodCategoryId = newFood.FoodCategoryId;
                    oldFood.Unit = newFood.Unit;
                    oldFood.Price = newFood.Price;
                    oldFood.Notes = newFood.Notes;

                }
                // Lưu thay đổi vào CSDL
                _dbContext.SaveChanges();
                // Đóng form và trả về kết quả OK
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
