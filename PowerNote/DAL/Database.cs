using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient; //NOTE: should learn a bit about this library.
using System.Configuration;

namespace PowerNote {
    class Database {
        String databaseName = "PowerNoteDatabase";
        SqlConnection myConn; //this needs to stay.
        String sqlStatement;
        String connectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=C:\\Users\\Chris\\Documents\\PowerNoteDatabase.mdf;Integrated Security=True;Connect Timeout=15;User Instance=True";
        //Above was copied from the wizard, so should work... But doesn't. SO i give up, I'll use the wizard.
        //String connectionString = "server=localhost;database=myDb;uid=myUser;password=myPass;"; //Nope
        //String connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString; //Nope
        //String connectionString = PowerNote.Properties.Settings.Default.ConnectionString; //Nope.
        //String connectionString = "Server=localhost;Integrated security=SSPI;database=master"; //This simply does not work! Won't even open a connection. (pretty sure, unless XAML is interfering) Thanks internet!
        //String connectionString = "data source=(local)\\SQLEXPRESS;Integrated security=SSPI; AttachDBFilename=|DataDirectory|\\Database.mdf; User instance=true"; //Works for App_Data database.
        //String connectionString = "Server=tcp:h7t5fvxi6f.database.windows.net; Database=Database; User ID=ct8356;Password=Monkeyscomb1;Trusted_Connection=False;Encrypt=True;"; //Works for remote database.
        SqlCommandBuilder builder;
        String escapedDatabaseName;

        public Database() {
            //nothing
        }

        public String addColumns(String tableName, List<String> columnNames) { //add columns to table
            var builder = new SqlCommandBuilder();
            var escapedTableName = builder.QuoteIdentifier(tableName);
            var escapedColumnNames = new List<String>();
            var escapedColumnNamesAndMetadata = new List<String>();
            for (int i = 0; i < columnNames.Count; i += 1) { // for each columnName in array
                escapedColumnNames.Add(builder.QuoteIdentifier(columnNames[i]));
                escapedColumnNamesAndMetadata.Add(escapedColumnNames[i] + " NVARCHAR(255)"); //Must allow nulls. SQL requirement.
            } //it works!
            var sqlStatement = String.Format("ALTER TABLE {0} ADD {1}", escapedTableName, String.Join<String>(", ", escapedColumnNamesAndMetadata));
            var myCommand = new SqlCommand(sqlStatement, myConn);
            myCommand.ExecuteNonQuery();
            return sqlStatement;
        }

        public void createDatabase() { //Broken. Don't use. 
            //(use the wizard instead. Just go Database Explorer "add a connection" and use "new" database name).
            using (SqlConnection creationConn = new SqlConnection(connectionString)) {
                SqlCommand command = creationConn.CreateCommand();
                //Ah, cool, so SQLStatement is not even needed...
                command.CommandText = "CREATE DATABASE mydb";
                command.ExecuteNonQuery();
            }
        }

        public String createTable() {
            using (SqlConnection myConn = new SqlConnection(connectionString)) {
                SqlCommand command = myConn.CreateCommand();
                //Ah, cool, so SQLStatement is not even needed...
                command.CommandText = "CREATE TABLE mytable (id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, mycolname NVARCHAR(255) NOT NULL)";
                command.ExecuteNonQuery();
                return sqlStatement;
            }
        }

        public String createTableWithList(String tableName, List<String> columnNames) {
            var builder = new SqlCommandBuilder();
            var escapedTableName = builder.QuoteIdentifier(tableName);
            var escapedColumnNames = new List<String>();
            var escapedColumnNamesAndMetadata = new List<String>();
            var parameters = new List<String>();
            for (int i = 0; i < columnNames.Count; i += 1) {
                escapedColumnNames.Add(builder.QuoteIdentifier(columnNames[i]));
                escapedColumnNamesAndMetadata.Add(escapedColumnNames[i] + " NVARCHAR(255) NOT NULL");
            } //it works! Possible could do this without for loop? No, because builder will only take 1 string at a time.
            var sqlStatement = String.Format("CREATE TABLE {0}(id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, {1})", escapedTableName, String.Join<String>(", ", escapedColumnNamesAndMetadata));
            var myCommand = new SqlCommand(sqlStatement, myConn);
            myCommand.ExecuteNonQuery();
            return sqlStatement;
        }

        public void dropDatabase(String databaseName) { //not working. Don't use it.
            builder = new SqlCommandBuilder();
            escapedDatabaseName = builder.QuoteIdentifier(databaseName);
            myConn = new SqlConnection();
            myConn.Open();
            sqlStatement = "DROP DATABASE " + escapedDatabaseName;
            SqlCommand myCommand = new SqlCommand(sqlStatement, myConn);
            myCommand.ExecuteNonQuery();
            if (myConn.State == ConnectionState.Open) {
                myConn.Close();
            }
        }

        public void dropTable(String tableName) {
            var builder = new SqlCommandBuilder();
            var escapedTableName = builder.QuoteIdentifier(tableName);
            var sqlStatement = "DROP TABLE " + escapedTableName;
            var myCommand = new SqlCommand(sqlStatement, myConn);
            myCommand.ExecuteNonQuery();
        }

        public void openConnection() {
            myConn = new SqlConnection(connectionString);//No risk of Injection attack here, I think.
            myConn.Open();
        }

        public void closeConnection() {
            if (myConn.State == ConnectionState.Open) {
                myConn.Close();
            }
        }
    
    }
}
