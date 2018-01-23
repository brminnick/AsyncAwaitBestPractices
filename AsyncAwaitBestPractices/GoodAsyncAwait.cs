using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AsyncAwaitBestPractices
{
    public class GoodAsyncAwait
    {
        #region Fields
		List<PersonModel> _personList = new List<PersonModel>();
        ICommand _getContactsCommand;
        #endregion

        #region Constructors
        public GoodAsyncAwait()
        {
            GetContactsCommand?.Execute(null);
        }
        #endregion

        #region Properties

        ICommand GetContactsCommand => _getContactsCommand ?? (_getContactsCommand = new Command(async () => _personList = await GetPersonModels()));
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
