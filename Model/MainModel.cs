using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MainModel
    {
        // Temporary products list used in the instance of this application
        public List<Product> Products = new List<Product>();
        // Event to invoke when an error occurs
        public event Action<string> ShowErrorMessageEvent;

        /// <summary>
        /// Send updated product to the database.
        /// </summary>
        /// <param name="product">Product to update</param>
        public async Task UpdateProductInDatabaseAsync(Product product)
        {
            try
            {
                using (ExampleDatabaseContext database = new ExampleDatabaseContext())
                {
                    Product updatingProduct = await database.Products.FindAsync(product.Id);
                    if (updatingProduct == null)
                    {
                        ShowErrorMessageEvent?.Invoke("Product doesn't exist in the database anymore.");
                        return;
                    }
                    updatingProduct.Name = product.Name;
                    updatingProduct.Count = product.Count;
                    updatingProduct.Value = product.Value;
                    await database.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                ShowErrorMessageEvent?.Invoke("Exception occured while updating product:" + Environment.NewLine + e.Message);
            }
        }

        /// <summary>
        /// Method loads and returns an products list from the file in <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path to the csv file</param>
        /// <returns>List of loaded products. Null when error occured.</returns>
        private async Task<List<Product>> LoadProductsFromCsvAsync(string path)
        {
            if (!File.Exists(path))
            {
                ShowErrorMessageEvent?.Invoke("File doesn't exist");
                return null;
            }

            // if file exists try to load products data
            List<Product> loadedProducts = new List<Product>();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(';');
                        Product newProduct = new Product()
                        {
                            Name = values[0],
                            Count = int.Parse(values[1]),
                            Value = double.Parse(values[2])
                        };
                        loadedProducts.Add(newProduct);
                    }
                }

                return loadedProducts;
            }
            catch(Exception e)
            {
                ShowErrorMessageEvent?.Invoke("Incorrect products data in file: " + Environment.NewLine + path);
                // on any exception return null
                return null;
            }
        }

        /// <summary>
        /// Load products from the file in <paramref name="path"/>.
        /// Save loaded data to the database when loading succeeded.
        /// </summary>
        /// <param name="path">Path to the csv file</param>
        /// <returns>Succeeded</returns>
        public async Task<bool> LoadProductsAsync(string path)
        {
            List<Product> newProducts = await LoadProductsFromCsvAsync(path);
            if (newProducts == null)
                return false;

            try
            {
                using (ExampleDatabaseContext database = new ExampleDatabaseContext())
                {
                    if (database.Products.Count() > 0)
                    {
                        IQueryable<Product> databaseProducts = from o in database.Products
                                                                select o;
                        foreach (Product procuct in databaseProducts)
                            database.Products.Remove(procuct);
                    }
                    database.Products.AddRange(newProducts);
                    database.SaveChanges();

                    Products = new List<Product>(database.Products.AsEnumerable());
                }
            }
            catch(Exception e)
            {
                ShowErrorMessageEvent?.Invoke("Exception occured while operating on database:" + Environment.NewLine + e.Message);
                // on database operation exception return false
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generate data with sample products and save it to ./exampleData.csv
        /// </summary>
        public static void GenerateExampleCsvFile()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(string.Format("{0};{1};{2}", "First product", 3, 4.55))
                .AppendLine(string.Format("{0};{1};{2}", "Second product", 1, 0.99))
                .AppendLine(string.Format("{0};{1};{2}", "Third product", 0, 12))
                .AppendLine(string.Format("{0};{1};{2}", "Forth product", 12, 586))
                .AppendLine(string.Format("{0};{1};{2}", "Fifth product", 10, 3.01));
            try
            {
                File.WriteAllText("./exampleData.csv", csv.ToString());
            }
            catch { }
        }
    }
}
