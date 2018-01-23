using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AsyncAwaitBestPractices
{
    public class BadAsyncAwait
    {
        #region Fields
        List<PersonModel> _personList = new List<PersonModel>();
        #endregion

        #region Constructors
        public BadAsyncAwait()
        {
            GetPersonModels();
        }
        #endregion

        #region Methods
        public async Task<List<PersonModel>> GetPersonModels()
        {
            if (_personList.Count == 0)
                _personList = await DocumentDbService.GetAll<PersonModel>();

            return _personList;
        }
        #endregion
    }
}
