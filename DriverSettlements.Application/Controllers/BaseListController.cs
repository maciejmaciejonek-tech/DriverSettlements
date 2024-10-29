using DriverSettlements.App.Resources.Localization;
using System.Collections.ObjectModel;
using System.Resources;
using ListView = Microsoft.Maui.Controls.ListView;

namespace DriverSettlements.App.Controllers
{
    public abstract class BaseListController<TObject>
    {
        public bool IncludeDeleted { get; set; } = false;
        public bool ForceDataRefresh { get; set; } = false;

        protected List<TObject> Objects = null;
        protected ListView ListView;

        public bool ListViewInitialized => ListView!=null && ListView.ItemsSource != null;

        public BaseListController() 
        {
        }

        public object SelectedItem => ListView.SelectedItem;

        public bool DataLoaded
        {
            get
            {
                if (ForceDataRefresh)
                {
                    ForceDataRefresh = false;
                    Objects = null;
                }
                return Objects != null;
            }
        }

        public async Task<List<TObject>> GetObjects()
        {
            if (ForceDataRefresh)
            {
                ForceDataRefresh = false;
                Objects = null;
            }
            if (!DataLoaded)
                await LoadData();
            return Objects;
        }

        public void EnsureListViewIsRegistered(ListView listView)
        {
            if (ListView == null || !ListView.Equals(listView))
                ListView = listView;
        }

        public async Task<bool> LoadData()
        {
            Objects = await GetDataFromService();
            return true;
        }

        public void SelectObject(TObject obj)
        {
            ListView.SelectedItem = obj;
        }

        public void RefreshListView()
        {
            if (ListView == null || Objects == null)
                return;
            ListView.ItemsSource = new ObservableCollection<TObject>(Objects);
        }

        public void RefreshListView(Func<List<TObject>, List<TObject>> FilterData)
        {
            if (ListView == null || Objects == null)
                return;
            ListView.ItemsSource = new ObservableCollection<TObject>(FilterData(Objects));
        }

        public async Task<bool> AddObject(TObject obj)
        {
            Objects.Add(await AddObjectToService(obj));
            return true;
        }

        public async Task<bool> UpdateObject(TObject obj)
        {
            await UpdateObjectInService(obj);
            return true;
        }

        public async Task<bool> DeleteObject(TObject obj)
        {
            await DeleteObjectInService(obj);
            Objects.Remove(obj);
            return true;
        }

        public void ClearController()
        {
            ListView = null;
            Objects = null;
        }

        protected abstract Task<List<TObject>> GetDataFromService();
        protected abstract Task<TObject> AddObjectToService(TObject obj);
        protected abstract Task<TObject> UpdateObjectInService(TObject obj);
        protected abstract Task<TObject> DeleteObjectInService(TObject obj);
        protected abstract List<TObject> FilterData(string filerString);
    }
}
