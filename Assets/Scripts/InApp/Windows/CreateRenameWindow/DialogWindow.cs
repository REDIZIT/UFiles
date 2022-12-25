using System.Threading.Tasks;
using UnityEngine;

namespace InApp.UI
{
    public abstract class DialogWindow<TModel, TAnswer> : Window<TModel>
    {
        public async Task<TAnswer> ShowDialog(TModel model)
        {
            Show(model);
            return await GetAnswer();
        }
        protected abstract Task<TAnswer> GetAnswer();
    }
}