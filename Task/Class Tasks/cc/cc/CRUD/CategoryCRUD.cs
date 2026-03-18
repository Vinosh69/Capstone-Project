using cc.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace cc.CRUD
{
    internal class CategoryCRUD
    {
        SqlConnection con = new SqlConnection(
            ConfigurationManager.ConnectionStrings["MyConnection"].ToString()
        );

        // ➕ Add Category
        public string AddCategory(Category c)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Categories ([Name]) VALUES (@Name)", con
                );
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.ExecuteNonQuery();
                return "Category Added Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        // ✏️ Update Category
        public string UpdateCategory(Category c)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Categories SET [Name]=@Name WHERE Id=@Id", con
                );
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Id", c.Id);
                cmd.ExecuteNonQuery();
                return "Category Updated Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        // ❌ Delete Category
        public string DeleteCategory(int id)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Categories WHERE Id=@Id", con
                );
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
                return "Category Deleted Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        // 📄 Get All Categories
        public List<Category> GetAllCategories()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Categories", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<Category> list = new List<Category>();

            foreach (DataRow row in dt.Rows)
            {
                Category c = new Category();
                c.Id = int.Parse(row["Id"].ToString());
                c.Name = row["Name"].ToString();
                list.Add(c);
            }

            return list;
        }
    }
}