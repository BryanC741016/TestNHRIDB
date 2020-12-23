using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHRIDB.Models.ViewModels
{
    public class FormViewModel
    {
        public Node json;
    }
    public class Node
    {
        public Guid id;
        public Nullable<Guid> parentId;
        public string text;
        public List<Node> children;
        public string type;
    }
}