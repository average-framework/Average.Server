namespace Average.Server.Data
{
    public class SQLConnection
    {
        public string Host { get; }
        public int Port { get; }
        public string Database { get; }
        public string Username { get; }
        public string Password { get; }

        public SQLConnection(string host, int port, string database, string username, string password)
        {
            Host = host;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
        }
    }
}
