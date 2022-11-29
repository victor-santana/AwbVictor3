using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace AwbRepository
{
    public class AwbRepository
    {
        private static SqlConnection ConfigBanco()
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
           .AddJsonFile("C://Users//victor.santana//source//repos//AwbVictor3//AwbVictor3//appsettings.json").Build();
            var stringConection = config.GetConnectionString("BancoDeDados");
            SqlConnection conexao = new SqlConnection(stringConection);            

            return conexao;
        }

        public static string? SelectBancoDeDados()
        {
            var conexao = ConfigBanco();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Awb", conexao);

            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();

                //metodo
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(ds);

                //metodo
                var dsJson = JsonConvert.SerializeObject(ds);
                var dsJsonClass = JsonConvert.DeserializeObject<AwbEntity>(dsJson);

                return dsJson;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possivel conectar", ex);
                return null;

            }
            finally
            {
                conexao.Close();
            }

        }

        public static string? InsertBancoDeDados(string body)
        {
            //Banco de dados:

            AwbEntityWriter entityClass = JsonConvert.DeserializeObject<AwbEntityWriter>(body);

            var conexao = ConfigBanco();
            var query = string.Format("INSERT INTO Awb([PackingShippingNumber],[CratedDate],[PackedDate],[ReceivedDate],[ShippedDate],[Timestamp])" +
                "values({0},'{1}', '{2}', '{3}', '{4}', '{5}');",
                $"{entityClass.PackingShippingNumber}",
                $"{entityClass.CratedDate}",
                $"{entityClass.PackedDate}",
                $"{entityClass.ReceivedDate}",
                $"{entityClass.ShippedDate}",
                $"{entityClass.Timestamp}");
            SqlCommand cmd = new SqlCommand(query, conexao);

            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();

                return "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possivel conectar", ex);
                return null;

            }
            finally
            {
                conexao.Close();
            }

        }

    }
}
