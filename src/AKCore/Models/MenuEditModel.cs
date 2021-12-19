using AKCore.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Models
{
    public class MenuEditModel
    {
        public IList<Page> Pages { get; set; }
        public IList<ModelMenu> Menus { get; set; }
    }
    public class ModelMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int LinkId { get; set; }
        public List<ModelMenu> Children { get; set; }
        public int PosIndex { get; set; }
        public bool LoggedIn { get; set; }
        public bool MenuLoggedIn { get; set; }
        public bool MenuBalett { get; set; }

        public ModelMenu(Menu menu, bool userLoggedIn)
        {
            Children = new List<ModelMenu>();
            Id = menu.Id;
            Name = menu.Name;
            MenuLoggedIn = menu.LoggedIn;
            MenuBalett = menu.Balett;
            Link = menu.Link?.Slug ?? "";
            LinkId = menu.Link?.Id ?? 0;
            if (menu.Link != null)
            {
                LoggedIn = menu.Link.LoggedIn;
            }

            PosIndex = menu.PosIndex;
            if (menu.Children == null)
            {
                return;
            }

            foreach (var m in menu.Children.Where(x => userLoggedIn || !x.Link.LoggedIn).OrderBy(x => x.SubPosIndex).ToList())
            {
                Children.Add(new ModelMenu(m));
            }
        }
        public ModelMenu(SubMenu menu)
        {
            Id = menu.Id;
            if (menu.Link != null)
            {
                LoggedIn = menu.Link.LoggedIn;
            }

            LinkId = menu.Link?.Id ?? 0;
            Name = menu.Name;
            if (menu.Link != null)
            {
                Link = menu.Link.Slug;
            }

            PosIndex = menu.SubPosIndex;
        }

        public ModelMenu(string name, string slug, bool loggedIn)
        {
            Name = name;
            Link = slug;
            LoggedIn = loggedIn;
            Children = new List<ModelMenu>();
        }
    }

}