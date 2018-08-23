using System.Collections.Generic;

namespace graph
{
    class Node
    {

        public string name { get; set; } = "defaultName";
        public string group { get; set; } = "defaultGroup";

        public Node(string name)
        {
            this.name = name;
        }
        public Node()
        {    }

        public List<Transaction> exits { get; set; } = new List<Transaction>();

        public override string ToString()
        {
            string result = name + "->[";
            foreach ( Transaction t in exits )
            {
                    result +=  " " + t.ToString() + "; " ;
            }
            result += "]";
            return result;
        }


    }

}    