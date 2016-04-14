using Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PasswordsKeeper.ViewModel
{
    public class CategoryViewModel : WorkspaceViewModel
    {
        public ObservableCollection<Category> CategoryList { get; private set; }
        private CategoryDAO dao;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategorViewModel"/> 
        /// </summary>
        public CategoryViewModel()
        {
            dao = new CategoryDAO();

            LoadData();
        }

        /// <summary>
        /// Loads list from database
        /// </summary>
        private void LoadData()
        {
            List<Category> list = dao.GetAll().ToList();
            CategoryList = new ObservableCollection<Category>(list);
        }
    }
}
