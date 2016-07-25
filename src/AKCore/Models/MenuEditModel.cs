using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class MenuEditModel
    {
        public List<Page> Pages { get; set; }
        public List<ModelMenu> Menus { get; set; }
    }
    public class ModelMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Page Link { get; set; }
        public List<ModelMenu> Children { get; set; }
        public int PosIndex { get; set; }
        public bool LoggedIn { get; set; }

        public ModelMenu(Menu menu)
        {
            Children = new List<ModelMenu>();
            Id = menu.Id;
            Name = menu.Name;
            Link = menu.Link;
            if (menu.Link != null) LoggedIn = menu.Link.LoggedIn;
            PosIndex = menu.PosIndex;
            if (menu.Children == null) return;
            foreach (var m in menu.Children.OrderBy(x=>x.SubPosIndex).ToList())
            {
                Children.Add(new ModelMenu(m));
            }
        }
        public ModelMenu(SubMenu menu)
        {
            Id = menu.Id;
            if (menu.Link != null) LoggedIn = menu.Link.LoggedIn;
            Name = menu.Name;
            Link = menu.Link;
            PosIndex = menu.SubPosIndex;
        }
    }

}