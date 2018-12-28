using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLayer
{
    public class Menu_Link_Model
    {
        private string _linkText;
        private string _linkIcon;
        private string _linkHref;

        public Menu_Link_Model(string linkHref, string linkIcon, string linkText)
        {
            this.linkHref = linkHref;
            this.linkIcon = linkIcon;
            this.linkText = linkText;
        }

        public string linkText
        {
            get { return _linkText; }
            set { _linkText = value; }
        }

        public string linkIcon
        {
            get { return _linkIcon; }
            set { _linkIcon = value; }
        }

        public string linkHref
        {
            get { return _linkHref; }
            set { _linkHref = value; }
        }
    }
}
