namespace Average.Server.Interfaces
{
    internal interface IMenuItem
    {
        string Name { get; }
        bool Visible { get; }
        MenuContainer Parent { get; set; }
    }
}
