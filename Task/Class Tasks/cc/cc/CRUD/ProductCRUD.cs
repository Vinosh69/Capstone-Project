using cc.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc.CRUD
{
    internal class ProductCRUD
    {
        SqlConnection con = new SqlConnection(
    ConfigurationManager.ConnectionStrings["MyConnection"].ToString()
);

        // ➕ Add Product
        public string AddProduct(Product product)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Products (Name, CateId) VALUES (@Name, @CateId)", con
                );
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@CateId", product.CategId);
                cmd.ExecuteNonQuery();
                return "Product Added Successfully";
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

        // ✏️ Update Product
        public Product UpdateProduct(Product p)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Products SET Name=@Name, CateId=@CateId WHERE ProductID=@ProductID", con
                );
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@CateId", p.CategId);
                cmd.Parameters.AddWithValue("@ProductID", p.ProductId);
                cmd.ExecuteNonQuery();
                return p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // ❌ Delete Product
        public string Delete(int id)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Products WHERE ProductID=@ProductID", con
                );
                cmd.Parameters.AddWithValue("@ProductID", id);
                cmd.ExecuteNonQuery();
                return "Product Deleted Successfully";
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

        // 📄 Get All Products
        public List<Product> GetProductsByCategory(int categId)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Products WHERE CateId = @cid", con);

                cmd.Parameters.AddWithValue("@cid", categId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                List<Product> products = new List<Product>();

                foreach (DataRow dr in dt.Rows)
                {
                    Product p = new Product()
                    {
                        ProductId = int.Parse(dr["ProductID"].ToString()),
                        Name = dr["Name"].ToString(),
                        CategId = int.Parse(dr["CateId"].ToString())
                    };

                    products.Add(p);
                }

                return products;
            }
            finally
            {
                con.Close();
            }
        }
    }
}
