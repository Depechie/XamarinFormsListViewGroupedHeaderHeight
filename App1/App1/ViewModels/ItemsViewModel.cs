using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace App1
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Grouping<string, Item>> items;
        public ObservableCollection<Grouping<string, Item>> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Grouping<string, Item>>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                //Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);

                var groupedItems = from item in items
                                   orderby item.Text
                                   group item by item.Text[0].ToString() into grouped
                                   select new Grouping<string, Item>(grouped.Key, grouped);

                Items = new ObservableCollection<Grouping<string, Item>>(groupedItems);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
