using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVZ2Logic.Scenes
{
    public interface IDialogDisplayer
    {
        void ShowDialog(string title, string desc, string[] options, Action<int> onSelect);
    }
    public interface IDialogDisplayerAsync
    {
        Task<int> ShowDialogAsync(string title, string desc, string[] options);
    }
}
