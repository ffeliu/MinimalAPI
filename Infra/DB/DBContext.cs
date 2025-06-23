namespace minimalapi.domain.db;

public class DBContext
{
    public string ConnectionString { get; set; }

    public DBContext(string connectionString)
    {
        ConnectionString = connectionString;
    }
}