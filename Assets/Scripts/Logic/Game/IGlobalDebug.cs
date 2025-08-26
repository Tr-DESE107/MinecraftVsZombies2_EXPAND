using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGlobalDebug
    {
        void Print(string message);
        string[] GetCommandHistory();
        void ExecuteCommand(string command);
        void ClearConsole();
        NamespaceID[] GetAllCommandsID();
        string GetCommandNameByID(NamespaceID id);
        NamespaceID GetCommandIDByName(string name);
    }
}
